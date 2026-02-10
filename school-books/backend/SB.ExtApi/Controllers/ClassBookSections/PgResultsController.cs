namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class PgResultsController : ClassBookSectionController
{
    /// <summary>{{PgResults.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{PgResults.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<PgResultDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.PgResultsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{PgResults.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreatePgResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]PgResultDO pgResult,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreatePgResultCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = pgResult.PersonId,
                SubjectId = pgResult.SubjectId,
                #pragma warning disable CS0618 // // member is obsolete
                CurriculumId = pgResult.CurriculumId,
                #pragma warning restore CS0618 // // member is obsolete
                StartSchoolYearResult = pgResult.StartSchoolYearResult,
                EndSchoolYearResult = pgResult.EndSchoolYearResult,
            },
            ct);

    /// <summary>{{PgResults.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="pgResultId">{{Common.PgResultId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{pgResultId:int}")]
    public async Task UpdatePgResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int pgResultId,
        [FromBody]PgResultDO pgResult,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdatePgResultCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PgResultId = pgResultId,

                StartSchoolYearResult = pgResult.StartSchoolYearResult,
                EndSchoolYearResult = pgResult.EndSchoolYearResult,
            },
            ct);

    /// <summary>{{PgResults.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="pgResultId">{{Common.PgResultId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{pgResultId:int}")]
    public async Task RemovePgResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int pgResultId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemovePgResultCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PgResultId = pgResultId,
            },
            ct);
}
