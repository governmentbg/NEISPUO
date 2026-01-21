namespace SB.Api.Controllers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class QualificationAcquisitionProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadQualificationAcquisitionProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IQualificationAcquisitionProtocolsQueryRepository qualificationAcquisitionProtocolsQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new MemoryStream(); // The memory stream must not be disposed

        var record = await qualificationAcquisitionProtocolsQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                record.SchoolYear,
                record.InstitutionName,
                record.InstitutionTownName,
                record.InstitutionMunicipalityName,
                record.InstitutionRegionName,
                ProtocolType = EnumUtils.GetEnumDescription(record.ProtocolType).ToUpper(),
                PassedExamResultTableText = record.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisition || record.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades ?
                    "държавен изпит за придобиване на степен на професионална квалификация" :
                    "изпит за придобиване на професионална квалификация по част от професия",
                FailedExamResultTableText = record.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisition || record.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades ?
                    "държавен/държавни изпит/и за придобиване на професионална квалификация" :
                    "изпит/и за придобиване на професионална квалификация по част от професия",
                ProtocolNumber = !string.IsNullOrEmpty(record.ProtocolNumber) ? record.ProtocolNumber : DefaultEmptyProtocolField,
                ProtocolDate = record.ProtocolDate ?? DefaultEmptyProtocolField,
                record.Profession,
                record.Speciality,
                record.QualificationDegree,
                record.Date,
                record.CommissionNominationOrderNumber,
                record.CommissionNominationOrderDate,
                record.DirectorName,
                record.DirectorNameInParentheses,
                record.ChairmanName,
                record.CommissionMembers,
                record.CommissionMembersDivided,
                PassedExamStudents = record.Students.Where(s => s.ExamsPassed),
                FailedExamStudents = record.Students.Where(s => !s.ExamsPassed)
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = record.ProtocolType == QualificationAcquisitionProtocolType.QualificationAcquisition ? "QualificationAcquisitionProtocol" : "QualificationAcquisitionExamGradesProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_прид_проф_квалиф_{record.ProtocolDate}_{record.ProtocolNumber}.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadQualificationAcquisitionProtocolsWordTemplateFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int protocolId,
        [FromQuery] QualificationAcquisitionProtocolType protocolType,
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
                record.InstitutionName,
                record.InstitutionTownName,
                record.InstitutionMunicipalityName,
                record.InstitutionRegionName,
                ProtocolType = EnumUtils.GetEnumDescription(protocolType).ToUpper(),
                PassedExamResultTableText = protocolType == QualificationAcquisitionProtocolType.QualificationAcquisition || protocolType == QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades ?
                    "държавен изпит за придобиване на степен на професионална квалификация" :
                    "изпит за придобиване на професионална квалификация по част от професия",
                FailedExamResultTableText = protocolType == QualificationAcquisitionProtocolType.QualificationAcquisition || protocolType == QualificationAcquisitionProtocolType.QualificationAcquisitionExamGrades ?
                    "държавен/държавни изпит/и за придобиване на професионална квалификация" :
                    "изпит/и за придобиване на професионална квалификация по част от професия",
                ProtocolNumber = (string?)null,
                ProtocolDate = (string?)null,
                Profession = (string?)null,
                Speciality = (string?)null,
                QualificationDegree = (string?)null,
                Date = (string?)null,
                CommissionNominationOrderNumber = (string?)null,
                CommissionNominationOrderDate = (string?)null,
                DirectorName = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorMiddleName, record.DirectorLastName),
                DirectorNameInParentheses = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorLastName),
                ChairmanName = (string?)null,
                CommissionMembers = Array.Empty<object>(),
                CommissionMembersDivided = Array.Empty<object>(),
                PassedExamStudents = Array.Empty<object>(),
                FailedExamStudents = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = protocolType == QualificationAcquisitionProtocolType.QualificationAcquisition ? "QualificationAcquisitionProtocol" : "QualificationAcquisitionExamGradesProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"Прот_прид_проф_квалиф_образец.docx" };
    }
}
