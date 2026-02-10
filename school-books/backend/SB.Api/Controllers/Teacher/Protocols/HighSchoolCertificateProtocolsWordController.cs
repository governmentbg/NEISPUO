namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class HighSchoolCertificateProtocolsWordController : ProtocolsWordController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadHighSchoolCertificateProtocolsWordFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int protocolId,
        [FromServices] IHighSchoolCertificateProtocolQueryRepository highSchoolCertificateProtocolQueryRepository,
        [FromServices] IWordTemplateService wordTemplateService,
        CancellationToken ct)
    {
        MemoryStream wordStream = new(); // The memory stream must not be disposed

        var protocol = await highSchoolCertificateProtocolQueryRepository.GetWordDataAsync(schoolYear, protocolId, ct);
        var jsonObject = JsonSerializer.Serialize(
            new
            {
                protocol.InstName,
                protocol.InstitutionTownName,
                protocol.InstitutionMunicipalityName,
                protocol.InstitutionRegionName,
                protocol.SchoolYear,
                protocol.ExamSession,
                protocol.Stage,
                StageUppercase = protocol.Stage.ToUpper(),
                ProtocolNum = !string.IsNullOrEmpty(protocol.ProtocolNum) ? protocol.ProtocolNum : DefaultEmptyProtocolField,
                ProtocolDate = protocol.ProtocolDate ?? DefaultEmptyProtocolField,
                protocol.CommissionMeetingDate,
                protocol.CommissionNominationOrderNumber,
                protocol.CommissionNominationOrderDate,
                protocol.DirectorName,
                protocol.DirectorNameInParentheses,
                protocol.ChairmanName,
                protocol.CommissionMembers,
                protocol.CommissionMembersDivided,
                protocol.Students,
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "HighSchoolCertificateProtocol";
        var protNumAndDate = string.Concat(protocol.ProtocolNum, "_", protocol.ProtocolDate);

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, false, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"3-84_{ protNumAndDate }.docx" };
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.WordMimeType)]
    [HttpGet]
    public async Task<FileStreamResult> DownloadHighSchoolCertificateProtocolsWordTemplateFileAsync(
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
                InstName = record.InstitutionName,
                InstitutionTownName = record.InstitutionTownName,
                InstitutionMunicipalityName = record.InstitutionMunicipalityName,
                InstitutionRegionName = record.InstitutionRegionName,
                ExamSession = (string?)null,
                Stage = (string?)null,
                StageUppercase = (string?)null,
                ProtocolNum = (string?)null,
                ProtocolDate = (string?)null,
                CommissionMeetingDate = (string?)null,
                CommissionNominationOrderNumber = (string?)null,
                CommissionNominationOrderDate = (string?)null,
                DirectorName = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorMiddleName, record.DirectorLastName),
                DirectorNameInParentheses = StringUtils.JoinNames(record.DirectorFirstName, record.DirectorLastName),
                ChairmanName = (string?)null,
                CommissionMembers = Array.Empty<object>(),
                CommissionMembersDivided = Array.Empty<object>(),
                Students = Array.Empty<object>()
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        var templateName = "HighSchoolCertificateProtocol";

        await wordTemplateService.TransformAsync(templateName, jsonObject, wordStream, true, ct);
        wordStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(wordStream, OpenXmlExtensions.WordMimeType) { FileDownloadName = $"3-84_образец.docx" };
    }
}
