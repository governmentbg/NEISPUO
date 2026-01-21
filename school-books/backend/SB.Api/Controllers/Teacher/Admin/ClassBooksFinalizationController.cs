namespace SB.Api;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SB.Domain;
using static SB.Domain.IClassBooksQueryRepository;

[Authorize(Policy = Policies.InstitutionAdminAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class ClassBooksFinalizationController
{
    [HttpGet]
    public async Task<GetAllForFinalizationVO[]> GetAllClassBooksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int[]? classBookIds,
        [FromServices] IClassBooksQueryRepository classBooksQueryRepository,
        CancellationToken ct)
        => await classBooksQueryRepository.GetAllForFinalizationAsync(schoolYear, instId, classBookIds, ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task FinalizeClassBooksAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody] FinalizeClassBooksCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [DisallowWhenInstHasCBExtProvider]
    [OpenApiBodyParameter("text/plain")]
    [HttpPost("{classBookId:int}/{classBookPrintId:int}")]
    public async Task SignClassBookPrintAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int classBookPrintId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new SignClassBookPrintCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                ClassBookId = classBookId,
                ClassBookPrintId = classBookPrintId,
                SignedClassBookPrintFileBase64 = httpContextAccessor.HttpContext!.Request.Body,
            },
            ct);

    public record FinalizeFormData(IFormFile? SignedClassBookPrintFile);

    [HttpPost]
    public async Task<GetAllForFinalizationVO> FinalizeClassBookWithSignedPdf(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromForm] FinalizeFormData form,
        [FromServices] IMediator mediator,
        [FromServices] IClassBooksQueryRepository classBooksQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        using Stream? signedClassBookPrintStream = form.SignedClassBookPrintFile?.OpenReadStream();

        int classBookId = await mediator.Send(
            new FinalizeClassBookWithSignedPdfCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                SignedClassBookPrintFile = signedClassBookPrintStream,
                SignedClassBookPrintFileName = form.SignedClassBookPrintFile?.FileName,
                ExtractClassBookIdFromMetadataOrFileName = true,
            },
            ct);

        var classBooks = await classBooksQueryRepository.GetAllForFinalizationAsync(
            schoolYear,
            instId,
            new[] { classBookId },
            ct);

        return classBooks.Single();
    }
}
