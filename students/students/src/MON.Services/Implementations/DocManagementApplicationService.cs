using Microsoft.EntityFrameworkCore;
using MON.Models;
using MON.Models.DocManagement;
using MON.Services.Security.Permissions;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;
using MON.Services.Interfaces;
using MON.DataAccess;
using MON.Shared.Enums;
using MON.Shared;
using System.IO;
using Domain;
using System.Text.Json;
using Z.EntityFramework.Plus;
using MON.Models.Configuration;
using Microsoft.Extensions.Options;
using MON.Models.Grid;
using MON.Services.Utils;
using DocumentFormat.OpenXml.Bibliography;

namespace MON.Services.Implementations
{
    public class DocManagementApplicationService : BaseService<DocManagementApplicationService>
    {
        private readonly IInstitutionService _institutionService;
        private readonly IWordTemplateService _wordTemplateService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public DocManagementApplicationService(DbServiceDependencies<DocManagementApplicationService> dependencies,
            IInstitutionService institutionService,
            IWordTemplateService wordTemplateService,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _institutionService = institutionService;
            _wordTemplateService = wordTemplateService;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        #region Private members
        private async Task<DocManagementCampaignViewModel> CampaignValidation(int campaignId, DocManagementApplication application = null)
        {
            DocManagementCampaignViewModel campaign = await _context.DocManagementCampaigns
                .Where(x => x.Id == campaignId)
                .Select(x => new DocManagementCampaignViewModel
                {
                    Id = x.Id,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated,
                    SchoolYear = x.SchoolYear,
                    ParentId = x.ParentId,
                    InstitutionId = x.InstitutionId ?? 0,
                })
                .FirstOrDefaultAsync();

            if (!(campaign?.IsActive ?? false) && (application?.Status ?? "") != nameof(ApplicationStatusEnum.ReturnedForCorrection))
            {
                throw new ApiException(Messages.DocManagementInvalidPeriod, new InvalidOperationException());
            }

            if (campaign.ParentId.HasValue && campaign.InstitutionId != _userInfo.InstitutionID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return campaign;
        }

        private async Task ProcessAddedDocs(DocManagementApplicationModel model, DocManagementApplication entity)
        {
            if (model.Attachments == null || !model.Attachments.Any() || entity == null)
            {
                return;
            }

            foreach (DocumentModel docModel in model.Attachments.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.DocManagementApplicationAttachments.Add(new DocManagementApplicationAttachment
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(DocManagementApplicationModel model, DocManagementApplication sanction)
        {
            if (model.Attachments == null || !model.Attachments.Any() || sanction == null)
            {
                return;
            }

            HashSet<int> blobsToDelete = model.Attachments
                .Where(x => x.Id.HasValue && x.Deleted == true && x.BlobId.HasValue)
                .Select(x => x.BlobId.Value).ToHashSet();

            if (blobsToDelete.Count > 0)
            {
                await _context.DocManagementApplicationAttachments
                    .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.Documents.Where(x => x.BlobId.HasValue && blobsToDelete.Contains(x.BlobId.Value)).DeleteAsync();
            }
        }

        private async Task<(string fileName, byte[] content)> CreateZipWithDocsAsync(List<(string fileName, byte[] content)> files)
        {
            using var ms = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                foreach (var (fileName, content) in files)
                {
                    var zipEntry = archive.CreateEntry(fileName, System.IO.Compression.CompressionLevel.Fastest);
                    using var zipStream = zipEntry.Open();
                    using var fileStream = new MemoryStream(content);
                    await fileStream.CopyToAsync(zipStream);
                }
            }

            return ($"ZUD_Reports_{DateTime.Now:yyyyMMddHHmmss}.zip", ms.ToArray());
        }

        private async Task<IEnumerable<DocManagementApplicationViewModel>> GetApplications(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            IQueryable<DocManagementApplication> query = _context.DocManagementApplications
                .AsNoTracking()
                .Where(x => !x.Campaign.IsHidden && !x.IsExchangeRequest);

            if (model.ApplicationId.HasValue)
            {
                query = query.Where(x => x.Id == model.ApplicationId);
            }
            else if (model.CampaignId.HasValue)
            {
                query = query.Where(x => x.CampaignId == model.CampaignId || x.Campaign.ParentId == model.CampaignId.Value);
                if (_userInfo.IsConsortium)
                {
                    // Вижда всичко
                }
                else if (_userInfo.IsMon || _userInfo.IsMonExpert)
                {
                    // Вижда всичко
                }
                else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
                {
                    query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == (_userInfo.RegionID ?? 0));
                }
                else if (_userInfo.IsSchoolDirector)
                {
                    query = query.Where(x => x.InstitutionId == (_userInfo.InstitutionID ?? 0));
                }
                else
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                if (model.RegionId.HasValue)
                {
                    query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == model.RegionId.Value);
                }

                if (model.MunicipalityId.HasValue)
                {
                    query = query.Where(x => x.InstitutionSchoolYear.Town.MunicipalityId == model.MunicipalityId.Value);
                }
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.CampaignType.HasValue)
            {
                switch (model.CampaignType.Value)
                {
                    case 1:
                        // Всички
                        break;
                    case 2:
                        // Основни кампании
                        query = query.Where(x => x.Campaign.ParentId == null);
                        break;
                    case 3:
                        // Допълнителни кампании
                        query = query.Where(x => x.Campaign.ParentId != null);
                        break;
                    default:
                        break;
                }
            }

            switch (model.InstType)
            {
                case 1:
                    // Всички
                    break;
                case 2:
                // С делегиран бюджет
                case 3:
                    // Без делегиран бюджет
                    query = from app in query
                            join id in _context.InstitutionDetails on app.InstitutionId equals id.InstitutionId into temp
                            from inst in temp.DefaultIfEmpty()
                            where (inst.IsDelegateBudget ?? false) == (model.InstType == 2)
                            select app;
                    break;
                default:
                    break;
            }

            return await query
                .Select(x => new DocManagementApplicationViewModel
                {
                    InstitutionId = x.InstitutionId,
                    InstitutionMunicipalityId = x.InstitutionSchoolYear.Town.MunicipalityId,
                    InstitutionMunicipalityName = x.InstitutionSchoolYear.Town.Municipality.Name,
                    InstitutionMunicipalityCode = x.InstitutionSchoolYear.Town.Municipality.Code,
                    InstitutionRegionId = x.InstitutionSchoolYear.Town.Municipality.RegionId,
                    InstitutionRegionName = x.InstitutionSchoolYear.Town.Municipality.Region.Name,
                    InstitutionRegionCode = x.InstitutionSchoolYear.Town.Municipality.Region.Code,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    BasicDocuments = x.DocManagementApplicationItems
                        .Select(i => new DocManagementApplicationBasicDocumentModel
                        {
                            Id = i.Id,
                            BasicDocumentId = i.BasicDocumentId,
                            BasicDocumentName = i.BasicDocument.Name,
                            Number = i.Number,
                        }),
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<DocManagementApplicationReportModel> GetReportModel(DocManagementReportImputModel model, IEnumerable<DocManagementApplicationViewModel> applications, CancellationToken cancellationToken)
        {
            DocManagementApplicationReportModel reportModel = new DocManagementApplicationReportModel();

            await GetReportDetails(reportModel, model, applications, cancellationToken);

            reportModel.documents = applications.SelectMany(x => x.BasicDocuments)
                    .GroupBy(x => x.BasicDocumentId)
                    .Select(x => new DocManagementApplicationReportDocumentModel
                    {
                        nomenclatureNumber = x.First().BasicDocumentName.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                        name = x.First().BasicDocumentName,
                        count = x.Sum(x => x.Number).ToString(),
                    })
                    .OrderBy(x => x.nomenclatureNumber)
                    .ToList();


            return reportModel;
        }

        private async Task GetReportDetails(DocManagementApplicationReportModel reportModel, DocManagementReportImputModel model, IEnumerable<DocManagementApplicationViewModel> applications, CancellationToken cancellationToken)
        {
            if (_userInfo.IsSchoolDirector)
            {
                DocManagementApplicationViewModel application = applications.FirstOrDefault() ?? throw new ApiException(Messages.EmptyEntityError);
                var inst = await _context.CiooreportCommonCurrData
                    .Where(x => x.InstId == application.InstitutionId && x.SchoolYear == application.SchoolYear)
                    .Select(x => new
                    {
                        x.InstId,
                        x.InstEik,
                        x.Director,
                        x.InstEmail,
                        x.InstMainPhone,
                        x.InstName,
                        x.InstAddress,
                        x.InstTownName,
                        x.InstMunName,
                        x.InstRegName,
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                reportModel.schoolYearName = application.SchoolYearName ?? "";
                reportModel.institutionCode = inst?.InstId != null ? inst.InstId.ToString() : "";
                reportModel.institutionDirectorName = inst?.Director ?? "";
                reportModel.institutionName = inst?.InstName ?? "";
                reportModel.institutionPhoneNumber = inst?.InstMainPhone ?? "";
                reportModel.institutionEmail = inst?.InstEmail ?? "";
                reportModel.institutionAddress = $"{inst?.InstAddress}, гр./с. {inst?.InstTownName}, общ. {inst?.InstMunName}, обл. {inst?.InstRegName}";
                reportModel.institutionBulstat = inst?.InstEik ?? "";

            }
            else
            {
                var campaign = await _context.DocManagementCampaigns
                   .Where(x => x.Id == model.CampaignId.Value)
                   .Select(x => new
                   {
                       x.SchoolYear,
                       SchoolYearName = x.SchoolYearNavigation.Name,
                   })
                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new ApiException(Messages.EmptyEntityError);

                if (_userInfo.IsConsortium || _userInfo.IsMon)
                {
                    reportModel.schoolYearName = campaign.SchoolYearName ?? "";
                    reportModel.institutionDirectorName = ""; //Todo: Директор на РУО
                    reportModel.institutionName = ""; // Todo: Име на РУО
                    reportModel.institutionPhoneNumber = ""; // Todo: Телефон на РУО
                    reportModel.institutionEmail = ""; // Todo: Имейл на РУО
                    reportModel.institutionAddress = ""; // Todo: Адрес на РУО
                    reportModel.institutionBulstat = ""; // Todo: Булстат на РУО
                }
                else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
                {
                    reportModel.schoolYearName = campaign.SchoolYearName ?? "";
                    reportModel.institutionDirectorName = ""; //Todo: Директор на РУО
                    reportModel.institutionName = ""; // Todo: Име на РУО
                    reportModel.institutionPhoneNumber = ""; // Todo: Телефон на РУО
                    reportModel.institutionEmail = ""; // Todo: Имейл на РУО
                    reportModel.institutionAddress = ""; // Todo: Адрес на РУО
                    reportModel.institutionBulstat = ""; // Todo: Булстат на РУО
                }
            }
        }

        private async Task<(string fileName, byte[] content)> GenerateApplicationReportInternal(DocManagementReportImputModel model, IEnumerable<DocManagementApplicationViewModel> applications, CancellationToken cancellationToken)
        {
            DocManagementApplicationReportModel reportModel = await GetReportModel(model, applications, cancellationToken);

            string templateName = null;
            if (model.ApplicationId.HasValue)
            {
                templateName = "Institution_ZUD_Request";
            }
            else
            {
                if (_userInfo.IsConsortium)
                {
                    templateName = "All_ZUD_Request";
                }
                else if (_userInfo.IsMon || _userInfo.IsMonExpert)
                {
                    templateName = "MON_ZUD_Request";
                }
                else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
                {
                    templateName = "ROU_ZUD_Request";
                }
                else if (_userInfo.IsSchoolDirector)
                {
                    templateName = "Institution_ZUD_Request";
                }
            }

            if (templateName.IsNullOrWhiteSpace())
            {
                throw new ApiException(Messages.EmptyFileError, new ArgumentNullException(nameof(templateName)));
            }

            using var ms = new MemoryStream();
            WordTemplateConfig templateConfig = await _wordTemplateService.TransformAsync(
                templateName,
                JsonSerializer.Serialize(reportModel),
                ms,
                cancellationToken);

            return (templateConfig.TemplateFileName.Replace("_1", reportModel.institutionCode.IsNullOrWhiteSpace() ? $"_{reportModel.schoolYearName}" : $"_{reportModel.institutionCode}_{reportModel.schoolYearName}"), ms.ToArray());
        }

        public static List<string> ParseSequence(string input)
        {
            var result = new List<string>();
            if (input.IsNullOrWhiteSpace()) return result;

            var parts = input.Split(',');

            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();

                if (trimmedPart.Contains('-'))
                {
                    // Check if this is actually a range (like "1-55/ОМ-24") or just a series with dash (like "200/ОМ-24")
                    var firstDashIndex = trimmedPart.IndexOf('-');
                    bool isRange = false;

                    if (firstDashIndex > 0 && firstDashIndex < trimmedPart.Length - 1)
                    {
                        string beforeDash = trimmedPart.Substring(0, firstDashIndex);
                        string afterDash = trimmedPart.Substring(firstDashIndex + 1);

                        // Check if part before dash is a number
                        if (int.TryParse(beforeDash, out _))
                        {
                            // Check if part after dash starts with a number (indicating range)
                            string afterDashNumberPart = afterDash.Contains('/') ? afterDash.Split('/')[0] : afterDash;
                            if (int.TryParse(afterDashNumberPart, out _))
                            {
                                isRange = true;
                            }
                        }
                    }

                    if (isRange)
                    {
                        // Handle range format: "2-22/2005", "2-22/Г-24", or "2-22"
                        string startPart = trimmedPart.Substring(0, firstDashIndex);
                        string endPart = trimmedPart.Substring(firstDashIndex + 1);

                        // Extract start number and end part (which may contain year/series)
                        if (int.TryParse(startPart, out int start))
                        {
                            string suffix = null; // year or series
                            int end;

                            // Check if end part contains suffix: "22/2005" or "22/ОМ-24"
                            if (endPart.Contains('/'))
                            {
                                var endWithSuffix = endPart.Split('/');
                                if (endWithSuffix.Length == 2 && int.TryParse(endWithSuffix[0], out end))
                                {
                                    suffix = endWithSuffix[1];
                                }
                                else
                                {
                                    continue; // Skip invalid format
                                }
                            }
                            else if (int.TryParse(endPart, out end))
                            {
                                // No suffix in end part
                            }
                            else
                            {
                                continue; // Skip invalid format
                            }

                            // Generate range
                            for (int i = start; i <= end; i++)
                            {
                                string docNumber = suffix != null
                                    ? $"{i:D6}/{suffix}"
                                    : i.ToString("D6");
                                result.Add(docNumber);
                            }
                        }
                    }
                    else
                    {
                        // Handle single number with series containing dash: "200/ОМ-24"
                        if (trimmedPart.Contains('/'))
                        {
                            // Format with suffix: "200/ОМ-24"
                            var numberWithSuffix = trimmedPart.Split('/');
                            if (numberWithSuffix.Length == 2 && int.TryParse(numberWithSuffix[0], out int number))
                            {
                                result.Add($"{number:D6}/{numberWithSuffix[1]}");
                            }
                        }
                        // If no slash, it might be an invalid format, skip it
                    }
                }
                else
                {
                    // Handle single number format: "1/2004", "1/Г-24", or "1"
                    if (trimmedPart.Contains('/'))
                    {
                        // Format with suffix: "1/2004" or "1/Г-24"
                        var numberWithSuffix = trimmedPart.Split('/');
                        if (numberWithSuffix.Length == 2 && int.TryParse(numberWithSuffix[0], out int number))
                        {
                            result.Add($"{number:D6}/{numberWithSuffix[1]}");
                        }
                    }
                    else
                    {
                        // Format without suffix: "1"
                        if (int.TryParse(trimmedPart, out int number))
                        {
                            result.Add(number.ToString("D6"));
                        }
                    }
                }
            }

            return result;
        }

        public static string FormatSequence(string input)
        {
            if (input.IsNullOrWhiteSpace()) return input;

            var parts = input.Split(',');

            for (int i = 0; i < parts.Length; i++)
            {
                var trimmedPart = parts[i].Trim();

                if (trimmedPart.Contains('-'))
                {
                    // Handle range format: "2-22/2005", "2-22/Г-24", or "2-22"
                    var rangeParts = trimmedPart.Split('-');
                    if (rangeParts.Length == 2)
                    {
                        if (int.TryParse(rangeParts[0], out int start))
                        {
                            string endPart = rangeParts[1];

                            // Check if end part contains suffix: "22/2005" or "22/Г-24"
                            if (endPart.Contains('/'))
                            {
                                var endWithSuffix = endPart.Split('/');
                                if (endWithSuffix.Length == 2 && int.TryParse(endWithSuffix[0], out int end))
                                {
                                    parts[i] = $"{start:D6}-{end:D6}/{endWithSuffix[1]}";
                                }
                            }
                            else if (int.TryParse(endPart, out int end))
                            {
                                parts[i] = $"{start:D6}-{end:D6}";
                            }
                        }
                    }
                }
                else
                {
                    // Handle single number format: "1/2004", "1/Г-24", or "1"
                    if (trimmedPart.Contains('/'))
                    {
                        var numberWithSuffix = trimmedPart.Split('/');
                        if (numberWithSuffix.Length == 2 && int.TryParse(numberWithSuffix[0], out int number))
                        {
                            parts[i] = $"{number:D6}/{numberWithSuffix[1]}";
                        }
                    }
                    else
                    {
                        if (int.TryParse(trimmedPart, out int number))
                        {
                            parts[i] = number.ToString("D6");
                        }
                    }
                }
            }

            return string.Join(",", parts);
        }
        #endregion

        public async Task<IPagedList<DocManagementApplicationViewModel>> List(DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<DocManagementApplication> query = _context.DocManagementApplications;

            if (input.CampaignId.HasValue)
            {
                query = query.Where(x => x.CampaignId == input.CampaignId.Value
                    || x.Campaign.ParentId == input.CampaignId.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.RegionId.HasValue)
            {
                query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == input.RegionId.Value);
            }

            if (input.MunicipalityId.HasValue)
            {
                query = query.Where(x => x.InstitutionSchoolYear.Town.MunicipalityId == input.MunicipalityId.Value);
            }

            if (input.CampaignType.HasValue)
            {
                switch (input.CampaignType.Value)
                {
                    case 1:
                        // Всички
                        break;
                    case 2:
                        // Основни кампании
                        query = query.Where(x => x.Campaign.ParentId == null && !x.Campaign.IsHidden);
                        break;
                    case 3:
                        // Допълнителни кампании
                        query = query.Where(x => x.Campaign.ParentId != null && !x.Campaign.IsHidden);
                        break;
                    case 4:
                        // Служежби кампании за размяна на документи от институции
                        query = query.Where(x => x.Campaign.IsHidden);
                        break;
                    default:
                        break;
                }
            }

            if (input.InstType.HasValue)
            {
                switch (input.InstType.Value)
                {
                    case 1:
                        // Всички
                        break;
                    case 2:
                    // С делегиран бюджет
                    case 3:
                        // Без делегиран бюджет
                        query = from app in query
                                join id in _context.InstitutionDetails on app.InstitutionId equals id.InstitutionId into temp
                                from inst in temp.DefaultIfEmpty()
                                where (inst.IsDelegateBudget ?? false) == (input.InstType.Value == 2)
                                select app;
                        break;
                    default:
                        break;
                }
            }

            var currentUserInstitutionCode = _userInfo.InstitutionID;
            if (currentUserInstitutionCode.HasValue)
            {
                query = query.Where(x => x.InstitutionId == currentUserInstitutionCode.Value
                 || x.RequestedInstitutionId == currentUserInstitutionCode.Value);
            }

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID.Value);
            }

            IQueryable<DocManagementApplicationViewModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Status.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.SchoolYearNavigation.Name.Contains(input.Filter)
                   || predicate.InstitutionSchoolYear.Name.Contains(input.Filter)
                   || predicate.InstitutionSchoolYear.Abbreviation.Contains(input.Filter)
                   || predicate.Campaign.Name.Contains(input.Filter))
                   .Select(x => new DocManagementApplicationViewModel
                   {
                       Id = x.Id,
                       ParentId = x.ParentId,
                       CampaignId = x.CampaignId,
                       InstitutionId = x.InstitutionId,
                       SchoolYear = x.SchoolYear,
                       Status = x.Status,
                       IsExchangeRequest = x.IsExchangeRequest,
                       RequestedInstitutionId = x.RequestedInstitutionId,
                       Campaign = new DocManagementCampaignViewModel
                       {
                           Id = x.CampaignId,
                           ParentId = x.Campaign.ParentId,
                           Name = x.Campaign.Name,
                           FromDate = x.Campaign.FromDate,
                           ToDate = x.Campaign.ToDate,
                           IsManuallyActivated = x.Campaign.IsManuallyActivated,
                           IsHidden = x.Campaign.IsHidden
                       },
                       ParentCampaign = x.Campaign.ParentId.HasValue
                            ? new DocManagementCampaignViewModel
                            {
                                Id = x.Campaign.ParentId,
                                Name = x.Campaign.Parent.Name,
                                FromDate = x.Campaign.Parent.FromDate,
                                ToDate = x.Campaign.Parent.ToDate,
                                IsManuallyActivated = x.Campaign.Parent.IsManuallyActivated,
                                IsHidden = x.Campaign.Parent.IsHidden
                            }
                       : null,
                       InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                       SchoolYearName = x.SchoolYearNavigation.Name,
                       RequestedInstitutionName = x.InstitutionSchoolYearNavigation.Abbreviation,
                       CreateDate = x.CreateDate,
                       Creator = x.CreatedBySysUser.Username,
                       ModifyDate = x.ModifyDate,
                       Updater = x.ModifiedBySysUser.Username,
                       CurrentUserInstitutionCode = currentUserInstitutionCode,
                       IsReportable = x.Status == nameof(ApplicationStatusEnum.Approved)
                        && ((x.IsExchangeRequest && x.InstitutionId == currentUserInstitutionCode) || !x.IsExchangeRequest),
                       HasApprovePermission = x.Status == nameof(ApplicationStatusEnum.Submitted)
                        && ((x.IsExchangeRequest && x.InstitutionId == currentUserInstitutionCode) || !x.IsExchangeRequest),
                       BasicDocuments = x.DocManagementApplicationItems
                            .Select(i => new DocManagementApplicationBasicDocumentModel
                            {
                                Id = i.Id,
                                BasicDocumentId = i.BasicDocumentId,
                                BasicDocumentName = i.BasicDocument.Name,
                                Number = i.Number,
                            }),
                       Attachments = x.DocManagementApplicationAttachments
                            .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                   })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "CampaignId desc, Id desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<DocManagementApplicationViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<VDocManagementReport>> ReportList(DocManagementReportListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VDocManagementReport> query = _context.VDocManagementReports;

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.BasicDocumentId.HasValue)
            {
                query = query.Where(x => x.BasicDocumentId == input.BasicDocumentId.Value);
            }

            if (input.RegionId.HasValue)
            {
                query = query.Where(x => x.InstitutionRegionId == input.RegionId.Value);
            }

            if (input.MunicipalityId.HasValue)
            {
                query = query.Where(x => x.InstitutionMunicipalityId == input.MunicipalityId.Value);
            }

            if (input.IsDiplomaSigned.HasValue)
            {
                query = query.Where(x => x.IsDiplomaSigned == input.IsDiplomaSigned.Value);
            }

            if (input.HasDiplomaDocument.HasValue)
            {
                query = query.Where(x => x.HasDiplomaDocument == input.HasDiplomaDocument.Value);
            }

            if (!input.SeriesFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("Series", typeof(string), input.SeriesFilterOp, input.SeriesFilter);
            }

            if (!input.FactoryNumberFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("DocNumber", typeof(string), input.FactoryNumberFilterOp, input.FactoryNumberFilter);
            }

            if (!input.BasicDocumentFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("BasicDocumentName", typeof(string), input.BasicDocumentFilterOp, input.BasicDocumentFilter);
            }

            if (!input.SchoolYearFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("SchoolYearName", typeof(string), input.SchoolYearFilterOp, input.SchoolYearFilter);
            }

            if (!input.PersonFullNameFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("PersonFullName", typeof(string), input.PersonFullNameFilterOp, input.PersonFullNameFilter);
            }

            if (!input.PersonIdentifierFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("PersonIdentifier", typeof(string), input.PersonIdentifierFilterOp, input.PersonIdentifierFilter);
            }

            var currentUserInstitutionCode = _userInfo.InstitutionID;
            if (currentUserInstitutionCode.HasValue)
            {
                query = query.Where(x => x.InstitutionId == currentUserInstitutionCode.Value);
            }

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.InstitutionRegionId == _userInfo.RegionID.Value);
            }

            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.BasicDocumentName.Contains(input.Filter)
                   || predicate.Series.Contains(input.Filter)
                   || predicate.DocNumber.Contains(input.Filter)
                   || predicate.PersonFullName.Contains(input.Filter)
                   || predicate.PersonIdentifier.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, BasicDocumentId desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            List<VDocManagementReport> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<DocManagementApplicationViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead)
               && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            bool isAdmin = _userInfo.IsConsortium;
            int? userRegionId = _userInfo.IsRuo || _userInfo.IsRuoExpert ? _userInfo.RegionID : null;

