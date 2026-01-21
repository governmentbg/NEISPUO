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

public class QualificationExamResultProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadQualificationExamResultProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IQualificationExamResultProtocolsQueryRepository qualificationExamResultProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await qualificationExamResultProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
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
                record.Profession,
                record.Speciality,
                record.QualificationDegree,
                GroupNum = !string.IsNullOrEmpty(record.GroupNum) ? record.GroupNum : DefaultEmptyProtocolField,
                ClassNames = string.Join(", ", record.ClassNames),
                record.EduFormName,
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
            record.CommissionMembers.Length <= 2 ? "QualificationExamResultProtocol" :
            record.CommissionMembers.Length <= 4 ? "QualificationExamResultProtocolV2" :
            record.CommissionMembers.Length <= 6 ? "QualificationExamResultProtocolV3" :
            record.CommissionMembers.Length <= 8 ? "QualificationExamResultProtocolV4" :
            record.CommissionMembers.Length <= 10 ? "QualificationExamResultProtocolV5" :
            "QualificationExamResultProtocolV6";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_рез_изпит_{record.ProtocolDate}_{record.ProtocolNumber}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadQualificationExamResultProtocolsWordTemplateFileAsync(
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
                Profession = (string?)null,
                Speciality = (string?)null,
                QualificationDegree = (string?)null,
                GroupNum = (string?)null,
                ClassNames = (string?)null,
                EduFormName = (string?)null,
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

        var templateName = "QualificationExamResultProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_рез_изпит_образец.docx" };
    }
}
