using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.StudentModels.StoredProcedures;
using MON.Services.Infrastructure.Validators;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using MON.Shared.Extensions;
using MON.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using MON.Report.Model;
using RelocationDocument = MON.DataAccess.RelocationDocument;
using MON.Models.StudentModels.Lod;
using System.Threading;
using MON.Models.StudentModels;
using DocumentFormat.OpenXml.Vml.Office;

namespace MON.Services.Implementations
{
    public class RelocationDocumentService : MovementDocumentBaseService<RelocationDocumentService>, IRelocationDocumentService
    {
        private readonly ISchoolBooksService _schoolBooksService;
        private readonly RelocationDocumentValidator _validator;
        private readonly EduStateCacheService _eduStateCacheService;
        private readonly IAppConfigurationService _configurationService;
        private readonly ILodAssessmentService _lodAssessmentService;
        private readonly IStudentService _studentService;

        public RelocationDocumentService(DbServiceDependencies<RelocationDocumentService> dependencies,
            MovementDocumentServiceDependencies<RelocationDocumentService> movementDocumentServiceDependencies,
            ISchoolBooksService schoolBooksService,
            RelocationDocumentValidator validator,
            EduStateCacheService eduStateCacheService,
            ILodAssessmentService lodAssessmentService,
            IStudentService studentService,
            IAppConfigurationService configurationService)
            : base(dependencies, movementDocumentServiceDependencies)
        {
            _schoolBooksService = schoolBooksService;
            _validator = validator;
            _eduStateCacheService = eduStateCacheService;
            _configurationService = configurationService;
            _lodAssessmentService = lodAssessmentService;
            _studentService = studentService;
        }

        #region Private members

        private async Task ProcessAddedDocs(RelocationDocumentModel model, RelocationDocument entity, CancellationToken cancellationToken = default)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType, cancellationToken);
                entity.RelocationDocumentDocuments.Add(new RelocationDocumentDocument
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(RelocationDocumentModel model, RelocationDocument relocationDocument, CancellationToken cancellationToken = default)
        {
            if (model.Documents == null || !model.Documents.Any() || relocationDocument == null) return;

            HashSet<int> docIdsToDelete = model.Documents
                .Where(x => x.Id.HasValue && x.Deleted == true)
                .Select(x => x.Id.Value).ToHashSet();

            await _context.RelocationDocumentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync(cancellationToken);
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync(cancellationToken);
        }

        /// <summary>
        /// Когато ни е поискан и ние сме освободоли за запис ученик
        /// следва след като го преместим да запишем ИД ном. на док за преместване
        /// в искането за запис.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private async Task UpdateAdmissionPermissionRequest(RelocationDocument entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(RelocationDocument)));
            }

            List<AdmissionPermissionRequest> admissionPermissionRequests = await _context.AdmissionPermissionRequests
                .Where(x => x.PersonId == entity.PersonId
                    && x.AuthorizingInstitutionId == entity.SendingInstitutionId
                    && x.RelocationDocumentId == null
                    && x.IsPermissionGranted)
                .ToListAsync(cancellationToken);

