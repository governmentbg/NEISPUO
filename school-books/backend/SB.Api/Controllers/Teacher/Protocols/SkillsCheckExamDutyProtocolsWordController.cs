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

public class SkillsCheckExamDutyProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadSkillsCheckExamDutyProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] ISkillsCheckExamDutyProtocolsQueryRepository skillsCheckExamDutyProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var protocol = await skillsCheckExamDutyProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
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
                ProtocolDate = protocol.ProtocolDate ?? DefaultEmptyProtocolField,
                Date = protocol.Date,
                DirectorName = protocol.DirectorName,
                Students = students.ToArray(),
                Supervisors = protocol.Supervisors,
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "SkillsCheckExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_ИПС_{protocol.Date}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadSkillsCheckExamDutyProtocolsWordTemplateFileAsync(
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
                Date = (string?)null,
                DirectorName = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorMiddleName, record.DirectorLastName),
                Students = Array.Empty<object>(),
                Supervisors = Array.Empty<object>(),
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "SkillsCheckExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_ИПС_образец.docx" };
    }
}
