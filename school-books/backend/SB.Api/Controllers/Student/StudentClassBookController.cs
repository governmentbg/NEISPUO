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
using static SB.Domain.IStudentClassBooksQueryRepository;

[Authorize(Policy = Policies.StudentClassBookAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{classBookId:int}/{personId:int}/[action]")]
public class StudentClassBookController
{
    [HttpGet]
    public async Task<GetClassBookInfoVO> GetClassBookInfoAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetClassBookInfoAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

    [HttpGet]
    public async Task<GetGradesVO[]> GetGradesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetGradesAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet("{gradeId:int}")]
    public async Task<GetGradeVO> GetGradeAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int gradeId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetGradeAsync(schoolYear, classBookId, personId, gradeId, ct);

    [HttpPost]
    public async Task<ActionResult> SetGradesAsReadAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute][SuppressMessage("", "IDE0060")] int personId,
        [FromQuery] int[] gradeIds,
        [FromServices] IGradesAggregateRepository gradesAggregateRepository,
        [FromServices] IUnitOfWork unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        SysRole sysRoleId = token.SelectedRole.SysRoleId;

        if (sysRoleId != SysRole.Parent)
        {
            return new ForbidResult();
        }

        foreach (var grade in await gradesAggregateRepository.FindAllByIdsAsync(schoolYear, gradeIds, ct))
        {
            grade.SetAsReadFromParent();
        }

        await unitOfWork.SaveAsync(ct);

        return new OkResult();
    }

    [HttpGet]
    public async Task<GetAbsencesVO[]> GetAbsencesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetAbsencesAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<GetAbsencesDplrVO[]> GetAbsencesDplrAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] AbsenceType type,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetAbsencesDplrAsync(
            schoolYear,
            classBookId,
            personId,
            type,
            ct);

    [HttpGet]
    public async Task<GetAbsencesForCurriculumAndTypeVO[]> GetAbsencesForCurriculumAndTypeAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int curriculumId,
        [FromQuery] AbsenceType type,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        [FromServices] IAbsencesAggregateRepository absencesAggregateRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        SysRole sysRoleId = token.SelectedRole.SysRoleId;

        var absences = await studentClassBooksQueryRepository.GetAbsencesForCurriculumAndTypeAsync(
            schoolYear,
            classBookId,
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
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetAttendancesMonthStatsAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<GetAttendancesVO[]> GetAttendancesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] AttendanceType type,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetAttendancesAsync(
            schoolYear,
            classBookId,
            personId,
            type,
            year,
            month,
            ct);

    [HttpGet]
    public async Task<GetCurriculumForTopicsVO[]> GetCurriculumForTopicsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetCurriculumForTopicsAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet("{curriculumId:int}")]
    public async Task<GetTopicsVO[]> GetTopicsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int curriculumId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetTopicsAsync(
            schoolYear,
            classBookId,
            personId,
            curriculumId,
            ct);

    [HttpGet]
    public async Task<GetRemarksVO[]> GetRemarksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetRemarksAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<ActionResult<GetRemarksForCurriculumAndTypeVO[]>> GetRemarksForCurriculumAndTypeAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int curriculumId,
        [FromQuery] RemarkType type,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        [FromServices] IRemarksAggregateRepository remarksAggregateRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        SysRole sysRoleId = token.SelectedRole.SysRoleId;

        var remarks = await studentClassBooksQueryRepository.GetRemarksForCurriculumAndTypeAsync(schoolYear, classBookId, personId, curriculumId, type, ct);

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
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int year,
        [FromRoute] int weekNumber,
        [FromQuery] bool showIndividualCurriculum,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetScheduleAsync(
            schoolYear,
            classBookId,
            personId,
            year,
            weekNumber,
            showIndividualCurriculum,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetParentMeetingsVO>> GetParentMeetingsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetParentMeetingsAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetExamsVO>> GetExamsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetExamsAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetIndividualWorksVO>> GetIndividualWorksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetIndividualWorksAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetSanctionsVO>> GetSanctionsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetSanctionsAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetSupportsVO>> GetSupportsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetSupportsAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet("{supportId:int}")]
    public async Task<GetSupportVO> GetSupportAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int supportId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetSupportAsync(
            schoolYear,
            classBookId,
            personId,
            supportId,
            ct);

    [HttpGet("{supportId:int}/activities")]
    public async Task<TableResultVO<GetSupportActivitiesVO>> GetSupportActivitiesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromRoute] int supportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetSupportActivitiesAsync(
            schoolYear,
            classBookId,
            personId,
            supportId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetNotesVO>> GetNotesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetNotesAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<GetGradeResultVO> GetGradeResultsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetGradeResultAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<GetFirstGradeResultsVO?> GetFirstGradeResultsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetFirstGradeResultsOrDefaultAsync(
            schoolYear,
            classBookId,
            personId,
            ct);

    [HttpGet]
    public async Task<TableResultVO<GetPgResultsVO>> GetPgResultsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetPgResultsAsync(
            schoolYear,
            classBookId,
            personId,
            offset,
            limit,
            ct);
}
