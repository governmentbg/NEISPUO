namespace SB.Api.Controllers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IConversationsQueryRepository;

[Authorize(Policy = Policies.AuthenticatedAccess)]
[ApiController]
[Route("api/[controller]/[action]")]
public class ConversationsController
{
    [HttpGet]
    public async Task<ActionResult<GetUnreadConversationsVO[]>> GetUnreadConversations(
        [FromQuery][SuppressMessage("", "IDE0060")] int? offset,
        [FromQuery] int? limit,
        [FromServices] IConversationsQueryRepository conversationQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await conversationQueryRepository.GetUnreadConversationsAsync(
            httpContextAccessor.GetSysUserId(),
            limit,
            ct);

    [HttpGet]
    public async Task<ActionResult<GetConversationsVO[]>> GetConversationsAsync(
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IConversationsQueryRepository conversationQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await conversationQueryRepository.GetConversationsAsync(
            httpContextAccessor.GetSysUserId(),
            offset,
            limit,
            ct);

    [HttpGet]
    public async Task<ActionResult<GetConversationInfoVO>> GetConversationInfoAsync(
        [FromQuery] int? conversationSchoolYear,
        [FromQuery] int? conversationId,
        [FromServices] IConversationsQueryRepository conversationQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (conversationSchoolYear == null)
        {
            throw new ArgumentNullException(nameof(conversationSchoolYear));
        }

        if (conversationId == null)
        {
            throw new ArgumentNullException(nameof(conversationId));
        }

        var sysUser = httpContextAccessor.GetSysUserId();

        if (!await conversationQueryRepository.CheckIfUserBelongToTheConversation(
                sysUser,
                conversationSchoolYear.GetValueOrDefault(),
                conversationId.GetValueOrDefault(),
                ct))
        {
            return new ForbidResult();
        }

        return await conversationQueryRepository.GetConversationInfoAsync(
            conversationSchoolYear.GetValueOrDefault(),
            conversationId.GetValueOrDefault(),
            sysUser,
            ct);
    }

    [HttpGet]
    public async Task<ActionResult<GetConversationParticipantsVO[]>> GetConversationParticipantsAsync(
        [FromQuery] int? conversationSchoolYear,
        [FromQuery] int? conversationId,
        [FromServices] IConversationsQueryRepository conversationQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (conversationSchoolYear == null)
        {
            throw new ArgumentNullException(nameof(conversationSchoolYear));
        }

        if (conversationId == null)
        {
            throw new ArgumentNullException(nameof(conversationId));
        }

        var sysUser = httpContextAccessor.GetSysUserId();

        if (!await conversationQueryRepository.CheckIfUserBelongToTheConversation(
                sysUser,
                conversationSchoolYear.GetValueOrDefault(),
                conversationId.GetValueOrDefault(),
                ct))
        {
            return new ForbidResult();
        }

        return await conversationQueryRepository.GetConversationParticipantsAsync(
            conversationSchoolYear.GetValueOrDefault(),
            conversationId.GetValueOrDefault(),
            sysUser,
            ct);
    }

    [HttpGet]
    public async Task<ActionResult<GetConversationMessagesVO[]>> GetConversationMessagesAsync(
        [FromQuery] int? conversationSchoolYear,
        [FromQuery] int? conversationId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IConversationsService conversationsService,
        [FromServices] IConversationsQueryRepository conversationQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (conversationSchoolYear == null)
        {
            throw new ArgumentNullException(nameof(conversationSchoolYear));
        }

        if (conversationId == null)
        {
            throw new ArgumentNullException(nameof(conversationId));
        }

        if (!await conversationQueryRepository.CheckIfUserBelongToTheConversation(
                httpContextAccessor.GetSysUserId(),
                conversationSchoolYear.GetValueOrDefault(),
                conversationId.GetValueOrDefault(),
                ct))
        {
            return new ForbidResult();
        }

        return await conversationsService.GetConversationMessagesAsync(
            httpContextAccessor.GetSysUserId(),
            conversationSchoolYear.GetValueOrDefault(),
            conversationId.GetValueOrDefault(),
            offset,
            limit,
            ct);
    }

    [HttpPost]
    public async Task<CreatedConversationVO> CreateConversationAsync(
        [FromQuery] int instId,
        [FromBody] CreateConversationCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    => await mediator.Send(
        command with
        {
            Creator = new CreateConversationCommandCreator
            {
                SysRoleId = httpContextAccessor.GetToken().SelectedRole.SysRoleId,
                SysUserId = httpContextAccessor.GetSysUserId()
            },
            InstId = instId,
        },
        ct);

    [HttpPost]
    public async Task<int> AddMessageAsync(
        [FromQuery] int conversationSchoolYear,
        [FromBody] AddConversationMessageCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = conversationSchoolYear,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
}
