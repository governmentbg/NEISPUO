using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.Enums;
using MON.Services.Infrastructure.Validators;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using MON.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class AdmissionDocumentService : MovementDocumentBaseService<AdmissionDocumentService>, IAdmissionDocumentService
    {
        private readonly StudentClassValidationContext _studentClassValidator;
        private readonly AdmissionDocumentValidator _validator;
        private readonly EduStateCacheService _eduStateCacheService;

        public AdmissionDocumentService(DbServiceDependencies<AdmissionDocumentService> dependencies,
            MovementDocumentServiceDependencies<AdmissionDocumentService> movementDocumentServiceDependencies,
            StudentClassValidationContext studentClassValidator,
            AdmissionDocumentValidator validator,
            EduStateCacheService eduStateCacheService)
            : base(dependencies, movementDocumentServiceDependencies)
        {
            _studentClassValidator = studentClassValidator;
            _validator = validator;
            _eduStateCacheService = eduStateCacheService;
        }

        #region Private members
        private async Task ProcessAddedDocs(AdmissionDocumentModel model, AdmissionDocument entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.AdmissionDocumentDocuments.Add(new AdmissionDocumentDocument
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(AdmissionDocumentModel model, AdmissionDocument admissionDocument)
        {
            if (model.Documents == null || !model.Documents.Any() || admissionDocument == null) return;

            HashSet<int> docIdsToDelete = model.Documents
                .Where(x => x.Id.HasValue && x.Deleted == true)
                .Select(x => x.Id.Value).ToHashSet();

            await _context.AdmissionDocumentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }

        private async Task UpdateAdmissionPermissionRequest(AdmissionDocument entity, int? eduStateId)
        {
            List<AdmissionPermissionRequest> admissionPermissionRequests = await _context.AdmissionPermissionRequests
                .Where(x => x.PersonId == entity.PersonId
                    && x.RequestingInstitutionId == entity.InstitutionId
                    && x.AdmissionDocumentId == null
                    && x.IsPermissionGranted)
                .ToListAsync();

            foreach (AdmissionPermissionRequest admissionPermissionRequest in admissionPermissionRequests)
            {
                admissionPermissionRequest.AdmissionDocument = entity;

                var relocationDocument = await _context.RelocationDocuments
                    .Where(x => x.PersonId == entity.PersonId
                        && x.Status == (int)DocumentStatus.Final
                        && x.SendingInstitutionId == admissionPermissionRequest.AuthorizingInstitutionId
                        && x.CreateDate >= admissionPermissionRequest.CreateDate
                        )
                    .Select(x => new { x.Id })
                    .FirstOrDefaultAsync();

                var dischargeDocument = await _context.DischargeDocuments
                    .Where(x => x.PersonId == entity.PersonId
                        && x.Status == (int)DocumentStatus.Final
                        && x.InstitutionId == admissionPermissionRequest.AuthorizingInstitutionId
                        && x.CreateDate >= admissionPermissionRequest.CreateDate
                        )
                    .Select(x => new { x.Id })
                    .FirstOrDefaultAsync();

                admissionPermissionRequest.RelocationDocumentId = relocationDocument?.Id;
                admissionPermissionRequest.DischargeDocumentId = dischargeDocument?.Id;
                admissionPermissionRequest.SecondEducationalStateId = eduStateId;
            }
        }

        private async Task UpdateRefugeeChildApplication(AdmissionDocument entity)
        {
            if (entity == null) return;

            List<ApplicationChild> refugeeChildApplications = await _context.ApplicationChildren
                .Where(x => x.PersonId != null
                    && x.PersonId == entity.PersonId
                    && x.Status == (int)ApplicationStatusEnum.Completed
                    && x.AdmissionDocumentId == null
                    && x.InstitutionId != null)
                .ToListAsync();

            if (refugeeChildApplications.Any())
            {
                refugeeChildApplications.ForEach(c => c.AdmissionDocumentId = entity.Id);
                await SaveAsync();

                InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(entity.InstitutionId);
                await _signalRNotificationService.RefugeeEnrolledInInstitution(entity.PersonId, entity.InstitutionId, institution?.RegionId);
            }
        }

        private async Task<RelocationDocumentViewModel> GetRelocationDocument(int id)
        {
            return await _context.RelocationDocuments
                .Where(r => r.Id == id)
                .Select(r => new RelocationDocumentViewModel
                {
                    Id = r.Id,
                    PersonId = r.PersonId,
                    NoteDate = r.NoteDate,
                    NoteNumber = r.NoteNumber,
                    Status = r.Status,
                    RUOOrderNumber = r.RuoorderNumber,
                    RUOOrderDate = r.RuoorderDate,
                    CurrentStudentClassId = r.CurrentStudentClassId,
                    CurrentStudentClassName = r.CurrentStudentClass.Class.ClassName,
                    InstitutionId = r.HostInstitutionId,
                    InstitutionName = r.HostInstitution.Name,
                    Documents = r.RelocationDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    AdmissionDocumentModels = r.AdmissionDocuments
                        .Select(a => new AdmissionDocumentGeneralModel
                        {
                            Id = a.Id,
                            NoteNumber = a.NoteNumber,
                            Status = a.Status
                        })
                }).FirstOrDefaultAsync();
        }
        #endregion

        public async Task<AdmissionDocumentViewModel> GetById(int id)
        {

            AdmissionDocumentViewModel model = await _context.AdmissionDocuments
                .Where(a => a.Id == id)
                .Select(a => new AdmissionDocumentViewModel
                {
                    Id = a.Id,
                    PersonId = a.PersonId,
                    NoteDate = a.NoteDate,
                    NoteNumber = a.NoteNumber,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Status = a.Status,
                    Position = a.PositionId,
                    RelocationDocumentId = a.RelocationDocumentId,
                    HasHealthStatusDocument = a.HasHealthStatusDocument,
                    HasImmunizationStatusDocument = a.HasImmunizationStatusDocument,
                    InstitutionId = a.InstitutionId,
                    InstitutionName = $"{a.InstitutionId}.{a.InstitutionSchoolYear.Name} гр./с.{a.InstitutionSchoolYear.Town.Name} общ.{a.InstitutionSchoolYear.Town.Municipality.Name} обл.{a.InstitutionSchoolYear.Town.Municipality.Region.Name}",
                    AdmissionReasonTypeId = a.AdmissionReasonType.Id,
                    AdmissionReasonTypeName = a.AdmissionReasonType.Name,
                    Documents = a.AdmissionDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    CreatedBySysUserId = a.CreatedBySysUserId

                }).SingleOrDefaultAsync();

            model.IsReferencedInStudentClass = await _context.StudentClasses
                      .Where(sc => sc.AdmissionDocumentId == id)
                      .Select(sc => sc.AdmissionDocumentId)
                      .SingleOrDefaultAsync() != null;

            if (model != null)
            {
                // Методът се използва при Details и Edit
                if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead)
                    && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentAdmissionDocumentUpdate))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                model.RelocationDocument = model.RelocationDocumentId != null ? await GetRelocationDocument(model.RelocationDocumentId.Value) : null;
                model.PositionName = ((PositionType)model.Position).GetEnumDescription();
                model.StatusName = ((DocumentStatus)model.Status).GetEnumDescription();
            }

            return model;
        }

        public async Task<IEnumerable<AdmissionDocumentViewModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<AdmissionDocumentViewModel> documents = await _context.AdmissionDocuments
                .AsNoTracking()
                .Where(a => a.PersonId == personId)
                .OrderByDescending(x => x.Id)
                .Select(a => new AdmissionDocumentViewModel
                {
                    Id = a.Id,
                    PersonId = personId,
                    NoteDate = a.NoteDate,
                    NoteNumber = a.NoteNumber,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Status = a.Status,
                    RelocationDocumentId = a.RelocationDocumentId,
                    Position = a.PositionId,
                    InstitutionId = a.InstitutionId,
                    InstitutionName = a.InstitutionSchoolYear.Abbreviation,
                    AdmissionReasonTypeId = a.AdmissionReasonType.Id,
                    AdmissionReasonTypeName = a.AdmissionReasonType.Name,
                    SchoolYearName = a.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    Documents = a.AdmissionDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    CreatedBySysUserId = a.CreatedBySysUserId,
                    UsedInClassEnrollment = a.StudentClasses.Any()
                })
                .ToListAsync();

            Dictionary<int, string> validationLog = new Dictionary<int, string>();

            // Id на последния "Потвърден" запис в AdmissionDocument за дадения ученик
            int? maxStudentClassId = documents.Where(x => x.Status == (int)DocumentStatus.Final).Max(x => x.Id);

            foreach (var doc in documents)
            {
                validationLog[doc.Id ?? 0] = $"UsedInClassEnrollment: {doc.UsedInClassEnrollment} (StudentClasses.Any())";

                var (canBeModified, canBeModifiedError) = _validator.CanBeModified(doc.Status, doc.InstitutionId); // В UI-a определя дали да са видими бутоните за редакция, потвърждаване или изтриване
                validationLog[doc.Id ?? 0] += $"{Environment.NewLine}CanBeModified: {canBeModified} with error: {canBeModifiedError}";

                var (showInitialEntollmentButtonCheck, showInitialEntollmentButtonCheckError) = await _studentClassValidator.ShowInitialEntollmentButton(doc); // В UI-а определя дали да се покаже бутона за запис в клас
                bool canBeEnrolled = showInitialEntollmentButtonCheck
                    && doc.Id == maxStudentClassId // Бутонът за запис в паралелка е видим само ако е последен запис в StudentClass
                    && !doc.UsedInClassEnrollment;

                validationLog[doc.Id ?? 0] += $"{Environment.NewLine}CanBeEnrolled: {canBeEnrolled} " +
                    $"( UsedInClassEnrollment: {doc.UsedInClassEnrollment} OR ShowInitialEntollmentButtonCheck: {showInitialEntollmentButtonCheck}) with error: {canBeModifiedError}";

                doc.StatusName = ((DocumentStatus)doc.Status).GetEnumDescription();
                doc.CanBeModified = canBeModified;
                doc.CanBeEnrolled = canBeEnrolled;
            }

            _logger.LogInformation(JsonSerializer.Serialize(validationLog));

            return documents;
        }

        public async Task<bool> CheckForAdmissionDocumentInTheSameInstitutionAsync(int personId, int institutionId)
        {
            return await _context.EducationalStates
                .AsNoTracking()
                .AnyAsync(e => e.PersonId == personId && e.InstitutionId == institutionId && e.PositionId == (int)PositionType.Student);
        }

        public async Task<bool> CheckForExistingAdmissionDocumentAsync(int personId, int institutionId)
        {
            return await _context.AdmissionDocuments
                .AsNoTracking()
                .AnyAsync(d => d.PersonId == personId && d.Status == (int)DocumentStatus.Draft && d.InstitutionId != institutionId);
        }

        public async Task Create(AdmissionDocumentModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentAdmissionDocumentCreate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ApiValidationResult validationResult = await _validator.ValidateCreation(model);
            if (!validationResult.IsValid)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.GetErrrorFromMessages());
            }

            AdmissionDocument entry = new AdmissionDocument
            {
                NoteNumber = model.NoteNumber,
                NoteDate = model.NoteDate,
                AdmissionDate = model.AdmissionDate,
                DischargeDate = model.DischargeDate,
                InstitutionId = model.InstitutionId,
                AdmissionReasonTypeId = model.AdmissionReasonTypeId,
                Status = model.Status,
                PersonId = model.PersonId,
                RelocationDocumentId = model.RelocationDocumentId,
                PositionId = model.Position,
                SchoolYear = await _institutionService.GetCurrentYear(model.InstitutionId),
                HasHealthStatusDocument = model.HasHealthStatusDocument,
                HasImmunizationStatusDocument = model.HasImmunizationStatusDocument
            };

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entry);

            // Когато entry.Status == (int)DocumentStatus.Final, трябва да го отпишем от всички класове, за които IsCurrent == true
            // за дадена инстиуция.
            if (!entry.IsDraft)
            {
                entry.ConfirmationDate = DateTime.UtcNow;

                int eduStateId = await UpdateEduStateOnEnrollment(entry.PersonId, entry.InstitutionId, entry.PositionId);
                await UpdateAdmissionPermissionRequest(entry, eduStateId);
                await SaveInstitutionChange(model.InstitutionId, entry.SchoolYear);
            }

            _context.AdmissionDocuments.Add(entry);

            await SaveAsync();

            if (!entry.IsDraft)
            {
                await ProcessLodFinalization(entry);

                await EnrollmentSchoolCreate(entry.PersonId, entry.InstitutionId);

                // Трябва ни entry.Id, затова го викаме след SaveAsync()
                await UpdateRefugeeChildApplication(entry);
            }

            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entry.PersonId);
        }

        private async Task ProcessLodFinalization(AdmissionDocument entry)
        {
            // Проверка за наличие на подписан ЛОД
            var lodFinalization = await _context.Lodfinalizations.FirstOrDefaultAsync(i => i.PersonId == entry.PersonId && i.SchoolYear == entry.SchoolYear);
            if (lodFinalization != null)
            {
                // Последен документ за отписване/ преместване
                var movementDocument = (
                        await (
                        from d in _context.DischargeDocuments.AsNoTracking()
                        where d.Status == (int)DocumentStatus.Final && d.PersonId == entry.PersonId
                        select new { Id = d.Id, Type = 0, CreateDate = d.CreateDate }).ToListAsync())
                    .Union(
                        await (
                        from d in _context.RelocationDocuments.AsNoTracking()
                        where d.Status == (int)DocumentStatus.Final && d.PersonId == entry.PersonId
                        select new { Id = d.Id, Type = 1, CreateDate = d.CreateDate }).ToListAsync()
                    )
                    .Union(
                        await (
                        from d in _context.AdmissionDocuments.AsNoTracking()
                        where d.Status == (int)DocumentStatus.Final && d.Id != entry.Id && d.PersonId == entry.PersonId
                        select new { Id = d.Id, Type = 2, CreateDate = d.CreateDate }).ToListAsync()
                    )
                    .OrderByDescending(i => i.CreateDate)
                    .FirstOrDefault();

                // Последният потвърден документ за движение е отписване/ преместване
                if (movementDocument != null && (movementDocument.Type == 0))
                {
                    _context.Lodfinalizations.Remove(lodFinalization);
                    await SaveAsync();
                }
            }
        }

        public async Task Update(AdmissionDocumentModel model)
        {
            AdmissionDocument entity = await _context.AdmissionDocuments
                .Include(d => d.AdmissionDocumentDocuments)
                    .ThenInclude(d => d.Document)
                .Include(d => d.RelocationDocument)
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentAdmissionDocumentUpdate))
            {
                // Ako намаме права за редакция на документите за записване на ученика
                // проверяваме за налични за четене и за съвпадение на институцията на док. с тази на логнатия потребител.
                // Това се прави с цел редакция на потвърдени собствени документи.
                if (!_userInfo.InstitutionID.HasValue || entity.InstitutionId != _userInfo.InstitutionID
                    || !await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            bool fromDraftToConfirmedStatusChange = entity.IsDraft && entity.Status != model.Status;

            ApiValidationResult validationResult = await _validator.ValidateUpdate(entity, model, fromDraftToConfirmedStatusChange);
            if (!validationResult.IsValid)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.GetErrrorFromMessages());
            }

            entity.NoteNumber = model.NoteNumber;
            entity.NoteDate = model.NoteDate;
            entity.AdmissionDate = model.AdmissionDate;
            entity.DischargeDate = model.DischargeDate;
            entity.AdmissionReasonTypeId = model.AdmissionReasonTypeId;
            entity.RelocationDocumentId = model.RelocationDocumentId;
            entity.HasHealthStatusDocument = model.HasHealthStatusDocument;
            entity.HasImmunizationStatusDocument = model.HasImmunizationStatusDocument;

            if (entity.Status != (int)DocumentStatus.Final)
            {
                // Само при редакция на чернова е позволена редакцията на
                // позицията, институцията и статуса.
                entity.Status = model.Status;
                entity.InstitutionId = model.InstitutionId;
                entity.PositionId = model.Position;
            }

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            if (fromDraftToConfirmedStatusChange)
            {
                entity.ConfirmationDate = DateTime.UtcNow;

                int eduStateId = await UpdateEduStateOnEnrollment(entity.PersonId, entity.InstitutionId, entity.PositionId);
                await UpdateAdmissionPermissionRequest(entity, eduStateId);
                await UpdateRefugeeChildApplication(entity);
                await SaveInstitutionChange(model.InstitutionId, entity.SchoolYear);
            }

            _context.Update(entity);

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entity.PersonId);

            if (fromDraftToConfirmedStatusChange)
            {
                await ProcessLodFinalization(entity);
                await EnrollmentSchoolCreate(entity.PersonId, entity.InstitutionId);
            }
        }

        public async Task Delete(int documentId)
        {
            AdmissionDocument entity = await _context.AdmissionDocuments
                .Include(d => d.AdmissionDocumentDocuments)
                    .ThenInclude(d => d.Document)
                .SingleOrDefaultAsync(d => d.Id == documentId);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentAdmissionDocumentDelete))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ApiValidationResult validationResult = _validator.ValidateDeletion(entity);
            if (!validationResult.IsValid)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.GetErrrorFromMessages());
            }

            IEnumerable<AdmissionDocumentDocument> admissionDocumentDocumentsToDelete = entity.AdmissionDocumentDocuments;
            IEnumerable<Document> documentsToDelete = admissionDocumentDocumentsToDelete.Select(d => d.Document);

            _context.AdmissionDocumentDocuments.RemoveRange(admissionDocumentDocumentsToDelete);
            _context.Documents.RemoveRange(documentsToDelete);
            _context.AdmissionDocuments.Remove(entity);

            await SaveAsync();
        }

        public async Task Confirm(int id)
        {
            AdmissionDocument entity = await _context.AdmissionDocuments
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentAdmissionDocumentUpdate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ApiValidationResult validationResult = await _validator.ValidateConfirmation(entity);
            if (!validationResult.IsValid)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.GetErrrorFromMessages());
            }

            entity.Status = (int)DocumentStatus.Final;
            entity.ConfirmationDate = DateTime.UtcNow;

            AdmissionDocumentModel model = new AdmissionDocumentModel
            {
                PersonId = entity.PersonId,
                NoteNumber = entity.NoteNumber,
                NoteDate = entity.NoteDate,
                Position = entity.PositionId,
                RelocationDocumentId = entity.RelocationDocumentId,
                InstitutionId = entity.InstitutionId
            };

            using var transaction = _context.Database.BeginTransaction();

            int eduStateId = await UpdateEduStateOnEnrollment(entity.PersonId, entity.InstitutionId, entity.PositionId);
            await UpdateAdmissionPermissionRequest(entity, eduStateId);
            await UpdateRefugeeChildApplication(entity);
            await SaveInstitutionChange(model.InstitutionId, entity.SchoolYear);

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entity.PersonId);


            await ProcessLodFinalization(entity);
            await EnrollmentSchoolCreate(entity.PersonId, entity.InstitutionId);


            //await transaction.RollbackAsync();
        }

        public Task<List<AdmissionDocumentViewModel>> GetListForRelocationDocument(int relocationDocumentId)
        {
            return _context.AdmissionDocuments
                 .AsNoTracking()
                 .Where(x => x.RelocationDocumentId == relocationDocumentId)
                 .OrderByDescending(x => x.NoteDate)
                 .Select(x => new AdmissionDocumentViewModel
                 {
                     NoteNumber = x.NoteNumber,
                     NoteDate = x.NoteDate,
                     InstitutionName = x.InstitutionSchoolYear.Name
                 })
                 .ToListAsync();
        }
    }
}
