namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class FirstGradeResultsController : ClassBookSectionController
{
    /// <summary>{{FirstGradeResults.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{FirstGradeResults.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<FirstGradeResultDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.FirstGradeResultsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{FirstGradeResults.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateFirstGradeResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]FirstGradeResultDO firstGradeResult,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateFirstGradeResultExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = firstGradeResult.PersonId,
                QualitativeGrade = firstGradeResult.QualitativeGrade,
                SpecialGrade = firstGradeResult.SpecialGrade,
            },
            ct);

    /// <summary>{{FirstGradeResults.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="firstGradeResultId">{{Common.FirstGradeResultId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{firstGradeResultId:int}")]
    public async Task UpdateFirstGradeResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int firstGradeResultId,
        [FromBody]FirstGradeResultDO firstGradeResult,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateFirstGradeResultExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                FirstGradeResultId = firstGradeResultId,

                QualitativeGrade = firstGradeResult.QualitativeGrade,
                SpecialGrade = firstGradeResult.SpecialGrade,
            },
            ct);

    /// <summary>{{FirstGradeResults.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="firstGradeResultId">{{Common.FirstGradeResultId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{firstGradeResultId:int}")]
    public async Task RemoveFirstGradeResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int firstGradeResultId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveFirstGradeResultExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                FirstGradeResultId = firstGradeResultId,
            },
            ct);
}
