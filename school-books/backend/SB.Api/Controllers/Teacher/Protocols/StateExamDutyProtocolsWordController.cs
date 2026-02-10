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

public class StateExamDutyProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadStateExamDutyProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IStateExamDutyProtocolsQueryRepository stateExamDutyProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var protocol = await stateExamDutyProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                SchoolYear = protocol.SchoolYear,
                InstitutionName = protocol.InstitutionName,
                InstitutionTownName = protocol.InstitutionTownName,
                InstitutionMunicipalityName = protocol.InstitutionMunicipalityName,
                InstitutionRegionName = protocol.InstitutionRegionName,
                ProtocolNumber = !string.IsNullOrEmpty(protocol.ProtocolNumber) ? protocol.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = protocol.ProtocolDate ?? DefaultEmptyProtocolField,
                SessionType = protocol.SessionType,
                SubjectName = protocol.SubjectName,
                EduFormName = protocol.EduFormName,
                OrderNumber = protocol.OrderNumber,
                OrderDate = protocol.OrderDate,
                Date = protocol.Date,
                ModulesCount = protocol.ModulesCount,
                RoomNumber = protocol.RoomNumber,
                Supervisors = protocol.Supervisors,
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "StateExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_ДЗИ_{protocol.OrderDate}_{protocol.OrderNumber}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadStateExamDutyProtocolsWordTemplateFileAsync(
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
                ProtocolNumber = (string?)null,
                ProtocolDate = (string?)null,
                SessionType = (string?)null,
                SubjectName = (string?)null,
                EduFormName = (string?)null,
                OrderNumber = (string?)null,
                OrderDate = (string?)null,
                Date = (string?)null,
                ModulesCount = (int?)null,
                RoomNumber = (string?)null,
                Supervisors = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "StateExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_ДЗИ_образец.docx" };
    }
}
