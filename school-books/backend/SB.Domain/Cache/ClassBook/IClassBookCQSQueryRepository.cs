namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookCQSQueryRepository
{
    Task<GetVO?> GetAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetClassBookSchoolYearSettingsVO?> GetClassBookSchoolYearSettingsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<int?> GetCurriculumSubjectTypeIdAsync(
        int schoolYear,
        int curriculumId,
        CancellationToken ct);

    Task<int?> GetScheduleLessonTeacherAbsenceIdAsync(
        int schoolYear,
        int scheduleLessonId,
        CancellationToken ct);

    Task<bool> ExistsCurriculumForClassBookAsync(
        int schoolYear,
        int classId,
        bool classIsLvl2,
        int curriculumId,
        CancellationToken ct);

    Task<bool> ExistsSubjectForClassBookAsync(
        int schoolYear,
        int classId,
        bool classIsLvl2,
        int subjectId,
        CancellationToken ct);

    Task<bool> ExistsScheduleLessonForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        CancellationToken ct);

    Task<bool> ExistsScheduleLessonForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        int personId,
        CancellationToken ct);

    Task<int?> GetScheduleLessonCurriculumIdForClassBookAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        int scheduleLessonId,
        int? personId,
        CancellationToken ct);

    Task<bool> ExistsStudentForClassBookAsync(
        int schoolYear,
        int classBookClassId,
        bool classBookClassIsLvl2,
        int personId,
        CancellationToken ct);

    Task<bool> PersonExistsInCurriculumStudentAsync(
        int curriculumId,
        int personId,
        CancellationToken ct);
}
