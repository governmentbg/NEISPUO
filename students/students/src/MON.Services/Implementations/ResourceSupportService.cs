namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Enums;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.ResourceSupport;
    using MON.Models.StudentModels.Update;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class ResourceSupportService : BaseService<ResourceSupportService>, IResourceSupportService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly ISignalRNotificationService _signalRNotificationService;

        public ResourceSupportService(DbServiceDependencies<ResourceSupportService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            ILodFinalizationService lodFinalizationService,
            ISignalRNotificationService signalRNotificationService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _lodFinalizationService = lodFinalizationService;
            _signalRNotificationService = signalRNotificationService;
        }

        public async Task<StudentResourceSupportModel> GetById(int id)
        {
            StudentResourceSupportModel model = await _context.ResourceSupportReports
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new StudentResourceSupportModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    ReportNumber = x.ReportNumber,
                    ReportDate = x.ReportDate,
                    Documents = x.ResourceSupportDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    ResourceSupports = x.ResourceSupports.Select(rs => new ResourceSupportModel
                    {
                        Id = rs.Id,
                        ResourceSupportTypeId = rs.ResourceSupportTypeId,
                        ResourceSupportTypeName = rs.ResourceSupportType.Name,
                        ResourceSupportReportId = rs.ResourceSupportReportId,
                        ResourceSupportSpecialists = rs.ResourceSupportSpecialists.Select(rss => new ResourceSupportSpecialistModel
                        {
                            Id = rss.Id,
                            Name = rss.Name,
                            OrganizationName = rss.OrganizationName,
                            OrganizationType = rss.OrganizationType,
                            SpecialistType = rss.SpecialistType,
                            ResourceSupportSpecialistTypeId = rss.ResourceSupportSpecialistTypeId,
                            WorkPlaceId = rss.WorkPlaceId,
                            ResourceSupportSpecialistTypeName = rss.ResourceSupportSpecialistType.Name,
                            WorkPlaceName = rss.WorkPlace.Name,
                        }).ToList()
                    }).ToList()
                })
                .SingleOrDefaultAsync();

            if (model != null)
            {
                // Методът се използва при Details и Edit
                if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentResourceSupportRead)
                    && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentResourceSupportManage))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            return model;
        }

        public async Task<List<StudentResourceSupportViewModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentResourceSupportRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await (from x in _context.ResourceSupportReports
                          join lf in _context.Lodfinalizations on new { x.PersonId, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                          from lodFin in temp.DefaultIfEmpty()
                          where x.PersonId == personId
                          orderby x.SchoolYear descending
                          select new StudentResourceSupportViewModel
                          {
                              Id = x.Id,
                              PersonId = x.PersonId,
                              SchoolYear = x.SchoolYear,
                              SchoolYearName = x.SchoolYearNavigation.Name,
                              ReportNumber = x.ReportNumber,
                              ReportDate = x.ReportDate,
                              Documents = x.ResourceSupportDocuments
                                  .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                              IsLodFinalized = lodFin != null && lodFin.IsFinalized,
                              RelatedAdditionalPersonalDevelopmentSupportId = x.ResourceSupports
                                .Where(r => r.AdditionalPersonalDevelopmentSupportItemId.HasValue)
                                .Select(r => r.AdditionalPersonalDevelopmentSupportItem.AdditionalPersonalDevelopmentSupportId)
                                .FirstOrDefault()
                          })
                            .ToListAsync();
        }

        public async Task<StudentResourceSupportViewModel> ChechForExistingByPerson(int personId, int schoolYear)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentResourceSupportRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.ResourceSupportReports
                .AsNoTracking()
                .Where(x => x.PersonId == personId && x.SchoolYear == schoolYear)
                .Select(x => new StudentResourceSupportViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    ReportNumber = x.ReportNumber,
                    ReportDate = x.ReportDate,
                })
                .FirstOrDefaultAsync();
        }

        public async Task Create(StudentResourceSupportModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentResourceSupportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ResourceSupportReport entry = new ResourceSupportReport
            {
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear,
                ReportNumber = model.ReportNumber,
                ReportDate = model.ReportDate,
            };

            if (model.ResourceSupports != null && model.ResourceSupports.Count > 0)
            {
                foreach (ResourceSupportModel rs in model.ResourceSupports)
                {
                    entry.ResourceSupports.Add(new ResourceSupport
                    {
                        ResourceSupportTypeId = rs.ResourceSupportTypeId,
                        ResourceSupportSpecialists = rs.ResourceSupportSpecialists?.Select(x => new ResourceSupportSpecialist
                        {
                            Name = x.Name,
                            OrganizationName = x.OrganizationName,
                            OrganizationType = x.OrganizationType,
                            SpecialistType = x.SpecialistType,
                            ResourceSupportSpecialistTypeId = x.ResourceSupportSpecialistTypeId,
                            WorkPlaceId = x.WorkPlaceId
                        }).ToList()
                    });
                }
            }

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entry);
            _context.ResourceSupportReports.Add(entry);

            await SaveAsync();
            await transaction.CommitAsync();
            await _signalRNotificationService.ResourceSupportModified(entry.PersonId, entry.Id);
        }

        public async Task Update(StudentResourceSupportModel model)
        {
            ResourceSupportReport entity = await _context.ResourceSupportReports
                .Include(x => x.ResourceSupports).ThenInclude(x => x.ResourceSupportSpecialists)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentResourceSupportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.SchoolYear = model.SchoolYear;
            entity.ReportNumber = model.ReportNumber;
            entity.ReportDate = model.ReportDate;

            HashSet<int> existedIds = model.ResourceSupports != null
                ? model.ResourceSupports.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            IEnumerable<ResourceSupport> resourceSupportsToDelete = entity.ResourceSupports.Where(x => !existedIds.Contains(x.Id));

            IEnumerable<ResourceSupportModel> resourceSupportsToAdd = null;
            if (model.ResourceSupports != null)
            {
                resourceSupportsToAdd = model.ResourceSupports.Where(x => !x.Id.HasValue);
            }

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            if (resourceSupportsToDelete != null && resourceSupportsToDelete.Any())
            {
                // Изтриване на ResourceSupports
                foreach (var toDelete in resourceSupportsToDelete)
                {
                    if (toDelete.ResourceSupportSpecialists.Any())
                    {
                        _context.ResourceSupportSpecialists.RemoveRange(toDelete.ResourceSupportSpecialists);
                    }
                }
                _context.ResourceSupports.RemoveRange(resourceSupportsToDelete);
            }

            if (resourceSupportsToAdd != null && resourceSupportsToAdd.Any())
            {
                // Добавяне на ResourceSupports
                _context.ResourceSupports.AddRange(resourceSupportsToAdd.Select(rs => new ResourceSupport
                {
                    ResourceSupportTypeId = rs.ResourceSupportTypeId,
                    ResourceSupportReportId = entity.Id,
                    ResourceSupportSpecialists = rs.ResourceSupportSpecialists?.Select(x => new ResourceSupportSpecialist
                    {
                        Name = x.Name,
                        OrganizationName = x.OrganizationName,
                        OrganizationType = x.OrganizationType,
                        SpecialistType = x.SpecialistType,
                        ResourceSupportSpecialistTypeId = x.ResourceSupportSpecialistTypeId,
                        WorkPlaceId = x.WorkPlaceId,
                    }).ToList()
                }));
            }

            if (existedIds.Any())
            {
                // Редакция на ResourceSupports
                foreach (var rsToUpdate in entity.ResourceSupports.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.ResourceSupports.Where(x => x.Id.HasValue && x.Id.Value == rsToUpdate.Id).SingleOrDefault();
                    if (source == null) continue;

                    rsToUpdate.ResourceSupportTypeId = source.ResourceSupportTypeId;

                    HashSet<int> existedRssIds = source.ResourceSupportSpecialists != null
                        ? source.ResourceSupportSpecialists.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                        : new HashSet<int>();

                    IEnumerable<ResourceSupportSpecialist> rssToDelete = rsToUpdate.ResourceSupportSpecialists.Where(x => !existedRssIds.Contains(x.Id));

                    IEnumerable<ResourceSupportSpecialistModel> rssToAdd = null;
                    if (source.ResourceSupportSpecialists != null)
                    {
                        rssToAdd = source.ResourceSupportSpecialists.Where(x => !x.Id.HasValue);
                    }

                    if (rssToDelete != null && rssToDelete.Any())
                    {
                        // Изтриване на ResourceSupportSpecialists
                        _context.ResourceSupportSpecialists.RemoveRange(rssToDelete);
                    }

                    if (rssToAdd != null && rssToAdd.Any())
                    {
                        // Добавяне на ResourceSupportSpecialists
                        _context.ResourceSupportSpecialists.AddRange(rssToAdd.Select(x => new ResourceSupportSpecialist
                        {
                            Name = x.Name,
                            OrganizationName = x.OrganizationName,
                            OrganizationType = x.OrganizationType,
                            SpecialistType = x.SpecialistType,
                            ResourceSupportSpecialistTypeId = x.ResourceSupportSpecialistTypeId,
                            WorkPlaceId = x.WorkPlaceId,
                            ResourceSupportId = rsToUpdate.Id
                        }));
                    }

                    if (existedRssIds.Any())
                    {
                        // Редакция на ResourceSupportSpecialists
                        foreach (var rssToUpdate in rsToUpdate.ResourceSupportSpecialists.Where(x => existedRssIds.Contains(x.Id)))
                        {
                            var rssSource = source.ResourceSupportSpecialists.Where(x => x.Id.HasValue && x.Id.Value == rssToUpdate.Id).SingleOrDefault();
                            if (rssSource == null) continue;

                            rssToUpdate.Name = rssSource.Name;
                            rssToUpdate.OrganizationName = rssSource.OrganizationName;
                            rssToUpdate.OrganizationType = rssSource.OrganizationType;
                            rssToUpdate.SpecialistType = rssSource.SpecialistType;
                            rssToUpdate.ResourceSupportSpecialistTypeId = rssSource.ResourceSupportSpecialistTypeId;
                            rssToUpdate.WorkPlaceId = rssSource.WorkPlaceId;
                        }
                    }
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();
            await _signalRNotificationService.ResourceSupportModified(entity.PersonId, entity.Id);
        }

        public async Task Delete(int id)
        {
            ResourceSupportReport entity = await _context.ResourceSupportReports
                .Include(x => x.ResourceSupports)
                .Include(x => x.ResourceSupportDocuments)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentResourceSupportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.ResourceSupports.Any(x => x.AdditionalPersonalDevelopmentSupportItemId.HasValue))
            {
                throw new ApiException($"Не е позволено изтриването на данни за РП, свързани със запис за ДПЛР за този ученик и учебна година {entity.SchoolYear}. Изтриването на данни за РП се извършва от ЛОД, меню ПЛР, подменю ДПЛР.", 500);
            }

            if (entity.ResourceSupportDocuments != null && entity.ResourceSupportDocuments.Any())
            {
                var docsIds = entity.ResourceSupportDocuments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.ResourceSupportDocuments.RemoveRange(entity.ResourceSupportDocuments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            if (entity.ResourceSupports != null)
            {
                _context.ResourceSupports.RemoveRange(entity.ResourceSupports);
            }

            _context.ResourceSupportReports.Remove(entity);

            await SaveAsync();
            await _signalRNotificationService.ResourceSupportModified(entity.PersonId, entity.Id);
        }

        public async Task<ResourceSupportViewModel> GetStudentResourceSupport(int personId, int? schoolYear)
        {
            if (!schoolYear.HasValue)
            {
                throw new ArgumentNullException(nameof(schoolYear), "School year cannot be null!");
            }

            // Методът се използва при Details и Edit
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentResourceSupportRead)
                && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentResourceSupportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var resourceSupport = await _context.People.AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new ResourceSupportViewModel
                {
                    ResourceSupportReports = x.ResourceSupportReports.Select(x => new ResourceSupportReportModel
                    {
                        Id = x.Id,
                        PersonId = x.PersonId,
                        ReportDate = x.ReportDate,
                        ReportNumber = x.ReportNumber,
                        SysUserID = x.SysUserId,
                        SchoolYear = x.SchoolYear,
                        ResourceSupportDetails = x.ResourceSupports.Select(rs => new ResourceSupportModel
                        {
                            Id = rs.Id,
                            ResourceSupportReportId = rs.ResourceSupportReportId,
                            ResourceSupportTypeId = rs.ResourceSupportTypeId,
                            ResourceSupportSpecialists = rs.ResourceSupportSpecialists.Select(rss => new ResourceSupportSpecialistModel
                            {
                                Id = rss.Id,
                                Name = rss.Name,
                                OrganizationName = rss.OrganizationName,
                                ResourceSupportId = rss.ResourceSupportId,
                                ResourceSupportSpecialistTypeId = rss.ResourceSupportSpecialistTypeId,
                                WorkPlaceId = rss.WorkPlaceId,
                                OrganizationType = rss.OrganizationType,
                                SpecialistType = rss.SpecialistType,
                                SysUserID = rss.SysUserId
                            }).ToList(),
                        }).ToList(),
                        ResourceSupportDocuments = x.ResourceSupportDocuments.Select(rsDocument => new ResourceSupportDocumentModel
                        {
                            Id = rsDocument.Id,
                            ResourceSupportReportId = x.Id,
                            Description = rsDocument.Description,
                            Document = rsDocument.Document.ToViewModel(_blobServiceConfig)
                        }).ToList()
                    }).ToList()
                }).SingleOrDefaultAsync();

            resourceSupport.ResourceSupportReportsPreviousYears = resourceSupport.ResourceSupportReports.Where(x => x.SchoolYear != schoolYear).ToList();
            resourceSupport.ResourceSupportReports = resourceSupport.ResourceSupportReports.Where(x => x.SchoolYear == schoolYear).ToList();

            return resourceSupport;
        }

        public async Task UpdateStudentResourceSupport(StudentResourceSupportUpdateModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentResourceSupportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            using var transaction = _context.Database.BeginTransaction();

            await ProcessResourceSupportUpdateAsync(model);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        #region Private members
        private async Task ProcessResourceSupportUpdateAsync(StudentResourceSupportUpdateModel model)
        {
            var currentYear = model.SchoolYear ?? DateTime.Now.CurrentSchoolYear();

            var person = await _context.People
              .SingleAsync(x => x.PersonId == model.PersonId);

            var resourceSupportReports = _context.ResourceSupportReports.Where(x => x.PersonId == person.PersonId && x.SchoolYear == currentYear)
                                                 .Include(c => c.ResourceSupportDocuments).Include(x => x.ResourceSupports).ThenInclude(x => x.ResourceSupportSpecialists);

            await DeleteReportsAsync(model, resourceSupportReports);

            foreach (var resourceSupportReport in model.ResourceSupportReports)
            {
                if (resourceSupportReport.Id <= 0)
                {
                    AddNewResourceSupportReport(person, resourceSupportReport, model.SchoolYear.Value);
                }
                else
                {
                    await UpdateResourceSupportReport(resourceSupportReport, resourceSupportReports);
                }
            }
        }

        private async Task UpdateResourceSupportReport(ResourceSupportReportModel resourceSupportReport, IIncludableQueryable<ResourceSupportReport, ICollection<ResourceSupportSpecialist>> resourceSupportReports)
        {
            var resourceSupportReportToUpdate = resourceSupportReports.Single(x => x.Id == resourceSupportReport.Id);
            resourceSupportReportToUpdate.ReportNumber = resourceSupportReport.ReportNumber;
            resourceSupportReportToUpdate.ReportDate = resourceSupportReport.ReportDate;
            resourceSupportReportToUpdate.SysUserId = resourceSupportReport.SysUserID;

            AddUploadedDocuments(resourceSupportReport.ResourceSupportDocuments.Where(x => x.Id < 0), resourceSupportReportToUpdate);
            await UpdateUploadedDocuments(resourceSupportReport.ResourceSupportDocuments.Where(x => x.Id > 0));

            if (resourceSupportReport.ResourceSupportDetails.Count == 0)
            {
                var rssToRemove = _context.ResourceSupports.Where(x => x.ResourceSupportReportId == resourceSupportReport.Id).Include(x => x.ResourceSupportSpecialists);
                await rssToRemove.ForEachAsync(x => _context.ResourceSupportSpecialists.RemoveRange(x.ResourceSupportSpecialists));
                _context.ResourceSupports.RemoveRange(rssToRemove);
            }
            else
            {
                var selectedSupportsIdsHash = resourceSupportReport.ResourceSupportDetails.Select(x => x.Id).ToHashSet();

                var supportsTodelete = _context.ResourceSupports.Where(x => false == selectedSupportsIdsHash.Contains(x.Id)).Include(x => x.ResourceSupportSpecialists);

                await supportsTodelete.ForEachAsync(x => _context.ResourceSupportSpecialists.RemoveRange(x.ResourceSupportSpecialists));

                _context.ResourceSupports.RemoveRange(supportsTodelete);
            }

            foreach (var resourceSupport in resourceSupportReport.ResourceSupportDetails)
            {
                var resourceSupportToUpdate = _context.ResourceSupports.Include(x => x.ResourceSupportSpecialists).SingleOrDefault(x => x.Id == resourceSupport.Id);

                if (resourceSupportToUpdate != null)
                {
                    resourceSupportToUpdate.ResourceSupportReportId = resourceSupport.ResourceSupportReportId;
                    resourceSupportToUpdate.ResourceSupportTypeId = resourceSupport.ResourceSupportTypeId;

                    foreach (var rssToDelete in resourceSupportToUpdate.ResourceSupportSpecialists)
                    {
                        if (resourceSupport.ResourceSupportSpecialists.Find(x => x.Id == rssToDelete.Id) == null)
                        {
                            _context.ResourceSupportSpecialists.Remove(rssToDelete);
                        }
                    }

                    foreach (var resourceSupportSpecialist in resourceSupport.ResourceSupportSpecialists)
                    {
                        if (resourceSupportSpecialist.Id == 0)
                        {
                            resourceSupportToUpdate.ResourceSupportSpecialists.Add(new ResourceSupportSpecialist
                            {
                                Name = resourceSupportSpecialist.Name,
                                OrganizationName = resourceSupportSpecialist.OrganizationName,
                                ResourceSupportId = resourceSupportSpecialist.ResourceSupportId,
                                ResourceSupportSpecialistTypeId = resourceSupportSpecialist.ResourceSupportSpecialistTypeId,
                                SysUserId = resourceSupportSpecialist.SysUserID,
                                OrganizationType = resourceSupportSpecialist.OrganizationType,
                                SpecialistType = resourceSupportSpecialist.SpecialistType,
                                WorkPlaceId = resourceSupportSpecialist.WorkPlaceId
                            });
                        }
                        else
                        {
                            var resourceSupportSpecialistToUpdate = _context.ResourceSupportSpecialists.Single(x => x.Id == resourceSupportSpecialist.Id);
                            resourceSupportSpecialistToUpdate.Name = resourceSupportSpecialist.Name;
                            resourceSupportSpecialistToUpdate.OrganizationName = resourceSupportSpecialist.OrganizationName;
                            resourceSupportSpecialistToUpdate.OrganizationType = resourceSupportSpecialist.OrganizationType;
                            resourceSupportSpecialistToUpdate.ResourceSupportSpecialistTypeId = resourceSupportSpecialist.ResourceSupportSpecialistTypeId;
                            resourceSupportSpecialistToUpdate.SpecialistType = resourceSupportSpecialist.SpecialistType;
                            resourceSupportSpecialistToUpdate.WorkPlaceId = resourceSupportSpecialist.WorkPlaceId;
                            resourceSupportSpecialistToUpdate.SysUserId = resourceSupportSpecialist.SysUserID;
                        }
                    }
                }
                else
                {
                    var newResourceSupport = new ResourceSupport
                    {
                        ResourceSupportReportId = resourceSupport.ResourceSupportReportId,
                        ResourceSupportTypeId = resourceSupport.ResourceSupportTypeId,
                        ResourceSupportSpecialists = resourceSupport.ResourceSupportSpecialists.Select(rss => new ResourceSupportSpecialist
                        {
                            Name = rss.Name,
                            OrganizationName = rss.OrganizationName,
                            OrganizationType = rss.OrganizationType,
                            SpecialistType = rss.SpecialistType,
                            SysUserId = rss.SysUserID,
                            ResourceSupportSpecialistTypeId = rss.ResourceSupportSpecialistTypeId,
                            WorkPlaceId = rss.WorkPlaceId
                        }).ToHashSet()
                    };

                    _context.ResourceSupports.Add(newResourceSupport);
                }
            }
        }

        private void AddNewResourceSupportReport(Person person, ResourceSupportReportModel resourceSupportReport, short schoolYear)
        {
            var reportToAdd = new ResourceSupportReport
            {
                PersonId = person.PersonId,
                ReportDate = resourceSupportReport.ReportDate,
                ReportNumber = resourceSupportReport.ReportNumber,
                SchoolYear = schoolYear,
                SysUserId = resourceSupportReport.SysUserID
            };

            resourceSupportReport.ResourceSupportDetails.ForEach(x => reportToAdd.ResourceSupports.Add(new ResourceSupport
            {
                ResourceSupportTypeId = x.ResourceSupportTypeId,
                ResourceSupportSpecialists = x.ResourceSupportSpecialists.Select(rss => new ResourceSupportSpecialist
                {
                    Name = rss.Name,
                    OrganizationName = rss.OrganizationName,
                    OrganizationType = rss.OrganizationType,
                    SpecialistType = rss.SpecialistType,
                    SysUserId = rss.SysUserID,
                    ResourceSupportSpecialistTypeId = rss.ResourceSupportSpecialistTypeId,
                    WorkPlaceId = rss.WorkPlaceId
                }).ToHashSet()
            }));

            AddUploadedDocuments(resourceSupportReport.ResourceSupportDocuments, reportToAdd);

            person.ResourceSupportReports.Add(reportToAdd);
        }

        private async Task DeleteReportsAsync(StudentResourceSupportUpdateModel model, IIncludableQueryable<ResourceSupportReport, ICollection<ResourceSupportSpecialist>> resourceSupportReports)
        {
            if (model.ResourceSupportReports.Count == 0)
            {
                await resourceSupportReports.ForEachAsync(x =>
                {
                    foreach (var rs in x.ResourceSupports)
                    {
                        _context.ResourceSupportSpecialists.RemoveRange(rs.ResourceSupportSpecialists);
                    }

                    _context.ResourceSupports.RemoveRange(x.ResourceSupports);
                });
                await resourceSupportReports.ForEachAsync(x => _context.RemoveRange(x.ResourceSupportDocuments));

                _context.ResourceSupportReports.RemoveRange(resourceSupportReports);
            }
            else
            {
                var selectedIdsHash = model.ResourceSupportReports.Select(x => x.Id).ToHashSet();
                var toRemove = resourceSupportReports.Where(x => false == selectedIdsHash.Contains(x.Id));

                foreach (var resourceSupportReportToRemove in toRemove)
                {
                    _context.ResourceSupports.RemoveRange(resourceSupportReportToRemove.ResourceSupports);
                    _context.ResourceSupportDocuments.RemoveRange(resourceSupportReportToRemove.ResourceSupportDocuments);
                    _context.ResourceSupportReports.Remove(resourceSupportReportToRemove);
                }
            }
        }

        private Document UploadDocument(DocumentModel document)
        {
            var result = _blobService.UploadFileAsync(document.NoteContents, document.NoteFileName, document.NoteFileType).Result;
            var doc = new Document
            {
                ContentType = document.NoteFileType,
                FileName = document.NoteFileName,
                BlobId = result.Data?.BlobId
            };
            return doc;
        }

        private void AddUploadedDocuments(IEnumerable<ResourceSupportDocumentModel> resourceSupportDocuments, ResourceSupportReport resourceSupportReport)
        {
            foreach (var rsDocument in resourceSupportDocuments)
            {
                var newRssDocument = new ResourceSupportDocument
                {
                    Description = rsDocument.Description,
                    ResourceSupporReportId = rsDocument.ResourceSupportReportId,
                    Document = UploadDocument(rsDocument.Document),
                };

                resourceSupportReport.ResourceSupportDocuments.Add(newRssDocument);
            }
        }

        private async Task UpdateUploadedDocuments(IEnumerable<ResourceSupportDocumentModel> resourceSupportDocuments)
        {
            foreach (var rsDocument in resourceSupportDocuments)
            {
                var rsDocumentToUpdate = await _context.ResourceSupportDocuments
                    .Where(x => x.Id == rsDocument.Id)
                    .Include(x => x.Document)
                    .SingleAsync();

                rsDocumentToUpdate.Description = rsDocument.Description;

                if (rsDocument.Document.NoteContents != null && rsDocument.Document.NoteContents.Length > 0)
                {
                    if (rsDocument.Document.NoteFileType != rsDocumentToUpdate.Document.ContentType
                        || rsDocument.Document.NoteFileName != rsDocumentToUpdate.Document.FileName)
                    {
                        var document = UploadDocument(rsDocument.Document);
                        rsDocumentToUpdate.Document.BlobId = document.BlobId;
                        rsDocumentToUpdate.Document.FileName = document.FileName;
                        rsDocumentToUpdate.Document.ContentType = document.ContentType;
                    }
                }
            }
        }

        private async Task ProcessAddedDocs(StudentResourceSupportModel model, ResourceSupportReport entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null)
            {
                return;
            }

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.ResourceSupportDocuments.Add(new ResourceSupportDocument
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(StudentResourceSupportModel model, ResourceSupportReport sanction)
        {
            if (model.Documents == null || !model.Documents.Any() || sanction == null)
            {
                return;
            }

            HashSet<int> blobsToDelete = model.Documents
                .Where(x => x.Id.HasValue && x.Deleted == true && x.BlobId.HasValue)
                .Select(x => x.BlobId.Value).ToHashSet();

            if (blobsToDelete.Count > 0)
            {
                await _context.AdditionalPersonalDevelopmentSupportAttachments
                   .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.ResourceSupportDocuments
                    .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.Documents.Where(x => x.BlobId.HasValue && blobsToDelete.Contains(x.BlobId.Value)).DeleteAsync();
            }
        }
        #endregion

    }
}