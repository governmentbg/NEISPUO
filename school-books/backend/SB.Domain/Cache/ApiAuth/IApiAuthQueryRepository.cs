namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IApiAuthQueryRepository
{
    Task<bool> GetInstitutionBelongsToRegionAsync(
        int instId,
        int regionId,
        CancellationToken ct);

    Task<bool> GetClassBookBelongsToInstitutionAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookLeadTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookCurriculumTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    Task<bool> GetPersonIsClassBookLessonTeacherAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct);

    Task<bool> GetPersonIsInSupportTeamAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int supportId,
        CancellationToken ct);

    Task<bool> GetPersonIsReplTeacherForDateAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct);

    Task<bool> GetPersonIsReplTeacherForDateAndCurriculumAsync(
        int personId,
        int schoolYear,
        int instId,
        int classBookId,
        int curriculumId,
        DateTime date,
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
