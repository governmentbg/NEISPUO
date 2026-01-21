namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using SB.Common;
using static SB.Domain.ISchedulesQueryRepository;

public partial interface IStudentInfoClassBooksQueryRepository
{
    Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        int schoolYear,
        int instId,
        int studentPersonId,
        CancellationToken ct);

    Task<GetClassBookInfoVO> GetClassBookInfoAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<GetGradesVO[]> GetGradesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<GetStudentInfoGradeVO> GetGradeAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int gradeId,
        CancellationToken ct);

    Task<GetAbsencesVO[]> GetAbsencesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<GetAbsencesDplrVO[]> GetAbsencesDplrAsync(
        int schoolYear,
        int classBookId,
        int personId,
        AbsenceType type,
        CancellationToken ct);

    Task<GetAbsencesForCurriculumAndTypeVO[]> GetAbsencesForCurriculumAndTypeAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        AbsenceType type,
        CancellationToken ct);

    Task<GetAttendanceMonthStatsVO[]> GetAttendancesMonthStatsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<GetAttendancesVO[]> GetAttendancesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        AttendanceType attendanceType,
        int year,
        int month,
        CancellationToken ct);

    Task<GetTopicsVO[]> GetTopicsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        CancellationToken ct);

    Task<GetRemarksVO[]> GetRemarksAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<GetRemarksForCurriculumAndTypeVO[]> GetRemarksForCurriculumAndTypeAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        RemarkType type,
        CancellationToken ct);

    Task<GetClassBookScheduleTableForWeekVO> GetScheduleAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int year,
        int weekNumber,
        bool showIndividualCurriculum,
        CancellationToken ct);

    Task<TableResultVO<GetParentMeetingsVO>> GetParentMeetingsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetExamsVO>> GetExamsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetIndividualWorksVO>> GetIndividualWorksAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetSanctionsVO>> GetSanctionsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetSupportsVO>> GetSupportsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetSupportVO> GetSupportAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int supportId,
        CancellationToken ct);

    Task<TableResultVO<GetSupportActivitiesVO>> GetSupportActivitiesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int supportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetNotesVO>> GetNotesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetGradeResultVO> GetGradeResultAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<GetFirstGradeResultsVO?> GetFirstGradeResultsOrDefaultAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<TableResultVO<GetPgResultsVO>> GetPgResultsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetCurriculumForTopicsVO[]> GetCurriculumForTopicsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);
}