            foreach (AdmissionPermissionRequest admissionPermissionRequest in admissionPermissionRequests)
            {
                admissionPermissionRequest.RelocationDocument = entity;
            }
        }

        private async Task FixMissingCurrentStudentClass(RelocationDocument entity)
        {
            if (entity == null || entity.CurrentStudentClassId.HasValue) return;
 
            List<StudentClassViewModel> mainStudentClasses = await _studentService.GetMainStudentClasses(entity.PersonId, entity.SendingInstitutionSchoolYear, true);

            int? currentStudentClassId    = mainStudentClasses
                .OrderByDescending(x => x.IsCurrent)
                .ThenBy(x => x.Position)
                .ThenByDescending(x => x.Id)
                .ThenByDescending(x => x.BasicClassId)
                .FirstOrDefault()?.Id;
            // Проверяваме дали съществува този StudentClass, тъй като функцията гледа в history данни
            var dbStudentClass = await _context.StudentClasses.FirstOrDefaultAsync(i => i.Id == currentStudentClassId);

            entity.CurrentStudentClassId = dbStudentClass?.Id;
        }
        #endregion

        public async Task<RelocationDocumentModel> GetById(int id, CancellationToken cancellationToken = default)
        {
            RelocationDocumentModel model = await _context.RelocationDocuments
                .Where(r => r.Id == id)
                .Select(r => new RelocationDocumentModel
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
                    SendingInstitution = r.SendingInstitution.Name,
                    RelocationReasonTypeId = r.RelocationReasonTypeId,
                    DischargeDate = r.DischargeDate,
                    Documents = r.RelocationDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                }).SingleOrDefaultAsync(cancellationToken);

            if (model == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            // Методът се използва при Details и Edit
            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentUpdate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<RelocationDocumentViewModel>> GetByPersonId(int personId, CancellationToken cancellationToken = default)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentRelocationDocumentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            string docToBasicClassStr = await _configurationService.GetValueByKey("RelocationDocumentToBasicClass");
            List<RelocationDocumentToBasicClassConfig> docToBasicClassConfig = string.IsNullOrWhiteSpace(docToBasicClassStr)
               ? new List<RelocationDocumentToBasicClassConfig>()
               : JsonSerializer.Deserialize<List<RelocationDocumentToBasicClassConfig>>(docToBasicClassStr, new JsonSerializerOptions
               {
                   PropertyNameCaseInsensitive = true
               });

            var documents = await _context.RelocationDocuments
                .AsNoTracking()
                .Where(r => r.PersonId == personId)
                .Select(r => new RelocationDocumentViewModel
                {
                    Id = r.Id,
                    PersonId = personId,
                    NoteDate = r.NoteDate,
                    NoteNumber = r.NoteNumber,
                    Status = r.Status,
                    RUOOrderNumber = r.RuoorderNumber,
                    RUOOrderDate = r.RuoorderDate,
                    DischargeDate = r.DischargeDate,
                    CurrentStudentClassId = r.CurrentStudentClassId,
                    CurrentStudentClassName = r.CurrentStudentClass.Class.ClassName,
                    CurrentStudentClassBasicClassId = r.CurrentStudentClass.BasicClassId,
                    CurrentStudentClassBasicClassName = r.CurrentStudentClass.BasicClass.Name,
                    SendingInstitution = r.SendingInstitution.Abbreviation,
                    SendingInstitutionId = r.SendingInstitutionId,
                    InstitutionId = r.HostInstitutionId,
                    InstitutionName = r.HostInstitution.Abbreviation,
                    RelocationReasonType = r.RelocationReasonType.Name,
                    SchoolYearName = r.SendingInstitution.SchoolYearNavigation.Name,
                    Documents = r.RelocationDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    AdmissionDocumentModels = r.AdmissionDocuments
                        .Select(a => new AdmissionDocumentGeneralModel
                        {
                            Id = a.Id,
                            NoteNumber = a.NoteNumber,
                            Status = a.Status,
                            InstitutionName = a.InstitutionSchoolYear.Name
                        })
                })
                .OrderByDescending(x => x.NoteDate)
                .ToListAsync(cancellationToken);

            foreach (var doc in documents)
            {
                doc.StatusName = ((DocumentStatus)doc.Status).GetEnumDescription();
                doc.CanBeModified = _validator.CanBeModified(doc.Status, doc.SendingInstitutionId ?? default);
                doc.ReportPath = docToBasicClassConfig.FirstOrDefault(x => x.BasicClasses != null && x.BasicClasses.Contains(doc.CurrentStudentClassBasicClassId))?.DocumentPath;
                // Определя видимостта на бутона за добавяне(импорт) на оценки в грида с документи за преместване
                doc.CanAddEvaluations = doc.ReportPath.IsNullOrWhiteSpace() || !doc.ReportPath.Contains("RelocationDocument_3_9`");
            }

            return documents;
        }

        public async Task<IEnumerable<RelocationDocumentOptionsModel>> GetRelocationDocumentOptionsByPerson(int personId, CancellationToken cancellationToken = default)
        {
            return await _context.RelocationDocuments
                .Where(x => x.PersonId == personId && x.Status == (int)DocumentStatus.Final)
                .Select(r => new RelocationDocumentOptionsModel
                {
                    Id = r.Id,
                    NoteDate = r.NoteDate,
                    NoteNumber = r.NoteNumber,
                }).ToListAsync(cancellationToken);
        }

        public async Task Create(RelocationDocumentModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentCreate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            model.SendingInstitutionId ??= _userInfo.InstitutionID;

            ApiValidationResult validationResult = await _validator.ValidateCreation(model);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }



            RelocationDocument entry = new RelocationDocument
            {
                NoteNumber = model.NoteNumber,
                NoteDate = model.NoteDate,
                HostInstitutionId = model.InstitutionId,
                HostInstitutionSchoolYear = model.InstitutionId.HasValue
                    ? await _institutionService.GetCurrentYear(model.InstitutionId)
                    : (short?)null,
                Status = model.Status,
                PersonId = model.PersonId,
                CurrentStudentClassId = model.CurrentStudentClassId == 0 ? null : model.CurrentStudentClassId,
                RuoorderNumber = model.RUOOrderNumber,
                RuoorderDate = model.RUOOrderDate,
                RelocationReasonTypeId = model.RelocationReasonTypeId,
                DischargeDate = model.DischargeDate,
                SendingInstitutionId = model.SendingInstitutionId,
                SendingInstitutionSchoolYear = await _institutionService.GetCurrentYear(model.SendingInstitutionId)
            };

            await FixMissingCurrentStudentClass(entry);

            using var transaction = await _context.Database.BeginTransactionAsync();
            await ProcessAddedDocs(model, entry);
            _context.RelocationDocuments.Add(entry);

            // Когато entry.Status == (int)DocumentStatus.Final, трябва да го отпишем от всички класове, за които IsCurrent == true
            // за дадена инстиуция.
            if (!entry.IsDraft)
            {
                entry.ConfirmationDate = DateTime.UtcNow;

                await DischargeFromInstitution(entry.PersonId, entry.SendingInstitutionId, entry.RelocationReasonTypeId, entry, null);
                await SaveAsync();
                await _eduStateCacheService.ClearEduStatesForStudent(model.PersonId);

                await UpdateAdmissionPermissionRequest(entry);

                //Запазваме промените по институцията ако имаме id на приемащата институция и учебна година 
                if (entry.HostInstitutionId.HasValue && entry.HostInstitutionSchoolYear.HasValue)
                {
                    await SaveInstitutionChange(entry.HostInstitutionId.Value, entry.HostInstitutionSchoolYear.Value);
                }

                //Запазваме промените по институцията ако имаме id на изпращата институция и учебна година 
                if (entry.SendingInstitutionId.HasValue)
                {
                    await SaveInstitutionChange(entry.SendingInstitutionId.Value, entry.SendingInstitutionSchoolYear);
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entry.PersonId);

            if (!entry.IsDraft)
            {
                await EnrollmentSchoolDelete(entry.PersonId, entry.SendingInstitutionId ?? default);
            }
        }

        public async Task Update(RelocationDocumentModel model)
        {
            RelocationDocument entity = await _context.RelocationDocuments
                .Include(x => x.CurrentStudentClass).ThenInclude(x => x.Class)
                .Include(d => d.RelocationDocumentDocuments)
                    .ThenInclude(d => d.Document)
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentRelocationDocumentUpdate))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = await _validator.ValidateUpdate(entity, model);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            bool hasToDischargeFromInstitution = entity.IsDraft && entity.Status != model.Status;

            entity.NoteNumber = model.NoteNumber;
            entity.NoteDate = model.NoteDate;
            entity.HostInstitutionId = model.InstitutionId;
            entity.HostInstitutionSchoolYear = model.InstitutionId.HasValue
                ? await _institutionService.GetCurrentYear(model.InstitutionId.Value)
                : (short?)null;
            entity.Status = model.Status;
            entity.RuoorderNumber = model.RUOOrderNumber;
            entity.RuoorderDate = model.RUOOrderDate;
            entity.CurrentStudentClassId = model.CurrentStudentClassId == 0 ? null : model.CurrentStudentClassId;
            entity.RelocationReasonTypeId = model.RelocationReasonTypeId;
            entity.DischargeDate = model.DischargeDate;

            await FixMissingCurrentStudentClass(entity);

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);
            if (hasToDischargeFromInstitution)
            {
                entity.ConfirmationDate = DateTime.UtcNow;

                await DischargeFromInstitution(entity.PersonId, entity.SendingInstitutionId, entity.RelocationReasonTypeId, entity, null);
                await SaveAsync();
                await _eduStateCacheService.ClearEduStatesForStudent(model.PersonId);

                await UpdateAdmissionPermissionRequest(entity);

                //Запазваме промените по институцията ако имаме id на приемащата институция и учебна година 
                if (entity.HostInstitutionId.HasValue && entity.HostInstitutionSchoolYear.HasValue)
                {
                    await SaveInstitutionChange(entity.HostInstitutionId.Value, entity.HostInstitutionSchoolYear.Value);
                }

                //Запазваме промените по институцията ако имаме id на изпращата институция и учебна година 
                if (entity.SendingInstitutionId.HasValue)
                {
                    await SaveInstitutionChange(entity.SendingInstitutionId.Value, entity.SendingInstitutionSchoolYear);
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entity.PersonId);

            if (hasToDischargeFromInstitution)
            {
                await EnrollmentSchoolDelete(entity.PersonId, entity.SendingInstitutionId ?? default);
            }
        }

        public async Task Delete(int documentId)
        {
            RelocationDocument entity = await _context.RelocationDocuments
                .Include(d => d.RelocationDocumentDocuments)
                    .ThenInclude(d => d.Document)
                .Include(d => d.StudentEvaluations)
                .Include(d => d.RelocationDocumentCurrentTermGrades)
                .SingleOrDefaultAsync(d => d.Id == documentId);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentDelete))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ApiValidationResult validationResult = _validator.ValidateDeletion(entity);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            IEnumerable<StudentEvaluation> studentEvaluationsToDelete = entity.StudentEvaluations;
            _context.StudentEvaluations.RemoveRange(studentEvaluationsToDelete);

            IEnumerable<RelocationDocumentCurrentTermGrade> currentTermGradesToDelete = entity.RelocationDocumentCurrentTermGrades;
            _context.RelocationDocumentCurrentTermGrades.RemoveRange(currentTermGradesToDelete);

            IEnumerable<RelocationDocumentDocument> relocationDocumentDocumentsToDelete = entity.RelocationDocumentDocuments;
            IEnumerable<Document> documentsToDelete = relocationDocumentDocumentsToDelete.Select(d => d.Document);

            _context.RelocationDocumentDocuments.RemoveRange(relocationDocumentDocumentsToDelete);
            _context.Documents.RemoveRange(documentsToDelete);
            _context.RelocationDocuments.Remove(entity);

            await SaveAsync();
        }

        public async Task Confirm(int id)
        {
            RelocationDocument entity = await _context.RelocationDocuments
                .Include(x => x.CurrentStudentClass).ThenInclude(x => x.Class)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentUpdate))
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

            if (!entity.IsDraft) return;

            entity.Status = (int)DocumentStatus.Final;
            entity.ConfirmationDate = DateTime.UtcNow;

            using var transaction = await _context.Database.BeginTransactionAsync();

            await DischargeFromInstitution(entity.PersonId, entity.SendingInstitutionId, entity.RelocationReasonTypeId, entity, null);
            await SaveAsync();

            await UpdateAdmissionPermissionRequest(entity);

            //Запазваме промените по институцията ако имаме id на приемащата институция и учебна година 
            if (entity.HostInstitutionId.HasValue && entity.HostInstitutionSchoolYear.HasValue)
            {
                await SaveInstitutionChange(entity.HostInstitutionId.Value, entity.HostInstitutionSchoolYear.Value);
            }

            //Запазваме промените по институцията ако имаме id на изпращата институция и учебна година 
            if (entity.SendingInstitutionId.HasValue)
            {
                await SaveInstitutionChange(entity.SendingInstitutionId.Value, entity.SendingInstitutionSchoolYear);
            }

            await SaveAsync();
            await transaction.CommitAsync();

            await _signalRNotificationService.StudentClassEduStateChange(entity.PersonId);

            await EnrollmentSchoolDelete(entity.PersonId, entity.SendingInstitutionId ?? default);
        }

        public async Task<StudentTermGradeViewModel> GetStudentCurrentTermGradesAsync(int relocationDocumentId, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken = default)
        {
            var relocationDocument = await _context.RelocationDocuments
                .Where(x => x.Id == relocationDocumentId)
                .Select(x => new
                {
                    x.PersonId,
                    x.CurrentStudentClass.BasicClassId,
                    x.CurrentStudentClass.SchoolYear,
                    x.SendingInstitutionSchoolYear
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (!await _authorizationService.HasPermissionForStudent(relocationDocument?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentRelocationDocumentRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (relocationDocument == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(relocationDocument)));
            }

            List<LodAssessmentCurriculumPartModel> partsCurrentGrades = await _lodAssessmentService.GetPersonCurrentAssessments(relocationDocument.PersonId, relocationDocument.BasicClassId, relocationDocument.SchoolYear, filterForCurrentInstitution, filterForCurrentSchoolBook, cancellationToken);

            List<StudentTermGradesModel> grades = new List<StudentTermGradesModel>();

            foreach (var p in partsCurrentGrades)
            {
                if (p.SubjectAssessments.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var subject in p.SubjectAssessments)
                {
                    grades.Add(new StudentTermGradesModel
                    {
                        SubjectID = subject.SubjectId,
                        SubjectName = subject.SubjectName,
                        SchoolYear = subject.SchoolYear,
                        CurriculumID = subject.CurriculumId ?? 0,
                        CurriculumPartID = p.CurriculumPartId,
                        CurriculumPart = p.CurriculumPart,
                        CurriculumPartName = p.CurriculumPartName,
                        SubjectTypeID = subject.SubjectTypeId,
                        SubjectTypeName = subject.SubjectTypeName,
                        Term = 1,
                        IsLoadedFromSchoolbook = true,
                        SortOrder = subject.SortOrder,
                        GradesString = string.Join(',', subject.FirstTermAssessments.Select(s => s.GradeText)),
                        Grades = subject.FirstTermAssessments
                    });

                    grades.Add(new StudentTermGradesModel
                    {
                        SubjectID = subject.SubjectId,
                        SubjectName = subject.SubjectName,
                        SchoolYear = subject.SchoolYear,
                        CurriculumID = subject.CurriculumId ?? 0,
                        CurriculumPartID = p.CurriculumPartId,
                        CurriculumPart = p.CurriculumPart,
                        CurriculumPartName = p.CurriculumPartName,
                        SubjectTypeID = subject.SubjectTypeId,
                        SubjectTypeName = subject.SubjectTypeName,
                        Term = 2,
                        IsLoadedFromSchoolbook = true,
                        SortOrder = subject.SortOrder,
                        GradesString = string.Join(',', subject.SecondTermAssessments.Select(s => s.GradeText)),
                        Grades = subject.SecondTermAssessments
                    });
                }
            }

            return new StudentTermGradeViewModel
            {
                TermGrades = grades,
                GradesLoadedFromSchoolbook = true
            };
        }

        public async Task<RelocationDocumentAbsencePrintModel> GetAbsences(int relocationDocumentId, CancellationToken cancellationToken = default)
        {
            var relocationDocument = await _context.RelocationDocuments
                .Where(x => x.Id == relocationDocumentId)
                .Select(x => new
                {
                    x.PersonId,
                    x.SendingInstitutionId,
                    x.SendingInstitutionSchoolYear
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (!await _authorizationService.HasPermissionForStudent(relocationDocument?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentRelocationDocumentRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (relocationDocument == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(relocationDocument)));
            }

            var absences = await _context.VSchoolBooksAbsences
                .Where(x => x.PersonId == relocationDocument.PersonId && x.InstitutionId == relocationDocument.SendingInstitutionId
                    && x.SchoolYear == relocationDocument.SendingInstitutionSchoolYear && x.Term != null)
                .Select(x => new
                {
                    x.Term,
                    x.Excused,
                    x.Unexcused,
                    x.Late
                })
                .ToListAsync(cancellationToken);

            RelocationDocumentAbsencePrintModel absencesModel = new RelocationDocumentAbsencePrintModel
            {
                AbsencesForValidReasonsFirstTerm = absences.Where(x => x.Term != null && x.Term == 1 && x.Excused != null && x.Excused == true).Count(),
                AbsencesForValidReasonsSecondTerm = absences.Where(x => x.Term != null && x.Term == 2 && x.Excused != null && x.Excused == true).Count(),
                AbsencesForInvalidReasonsFirstTerm = absences.Where(x => x.Term != null && x.Term == 1 && x.Unexcused != null && x.Unexcused == true).Count()
                    + absences.Where(x => x.Term != null && x.Term == 1 && x.Late != null && x.Late == true).Count() / 2f,
                AbsencesForInvalidReasonsSecondTerm = absences.Where(x => x.Term != null && x.Term == 2 && x.Unexcused != null && x.Unexcused == true).Count()
                    + absences.Where(x => x.Term != null && x.Term == 2 && x.Late != null && x.Late == true).Count() / 2f
            };

            return absencesModel;
        }

        private async Task<List<StudentEvaluation>> CreateEvaluationsToAdd(List<StudentGradeEvaluationDto> studentGradeEvaluations, int relocationDocumentId, int basicClassId, CancellationToken cancellationToken = default)
        {
            List<StudentEvaluation> studentEvaluationsToAdd = new List<StudentEvaluation>();

            if (studentGradeEvaluations.IsNullOrEmpty())
            {
                return studentEvaluationsToAdd;
            }

            // Оценки групирани по SubjectId.
            // При документ за премстване за 7-ми или 6-ти клас идват и запис и предишни учебни години.
            // 7-ми: за 7-ми, 6-ти и 5-ти.
            // 6-ти: за 6-ти и 5-ти.
            // Всички останали само за учебната година на док. за пресметване.
            Dictionary<(int? CurriculumPartId, int SubjectId), List<StudentGradeEvaluationDto>> gradesDict = studentGradeEvaluations
                .Where(x => x.SchoolYear.HasValue)
                .OrderBy(x => x.CurriculumPartId).ThenBy(x => x.SubjectId)
                .GroupBy(x => new { x.CurriculumPartId, x.SubjectId })
                .ToDictionary(x => (x.Key.CurriculumPartId, x.Key.SubjectId), x => x.ToList());

            Dictionary<int?, int> sortOrders = (await _context.StudentEvaluations
                .Where(x => x.RelocationDocumentId == relocationDocumentId)
                .Select(x => new
                {
                    x.CurriculumPartId,
                    x.SortOrder
                })
                .ToListAsync(cancellationToken))
                .GroupBy(x => x.CurriculumPartId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.SortOrder).ToList().DefaultIfEmpty().Max());

            foreach (KeyValuePair<(int? CurriculumPartId, int SubjectId), List<StudentGradeEvaluationDto>> gradeKvp in gradesDict)
            {
                // Оценка за basicClassId на док. за преместване (оценка за текущата учебна година).
                StudentGradeEvaluationDto currentBasicClassGrade = gradeKvp.Value.FirstOrDefault(x => x.BasicClassId == basicClassId);
                if (currentBasicClassGrade == null)
                {
                    continue;
                }

                if (!sortOrders.Keys.Contains(gradeKvp.Key.CurriculumPartId))
                {
                    sortOrders[gradeKvp.Key.CurriculumPartId] = 0;
                }

                StudentEvaluation gradeToAdd = new StudentEvaluation
                {
                    RelocationDocumentId = relocationDocumentId,
                    SubjectId = gradeKvp.Key.SubjectId,
                    BasicClassId = currentBasicClassGrade.BasicClassId,
                    SchoolYear = currentBasicClassGrade.SchoolYear.Value, // Няма да гръмне. Малко по-горе филтрираме тези, които имат SchoolYear
                    CurriculumPartId = gradeKvp.Key.CurriculumPartId,
                    FirstTermEvaluation = (int?)currentBasicClassGrade.GradeFirstTerm,
                    SecondTermEvaluation = (int?)currentBasicClassGrade.GradeSecondTerm,
                    AnnualEvaluation = (int?)currentBasicClassGrade.FinalGrade,
                    SortOrder = ++sortOrders[gradeKvp.Key.CurriculumPartId],
                };

                switch (basicClassId)
                {
                    case 7:
                        gradeToAdd.OneYearAgoEvaluation = (int?)gradeKvp.Value
                            .FirstOrDefault(x => x.BasicClassId != null && x.BasicClassId == 6)?.FinalGrade;
                        gradeToAdd.TwoYearsAgoEvaluation = (int?)gradeKvp.Value
                            .FirstOrDefault(x => x.BasicClassId != null && x.BasicClassId == 5)?.FinalGrade;

                        break;
                    case 6:
                        gradeToAdd.OneYearAgoEvaluation = (int?)gradeKvp.Value
                           .FirstOrDefault(x => x.BasicClassId != null && x.BasicClassId == 5)?.FinalGrade;

                        break;
                    default:
                        break;
                }

                studentEvaluationsToAdd.Add(gradeToAdd);
            }

            return studentEvaluationsToAdd;
        }

        private List<RelocationDocumentCurrentTermGrade> CreateCurrentGradesToAdd(List<StudentGradeForBasicClassDto> schoolBookCurrentGrades, int relocationDocumentId)
        {
            List<RelocationDocumentCurrentTermGrade> currentGradesToAdd = new List<RelocationDocumentCurrentTermGrade>();

            if (schoolBookCurrentGrades.IsNullOrEmpty())
            {
                return currentGradesToAdd;
            }

            // Размножааме предметите от учебния план за двата срока.
            // student.fn_student_grades_for_basicClass идват агрегирани данни за всеки предмет от учебния план.
            List<int> terms = new List<int> { 1, 2 };

            foreach (int term in terms)
            {
                foreach (StudentGradeForBasicClassDto grade in schoolBookCurrentGrades)
                {
                    if (currentGradesToAdd.Any(x => x.Term == term && x.SubjectId == grade.SubjectId
                        && x.CurriculumPartId == grade.CurriculumPartId))
                    {
                        // Оценката от дневника вече съществуа в _context.RelocationDocumentCurrentTermGrades.
                        // Добавена е при редакция и добавяне ръчно на оценка.
                        continue;
                    }

                    RelocationDocumentCurrentTermGrade toAdd = new RelocationDocumentCurrentTermGrade
                    {
                        RelocationDocumentId = relocationDocumentId,
                        PersonId = grade.PersonId,
                        SubjectId = grade.SubjectId,
                        SubjectName = grade.SubjectName,
                        SubjectTypeId = grade.SubjectTypeId,
                        Term = term,
                        CurriculumId = grade.CurriculumId,
                        CurriculumPartId = grade.CurriculumPartId,
                        InstitutionId = grade.InstitutionId,
                        SchoolYear = grade.SchoolYear,
                        IsLoadedFromSchoolbook = true
                    };

                    if (term == 1)
                    {
                        // Първи срок
                        toAdd.Grades = !grade.FirstTermDecimalGrades.IsNullOrEmpty()
                            ? grade.FirstTermDecimalGrades
                            : (!grade.FirstTermQualitativeGrades.IsNullOrEmpty()
                                ? grade.FirstTermQualitativeGrades
                                : grade.FirstTermSpecialGrades);
                    }
                    else
                    {
                        // Втори срок
                        toAdd.Grades = !grade.SecondTermDecimalGrades.IsNullOrEmpty()
                            ? grade.SecondTermDecimalGrades
                            : (!grade.SecondTermQualitativeGrades.IsNullOrEmpty()
                                ? grade.SecondTermQualitativeGrades
                                : grade.SecondTermSpecialGrades);
                    }

                    currentGradesToAdd.Add(toAdd);
                }
            }

            return currentGradesToAdd;
        }

        public async Task<IEnumerable<StudentLodAssessmentListModel>> GetLodAssessmentsList(int relocationDocumentId, CancellationToken cancellationToken = default)
        {
            var relocationDocument = await _context.RelocationDocuments
                .Where(x => x.Id == relocationDocumentId)
                .Select(x => new
                {
                    x.PersonId,
                    x.CurrentStudentClass.BasicClassId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (!await _authorizationService.HasPermissionForStudent(relocationDocument.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (relocationDocument == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(relocationDocument)));
            }

            List<int> includedBasicClasses = relocationDocument.BasicClassId switch
            {
                7 => new List<int> { 5, 6, 7 }, // В удостоверението за 7-ми клас има секции за оценките от 5-ти, 6-ти и 7-ми клас
                6 => new List<int> { 5, 6 }, // В удостоверението за 6-ми клас има секции за оценките от 5-ти и 6-ти клас
                _ => new List<int> { relocationDocument.BasicClassId } // За всички останали удостоверения се взимат оценките от класа на документа за преместване
            };

            List<StudentLodAssessmentListModel> list = await _context.VStudentLodAsssessmentLists
                .Where(x => x.PersonId == relocationDocument.PersonId && x.BasicClassId.HasValue && includedBasicClasses.Contains(x.BasicClassId.Value))
                .Select(x => new StudentLodAssessmentListModel
                {
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    SchoolYear = x.SchoolYear,
                    BasicClassId = x.BasicClassId ?? 0,
                    IsSelfEduForm = x.IsSelfEduForm ?? false,
                    BasicClassName = x.BasicClassRomeName,
                    BasicClassDescription = x.BasicClassName,
                    SchooYearName = x.SchoolYearName,
                    Categories = x.Categories,
                    GradesCount = x.GradesCount ?? 0,
                    LodAssessmentsCount = x.LodAssessmentsCount ?? 0
                })
                .ToListAsync(cancellationToken);

            return list.OrderBy(x => x.BasicClassId).ThenBy(x => x.IsSelfEduForm);
        }
    }
}
