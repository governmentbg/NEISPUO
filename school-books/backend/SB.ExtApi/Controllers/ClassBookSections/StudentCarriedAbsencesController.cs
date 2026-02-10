namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class StudentCarriedAbsencesController : ClassBookSectionController
{
    /// <summary>{{StudentCarriedAbsences.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{StudentCarriedAbsences.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<StudentCarriedAbsencesDO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.StudentCarriedAbsencesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{StudentCarriedAbsences.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="personId">{{Common.PersonId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{personId:int}")]
    public async Task UpdateStudentCarriedAbsencesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int personId,
        [FromBody] StudentCarriedAbsencesDO studentCarriedAbsences,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateClassBookStudentCarriedAbsencesCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = personId,
                FirstTermExcusedCount = studentCarriedAbsences.FirstTermExcusedCount,
                FirstTermUnexcusedCount = studentCarriedAbsences.FirstTermUnexcusedCount,
                FirstTermLateCount = studentCarriedAbsences.FirstTermLateCount,
                SecondTermExcusedCount = studentCarriedAbsences.SecondTermExcusedCount,
                SecondTermUnexcusedCount = studentCarriedAbsences.SecondTermUnexcusedCount,
                SecondTermLateCount = studentCarriedAbsences.SecondTermLateCount
            },
            ct);
}
