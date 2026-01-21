namespace SB.Domain;

using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;
using static SB.Domain.IClassBookCQSQueryRepository;

internal class ClassBookCachedQueryStore : IClassBookCachedQueryStore
{
    private readonly IFusionCache fusionCache;
    private readonly IClassBookCQSQueryRepository classBookCQSQueryRepository;
    private readonly DomainOptions domainOptions;
    private readonly ICommonCachedQueryStore commonCachedQueryStore;
    private const string ClassBookCachedQueryStoreKeyPrefix = "CBCQS";
    private const int FirstGradeBasicClassId = 1;

    public ClassBookCachedQueryStore(
        IFusionCache fusionCache,
        IClassBookCQSQueryRepository classBookCQSQueryRepository,
        IOptions<DomainOptions> domainOptions,
        ICommonCachedQueryStore commonCachedQueryStore)
    {
        this.fusionCache = fusionCache;
        this.classBookCQSQueryRepository = classBookCQSQueryRepository;
        this.domainOptions = domainOptions.Value;
        this.commonCachedQueryStore = commonCachedQueryStore;
    }

    public async Task<SchoolTerm> GetTermForDateAsync(int schoolYear, int classBookId, DateTime date, CancellationToken ct)
    {
        var schoolYearSettings =
            (await this.GetClassBookSchoolYearSettingsAsync(schoolYear, classBookId, ct))
            ?? throw new Exception($"No ClassBookSchoolYearSettings found for schoolYear:{schoolYear}, classBookId:{classBookId}");

        if (schoolYearSettings.SchoolYearStartDateLimit <= date && date < schoolYearSettings.SecondTermStartDate)
        {
            return SchoolTerm.TermOne;
        }
        else if (schoolYearSettings.SecondTermStartDate <= date && date <= schoolYearSettings.SchoolYearEndDateLimit)
        {
            return SchoolTerm.TermTwo;
        }
        else
        {
            throw new DomainValidationException($"The date falls outside of the school year {schoolYearSettings.SchoolYearStartDateLimit:dd.MM.yyy} - {schoolYearSettings.SchoolYearEndDateLimit:dd.MM.yyyy}.");
        }
    }

