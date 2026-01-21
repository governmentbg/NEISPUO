namespace SB.ExtApi;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

[ApiController]
[Authorize(Policy = Policies.MedicalNoticesAccess)]
[Route("[controller]")]
public class MedicalNoticesController
{
    /// <summary>{{MedicalNotices.GetNext.Summary}}</summary>
    /// <param name="next">{{MedicalNotices.GetNext.NextParam}}</param>
    /// <returns>{{MedicalNotices.GetNext.Returns}}</returns>
    [HttpGet("next/{next:int}")]
    public Task<MedicalNoticeBatchDO> GetNextAsync(
        [FromRoute][Range(100, 10000)] int next,
        [FromServices] IHisMedicalNoticesQueryRepository hisMedicalNoticesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => hisMedicalNoticesQueryRepository.GetNextWithReadReceiptsAndSaveAsync(
            httpContextAccessor.GetExtSystemId() ?? throw new Exception("Missing ExtSystemId."),
            next,
            ct);

    /// <summary>{{MedicalNotices.Acknowledge.Summary}}</summary>
    /// <param name="medicalNoticeIds">{{MedicalNotices.Acknowledge.MedicalNoticeIdsParam}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPost("acknowledge")]
    public async Task АcknowledgeAsync(
        [FromBody][Required] int[] medicalNoticeIds,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new АcknowledMedicalNoticesCommand
            {
                ExtSystemId = httpContextAccessor.GetExtSystemId() ?? throw new Exception("Missing ExtSystemId."),
                HisMedicalNoticeIds = medicalNoticeIds,
            },
            ct);
}
