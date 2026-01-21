namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Domain.IConversationParticipantsNomRepository;

[Authorize(Policy = Policies.ConversationAccess)]
[ApiController]
[Route("api/[controller]/{instId:int}/[action]")]
public class ConversationParticipantsNomController
{
    [HttpGet]
    public async Task<ConversationParticipantsNomVO[]> GetNomsByIdAsync(
        [FromRoute] int instId,
        [FromQuery] ConversationParticipantsNomVOParticipant[] ids,
        [FromServices] IConversationParticipantsNomRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(instId, ids, ct);

    [HttpGet]
    public async Task<ConversationParticipantsNomVO[]> GetNomsByTermAsync(
        [FromRoute] int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IConversationParticipantsNomRepository repository,
        CancellationToken ct)
    {
        var sysRole = httpContextAccessor.GetToken().SelectedRole.SysRoleId;
        var userPersonId = httpContextAccessor.GetPersonId();
        var studentIds = sysRole == SysRole.Parent ? httpContextAccessor.GetStudentPersonIds() : null;

        return await repository.GetNomsByTermAsync(
            instId,
            sysRole,
            userPersonId,
            studentIds,
            term,
            offset,
            limit,
            ct);
    }
}