            var application = await _context.DocManagementApplications
                .Where(x => x.Id == id)
                 .Select(x => new DocManagementApplicationViewModel
                 {
                     Id = x.Id,
                     ParentId = x.ParentId,
                     CampaignId = x.CampaignId,
                     InstitutionId = x.InstitutionId,
                     SchoolYear = x.SchoolYear,
                     Status = x.Status,
                     InstitutionRegionId = x.InstitutionSchoolYear.Town.Municipality.RegionId,
                     IsExchangeRequest = x.IsExchangeRequest,
                     RequestedInstitutionId = x.RequestedInstitutionId,
                     Campaign = new DocManagementCampaignViewModel
                     {
                         Id = x.CampaignId,
                         ParentId = x.Campaign.ParentId,
                         Name = x.Campaign.Name,
                         FromDate = x.Campaign.FromDate,
                         ToDate = x.Campaign.ToDate,
                         IsManuallyActivated = x.Campaign.IsManuallyActivated,
                         Description = x.Campaign.Description,
                         SchoolYear = x.Campaign.SchoolYear,
                         SchoolYearName = x.Campaign.SchoolYearNavigation.Name,
                         IsHidden = x.Campaign.IsHidden
                     },
                     ParentCampaign = x.Campaign.ParentId.HasValue
                        ? new DocManagementCampaignViewModel
                        {
                            Id = x.Campaign.ParentId,
                            Name = x.Campaign.Parent.Name,
                            FromDate = x.Campaign.Parent.FromDate,
                            ToDate = x.Campaign.Parent.ToDate,
                            IsManuallyActivated = x.Campaign.Parent.IsManuallyActivated,
                            IsHidden = x.Campaign.Parent.IsHidden,
                        }
                       : null,
                     InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                     SchoolYearName = x.SchoolYearNavigation.Name,
                     RequestedInstitutionName = x.InstitutionSchoolYearNavigation.Abbreviation,
                     CreateDate = x.CreateDate,
                     Creator = x.CreatedBySysUser.Username,
                     ModifyDate = x.ModifyDate,
                     Updater = x.ModifiedBySysUser.Username,
                     BasicDocuments = x.DocManagementApplicationItems
                        .Select(i => new DocManagementApplicationBasicDocumentModel
                        {
                            Id = i.Id,
                            BasicDocumentId = i.BasicDocumentId,
                            BasicDocumentName = i.BasicDocument.Name,
                            Number = i.Number,
                            HasFactoryNumber = i.BasicDocument.HasFactoryNumber,
                            SeriesFormat = i.BasicDocument.SeriesFormat,
                            IsDuplicate = i.BasicDocument.IsDuplicate,
                            DeliveredCount = i.DeliveredCount,
                            DeliveredNumbers = i.DeliveredNumbers,
                            DeliveryNotes = i.DeliveryNotes,
                        }),
                     Attachments = x.DocManagementApplicationAttachments
                            .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                     HasEvaluationPermission = x.Status != nameof(ApplicationStatusEnum.ReturnedForCorrection) && x.Status != nameof(ApplicationStatusEnum.Draft)
                        && (isAdmin
                            || (userRegionId.HasValue && x.InstitutionSchoolYear.Town.Municipality.RegionId == userRegionId )),
                     HasApprovePermission = x.Status == nameof(ApplicationStatusEnum.Submitted)
                        && ((x.IsExchangeRequest && x.RequestedInstitutionId == _userInfo.InstitutionID)
                        ||
                        (!x.IsExchangeRequest && (isAdmin
                            || (userRegionId.HasValue && x.InstitutionSchoolYear.Town.Municipality.RegionId == userRegionId))))                      ,
                 })
                 .FirstOrDefaultAsync(cancellationToken);

