namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class StudentGradelessCurriculumsController : ClassBookSectionController
{
    /// <summary>{{StudentGradelessCurriculums.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{StudentGradelessCurriculums.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<StudentGradelessCurriculumsDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.StudentGradelessCurriculumsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{StudentGradelessCurriculums.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="personId">{{Common.PersonId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{personId:int}")]
    public async Task UpdateStudentGradelessCurriculumsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int personId,
        [FromBody]StudentGradelessCurriculumsDO studentGradelessCurriculums,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateClassBookStudentGradelessCurriculumsCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = personId,
                GradelessCurriculums = studentGradelessCurriculums.GradelessCurriculums
                    ?.Select(gc =>
                        new UpdateClassBookStudentGradelessCurriculumsCommandGradeless
                        {
                            CurriculumId = gc.CurriculumId,
                            WithoutFirstTermGrade = gc.WithoutFirstTermGrade,
                            WithoutSecondTermGrade = gc.WithoutSecondTermGrade,
                            WithoutFinalGrade = gc.WithoutFinalGrade
                        })
                    ?.ToArray(),
            },
            ct);
}
