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

public class GraduationThesisDefenseProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadGraduationThesisDefenseProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IGraduationThesisDefenseProtocolQueryRepository graduationThesisDefenseProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var protocol = await graduationThesisDefenseProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var section1Students = new List<object>();
        var section2Students = new List<object>();
        var section3Students = new List<object>();
        var section4Students = new List<object>();

        for (int i = 1; i <= protocol.Section1StudentsCapacity; i++)
        {
            section1Students.Add(new { OrderNum = i });
        }
        for (int i = 1; i <= protocol.Section2StudentsCapacity; i++)
        {
            section2Students.Add(new { OrderNum = i });
        }
        for (int i = 1; i <= protocol.Section3StudentsCapacity; i++)
        {
            section3Students.Add(new { OrderNum = i });
        }
        for (int i = 1; i <= protocol.Section4StudentsCapacity; i++)
        {
            section4Students.Add(new { OrderNum = i });
        }

        var jsonObject = JsonSerializer.Serialize(
            new
            {
                protocol.SchoolYear,
                protocol.InstitutionName,
                protocol.InstitutionTownName,
                protocol.InstitutionMunicipalityName,
                protocol.InstitutionRegionName,
                ProtocolNumber = !string.IsNullOrEmpty(protocol.ProtocolNumber) ? protocol.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = protocol.ProtocolDate ?? DefaultEmptyProtocolField,
                protocol.SessionType,
                protocol.EduFormName,
                protocol.CommissionMeetingDate,
                protocol.DirectorOrderNumber,
                protocol.DirectorOrderDate,
                protocol.ChairmanName,
                protocol.CommissionMembers,
                protocol.CommissionMembersDivided,
                protocol.DirectorName,
                protocol.DirectorNameInParentheses,
                Section1Students = section1Students.ToArray(),
                Section2Students = section2Students.ToArray(),
                Section3Students = section3Students.ToArray(),
                Section4Students = section4Students.ToArray(),
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "GraduationThesisDefenseProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_защ_дип_проект_{protocol.ProtocolNumber}_{protocol.ProtocolDate}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadGraduationThesisDefenseProtocolsWordTemplateFileAsync(
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
                EduFormName = (string?)null,
                CommissionMeetingDate = (string?)null,
                DirectorOrderNumber = (string?)null,
                DirectorOrderDate = (string?)null,
                ChairmanName = (string?)null,
                CommissionMembers = Array.Empty<object>(),
                CommissionMembersDivided = Array.Empty<object>(),
                DirectorName = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorMiddleName, record.DirectorLastName),
                DirectorNameInParentheses = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorLastName),
                Section1Students = Array.Empty<object>(),
                Section2Students = Array.Empty<object>(),
                Section3Students = Array.Empty<object>(),
                Section4Students = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "GraduationThesisDefenseProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_защ_дип_проект_образец.docx" };
    }
}