            if (_userInfo.IsConsortium)
            {
                // Чете всчико
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                if (application.InstitutionRegionId != _userInfo.RegionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else if (_userInfo.IsSchoolDirector)
            {
                if (application.IsExchangeRequest)
                {
                    if (application.InstitutionId != _userInfo.InstitutionID && application.RequestedInstitutionId != _userInfo.InstitutionID)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }
                }
                else
                {
                    if (application.InstitutionId != _userInfo.InstitutionID)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }
                }
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return application;
        }

        /// <summary>
        /// Връща основните документи, които могат да се заявяват,
        /// според типа на училището(detailedSchoolType) на логнатия потребител
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<DropdownViewModel>> GetBasicDocuments(CancellationToken cancellationToken)
        {
            var detailedSchoolType = await _institutionService.GetInstitutionDetailedSchoolTypeId(_userInfo?.InstitutionID);

            return await _context.BasicDocumentLimits
                .Where(x => x.DetailedSchoolTypeId == detailedSchoolType && x.BasicDocument.IsValid)
                .OrderBy(x => x.BasicDocument.Name)
                .Select(x => new DropdownViewModel
                {
                    Value = x.BasicDocumentId,
                    Text = x.BasicDocument.Name,
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocManagementApplicationStatusViewModel>> GetStatuses(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.DocManagementApplicationStatuses
                .Where(x => x.ApplicationId == id && x.ParentId == null)
                .OrderByDescending(x => x.Id)
                .Select(x => new DocManagementApplicationStatusViewModel
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    InstitutionId = x.Application.InstitutionId,
                    Status = x.Status,
                    Description = x.Description,
                    ParentId = x.ParentId,
                    CreateDate = x.CreateDate,
                    Creator = x.CreatedBySysUser.Username,
                    ModifyDate = x.ModifyDate,
                    Updater = x.ModifiedBySysUser.Username,
                    HasResponsePermission = x.Status == nameof(ApplicationStatusEnum.ReturnedForCorrection)
                        && x.Application.InstitutionId == _userInfo.InstitutionID
                        && x.InverseParent.Count == 0,
                    Responses = x.InverseParent
                        .OrderByDescending(c => c.Id)
                        .Select(c => new DocManagementApplicationStatusViewModel
                        {
                            Id = c.Id,
                            ApplicationId = c.ApplicationId,
                            InstitutionId = c.Application.InstitutionId,
                            Status = c.Status,
                            Description = c.Description,
                            HasResponsePermission = false,
                        })
                })
                .ToListAsync(cancellationToken);
        }

        public async Task Create(DocManagementApplicationModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int institutionId = _userInfo.InstitutionID ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementCampaignViewModel campaign = await CampaignValidation(model.CampaignId);
            if (await _context.DocManagementApplications.AnyAsync(x => x.CampaignId == model.CampaignId && x.InstitutionId == institutionId))
            {
                throw new ApiException(Messages.DocManagementApplicationAlreadyExits);
            }

            DocManagementApplication entry = new DocManagementApplication
            {
                CampaignId = campaign.Id ?? 0,
                InstitutionId = institutionId,
                SchoolYear = campaign.SchoolYear,
                Status = nameof(ApplicationStatusEnum.Submitted),
                DocManagementApplicationItems = model.BasicDocuments.Select(x => new DocManagementApplicationItem
                {
                    BasicDocumentId = x.BasicDocumentId,
                    Number = x.Number,
                }).ToArray(),
            };

            entry.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
            {
                Status = nameof(ApplicationStatusEnum.Submitted)
            });

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entry);
            _context.DocManagementApplications.Add(entry);
            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(DocManagementApplicationModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationItems)
                .Include(x => x.DocManagementApplicationAttachments)
                .Include(x => x.DocManagementApplicationStatuses)
                .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new ApiException(Messages.EmptyEntityError);

            int institutionId = _userInfo.InstitutionID ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);
            if (entity.InstitutionId != institutionId)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.Status == nameof(ApplicationStatusEnum.Approved) || entity.Status == nameof(ApplicationStatusEnum.Rejected))
            {
                throw new ApiException(Messages.ApplicationStatusError, new InvalidOperationException());
            }

