namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class ReplrParticipationsController : ClassBookSectionController
{
    /// <summary>{{ReplrParticipations.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{ReplrParticipations.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<ReplrParticipationDO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.ReplrParticipationsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{ReplrParticipations.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateReplrParticipationAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromBody] ReplrParticipationDO replrParticipation,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateReplrParticipationCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                ReplrParticipationTypeId = replrParticipation.ReplrParticipationTypeId,
                Date = replrParticipation.Date,
                Topic = replrParticipation.Topic,
                InstitutionId = replrParticipation.InstId,
                Attendees = replrParticipation.Attendees
            },
            ct);

    /// <summary>{{ReplrParticipations.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="replrParticipationId">{{Common.ReplrParticipationId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{replrParticipationId:int}")]
    public async Task UpdateReplrParticipationAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int replrParticipationId,
        [FromBody] ReplrParticipationDO replrParticipation,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateReplrParticipationCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ReplrParticipationId = replrParticipationId,

                ReplrParticipationTypeId = replrParticipation.ReplrParticipationTypeId,
                Date = replrParticipation.Date,
                Topic = replrParticipation.Topic,
                InstitutionId = replrParticipation.InstId,
                Attendees = replrParticipation.Attendees
            },
            ct);

    /// <summary>{{ReplrParticipations.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="replrParticipationId">{{Common.ReplrParticipationId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{replrParticipationId:int}")]
    public async Task RemoveReplrParticipationAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int replrParticipationId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveReplrParticipationCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ReplrParticipationId = replrParticipationId,
            },
            ct);
}
