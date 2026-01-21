namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IPersonMedicalNoticeQueryRepository;

public class PersonMedicalNoticesController : BookController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllByClassBookVO>>> GetAllByClassBookAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? studentPersonId,
        [FromQuery]DateTime? fromDate,
        [FromQuery]DateTime? toDate,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices]IPersonMedicalNoticeQueryRepository personMedicalNoticeQueryRepository,
        CancellationToken ct)
        => await personMedicalNoticeQueryRepository.GetAllByClassBookAsync(
            schoolYear,
            classBookId,
            studentPersonId,
            fromDate,
            toDate,
            offset,
            limit,
            ct);
}
