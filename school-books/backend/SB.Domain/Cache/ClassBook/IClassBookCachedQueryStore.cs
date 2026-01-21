namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IClassBookCachedQueryStore
{
    Task<SchoolTerm> GetTermForDateAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct);

    Task<int?> GetCurriculumSubjectTypeIdAsync(
        int schoolYear,
        int curriculumId,
        CancellationToken ct);

    Task<int?> GetScheduleLessonTeacherAbsenceIdAsync(
        int schoolYear,
        int scheduleLessonId,
        CancellationToken ct);

    Task ClearScheduleLessonTeacherAbsenceIdAsync(
        int schoolYear,
        int scheduleLessonId,
        CancellationToken ct);

    Task ClearClassBookSchoolYearSettingsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task ClearClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task ClearClassBooksAsync(
        int schoolYear,
        int[] classBookIds);

    Task<bool> CheckBookTypeAllowsDecimalGradesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsQualitativeGradesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsSpecialGradesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsAbsencesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsDplrAbsencesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsAbsenceTypeAsync(
        int schoolYear,
        int classBookId,
        AbsenceType type,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsAttendancesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookTypeAllowsAdditionalActivitiesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckSchoolYearAllowsModificationsAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<bool> CheckClassBookIsValidAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookAllowsModificationsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> CheckBookAllowsGradeModificationsAsync(
        int schoolYear,
        int classBookId,
        GradeType gradeType,
        DateTime gradeDate,
        CancellationToken ct);

    Task<bool> CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct);

    Task<bool> CheckBookAllowsAdditionalActivityModificationsAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct);

    Task<bool> CheckPersonExistsInCurriculumStudentsOrItsProfilingSubjectAsync(
        int schoolYear,
        int curriculumId,
        int personId,
        CancellationToken ct);

    Task<bool> CheckIsFirstGradeClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> ExistsClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> ExistsCurriculumForClassBookAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    Task<bool> ExistsSubjectForClassBookAsync(
        int schoolYear,
        int classBookId,
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
        int classBookId,
        int personId,
        CancellationToken ct);
}
