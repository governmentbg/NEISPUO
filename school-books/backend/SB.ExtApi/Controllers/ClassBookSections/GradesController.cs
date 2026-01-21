namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class GradesController : ClassBookSectionController
{
    /// <summary>{{Grades.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Grades.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<GradeDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.GradesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Grades.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateGradeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]GradeDO grade,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateGradeExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = grade.PersonId,
                CurriculumId = grade.CurriculumId,
                Date = grade.Date,
                Category = grade.Category,
                Type = grade.Type,
                DecimalGrade = grade.DecimalGrade,
                QualitativeGrade = grade.QualitativeGrade,
                SpecialGrade = grade.SpecialGrade,
                Comment = grade.Comment,
                ScheduleLessonId = grade.ScheduleLessonId,
                Term = grade.Term,
            },
            ct);

    /// <summary>{{Grades.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="gradeId">{{Common.GradeId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{gradeId:int}")]
    public async Task RemoveGradeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int gradeId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveGradeExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeId = gradeId,
            },
            ct);

}
