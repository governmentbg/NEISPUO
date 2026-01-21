namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class AttendancesController : ClassBookSectionController
{
    /// <summary>{{Attendances.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Attendances.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<AttendanceDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.AttendancesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Attendances.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateAttendanceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]AttendanceDO attendance,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateAttendanceExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = attendance.PersonId,
                Date = attendance.Date,
                Type = attendance.Type,
                ExcusedReasonId = attendance.ExcusedReason,
                ExcusedReasonComment = attendance.ExcusedReasonComment,
            },
            ct);

    /// <summary>{{Attendances.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="attendanceId">{{Common.AttendanceId}}</param>
    /// /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{attendanceId:int}")]
    public async Task RemoveAttendanceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int attendanceId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveAttendanceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AttendanceId = attendanceId,
            },
            ct);

    /// <summary>{{Attendances.Excuse.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="attendanceId">{{Common.AttendanceId}}</param>
    /// /// <returns>{{Common.NoResponse}}</returns>
    [HttpPost("excuse/{attendanceId:int}")]
    public async Task ExcuseAttendanceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int attendanceId,
        [FromBody]ExcusedReasonDO excusedReason,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new ExcuseAttendanceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                AttendanceId = attendanceId,
                ExcusedReasonId = excusedReason.ExcusedReason,
                ExcusedReasonComment = excusedReason.ExcusedReasonComment,
            },
            ct);
}
