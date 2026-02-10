namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class RegBookCertificatesWordController : RegBooksWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int recordId,
        [FromServices] IRegBookCertificateQueryRepository regBookCertificateQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await regBookCertificateQueryRepository.GetWordDataAsync(schoolYear, recordId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                InstitutionName = record.InstitutionName,
                InstitutionTownName = record.InstitutionTownName,
                InstitutionMunicipalityName = record.InstitutionMunicipalityName,
                InstitutionLocalAreaName = record.InstitutionLocalAreaName,
                InstitutionRegionName = record.InstitutionRegionName,
                RegistrationNumberTotal = record.RegistrationNumberTotal,
                BasicDocumentName = record.BasicDocumentName,
                RegistrationNumberYear = record.RegistrationNumberYear,
                RegistrationDate = record.RegistrationDate?.ToString("dd.MM.yyyy") ?? string.Empty,
                FullName = record.FullName,
                BookName = "издадените удостоверения"
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "RegBookDocumentForSign";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Рег_кн_изд_серт_{record.RegistrationNumberTotal}-{record.RegistrationNumberYear}/{record.RegistrationDate?.ToString("dd.MM.yyyy") ?? string.Empty}_подпис.docx" };
    }
}
