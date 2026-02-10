namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class IndividualWorksController : ClassBookSectionController
{
    /// <summary>{{IndividualWorks.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{IndividualWorks.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<IndividualWorkDO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.IndividualWorksGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{IndividualWorks.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateIndividualWorkAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromBody] IndividualWorkDO individualWork,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateIndividualWorkCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                PersonId = individualWork.PersonId,
                Date = individualWork.Date,
                IndividualWorkActivity = individualWork.IndividualWorkActivity
            },
            ct);

    /// <summary>{{IndividualWorks.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="individualWorkId">{{Common.IndividualWorkId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{individualWorkId:int}")]
    public async Task UpdateIndividualWorkAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int individualWorkId,
        [FromBody] IndividualWorkDO individualWork,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateIndividualWorkCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IndividualWorkId = individualWorkId,

                Date = individualWork.Date,
                IndividualWorkActivity = individualWork.IndividualWorkActivity
            },
            ct);

    /// <summary>{{IndividualWorks.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="individualWorkId">{{Common.IndividualWorkId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{individualWorkId:int}")]
    public async Task RemoveIndividualWorkAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int individualWorkId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveIndividualWorkCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IndividualWorkId = individualWorkId,
            },
            ct);
}
