namespace SB.Api;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Policies.AuthenticatedAccess)]
[ApiController]
[Route("api/[controller]")]
public class SignController
{
    public record SignRequest
    {
        public string? Url { get; init; }
    }

    public record SignResult(string Url);

    [HttpPost]
    public ActionResult<SignResult> Sign(
        [FromBody][Required] SignRequest req,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        if (string.IsNullOrEmpty(req.Url))
        {
            return new ContentResult()
            {
                Content = "missing 'url'",
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status400BadRequest,
            };
        }

        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(SignController)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        var jwtBearer = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(jwtBearer) || !jwtBearer.StartsWith("Bearer "))
        {
            throw new Exception("Missing bearer token!");
        }

        var token = jwtBearer.Substring(7);

        UriBuilder signedUrl = new(req.Url);
        string signature = $"access_token={token}";

        if (signedUrl.Query != null && signedUrl.Query.Length > 1)
        {
            signedUrl.Query = $"{signedUrl.Query[1..]}&{signature}";
        }
        else
        {
            signedUrl.Query = signature;
        }

        return new SignResult(signedUrl.ToString());
    }
}
