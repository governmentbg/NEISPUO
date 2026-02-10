namespace SB.Domain;

using Medallion.Threading;
using SB.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IOffDaysQueryRepository;

class OffDayRebuildService : IOffDayRebuildService
{
    private Dictionary<(int, int), GetAllForRebuildVO[]> offDays = new();

    private IUnitOfWork unitOfWork;
    private IOffDaysQueryRepository offDaysQueryRepository;
    private IClassBookOffDayDatesAggregateRepository classBookOffDayDatesAggregateRepository;
    private ISchedulesAggregateRepository schedulesAggregateRepository;
    private Func<string, IDbTransaction, bool, IDistributedLock> lockFactory;

    public OffDayRebuildService(
        IUnitOfWork unitOfWork,
        IOffDaysQueryRepository offDaysQueryRepository,
        IClassBookOffDayDatesAggregateRepository classBookOffDayDatesAggregateRepository,
        ISchedulesAggregateRepository schedulesAggregateRepository,
        Func<string, IDbTransaction, bool, IDistributedLock> lockFactory)
    {
        this.unitOfWork = unitOfWork;
        this.offDaysQueryRepository = offDaysQueryRepository;
        this.classBookOffDayDatesAggregateRepository = classBookOffDayDatesAggregateRepository;
        this.schedulesAggregateRepository = schedulesAggregateRepository;
        this.lockFactory = lockFactory;
    }

    public async Task RebuildAndSaveAsync(int schoolYear, int instId, int? exceptOffDayId, ITransaction transaction, CancellationToken ct)
    {
        using var lockReleaseHandle = await this.TryAcquireDistributedLockAsync(
            $"OffDayRebuild:{schoolYear}:{instId}",
            transaction.GetDbTransaction(),
            true,
            TimeSpan.FromSeconds(5),
            ct);

        if (lockReleaseHandle == null)
        {
            // we couldn't take the lock
            throw new DomainValidationException(
                new[] { "lock_acquire_failed" },
                new[] { "Някой друг работи с неучебните дни, моля изчакайте и опитайте отново." });
        }

        var offDays = await this.GetAllForRebuildAsync(schoolYear, instId, ct);
        if (exceptOffDayId.HasValue)
        {
            offDays = offDays.Where(od => od.OffDayId != exceptOffDayId.Value).ToArray();
        }

        var allClassesOffDays = offDays.Where(od => od.IsForAllClasses).ToArray();
        var specificOffDays = offDays.Where(od => !od.IsForAllClasses).ToArray();

        await this.ValidateAsync(instId, allClassesOffDays, specificOffDays, ct);

        var classBooks = await this.offDaysQueryRepository.GetAllClassBooksAsync(schoolYear, instId, ct);

        var specificOffDaysByBasicClassId =
            (from basicClassId in classBooks.Where(cb => cb.BasicClassId != null).Select(cb => cb.BasicClassId).Distinct()
            join od in specificOffDays.SelectMany(od2 => od2.BasicClassIds.Select(bcId => (bcId, od: od2)))
            on basicClassId equals od.bcId
            select od)
            .ToLookup(od => od.bcId, od => od.od);

        var specificOffDaysByClassBookId =
            (from classBookId in classBooks.Select(cb => cb.ClassBookId)
            join od in specificOffDays.SelectMany(od2 => od2.ClassBookIds.Select(cbId => (cbId, od: od2)))
            on classBookId equals od.cbId
            select od)
            .ToLookup(od => od.cbId, od => od.od);

        List<ClassBookOffDayDate> @new = new();
        foreach (var cb in classBooks)
        {
            var specificOffDaysForClassBook = specificOffDaysByClassBookId[cb.ClassBookId];
            if (cb.BasicClassId != null)
            {
                specificOffDaysForClassBook =
                    specificOffDaysForClassBook.Union(
                        specificOffDaysByBasicClassId[cb.BasicClassId.Value]);
            }

            @new.AddRange(
                this.CreateForClassBookInt(
                    schoolYear,
                    cb.ClassBookId,
                    allClassesOffDays,
                    specificOffDaysForClassBook.ToArray()));
        }

        var old = await this.classBookOffDayDatesAggregateRepository.FindAllByInstitutionAsync(schoolYear, instId, ct);

        static (int, DateTime) keySelector(ClassBookOffDayDate od) => (od.ClassBookId, od.Date);
        static IEnumerable<ClassBookOffDayDate> except(IEnumerable<ClassBookOffDayDate> first, IEnumerable<ClassBookOffDayDate> second)
            => first.ExceptBy(second.Select(keySelector), keySelector);
        static IEnumerable<(ClassBookOffDayDate, ClassBookOffDayDate)> join(IEnumerable<ClassBookOffDayDate> first, IEnumerable<ClassBookOffDayDate> second)
            => first.Join(second, keySelector, keySelector, (f, s) => (f, s));

        var forCreation = except(@new, old);
        var forRemoval = except(old, @new);
        var forUpdate = join(old, @new);

        foreach (var od in forCreation)
        {
            await this.classBookOffDayDatesAggregateRepository.AddAsync(
                entity: od,
                preventDetectChanges: true,
                ct: ct);

            await this.schedulesAggregateRepository.RemoveScheduleLessonsForDateAsync(schoolYear, instId, od.ClassBookId, od.Date, ct);
        }

        foreach (var od in forRemoval)
        {
            this.classBookOffDayDatesAggregateRepository.Remove(od, preventDetectChanges: true);
            var schedules = await this.schedulesAggregateRepository.FindSchedulesByDateAsync(schoolYear, od.ClassBookId, od.Date, ct);

            foreach (var schedule in schedules)
            {
                schedule.AddLessonsForDate(od.Date);
            }
        }

        foreach (var (oldOd, newOd) in forUpdate)
        {
            oldOd.UpdateData(
                newOd.OffDayId,
                newOd.IsPgOffProgramDay);
        }

        await this.unitOfWork.SaveAsync(ct);
    }

