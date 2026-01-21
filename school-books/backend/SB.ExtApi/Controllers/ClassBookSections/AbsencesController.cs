namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class AbsencesController : ClassBookSectionController
{
    /// <summary>{{Absences.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Absences.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<AbsenceDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.AbsencesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Absences.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]AbsenceDO absence,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateAbsenceExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = absence.PersonId,
                Date = absence.Date,
                Type = absence.Type,
                ExcusedReason = absence.ExcusedReason,
                ExcusedReasonComment = absence.ExcusedReasonComment,
                ScheduleLessonId = absence.ScheduleLessonId
            },
            ct);

    /// <summary>{{Absences.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="absenceId">{{Common.AbsenceId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{absenceId:int}")]
    public async Task RemoveAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int absenceId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IsExternal = true,

                AbsenceId = absenceId,
            },
            ct);

    /// <summary>{{Absences.Excuse.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="absenceId">{{Common.AbsenceId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPost("excuse/{absenceId:int}")]
    public async Task ExcuseAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int absenceId,
        [FromBody]ExcusedReasonDO excusedReason,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new ExcuseAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                AbsenceId = absenceId,
                ExcusedReasonId = excusedReason.ExcusedReason,
                ExcusedReasonComment = excusedReason.ExcusedReasonComment,
            },
            ct);
}
