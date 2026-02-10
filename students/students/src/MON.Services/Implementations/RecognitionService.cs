namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.Enums.SchoolBooks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class RecognitionService : BaseService<RecognitionService>, IRecognitionService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IAppConfigurationService _configurationService;
        private readonly IInstitutionService _institutionService;

        public RecognitionService(DbServiceDependencies<RecognitionService> dependencies,
            IBlobService blobService,
            IInstitutionService institutionService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IAppConfigurationService configurationService)
            : base(dependencies)
        {
            _blobService = blobService;
            _institutionService= institutionService;
            _blobServiceConfig = blobServiceConfig.Value;
            _configurationService = configurationService;
        }

        public async Task<RecognitionModel> GetById(int id)
        {
            RecognitionModel model = await _context.Recognitions
                .Where(x => x.Id == id)
                .Select(x => new RecognitionModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    InstitutionName = x.InstitutionName,
                    InstitutionCountryId = x.InstitutionCountryId,
                    EducationLevelId = x.EducationLevelId,
                    EducationLeveName = x.EducationLevel.Name,
                    SPPOOProfessionId = x.SppooprofessionId,
                    SPPOOProfessionName = $"{x.Sppooprofession.SppooprofessionCode} - {x.Sppooprofession.Name}",
                    SPPOOSpecialityId = x.SppoospecialityId,
                    SPPOOSpecialityName =$"{x.Sppoospeciality.SppoospecialityCode} - {x.Sppoospeciality.Name}",
                    VetLevel = x.Sppoospeciality.Vetlevel,
                    BasicClassId = x.BasicClassId,
                    BasicClassName = x.BasicClass.Description,
                    Term = x.Term,
                    RuoDocumentDate = x.RuoDocumentDate,
                    RuoDocumentNumber = x.RuoDocumentNumber,
                    DiplomaNumber = x.DiplomaNumber,
                    DiplomaDate = x.DiplomaDate,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    IsSelfEduForm = x.IsSelfEduForm,
                    Documents = x.RecognitionDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                    Equalizations = x.RecognitionEqualizations
                        .OrderBy(e => e.Position)
                        .Select(e => new RecognitionEqualizationModel
                        {
                            Id = e.Id,
                            SubjectId = e.SubjectId,
                            SubjectName = e.Subject.SubjectName,
                            SubjectTypeId = e.SubjectTypeId,
                            SubjectTypeName = e.SubjectType.Name,
                            GradeCategory = e.GradeCategory,
                            Grade = e.Grade,
                            SpecialNeedsGrade= e.SpecialNeedsGrade,
                            OtherGrade = e.OtherGrade,
                            OriginalSubject = e.OriginalSubject,
                            OriginalGrade = e.OriginalGrade,
                            IsRequired = e.IsRequired,
                            SortOrder = e.Position,
                            Uid = Guid.NewGuid().ToString(),
                            BasicClassId = e.BasicClassId,
                            BasicClassName = e.BasicClass.Description,
                        }).ToList()
                })
                .SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentRecognitionRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentRecognitionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return model;
        }

        public async Task<List<RecognitionViewModel>> GetListForPerson(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentRecognitionRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.Recognitions
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new RecognitionViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    InstitutionName = x.InstitutionName,
                    InstitutionCountry = x.InstitutionCountry.Name,
                    BasicClass = x.BasicClass.Name,
                    Term = (SchoolTerm)x.Term,
                    EducationLevel = x.EducationLevel.Name,
                    SPPOOProfession = x.Sppooprofession.Name,
                    SPPOOSpeciality = x.Sppoospeciality.Name,
                    RuoDocumentDate = x.RuoDocumentDate,
                    RuoDocumentNumber = x.RuoDocumentNumber,
                    DiplomaNumber = x.DiplomaNumber,
                    DiplomaDate = x.DiplomaDate,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    Documents = x.RecognitionDocuments
                        .Select(x => x.Document.ToViewModel(_blobServiceConfig))

                })
                .ToListAsync();
        }

        public async Task Create(RecognitionModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentRecognitionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = await ValidateRecognitionCreation(model);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Messages);
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            var entry = new Recognition
            {
                PersonId = model.PersonId,
                InstitutionName = model.InstitutionName ?? "",
                InstitutionCountryId = model.InstitutionCountryId,
                EducationLevelId = model.EducationLevelId,
                SppooprofessionId = model.SPPOOProfessionId,
                SppoospecialityId = model.SPPOOSpecialityId,
                BasicClassId = model.BasicClassId,
                Term = model.Term,
                RuoDocumentDate = model.RuoDocumentDate,
                RuoDocumentNumber = model.RuoDocumentNumber,
                DiplomaNumber = model.DiplomaNumber,
                DiplomaDate = model.DiplomaDate,
                SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID),
                IsSelfEduForm = model.IsSelfEduForm,
                RecognitionEqualizations = model.Equalizations?.OrderBy(x => x.SortOrder)
                    .Select(x => new RecognitionEqualization
                    {
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        OriginalSubject = x.OriginalSubject,
                        OriginalGrade = x.OriginalGrade,
                        Position = x.SortOrder ?? 1,
                        IsRequired = x.IsRequired,
                        GradeCategory = x.GradeCategory,
                        Grade = x.GradeCategory == (int)GradeCategoryEnum.Normal || x.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? x.Grade
                            : null,
                        SpecialNeedsGrade = x.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                            ? x.SpecialNeedsGrade
                            : null,
                        OtherGrade = x.GradeCategory == (int)GradeCategoryEnum.Other
                            ? x.OtherGrade
                            : null,
                        BasicClassId = x.GetBasciClassId(model.EducationLevelId, model.BasicClassId) 
                    }).ToList()
            };
           
            using var transaction = _context.Database.BeginTransaction();
            _context.Recognitions.Add(entry);
            await ProcessAddedDocs(model, entry);
            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(RecognitionModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            Recognition entity = await _context.Recognitions
                .Include(x => x.RecognitionDocuments)
                .Include(x => x.RecognitionEqualizations)
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), nameof(Recognition));
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentRecognitionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = await ValidateRecognitionUpdate(entity, model);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Messages);
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            entity.InstitutionName = model.InstitutionName ?? "";
            entity.InstitutionCountryId = model.InstitutionCountryId;
            entity.EducationLevelId = model.EducationLevelId;
            entity.SppooprofessionId = model.SPPOOProfessionId;
            entity.SppoospecialityId = model.SPPOOSpecialityId;
            entity.BasicClassId = model.BasicClassId;
            entity.Term = model.Term;
            entity.RuoDocumentDate = model.RuoDocumentDate;
            entity.RuoDocumentNumber = model.RuoDocumentNumber;
            entity.DiplomaNumber = model.DiplomaNumber;
            entity.DiplomaDate = model.DiplomaDate;
            entity.IsSelfEduForm = model.IsSelfEduForm;
            entity.SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);


            HashSet<int> existedIds = model.Equalizations != null
                ? model.Equalizations.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.RecognitionEqualizations.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // Оценки за изтриване
                _context.RecognitionEqualizations.RemoveRange(toDelete);
            }

            if (model.Equalizations != null)
            {
                // Оценки за добавяне
                var toAdd = model.Equalizations.Where(x => !x.Id.HasValue);
                if (toAdd.Any())
                {
                    _context.RecognitionEqualizations.AddRange(toAdd.Select(x => new RecognitionEqualization
                    {
                        RecognitionId = entity.Id,
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        Position = x.SortOrder ?? 1,
                        OriginalSubject = x.OriginalSubject,
                        OriginalGrade = x.OriginalGrade,
                        IsRequired = x.IsRequired,
                        GradeCategory = x.GradeCategory,
                        Grade = x.GradeCategory == (int)GradeCategoryEnum.Normal || x.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? x.Grade
                            : null,
                        SpecialNeedsGrade = x.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                            ? x.SpecialNeedsGrade
                            : null,
                        OtherGrade = x.GradeCategory == (int)GradeCategoryEnum.Other
                            ? x.OtherGrade
                            : null,
                        BasicClassId = x.GetBasciClassId(model.EducationLevelId, model.BasicClassId)
                    }));

                }
            }

            if (existedIds.Any())
            {
                // Оценки за редакция
                foreach (var toUpdate in entity.RecognitionEqualizations.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.Equalizations.SingleOrDefault(x => x.Id.HasValue && x.Id.Value == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.SubjectId = source.SubjectId;
                    toUpdate.SubjectTypeId = source.SubjectTypeId;
                    toUpdate.Position = source.SortOrder ?? 1;
                    toUpdate.OriginalSubject = source.OriginalSubject;
                    toUpdate.OriginalGrade = source.OriginalGrade;
                    toUpdate.IsRequired = source.IsRequired;
                    toUpdate.GradeCategory = source.GradeCategory;
                    toUpdate.Grade = source.GradeCategory == (int)GradeCategoryEnum.Normal || source.GradeCategory == (int)GradeCategoryEnum.Qualitative
                          ? source.Grade
                          : null;
                    toUpdate.SpecialNeedsGrade = source.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                          ? source.SpecialNeedsGrade
                          : null;
                    toUpdate.OtherGrade = source.GradeCategory == (int)GradeCategoryEnum.Other
                          ? source.OtherGrade
                          : null;
                    toUpdate.BasicClassId = source.GetBasciClassId(model.EducationLevelId, model.BasicClassId);
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            Recognition entity = await _context.Recognitions
                .Include(x => x.RecognitionDocuments)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentRecognitionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ApiValidationResult validationResult = await ValidateRecognitiontDeletion(entity);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Messages);
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            if (entity.RecognitionDocuments != null && entity.RecognitionDocuments.Any())
            {
                var docsIds = entity.RecognitionDocuments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.RecognitionDocuments.RemoveRange(entity.RecognitionDocuments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            _context.Recognitions.Remove(entity); // Това трябва да преди изтртиването на свързаните student.Document

            await SaveAsync();
        }

        public Task<string> GetRecognitionRequiredSubjects()
        {
            return _configurationService.GetValueByKey("RecognitionRequiredSubjects");
        }

        private async Task ProcessAddedDocs(RecognitionModel model, Recognition entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.RecognitionDocuments.Add(new RecognitionDocument
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(RecognitionModel model, Recognition entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.RecognitionDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }

        #region Validation
        private async Task<ApiValidationResult> ValidateRecognitionCreation(RecognitionModel model)
        {
            ApiValidationResult validationResult = new ApiValidationResult
            {
                IsValid = true
            };

            await ValidateRequiredSubjects(validationResult, model);

            return validationResult;
        }

        private async Task<ApiValidationResult> ValidateRecognitionUpdate(Recognition entity, RecognitionModel model)
        {
            ApiValidationResult validationResult = new ApiValidationResult
            {
                IsValid = true
            };

            await ValidateRequiredSubjects(validationResult, model);


            return validationResult;
        }

        private async Task ValidateRequiredSubjects(ApiValidationResult validationResult, RecognitionModel model)
        {
            string config = await _configurationService.GetValueByKey("RecognitionRequiredSubjectsValidation");
            bool.TryParse(config ?? "", out bool hasToValidate);
            if (!hasToValidate) return;

            HashSet<int> requiredSubjects = await GetRequiredSubjects(model.EducationLevelId, model.BasicClassId);
            if (requiredSubjects == null || !requiredSubjects.Any()) return;

            HashSet<int> missingRequiredSubjects = new HashSet<int>();
            foreach (var reqSubject in requiredSubjects)
            {
                if (model.Equalizations == null || !model.Equalizations.Any(x => x.SubjectId == reqSubject))
                {
                    missingRequiredSubjects.Add(reqSubject);
                }
            }

            if (missingRequiredSubjects.Any())
            {
                validationResult.IsValid = false;

                var subjectsInfo = await _context.Subjects
                    .AsNoTracking()
                    .Where(x => missingRequiredSubjects.Contains(x.SubjectId))
                    .ToDictionaryAsync(x => x.SubjectId, x => x.SubjectName);

                foreach (int missingSubjectId in missingRequiredSubjects)
                {
                    if (subjectsInfo.TryGetValue(missingSubjectId, out string subjectName))
                    {
                        validationResult.Messages.Add($"Липсва задължителен предмет '{subjectName}'.");
                    }
                }
            }
        }

        private async Task<HashSet<int>> GetRequiredSubjects(int? educationLevel, int? basicClass)
        {
            HashSet<int> requiredSubjcts = new HashSet<int>();

            string requiredSubjectsStr = await GetRecognitionRequiredSubjects();
            if (requiredSubjectsStr.IsNullOrWhiteSpace()) return requiredSubjcts;

            try
            {
                Dictionary<string, object> exp = JsonConvert.DeserializeObject<Dictionary<string, object>>(requiredSubjectsStr);
                if (exp.TryGetValue(educationLevel?.ToString() ?? "", out object value))
                {
                    if (value is JArray jArr)
                    {
                        requiredSubjcts = jArr.ToObject<HashSet<int>>();
                    }
                    else
                    {
                        if (value is JObject jObj)
                        {
                            var dict = jObj.ToObject<Dictionary<string, HashSet<int>>>();
                            if (dict != null)
                            {
                                dict.TryGetValue(basicClass?.ToString() ?? "", out requiredSubjcts);
                            }
                        }
                    }
                }

                return requiredSubjcts;

            }
            catch (Exception)
            {

                throw;
            }

        }

        private Task<ApiValidationResult> ValidateRecognitiontDeletion(Recognition entity)
        {
            ApiValidationResult validationResult = new ApiValidationResult
            {
                IsValid = true
            };

            return Task.FromResult(validationResult);
        }
        #endregion
    }
}
