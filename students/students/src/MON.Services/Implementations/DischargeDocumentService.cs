using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.Enums;
using MON.Models.UserManagement;
using MON.Services.Infrastructure.Validators;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{

    public class DischargeDocumentService : MovementDocumentBaseService<DischargeDocumentService>, IDischargeDocumentService
    {
        /*
            При отписване всички записи в EduState с позиция 3 за дадената институция стават 9.
            При отписване от позиция 7 (когато институцията не е ЦСОП - InstType <> 4), 8 и 10, в EduState записът за дадената институция се изтрива
            При отписване от позиция 7 (когато институцията е ЦСОП - InstType = 4), позицията в EduState става 9
            Всички записи от StudentClass стават с IsCurrent = 0 в дадената институция, Status = 2
         */
        private readonly DischargeDocumentValidator _validator;
        private readonly ILodFinalizationService _lodFinalizationService;

        public DischargeDocumentService(DbServiceDependencies<DischargeDocumentService> dependencies,
            MovementDocumentServiceDependencies<DischargeDocumentService> movementDocumentServiceDependencies,
            ILodFinalizationService lodFinalizationService,
            DischargeDocumentValidator validator)
            : base(dependencies, movementDocumentServiceDependencies)
        {
            _validator = validator;
            _lodFinalizationService = lodFinalizationService;
        }

        #region Private members
        private async Task ProcessAddedDocs(DischargeDocumentModel model, DischargeDocument entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.DischargeDocumentDocuments.Add(new DischargeDocumentDocument
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });

            }
        }

        private async Task ProcessDeletedDocs(DischargeDocumentModel model, DischargeDocument dischargeDocument)
        {
            if (model.Documents == null || !model.Documents.Any() || dischargeDocument == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.DischargeDocumentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }

        private async Task UpdateAdmissionPermissionRequest(DischargeDocument entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), nameof(DischargeDocument));
            }

            List<AdmissionPermissionRequest> admissionPermissionRequests = await _context.AdmissionPermissionRequests
                .Where(x => x.PersonId == entity.PersonId
                    && x.AuthorizingInstitutionId == entity.InstitutionId
                    && x.DischargeDocumentId == null
                    && x.IsPermissionGranted)
                .ToListAsync();

            foreach (AdmissionPermissionRequest admissionPermissionRequest in admissionPermissionRequests)
            {
                admissionPermissionRequest.DischargeDocument = entity;
            }
        }

        /// <summary>
        /// Добавяне на финализиране на ЛОД–а
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        private async Task AddFinalizationIfNotExists(int personId, short schoolYear, int? studentClassId, string Signature)
        {
            bool isLodApproved = await _lodFinalizationService.IsLodApproved(personId, schoolYear);
            if (!isLodApproved)
            {
                await _lodFinalizationService.ApproveLodAsync(new Models.StudentModels.Lod.LodFinalizationModel()
                {
                    ClassId = studentClassId,
                    PersonIds = new List<int>() { personId },
                    SchoolYear = schoolYear
                });
                await _lodFinalizationService.SignLodAsync(new Models.StudentModels.Lod.LodSignatureModel()
                {
                    //ClassId = studentClassId,
                    PersonId = personId,
                    SchoolYear = schoolYear,
                    Signature = Signature
                });
            }
        }
        #endregion

        public async Task<DischargeDocumentModel> GetById(int id)
        {
            DischargeDocumentModel model = await _context.DischargeDocuments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new DischargeDocumentModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    NoteNumber = x.NoteNumber,
                    NoteDate = x.NoteDate,
                    DischargeDate = x.DischargeDate,
                    DischargeReasonTypeId = x.DischargeReasonTypeId,
                    StudentClassId = x.CurrentStudentClassId,
                    Status = x.Status,
                    InstitutionId = x.InstitutionId,
                    Documents = x.DischargeDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                }).SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentUpdate))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return model;
        }

        public async Task<List<DischargeDocumentModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentDischargeDocumentRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            IQueryable<DischargeDocument> query = _context.DischargeDocuments
                .Where(x => x.PersonId == personId);

            if (_userInfo.IsSchoolDirector)
            {
                int institutionId = _userInfo?.InstitutionID ?? int.MinValue;
                query = query.Where(x => (x.InstitutionId.HasValue && x.InstitutionId == institutionId)
                    || (x.CurrentStudentClassId.HasValue && x.CurrentStudentClass.Class.InstitutionId == institutionId));
            }

            return await query.AsNoTracking()
                .Select(x => new DischargeDocumentModel
                {
                    Id = x.Id,
                    PersonId = personId,
                    DischargeDate = x.DischargeDate,
                    NoteDate = x.NoteDate,
                    NoteNumber = x.NoteNumber,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    CurrentStudentClass = x.CurrentStudentClassId,
                    CurrentStudentClassName = x.CurrentStudentClass.Class.ClassName,
                    Status = x.Status,
                    DischargeReasonTypeId = x.DischargeReasonTypeId,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    Documents = x.DischargeDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .OrderByDescending(x => x.NoteDate)
                .ToListAsync();
        }

        public async Task Create(DischargeDocumentModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentDischargeDocumentCreate))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = await _validator.ValidateCreation(model);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            // По някава причина има ситуации, където model.InstitutionId няма стойност.
            // Затова ще я вземем от данните на логнатия потребител.
            if (!model.InstitutionId.HasValue)
            {
                model.InstitutionId = GetInstitutionId(model.InstitutionId);
            }

            DischargeDocument entry = new DischargeDocument
            {
                NoteNumber = model.NoteNumber,
                NoteDate = model.NoteDate,
                InstitutionId = model.InstitutionId,
                DischargeReasonTypeId = model.DischargeReasonTypeId,
                PersonId = model.PersonId,
                CurrentStudentClassId = await GetCurrentStudentClass(model.PersonId, GetInstitutionId(model.InstitutionId), model.StudentClassId),
                DischargeDate = model.DischargeDate,
                Status = model.Status,
                SchoolYear = await _institutionService.GetCurrentYear(model.InstitutionId)
            };
                    
            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entry);
            _context.DischargeDocuments.Add(entry);

            if (!entry.IsDraft)
            {
                entry.ConfirmationDate = DateTime.UtcNow;
                if (await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentSignLOD)){
                    var signedDocument = model.Documents?.FirstOrDefault(i => i.NoteFileName.Contains("картон") && i.NoteContents != null && i.NoteContents.Length > 0);
                    if (signedDocument != null)
                    {
                        await AddFinalizationIfNotExists(model.PersonId, entry.SchoolYear, entry.CurrentStudentClassId, Convert.ToBase64String(signedDocument?.NoteContents));
                    }
                }
                await DischargeFromInstitution(model.PersonId, model.InstitutionId, model.DischargeReasonTypeId, null, entry);
                await SaveAsync();
                await _eduStateCacheService.ClearEduStatesForStudent(model.PersonId);

                await UpdateAdmissionPermissionRequest(entry);
                if (entry.InstitutionId.HasValue)
                {
                    await SaveInstitutionChange(entry.InstitutionId.Value, entry.SchoolYear);
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entry.PersonId);

            if (!entry.IsDraft)
            {
                await EnrollmentSchoolDelete(model.PersonId, GetInstitutionId(model.InstitutionId));
            }
            //await transaction.RollbackAsync();
        }

        public async Task Update(DischargeDocumentModel model)
        {
            DischargeDocument entity = await _context.DischargeDocuments
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentDischargeDocumentUpdate))
            {
                // Ako нямаме права за редакция на документите за отписване на ученика
                // проверяваме за налични за четене и за съвпадение на институцията на док. с тази на логнатия потребител.
                // Това се прави с цел редакция на потвърдени собствени документи.
                if (!_userInfo.InstitutionID.HasValue || entity.InstitutionId != _userInfo.InstitutionID
                    || !await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            bool fromDraftToConfirmedStatusChange = entity.IsDraft && entity.Status != model.Status;

            ApiValidationResult validationResult = await _validator.ValidateUpdate(entity, fromDraftToConfirmedStatusChange);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            entity.NoteNumber = model.NoteNumber;
            entity.DischargeDate = model.DischargeDate;
            entity.NoteDate = model.NoteDate;
            entity.DischargeReasonTypeId = model.DischargeReasonTypeId;

            if (entity.Status != (int)DocumentStatus.Final)
            {
                // Само при редакция на чернова е позволена редакцията на статуса.
                entity.Status = model.Status;
            }

            //IEnumerable<StudentEvaluation> currentStudentEvaluations = dischargeDocumentToUpdate.StudentEvaluations;
            //List<EvaluationModel> evaluationsToUpdateModels = model.Evaluations.ToList();
            //SetStudentEvaluations(dischargeDocumentToUpdate, currentStudentEvaluations, evaluationsToUpdateModels);

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            // По някава причина има ситуации, където model.InstitutionId няма стойност.
            // Затова ще я вземем от данните на логнатия потребител.
            if (!model.InstitutionId.HasValue)
            {
                model.InstitutionId = GetInstitutionId(model.InstitutionId);
            }

            int institutionId = model.InstitutionId.Value;

            if (fromDraftToConfirmedStatusChange)
            {
                entity.ConfirmationDate = DateTime.UtcNow;

                if (!entity.CurrentStudentClassId.HasValue)
                {
                    entity.CurrentStudentClassId = await GetCurrentStudentClass(entity.PersonId, institutionId);
                }
                if (await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentSignLOD))
                {
                    var signedDocument = model.Documents?.FirstOrDefault(i => i.NoteFileName.Contains("картон") && i.NoteContents != null && i.NoteContents.Length > 0);
                    if (signedDocument != null)
                    {
                        await AddFinalizationIfNotExists(model.PersonId, entity.SchoolYear, entity.CurrentStudentClassId, Convert.ToBase64String(signedDocument?.NoteContents));
                    }
                }
                await DischargeFromInstitution(model.PersonId, model.InstitutionId, model.DischargeReasonTypeId, null, entity);
                await SaveAsync();
                await _eduStateCacheService.ClearEduStatesForStudent(model.PersonId);

                await UpdateAdmissionPermissionRequest(entity);

                if (entity.InstitutionId.HasValue)
                {
                    await SaveInstitutionChange(institutionId, entity.SchoolYear);
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entity.PersonId);

            if (fromDraftToConfirmedStatusChange)
            {
                await EnrollmentSchoolDelete(entity.PersonId, institutionId);
            }
            //await transaction.RollbackAsync();
        }

        public async Task Delete(int id)
        {
            DischargeDocument entity = await _context.DischargeDocuments
                .Include(d => d.DischargeDocumentDocuments)
                    .ThenInclude(d => d.Document)
                .Include(d => d.StudentEvaluations)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentDischargeDocumentDelete))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = _validator.ValidateDeletion(entity);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            IEnumerable<DischargeDocumentDocument> dischargeDocumentDocumentsToDelete = entity.DischargeDocumentDocuments;
            IEnumerable<Document> documentsToDelete = dischargeDocumentDocumentsToDelete.Select(d => d.Document);

            _context.DischargeDocumentDocuments.RemoveRange(dischargeDocumentDocumentsToDelete);
            _context.Documents.RemoveRange(documentsToDelete);
            _context.DischargeDocuments.Remove(entity);

            await SaveAsync();
        }

        public async Task Confirm(int id)
        {
            DischargeDocument entity = await _context.DischargeDocuments
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentDischargeDocumentUpdate))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = await _validator.ValidateConfirmation(entity);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            entity.Status = (int)DocumentStatus.Final;
            entity.ConfirmationDate = DateTime.UtcNow;

            if (!entity.CurrentStudentClassId.HasValue)
            {
                entity.CurrentStudentClassId = await GetCurrentStudentClass(entity.PersonId, _userInfo.InstitutionID ?? int.MinValue);
            }

            DischargeDocumentModel model = new DischargeDocumentModel
            {
                PersonId = entity.PersonId,
                NoteNumber = entity.NoteNumber,
                NoteDate = entity.NoteDate,
                InstitutionId = entity.InstitutionId,
                DischargeReasonTypeId = entity.DischargeReasonTypeId
            };

            // По някава причина има ситуации, където entity.InstitutionId няма стойност.
            // Затова ще я вземем от данните на логнатия потребител.
            if (!entity.InstitutionId.HasValue)
            {
                entity.InstitutionId = GetInstitutionId(entity.InstitutionId);
            }

            using var transaction = _context.Database.BeginTransaction();

            await AddFinalizationIfNotExists(model.PersonId, entity.SchoolYear, entity.CurrentStudentClassId, null);
            await DischargeFromInstitution(model.PersonId, model.InstitutionId, model.DischargeReasonTypeId, null, entity);
            await SaveAsync();

            await UpdateAdmissionPermissionRequest(entity);

            await SaveInstitutionChange(entity.InstitutionId.Value, entity.SchoolYear);

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entity.PersonId);

            // if the institutionId is null
            await EnrollmentSchoolDelete(entity.PersonId, GetInstitutionId(entity.InstitutionId));
            //await transaction.RollbackAsync();
        }

        public async Task<int?> GetCurrentStudentClass(int personId, int institutionId, int? selectedStudentClassId = null)
        {
            if (selectedStudentClassId.HasValue)
            {
                // От SPA-то идва избран StudentClass.Id
                return selectedStudentClassId;
            }

            // Подреждаме по позиция и разчитаме да има с 3 и да върнем Id-то на този StudentClass
            var sc = await _context.StudentClasses.AsNoTracking()
                .Where(x => x.IsCurrent && x.PersonId == personId && x.InstitutionId == institutionId)
                .OrderBy(x => x.PositionId)
                .Select(x => new { x.Id, x.PositionId })
                .FirstOrDefaultAsync();

            // Todo: по тип на институцията трябва да се определя StudentClass с каква позиция трябва да връщаме

            return sc?.Id;
        }
    }
}
