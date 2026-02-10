

namespace MON.Services.Implementations
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.DocManagement;
    using MON.Models.Grid;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;
    using System.Threading;
    using MON.Shared.ErrorHandling;
    using MON.Services.Security.Permissions;
    using System.Linq;
    using MON.Shared;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq.Dynamic.Core;
    using MON.Models;
    using MON.DataAccess;
    using MON.Shared.Enums;
    using MON.Services.Interfaces;
    using System;
    using Z.EntityFramework.Plus;
    using MON.Models.Configuration;
    using Microsoft.Extensions.Options;
    using DocumentFormat.OpenXml.Office2010.Excel;
    using DocumentFormat.OpenXml.Bibliography;
    using Domain;
    using System.IO;
    using System.Text.Json;
    using MON.Services.Utils;

    public class DocManagementExchangeService : BaseService<DocManagementExchangeService>
    {
        private readonly DocManagementApplicationService _docManagementApplicationService;
        private readonly IInstitutionService _institutionService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IWordTemplateService _wordTemplateService;


        public DocManagementExchangeService(DbServiceDependencies<DocManagementExchangeService> dependencies, 
            DocManagementApplicationService docManagementApplicationService,
            IInstitutionService institutionService,
            IBlobService blobService,
            IWordTemplateService wordTemplateService,
            IOptions<BlobServiceConfig> blobServiceConfig) : base(dependencies)
        {
            _docManagementApplicationService = docManagementApplicationService;
            _institutionService = institutionService;
            _blobService = blobService;
            _wordTemplateService = wordTemplateService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        #region Private members
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
       
        private class ApplicationDataModel
        {
            public short SchoolYear { get; set; }
            public int InstitutionId { get; set; }
            public string InstitutionName { get; set; }
            public int? RequestedInstitutionId { get; set; }
            public string RequestedInstitutionName { get; set; }
            public string RequestedInstitutionTown { get; set; }
            public string RequestedInstitutionMunicipality { get; set; }
            public string RequestedInstitutionRegion { get; set; }
            public List<DocManagementApplicationBasicDocumentModel> BasicDocuments { get; set; }
        }

        private async Task<ApplicationDataModel> GetApplicationData(int? applicationId, CancellationToken cancellationToken)
        {
            var result = await _context.DocManagementApplications
                .Where(x => x.Id == applicationId && x.IsExchangeRequest)
                .Select(x => new
                {
                    x.SchoolYear,
                    x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    x.RequestedInstitutionId,
                    RequestedInstitutionName = x.InstitutionSchoolYearNavigation.Name,
                    RequestedInstitutionTown = x.InstitutionSchoolYearNavigation.Town.Name,
                    RequestedInstitutionMunicipality = x.InstitutionSchoolYearNavigation.Town.Municipality.Name,
                    RequestedInstitutionRegion = x.InstitutionSchoolYearNavigation.Town.Municipality.Region.Name,
                    BasicDocuments = x.DocManagementApplicationItems
                        .Where(d => d.BasicDocument.HasFactoryNumber)
                        .Select(i => new DocManagementApplicationBasicDocumentModel
                        {
                            Id = i.Id,
                            BasicDocumentId = i.BasicDocumentId,
                            BasicDocumentName = i.BasicDocument.Name,
                            IsDuplicate = i.BasicDocument.IsDuplicate,
                            HasFactoryNumber = i.BasicDocument.HasFactoryNumber,
                            SeriesFormat = i.BasicDocument.SeriesFormat,
                            Number = i.Number,
                            SchoolYear = x.SchoolYear,
                            DeliveredCount = i.DeliveredCount,
                            DeliveredItems = i.DocManagementApplicationItemDeliveredDocAppItems
                                .Select(d => new DocManagementApplicationDeliveredDocModel
                                {
                                    DocNumber = d.DocNumber,
                                    Edition = d.Edition,
                                    Series = d.Series,
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException(Messages.EmptyEntityError);

            return new ApplicationDataModel
            {
                SchoolYear = result.SchoolYear,
                InstitutionId = result.InstitutionId,
                InstitutionName = result.InstitutionName,
                RequestedInstitutionId = result.RequestedInstitutionId,
                RequestedInstitutionName = result.RequestedInstitutionName,
                RequestedInstitutionTown = result.RequestedInstitutionTown,
                RequestedInstitutionMunicipality = result.RequestedInstitutionMunicipality,
                RequestedInstitutionRegion = result.RequestedInstitutionRegion,
                BasicDocuments = result.BasicDocuments
            };
        }

        private DocManagementRequestReportModel CreateReportModel(ApplicationDataModel application)
        {
            return new DocManagementRequestReportModel
            {
                today = $"{DateTime.Now: dd.MM.yyyy} г.",
                institutionName = application.InstitutionName ?? "",
                institutionRepresentative = "",
                requestedInstitutionName = application.RequestedInstitutionName ?? "",
                requestedInstitutionTown = application.RequestedInstitutionTown ?? "",
                requestedInstitutionMunicipality = application.RequestedInstitutionMunicipality ?? "",
                requestedInstitutionRegion = application.RequestedInstitutionRegion ?? "",
                requestedInstitutionRepresentative = "",
                documents = DocManagementUtils.ProcessDocumentGroups(application.BasicDocuments).ToList()
            };
        }

        #endregion

        public async Task<IPagedList<DocManagementFreeDocListModel>> ListFree([FromQuery] DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var query = from docs in _context.VFreeDocManagementApplicationItems
                        join inst in _context.Institutions on docs.InstitutionId equals inst.InstitutionId
                        join town in _context.Towns on inst.TownId equals town.TownId
                        join mun in _context.Municipalities on town.MunicipalityId equals mun.MunicipalityId
                        select new { docs, inst, mun };

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.docs.InstitutionId == input.InstitutionId.Value);
            }

            if (input.RegionId.HasValue)
            {
                query = query.Where(x => x.mun.RegionId == input.RegionId.Value);
            }

            if (input.MunicipalityId.HasValue)
            {
                query = query.Where(x => x.mun.MunicipalityId == input.MunicipalityId.Value);
            }

            if (input.TownId.HasValue)
            {
                query = query.Where(x => x.inst.TownId == input.TownId.Value);
            }

            if (input.BasicDocumentId.HasValue)
            {
                query = query.Where(x => x.docs.BasicDocumentId == input.BasicDocumentId.Value);
            }

            IQueryable<DocManagementFreeDocListModel> listQuery = query
                 .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.docs.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.inst.Abbreviation.Contains(input.Filter)
                   || predicate.inst.Name.Contains(input.Filter)
                   || predicate.docs.BasicDocumentName.Contains(input.Filter))
                .Select(x => new DocManagementFreeDocListModel
                {
                    BasicDocumentId = x.docs.BasicDocumentId,
                    BasicDocumentName = x.docs.BasicDocumentName,
                    InstitutionId = x.docs.InstitutionId,
                    InstitutionName = x.inst.Abbreviation,
                    //FreeDocNumbers = x.docs.FreeDocNumbers,
                    FreeDocCount = x.docs.FreeDocCount,
                    //UsedDocCount = x.docs.UsedDocCount
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "BasicDocumentId desc, InstitutionId asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<DocManagementFreeDocListModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<IEnumerable<DocManagementFreeDocExchageListModel>> GetFreeForExchange([FromQuery] DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<DropdownViewModel> currentInstBasicDocuments = await _docManagementApplicationService.GetBasicDocuments(cancellationToken);
            HashSet<int> basicDocumentsLimit = currentInstBasicDocuments.Select(x => x.Value).ToHashSet();

            var query = _context.VFreeDocManagementApplicationItems
                .Where(x => basicDocumentsLimit.Contains(x.BasicDocumentId));

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (input.BasicDocumentId.HasValue)
            {
                query = query.Where(x => x.BasicDocumentId == input.BasicDocumentId.Value);
            }

            return await query
                .OrderByDescending(x => x.BasicDocumentId)
                .ThenBy(x => x.InstitutionId)
                .Select(x => new DocManagementFreeDocExchageListModel
                {
                    BasicDocumentId = x.BasicDocumentId,
                    BasicDocumentName = x.BasicDocumentName,
                    FreeDocCount = x.FreeDocCount,
                })
                .ToListAsync(cancellationToken);
        }

        public async Task CreateRequest(DocManagementExchangeRequestModel model)
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

            if (await _context.DocManagementApplications.AnyAsync(x => x.CampaignId == model.CampaignId && x.InstitutionId == institutionId))
            {
                throw new ApiException(Messages.DocManagementApplicationAlreadyExits);
            }

            short schoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);
            DocManagementApplication entry = new DocManagementApplication
            {
                Campaign = new DocManagementCampaign
                {
                    SchoolYear = schoolYear,
                    FromDate = DateTime.Now.Date,
                    ToDate = DateTime.Now.AddDays(1).Date,
                    IsHidden = true,
                },
                IsExchangeRequest = true,
                InstitutionId = _userInfo.InstitutionID ?? throw new ArgumentNullException(nameof(_userInfo.InstitutionID)),
                RequestedInstitutionId = model.InstitutionId,
                SchoolYear = schoolYear,
                Status = nameof(ApplicationStatusEnum.Submitted),
                DocManagementApplicationItems = model.BasicDocuments
                .Where(x => x.Number.HasValue && x.Number.Value > 0)
                .Select(x => new DocManagementApplicationItem
                {
                    BasicDocumentId = x.BasicDocumentId,
                    Number = x.Number.Value,
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

        public async Task DeleteRequest(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementApplicationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementApplication entity = await _context.DocManagementApplications
                .Include(x => x.DocManagementApplicationAttachments)
                .Include(x => x.Campaign)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsExchangeRequest) ?? throw new ApiException(Messages.EmptyEntityError);

            int institutionId = _userInfo.InstitutionID ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);
            if (entity.InstitutionId != institutionId)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

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
            _context.DocManagementCampaigns.Remove(entity.Campaign);

            await SaveAsync(cancellationToken);
        }

        public async Task ApproveRequest(DocManagementExchangeRequestApproveModel model, CancellationToken cancellationToken)
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
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId && x.IsExchangeRequest, cancellationToken) ?? throw new ApiException(Messages.EmptyEntityError);

            if (!entity.RequestedInstitutionId.HasValue || entity.RequestedInstitutionId != _userInfo.InstitutionID
                || entity.Status != nameof(ApplicationStatusEnum.Submitted))
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

        public async Task RejectRequest(DocManagementExchangeRequestRejectModel model, CancellationToken cancellationToken)
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
                .FirstOrDefaultAsync(x => x.Id == model.ApplicationId && x.IsExchangeRequest, cancellationToken) ?? throw new ApiException(Messages.EmptyEntityError);

            if (!entity.RequestedInstitutionId.HasValue || entity.RequestedInstitutionId != _userInfo.InstitutionID
                || entity.Status != nameof(ApplicationStatusEnum.Submitted))
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

        public async Task<(string fileName, byte[] fileContents, string contentType)> GenerateProtocol(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementReportCreate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var application = await GetApplicationData(model.ApplicationId, cancellationToken);
            DocManagementRequestReportModel reportModel = CreateReportModel(application);

            return await _wordTemplateService.GenerateWordDocument(reportModel, "Protocol_ZUD_Request", cancellationToken);
        }
    }
}
