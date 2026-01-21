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

public class ExamResultProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadExamResultProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IExamResultProtocolsQueryRepository examResultProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await examResultProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                record.SchoolYear,
                record.InstitutionName,
                record.InstitutionTownName,
                record.InstitutionMunicipalityName,
                record.InstitutionRegionName,
                ProtocolNumber = !string.IsNullOrEmpty(record.ProtocolNumber) ? record.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = record.ProtocolDate ?? DefaultEmptyProtocolField,
                record.SessionType,
                record.SubjectName,
                GroupNum = !string.IsNullOrEmpty(record.GroupNum) ? record.GroupNum : DefaultEmptyProtocolField,
                ClassNames = string.Join(", ", record.ClassNames),
                record.EduFormName,
                record.ProtocolType,
                record.ExamType,
                record.Date,
                record.CommissionNominationOrderNumber,
                record.CommissionNominationOrderDate,
                record.ChairmanName,
                record.CommissionMembers,
                record.CommissionMembersDivided,
                record.Students
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName =
            record.CommissionMembers.Length <= 1 ? "ExamResultProtocol" :
            record.CommissionMembers.Length <= 3 ? "ExamResultProtocolV2" :
            record.CommissionMembers.Length <= 5 ? "ExamResultProtocolV3" :
            record.CommissionMembers.Length <= 7 ? "ExamResultProtocolV4" :
            record.CommissionMembers.Length <= 9 ? "ExamResultProtocolV5" :
            "ExamResultProtocolV6";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_рез_изпит_{record.ProtocolDate}_{record.ProtocolNumber}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadExamResultProtocolsWordTemplateFileAsync(
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
                GroupNum = (string?)null,
                ClassNames = (string?)null,
                EduFormName = (string?)null,
                ProtocolType = (string?)null,
                ExamType = (string?)null,
                Date = (string?)null,
                CommissionNominationOrderNumber = (string?)null,
                CommissionNominationOrderDate = (string?)null,
                ChairmanName = (string?)null,
                CommissionMembers = Array.Empty<object>(),
                CommissionMembersDivided = Array.Empty<object>(),
                Students = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "ExamResultProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_рез_изпит_образец.docx" };
    }
}
