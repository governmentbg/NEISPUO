namespace SB.Domain;

using Medallion.Threading;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class SchoolYearSettingsRebuildService : ISchoolYearSettingsRebuildService
{
    private IUnitOfWork unitOfWork;
    private ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository;
    private IClassBookSchoolYearSettingsAggregateRepository classBookSchoolYearSettingsAggregateRepository;
    private ISchoolYearSettingsProvider schoolYearSettingsProvider;
    private IClassBookCachedQueryStore classBookCachedQueryStore;
    private Func<string, IDbTransaction, bool, IDistributedLock> lockFactory;

    public SchoolYearSettingsRebuildService(
        IUnitOfWork unitOfWork,
        ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository,
        IClassBookSchoolYearSettingsAggregateRepository classBookSchoolYearSettingsAggregateRepository,
        ISchoolYearSettingsProvider schoolYearSettingsProvider,
        IClassBookCachedQueryStore classBookCachedQueryStore,
        Func<string, IDbTransaction, bool, IDistributedLock> lockFactory)
    {
        this.unitOfWork = unitOfWork;
        this.schoolYearSettingsQueryRepository = schoolYearSettingsQueryRepository;
        this.classBookSchoolYearSettingsAggregateRepository = classBookSchoolYearSettingsAggregateRepository;
        this.schoolYearSettingsProvider = schoolYearSettingsProvider;
        this.classBookCachedQueryStore = classBookCachedQueryStore;
        this.lockFactory = lockFactory;
    }

    public async Task RebuildAndSaveAsync(int schoolYear, int instId, ITransaction transaction, CancellationToken ct)
    {
        using var lockReleaseHandle = await this.TryAcquireDistributedLockAsync(
            $"SchoolYearSettingsRebuild:{schoolYear}:{instId}",
            transaction.GetDbTransaction(),
            true,
            TimeSpan.FromSeconds(5),
            ct);

        if (lockReleaseHandle == null)
        {
            // we couldn't take the lock
            throw new DomainValidationException(
                new[] { "lock_acquire_failed" },
                new[] { "Някой друг работи с настройките на учебната година, моля изчакайте и опитайте отново." });
        }

        var classBooks = await this.schoolYearSettingsQueryRepository.GetAllClassBooksAsync(schoolYear, instId, ct);
        var classBookSettings = await this.classBookSchoolYearSettingsAggregateRepository.FindAllByInstitutionAsync(schoolYear, instId, ct);
        var classBookSettingsDict = classBookSettings.ToDictionary(d => d.ClassBookId);

        foreach (var classBook in classBooks)
        {
            var sys = await this.schoolYearSettingsProvider.GetForClassBookAsync(
                schoolYear,
                instId,
                classBook.ClassBookId,
                classBook.BasicClassId,
                classBook.ChildBasicClassIds,
                ct);

            classBookSettingsDict[classBook.ClassBookId].UpdateData(
                sys.SchoolYearSettingsId,
                sys.SchoolYearStartDateLimit,
                sys.SchoolYearStartDate,
                sys.FirstTermEndDate,
                sys.SecondTermStartDate,
                sys.SchoolYearEndDate,
                sys.SchoolYearEndDateLimit,
                sys.HasFutureEntryLock,
                sys.PastMonthLockDay);
        }

        await this.unitOfWork.SaveAsync(ct);

        foreach (var classBook in classBooks)
        {
            // no cancellation, we have already successfully saved the new info
            await this.classBookCachedQueryStore.ClearClassBookSchoolYearSettingsAsync(schoolYear, classBook.ClassBookId, default);
        }
    }

    private ValueTask<IDistributedSynchronizationHandle?> TryAcquireDistributedLockAsync(
        string name,
        IDbTransaction transaction,
        bool exactName,
        TimeSpan timeout,
        CancellationToken ct)
        => this.lockFactory(name, transaction, exactName).TryAcquireAsync(timeout, ct);
}
