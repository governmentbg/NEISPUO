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

public class ExamDutyProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadExamDutyProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IExamDutyProtocolsQueryRepository examDutyProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await examDutyProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                SchoolYear = record.SchoolYear,
                InstitutionName = record.InstitutionName,
                InstitutionTownName = record.InstitutionTownName,
                InstitutionMunicipalityName = record.InstitutionMunicipalityName,
                InstitutionRegionName = record.InstitutionRegionName,
                ProtocolNumber = !string.IsNullOrEmpty(record.ProtocolNumber) ? record.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = record.ProtocolDate ?? DefaultEmptyProtocolField,
                SessionType = record.SessionType,
                SubjectName = record.SubjectName,
                EduFormName = record.EduFormName,
                ExamType = record.ExamType,
                ExamTypeUppercase = record.ExamType.ToUpper(),
                ExamSubType = record.ExamSubType,
                OrderNumber = record.OrderNumber,
                OrderDate = record.OrderDate,
                Date = record.Date,
                GroupNum = !string.IsNullOrEmpty(record.GroupNum) ? record.GroupNum : DefaultEmptyProtocolField,
                ClassNames = string.Join(", ", record.ClassNames),
                Supervisors = record.Supervisors,
                Students = record.Students
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "ExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_изпит_{record.OrderDate}_{record.OrderNumber}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadExamDutyProtocolsWordTemplateFileAsync(
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
                ExamType = (string?)null,
                ExamTypeUppercase = (string?)null,
                ExamSubType = (string?)null,
                OrderNumber = (string?)null,
                OrderDate = (string?)null,
                Date = (string?)null,
                GroupNum = (string?)null,
                ClassNames = (string?)null,
                Supervisors = Array.Empty<object>(),
                Students = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "ExamDutyProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_дежурство_изпит_образец.docx" };
    }
}
