namespace SB.Api.Controllers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class NvoExamDutyProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadNvoExamDutyProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] INvoExamDutyProtocolsQueryRepository nvoExamDutyProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await nvoExamDutyProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                SchoolYear = record.SchoolYear,
                InstitutionName = record.InstitutionName,
                InstitutionTownName = record.InstitutionTownName,
                InstitutionMunicipalityName = record.InstitutionMunicipalityName,
                InstitutionRegionName = record.InstitutionRegionName,
                BasicClassName = record.BasicClassName,
                ProtocolNumber = !string.IsNullOrEmpty(record.ProtocolNumber) ? record.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = record.ProtocolDate ?? DefaultEmptyProtocolField,
                SubjectName = record.SubjectName,
                Date = record.Date,
                RoomNumber = record.RoomNumber,
                DirectorName = record.DirectorName,
                Supervisors = record.Supervisors,
                Students = record.Students
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "NvoExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_изпит_НВО_{record.Date}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadNvoExamDutyProtocolsWordTemplateFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int protocolId,
        [FromServices] IInstitutionsQueryRepository institutionsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await institutionsQueryRepository.GetProtocolTemplateDataAsync(schoolYear, instId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                SchoolYear = schoolYear + " / " + (schoolYear + 1),
                InstitutionName = record.InstitutionName,
                InstitutionTownName = record.InstitutionTownName,
                InstitutionMunicipalityName = record.InstitutionMunicipalityName,
                InstitutionRegionName = record.InstitutionRegionName,
                BasicClassName = (string?)null,
                ProtocolNumber = (string?)null,
                ProtocolDate = (string?)null,
                SubjectName = (string?)null,
                Date = (string?)null,
                RoomNumber = (string?)null,
                DirectorName = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorMiddleName, record.DirectorLastName),
                Supervisors = Array.Empty<object>(),
                Students = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "NvoExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_изпит_НВО_образец.docx" };
    }
}
