namespace SB.ExtApi;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class ParentMeetingsController : ClassBookSectionController
{
    /// <summary>{{ParentMeetings.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{ParentMeetings.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<ParentMeetingDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.ParentMeetingsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{ParentMeetings.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateParentMeetingAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]ParentMeetingDO parentMeeting,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateParentMeetingCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                Date = parentMeeting.Date,
                StartTime = parentMeeting.StartTime,
                Location = parentMeeting.Location,
                Title = parentMeeting.Title,
                Description = parentMeeting.Description,
            },
            ct);

    /// <summary>{{ParentMeetings.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="parentMeetingId">{{Common.ParentMeetingId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{parentMeetingId:int}")]
    public async Task UpdateParentMeetingAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int parentMeetingId,
        [FromBody]ParentMeetingDO parentMeeting,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateParentMeetingCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ParentMeetingId = parentMeetingId,

                Date = parentMeeting.Date,
                StartTime = parentMeeting.StartTime,
                Location = parentMeeting.Location,
                Title = parentMeeting.Title,
                Description = parentMeeting.Description,
            },
            ct);

    /// <summary>{{ParentMeetings.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="parentMeetingId">{{Common.ParentMeetingId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{parentMeetingId:int}")]
    public async Task RemoveParentMeetingAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int parentMeetingId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveParentMeetingCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ParentMeetingId = parentMeetingId,
            },
            ct);
}