    public async Task<ClassBookOffDayDate[]> CreateForClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? basicClassId,
        CancellationToken ct)
    {
        // the classBookId is always for a newly created ClassBook
        // so we dont need to query for OffDayClassBooks

        var offDays =
            await this.GetAllForRebuildAsync(
                schoolYear,
                instId,
                ct);

        var allClassesOffDays = offDays.Where(od => od.IsForAllClasses).ToArray();
        var specificOffDays = basicClassId != null ?
            offDays
                .Where(od =>
                    !od.IsForAllClasses &&
                    od.BasicClassIds.Contains(basicClassId.Value))
                .ToArray()
            : Array.Empty<GetAllForRebuildVO>();

        return this.CreateForClassBookInt(
            schoolYear,
            classBookId,
            allClassesOffDays,
            specificOffDays);
    }

    private ClassBookOffDayDate[] CreateForClassBookInt(
        int schoolYear,
        int classBookId,
        GetAllForRebuildVO[] allClassesOffDays,
        GetAllForRebuildVO[] specificOffDays)
    {
        Dictionary<DateTime, (int offDayId, bool isPgOffProgramDay)> result = new();

        foreach (var od in allClassesOffDays)
        {
            for (var date = od.From; date <= od.To; date = date.AddDays(1))
            {
                result.AddOrUpdate(date, (od.OffDayId, od.IsPgOffProgramDay));
            }
        }

        foreach (var od in specificOffDays)
        {
            for (var date = od.From; date <= od.To; date = date.AddDays(1))
            {
                result.AddOrUpdate(date, (od.OffDayId, od.IsPgOffProgramDay));
            }
        }

        return result.Select(
            kvp => new ClassBookOffDayDate(
                schoolYear,
                classBookId,
                kvp.Key,
                kvp.Value.offDayId,
                kvp.Value.isPgOffProgramDay))
            .ToArray();
    }

    private ValueTask<IDistributedSynchronizationHandle?> TryAcquireDistributedLockAsync(
        string name,
        IDbTransaction transaction,
        bool exactName,
        TimeSpan timeout,
        CancellationToken ct)
        => this.lockFactory(name, transaction, exactName).TryAcquireAsync(timeout, ct);

    private async Task ValidateAsync(
        int instId,
        GetAllForRebuildVO[] allClassesOffDays,
        GetAllForRebuildVO[] specificOffDays,
        CancellationToken ct)
    {
        if (allClassesOffDays.Any(od => od.BasicClassIds.Any() || od.ClassBookIds.Any()))
        {
            throw new DomainValidationException("OffDays with IsForAllClasses=True should not have BasicClassIds or ClassBookIds.");
        }
        if (specificOffDays.Any(od => od.BasicClassIds.Any() && od.ClassBookIds.Any()))
        {
            throw new DomainValidationException("OffDays with IsForAllClasses=False should not have both BasicClassIds and ClassBookIds.");
        }

        static bool areIntersecting(GetAllForRebuildVO o1, GetAllForRebuildVO o2)
            => (o1.From <= o2.To) &&
                (o1.To >= o2.From) &&
                // to prevent duplicates as the cartesian product
                // will contain both (o1, o2) and (o2, o1)
                (o1.OffDayId < o2.OffDayId);
        static (DateTime start, DateTime end) getIntersection(GetAllForRebuildVO o1, GetAllForRebuildVO o2)
            => (o1.From > o2.From ? o1.From : o2.From, o1.To < o2.To ? o1.To : o2.To);
        static bool areBasicClassIntersecting(GetAllForRebuildVO o1, GetAllForRebuildVO o2)
            => areIntersecting(o1, o2) && o1.BasicClassIds.Intersect(o2.BasicClassIds).Any();
        static bool areClassBookIntersecting(GetAllForRebuildVO o1, GetAllForRebuildVO o2)
            => areIntersecting(o1, o2) && o1.ClassBookIds.Intersect(o2.ClassBookIds).Any();
        static (DateTime start, DateTime end, int[] basicClassIds) getBasicClassIntersection(GetAllForRebuildVO o1, GetAllForRebuildVO o2)
        {
            (DateTime start, DateTime end) = getIntersection(o1, o2);
            return (start, end, o1.BasicClassIds.Intersect(o2.BasicClassIds).ToArray());
        }
        static (DateTime start, DateTime end, int[] classBookIds) getClassBookIntersection(GetAllForRebuildVO o1, GetAllForRebuildVO o2)
        {
            (DateTime start, DateTime end) = getIntersection(o1, o2);
            return (start, end, o1.ClassBookIds.Intersect(o2.ClassBookIds).ToArray());
        }
        static (GetAllForRebuildVO o1, GetAllForRebuildVO o2)[] findAllIntersections(
            GetAllForRebuildVO[] offDays,
            Func<GetAllForRebuildVO, GetAllForRebuildVO, bool> condition)
            => (from o1 in offDays
                from o2 in offDays
                where condition(o1, o2)
                select (o1, o2))
                .ToArray();

        var intesectingAllClassesOffDays = findAllIntersections(allClassesOffDays, areIntersecting);
        if (intesectingAllClassesOffDays.Any())
        {
            string[] errors = intesectingAllClassesOffDays.Select(
                i =>
                {
                    (DateTime start, DateTime end) = getIntersection(i.o1, i.o2);
                    return $"The OffDays {i.o1.OffDayId} and {i.o2.OffDayId} are intersecting for the period {start:dd.MM.yyyy}-{end:dd.MM.yyyy}.";
                }).ToArray();
            string[] errorMessages = intesectingAllClassesOffDays.Select(
                i =>
                {
                    (DateTime start, DateTime end) = getIntersection(i.o1, i.o2);

                    string dateStr;
                    if (start.Date == end.Date)
                    {
                        dateStr = $"датата {start:dd.MM.yyyy}";
                    }
                    else
                    {
                        dateStr = $"дните {start:dd.MM.yyyy}-{end:dd.MM.yyyy}";
                    }

                    return $"Неучебните дни {i.o1.Description} и {i.o2.Description} се засичат за {dateStr}.";
                }).ToArray();

            throw new DomainValidationException(errors, errorMessages);
        }

        var basicClassIntesectingSpecificOffDays = findAllIntersections(specificOffDays, areBasicClassIntersecting);
        if (basicClassIntesectingSpecificOffDays.Any())
        {
            var basicClasses = await this.offDaysQueryRepository.GetAllBasicClassNamesAsync(ct);
            var basicClassNamesDict = basicClasses.ToDictionary(bc => bc.BasicClassId, bc => bc.Name);

            string[] errors = basicClassIntesectingSpecificOffDays.Select(
                i =>
                {
                    (DateTime start, DateTime end, int[] basicClassIds) = getBasicClassIntersection(i.o1, i.o2);
                    return $"The OffDays {i.o1.OffDayId} and {i.o2.OffDayId} have intersecting BasicClassIds for the period {start:dd.MM.yyyy}-{end:dd.MM.yyyy}.";
                }).ToArray();
            string[] errorMessages = basicClassIntesectingSpecificOffDays.Select(
                i =>
                {
                    (DateTime start, DateTime end, int[] basicClassIds) = getBasicClassIntersection(i.o1, i.o2);

                    string dateStr;
                    if (start.Date == end.Date)
                    {
                        dateStr = $"датата {start:dd.MM.yyyy}";
                    }
                    else
                    {
                        dateStr = $"дните {start:dd.MM.yyyy}-{end:dd.MM.yyyy}";
                    }

                    return $"Неучебните дни {i.o1.Description} и {i.o2.Description} се засичат за {dateStr} за {(basicClassIds.Length > 1 ? "випуските" : "випуска")} {basicClassIds.Select(bcId => basicClassNamesDict[bcId]).JoinAsNaturalLanguageList()}.";
                }).ToArray();

            throw new DomainValidationException(errors, errorMessages);
        }

        var classBookIntesectingSpecificOffDays = findAllIntersections(specificOffDays, areClassBookIntersecting);
        if (classBookIntesectingSpecificOffDays.Any())
        {
            var classBooks = await this.offDaysQueryRepository.GetAllClassBookNamesAsync(instId, ct);
            var classBooksNamesDict = classBooks.ToDictionary(cb => cb.ClassBookId, bc => bc.FullBookName);

            string[] errors = classBookIntesectingSpecificOffDays.Select(
                i =>
                {
                    (DateTime start, DateTime end, int[] classBookIds) = getClassBookIntersection(i.o1, i.o2);
                    return $"The OffDays {i.o1.OffDayId} and {i.o2.OffDayId} have intersecting ClassBookIds for the period {start:dd.MM.yyyy}-{end:dd.MM.yyyy}.";
                }).ToArray();
            string[] errorMessages = classBookIntesectingSpecificOffDays.Select(
                i =>
                {
                    (DateTime start, DateTime end, int[] classBookIds) = getClassBookIntersection(i.o1, i.o2);

                    string dateStr;
                    if (start.Date == end.Date)
                    {
                        dateStr = $"датата {start:dd.MM.yyyy}";
                    }
                    else
                    {
                        dateStr = $"дните {start:dd.MM.yyyy}-{end:dd.MM.yyyy}";
                    }

                    return $"Неучебните дни {i.o1.Description} и {i.o2.Description} се засичат за {dateStr} за {(classBookIds.Length > 1 ? "групите/паралелките" : "групата/паралелката")} {classBookIds.Select(bcId => classBooksNamesDict[bcId]).JoinAsNaturalLanguageList()}.";
                }).ToArray();

            throw new DomainValidationException(errors, errorMessages);
        }
    }

    private async Task<GetAllForRebuildVO[]> GetAllForRebuildAsync(int schoolYear, int instId, CancellationToken ct)
    {
        // this class is not supposed to be called concurrently
        // so we dont need any synchronization

        if (this.offDays.TryGetValue((schoolYear, instId), out var cachedOffDays))
        {
            return cachedOffDays;
        }

        var offDays = await this.offDaysQueryRepository.GetAllForRebuildAsync(schoolYear, instId, ct);

        this.offDays.Add((schoolYear, instId), offDays);

        return offDays;
    }
}
