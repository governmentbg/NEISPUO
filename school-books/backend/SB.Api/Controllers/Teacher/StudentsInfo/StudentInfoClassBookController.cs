namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISchedulesQueryRepository;
using static SB.Domain.IStudentInfoClassBooksQueryRepository;

[Authorize(Policy = Policies.StudentInfoClassBookAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/{classBookId:int}/{personId:int}/{studentClassBookId:int}/[action]")]
public class StudentInfoClassBookController
{
    [HttpGet]
    public async Task<GetClassBookInfoVO> GetClassBookInfoAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetClassBookInfoAsync(
                schoolYear,
                studentClassBookId,
                personId,
                ct);

    [HttpGet]
    public async Task<GetGradesVO[]> GetGradesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetGradesAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet("{gradeId:int}")]
    public async Task<GetStudentInfoGradeVO> GetGradeAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromRoute] int gradeId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetGradeAsync(schoolYear, studentClassBookId, personId, gradeId, ct);

    [HttpGet]
    public async Task<GetAbsencesVO[]> GetAbsencesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetAbsencesAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<GetAbsencesDplrVO[]> GetAbsencesDplrAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] AbsenceType type,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetAbsencesDplrAsync(
            schoolYear,
            studentClassBookId,
            personId,
            type,
            ct);

    [HttpGet]
    public async Task<GetAbsencesForCurriculumAndTypeVO[]> GetAbsencesForCurriculumAndTypeAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int curriculumId,
        [FromQuery] AbsenceType type,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        [FromServices] IAbsencesAggregateRepository absencesAggregateRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        SysRole sysRoleId = token.SelectedRole.SysRoleId;

        var absences = await studentInfoClassBooksQueryRepository.GetAbsencesForCurriculumAndTypeAsync(
            schoolYear,
            studentClassBookId,
            personId,
            curriculumId,
            type,
            ct);

        var notReadByParentAbsenceIds = absences.Where(a => !a.IsReadFromParent).Select(a => a.AbsenceId).ToArray();

        if (sysRoleId == SysRole.Parent && notReadByParentAbsenceIds.Any())
        {
            foreach (var absence in await absencesAggregateRepository.FindAllByIdsAsync(schoolYear, notReadByParentAbsenceIds, ct))
            {
                absence.SetAsReadFromParent();
            }

            await unitOfWork.SaveAsync(ct);
        }

        return absences;
    }

    [HttpGet]
    public async Task<GetAttendanceMonthStatsVO[]> GetAttendancesMonthStatsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetAttendancesMonthStatsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<GetAttendancesVO[]> GetAttendancesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] AttendanceType type,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetAttendancesAsync(
            schoolYear,
            studentClassBookId,
            personId,
            type,
            year,
            month,
            ct);

    [HttpGet]
    public async Task<GetCurriculumForTopicsVO[]> GetCurriculumForTopicsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetCurriculumForTopicsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet("{curriculumId:int}")]
    public async Task<GetTopicsVO[]> GetTopicsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromRoute] int curriculumId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetTopicsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            curriculumId,
            ct);

    [HttpGet]
    public async Task<GetRemarksVO[]> GetRemarksAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetRemarksAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<ActionResult<GetRemarksForCurriculumAndTypeVO[]>> GetRemarksForCurriculumAndTypeAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int curriculumId,
        [FromQuery] RemarkType type,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        [FromServices] IRemarksAggregateRepository remarksAggregateRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        SysRole sysRoleId = token.SelectedRole.SysRoleId;

        var remarks = await studentInfoClassBooksQueryRepository.GetRemarksForCurriculumAndTypeAsync(schoolYear, studentClassBookId, personId, curriculumId, type, ct);

        var notReadByParentRemarkIds = remarks.Where(a => !a.IsReadFromParent).Select(a => a.RemarkId).ToArray();

        if (sysRoleId == SysRole.Parent && notReadByParentRemarkIds.Any())
        {
            foreach (var remark in await remarksAggregateRepository.FindAllByIdsAsync(schoolYear, notReadByParentRemarkIds, ct))
            {
                remark.SetAsReadFromParent();
            }

            await unitOfWork.SaveAsync(ct);
        }

        return remarks;
    }

    [HttpGet("{year:int}/{weekNumber:int}")]
    public async Task<GetClassBookScheduleTableForWeekVO> GetScheduleAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromRoute] int year,
        [FromRoute] int weekNumber,
        [FromQuery] bool showIndividualCurriculum,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetScheduleAsync(
            schoolYear,
            studentClassBookId,
            personId,
            year,
            weekNumber,
            showIndividualCurriculum,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetParentMeetingsVO>> GetParentMeetingsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetParentMeetingsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetExamsVO>> GetExamsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetExamsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetIndividualWorksVO>> GetIndividualWorksAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetIndividualWorksAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetSanctionsVO>> GetSanctionsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetSanctionsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetSupportsVO>> GetSupportsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetSupportsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet("{supportId:int}")]
    public async Task<GetSupportVO> GetSupportAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromRoute] int supportId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetSupportAsync(
            schoolYear,
            studentClassBookId,
            personId,
            supportId,
            ct);

    [HttpGet("{supportId:int}/activities")]
    public async Task<TableResultVO<GetSupportActivitiesVO>> GetSupportActivitiesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromRoute] int supportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetSupportActivitiesAsync(
            schoolYear,
            studentClassBookId,
            personId,
            supportId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetNotesVO>> GetNotesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetNotesAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<GetGradeResultVO> GetGradeResultsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetGradeResultAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<GetFirstGradeResultsVO?> GetFirstGradeResultsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetFirstGradeResultsOrDefaultAsync(
            schoolYear,
            studentClassBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetPgResultsVO>> GetPgResultsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int studentClassBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetPgResultsAsync(
            schoolYear,
            studentClassBookId,
            personId,
            offset,
            limit,
            ct);
}
