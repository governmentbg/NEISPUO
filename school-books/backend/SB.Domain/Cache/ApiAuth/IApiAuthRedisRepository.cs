namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IApiAuthRedisRepository
{
    Task<bool?> GetPersonIsClassBookTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, CancellationToken ct);

    Task CachePersonIsClassBookTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, bool result, CancellationToken ct);

    Task<bool?> GetPersonIsClassBookLeadTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, CancellationToken ct);

    Task CachePersonIsClassBookLeadTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, bool result, CancellationToken ct);

    Task<bool?> GetPersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int curriculumId, CancellationToken ct);

    Task CachePersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int curriculumId, bool result, CancellationToken ct);

    Task<bool?> GetPersonIsClassBookLeadTeacherOrLessonTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int scheduleLessonId, CancellationToken ct);

    Task CachePersonIsClassBookLeadTeacherOrLessonTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int scheduleLessonId, bool result, CancellationToken ct);

    Task<bool?> GetPersonIsInSupportTeamAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int supportId, CancellationToken ct);

    Task CachePersonIsInSupportTeamAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int supportId, bool result, CancellationToken ct);

    Task<bool?> GetPersonIsReplTeacherForDateAsync(string jti, int personId, int schoolYear, int instId, int classBookId, DateTime date, CancellationToken ct);

    Task<bool?> GetPersonIsReplTeacherForDateAndCurriculumAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, CancellationToken ct);

    Task CachePersonIsReplTeacherForDateAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, DateTime date, bool result, CancellationToken ct);

    Task CachePersonIsReplTeacherForDateAndCurriculumAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, bool result, CancellationToken ct);

    Task<bool?> GetInstitutionBelongsToRegionAsync(int instId, int regionId, CancellationToken ct);

    Task CacheInstitutionBelongsToRegionAsync(TimeSpan exp, int instId, int regionId, bool result, CancellationToken ct);

    Task<bool?> GetClassBookBelongsToInstitutionAsync(int schoolYear, int instId, int classBookId, CancellationToken ct);

    Task CacheClassBookBelongsToInstitutionAsync(TimeSpan exp, int schoolYear, int instId, int classBookId, bool result, CancellationToken ct);

    Task<bool?> GetHisMedicalNoticeBelongsToRegionAsync(int hisMedicalNoticeId, int regionId, CancellationToken ct);

    Task CacheHisMedicalNoticeBelongsToRegionAsync(TimeSpan exp, int hisMedicalNoticeId, int regionId, bool result, CancellationToken ct);

    Task<bool?> GetStudentBelongsToClassBookAsync(int schoolYear, int instId, int classBookId, int personId, CancellationToken ct);

    Task CacheStudentBelongsToClassBookAsync(TimeSpan exp, int schoolYear, int instId, int classBookId, int personId, bool result, CancellationToken ct);

    Task<bool?> GetStudentsBelongsToInstitutionAsync(int schoolYear, int instId, int[] studentsPersonId, CancellationToken ct);

    Task CacheStudentsBelongsToInstitutionAsync(TimeSpan exp, int schoolYear, int instId, int[] studentsPersonId, bool result, CancellationToken ct);
}
