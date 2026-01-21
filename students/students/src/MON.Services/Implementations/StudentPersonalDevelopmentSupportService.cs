using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.Enums;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class StudentPersonalDevelopmentSupportService : BaseService<StudentPersonalDevelopmentSupportService>, IStudentPersonalDevelopmentSupportService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly ISignalRNotificationService _signalRNotificationService;

        public StudentPersonalDevelopmentSupportService(DbServiceDependencies<StudentPersonalDevelopmentSupportService> dependencies,
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

        public async Task<PersonalDevelopmentSupportModel> GetById(int id)
        {
            PersonalDevelopmentSupportModel model = await _context.PersonalDevelopmentSupports
                .Where(x => x.Id == id)
                .Select(x => new PersonalDevelopmentSupportModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    AdditionalModulesNeededForNonBulgarianSpeakingInfo = x.AdditionalModulesNeededForNonBulgarianSpeakingInfo,
                    EarlyEvaluationAndEducationalRiskInfo = x.EarlyEvaluationAndEducationalRiskInfo,
                    EvaluationConclusionInfo = x.EvaluationConclusionInfo,
                    StudentTypeId = x.StudentTypeId,
                    SupportPeriodTypeId = x.SupportPeriodTypeId,
                    EarlyEvaluationReasons = x.PersonalDevelopmentEarlyEvaluationReasons.Select(p => new PersonalDevelopmenReasonsModel
                    {
                        Id = p.Id,
                        Information = p.Information,
                        ReasonId = p.EarlyEvaluationReasonId,
                    }),
                    CommonSupportTypeReasons = x.PersonalDevelopmentCommonSupportTypes.Select(p => new PersonalDevelopmenReasonsModel
                    {
                        Id = p.Id,
                        Information = p.Information,
                        ReasonId = p.CommonSupportTypeId
                    }),
                    AdditionalSupportTypeReasons = x.PersonalDevelopmentAdditionalSupportTypes.Select(p => new PersonalDevelopmenReasonsModel
                    {
                        Id = p.Id,
                        Information = p.Information,
                        ReasonId = p.AdditionalSupportTypeId
                    }),
                    AdditionalSupportDocuments = x.PersonalDevelopmentDocuments
                        .Where(d => d.DocumentType == (int)PersonalDevelopmentDocumentType.AdditionalSupport)
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    CommonSupportDocuments = x.PersonalDevelopmentDocuments
                        .Where(d => d.DocumentType == (int)PersonalDevelopmentDocumentType.CommonSupport)
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    EarlyEvaluationDocuments = x.PersonalDevelopmentDocuments
                        .Where(d => d.DocumentType == (int)PersonalDevelopmentDocumentType.EarlyEvaluation)
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                })
                .SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<PersonalDevelopmentSupportViewModel>> GetListForPerson(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await (from x in _context.PersonalDevelopmentSupports
                          join lf in _context.Lodfinalizations on new { x.PersonId, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                          from lodFin in temp.DefaultIfEmpty()
                          where x.PersonId == personId
                          orderby x.SchoolYear descending
                          select new PersonalDevelopmentSupportViewModel
                          {
                              Id = x.Id,
                              SchoolYear = x.SchoolYear,
                              SchoolYearName = x.SchoolYearNavigation.Name,
                              EarlyEvaluationReasons = x.PersonalDevelopmentEarlyEvaluationReasons.Select(x => new PdsReasonViewModel
                              {
                                  ReasonName = x.EarlyEvaluationReason.Name
                              }),
                              CommonSupportTypeReasons = x.PersonalDevelopmentCommonSupportTypes.Select(x => new PdsReasonViewModel
                              {
                                  ReasonName = x.CommonSupportType.Name
                              }),
                              AdditionalSupportTypeReasons = x.PersonalDevelopmentAdditionalSupportTypes.Select(x => new PdsReasonViewModel
                              {
                                  ReasonName = x.AdditionalSupportType.Name
                              }),
                              Documents = x.PersonalDevelopmentDocuments.Select(x => x.Document.ToViewModel(_blobServiceConfig)),
                              IsLodFinalized = lodFin != null && lodFin.IsFinalized
                          }
                )
                .ToListAsync();
        }

        public async Task Create(PersonalDevelopmentSupportModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _context.PersonalDevelopmentSupports.AnyAsync(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear))
            {
                throw new ApiException(Messages.InvalidOperation,
                    new InvalidOperationException($"Подкрепа за личностно развитие за този ученик и учебна година {model.SchoolYear} вече съществува!"));
            }

            PersonalDevelopmentSupport entry = new PersonalDevelopmentSupport
            {
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear,
                EvaluationConclusionInfo = model.EvaluationConclusionInfo,
                EarlyEvaluationAndEducationalRiskInfo = model.EarlyEvaluationAndEducationalRiskInfo,
                AdditionalModulesNeededForNonBulgarianSpeakingInfo = model.AdditionalModulesNeededForNonBulgarianSpeakingInfo,
                PersonalDevelopmentEarlyEvaluationReasons = new List<PersonalDevelopmentEarlyEvaluationReason>(),
                PersonalDevelopmentCommonSupportTypes = new List<PersonalDevelopmentCommonSupportType>(),
                PersonalDevelopmentAdditionalSupportTypes = new List<PersonalDevelopmentAdditionalSupportType>(),
                StudentTypeId = model.StudentTypeId,
                SupportPeriodTypeId = model.SupportPeriodTypeId
            };

            ManageEarlyEvaluationReason(model, entry);
            ManageCommonSupportTypes(model, entry);
            ManageAdditionalSupportType(model, entry);

            using var transaction = _context.Database.BeginTransaction();
            _context.PersonalDevelopmentSupports.Add(entry);

            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
            await _signalRNotificationService.PersonalDevelopmentModified(entry.PersonId, entry.Id);
        }

        public async Task Update(PersonalDevelopmentSupportModel model)
        {
            PersonalDevelopmentSupport entity = await _context.PersonalDevelopmentSupports
                .Include(x => x.PersonalDevelopmentEarlyEvaluationReasons)
                .Include(x => x.PersonalDevelopmentCommonSupportTypes)
                .Include(x => x.PersonalDevelopmentAdditionalSupportTypes)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(entity), nameof(PersonalDevelopmentSupport)));
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _context.PersonalDevelopmentSupports.AnyAsync(x => x.Id != entity.Id
                && x.PersonId == model.PersonId
                && x.SchoolYear == model.SchoolYear))
            {
                throw new ApiException(Messages.InvalidOperation,
                    new InvalidOperationException($"Подкрепа за личностно развитие за този ученик и учебна година {model.SchoolYear} вече съществува!"));
            }

            entity.SchoolYear = model.SchoolYear;
            entity.StudentTypeId = model.StudentTypeId;
            entity.SupportPeriodTypeId = model.SupportPeriodTypeId;
            entity.AdditionalModulesNeededForNonBulgarianSpeakingInfo = model.AdditionalModulesNeededForNonBulgarianSpeakingInfo;
            entity.EarlyEvaluationAndEducationalRiskInfo = model.EarlyEvaluationAndEducationalRiskInfo;
            entity.EvaluationConclusionInfo = model.EvaluationConclusionInfo;

            ManageEarlyEvaluationReason(model, entity);
            ManageCommonSupportTypes(model, entity);
            ManageAdditionalSupportType(model, entity);

            using var transaction = _context.Database.BeginTransaction();

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model);

            await SaveAsync();
            await transaction.CommitAsync();
            await _signalRNotificationService.PersonalDevelopmentModified(entity.PersonId, entity.Id);
        }

        public async Task Delete(int id)
        {
            PersonalDevelopmentSupport entity = await _context.PersonalDevelopmentSupports
                .Include(x => x.PersonalDevelopmentDocuments)
                .Include(x => x.PersonalDevelopmentEarlyEvaluationReasons)
                .Include(x => x.PersonalDevelopmentCommonSupportTypes)
                .Include(x => x.PersonalDevelopmentAdditionalSupportTypes)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.PersonalDevelopmentDocuments != null && entity.PersonalDevelopmentDocuments.Any())
            {
                _context.PersonalDevelopmentDocuments.RemoveRange(entity.PersonalDevelopmentDocuments);
            }

            if (entity.PersonalDevelopmentEarlyEvaluationReasons != null && entity.PersonalDevelopmentEarlyEvaluationReasons.Any())
            {
                _context.PersonalDevelopmentEarlyEvaluationReasons.RemoveRange(entity.PersonalDevelopmentEarlyEvaluationReasons);
            }

            if (entity.PersonalDevelopmentCommonSupportTypes != null && entity.PersonalDevelopmentCommonSupportTypes.Any())
            {
                _context.PersonalDevelopmentCommonSupportTypes.RemoveRange(entity.PersonalDevelopmentCommonSupportTypes);
            }

            if (entity.PersonalDevelopmentAdditionalSupportTypes != null && entity.PersonalDevelopmentAdditionalSupportTypes.Any())
            {
                _context.PersonalDevelopmentAdditionalSupportTypes.RemoveRange(entity.PersonalDevelopmentAdditionalSupportTypes);
            }

            _context.PersonalDevelopmentSupports.Remove(entity);

            await SaveAsync();
            await _signalRNotificationService.PersonalDevelopmentModified(entity.PersonId, entity.Id);
        }

        private void ManageEarlyEvaluationReason(PersonalDevelopmentSupportModel model, PersonalDevelopmentSupport entity)
        {
            if (model == null) return;

            HashSet<int> existedIds = model.EarlyEvaluationReasons != null
                ? model.EarlyEvaluationReasons.Where(x => x.Id.HasValue)
                .Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.PersonalDevelopmentEarlyEvaluationReasons.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // За изтриване
                _context.PersonalDevelopmentEarlyEvaluationReasons.RemoveRange(toDelete);
            }

            if (model.EarlyEvaluationReasons != null)
            {
                // За добавяне
                var toAdd = model.EarlyEvaluationReasons.Where(x => !x.Id.HasValue);
                foreach (var item in toAdd)
                {
                    entity.PersonalDevelopmentEarlyEvaluationReasons.Add(new PersonalDevelopmentEarlyEvaluationReason
                    {
                        EarlyEvaluationReasonId = item.ReasonId,
                        Information = item.Information
                    });
                }
            }

            if (existedIds.Any())
            {
                // За редакция
                foreach (var toUpdate in entity.PersonalDevelopmentEarlyEvaluationReasons.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.EarlyEvaluationReasons.FirstOrDefault(x => x.Id == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.EarlyEvaluationReasonId = source.ReasonId;
                    toUpdate.Information = source.Information;
                }
            }
        }

        private void ManageCommonSupportTypes(PersonalDevelopmentSupportModel model, PersonalDevelopmentSupport entity)
        {
            if (model == null) return;

            HashSet<int> existedIds = model.CommonSupportTypeReasons != null
                ? model.CommonSupportTypeReasons.Where(x => x.Id.HasValue)
                .Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.PersonalDevelopmentCommonSupportTypes.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // За изтриване
                _context.PersonalDevelopmentCommonSupportTypes.RemoveRange(toDelete);
            }

            if (model.CommonSupportTypeReasons != null)
            {
                // За добавяне
                var toAdd = model.CommonSupportTypeReasons.Where(x => !x.Id.HasValue);
                foreach (var item in toAdd)
                {
                    entity.PersonalDevelopmentCommonSupportTypes.Add(new PersonalDevelopmentCommonSupportType
                    {
                        CommonSupportTypeId = item.ReasonId,
                        Information = item.Information
                    });
                }
            }

            if (existedIds.Any())
            {
                // За редакция
                foreach (var toUpdate in entity.PersonalDevelopmentCommonSupportTypes.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.CommonSupportTypeReasons.FirstOrDefault(x => x.Id == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.CommonSupportTypeId = source.ReasonId;
                    toUpdate.Information = source.Information;
                }
            }
        }

        private void ManageAdditionalSupportType(PersonalDevelopmentSupportModel model, PersonalDevelopmentSupport entity)
        {
            if (model == null) return;

            HashSet<int> existedIds = model.AdditionalSupportTypeReasons != null
                ? model.AdditionalSupportTypeReasons.Where(x => x.Id.HasValue)
                .Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.PersonalDevelopmentAdditionalSupportTypes.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // За изтриване
                _context.PersonalDevelopmentAdditionalSupportTypes.RemoveRange(toDelete);
            }

            if (model.AdditionalSupportTypeReasons != null)
            {
                // За добавяне
                var toAdd = model.AdditionalSupportTypeReasons.Where(x => !x.Id.HasValue);
                foreach (var item in toAdd)
                {
                    entity.PersonalDevelopmentAdditionalSupportTypes.Add(new PersonalDevelopmentAdditionalSupportType
                    {
                        AdditionalSupportTypeId = item.ReasonId,
                        Information = item.Information
                    });
                }
            }

            if (existedIds.Any())
            {
                // За редакция
                foreach (var toUpdate in entity.PersonalDevelopmentAdditionalSupportTypes.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.AdditionalSupportTypeReasons.FirstOrDefault(x => x.Id == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.AdditionalSupportTypeId = source.ReasonId;
                    toUpdate.Information = source.Information;
                }
            }
        }

        private async Task ProcessAddedDocs(PersonalDevelopmentSupportModel model, PersonalDevelopmentSupport entity)
        {
            if (model.AdditionalSupportDocuments != null && model.AdditionalSupportDocuments.Any())
            {
                foreach (DocumentModel docModel in model.AdditionalSupportDocuments.Where(x => x.HasToAdd()))
                {
                    var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                    entity.PersonalDevelopmentDocuments.Add(new PersonalDevelopmentDocument
                    {
                        Document = docModel.ToDocument(result?.Data?.BlobId),
                        DocumentType = (int)PersonalDevelopmentDocumentType.AdditionalSupport
                    });
                }
            }

            if (model.CommonSupportDocuments != null && model.CommonSupportDocuments.Any())
            {
                foreach (DocumentModel docModel in model.CommonSupportDocuments.Where(x => x.HasToAdd()))
                {
                    var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                    entity.PersonalDevelopmentDocuments.Add(new PersonalDevelopmentDocument
                    {
                        Document = docModel.ToDocument(result?.Data?.BlobId),
                        DocumentType = (int)PersonalDevelopmentDocumentType.CommonSupport
                    });
                }
            }

            if (model.EarlyEvaluationDocuments != null && model.EarlyEvaluationDocuments.Any())
            {
                foreach (DocumentModel docModel in model.EarlyEvaluationDocuments.Where(x => x.HasToAdd()))
                {
                    var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                    entity.PersonalDevelopmentDocuments.Add(new PersonalDevelopmentDocument
                    {
                        Document = docModel.ToDocument(result?.Data?.BlobId),
                        DocumentType = (int)PersonalDevelopmentDocumentType.EarlyEvaluation
                    });
                }
            }
        }

        private async Task ProcessDeletedDocs(PersonalDevelopmentSupportModel model)
        {
            HashSet<int> docIdsToDelete = new HashSet<int>();

            if (model.AdditionalSupportDocuments != null)
            {
                docIdsToDelete.UnionWith(model.AdditionalSupportDocuments.Where(x => x.Id.HasValue && x.Deleted == true).Select(x => x.Id.Value));
            }

            if (model.CommonSupportDocuments != null)
            {
                docIdsToDelete.UnionWith(model.CommonSupportDocuments.Where(x => x.Id.HasValue && x.Deleted == true).Select(x => x.Id.Value));
            }

            if (model.EarlyEvaluationDocuments != null)
            {
                docIdsToDelete.UnionWith(model.EarlyEvaluationDocuments.Where(x => x.Id.HasValue && x.Deleted == true).Select(x => x.Id.Value));
            }

            if (docIdsToDelete.Any())
            {
                await _context.PersonalDevelopmentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
                await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
            }
        }


    }
}
