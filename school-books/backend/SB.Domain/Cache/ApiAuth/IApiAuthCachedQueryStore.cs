namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IApiAuthCachedQueryStore
{
    Task<bool> GetPersonIsClassBookTeacherAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookLeadTeacherAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookLeadTeacherOrLessonTeacherAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct);

    Task<bool> GetPersonIsInSupportTeamAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int supportId,
        CancellationToken ct);

    Task<bool> GetPersonIsReplTeacherForDateAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct);

    Task<bool> GetPersonIsReplTeacherForDateAndCurriculumAsync(
        string jti,
        DateTime exp,
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        DateTime date,
        CancellationToken ct);

    Task<bool> GetInstitutionBelongsToRegionAsync(
        int instId,
        int regionId,
        CancellationToken ct);

    Task<bool> GetClassBookBelongsToInstitutionAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> GetHisMedicalNoticeBelongsToRegionAsync(
        int hisMedicalNoticeId,
        int regionId,
        CancellationToken ct);

    Task<bool> GetStudentBelongsToClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<bool> GetStudentsBelongsToInstitutionAsync(
        int schoolYear,
        int instId,
        int[] studentsPersonId,
        CancellationToken ct);
}
