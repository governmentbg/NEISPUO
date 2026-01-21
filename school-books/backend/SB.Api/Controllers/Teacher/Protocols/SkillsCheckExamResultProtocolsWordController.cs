namespace SB.Api.Controllers;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class SkillsCheckExamResultProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadSkillsCheckExamResultProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] ISkillsCheckExamResultProtocolsQueryRepository skillsCheckExamResultProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var protocol = await skillsCheckExamResultProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var students = new List<object>();

        for (int i = 1; i <= protocol.StudentsCapacity; i++)
        {
            students.Add(new { OrderNum = i });
        }

        var jsonObject = JsonSerializer.Serialize(
            new
            {
                SchoolYear = protocol.SchoolYear,
                InstitutionName = protocol.InstitutionName,
                InstitutionTownName = protocol.InstitutionTownName,
                InstitutionMunicipalityName = protocol.InstitutionMunicipalityName,
                InstitutionRegionName = protocol.InstitutionRegionName,
                SubjectName = protocol.SubjectName,
                ProtocolNumber = !string.IsNullOrEmpty(protocol.ProtocolNumber) ? protocol.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = protocol.Date ?? DefaultEmptyProtocolField,
                Students = students.ToArray(),
                protocol.Evaluators
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName =
            protocol.Evaluators.Length <= 2 ? "SkillsCheckExamResultProtocol" :
            protocol.Evaluators.Length <= 4 ? "SkillsCheckExamResultProtocolV2" :
            protocol.Evaluators.Length <= 6 ? "SkillsCheckExamResultProtocolV3" :
            protocol.Evaluators.Length <= 8 ? "SkillsCheckExamResultProtocolV4" :
            "SkillsCheckExamResultProtocolV5";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_рез_ИПС_{protocol.Date}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadSkillsCheckExamResultProtocolsWordTemplateFileAsync(
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
                SubjectName = (string?)null,
                ProtocolNumber = (string?)null,
                ProtocolDate = (string?)null,
                Students = Array.Empty<object>(),
                Evaluators = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "SkillsCheckExamResultProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_рез_ИПС_образец.docx" };
    }
}
