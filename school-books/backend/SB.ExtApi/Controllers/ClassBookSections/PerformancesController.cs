namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class PerformancesController : ClassBookSectionController
{
    /// <summary>{{Performances.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Performances.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<PerformanceDO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.PerformancesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Performances.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreatePerformanceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromBody] PerformanceDO performance,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreatePerformanceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PerformanceTypeId = performance.PerformanceTypeId,
                Name = performance.Name,
                Description = performance.Description,
                StartDate = performance.StartDate,
                EndDate = performance.EndDate,
                Location = performance.Location,
                StudentAwards = performance.StudentAwards
            },
            ct);

    /// <summary>{{Performances.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="performanceId">{{Common.PerformanceId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{performanceId:int}")]
    public async Task UpdatePerformanceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int performanceId,
        [FromBody] PerformanceDO performance,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdatePerformanceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PerformanceId = performanceId,

                PerformanceTypeId = performance.PerformanceTypeId,
                Name = performance.Name,
                Description = performance.Description,
                StartDate = performance.StartDate,
                EndDate = performance.EndDate,
                Location = performance.Location,
                StudentAwards = performance.StudentAwards
            },
            ct);

    /// <summary>{{Performances.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="performanceId">{{Common.PerformanceId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{performanceId:int}")]
    public async Task RemovePerformanceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int performanceId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemovePerformanceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PerformanceId = performanceId,
            },
            ct);
}