            DocManagementCampaignViewModel campaign = await CampaignValidation(entity.CampaignId, entity);

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            foreach (DocManagementApplicationBasicDocumentModel basicDocumentModel in model.BasicDocuments)
            {
                DocManagementApplicationItem item = entity.DocManagementApplicationItems.FirstOrDefault(x => x.Id == basicDocumentModel.Id);
                if (item != null)
                {
                    item.Number = basicDocumentModel.Number;
                }
                else
                {
                    entity.DocManagementApplicationItems.Add(new DocManagementApplicationItem
                    {
                        BasicDocumentId = basicDocumentModel.BasicDocumentId,
                        Number = basicDocumentModel.Number
                    });
                }
            }

            if (entity.Status == nameof(ApplicationStatusEnum.ReturnedForCorrection))
            {
                entity.Status = nameof(ApplicationStatusEnum.Submitted);

                var lastStatus = entity.DocManagementApplicationStatuses.OrderByDescending(x => x.Id).FirstOrDefault();
                entity.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
                {
                    Status = lastStatus?.Status == nameof(ApplicationStatusEnum.ReturnedForCorrection) ? nameof(ApplicationStatusEnum.Response) : nameof(ApplicationStatusEnum.Submitted),
                    ParentId = lastStatus?.Status == nameof(ApplicationStatusEnum.ReturnedForCorrection) ? lastStatus.Id : (int?)null,
                    Description = "Последваща промяна"
                });
            }

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task AttachApplicationReport(DocManagementApplicationModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationAttachments)
                .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new ApiException(Messages.EmptyEntityError);

            int institutionId = _userInfo.InstitutionID ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);
            if (entity.InstitutionId != institutionId)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementCampaignViewModel campaign = await CampaignValidation(entity.CampaignId, entity);

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task ReportDelivery(DocManagementApplicationModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication application = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationItems).ThenInclude(x => x.DocManagementApplicationItemDeliveredDocAppItems)
                .Include(x => x.DocManagementApplicationItems).ThenInclude(x => x.BasicDocument)
                .Include(x => x.DocManagementApplicationItems).ThenInclude(x => x.DocManagementApplicationItemDeliveredDocOrigAppItems)
                .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new ApiException(Messages.EmptyEntityError);

            if (application.InstitutionId != _userInfo.InstitutionID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (application.Status != nameof(ApplicationStatusEnum.Submitted) && application.Status != nameof(ApplicationStatusEnum.Approved))
            {
                throw new ApiException(Messages.ApplicationStatusError, 400);
            }

            application.DeliveryNotes = model.DeliveryNotes;


            var requestedInstItems = application.IsExchangeRequest
                ? await _context.DocManagementApplicationItemDeliveredDocs
                .Include(x => x.AppItem)
                .Where(x => x.AppItem.Application.InstitutionId == application.RequestedInstitutionId
                    && (x.Edition.HasValue || x.AppItem.Application.SchoolYear == application.SchoolYear))
                .ToListAsync()
                : new List<DocManagementApplicationItemDeliveredDoc>();


            ApiValidationResult validationResult = new ApiValidationResult();

            foreach (DocManagementApplicationBasicDocumentModel basicDocumentModel in model.BasicDocuments)
            {
                DocManagementApplicationItem item = application.DocManagementApplicationItems.FirstOrDefault(x => x.Id == basicDocumentModel.Id);
                if (item != null)
                {

                    if (!item.BasicDocument.HasFactoryNumber && application.IsExchangeRequest)
                    {
                        // Документ от одобряващата институция, който съответства на типа на документа
                        DocManagementApplicationItemDeliveredDoc requestedInstItem = requestedInstItems
                            .FirstOrDefault(x => x.AppItem.BasicDocumentId == item.BasicDocumentId);
                        if (requestedInstItem != null)
                        {
                            var diff = basicDocumentModel.DeliveredCount - item.DeliveredCount;
                            requestedInstItem.AppItem.DeliveredCount -= basicDocumentModel.DeliveredCount;
                        }
                    }

                    item.DeliveredCount = basicDocumentModel.DeliveredCount;
                    item.DeliveryNotes = basicDocumentModel.DeliveryNotes;

                    if (!string.Equals(item.DeliveredNumbers, basicDocumentModel.DeliveredNumbers, StringComparison.OrdinalIgnoreCase))
                    {
                        var formattedNumbers = FormatSequence(basicDocumentModel.DeliveredNumbers) ?? string.Empty;
                        item.DeliveredNumbers = formattedNumbers;

                        HashSet<string> numbers = ParseSequence(basicDocumentModel.DeliveredNumbers).ToHashSet();

                        // Get existing doc numbers in the same format for comparison
                        var existingNumbers = item.DocManagementApplicationItemDeliveredDocAppItems
                            .Select(d =>
                            {
                                if (d.Edition.HasValue)
                                    return $"{d.DocNumber}/{d.Edition}";
                                else if (!string.IsNullOrEmpty(d.Series))
                                    return $"{d.DocNumber}/{d.Series}";
                                else
                                    return d.DocNumber;
                            })
                            .ToHashSet();

                        var toAdd = numbers.Except(existingNumbers).ToList();
                        var toDelete = item.DocManagementApplicationItemDeliveredDocAppItems
                            .Where(d =>
                            {
                                string existingDoc;
                                if (d.Edition.HasValue)
                                    existingDoc = $"{d.DocNumber}/{d.Edition}";
                                else if (!string.IsNullOrEmpty(d.Series))
                                    existingDoc = $"{d.DocNumber}/{d.Series}";
                                else
                                    existingDoc = d.DocNumber;
                                return !numbers.Contains(existingDoc);
                            })
                            .ToList();

                        foreach (var docNum in toAdd)
                        {
                            // Extract year/series if present in format "000123/2024" or "000123/Г-24"
                            string docNumber = docNum;
                            short? year = null;
                            string series = null;

                            if (docNum.Contains('/'))
                            {
                                var parts = docNum.Split('/');
                                if (parts.Length == 2)
                                {
                                    docNumber = parts[0];
                                    // Check if it's a year (4 digits) or series format
                                    if (short.TryParse(parts[1], out short parsedYear) && parts[1].Length == 4)
                                    {
                                        year = parsedYear;
                                    }
                                    else
                                    {
                                        series = parts[1];
                                    }
                                }
                            }

                            // Документ от одобряващата институция, който съответства на номера, годината и серията
                            DocManagementApplicationItemDeliveredDoc requestedInstItem = requestedInstItems
                                .FirstOrDefault(x => x.AppItem.BasicDocumentId == item.BasicDocumentId
                                    && x.DocNumber == docNumber
                                    && x.Edition == year
                                    && x.Series == series);

                            if (requestedInstItem != null)
                            {
                                requestedInstItem.OrigAppItemId = requestedInstItem.AppItemId;
                                requestedInstItem.AppItemId = item.Id;
                            }
                            else
                            {
                                if (requestedInstItem == null
                                    && item.DocManagementApplicationItemDeliveredDocOrigAppItems
                                        .Any(x => x.DocNumber == docNum && x.Series == series && x.Edition == year))
                                {
                                    var origItem = item.DocManagementApplicationItemDeliveredDocOrigAppItems
                                        .FirstOrDefault(x => x.DocNumber == docNum && x.Series == series && x.Edition == year);
                                    if (origItem != null)
                                    {
                                        var app = await _context.DocManagementApplicationItems
                                            .Where(x => x.Id == origItem.AppItemId)
                                            .Select(x => new
                                            {
                                                x.Application.InstitutionId,
                                                x.Application.InstitutionSchoolYearNavigation.Name,
                                            })
                                            .FirstOrDefaultAsync();
                                        validationResult.Errors.Add(new ValidationError($"Документ с №: {docNum} , серия: {series}, издание: {year} и предеаден и заприходен на институция: {app?.InstitutionId} - {app?.Name}"));
                                    }
                                }

                                item.DocManagementApplicationItemDeliveredDocAppItems.Add(new DocManagementApplicationItemDeliveredDoc
                                {
                                    DocNumber = docNumber,
                                    Edition = year,
                                    Series = series,
                                    OrigAppItemId = requestedInstItem?.AppItemId, // Документ от одобряващата институция
                                });
                            }
                        }

                        if (validationResult.HasErrors)
                        {
                            // Съществуват валидационни грешки. Прекъсваме изпълнението.
                            throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
                        }

                        if (toDelete.Count > 0)
                        {
                            // OrigAppItemId показа връзката на елемента с елемените от одобрявашата институция.
                            var origItemIds = toDelete.Where(x => x.OrigAppItemId.HasValue)
                                .Select(x => x.OrigAppItemId.Value).ToHashSet();

                            // Елементи от обобряващата институция.
                            var origItems = await _context.DocManagementApplicationItemDeliveredDocs
                                .Where(x => origItemIds.Contains(x.AppItemId))
                                .Select(x => new
                                {
                                    x.AppItemId,
                                    x.DocNumber,
                                    x.Edition,
                                    x.Series,
                                }).ToListAsync();

                            foreach (DocManagementApplicationItemDeliveredDoc doc in toDelete)
                            {
                                if (doc.OrigAppItemId.HasValue && !origItems.Any(x => x.DocNumber == doc.DocNumber && x.Edition == doc.Edition && x.Series == doc.Series))
                                {
                                    // За одобряващата институция липсва запис с OrigAppItemId и същия номер, година и серия.
                                    // Връщаме му го
                                    doc.AppItemId = doc.OrigAppItemId.Value;
                                    doc.OrigAppItemId = null;
                                }
                                else
                                {
                                    // Изтриваме доставения документ
                                    _context.DocManagementApplicationItemDeliveredDocs.Remove(doc);
                                }
                            }
                        }
                    }
                }

            }

            await SaveAsync();
        }

        public async Task Submit(DocManagementApplicationReturnForCorectionModel model, CancellationToken cancellationToken)
        {

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationStatuses)
                .Include(x => x.Campaign)
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId, cancellationToken) ?? throw new ApiException(Messages.EmptyEntityError);

            if (entity.InstitutionId != _userInfo.InstitutionID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.Status = nameof(ApplicationStatusEnum.Submitted);
            entity.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
            {
                Status = nameof(ApplicationStatusEnum.Submitted),
                Description = model.Description,
            });

            await SaveAsync(cancellationToken);
        }

        public async Task Approve(DocManagementExchangeRequestApproveModel model, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId && !x.IsExchangeRequest, cancellationToken)
                ?? throw new ApiException(Messages.EmptyEntityError);

            if (entity.Status != nameof(ApplicationStatusEnum.Submitted))
            {
                throw new ApiException(Messages.ApplicationStatusError, 404);
            }

            bool isAdmin = _userInfo.IsConsortium;
            int? regionId = _userInfo.IsRuo || _userInfo.IsRuoExpert ? _userInfo.RegionID : null;
            var hasEvaluationPermission = isAdmin
                || (regionId.HasValue
                    && await _context.DocManagementApplications.AnyAsync(x => x.Id == model.ApplicationId && x.InstitutionSchoolYear.Town.Municipality.RegionId == regionId));

            if (!hasEvaluationPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.Status = nameof(ApplicationStatusEnum.Approved);
            entity.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
            {
                Status = nameof(ApplicationStatusEnum.Approved),
                Description = model.Description
            });

            await SaveAsync(cancellationToken);
        }

        public async Task Reject(DocManagementExchangeRequestRejectModel model, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId && !x.IsExchangeRequest, cancellationToken)
                ?? throw new ApiException(Messages.EmptyEntityError);

            if (entity.Status != nameof(ApplicationStatusEnum.Submitted))
            {
                throw new ApiException(Messages.ApplicationStatusError, 404);
            }

            bool isAdmin = _userInfo.IsConsortium;
            int? regionId = _userInfo.IsRuo || _userInfo.IsRuoExpert ? _userInfo.RegionID : null;
            var hasEvaluationPermission = isAdmin
                || (regionId.HasValue
                    && await _context.DocManagementApplications.AnyAsync(x => x.Id == model.ApplicationId && x.InstitutionSchoolYear.Town.Municipality.RegionId == regionId));

            if (!hasEvaluationPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.Status = nameof(ApplicationStatusEnum.Rejected);
            entity.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
            {
                Status = nameof(ApplicationStatusEnum.Rejected),
                Description = model.Description
            });

            await SaveAsync(cancellationToken);
        }

        public async Task ReturnForCorection(DocManagementApplicationReturnForCorectionModel model, CancellationToken cancellationToken)
        {

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationStatuses)
                .Include(x => x.Campaign)
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId, cancellationToken) ?? throw new ApiException(Messages.EmptyEntityError);

            bool isAdmin = _userInfo.IsConsortium;
            int? regionId = _userInfo.IsRuo || _userInfo.IsRuoExpert ? _userInfo.RegionID : null;
            var hasEvaluationPermission = isAdmin
                || (regionId.HasValue
                    && await _context.DocManagementApplications.AnyAsync(x => x.Id == model.ApplicationId && x.InstitutionSchoolYear.Town.Municipality.RegionId == regionId));

            if (!hasEvaluationPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.Status = nameof(ApplicationStatusEnum.ReturnedForCorrection);
            entity.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
            {
                Status = nameof(ApplicationStatusEnum.ReturnedForCorrection),
                Description = model.Description
            });

            await SaveAsync(cancellationToken);
        }

        public async Task ActionResponse(DocManagementApplicationResponseModel model, CancellationToken cancellationToken)
        {

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationStatuses)
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId, cancellationToken) ?? throw new ApiException(Messages.EmptyEntityError);

            if (entity.InstitutionId != _userInfo.InstitutionID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.Status = nameof(ApplicationStatusEnum.Submitted);
            entity.DocManagementApplicationStatuses.Add(new DocManagementApplicationStatus
            {
                Status = nameof(ApplicationStatusEnum.Response),
                Description = model.Description,
                ParentId = model.ParentId,
            });

            await SaveAsync(cancellationToken);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationAttachments)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsExchangeRequest) ?? throw new ApiException(Messages.EmptyEntityError);

            int institutionId = _userInfo.InstitutionID ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);
            if (entity.InstitutionId != institutionId)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementCampaignViewModel campaign = await CampaignValidation(entity.CampaignId);

            if (entity.DocManagementApplicationAttachments != null && entity.DocManagementApplicationAttachments.Any())
            {
                var docsIds = entity.DocManagementApplicationAttachments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.DocManagementApplicationAttachments.RemoveRange(entity.DocManagementApplicationAttachments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            _context.DocManagementApplications.Remove(entity);
            await SaveAsync(cancellationToken);
        }

        public async Task<(string fileName, byte[] content, string contentType)> GenerateApplicationReport(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementReportCreate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IEnumerable<DocManagementApplicationViewModel> applications = await GetApplications(model, cancellationToken);

            if (_userInfo.IsSchoolDirector && !applications.All(x => x.InstitutionId == _userInfo.InstitutionID))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.GroupingType == 3)
            {
                List<(string fileName, byte[] content)> files = new List<(string fileName, byte[] content)>();

                var grouped = applications.GroupBy(x => x.InstitutionMunicipalityId);
                foreach (var group in grouped)
                {
                    var apps = group.ToList();
                    (string fileName, byte[] content) = await GenerateApplicationReportInternal(model, apps, cancellationToken);
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{apps.First().InstitutionMunicipalityCode}{Path.GetExtension(fileName)}";
                    files.Add((fileName, content));
                }

                (string zipFileName, byte[] zipContent) = await CreateZipWithDocsAsync(files);

                return (zipFileName, zipContent, "application/zip");
            }
            else if (model.GroupingType == 2)
            {
                List<(string fileName, byte[] content)> files = new List<(string fileName, byte[] content)>();

                var grouped = applications.GroupBy(x => x.InstitutionRegionId);
                foreach (var group in grouped)
                {
                    var apps = group.ToList();
                    (string fileName, byte[] content) = await GenerateApplicationReportInternal(model, apps, cancellationToken);
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{apps.First().InstitutionRegionCode}{Path.GetExtension(fileName)}";
                    files.Add((fileName, content));
                }

                (string zipFileName, byte[] zipContent) = await CreateZipWithDocsAsync(files);

                return (zipFileName, zipContent, "application/zip");
            }
            else
            {
                (string fileName, byte[] content) = await GenerateApplicationReportInternal(model, applications, cancellationToken);

                return (fileName, content, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
        }


        /// <summary>
        /// Приложение № 6 към чл. 52, ал. 1
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(string fileName, byte[] content, string contentType)> GenerateReport(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage)
                || !_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ReportDataModel reportData = await GetReportData(model.SchoolYear, _userInfo.InstitutionID.Value, cancellationToken);
            DocManagementReportModel reportModel = CreateReportModel(reportData);
            return await _wordTemplateService.GenerateWordDocument(reportModel, "Report_ZUD", cancellationToken);
        }

        /// <summary>
        /// Приложение № 6 към чл. 52, ал. 1
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(string fileName, byte[] content, string contentType)> GenerateDestructionProtocol(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage)
                || !_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var docs = await _context.VDocManagementReports
                .Where(x => x.InstitutionId == _userInfo.InstitutionID && x.SchoolYear == model.SchoolYear && x.HasFactoryNumber && x.HasDiplomaDocument != true)
                .ToListAsync(cancellationToken);

            var institution = await _context.InstitutionSchoolYears
                .Where(x => x.InstitutionId == _userInfo.InstitutionID && x.SchoolYear == model.SchoolYear)
                .Select(x => new
                {
                    x.InstitutionId,
                    x.Name,
                    x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    TownName = x.Town.Name,
                    MunicipalityName = x.Town.Municipality.Name,
                    RegionName = x.Town.Municipality.Region.Name,
                })
                .FirstOrDefaultAsync() ?? throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(InstitutionSchoolYear)));

            var reportModel = new DocManagementReportModel
            {
                institutionName = institution.Name ?? "",
                institutionTown = institution.TownName ?? "",
                institutionMunicipality = institution.MunicipalityName ?? "",
                institutionRegion = institution.RegionName ?? "",
                schoolYearName = institution.SchoolYearName ?? "",
                today = $"{DateTime.Now: dd.MM.yyyy} г.",
                destructionDocuments = docs.GroupBy(x => x.BasicDocumentName)
                    .Select(g => new DocManagementApplicationReportDocumentModel
                    {
                        name = g.Key,
                        nomenclatureNumber = g.Key.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                        count = $"{g.Count()} бр.",
                        documents = g.OrderBy(x => x.Series).ThenBy(x => x.DocNumber)
                        .Select(x => new DocManagementApplicationReportDocumentModel
                        {
                            name = x.BasicDocumentName,
                            nomenclatureNumber = x.BasicDocumentName.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                            series = x.Series,
                            fromNumber = x.DocNumber,
                            edition = x.Edition.HasValue ? x.Edition.Value.ToString() : "",
                            count = "1 бр."
                        }).ToList()
                    })
                    .OrderBy(x => x.nomenclatureNumber)
                    .ToList()
            };

            return await _wordTemplateService.GenerateWordDocument(reportModel, "Destruction_Protocol_ZUD", cancellationToken);
        }

        public async Task<IPagedList<VDocManagementDashboard>> Dashboard(DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VDocManagementDashboard> query = _context.VDocManagementDashboards;

            if (input.CampaignId.HasValue)
            {
                query = query.Where(x => x.DocManagementCampaignId == input.CampaignId.Value
                    || x.DocManagementCampaignParentId == input.CampaignId.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (input.RequestedInstitutionId.HasValue)
            {
                query = query.Where(x => x.RequestedInstitutionId == input.RequestedInstitutionId.Value);
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.RegionId.HasValue)
            {
                query = query.Where(x => x.InstitutionRegionId == input.RegionId.Value);
            }

            if (input.MunicipalityId.HasValue)
            {
                query = query.Where(x => x.InstitutionMunicipalityId == input.MunicipalityId.Value);
            }

            if (input.CampaignType.HasValue)
            {
                switch (input.CampaignType.Value)
                {
                    case 1:
                        // Всички
                        break;
                    case 2:
                        // Основни кампании
                        query = query.Where(x => x.DocManagementCampaignParentId == null && x.DocManagementCampaignIsHidden == true);
                        break;
                    case 3:
                        // Допълнителни кампании
                        query = query.Where(x => x.DocManagementCampaignParentId != null && x.DocManagementCampaignIsHidden != true);
                        break;
                    case 4:
                        // Служежби кампании за размяна на документи от институции
                        query = query.Where(x => x.DocManagementCampaignIsHidden == true);
                        break;
                    default:
                        break;
                }
            }

            if (input.InstType.HasValue)
            {
                switch (input.InstType.Value)
                {
                    case 1:
                        // Всички
                        break;
                    case 2:
                    // С делегиран бюджет
                    case 3:
                        // Без делегиран бюджет
                        query = query.Where(x => (x.InstitutionIsDelegateBudget ?? false) == (input.InstType.Value == 2));
                        break;
                    default:
                        break;
                }
            }

            var currentUserInstitutionCode = _userInfo.InstitutionID;
            if (currentUserInstitutionCode.HasValue)
            {
                query = query.Where(x => x.InstitutionId == currentUserInstitutionCode.Value
                 || x.RequestedInstitutionId == currentUserInstitutionCode.Value);
            }

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.InstitutionRegionId == _userInfo.RegionID.Value);
            }

            switch (input.ApplicationStatusFilter)
            {
                case 0: // Неподадени
                    query = query.Where(x => !x.DocManagementApplicationId.HasValue);
                    break;
                case 1: // Подадени
                    query = query.Where(x => x.DocManagementApplicationId.HasValue);
                    break;
                case 3: // Всики
                default:
                    break;
            }

            switch (input.CampaignStatusFilter)
            {
                case 0: // Неактивна кампания
                    query = query.Where(x => x.DocManagementCampaignIsActive != true);
                    break;
                case 1: // Активна кампания
                    query = query.Where(x => x.DocManagementCampaignIsActive == true);
                    break;
                case 2: // Всики
                default:
                    break;
            }

            if (!input.Status.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.DocManagementApplicationStatus == input.Status);
            }

            IQueryable<VDocManagementDashboard> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.DocManagementApplicationStatus.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.RequestedInstitutionId.ToString().Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.InstitutionAbbreviation.Contains(input.Filter)
                   || predicate.RequestedInstitutionName.Contains(input.Filter)
                   || predicate.RequestedInstitutionAbbreviation.Contains(input.Filter)
                   || predicate.DocManagementCampaignName.Contains(input.Filter))
                   .Select(x => x)   
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<VDocManagementDashboard> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        private DocManagementReportModel CreateReportModel(ReportDataModel reportData)
        {
            var model = new DocManagementReportModel
            {
                institutionName = reportData.InstitutionName ?? "",
                institutionTown = reportData.InstitutionTown ?? "",
                institutionMunicipality = reportData.InstitutionMunicipality ?? "",
                institutionRegion = reportData.InstitutionRegion ?? "",
                schoolYearName = reportData.SchoolYearName ?? "",
                today = $"{DateTime.Now: dd.MM.yyyy} г.",
                applicationDocuments = DocManagementUtils.ProcessDocumentGroups(reportData.ApplicationDocuments).ToList(),
                requestedDocuments = DocManagementUtils.ProcessDocumentGroups(reportData.RequestedDocuments).ToList(),
                transferredDocuments = DocManagementUtils.ProcessDocumentGroups(reportData.TransferredDocuments).ToList()
            };

            foreach (var doc in reportData.ApplicationDocuments.Where(x => x.Number > 0).GroupBy(x => x.BasicDocumentId))
            {
                var docModel = model.applicationDocuments.Where(x => x.BasicDocumentId == doc.Key)
                    .FirstOrDefault();

                if (docModel == null)
                {
                    docModel = new DocManagementApplicationReportDocumentModel
                    {
                        BasicDocumentId = doc.Key,
                        nomenclatureNumber = doc.First().BasicDocumentName.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                        name = doc.First().BasicDocumentName,
                    };

                    model.applicationDocuments.Add(docModel);
                }

                docModel.requestedCount = doc.Sum(x => x.Number).ToString();
            }

            model.applicationDocuments = model.applicationDocuments
                .OrderBy(x => x.nomenclatureNumber)
                .ThenByDescending(x => x.requestedCount)
                .ToList();

            return model;
        }

        private async Task<ReportDataModel> GetReportData(short schooYear, int institutionId, CancellationToken cancellationToken)
        {
            var institution = await _context.InstitutionSchoolYears
                .Where(x => x.InstitutionId == institutionId && x.SchoolYear == schooYear)
                .Select(x => new
                {
                    x.InstitutionId,
                    x.Name,
                    x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    TownName = x.Town.Name,
                    MunicipalityName = x.Town.Municipality.Name,
                    RegionName = x.Town.Municipality.Region.Name,
                })
                .FirstOrDefaultAsync() ?? throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(InstitutionSchoolYear)));


            var model = new ReportDataModel
            {
                InstitutionId = institution.InstitutionId,
                InstitutionName = institution.Name,
                InstitutionTown = institution.TownName,
                InstitutionMunicipality = institution.MunicipalityName,
                InstitutionRegion = institution.RegionName,
                SchoolYear = institution.SchoolYear,
                SchoolYearName = institution.SchoolYearName,
                ApplicationDocuments = await _context.DocManagementApplicationItems
                    .Where(x => x.Application.InstitutionId == institutionId && x.Application.SchoolYear == schooYear
                        && !x.Application.IsExchangeRequest && x.Application.Status == nameof(ApplicationStatusEnum.Approved)
                        && x.BasicDocument.HasFactoryNumber)
                    .Select(i => new DocManagementApplicationBasicDocumentModel
                    {
                        Id = i.Id,
                        BasicDocumentId = i.BasicDocumentId,
                        BasicDocumentName = i.BasicDocument.Name,
                        IsDuplicate = i.BasicDocument.IsDuplicate,
                        HasFactoryNumber = i.BasicDocument.HasFactoryNumber,
                        SeriesFormat = i.BasicDocument.SeriesFormat,
                        Number = i.Number,
                        SchoolYear = schooYear,
                        DeliveredCount = i.DeliveredCount,
                        DeliveredItems = i.DocManagementApplicationItemDeliveredDocAppItems
                            .Select(d => new DocManagementApplicationDeliveredDocModel
                            {
                                DocNumber = d.DocNumber,
                                Edition = d.Edition,
                                Series = d.Series,
                            }).ToList()
                    })
                    .ToListAsync(cancellationToken),
                RequestedDocuments = await _context.DocManagementApplicationItems
                    .Where(x => x.Application.InstitutionId == institutionId && x.Application.SchoolYear == schooYear
                        && x.Application.IsExchangeRequest && x.Application.Status == nameof(ApplicationStatusEnum.Approved)
                        && x.BasicDocument.HasFactoryNumber)
                    .Select(i => new DocManagementApplicationBasicDocumentModel
                    {
                        Id = i.Id,
                        BasicDocumentId = i.BasicDocumentId,
                        BasicDocumentName = i.BasicDocument.Name,
                        IsDuplicate = i.BasicDocument.IsDuplicate,
                        HasFactoryNumber = i.BasicDocument.HasFactoryNumber,
                        SeriesFormat = i.BasicDocument.SeriesFormat,
                        Number = i.Number,
                        SchoolYear = schooYear,
                        DeliveredCount = i.DeliveredCount,
                        DeliveredItems = i.DocManagementApplicationItemDeliveredDocAppItems
                            .Select(d => new DocManagementApplicationDeliveredDocModel
                            {
                                DocNumber = d.DocNumber,
                                Edition = d.Edition,
                                Series = d.Series,
                            }).ToList(),
                    })
                    .ToListAsync(cancellationToken),
                TransferredDocuments = await _context.DocManagementApplicationItems
                    .Where(x => x.Application.RequestedInstitutionId == institutionId && x.Application.SchoolYear == schooYear
                        && x.Application.IsExchangeRequest && x.Application.Status == nameof(ApplicationStatusEnum.Approved)
                        && x.BasicDocument.HasFactoryNumber)
                    .Select(i => new DocManagementApplicationBasicDocumentModel
                    {
                        Id = i.Id,
                        BasicDocumentId = i.BasicDocumentId,
                        BasicDocumentName = i.BasicDocument.Name,
                        IsDuplicate = i.BasicDocument.IsDuplicate,
                        HasFactoryNumber = i.BasicDocument.HasFactoryNumber,
                        SeriesFormat = i.BasicDocument.SeriesFormat,
                        Number = i.Number,
                        SchoolYear = schooYear,
                        DeliveredCount = i.DeliveredCount,
                        DeliveredItems = i.DocManagementApplicationItemDeliveredDocAppItems
                            .Select(d => new DocManagementApplicationDeliveredDocModel
                            {
                                DocNumber = d.DocNumber,
                                Edition = d.Edition,
                                Series = d.Series,
                            }).ToList(),
                    })
                    .ToListAsync(cancellationToken)
            };

            var itemsIds = model.ApplicationDocuments.Select(x => x.Id).ToHashSet();
            var oridItems = await _context.DocManagementApplicationItemDeliveredDocs
                .Where(x => x.OrigAppItemId.HasValue && itemsIds.Contains(x.OrigAppItemId.Value)
                && x.AppItem.Application.IsExchangeRequest && x.AppItem.Application.Status == nameof(ApplicationStatusEnum.Approved)
                        && x.AppItem.BasicDocument.HasFactoryNumber)
                .Select(x => new
                {
                    x.AppItemId,
                    x.DocNumber,
                    x.Edition,
                    x.Series,
                    x.OrigAppItemId
                })
                .ToListAsync(cancellationToken);

            if (oridItems.Count > 0)
            {
                foreach (var doc in model.ApplicationDocuments)
                {
                    var docOrigItems = oridItems.Where(x => x.OrigAppItemId == doc.Id).ToList();
                    if (docOrigItems.Count > 0)
                    {
                        doc.DeliveredItems.AddRange(docOrigItems.Select(d => new DocManagementApplicationDeliveredDocModel
                        {
                            DocNumber = d.DocNumber,
                            Edition = d.Edition,
                            Series = d.Series,
                        }));
                    }
                }
            }

            return model;
        }

        private class ReportDataModel
        {
            public short SchoolYear { get; set; }
            public string SchoolYearName { get; set; }
            public int InstitutionId { get; set; }
            public string InstitutionName { get; set; }
            public string InstitutionTown { get; set; }
            public string InstitutionMunicipality { get; set; }
            public string InstitutionRegion { get; set; }
            public List<DocManagementApplicationBasicDocumentModel> ApplicationDocuments { get; set; }
            public List<DocManagementApplicationBasicDocumentModel> RequestedDocuments { get; set; }
            public List<DocManagementApplicationBasicDocumentModel> TransferredDocuments { get; set; }
            public List<DocManagementApplicationBasicDocumentModel> PrevPeriodsDocuments { get; set; }
        }
    }
}
