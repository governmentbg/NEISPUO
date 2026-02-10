namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static SB.Domain.IStudentSettingsQueryRepository;

[Authorize(Policy = Policies.StudentAccess)]
[ApiController]
[Route("api/[controller]/[action]")]
public class StudentSettingsController
{
    [HttpGet]
    public async Task<ActionResult<GetVO?>> GetStudentSettingsAsync(
        [FromServices] IStudentSettingsQueryRepository studentSettingsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    => await studentSettingsQueryRepository.GetStudentSettings(
            httpContextAccessor.GetPersonId()!.Value,
            ct);

    [HttpPost]
    public async Task<int> CreateUpdateStudentSettingsAsync(
        [FromBody] CreateUpdateStudentSettingsCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SysUserId = httpContextAccessor.GetSysUserId(),
                UserPersonId = httpContextAccessor.GetPersonId()
            },
            ct);
}
