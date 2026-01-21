namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/{topicPlanId:int}/[action]")]
public partial class TopicPlanItemsExcelController
{
    [GeneratedRegex("[^0-9a-zA-Zа-яА-Я_\\-\\[\\]\\(\\)]")]
    private static partial Regex TopicPlanNameRegex();

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int topicPlanId,
        [FromServices]ITopicPlanItemExcelService topicPlanExcelService,
        [FromServices] ITopicPlansQueryRepository topicPlansQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var sysUserId = httpContextAccessor.GetSysUserId();

        var ms = new MemoryStream();

        await topicPlanExcelService.ExportAsync(sysUserId, topicPlanId, ms, ct);
        ms.Seek(0, SeekOrigin.Begin);

        var topicPlanName = (await topicPlansQueryRepository.GetAsync(sysUserId, topicPlanId, ct)).Name;

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = $"{TopicPlanNameRegex().Replace(topicPlanName, "_").Truncate(60)}.xlsx"
        };
    }

    public async Task<ActionResult> ImportFromExcelFileAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int topicPlanId,
        [FromQuery] int blobId,
        [FromServices] ITopicPlanItemExcelService topicPlanExcelService,
        CancellationToken ct)
    {
        var errors = await topicPlanExcelService.ImportFromExcelBlobAsync(blobId, topicPlanId, ct);

        if (errors.Any())
        {
            throw new DomainValidationException(new[] { "has_import_errors" }, errors);
        }

        return new OkResult();
    }
}
