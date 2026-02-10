namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class AdditionalActivitiesController : ClassBookSectionController
{
    /// <summary>{{AdditionalActivities.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{AdditionalActivities.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<AdditionalActivityDO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.AdditionalActivitiesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{AdditionalActivities.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateAdditionalActivityAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromBody] AdditionalActivityDO additionalActivity,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateAdditionalActivityCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                Year = additionalActivity.Year,
                WeekNumber= additionalActivity.WeekNumber,
                Activity = additionalActivity.Activity
            },
            ct);

    /// <summary>{{AdditionalActivities.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="additionalActivityId">{{Common.AdditionalActivityId}}</param>
    /// /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{additionalActivityId:int}")]
    public async Task UpdateAdditionalActivityAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int additionalActivityId,
        [FromBody] AdditionalActivityDO additionalActivity,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateAdditionalActivityCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AdditionalActivityId = additionalActivityId,

                Activity = additionalActivity.Activity
            },
            ct);

    /// <summary>{{AdditionalActivities.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="additionalActivityId">{{Common.AdditionalActivityId}}</param>
    /// /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{additionalActivityId:int}")]
    public async Task RemoveAdditionalActivityAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int additionalActivityId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveAdditionalActivityCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AdditionalActivityId = additionalActivityId,
            },
            ct);
}
