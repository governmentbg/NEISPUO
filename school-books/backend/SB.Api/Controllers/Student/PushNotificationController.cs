namespace SB.Api.Controllers.Student;

using Lib.Net.Http.WebPush;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

[Authorize(Policy = Policies.StudentAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/[action]")]
public class PushNotificationsController
{
    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeToPushNotifications(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromBody] PushSubscription subscription,
        [FromServices] IPushSubscriptionsService pushSubscriptionsService,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        int sysUserId = token.SelectedRole.SysUserId;

        await pushSubscriptionsService.AddOrUpdateAsync(
            subscription,
            sysUserId,
            ct);

        return new OkResult();
    }
}
