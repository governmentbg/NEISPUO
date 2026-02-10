namespace SB.Blobs;

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

[ApiController]
public class BlobsController
{
    // Get the default form options so that we can use them to set the default limits for
    // request body data
    private static readonly FormOptions defaultFormOptions = new FormOptions();

    private const string TimestampIsInTheFuture = "Timestamp is in the future";
    private const string TimestampTooOld = "Линкът за сваляне е с изтекъл срок на валидност. Моля презаредете страницата, от която идвате и натиснете файла за сваляне отново.";
    private const string IncorrectHMAC = "Incorrect HMAC";

    [HttpGet]
    [Route("{blobId:int}")]
    public ActionResult GetFile(
        [FromRoute]int blobId,
        [FromQuery(Name="t"), Required]long? unixTimeSeconds,
        [FromQuery(Name="h"), Required]string urlSafeBase64HMAC,
        [FromServices]IOptions<BlobsOptions> optionsAccessor)
    {
        var options = optionsAccessor.Value;
        string hmacError = VerifyHMAC(
            options.BlobUrlExpiration,
            options.ClockSkew,
            options.HMACKey,
            $"{blobId}/{unixTimeSeconds}",
            unixTimeSeconds,
            urlSafeBase64HMAC);
        if (!string.IsNullOrEmpty(hmacError))
        {
            return new ContentResult()
            {
                Content = hmacError,
                ContentType = "text/plain;charset=utf-8",
                StatusCode = StatusCodes.Status400BadRequest,
            };
        }

        return new BlobStreamResult(blobId);
    }

    [HttpDelete]
    [Route("{blobId:int}")]
    public async Task<ActionResult> DeleteFileAsync(
        [FromRoute]int blobId,
        [FromQuery(Name="t"), Required]long? unixTimeSeconds,
        [FromQuery(Name="h"), Required]string urlSafeBase64HMAC,
        [FromServices]ILoggerFactory loggerFactory,
        [FromServices]IOptions<BlobsOptions> blobsOptions,
        [FromServices]IOptions<DataOptions> dataOptions,
        CancellationToken ct)
    {
        var options = blobsOptions.Value;
        string hmacError = VerifyHMAC(
            options.BlobUrlExpiration,
            options.ClockSkew,
            options.HMACKey,
            $"{blobId}/{unixTimeSeconds}",
            unixTimeSeconds,
            urlSafeBase64HMAC);
        if (!string.IsNullOrEmpty(hmacError))
        {
            return new ContentResult()
            {
                Content = hmacError,
                ContentType = "text/plain;charset=utf-8",
                StatusCode = StatusCodes.Status400BadRequest,
            };
        }

        await using SqlConnection conn = new(dataOptions.Value.GetConnectionString());
        await conn.OpenAsync(ct);

        BlobWriteRepository blobWriteRepository = new (conn, loggerFactory);

        if (!ct.IsCancellationRequested)
        {
            // the delete will not be cancellable as it is strange to do so
            await blobWriteRepository.DeleteBlobAsync(blobId, default(CancellationToken));
        }

        return new OkResult();
    }

    public record BlobDO(string Name, long Size, int BlobId, string Location);

    [HttpPost]
    [DisableFormValueModelBinding]
    [RequestSizeLimit(51 * 1024 * 1024)] // the max file size is 50MB but we give it some slack for headers, etc.
    [Route("")]
    public async Task<ActionResult<BlobDO>> PostFileAsync(
        [FromServices]ILoggerFactory loggerFactory,
        [FromServices]IOptions<BlobsOptions> blobsOptions,
        [FromServices]IOptions<DataOptions> dataOptions,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromQuery(Name="t")]long? unixTimeSeconds,
        [FromQuery(Name="h")]string urlSafeBase64HMAC,
        [FromServices]IOptions<BlobsOptions> optionsAccessor,
        CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var request = httpContext.Request;
        var blobsOpt = blobsOptions.Value;
        var dataOpt = dataOptions.Value;

