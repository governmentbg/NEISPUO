namespace SB.ExtApi;

using System.IO;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SB.Common;

[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
[ApiController]
[Route("{lg}/[controller]")]
public class DocumentationController
{
    [HttpGet("release-notes")]
    public async Task<ActionResult> GetReleaseNotesAsync(
        [FromRoute]string? lg,
        [FromServices]IWebHostEnvironment webHostEnvironment,
        [FromServices]IHttpContextAccessor context)
    {
        var language = "BG";
        if (lg != null && MultiLanguageProcessor.IsLanguageSupported(lg))
        {
            language = lg.ToUpper();
        }

        await using Stream releaseNotesStream =
            webHostEnvironment.ContentRootFileProvider
                .GetFileInfo($"Documentation/ReleaseNotes/RELEASE_NOTES_{language}.md")
                .CreateReadStream();
        using StreamReader reader = new(releaseNotesStream, encoding: System.Text.Encoding.UTF8);
        string releaseNotesMarkdown = await reader.ReadToEndAsync();
        var pipeline = new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();
        var releaseNotesHtml = Markdown.ToHtml(releaseNotesMarkdown, pipeline);

        var contentType = MediaTypeHeaderValue.Parse("text/html");
        contentType.Encoding = System.Text.Encoding.UTF8;

        context.HttpContext!.Response.Headers.AddOrUpdate("Content-Security-Policy",
            "default-src 'none'; " +
            "frame-ancestors 'none'; " +
            "style-src " +
                "https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.1.0/github-markdown.min.css " +
                "'sha256-0n+wBcWkNUirqqdwEhnjK4+k93U/r7WFi6DYgIGdyqs=' " +
                "'sha256-yUJ1UiYCDJLMNmDcrBKG8jUt6ekZjzmYL6q+lDhpLKU='");

        return new ContentResult()
        {
            Content = this.WrapInDocument(releaseNotesHtml),
            ContentType = contentType.ToString(),
            StatusCode = 200,
        };
    }

    [HttpGet("business-processes")]
    public async Task<ActionResult> GetBusinessProcessesAsync(
        [FromRoute] string? lg,
        [FromServices] IWebHostEnvironment webHostEnvironment,
        [FromServices] IHttpContextAccessor context)
    {
        var language = "BG";
        if (lg != null && MultiLanguageProcessor.IsLanguageSupported(lg))
        {
            language = lg.ToUpper();
        }

        await using Stream businessProcessesStream =
            webHostEnvironment.ContentRootFileProvider
                .GetFileInfo($"Documentation/BusinessProcesses/BUSINESS_PROCESSES_{language}.md")
                .CreateReadStream();
        using StreamReader reader = new(businessProcessesStream, encoding: System.Text.Encoding.UTF8);
        string businessProcessesMarkdown = await reader.ReadToEndAsync();
        var pipeline = new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();
        var businessProcessesHtml = Markdown.ToHtml(businessProcessesMarkdown, pipeline);

        var contentType = MediaTypeHeaderValue.Parse("text/html");
        contentType.Encoding = System.Text.Encoding.UTF8;

        context.HttpContext!.Response.Headers.AddOrUpdate("Content-Security-Policy",
            "default-src 'none'; " +
            "frame-ancestors 'none'; " +
            "style-src " +
            "https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.1.0/github-markdown.min.css " +
            "'sha256-0n+wBcWkNUirqqdwEhnjK4+k93U/r7WFi6DYgIGdyqs=' " +
            "'sha256-yUJ1UiYCDJLMNmDcrBKG8jUt6ekZjzmYL6q+lDhpLKU='");

        return new ContentResult()
        {
            Content = this.WrapInDocument(businessProcessesHtml),
            ContentType = contentType.ToString(),
            StatusCode = 200,
        };
    }

    private string WrapInDocument(string html)
    {
        // IMPORTANT! If your change the <style> tags bellow,
        // make sure to update the CSP header above.
        return
@$"<!DOCTYPE html>
    <html lang=""bg"">
    <head>
        <meta charset=""utf-8"" />
        <title>NEISPUO API release notes</title>
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.1.0/github-markdown.min.css"">
        <style>
            @media (prefers-color-scheme: dark) {{
                body {{
                    background-color: #0d1117;
                }}
            }}
        </style>
        <style>
            .markdown-body {{
                box-sizing: border-box;
                min-width: 200px;
                max-width: 980px;
                margin: 0 auto;
                padding: 45px;
            }}

            @media (max-width: 767px) {{
                .markdown-body {{
                    padding: 15px;
                }}
            }}
        </style>
    </head>
    <body>
        <article class=""markdown-body"">
        {html}
        </article>
    </body>
    </html>
";
    }
}