    public async Task<int?> GetCurriculumSubjectTypeIdAsync(int schoolYear, int curriculumId, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<int?>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:GCSTI:{schoolYear}:{curriculumId}",
            factory: async (ctx, ct) =>
            {
                var res = await this.classBookCQSQueryRepository.GetCurriculumSubjectTypeIdAsync(schoolYear, curriculumId, ct);

                if (res == null)
                {
                    // cache nulls for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return null;
                }

                ctx.Options.Duration = this.domainOptions.LongCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize,
                },
            token: ct);
    }

    public async Task<int?> GetScheduleLessonTeacherAbsenceIdAsync(int schoolYear, int scheduleLessonId, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<int?>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:GSLTAI:{schoolYear}:{scheduleLessonId}",
            factory: async (ctx, ct) =>
            {
                var res = await this.classBookCQSQueryRepository.GetScheduleLessonTeacherAbsenceIdAsync(schoolYear, scheduleLessonId, ct);

                if (res == null)
                {
                    // cache nulls for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return null;
                }

                ctx.Options.Duration = this.domainOptions.LongCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize,
                },
            token: ct);
    }

    public async Task ClearScheduleLessonTeacherAbsenceIdAsync(int schoolYear, int scheduleLessonId, CancellationToken ct)
    {
        var key = GetScheduleLessonTeacherAbsenceIdKey(schoolYear, scheduleLessonId);
        await this.fusionCache.RemoveAsync(key, token: ct);
    }

    public async Task ClearClassBookSchoolYearSettingsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        await this.fusionCache.RemoveAsync(GetClassBookSchoolYearSettingsKey(schoolYear, classBookId), token: ct);
    }

    public async Task ClearClassBookAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        await this.fusionCache.RemoveAsync(GetClassBookKey(schoolYear, classBookId), token: ct);
    }

    public async Task ClearClassBooksAsync(int schoolYear, int[] classBookIds)
    {
        var defaultEntryOptions = this.fusionCache.DefaultEntryOptions.Duplicate();
        defaultEntryOptions.AllowBackgroundDistributedCacheOperations = true;

        foreach (var classBookId in classBookIds)
        {
            await this.fusionCache.RemoveAsync(
                GetClassBookKey(schoolYear, classBookId),
                defaultEntryOptions);
        }
    }

    public async Task<bool> CheckBookTypeAllowsDecimalGradesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsDecimalGrades(cb.BookType);
    }

    public async Task<bool> CheckBookTypeAllowsQualitativeGradesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsQualitativeGrades(cb.BookType);
    }

    public async Task<bool> CheckBookTypeAllowsSpecialGradesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsSpecialGrades(cb.BookType);
    }

    public async Task<bool> CheckBookTypeAllowsAbsencesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsAbsences(cb.BookType);
    }

    public async Task<bool> CheckBookTypeAllowsDplrAbsencesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsDplrAbsences(cb.BookType);
    }

    public async Task<bool> CheckBookTypeAllowsAbsenceTypeAsync(
        int schoolYear,
        int classBookId,
        AbsenceType type,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return type switch
        {
            AbsenceType.Late or
            AbsenceType.Unexcused or
            AbsenceType.Excused
                => ClassBook.CheckBookTypeAllowsAbsences(cb.BookType),
            AbsenceType.DplrAbsence or
            AbsenceType.DplrAttendance
                => ClassBook.CheckBookTypeAllowsDplrAbsences(cb.BookType),
            _ => throw new Exception("Unknown absence type"),
        };
    }

    public async Task<bool> CheckBookTypeAllowsAttendancesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsAttendances(cb.BookType);
    }

    public async Task<bool> CheckBookTypeAllowsAdditionalActivitiesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return ClassBook.CheckBookTypeAllowsAdditionalActivities(cb.BookType);
    }

    public async Task<bool> CheckSchoolYearAllowsModificationsAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        bool schoolYearIsFinalized =
            await this.commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                instId,
                ct);

        return !schoolYearIsFinalized;
    }

    public async Task<bool> CheckClassBookIsValidAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        return cb?.IsValid ?? false;
    }

    public async Task<bool> CheckBookAllowsModificationsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        bool schoolYearIsFinalized =
            await this.commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                cb.InstId,
                ct);

        return ClassBook.CheckBookAllowsModifications(
            schoolYearIsFinalized,
            cb.IsFinalized);
    }

    public async Task<bool> CheckBookAllowsGradeModificationsAsync(
        int schoolYear,
        int classBookId,
        GradeType gradeType,
        DateTime gradeDate,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        bool schoolYearIsFinalized =
            await this.commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                cb.InstId,
                ct);

        var schoolYearSettings =
            (await this.GetClassBookSchoolYearSettingsAsync(schoolYear, classBookId, ct))
            ?? throw new Exception($"No ClassBookSchoolYearSettings found for schoolYear:{schoolYear}, classBookId:{classBookId}");

        return ClassBook.CheckBookAllowsGradeModifications(
            schoolYearIsFinalized,
            cb.IsFinalized,
            schoolYearSettings.HasFutureEntryLock,
            schoolYearSettings.PastMonthLockDay,
            DateTime.Now,
            gradeType,
            gradeDate);
    }

    public async Task<bool> CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        bool schoolYearIsFinalized =
            await this.commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                cb.InstId,
                ct);

        var schoolYearSettings =
            (await this.GetClassBookSchoolYearSettingsAsync(schoolYear, classBookId, ct))
            ?? throw new Exception($"No ClassBookSchoolYearSettings found for schoolYear:{schoolYear}, classBookId:{classBookId}");

        return ClassBook.CheckBookAllowsAttendanceAbsenceTopicModifications(
            schoolYearIsFinalized,
            cb.IsFinalized,
            schoolYearSettings.HasFutureEntryLock,
            schoolYearSettings.PastMonthLockDay,
            DateTime.Now,
            date);
    }

    public async Task<bool> CheckBookAllowsAdditionalActivityModificationsAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        bool schoolYearIsFinalized =
            await this.commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                cb.InstId,
                ct);

        var schoolYearSettings =
            (await this.GetClassBookSchoolYearSettingsAsync(schoolYear, classBookId, ct))
            ?? throw new Exception($"No ClassBookSchoolYearSettings found for schoolYear:{schoolYear}, classBookId:{classBookId}");

        return ClassBook.CheckBookAllowsAdditionalActivityModifications(
            schoolYearIsFinalized,
            cb.IsFinalized,
            schoolYearSettings.HasFutureEntryLock,
            schoolYearSettings.PastMonthLockDay,
            DateTime.Now,
            year,
            weekNumber);
    }

    public async Task<bool> CheckPersonExistsInCurriculumStudentsOrItsProfilingSubjectAsync(
        int schoolYear,
        int curriculumId,
        int personId,
        CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<bool>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:PEICSOIPS:{schoolYear}:{curriculumId}:{personId}",
            factory: async (ctx, ct) =>
            {
                var curriculumSubjectTypeId =
                    await this.classBookCQSQueryRepository.GetCurriculumSubjectTypeIdAsync(schoolYear, curriculumId, ct);

                var res = Grade.SubjectTypeIsProfilingSubject(curriculumSubjectTypeId) ||
                    await this.classBookCQSQueryRepository.PersonExistsInCurriculumStudentAsync(
                        curriculumId,
                        personId,
                        ct);

                if (res == false)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return false;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
            new FusionCacheEntryOptions
            {
                Size = CacheConstants.SmallObjectSize
            },
            token: ct);
    }

    public async Task<bool> CheckIsFirstGradeClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return cb.BookType == ClassBookType.Book_I_III && cb.BasicClassId == FirstGradeBasicClassId;
    }

    public async Task<bool> ExistsClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);
        return cb != null;
    }

    public async Task<bool> ExistsCurriculumForClassBookAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return await this.fusionCache.GetOrSetAsync<bool>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:ECFCB:{schoolYear}:{classBookId}:{curriculumId}",
            factory: async (ctx, ct) =>
            {
                var res =
                    await this.classBookCQSQueryRepository.ExistsCurriculumForClassBookAsync(
                        schoolYear,
                        cb.ClassId,
                        cb.ClassIsLvl2,
                        curriculumId,
                        ct);

                if (res == false)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return false;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }

    public async Task<bool> ExistsSubjectForClassBookAsync(
        int schoolYear,
        int classBookId,
        int subjectId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return await this.fusionCache.GetOrSetAsync<bool>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:ESFCB:{schoolYear}:{classBookId}:{subjectId}",
            factory: async (ctx, ct) =>
            {
                var res =
                    await this.classBookCQSQueryRepository.ExistsSubjectForClassBookAsync(
                        schoolYear,
                        cb.ClassId,
                        cb.ClassIsLvl2,
                        subjectId,
                        ct);

                if (res == false)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return false;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }

    public async Task<bool> ExistsScheduleLessonForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<bool>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:ESLFCB:{schoolYear}:{classBookId}:{date:yyyy-MM-dd}:{scheduleLessonId}",
            factory: async (ctx, ct) =>
            {
                var res =
                    await this.classBookCQSQueryRepository.ExistsScheduleLessonForClassBookAsync(
                        schoolYear,
                        classBookId,
                        date,
                        scheduleLessonId,
                        ct);

                if (res == false)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return false;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }

    public async Task<bool> ExistsScheduleLessonForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        int personId,
        CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<bool>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:ESLFCB:{schoolYear}:{classBookId}:{date:yyyy-MM-dd}:{scheduleLessonId}:{personId}",
            factory: async (ctx, ct) =>
            {
                var res =
                    await this.classBookCQSQueryRepository.ExistsScheduleLessonForClassBookAsync(
                        schoolYear,
                        classBookId,
                        date,
                        scheduleLessonId,
                        personId,
                        ct);

                if (res == false)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return false;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }

    public async Task<int?> GetScheduleLessonCurriculumIdForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        int? personId,
        CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<int?>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:GSLCFCB:{schoolYear}:{classBookId}:{date:yyyy-MM-dd}:{scheduleLessonId}:{personId}",
            factory: async (ctx, ct) =>
            {
                int? curriculumId =
                    await this.classBookCQSQueryRepository.GetScheduleLessonCurriculumIdForClassBookAsync(
                        schoolYear,
                        classBookId,
                        date,
                        scheduleLessonId,
                        personId,
                        ct);

                if (curriculumId == null)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return null;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return curriculumId;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }

    public async Task<bool> ExistsStudentForClassBookAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var cb = await this.GetClassBookAsync(schoolYear, classBookId, ct);

        if (cb == null)
        {
            return false;
        }

        return await this.fusionCache.GetOrSetAsync<bool>(
            key: $"{ClassBookCachedQueryStoreKeyPrefix}:ESFCB:{schoolYear}:{classBookId}:{personId}",
            factory: async (ctx, ct) =>
            {
                var res =
                    await this.classBookCQSQueryRepository.ExistsStudentForClassBookAsync(
                        schoolYear,
                        cb.ClassId,
                        cb.ClassIsLvl2,
                        personId,
                        ct);

                if (res == false)
                {
                    // cache negatives for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return false;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }

    private static string GetClassBookKey(int schoolYear, int classBookId)
        => $"{ClassBookCachedQueryStoreKeyPrefix}:GCB:{schoolYear}:{classBookId}";
    private async Task<GetVO?> GetClassBookAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<GetVO?>(
            key: GetClassBookKey(schoolYear, classBookId),
            factory: async (ctx, ct) =>
            {
                var res = await this.classBookCQSQueryRepository.GetAsync(schoolYear, classBookId, ct);

                if (res == null)
                {
                    // cache nulls for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return null;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize,
                },
            token: ct);
    }

    private static string GetClassBookSchoolYearSettingsKey(int schoolYear, int classBookId)
        => $"{ClassBookCachedQueryStoreKeyPrefix}:GCBSYS:{schoolYear}:{classBookId}";
    private async Task<GetClassBookSchoolYearSettingsVO?> GetClassBookSchoolYearSettingsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<GetClassBookSchoolYearSettingsVO?>(
            key: GetClassBookSchoolYearSettingsKey(schoolYear, classBookId),
            factory: async (ctx, ct) =>
            {
                var res = await this.classBookCQSQueryRepository.GetClassBookSchoolYearSettingsAsync(schoolYear, classBookId, ct);

                if (res == null)
                {
                    // cache nulls for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return null;
                }

                ctx.Options.Duration = this.domainOptions.MediumCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize,
                },
            token: ct);
    }

    private static string GetScheduleLessonTeacherAbsenceIdKey(int schoolYear, int scheduleLessonId)
        => $"{ClassBookCachedQueryStoreKeyPrefix}:GSLTAI:{schoolYear}:{scheduleLessonId}";
}
