namespace SB.Domain;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ISchoolYearSettingsProvider;
using static SB.Domain.ISchoolYearSettingsQueryRepository;

internal class SchoolYearSettingsProvider : ISchoolYearSettingsProvider
{
    private Dictionary<(int, int), bool> isSportSchool = new();
    private Dictionary<(int, int), bool> isCplr = new();
    private Dictionary<int, GetDefaultVO> defaultSettings = new();
    private Dictionary<(int, int), (GetAllForRebuildVO?, GetAllForRebuildVO[])> allSettings = new();

    private readonly ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository;
    private readonly ILogger<SchoolYearSettingsProvider> logger;

    public SchoolYearSettingsProvider(
        ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository,
        ILogger<SchoolYearSettingsProvider> logger)
    {
        this.schoolYearSettingsQueryRepository = schoolYearSettingsQueryRepository;
        this.logger = logger;
    }

    public async Task<GetForClassBookVO> GetForClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? basicClassId,
        int?[] childBasicClassIds,
        CancellationToken ct)
    {
        bool isSportSchool = await this.IsSportSchoolAsync(schoolYear, instId, ct);
        bool isCplr = await this.IsCplrAsync(schoolYear, instId, ct);
        var defaultSettings = await this.GetDefaultAsync(schoolYear, ct);
        (var allClassesSettings, var otherSettings) = await this.GetAllForRebuildAsync(schoolYear, instId, ct);

        static bool isPgBasicClassId(int? basicClassId) => basicClassId < 0 || basicClassId == 21 || basicClassId == 32;

        if (basicClassId == null &&
            !(childBasicClassIds.All(cbId => isPgBasicClassId(cbId))
            || childBasicClassIds.All(cbId => !isPgBasicClassId(cbId))))
        {
            this.logger.LogError("Child BasicClassIds missmatch! schoolYear: {schoolYear}, instId: {instId}, basicClassId: {basicClassId}, childBasicClassIds: {childBasicClassIds}", schoolYear, instId, basicClassId, "[" + string.Join(", ", childBasicClassIds) + "]");
            throw new DomainValidationException("Child BasicClassIds missmatch!");
        }

        GetAllForRebuildVO? sys = otherSettings
            .Where(sys => sys.ClassBookIds.Contains(classBookId))
            .OrderByDescending(sys => sys.SchoolYearEndDate)
            .FirstOrDefault();

        sys ??= otherSettings
            .Where(
                sys => basicClassId.HasValue
                    ? sys.BasicClassIds.Contains(basicClassId.Value)
                    : sys.BasicClassIds.Cast<int?>().Intersect(childBasicClassIds).Any())
            .OrderByDescending(sys => sys.SchoolYearEndDate)
            .FirstOrDefault();

        sys ??= allClassesSettings;

        if (isPgBasicClassId(basicClassId) || (basicClassId == null && childBasicClassIds.All(cbId => isPgBasicClassId(cbId)))) // PG
        {
            //Set PG start and end of school year as default values, they can't be changed
            return new GetForClassBookVO(
                sys?.SchoolYearSettingsId,
                defaultSettings.PgSchoolYearStartDateLimit,
                defaultSettings.PgSchoolYearStartDate,
                sys?.FirstTermEndDate ?? defaultSettings.PgFirstTermEndDate,
                sys?.SecondTermStartDate ?? defaultSettings.PgSecondTermStartDate,
                defaultSettings.PgSchoolYearEndDate,
                defaultSettings.PgSchoolYearEndDateLimit,
                sys?.HasFutureEntryLock ?? false,
                sys?.PastMonthLockDay);
        }

        if (isSportSchool)
        {
            return new GetForClassBookVO(
                sys?.SchoolYearSettingsId,
                defaultSettings.SportSchoolYearStartDateLimit,
                sys?.SchoolYearStartDate ?? defaultSettings.SportSchoolYearStartDate,
                sys?.FirstTermEndDate ?? defaultSettings.SportFirstTermEndDate,
                sys?.SecondTermStartDate ?? defaultSettings.SportSecondTermStartDate,
                sys?.SchoolYearEndDate ?? defaultSettings.SportSchoolYearEndDate,
                defaultSettings.SportSchoolYearEndDateLimit,
                sys?.HasFutureEntryLock ?? false,
                sys?.PastMonthLockDay);
        }

        if (isCplr)
        {
            return new GetForClassBookVO(
                sys?.SchoolYearSettingsId,
                defaultSettings.CplrSchoolYearStartDateLimit,
                sys?.SchoolYearStartDate ?? defaultSettings.CplrSchoolYearStartDate,
                sys?.FirstTermEndDate ?? defaultSettings.CplrFirstTermEndDate,
                sys?.SecondTermStartDate ?? defaultSettings.CplrSecondTermStartDate,
                sys?.SchoolYearEndDate ?? defaultSettings.CplrSchoolYearEndDate,
                defaultSettings.CplrSchoolYearEndDateLimit,
                sys?.HasFutureEntryLock ?? false,
                sys?.PastMonthLockDay);
        }

        return new GetForClassBookVO(
            sys?.SchoolYearSettingsId,
            defaultSettings.OtherSchoolYearStartDateLimit,
            sys?.SchoolYearStartDate ?? defaultSettings.OtherSchoolYearStartDate,
            sys?.FirstTermEndDate ?? defaultSettings.OtherFirstTermEndDate,
            sys?.SecondTermStartDate ?? defaultSettings.OtherSecondTermStartDate,
            sys?.SchoolYearEndDate ?? defaultSettings.OtherSchoolYearEndDate,
            defaultSettings.OtherSchoolYearEndDateLimit,
            sys?.HasFutureEntryLock ?? false,
            sys?.PastMonthLockDay);
    }

    private async Task<bool> IsSportSchoolAsync(int schoolYear, int instId, CancellationToken ct)
    {
        // this class is not supposed to be called concurrently
        // so we dont need any synchronization

        if (this.isSportSchool.TryGetValue((schoolYear, instId), out var cachedIsSportSchool))
        {
            return cachedIsSportSchool;
        }

        bool isSportSchool = await this.schoolYearSettingsQueryRepository.IsSportSchoolAsync(schoolYear, instId, ct);
        this.isSportSchool.Add((schoolYear, instId), isSportSchool);

        return isSportSchool;
    }

    private async Task<bool> IsCplrAsync(int schoolYear, int instId, CancellationToken ct)
    {
        // this class is not supposed to be called concurrently
        // so we dont need any synchronization

        if (this.isCplr.TryGetValue((schoolYear, instId), out var cachedIsCplr))
        {
            return cachedIsCplr;
        }

        bool isCplr = await this.schoolYearSettingsQueryRepository.IsCplrAsync(schoolYear, instId, ct);
        this.isCplr.Add((schoolYear, instId), isCplr);

        return isCplr;
    }

    private async Task<GetDefaultVO> GetDefaultAsync(int schoolYear, CancellationToken ct)
    {
        // this class is not supposed to be called concurrently
        // so we dont need any synchronization

        if (this.defaultSettings.TryGetValue(schoolYear, out var cachedDefaultSettings))
        {
            return cachedDefaultSettings;
        }

        var defaultSettings = await this.schoolYearSettingsQueryRepository.GetDefaultAsync(schoolYear, ct);
        this.defaultSettings.Add(schoolYear, defaultSettings);

        return defaultSettings;
    }

    private async Task<(GetAllForRebuildVO?, GetAllForRebuildVO[])> GetAllForRebuildAsync(int schoolYear, int instId, CancellationToken ct)
    {
        // this class is not supposed to be called concurrently
        // so we dont need any synchronization

        if (this.allSettings.TryGetValue((schoolYear, instId), out var cachedAllSettings))
        {
            return cachedAllSettings;
        }

        var allSettings = await this.schoolYearSettingsQueryRepository.GetAllForRebuildAsync(schoolYear, instId, ct);

        var allAllClassesSettings = allSettings.Where(sys => sys.IsForAllClasses).ToArray();
        GetAllForRebuildVO? allClassesSettings;
        if (allAllClassesSettings.Length == 0)
        {
            allClassesSettings = null; // default
        }
        else if (allAllClassesSettings.Length > 1)
        {
            throw new DomainValidationException("There should be at maximum 1 SchoolYearSettings with IsForAllClasses=True");
        }
        else
        {
            allClassesSettings = allAllClassesSettings[0];
        }

        var otherSettings = allSettings.Where(sys => !sys.IsForAllClasses).ToArray();

        this.allSettings.Add((schoolYear, instId), (allClassesSettings, otherSettings));

        return (allClassesSettings, otherSettings);
    }
}
