namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class StudentActivitiesController : ClassBookSectionController
{
    /// <summary>{{StudentActivities.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{StudentActivities.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<StudentActivitiesDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.StudentActivitiesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{StudentActivities.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="personId">{{Common.PersonId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{personId:int}")]
    public async Task UpdateStudentActivitiesAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int personId,
        [FromBody]StudentActivitiesDO studentActivities,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateClassBookStudentActivitiesCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = personId,
                Activities = studentActivities.Activities,
            },
            ct);
}
