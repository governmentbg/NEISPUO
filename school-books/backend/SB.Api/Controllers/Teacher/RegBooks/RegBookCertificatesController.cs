namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IRegBookCertificateQueryRepository;

public class RegBookCertificatesController : RegBooksController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] string? registrationNumberTotal,
        [FromQuery] string? registrationNumberYear,
        [FromQuery] DateTime? registrationDate,
        [FromQuery] string? fullName,
        [FromQuery] string? identifier,
        [FromQuery] int? basicDocumentId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IRegBookCertificateQueryRepository regBookCertificateQueryRepository,
        CancellationToken ct)
        => await regBookCertificateQueryRepository.GetAllAsync(
            schoolYear,
            instId,
            registrationNumberTotal,
            registrationNumberYear,
            registrationDate,
            fullName,
            identifier,
            basicDocumentId,
            offset,
            limit,
            ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int id,
        [FromServices] IRegBookCertificateQueryRepository regBookCertificateQueryRepository,
        CancellationToken ct)
        => await regBookCertificateQueryRepository.GetAsync(schoolYear, id, ct);
}
