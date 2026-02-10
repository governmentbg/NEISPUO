namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class StudentClassNumbersController : ClassBookSectionController
{
    /// <summary>{{StudentClassNumbers.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{StudentClassNumbers.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<StudentClassNumberDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.StudentClassNumbersGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{StudentClassNumbers.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="personId">{{Common.PersonId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{personId:int}")]
    public async Task UpdateStudentClassNumberAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int personId,
        [FromBody]StudentClassNumberDO studentClassNumber,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateClassBookStudentClassNumberCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = personId,
                ClassNumber = studentClassNumber.ClassNumber,
            },
            ct);
}
