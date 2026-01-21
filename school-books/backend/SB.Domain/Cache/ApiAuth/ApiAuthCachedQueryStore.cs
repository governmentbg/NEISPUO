namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

internal class ApiAuthCachedQueryStore : IApiAuthCachedQueryStore
{
    private readonly IApiAuthQueryRepository apiAuthQueryRepository;
    private readonly IApiAuthRedisRepository apiAuthRedisRepository;
    private readonly DomainOptions domainOptions;

    public ApiAuthCachedQueryStore(
        IApiAuthQueryRepository apiAuthQueryRepository,
        IApiAuthRedisRepository apiAuthRedisRepository,
        IOptions<DomainOptions> domainOptions)
    {
        this.apiAuthQueryRepository = apiAuthQueryRepository;
        this.apiAuthRedisRepository = apiAuthRedisRepository;
        this.domainOptions = domainOptions.Value;
    }

    public async Task<bool> GetPersonIsClassBookTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsClassBookTeacherAsync(jti, personId, schoolYear, instId, classBookId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsClassBookTeacherAsync(personId, schoolYear, instId, classBookId, ct);

        await this.apiAuthRedisRepository.CachePersonIsClassBookTeacherAsync(jti, exp, personId, schoolYear, instId, classBookId, result, ct);

        return result;
    }

    public async Task<bool> GetPersonIsClassBookLeadTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsClassBookLeadTeacherAsync(jti, personId, schoolYear, instId, classBookId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsClassBookLeadTeacherAsync(personId, schoolYear, instId, classBookId, ct);

        await this.apiAuthRedisRepository.CachePersonIsClassBookLeadTeacherAsync(jti, exp, personId, schoolYear, instId, classBookId, result, ct);

        return result;
    }

    public async Task<bool> GetPersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int curriculumId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(jti, personId, schoolYear, instId, classBookId, curriculumId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsClassBookLeadTeacherAsync(personId, schoolYear, instId, classBookId, ct) ||
            await this.apiAuthQueryRepository.GetPersonIsClassBookCurriculumTeacherAsync(personId, schoolYear, instId, classBookId, curriculumId, ct);

        await this.apiAuthRedisRepository.CachePersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(jti, exp, personId, schoolYear, instId, classBookId, curriculumId, result, ct);

        return result;
    }

    public async Task<bool> GetPersonIsClassBookLeadTeacherOrLessonTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int scheduleLessonId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsClassBookLeadTeacherOrLessonTeacherAsync(jti, personId, schoolYear, instId, classBookId, scheduleLessonId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsClassBookLeadTeacherAsync(personId, schoolYear, instId, classBookId, ct) ||
            await this.apiAuthQueryRepository.GetPersonIsClassBookLessonTeacherAsync(personId, schoolYear, instId, classBookId, scheduleLessonId, ct);

        await this.apiAuthRedisRepository.CachePersonIsClassBookLeadTeacherOrLessonTeacherAsync(jti, exp, personId, schoolYear, instId, classBookId, scheduleLessonId, result, ct);

        return result;
    }

    public async Task<bool> GetPersonIsInSupportTeamAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int supportId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsInSupportTeamAsync(jti, personId, schoolYear, instId, classBookId, supportId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsInSupportTeamAsync(personId, schoolYear, instId, classBookId, supportId, ct);

        await this.apiAuthRedisRepository.CachePersonIsInSupportTeamAsync(jti, exp, personId, schoolYear, instId, classBookId, supportId, result, ct);

        return result;
    }

    public async Task<bool> GetPersonIsReplTeacherForDateAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, DateTime date, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsReplTeacherForDateAsync(jti, personId, schoolYear, instId, classBookId, date, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsReplTeacherForDateAsync(personId, schoolYear, instId, classBookId, date, ct);

        await this.apiAuthRedisRepository.CachePersonIsReplTeacherForDateAsync(jti, exp, personId, schoolYear, instId, classBookId, date, result, ct);

        return result;
    }

    public async Task<bool> GetPersonIsReplTeacherForDateAndCurriculumAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetPersonIsReplTeacherForDateAndCurriculumAsync(jti, personId, schoolYear, instId, classBookId, curriculumId, date, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetPersonIsReplTeacherForDateAndCurriculumAsync(personId, schoolYear, instId, classBookId, curriculumId, date, ct);

        await this.apiAuthRedisRepository.CachePersonIsReplTeacherForDateAndCurriculumAsync(jti, exp, personId, schoolYear, instId, classBookId, curriculumId, date, result, ct);

        return result;
    }

    public async Task<bool> GetInstitutionBelongsToRegionAsync(int instId, int regionId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetInstitutionBelongsToRegionAsync(instId, regionId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetInstitutionBelongsToRegionAsync(instId, regionId, ct);

        await this.apiAuthRedisRepository.CacheInstitutionBelongsToRegionAsync(
            this.domainOptions.LongCacheExpiration,
            instId,
            regionId,
            result,
            ct);

        return result;
    }

    public async Task<bool> GetClassBookBelongsToInstitutionAsync(int schoolYear, int instId, int classBookId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetClassBookBelongsToInstitutionAsync(schoolYear, instId, classBookId, ct);

        await this.apiAuthRedisRepository.CacheClassBookBelongsToInstitutionAsync(
            this.domainOptions.LongCacheExpiration,
            schoolYear,
            instId,
            classBookId,
            result,
            ct);

        return result;
    }

    public async Task<bool> GetHisMedicalNoticeBelongsToRegionAsync(int hisMedicalNoticeId, int regionId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetHisMedicalNoticeBelongsToRegionAsync(hisMedicalNoticeId, regionId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetHisMedicalNoticeBelongsToRegionAsync(hisMedicalNoticeId, regionId, ct);

        await this.apiAuthRedisRepository.CacheHisMedicalNoticeBelongsToRegionAsync(
            this.domainOptions.LongCacheExpiration,
            hisMedicalNoticeId,
            regionId,
            result,
            ct);

        return result;
    }

    public async Task<bool> GetStudentBelongsToClassBookAsync(int schoolYear, int instId, int classBookId, int personId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetStudentBelongsToClassBookAsync(schoolYear, instId, classBookId, personId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetStudentBelongsToClassBookAsync(schoolYear, instId, classBookId, personId, ct);

        await this.apiAuthRedisRepository.CacheStudentBelongsToClassBookAsync(
            this.domainOptions.LongCacheExpiration,
            schoolYear,
            instId,
            classBookId,
            personId,
            result,
            ct);

        return result;
    }

    public async Task<bool> GetStudentsBelongsToInstitutionAsync(int schoolYear, int instId, int[] studentsPersonId, CancellationToken ct)
    {
        var cacheResult = await this.apiAuthRedisRepository.GetStudentsBelongsToInstitutionAsync(schoolYear, instId, studentsPersonId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        var result =
            await this.apiAuthQueryRepository.GetStudentsBelongsToInstitutionAsync(schoolYear, instId, studentsPersonId, ct);

        await this.apiAuthRedisRepository.CacheStudentsBelongsToInstitutionAsync(
            this.domainOptions.LongCacheExpiration,
            schoolYear,
            instId,
            studentsPersonId,
            result,
            ct);

        return result;
    }
}
