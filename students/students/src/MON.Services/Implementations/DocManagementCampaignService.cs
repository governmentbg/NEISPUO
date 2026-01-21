using DocumentFormat.OpenXml.Bibliography;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.DocManagement;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class DocManagementCampaignService: BaseService<DocManagementCampaignService>
    {
        private readonly IInstitutionService _institutionService;
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly DocManagementApplicationService _docManagementApplicationService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;


        #region Private members
        private async Task SendNotification(DocManagementCampaign entity)
        {
            // Todo: Да се развие логиката за изпрашане на нотификации
            try
            {
                await _signalRNotificationService.DocManagementCampaignModified(entity.Id);
            }
            catch (Exception ex)
            {
                // Ignore
                _logger.LogError($"DocManagementCampaign notification failed:{entity.Id}", ex);
            }
        }

        private async Task ProcessAddedDocs(DocManagementCampaignAttachmentModel model, DocManagementCampaign entity, int? regionId)
        {
            if (model.Attachments == null || !model.Attachments.Any() || entity == null)
            {
                return;
            }

            foreach (DocumentModel docModel in model.Attachments.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                var doc = docModel.ToDocument(result?.Data?.BlobId);
                if (regionId.HasValue)
                {
                    doc.FileName = $"РУО_{regionId.Value}_{doc.FileName}";
                }
                entity.DocManagementCampaignAttachments.Add(new DocManagementCampaignAttachment
                {
                    RegionId = regionId,
                    Document = doc
                });
            }
        }

        private async Task ProcessDeletedDocs(DocManagementCampaignAttachmentModel model, DocManagementCampaign entity, int? regionId)
        {
            if (model.Attachments == null || !model.Attachments.Any() || entity == null)
            {
                return;
            }

            HashSet<int> blobsToDelete = model.Attachments
                .Where(x => x.Id.HasValue && x.Deleted == true && x.BlobId.HasValue)
                .Select(x => x.BlobId.Value).ToHashSet();

            if (blobsToDelete.Count > 0)
            {
                await _context.DocManagementCampaignAttachments
                    .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.Documents.Where(x => x.BlobId.HasValue && blobsToDelete.Contains(x.BlobId.Value)).DeleteAsync();
            }
        }
        #endregion

        public DocManagementCampaignService(DbServiceDependencies<DocManagementCampaignService> dependencies,
            IInstitutionService institutionService,
            ISignalRNotificationService signalRNotificationService,
            IWordTemplateService wordTemplateService,
            DocManagementApplicationService docManagementApplicationService,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _institutionService = institutionService;
            _signalRNotificationService = signalRNotificationService;;
            _docManagementApplicationService = docManagementApplicationService;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<IPagedList<DocManagementCampaignViewModel>> List(PagedListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int? institutionId = _userInfo.InstitutionID;
            bool isFileAdmin = _userInfo.IsConsortium || _userInfo.IsMon || _userInfo.IsMonExpert
               || _userInfo.IsMonHR || _userInfo.IsCIOO;
            bool isRuo = _userInfo.IsRuo || _userInfo.IsRuoExpert;

            IQueryable<DocManagementCampaignViewModel> query = _context.DocManagementCampaigns
                .AsNoTracking()
                .Where(x => x.ParentId == null && !x.IsHidden)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Name.Contains(input.Filter)
                   || predicate.Description.Contains(input.Filter)
                   || predicate.SchoolYearNavigation.Name.Contains(input.Filter)
                   || predicate.InstitutionSchoolYear.Name.Contains(input.Filter)
                   || predicate.InstitutionSchoolYear.Abbreviation.Contains(input.Filter))
                   .Select(x => new DocManagementCampaignViewModel
                   {
                       Id = x.Id,
                       ParentId = x.ParentId,
                       InstitutionId = x.InstitutionId,
                       Name = x.Name,
                       Description = x.Description,
                       SchoolYear = x.SchoolYear,
                       FromDate = x.FromDate,
                       ToDate = x.ToDate,
                       IsManuallyActivated = x.IsManuallyActivated,
                       SchoolYearName = x.SchoolYearNavigation.Name,
                       InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                       CreateDate = x.CreateDate,
                       Creator = x.CreatedBySysUser.Username,
                       ModifyDate = x.ModifyDate,
                       Updater = x.ModifiedBySysUser.Username,
                       HasApplication = institutionId.HasValue && x.DocManagementApplications.Any(a => a.InstitutionId == institutionId && a.SchoolYear == x.SchoolYear),
                       HasRuoAttachmentPermision = isFileAdmin || isRuo
                   })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, Id desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            List<DocManagementCampaignViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);

        }

        public async Task<DocManagementCampaignViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignRead)
               && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int? institutionId = _userInfo.InstitutionID;
            bool isAdminFileReader = _userInfo.IsConsortium || _userInfo.IsMon || _userInfo.IsMonExpert
                || _userInfo.IsMonHR || _userInfo.IsCIOO;
            bool isRuoFileReader = _userInfo.IsRuo || _userInfo.IsRuoExpert;

            return await _context.DocManagementCampaigns
                .Where(x => x.Id == id && !x.IsHidden)
                 .Select(x => new DocManagementCampaignViewModel
                 {
                     Id = x.Id,
                     ParentId = x.ParentId,
                     InstitutionId = x.InstitutionId,
                     Name = x.Name,
                     Description = x.Description,
                     SchoolYear = x.SchoolYear,
                     FromDate = x.FromDate,
                     ToDate = x.ToDate,
                     IsManuallyActivated = x.IsManuallyActivated,
                     SchoolYearName = x.SchoolYearNavigation.Name,
                     InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                     CreateDate = x.CreateDate,
                     Creator = x.CreatedBySysUser.Username,
                     ModifyDate = x.ModifyDate,
                     Updater = x.ModifiedBySysUser.Username,
                     HasApplication = institutionId.HasValue && x.DocManagementApplications.Any(a => a.InstitutionId == institutionId && a.SchoolYear == x.SchoolYear),
                     Attachments =  isAdminFileReader
                            ? x.DocManagementCampaignAttachments
                                .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                            : isRuoFileReader
                                ? x.DocManagementCampaignAttachments
                                    .Where(d => d.RegionId.HasValue && d.RegionId == _userInfo.RegionID)
                                    .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                                : Enumerable.Empty<DocumentViewModel>(),
                     HasRuoAttachmentPermision = isAdminFileReader || isRuoFileReader
                 })
                 .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<DocManagementCampaignViewModel>> GetActive(CancellationToken cancellationToken)
        {
            DateTime now = Now;

            return await _context.DocManagementCampaigns
                .AsNoTracking()
                .Where(x => !x.ParentId.HasValue && !x.IsHidden)
                .Where(x => x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1)))
                .OrderByDescending(x => x.SchoolYear)
                .ThenByDescending(x => x.Id)
                .Select(x => new DocManagementCampaignViewModel
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    InstitutionId = x.InstitutionId,
                    Name = x.Name,
                    Description = x.Description,
                    SchoolYear = x.SchoolYear,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    CreateDate = x.CreateDate,
                    Creator = x.CreatedBySysUser.Username,
                    ModifyDate = x.ModifyDate,
                    Updater = x.ModifiedBySysUser.Username,
                })
                .ToListAsync(cancellationToken);
        }

        public async Task Create(DocManagementCampaignModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (model.ToDate < model.FromDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }

            short currentYear = await _institutionService.GetCurrentYear(null);
            if (model.SchoolYear < currentYear)
            {
                throw new ApiException(Messages.PastSchoolYearError);
            }

            if (await _context.DocManagementCampaigns.AnyAsync(x => x.SchoolYear == currentYear && x.InstitutionId == null))
            {
                throw new ApiException(Messages.DocManagementCampaignAlreadyExits);
            }

            DocManagementCampaign entry = new DocManagementCampaign
            {
                Name = model.Name,
                Description = model.Description,
                SchoolYear = model.SchoolYear,
                ParentId = null,
                InstitutionId = null,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsManuallyActivated = model.IsManuallyActivated
            };

            _context.DocManagementCampaigns.Add(entry);

            await SaveAsync();
            await SendNotification(entry);

            //var payload = new { campaignId = entry.Id };

            //var sqlASPCampaignStartParameters = new[]
            //{
            //        new SqlParameter("@TaskTypeCode", "DocManagementCampaignStart"),          // string
            //        new SqlParameter("@Name", entry.Name),    // string
            //        new SqlParameter("@SchoolYear", entry.SchoolYear),               // int
            //        new SqlParameter("@ScheduledTime", DateTime.Now), // datetime
            //        new SqlParameter("@Payload", JsonConvert.SerializeObject(payload)) // string/json
            //    };

            //_ = await _context.Database.ExecuteSqlRawAsync("EXEC task.InsertScheduledTask  @TaskTypeCode, @Name, @SchoolYear, @ScheduledTime, @Payload", sqlASPCampaignStartParameters);
        }

        public async Task Update(DocManagementCampaignModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
              .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsHidden) ?? throw new ApiException(Messages.EmptyEntityError);

            if (model.ToDate < model.FromDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }

            short currentYear = await _institutionService.GetCurrentYear(null);
            if (model.SchoolYear < currentYear)
            {
                throw new ApiException(Messages.PastSchoolYearError);
            }

            if (await _context.DocManagementCampaigns.AnyAsync(x => x.Id != model.Id && x.SchoolYear == model.SchoolYear && x.InstitutionId == null))
            {
                throw new ApiException(Messages.DocManagementCampaignAlreadyExits);
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.SchoolYear = model.SchoolYear;
            entity.FromDate = model.FromDate;
            entity.ToDate = model.ToDate;
            entity.IsManuallyActivated = model.IsManuallyActivated;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsHidden, cancellationToken)
                ?? throw new ApiException(Messages.EmptyModelError);

            if (entity.FromDate <= Now || entity.IsManuallyActivated)
            {
                throw new ApiException("Кампанията е ръчно активирана или началната дата е настъпила");
            }

            _context.DocManagementCampaigns.Remove(entity);
            await SaveAsync();
        }

        public async Task ToggleManuallyActivation(DocManagementCampaignModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsHidden) ?? throw new ApiException(Messages.EmptyEntityError);

            entity.IsManuallyActivated = !entity.IsManuallyActivated;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task<List<DropdownViewModel>> GetDropdownOptions(CancellationToken cancellationToken)
        {
            return await _context.DocManagementCampaigns
                .Where(x => !x.ParentId.HasValue && !x.IsHidden)
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Text = $"{x.SchoolYearNavigation.Name} - {x.Name}"
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<List<DocumentViewModel>> GetAttachments(int campaignId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var query = _context.DocManagementCampaignAttachments
                .AsNoTracking()
                .Where(x => x.CampaignId == campaignId && !x.Campaign.IsHidden);

            bool isFileAdmin = _userInfo.IsConsortium || _userInfo.IsMon || _userInfo.IsMonExpert
               || _userInfo.IsMonHR || _userInfo.IsCIOO;
            bool isRuo = _userInfo.IsRuo || _userInfo.IsRuoExpert;

            if (isFileAdmin)
            {

            } else if (isRuo)
            {
                query = query.Where(x => x.RegionId.HasValue && x.RegionId == _userInfo.RegionID);
            } else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await query
                .Select(x => x.Document.ToViewModel(_blobServiceConfig))
                .ToListAsync(cancellationToken);
        }

        public async Task SaveAttachments(DocManagementCampaignAttachmentModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            bool isFileAdmin = _userInfo.IsConsortium || _userInfo.IsMon || _userInfo.IsMonExpert
               || _userInfo.IsMonHR || _userInfo.IsCIOO;
            bool isRuo = _userInfo.IsRuo || _userInfo.IsRuoExpert;

            if (!isFileAdmin && !isRuo)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
                .Include(x => x.DocManagementCampaignAttachments)
                .ThenInclude(x => x.Document)
                .FirstOrDefaultAsync(x => x.Id == model.CampaignId && !x.IsHidden)
                ?? throw new ApiException(Messages.EmptyEntityError);

            int? regionId = _userInfo.RegionID;

            await ProcessAddedDocs(model, entity, regionId);
            await ProcessDeletedDocs(model, entity, regionId);

            await SaveAsync();
        }

        public async Task<(string templateName, byte[] content, string contentType)> GenerateReport(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            return await _docManagementApplicationService.GenerateApplicationReport(model, cancellationToken);
        }

        public async Task DownloadAllAttachments(int campaignId, Stream outputStream, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementCampaignRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            bool isFileAdmin = _userInfo.IsConsortium || _userInfo.IsMon || _userInfo.IsMonExpert
               || _userInfo.IsMonHR || _userInfo.IsCIOO;
            bool isRuo = _userInfo.IsRuo || _userInfo.IsRuoExpert;

            if (!isFileAdmin && !isRuo)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var campaignAttachmentsQuery = _context.DocManagementCampaigns
                .Where(x => x.Id == campaignId)
                .SelectMany(x => x.DocManagementCampaignAttachments.Where(x => isFileAdmin || x.RegionId == _userInfo.RegionID))
                .Include(x => x.Document);

            var campaignApplicationAttachmentsQuery = _context.DocManagementApplications
                .Where(x => x.CampaignId == campaignId && x.Status == nameof(ApplicationStatusEnum.Approved))
                .Where(x => isFileAdmin || x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID)
                .SelectMany(x => x.DocManagementApplicationAttachments)
                .Include(x => x.Document);

            using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true);
            var usedNames = new HashSet<string>();

            // Campaign Attachments
            foreach (var attachment in await campaignAttachmentsQuery.ToListAsync(cancellationToken))
            {
                if (attachment.Document?.BlobId != null)
                {
                    if (isRuo && attachment.RegionId.HasValue && attachment.RegionId != _userInfo.RegionID)
                    {
                        continue;
                    }

                    var docViewModel = attachment.Document.ToViewModel(_blobServiceConfig);
                    var entryName = GetUniqueName($"Campaign/{attachment.Document.FileName}", usedNames);
                    await AddToArchive(archive, entryName, docViewModel, cancellationToken);
                }
            }

            // Application Attachments
            foreach (var attachment in await campaignApplicationAttachmentsQuery.ToListAsync(cancellationToken))
            {
                if (attachment.Document?.BlobId != null)
                {
                    var docViewModel = attachment.Document.ToViewModel(_blobServiceConfig);
                    var entryName = GetUniqueName($"Applications/{attachment.ApplicationId}/{attachment.Document.FileName}", usedNames);
                    await AddToArchive(archive, entryName, docViewModel, cancellationToken);
                }
            }
        }

        private string GetUniqueName(string name, HashSet<string> usedNames)
        {
            if (usedNames.Add(name))
            {
                return name;
            }

            var extension = Path.GetExtension(name);
            var fileName = Path.GetFileNameWithoutExtension(name);
            var directory = Path.GetDirectoryName(name);
            var counter = 1;

            while (true)
            {
                var newName = Path.Combine(directory, $"{fileName} ({counter}){extension}");
                if (usedNames.Add(newName))
                {
                    return newName;
                }
                counter++;
            }
        }

        private async Task AddToArchive(ZipArchive archive, string entryName, DocumentViewModel document, CancellationToken cancellationToken)
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
            using (var entryStream = entry.Open())
            {
                using (var blobStream = await _blobService.DownloadStreamAsync(document, cancellationToken))
                {
                    await blobStream.CopyToAsync(entryStream, cancellationToken);
                }
            }
        }
    }
}