        if (blobsOpt.HasPostAuthentication)
        {
            if (unixTimeSeconds == null || string.IsNullOrEmpty(urlSafeBase64HMAC))
            {
                var authRes = await httpContext.AuthenticateAsync();
                if (!authRes.Succeeded)
                {
                    return new StatusCodeResult(StatusCodes.Status401Unauthorized);
                }
            }
            else
            {
                var options = optionsAccessor.Value;
                string hmacError = VerifyHMAC(
                    options.BlobUploadExpiration,
                    options.ClockSkew,
                    options.HMACKey,
                    $"{unixTimeSeconds}",
                    unixTimeSeconds,
                    urlSafeBase64HMAC);
                if (!string.IsNullOrEmpty(hmacError))
                {
                    return new ContentResult()
                    {
                        Content = hmacError,
                        ContentType = "text/plain;charset=utf-8",
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                }
            }
        }

        if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
        {
            return new ContentResult
            {
                Content = $"Expected a multipart request, but got {request.ContentType}",
                ContentType = "text/plain;charset=utf-8",
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var boundary = MultipartRequestHelper.GetBoundary(
            MediaTypeHeaderValue.Parse(request.ContentType),
            defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, httpContext.Request.Body);

        int? blobId = null;
        long? size = null;
        string fileName = null;
        var section = await reader.ReadNextSectionAsync(ct);
        while (section != null)
        {
            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
                MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
            {
                string file;
                if (!string.IsNullOrEmpty(contentDisposition.FileNameStar.Value))
                {
                    file = contentDisposition.FileNameStar.Value;
                }
                else if (!string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    file = contentDisposition.FileName.Value;
                }
                else
                {
                    throw new Exception("At least one of FileName/FileNameStar should have a value");
                }

                fileName = Path.GetFileName(file);

                await using SqlConnection conn = new(dataOpt.GetConnectionString());
                await conn.OpenAsync(ct);

                BlobWriteRepository blobWriteRepository = new (conn, loggerFactory);
                var blob = await blobWriteRepository.DrainStreamAsync(section.Body, fileName, ct);

                blobId = blob.BlobId;
                size = blob.Size;

                // we are looking for the first file and nothing else
                break;
            }

            // Drains any remaining section body that has not been consumed and
            // reads the headers for the next section.
            section = await reader.ReadNextSectionAsync(ct);
        }

        if (blobId == null)
        {
            return new ContentResult
            {
                Content = $"Non file content disposition found",
                ContentType = "text/plain;charset=utf-8",
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        long newUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        byte[] hmac = BlobUtils.GetHMAC(
            BlobUtils.USAsciiStrict.GetBytes($"{blobId}/{newUnixTimeSeconds}"),
            BlobUtils.USAsciiStrict.GetBytes(blobsOpt.HMACKey));
        string newUrlSafeBase64HMAC = BlobUtils.ToUrlSafeBase64(hmac);

        var req = httpContextAccessor.HttpContext.Request;
        var location = $"{req.Scheme}://{req.Host.Value}/{blobId}?t={newUnixTimeSeconds}&h={newUrlSafeBase64HMAC}";
        return new BlobDO(
            fileName,
            size.Value,
            blobId.Value,
            location);
    }

    private static string VerifyHMAC(
        TimeSpan expiration,
        TimeSpan clockSkew,
        string key,
        string message,
        long? unixTimeSeconds,
        string urlSafeBase64HMAC)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds.Value);
        var now = DateTimeOffset.UtcNow;

        // check that the timestamp is not in the past taking ClockSkew into account
        if (now.Subtract(timestamp)
            .Add(clockSkew)
            .Duration() < TimeSpan.Zero)
        {
            return TimestampIsInTheFuture;
        }

        // check that the timestamp is not too old taking ClockSkew into account
        if (now.Subtract(timestamp)
            .Subtract(clockSkew)
            .Duration() > expiration)
        {
            return TimestampTooOld;
        }

        byte[] providedHMAC = BlobUtils.FromUrlSafeBase64(urlSafeBase64HMAC);
        byte[] expectedHMAC = BlobUtils.GetHMAC(
            BlobUtils.USAsciiStrict.GetBytes(message),
            BlobUtils.USAsciiStrict.GetBytes(key));

        if (!Enumerable.SequenceEqual(providedHMAC, expectedHMAC))
        {
            return IncorrectHMAC;
        }

        return null; // HMAC is OK
    }
}
