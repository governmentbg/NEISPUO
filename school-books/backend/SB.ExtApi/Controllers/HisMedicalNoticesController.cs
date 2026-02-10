namespace SB.ExtApi;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

[ApiController]
[Authorize(Policy = Policies.HisMedicalNoticesAccess)]
[Route("[controller]")]
public class HisMedicalNoticesController
{
    /// <summary>{{HisMedicalNotices.Post.Summary}}</summary>
    /// <param name="hisMedicalNotices">{{Common.HisMedicalNoticesForCreation}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPost]
    public async Task PostAsync(
        [FromBody] HisMedicalNoticeDO[] hisMedicalNotices,
        [FromServices] IMediator mediator,
        CancellationToken ct)
        => await mediator.Send(
            new CreateHisMedicalNoticesCommand
            {
                MedicalNotices = hisMedicalNotices,
            },
            ct);
}
