namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class StudentSpecialNeedCurriculumsController : ClassBookSectionController
{
    /// <summary>{{StudentSpecialNeedCurriculums.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{StudentSpecialNeedCurriculums.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<StudentSpecialNeedCurriculumsDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.StudentSpecialNeedCurriculumsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{StudentSpecialNeedCurriculums.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="personId">{{Common.PersonId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{personId:int}")]
    public async Task UpdateStudentSpecialNeedCurriculumsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int personId,
        [FromBody]StudentSpecialNeedCurriculumsDO studentSpecialNeedCurriculums,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateClassBookStudentSpecialNeedCurriculumsCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = personId,
                SpecialNeedCurriculumIds = studentSpecialNeedCurriculums.StudentSpecialNeedCurriculumIds,
                HasSpecialNeedFirstGradeResult = studentSpecialNeedCurriculums.HasSpecialNeedFirstGradeResult
            },
            ct);
}
