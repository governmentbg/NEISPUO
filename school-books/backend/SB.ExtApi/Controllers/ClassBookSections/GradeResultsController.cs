namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class GradeResultsController : ClassBookSectionController
{
    /// <summary>{{GradeResults.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{GradeResults.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<GradeResultDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.GradeResultsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{GradeResults.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateGradeResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]GradeResultDO gradeResult,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateGradeResultExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = gradeResult.PersonId,
                InitialResultType = gradeResult.InitialResultType,
                RetakeExamCurriculums = gradeResult.RetakeExamCurriculums.Select(c =>
                    new CreateUpdateGradeResultExtApiCommandCurriculum
                    {
                        CurriculumId = c.CurriculumId,
                        Session1Grade = c.Session1Grade,
                        Session1NoShow = c.Session1NoShow,
                        Session2Grade = c.Session2Grade,
                        Session2NoShow = c.Session2NoShow,
                        Session3Grade = c.Session3Grade,
                        Session3NoShow = c.Session3NoShow,
                    })
                    .ToArray(),
                FinalResultType = gradeResult.FinalResultType,
            },
            ct);

    /// <summary>{{GradeResults.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="gradeResultId">{{Common.GradeResultId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{gradeResultId:int}")]
    public async Task UpdateGradeResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int gradeResultId,
        [FromBody]GradeResultDO gradeResult,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateGradeResultExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeResultId = gradeResultId,

                InitialResultType = gradeResult.InitialResultType,
                RetakeExamCurriculums = gradeResult.RetakeExamCurriculums.Select(c =>
                    new CreateUpdateGradeResultExtApiCommandCurriculum
                    {
                        CurriculumId = c.CurriculumId,
                        Session1Grade = c.Session1Grade,
                        Session1NoShow = c.Session1NoShow,
                        Session2Grade = c.Session2Grade,
                        Session2NoShow = c.Session2NoShow,
                        Session3Grade = c.Session3Grade,
                        Session3NoShow = c.Session3NoShow,
                    })
                    .ToArray(),
                FinalResultType = gradeResult.FinalResultType,
            },
            ct);

    /// <summary>{{GradeResults.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="gradeResultId">{{Common.GradeResultId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{gradeResultId:int}")]
    public async Task RemoveGradeResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int gradeResultId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveGradeResultExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeResultId = gradeResultId,
            },
            ct);
}
