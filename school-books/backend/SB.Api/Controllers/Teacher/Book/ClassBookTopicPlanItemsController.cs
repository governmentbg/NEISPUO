namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassBookTopicPlanItemsQueryRepository;
using static SB.Domain.ITopicPlanItemsExcelReaderWriter;

public class ClassBookTopicPlanItemsController : BookController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int curriculumId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IClassBookTopicPlanItemsQueryRepository classBookTopicPlanItemsQueryRepository,
        CancellationToken ct)
        => await classBookTopicPlanItemsQueryRepository.GetAllAsync(schoolYear, classBookId, curriculumId, offset, limit, ct);

    [HttpGet("{classBookTopicPlanItemId:int}")]
    public async Task<GetVO> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int classBookTopicPlanItemId,
        [FromServices] IClassBookTopicPlanItemsQueryRepository classBookTopicPlanItemsQueryRepository,
        CancellationToken ct)
        => await classBookTopicPlanItemsQueryRepository.GetAsync(schoolYear, classBookTopicPlanItemId, ct);

    [HttpPost("{curriculumId:int}")]
    public async Task<ActionResult> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromBody] CreateClassBookTopicPlanItemCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        _ = await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
            },
            ct);

        return new OkResult();
    }

    [HttpPost("{curriculumId:int}/{classBookTopicPlanItemId:int}")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromRoute] int classBookTopicPlanItemId,
        [FromBody] UpdateClassBookTopicPlanItemCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
                ClassBookTopicPlanItemId = classBookTopicPlanItemId
            },
            ct);

        return new OkResult();
    }

    [HttpPost("{curriculumId:int}/{classBookTopicPlanItemId:int}")]
    public async Task<ActionResult> UpdateTakenAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromRoute] int classBookTopicPlanItemId,
        [FromBody] UpdateTakenClassBookTopicPlanItemCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
                ClassBookTopicPlanItemId = classBookTopicPlanItemId
            },
            ct);

        return new OkResult();
    }

    [HttpDelete("{curriculumId:int}")]
    public async Task<ActionResult> RemoveAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveAllClassBookTopicPlanItemsCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
            },
            ct);

        return new OkResult();
    }

    [HttpDelete("{curriculumId:int}/{classBookTopicPlanItemId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromRoute] int classBookTopicPlanItemId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveClassBookTopicPlanItemCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
                ClassBookTopicPlanItemId = classBookTopicPlanItemId,
            },
            ct);

        return new OkResult();
    }

    [HttpGet("{curriculumId:int}")]
    public async Task<ActionResult> LoadFromTopicPlanAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromQuery] int topicPlanId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        _ = await mediator.Send(
            new CreateClassBookTopicPlanItemsFromTopicPlanLoadCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
                TopicPlanId = topicPlanId
            },
            ct);

        return new OkResult();
    }

    [HttpGet]
    public async Task<ActionResult> ImportFromExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int curriculumId,
        [FromQuery] int blobId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
        {
            return new ForbidResult();
        }

        _ = await mediator.Send(
            new CreateClassBookTopicPlanItemsFromExcelImportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
                BlobId = blobId
            },
            ct);

        return new OkResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{curriculumId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromServices] IClassBookTopicPlanItemsQueryRepository classBookTopicPlanItemsQueryRepository,
        [FromServices] ITopicPlanItemsExcelReaderWriter topicPlanItemsExcelReaderWriter,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        var topicPlanItems =
            await classBookTopicPlanItemsQueryRepository.GetExcelDataAsync(
                schoolYear,
                classBookId,
                curriculumId,
                ct);
        topicPlanItemsExcelReaderWriter.Write(
            topicPlanItems.Select(tpi => new TopicPlanItemDO(tpi.Number, tpi.Title, tpi.Note)).ToArray(),
            ms);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = $"тематично_разпределение.xlsx"
        };
    }
}
