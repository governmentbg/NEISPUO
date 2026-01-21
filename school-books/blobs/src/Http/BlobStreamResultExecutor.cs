namespace SB.Blobs;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

public class BlobStreamResultExecutor : FileResultExecutorBase, IActionResultExecutor<BlobStreamResult>
{
    class DummyFileResult : FileResult
    {
        public DummyFileResult(string contentType)
            : base(contentType)
        {
        }
    }

    private FileExtensionContentTypeProvider contentTypeProvider = new();
    private IOptions<DataOptions> dataOptions;

    public BlobStreamResultExecutor(ILoggerFactory loggerFactory, IOptions<DataOptions> dataOptions)
        : base(CreateLogger<BlobStreamResultExecutor>(loggerFactory))
    {
        this.dataOptions = dataOptions;
    }

    public async Task ExecuteAsync(ActionContext context, BlobStreamResult result)
    {
        context = context ?? throw new ArgumentNullException(nameof(context));
        result = result ?? throw new ArgumentNullException(nameof(result));

        try
        {
            var ct = context.HttpContext.RequestAborted;

            await using SqlConnection conn = new(this.dataOptions.Value.GetConnectionString());
            await conn.OpenAsync(ct);

            BlobReadRepository blobReadRepository = new(conn);

            var blobInfo = await blobReadRepository.GetBlobInfoAsync(result.BlobId, ct);

            if (blobInfo == null)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var fileResult =
                new DummyFileResult(this.GetFileContentType(blobInfo.Filename))
                {
                    FileDownloadName = blobInfo.Filename,
                    LastModified = null,
                    EntityTag = new EntityTagHeaderValue($"\"{Convert.ToBase64String(blobInfo.Version)}\""),
                    EnableRangeProcessing = true,
                };

            // using the FileResultExecutorBase for the range negotiation
            var (range, rangeLength, serveBody) = this.SetHeadersAndLog(
                context,
                fileResult,
                blobInfo.Size,
                fileResult.EnableRangeProcessing,
                fileResult.LastModified,
                fileResult.EntityTag);

            if (!serveBody)
            {
                return;
            }

            if (range != null)
            {
                if (rangeLength == 0)
                {
                    return;
                }

                await blobReadRepository.CopyBlobContentToStreamAsync(
                    blobInfo.BlobContentId,
                    context.HttpContext.Response.Body,
                    FileResultExecutorBase.BufferSize,
                    range.From.Value,
                    rangeLength,
                    ct);
            }
            else
            {
                await blobReadRepository.CopyBlobContentToStreamAsync(
                    blobInfo.BlobContentId,
                    context.HttpContext.Response.Body,
                    FileResultExecutorBase.BufferSize,
                    ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Don't throw this exception, it's most likely caused by the client disconnecting.
            // However, if it was cancelled for any other reason we need to prevent empty responses.
            context.HttpContext.Abort();
        }
    }

    private string GetFileContentType(string fileName)
    {
        if(!this.contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}
