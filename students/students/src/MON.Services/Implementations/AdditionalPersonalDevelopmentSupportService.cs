namespace MON.Services.Implementations
{
    using Microsoft.Extensions.Options;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.StudentModels.PersonalDevelopmentSupport;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MON.Shared;
    using System;
    using MON.Services.Security.Permissions;
    using System.Collections.Generic;
    using System.Linq.Dynamic.Core;
    using MON.Shared.ErrorHandling;
    using MON.DataAccess;
    using Z.EntityFramework.Plus;
    using MON.Models.Enums;
    using MON.Models.StudentModels;
    using MON.DataAccess.Dto;
    using MON.Models.Grid;
    using MON.Models.StudentModels.Update;

    public class AdditionalPersonalDevelopmentSupportService : BaseService<AdditionalPersonalDevelopmentSupportService>, IAdditionalPersonalDevelopmentSupportService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly int[] NeedForResourceSupport = new[] {
            (int)AdditipnalPersonalDevelopmentSupportTypeEnum.HearingAndSpeechRehabilitation,
            (int)AdditipnalPersonalDevelopmentSupportTypeEnum.VisualRehabilitation,
            (int)AdditipnalPersonalDevelopmentSupportTypeEnum.ResourceSupport,
        };


        public AdditionalPersonalDevelopmentSupportService(DbServiceDependencies<AdditionalPersonalDevelopmentSupportService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _blobServiceConfig = blobServiceConfig.Value;
            _blobService = blobService;
        }

        #region Private members

        private async Task CheckPermission(string permission, int? personId)
        {
            if (!personId.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!await _authorizationService.HasPermissionForStudent(personId.Value, permission))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        /// <summary>
        /// Изтриване на ДПЛР.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="resourceSupportEntity"></param>
        private void Delete(AdditionalPersonalDevelopmentSupportModel model, AdditionalPersonalDevelopmentSupport entity, ResourceSupportReport resourceSupportEntity)
        {
            HashSet<int> existedIds = model.Items.Where(x => x.Id.HasValue && x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet();
            IEnumerable<AdditionalPersonalDevelopmentSupportItem> toDelete = entity.AdditionalPersonalDevelopmentSupportItems.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.IsNullOrEmpty())
            {
                return;
            }

            _context.AdditionalPersonalDevelopmentSupportItems.RemoveRange(toDelete);

            #region Изтриване на свързаното Ресурсно подпомагане

            if (resourceSupportEntity != null)
            {
                if (!model.Items.Any(x => !x.ResourceSupport.IsNullOrEmpty()))
                {
                    _context.ResourceSupportDocuments.RemoveRange(resourceSupportEntity.ResourceSupportDocuments);
                    _context.ResourceSupportSpecialists.RemoveRange(resourceSupportEntity.ResourceSupports.SelectMany(x => x.ResourceSupportSpecialists));
                    _context.ResourceSupports.RemoveRange(resourceSupportEntity.ResourceSupports);
                    _context.ResourceSupportReports.Remove(resourceSupportEntity);
                }
                else
                {
                    // Изриване на свързаното Ресурсно подпомагане
                    var resourceSupportToDelete = resourceSupportEntity.ResourceSupports
                        .Where(x => x.AdditionalPersonalDevelopmentSupportItemId.HasValue && toDelete.Any(i => i.Id == x.AdditionalPersonalDevelopmentSupportItemId.Value))
                        .ToList();

                    _context.ResourceSupportSpecialists.RemoveRange(resourceSupportToDelete.SelectMany(x => x.ResourceSupportSpecialists));
                    _context.ResourceSupports.RemoveRange(resourceSupportToDelete);
                }
            }
            #endregion
        }

        /// <summary>
        /// Добавяне на ДПЛР.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="resourceSupportEntity"></param>
        /// <returns></returns>
        private async Task<ResourceSupportReport> Add(AdditionalPersonalDevelopmentSupportModel model, AdditionalPersonalDevelopmentSupport entity, ResourceSupportReport resourceSupportEntity)
        {
            IEnumerable<AdditionalPersonalDevelopmentSupportItemModel> toAdd = model.Items.Where(x => !(x.Id.HasValue && x.Id.Value != 0));
            if (toAdd.IsNullOrEmpty())
            {
                return resourceSupportEntity;
            }

            foreach (var item in toAdd)
            {
                entity.AdditionalPersonalDevelopmentSupportItems.Add(new AdditionalPersonalDevelopmentSupportItem
                {
                    TypeId = item.TypeId,
                    Details = item.Details,
                });
            }

            #region Добавяне на свързаното Ресурсно подпомагане

            IEnumerable<AdditionalPersonalDevelopmentSupportItemModel> itemsWithResourceSupport = toAdd.Where(x => !x.ResourceSupport.IsNullOrEmpty());
            if (!itemsWithResourceSupport.IsNullOrEmpty())
            {
                /* Запазваме защото ни трябват AdditionalPersonalDevelopmentSupportItems Id-тата,
                * за да ги обвържем с ResourceSupport.AdditionalPersonalDevelopmentSupportItemId.
                */
                await SaveAsync();

                if (resourceSupportEntity == null)
                {
                    resourceSupportEntity = new ResourceSupportReport
                    {
                        PersonId = model.PersonId,
                        SchoolYear = model.SchoolYear,
                        ReportNumber = model.OrderNumber ?? "",
                        ReportDate = model.OrderDate
                    };

                    _context.ResourceSupportReports.Add(resourceSupportEntity);
                }

                foreach (var item in itemsWithResourceSupport)
                {
                    foreach (var resourceSupportModel in item.ResourceSupport)
                    {
                        resourceSupportEntity.ResourceSupports.Add(new ResourceSupport
                        {
                            ResourceSupportTypeId = resourceSupportModel.ResourceSupportTypeId,
                            AdditionalPersonalDevelopmentSupportItemId = entity.AdditionalPersonalDevelopmentSupportItems
                                .Where(x => x.TypeId == item.TypeId)
                                .Select(x => x.Id).FirstOrDefault(),
                            ResourceSupportSpecialists = resourceSupportModel.ResourceSupportSpecialists.Select(x => new ResourceSupportSpecialist
                            {
                                Name = x.Name,
                                OrganizationType = x.OrganizationType,
                                OrganizationName = x.OrganizationName,
                                SpecialistType = x.SpecialistType,
                                ResourceSupportSpecialistTypeId = x.ResourceSupportSpecialistTypeId,
                                WorkPlaceId = x.WorkPlaceId,
                                WeeklyHours = x.WeeklyHours,
                            }).ToList()

                        });
                    }
                }

            }

            #endregion

            return resourceSupportEntity;
        }

        /// <summary>
        /// Обновяване на ДПЛР.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="resourceSupportEntity"></param>
        private void Update(AdditionalPersonalDevelopmentSupportModel model, AdditionalPersonalDevelopmentSupport entity, ResourceSupportReport resourceSupportEntity)
        {
            HashSet<int> existedIds = model.Items.Where(x => x.Id.HasValue && x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet();

            if (resourceSupportEntity != null) { 
                resourceSupportEntity.SchoolYear = entity.SchoolYear;
                resourceSupportEntity.ReportNumber = entity.OrderNumber;
                resourceSupportEntity.ReportDate = entity.OrderDate;
            }

            foreach (AdditionalPersonalDevelopmentSupportItem toUpdate in entity.AdditionalPersonalDevelopmentSupportItems.Where(x => existedIds.Contains(x.Id)))
            {
                AdditionalPersonalDevelopmentSupportItemModel source = model.Items.SingleOrDefault(x => x.Id.HasValue && x.Id.Value != 0 && x.Id.Value == toUpdate.Id);
                if (source == null) continue;

                toUpdate.TypeId = source.TypeId;
                toUpdate.Details = source.Details;

                // Изтриване на свръзаните към ДПЛР-то записи от Ресурсно подпомагане
                IEnumerable<ResourceSupport> resourceSupportToDelete = resourceSupportEntity?.ResourceSupports
                    .Where(x => x.AdditionalPersonalDevelopmentSupportItemId.HasValue && x.AdditionalPersonalDevelopmentSupportItemId == toUpdate.Id
                    && !source.ResourceSupport.Any(r => r.Id == x.Id));
                if (!resourceSupportToDelete.IsNullOrEmpty())
                {
                    _context.ResourceSupportSpecialists.RemoveRange(resourceSupportToDelete.SelectMany(x => x.ResourceSupportSpecialists));
                    _context.ResourceSupports.RemoveRange(resourceSupportToDelete);
                }

                // Добавяне на Ресурсно подпомагане към ДПЛР-то
                IEnumerable<ResourceSupportModel> resourceSupportToAdd = source.ResourceSupport.Where(x => !x.Id.HasValue);
                if (!resourceSupportToAdd.IsNullOrEmpty())
                {
                    if (resourceSupportEntity == null)
                    {
                        resourceSupportEntity = new ResourceSupportReport
                        {
                            PersonId = model.PersonId,
                            SchoolYear = model.SchoolYear,
                            ReportNumber = model.OrderNumber ?? "",
                            ReportDate = model.OrderDate
                        };

                        _context.ResourceSupportReports.Add(resourceSupportEntity);

                    }

                    foreach (var resourceSupportModel in resourceSupportToAdd)
                    {
                        resourceSupportEntity.ResourceSupports.Add(new ResourceSupport
                        {
                            ResourceSupportTypeId = resourceSupportModel.ResourceSupportTypeId,
                            AdditionalPersonalDevelopmentSupportItemId = toUpdate.Id,
                            ResourceSupportSpecialists = resourceSupportModel.ResourceSupportSpecialists.Select(x => new ResourceSupportSpecialist
                            {
                                Name = x.Name,
                                OrganizationType = x.OrganizationType,
                                OrganizationName = x.OrganizationName,
                                SpecialistType = x.SpecialistType,
                                ResourceSupportSpecialistTypeId = x.ResourceSupportSpecialistTypeId,
                                WorkPlaceId = x.WorkPlaceId,
                                WeeklyHours = x.WeeklyHours,
                            }).ToList()
                        });
                    }
                }

                if (resourceSupportEntity != null && !resourceSupportEntity.ResourceSupports.IsNullOrEmpty())
                {
                    // Обновяване на свързаните към ДПЛР-то записи от Ресурсно подпомагане
                    HashSet<int> existedResourceSupportIds = source.ResourceSupport.Where(x => x.Id.HasValue && x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet();
                    foreach (ResourceSupport rsToUpdate in resourceSupportEntity.ResourceSupports.Where(x => existedResourceSupportIds.Contains(x.Id)))
                    {
                        ResourceSupportModel rsSource = source?.ResourceSupport.FirstOrDefault(x => x.Id.HasValue && x.Id.Value != 0 && x.Id.Value == rsToUpdate.Id);
                        if (rsSource == null)
                        {
                            continue;
                        }

                        rsToUpdate.ResourceSupportTypeId = rsSource.ResourceSupportTypeId;

                        IEnumerable<ResourceSupportSpecialist> rsSpecialistsToDelete = rsToUpdate.ResourceSupportSpecialists
                            .Where(x => !rsSource.ResourceSupportSpecialists.Any(r => r.Id == x.Id));
                        if (!rsSpecialistsToDelete.IsNullOrEmpty())
                        {
                            _context.ResourceSupportSpecialists.RemoveRange(rsSpecialistsToDelete);
                        }

                        IEnumerable<ResourceSupportSpecialistModel> rsSpecialistsToAdd = rsSource.ResourceSupportSpecialists.Where(x => !x.Id.HasValue);
                        if (!rsSpecialistsToAdd.IsNullOrEmpty())
                        {
                            foreach (var rsSpecialist in rsSpecialistsToAdd)
                            {
                                rsToUpdate.ResourceSupportSpecialists.Add(new ResourceSupportSpecialist
                                {
                                    Name = rsSpecialist.Name,
                                    OrganizationType = rsSpecialist.OrganizationType,
                                    OrganizationName = rsSpecialist.OrganizationName,
                                    SpecialistType = rsSpecialist.SpecialistType,
                                    ResourceSupportSpecialistTypeId = rsSpecialist.ResourceSupportSpecialistTypeId,
                                    WorkPlaceId = rsSpecialist.WorkPlaceId,
                                    WeeklyHours = rsSpecialist.WeeklyHours,
                                });
                            }
                        }

                        HashSet<int> existedResourceSupportSpecialistsIds = rsSource.ResourceSupportSpecialists.Where(x => x.Id.HasValue && x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet();
                        foreach (ResourceSupportSpecialist rsSpecialistToUpdate in rsToUpdate.ResourceSupportSpecialists.Where(x => existedResourceSupportSpecialistsIds.Contains(x.Id)))
                        {
                            ResourceSupportSpecialistModel rsSpecialistSource = rsSource.ResourceSupportSpecialists.FirstOrDefault(x => x.Id.HasValue && x.Id.Value != 0 && x.Id.Value == rsSpecialistToUpdate.Id);
                            if (rsSpecialistSource == null)
                            {
                                continue;
                            }

                            rsSpecialistToUpdate.Name = rsSpecialistSource.Name;
                            rsSpecialistToUpdate.OrganizationType = rsSpecialistSource.OrganizationType;
                            rsSpecialistToUpdate.OrganizationName = rsSpecialistSource.OrganizationName;
                            rsSpecialistToUpdate.SpecialistType = rsSpecialistSource.SpecialistType;
                            rsSpecialistToUpdate.ResourceSupportSpecialistTypeId = rsSpecialistSource.ResourceSupportSpecialistTypeId;
                            rsSpecialistToUpdate.WorkPlaceId = rsSpecialistSource.WorkPlaceId;
                            rsSpecialistToUpdate.WeeklyHours = rsSpecialistSource.WeeklyHours;
                        }
                    }
                }
            }
        }

        private async Task ProcessAddedDocs(AdditionalPersonalDevelopmentSupportModel model, AdditionalPersonalDevelopmentSupport entity,
            SpecialNeedsYear sopEntity, ResourceSupportReport resourceSupportEntity)
        {
            foreach (var x in model.AllDocuments.Where(x => x.Document.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(x.Document.NoteContents, x.Document.NoteFileName, x.Document.NoteFileType);
                entity.AdditionalPersonalDevelopmentSupportAttachments.Add(new AdditionalPersonalDevelopmentSupportAttachment
                {
                    Type = x.Type,
                    Document = x.Document.ToDocument(result?.Data.BlobId)
                });

                sopEntity?.SpecialNeedsYearAttachments.Add(new SpecialNeedsYearAttachment
                {
                    Document = x.Document.ToDocument(result?.Data.BlobId)
                });

                resourceSupportEntity?.ResourceSupportDocuments.Add(new ResourceSupportDocument
                {
                    Document = x.Document.ToDocument(result?.Data.BlobId)
                });
            }

            if (resourceSupportEntity != null)
            {
                foreach (var doc in model.AllDocuments.Where(x => x.Document.Deleted != true
                    && x.Document.BlobId.HasValue && !resourceSupportEntity.ResourceSupportDocuments.Any(d => d.Document.BlobId == x.Document.BlobId)))
                {
                    resourceSupportEntity.ResourceSupportDocuments.Add(new ResourceSupportDocument
                    {
                        Document = doc.Document.ToDocument(doc.Document.BlobId)
                    });
                }
            }
        }

        private async Task ProcessDeletedDocs(AdditionalPersonalDevelopmentSupportModel model)
        {
            HashSet<int> blobsToDelete = model.AllDocuments
                .Where(x => x.Document.Id.HasValue && x.Document.Deleted == true && x.Document.BlobId.HasValue)
                .Select(x => x.Document.BlobId.Value).ToHashSet();

            if (blobsToDelete.Count > 0)
            {
                await _context.AdditionalPersonalDevelopmentSupportAttachments
                    .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.PersonalDevelopmentDocuments
                    .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.SpecialNeedsYearAttachments
                   .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.ResourceSupportDocuments
                   .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.Documents.Where(x => x.BlobId.HasValue && blobsToDelete.Contains(x.BlobId.Value)).DeleteAsync();
            }
        }

        private async Task<SpecialNeedsYear> ProcessSops(AdditionalPersonalDevelopmentSupportModel model)
        {
            if (model.StudentTypeId != (int)PersonalDevelopmentSupportStudentTypeEnum.SOP)
            {
                return null;
            }

            HashSet<int> existedIds = model.Sop.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet();
            List<SopDetailsModel> toAdd = model.Sop.Where(x => !x.Id.HasValue).ToList();

            SpecialNeedsYear entity = await _context.SpecialNeedsYears
                   .Include(x => x.SpecialNeeds)
                   .Include(x => x.SpecialNeedsYearAttachments)
                   .FirstOrDefaultAsync(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear);

            if (entity == null && toAdd.Count > 0)
            {
                entity = new SpecialNeedsYear
                {
                    PersonId = model.PersonId,
                    SchoolYear = model.SchoolYear
                };

                _context.SpecialNeedsYears.Add(entity);
            }

            if (entity == null)
            {
                return entity;
            }

            var toDelete = entity.SpecialNeeds.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // За изтриване
                _context.SpecialNeeds.RemoveRange(toDelete);
            }

            foreach (SopDetailsModel sop in toAdd)
            {
                entity.SpecialNeeds.Add(new SpecialNeed
                {
                    SpecialNeedsTypeId = sop.SpecialNeedsTypeId,
                    SpecialNeedsSubTypeId = sop.SpecialNeedsSubTypeId
                });
            }

            if (existedIds.Count > 0)
            {
                // За редакция
                foreach (var toUpdate in entity.SpecialNeeds.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.Sop.SingleOrDefault(x => x.Id == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.SpecialNeedsTypeId = source.SpecialNeedsTypeId;
                    toUpdate.SpecialNeedsSubTypeId = source.SpecialNeedsSubTypeId;
                }
            }

            return entity;
        }

        private async Task<ApiValidationResult> Validate(AdditionalPersonalDevelopmentSupportModel model)
        {
            ApiValidationResult validationResult = new ApiValidationResult();
            if (model == null)
            {
                validationResult.Errors.Add(Messages.EmptyModelError);
                return validationResult;
            }

            if (await _context.AdditionalPersonalDevelopmentSupports.AnyAsync(x => x.Id != model.Id
                && x.IsSuspended != true
                && x.PersonId == model.PersonId
                && x.SchoolYear == model.SchoolYear
                && x.StudentTypeId == model.StudentTypeId))
            {
                validationResult.Errors.Add($"Вече съществува ДПЛР за този ученик и учебна година {model.SchoolYear}.");
            }

            if (model.StudentTypeId == (int)PersonalDevelopmentSupportStudentTypeEnum.SOP
                && model.Sop.IsNullOrEmpty())
            {

                validationResult.Errors.Add($"При избор на вид ученик „със СОП“, на който се предоставя ДПЛР, задължително трябва да се посочи вид СОП.");

            }

            IEnumerable<AdditionalPersonalDevelopmentSupportItemModel> items = model.Items.Where(x => !x.IsSuspended);

            if (!items.Any())
            {
                validationResult.Errors.Add("Необходимо е да въведете поне една дейност за ДПЛР.");
            }

            if (items.Any(x => NeedForResourceSupport.Contains(x.TypeId)))
            {
                if (!model.Orders.Any(x => x.Deleted != true))
                {
                    validationResult.Errors.Add("При избор на вид ДПЛР \"Ресурсно подпомагане\", \"Рехабилитация на слуха и говора\" и \"Зрителна рехабилитация\" задължително трябва да се прикачи \"Заповед/документ от РЦПППО за определяне на ДПЛР\".");
                }

                if (!model.Scorecards.Any(x => x.Deleted != true))
                {
                    validationResult.Errors.Add("При избор на вид ДПЛР \"Ресурсно подпомагане\", \"Рехабилитация на слуха и говора\" и \"Зрителна рехабилитация\" задължително трябва да се прикачи \"Карта за оценка на индивидуалните потребности на детето/ученика\".");
                }

                if (!model.Plans.Any(x => x.Deleted != true))
                {
                    validationResult.Errors.Add("При избор на вид ДПЛР \"Ресурсно подпомагане\", \"Рехабилитация на слуха и говора\" и \"Зрителна рехабилитация\" задължително трябва да се прикачи \"План за подкрепа на детето/ученика\".");
                }
            }

            if (items.Any(x => x.ResourceSupport.Any(rs => !rs.ResourceSupportSpecialists.Any())))
            {
                validationResult.Errors.Add("При избор на \"Ресурсно подпомагане\" задължително се изисква посочването на поне един специалист.");
            }

            if (items.Any(x => NeedForResourceSupport.Contains(x.TypeId) && !x.ResourceSupport.Any(rs => rs.ResourceSupportTypeId > 0)))
            {
                validationResult.Errors.Add("При избор на вид ДПЛР \"Ресурсно подпомагане\", \"Рехабилитация на слуха и говора\" и \"Зрителна рехабилитация\" " +
                    "задължително трябва да се посочи вид на институцията, която ще предоставя подкрепата.");
            }

            if (items.Any(x => x.TypeId == (int)AdditipnalPersonalDevelopmentSupportTypeEnum.ResourceSupport
                && !x.ResourceSupport.Any(rs => rs.ResourceSupportSpecialists.Any(rss => rss.ResourceSupportSpecialistTypeId == (int)ResourceSupportSpecialistTypeEnum.ResourceTeacher))))
            {
                validationResult.Errors.Add("При избор на вид ДПЛР \"Ресурсно подпомагане\" задължително се изисква поне един от посочените специалисти да е ресурсен учител.");
            }

            if (items.Any(x => (x.TypeId == (int)AdditipnalPersonalDevelopmentSupportTypeEnum.HearingAndSpeechRehabilitation || x.TypeId == (int)AdditipnalPersonalDevelopmentSupportTypeEnum.VisualRehabilitation)
                && !x.ResourceSupport.Any(rs => rs.ResourceSupportSpecialists.Any(rss => rss.ResourceSupportSpecialistTypeId == (int)ResourceSupportSpecialistTypeEnum.SensorySpecialistDisabilities))))
            {
                validationResult.Errors.Add("При избор на вид ДПЛР \"Рехабилитация на слуха и говора\" или \"Зрителна рехабилитация\"задължително се изисква поне един от посочените специалисти да е специалист по сензорни увреждания.");
            }

            var resourceSupportSpecialistGroups = items.SelectMany(x => x.ResourceSupport.SelectMany(rs => rs.ResourceSupportSpecialists))
                .GroupBy(rss => rss.ResourceSupportSpecialistTypeId);

            if (resourceSupportSpecialistGroups.Any(rss => rss.Count() > 1))
            {
                validationResult.Errors.Add("Има дублиране на 'Вид специалист'.");
            }

            return validationResult;
        }
        #endregion

        public async Task<AdditionalPersonalDevelopmentSupportViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            AdditionalPersonalDevelopmentSupportViewModel model = await
                _context.AdditionalPersonalDevelopmentSupports
                .Where(x => x.Id == id)
                .Select(x => new AdditionalPersonalDevelopmentSupportViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    FinalSchoolYear = x.FinalSchoolYear,
                    FinalSchoolYearName = x.FinalSchoolYearNavigation.Name,
                    PeriodTypeId = x.PeriodTypeId,
                    PeriodTypeName = x.PeriodType.Name,
                    StudentTypeId = x.StudentTypeId,
                    StudentTypeName = x.StudentType.Name,
                    OrderNumber = x.OrderNumber,
                    OrderDate = x.OrderDate,
                    IsSuspended = x.IsSuspended,
                    SuspensionDate = x.SuspensionDate,
                    SuspensionReason = x.SuspensionReason,
                    Items = x.AdditionalPersonalDevelopmentSupportItems
                        .Select(i => new AdditionalPersonalDevelopmentSupportItemViewModel
                        {
                            Id = i.Id,
                            TypeId = i.TypeId,
                            TypeName = i.Type.Name,
                            Details = i.Details,
                            IsSuspended = i.IsSuspended,
                            SuspensionDate = i.SuspensionDate,
                            SuspensionReason = i.SuspensionReason,
                            ResourceSupport = i.ResourceSupports
                                .Select(r => new ResourceSupportModel
                                {
                                    Id = r.Id,
                                    ResourceSupportReportId = r.ResourceSupportReportId,
                                    ResourceSupportTypeId = r.ResourceSupportTypeId,
                                    ResourceSupportTypeName = r.ResourceSupportType.Name,
                                    ResourceSupportSpecialists = r.ResourceSupportSpecialists.Select(s => new ResourceSupportSpecialistModel
                                    {
                                        Id = s.Id,
                                        Name = s.Name,
                                        OrganizationName = s.OrganizationName,
                                        OrganizationType = s.OrganizationType,
                                        ResourceSupportSpecialistTypeId = s.ResourceSupportSpecialistTypeId,
                                        WorkPlaceId = s.WorkPlaceId,
                                        SpecialistType = s.SpecialistType,
                                        ResourceSupportSpecialistTypeName = s.ResourceSupportSpecialistType.Name,
                                        WorkPlaceName = s.WorkPlace.Name,
                                        WeeklyHours = s.WeeklyHours,

                                    }).ToList(),
                                }),
                            SuspensionDocuments = i.AdditionalPersonalDevelopmentSupportItemAttachments
                                .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Suspension))
                                .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                        }),
                    Orders = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Order))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Scorecards = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Scorecard))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Plans = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Plan))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Documents = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Other))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Sop = _context.SpecialNeedsYears.Where(s => s.PersonId == x.PersonId && s.SchoolYear == x.SchoolYear)
                        .SelectMany(s => s.SpecialNeeds)
                        .Select(s => new SopDetailsViewModel
                        {
                            Id = s.Id,
                            SpecialNeedsTypeId = s.SpecialNeedsTypeId,
                            SpecialNeedsSubTypeId = s.SpecialNeedsSubTypeId,
                            SopTypeName = s.SpecialNeedsType.Name,
                            SopSubTypeName = s.SpecialNeedsSubType.Name,
                        })
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentException(nameof(model), nameof(AdditionalPersonalDevelopmentSupportViewModel)));
            }

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead, model.PersonId);

            return model;
        }

        public async Task<IEnumerable<SopDetailsViewModel>> GetSopForPerson(int personId, int schoolYear, CancellationToken cancellationToken)
        {
            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, personId);

            List<SopDetailsViewModel> models = await _context.SpecialNeedsYears
                .Where(x => x.PersonId == personId && x.SchoolYear == schoolYear)
                .SelectMany(s => s.SpecialNeeds)
                .OrderBy(x => x.Id)
                .Select(x => new SopDetailsViewModel
                {
                    Id = x.Id,
                    SpecialNeedsTypeId = x.SpecialNeedsTypeId,
                    SpecialNeedsSubTypeId = x.SpecialNeedsSubTypeId,
                    SopTypeName = x.SpecialNeedsType.Name,
                    SopSubTypeName = x.SpecialNeedsSubType.Name,
                })
                .ToListAsync(cancellationToken);

            return models;
        }

        public async Task<IPagedList<AdditionalPersonalDevelopmentSupportViewModel>> List(StudentListInput input, CancellationToken cancellationToken)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead, input.StudentId);

            IQueryable<AdditionalPersonalDevelopmentSupportViewModel> query = _context.AdditionalPersonalDevelopmentSupports
                .Where(x => x.PersonId == input.StudentId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.SchoolYearNavigation.Name.Contains(input.Filter)
                    || predicate.FinalSchoolYearNavigation.Name.Contains(input.Filter)
                    || predicate.PeriodType.Name.Contains(input.Filter)
                    || predicate.OrderNumber.Contains(input.Filter)
                    || predicate.OrderDate.ToString("dd/MM/yyyy").Contains(input.Filter)
                    || predicate.StudentType.Name.Contains(input.Filter))
                .Select(x => new AdditionalPersonalDevelopmentSupportViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    FinalSchoolYear = x.FinalSchoolYear,
                    FinalSchoolYearName = x.FinalSchoolYearNavigation.Name,
                    PeriodTypeId = x.PeriodTypeId,
                    PeriodTypeName = x.PeriodType.Name,
                    StudentTypeId = x.StudentTypeId,
                    StudentTypeName = x.StudentType.Name,
                    OrderNumber = x.OrderNumber,
                    OrderDate = x.OrderDate,
                    IsSuspended = x.IsSuspended,
                    SuspensionDate = x.SuspensionDate,
                    SuspensionReason = x.SuspensionReason,
                    Orders = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Order))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Scorecards = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Scorecard))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Plans = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Plan))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Documents = x.AdditionalPersonalDevelopmentSupportAttachments
                        .Where(x => x.Type == nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Other))
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, Id desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            IList<AdditionalPersonalDevelopmentSupportViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<VStudentEplrHoursTaken>> ListSchoolBookData(OresListInput input, CancellationToken cancellationToken)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead, input.PersonId);

            IQueryable<VStudentEplrHoursTaken> query = _context.VStudentEplrHoursTakens
               .Where(x => x.PersonId == input.PersonId && x.SchoolYear == input.SchoolYear)
               .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.PositionName.Contains(input.Filter)
                   || predicate.SubjectName.Contains(input.Filter)
                   || predicate.Date.ToString("dd/MM/yyyy").Contains(input.Filter)
                   || predicate.SubjectTypeName.Contains(input.Filter))
               .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Date desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            IList<VStudentEplrHoursTaken> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }
        public async Task Create(AdditionalPersonalDevelopmentSupportModel model)
        {
            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, model.PersonId);
            ApiValidationResult validationResult = await Validate(model);

            if (validationResult.HasErrors)
            {
                throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
            }

            using var transaction = _context.Database.BeginTransaction();

            AdditionalPersonalDevelopmentSupport entry = new AdditionalPersonalDevelopmentSupport
            {
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear,
                FinalSchoolYear = model.PeriodTypeId == (int)PersonalDevelopmentSupportPeriodTypeEnum.ShortTerm ? null : model.FinalSchoolYear,
                PeriodTypeId = model.PeriodTypeId,
                StudentTypeId = model.StudentTypeId,
                OrderNumber = model.OrderNumber ?? "",
                OrderDate = model.OrderDate
            };

            _context.AdditionalPersonalDevelopmentSupports.Add(entry);

            ResourceSupportReport resourceSupportEntity = await Add(model, entry, null);

            SpecialNeedsYear sopEntry = await ProcessSops(model);

            await ProcessAddedDocs(model, entry, sopEntry, resourceSupportEntity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(AdditionalPersonalDevelopmentSupportModel model)
        {
            AdditionalPersonalDevelopmentSupport entity = await _context.AdditionalPersonalDevelopmentSupports
                .Include(x => x.AdditionalPersonalDevelopmentSupportItems)
                .Include(x => x.AdditionalPersonalDevelopmentSupportAttachments)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, entity?.PersonId);
            ApiValidationResult validationResult = await Validate(model);

            if (validationResult.HasErrors)
            {
                throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
            }

            using var transaction = _context.Database.BeginTransaction();

            entity.SchoolYear = model.SchoolYear;
            entity.FinalSchoolYear = model.PeriodTypeId == (int)PersonalDevelopmentSupportPeriodTypeEnum.ShortTerm ? null : model.FinalSchoolYear;
            entity.PeriodTypeId = model.PeriodTypeId;
            entity.StudentTypeId = model.StudentTypeId;
            entity.OrderNumber = model.OrderNumber ?? "";
            entity.OrderDate = model.OrderDate;

            // Свързано Ресурсно подпомагане
            List<int> ids = entity.AdditionalPersonalDevelopmentSupportItems.Select(x => x.Id).ToList();
            ResourceSupportReport resourceSupportEntity = await _context.ResourceSupportReports
                .Include(x => x.ResourceSupports).ThenInclude(x => x.ResourceSupportSpecialists)
                .Include(x => x.ResourceSupportDocuments).ThenInclude(x => x.Document)
                .Where(x => x.ResourceSupports.Any(r => r.AdditionalPersonalDevelopmentSupportItemId.HasValue && ids.Contains(r.AdditionalPersonalDevelopmentSupportItemId.Value)))
                .FirstOrDefaultAsync();

            Delete(model, entity, resourceSupportEntity);
            await Add(model, entity, resourceSupportEntity);
            Update(model, entity, resourceSupportEntity);

            SpecialNeedsYear sopEntry = await ProcessSops(model);

            await ProcessAddedDocs(model, entity, sopEntry, resourceSupportEntity);
            await ProcessDeletedDocs(model);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            AdditionalPersonalDevelopmentSupport entity = await _context.AdditionalPersonalDevelopmentSupports
                .Include(x => x.AdditionalPersonalDevelopmentSupportItems)
                .Include(x => x.AdditionalPersonalDevelopmentSupportAttachments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(entity), nameof(AdditionalPersonalDevelopmentSupport)));
            }

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, entity.PersonId);

            if (!entity.AdditionalPersonalDevelopmentSupportItems.IsNullOrEmpty())
            {
                // Изтриване на свързаното Ресурсно подпомагане
                List<int> ids = entity.AdditionalPersonalDevelopmentSupportItems.Select(x => x.Id).ToList();
                ResourceSupportReport resourceSupportEntity = await _context.ResourceSupportReports
                    .Include(x => x.ResourceSupports).ThenInclude(x => x.ResourceSupportSpecialists)
                    .Include(x => x.ResourceSupportDocuments)
                    .Where(x => x.ResourceSupports.Any(r => r.AdditionalPersonalDevelopmentSupportItemId.HasValue && ids.Contains(r.AdditionalPersonalDevelopmentSupportItemId.Value)))
                    .FirstOrDefaultAsync();

                if (resourceSupportEntity != null)
                {
                    _context.ResourceSupportDocuments.RemoveRange(resourceSupportEntity.ResourceSupportDocuments);
                    _context.ResourceSupportSpecialists.RemoveRange(resourceSupportEntity.ResourceSupports.SelectMany(x => x.ResourceSupportSpecialists));
                    _context.ResourceSupports.RemoveRange(resourceSupportEntity.ResourceSupports);
                    _context.ResourceSupportReports.Remove(resourceSupportEntity);
                }

                _context.AdditionalPersonalDevelopmentSupportItems.RemoveRange(entity.AdditionalPersonalDevelopmentSupportItems);
            }

            _context.AdditionalPersonalDevelopmentSupportAttachments.RemoveRange(entity.AdditionalPersonalDevelopmentSupportAttachments);

            if (entity.StudentTypeId == (int)PersonalDevelopmentSupportStudentTypeEnum.SOP)
            {
                // При изтриване на ДПЛР от тип СОП ще изтрием и свързаните данни в таблицита за СОП.
                List<SpecialNeedsYear> sopEntities = await _context.SpecialNeedsYears
                   .Include(x => x.SpecialNeeds)
                   .Include(x => x.SpecialNeedsYearAttachments)
                   .Where(x => x.PersonId == entity.PersonId && x.SchoolYear == entity.SchoolYear)
                   .ToListAsync();

                foreach (SpecialNeedsYear sopEntity in sopEntities)
                {
                    _context.SpecialNeedsYearAttachments.RemoveRange(sopEntity.SpecialNeedsYearAttachments);
                    _context.SpecialNeeds.RemoveRange(sopEntity.SpecialNeeds);
                    _context.SpecialNeedsYears.Remove(sopEntity);
                }
            }

            _context.AdditionalPersonalDevelopmentSupports.Remove(entity);

            await SaveAsync();
        }

        public async Task SuspendAdditionalPersonalDevelopmentSupport(AdditionalPersonalDevelopmentSupportISuspendtemModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AdditionalPersonalDevelopmentSupportItem entity = await _context.AdditionalPersonalDevelopmentSupportItems
                .Include(x => x.AdditionalPersonalDevelopmentSupport)
                .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

            await CheckPermission(DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage, entity?.AdditionalPersonalDevelopmentSupport.PersonId);

            if (entity.IsSuspended)
            {
                throw new ApiException("ДПЛР-от вече е прекратено!");
            }

            /* •Данните за период на предоставяне на ДПЛР ще се разширят с крайна дата (за случаите в които се прекратява ДПЛР).
                Ако към избраната от потребителя на НЕИСПУО дата на прекратяване на ДПЛР детето/ученикът е записан в група
                от тип „Група за ресурсно подпомагане“ и/или е включен в дейност за ПЛР от вид „ДПЛР – рехабилитация на слуха и говора“, 
                „ДПЛР – зрителна рехабилитация“, „ДПЛР – ресурсно подпомагане“, ще се сигнализира съобщение за грешка
                и запис на данните няма да се позволява.
             */

            if (NeedForResourceSupport.Contains(entity.TypeId) && model.SuspensionDate.HasValue)
            {
                FormattableString queryString = $"select * from student.StudentClassTemporal_AsOf({model.SuspensionDate:yyyy-MM-dd})";
                IQueryable<StudentClassTemporal> query = _context.Set<StudentClassTemporal>()
                    .FromSqlInterpolated(queryString);

                List<StudentClassTemporal> studentClasses = await (
                        from sc in query.Where(x => x.PersonId == entity.AdditionalPersonalDevelopmentSupport.PersonId && x.IsCurrent)
                        join cg in _context.ClassGroups on sc.ClassId equals cg.ClassId
                        where cg.ClassTypeId == 37
                            ||
                            (_context.CurriculumStudents.Any(cs => cs.StudentId == sc.Id && cs.IsValid == true && new int?[] { 136, 137, 141 }.Contains(cs.Curriculum.SubjectTypeId)))     
                        select sc)
                        .ToListAsync(cancellationToken);

                if (studentClasses.Count > 0)
                {
                    throw new ApiException($"Към избраната дата на прекратяване на ДПЛР ({model.SuspensionDate:dd.MM.yyyy}), детето/ученикът е записан в група за ресурсно подпомагане " +
                        $"и/или е включен в дейност \"ДПЛР – рехабилитация на слуха и говора\",\"ДПЛР – зрителна рехабилитация\" или \"ДПЛР – ресурсно подпомагане\"");
                }
            }

            entity.IsSuspended = true;
            entity.SuspensionDate = model.SuspensionDate;
            entity.SuspensionReason = model.SuspensionReason;

            foreach (var x in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(x.NoteContents, x.NoteFileName, x.NoteFileType);
                entity.AdditionalPersonalDevelopmentSupportItemAttachments.Add(new AdditionalPersonalDevelopmentSupportItemAttachment
                {
                    Type = nameof(AdditionalPersonalDevelopmentSupportFileTypeEnum.Suspension),
                    Document = x.ToDocument(result?.Data.BlobId)
                });

            }

            await SaveAsync(cancellationToken);
        }
    }
}
