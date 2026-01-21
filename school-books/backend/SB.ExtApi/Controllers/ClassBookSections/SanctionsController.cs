namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class SanctionsController : ClassBookSectionController
{
    /// <summary>{{Sanctions.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Sanctions.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<SanctionDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.SanctionsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Sanctions.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateSanctionAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]SanctionDO sanction,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateSanctionCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = sanction.PersonId,
                SanctionTypeId = sanction.Type,
                OrderNumber = sanction.OrderNumber,
                OrderDate = sanction.OrderDate,
                StartDate = sanction.StartDate,
                EndDate = sanction.EndDate,
                Description = sanction.Description,
                CancelOrderNumber = sanction.CancelOrderNumber,
                CancelOrderDate = sanction.CancelOrderDate,
                CancelReason = sanction.CancelReason,
                RuoOrderNumber = sanction.RuoOrderNumber,
                RuoOrderDate = sanction.RuoOrderDate,
            },
            ct);

    /// <summary>{{Sanctions.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="sanctionId">{{Common.SanctionId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{sanctionId:int}")]
    public async Task UpdateSanctionAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int sanctionId,
        [FromBody]SanctionDO sanction,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateSanctionCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SanctionId = sanctionId,

                SanctionTypeId = sanction.Type,
                OrderNumber = sanction.OrderNumber,
                OrderDate = sanction.OrderDate,
                StartDate = sanction.StartDate,
                EndDate = sanction.EndDate,
                Description = sanction.Description,
                CancelOrderNumber = sanction.CancelOrderNumber,
                CancelOrderDate = sanction.CancelOrderDate,
                CancelReason = sanction.CancelReason,
                RuoOrderNumber = sanction.RuoOrderNumber,
                RuoOrderDate = sanction.RuoOrderDate,
            },
            ct);

    /// <summary>{{Sanctions.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="sanctionId">{{Common.SanctionId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{sanctionId:int}")]
    public async Task RemoveSanctionAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int sanctionId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSanctionCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SanctionId = sanctionId,
            },
            ct);
}
