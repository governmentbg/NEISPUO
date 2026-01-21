using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Services.Extensions;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{

    public class OtherDocumentService : BaseService<OtherDocumentService>, IOtherDocumentService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly IInstitutionService _institutionService;
        private readonly IAppConfigurationService _configurationService;
        private bool? _basicDocumentSequenceCheck = null;
        private HashSet<int> _basicDocumentSequenceValidationException = null;

        public OtherDocumentService(DbServiceDependencies<OtherDocumentService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            ILodFinalizationService lodFinalizationService,
            IInstitutionService institutionService,
            IAppConfigurationService configurationService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _lodFinalizationService = lodFinalizationService;
            _institutionService = institutionService;
            _configurationService = configurationService;
        }

        #region Private members
        private async Task ProcessAddedDocs(OtherDocumentModel model, OtherDocument entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.OtherDocumentDocuments.Add(new OtherDocumentDocument
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(OtherDocumentModel model, OtherDocument entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.OtherDocumentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }

        private bool BasicDocumentSequenceCheck
        {
            get
            {
                if (_basicDocumentSequenceCheck == null)
                {
                    string config = _configurationService.GetValueByKey("ValidateBasicDocumentSequence").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _basicDocumentSequenceCheck = result;
                }

                return _basicDocumentSequenceCheck.Value;
            }
        }

        private HashSet<int> BasicDocumentSequenceValidationException
        {
            get
            {
                if (_basicDocumentSequenceValidationException == null)
                {
                    string config = _configurationService.GetValueByKey("BasicDocumentSequenceValidationException").Result;
                    if (string.IsNullOrWhiteSpace(config))
                    {
                        _basicDocumentSequenceValidationException = new HashSet<int>();
                    }
                    else
                    {
                        _basicDocumentSequenceValidationException = JsonSerializer.Deserialize<HashSet<int>>(config ?? "");
                    }
                }

                return _basicDocumentSequenceValidationException;
            }
        }

        #endregion

        public async Task<OtherDocumentModel> GetById(int id)
        {
            OtherDocumentModel model = await _context.OtherDocuments.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new OtherDocumentModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Description = x.Description,
                    Series = x.Series,
                    FactoryNumber = x.FactoryNumber,
                    RegNumberTotal = x.RegNumberTotal,
                    RegNumber = x.RegNumber,
                    DeliveryDate = x.DeliveryDate,
                    IssueDate = x.IssueDate,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    DocumentTypeId = x.BasicDocumentId,
                    DocumentTypeName = x.BasicDocument.Name,
                    Documents = x.OtherDocumentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))

                })
                .SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentOtherDocumentRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentOtherDocumentManage))
            {
                // Необходим достъп до Други документи  https://github.com/Neispuo/students/issues/1187
                if (!_userInfo.InstitutionID.HasValue || !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentOtherDocumentManage)
                    || _userInfo.InstitutionID.Value != model.InstitutionId)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            return model;
        }

        public async Task<List<OtherDocumentModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentOtherDocumentRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentOtherDocumentManage)) // Необходим достъп до Други документи  https://github.com/Neispuo/students/issues/1187
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await (from x in _context.OtherDocuments
                          join lf in _context.Lodfinalizations on new { x.PersonId, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                          from lodFin in temp.DefaultIfEmpty()
                          where x.PersonId == personId
                          orderby x.SchoolYear descending, x.Id descending
                          select new OtherDocumentModel
                          {
                              Id = x.Id,
                              PersonId = x.PersonId,
                              Description = x.Description,
                              Series = x.Series,
                              FactoryNumber = x.FactoryNumber,
                              RegNumberTotal = x.RegNumberTotal,
                              RegNumber = x.RegNumber,
                              DeliveryDate = x.DeliveryDate,
                              IssueDate = x.IssueDate,
                              SchoolYear = x.SchoolYear,
                              SchoolYearName = x.SchoolYearNavigation.Name,
                              InstitutionId = x.InstitutionId,
                              InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                              DocumentTypeId = x.BasicDocumentId,
                              DocumentTypeName = x.BasicDocument.Name,
                              Documents = x.OtherDocumentDocuments
                                  .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                              IsLodFinalized = lodFin != null && lodFin.IsFinalized
                          })
                    .ToListAsync();
        }

        public async Task Create(OtherDocumentModel model)
        {
            if (!model.InstitutionId.HasValue) {
                model.InstitutionId = _userInfo?.InstitutionID;
            }

            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentOtherDocumentManage))
            {
                // Необходим достъп до Други документи  https://github.com/Neispuo/students/issues/1187
                if (!_userInfo.InstitutionID.HasValue || !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentOtherDocumentManage)
                    || _userInfo.InstitutionID.Value != model.InstitutionId)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            if (model.SchoolYear.HasValue)
            {
                if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear.Value))
                {
                    throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
                }
            }

            var entry = new OtherDocument
            {
                PersonId = model.PersonId,
                Description = model.Description,
                Series = model.Series,
                FactoryNumber = model.FactoryNumber,
                RegNumberTotal = model.RegNumberTotal,
                RegNumber = model.RegNumber,
                IssueDate = model.IssueDate,
                DeliveryDate = model.DeliveryDate,
                InstitutionId = model.InstitutionId,
                BasicDocumentId = model.DocumentTypeId ?? throw new ArgumentNullException(nameof(model), Messages.EmptyModelError),
                SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(model.InstitutionId ?? _userInfo?.InstitutionID)
            };

            ApiValidationResult validationResult = await ValidateOtherDocumentCreateOrUpdate(entry);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            using var transaction = _context.Database.BeginTransaction();
            _context.OtherDocuments.Add(entry);
            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
            //await transaction.RollbackAsync();
        }

        public async Task Update(OtherDocumentModel model)
        {
            OtherDocument entity = await _context.OtherDocuments
                .Include(x => x.OtherDocumentDocuments)
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentOtherDocumentManage))
            {
                // Необходим достъп до Други документи  https://github.com/Neispuo/students/issues/1187
                if (!_userInfo.InstitutionID.HasValue || !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentOtherDocumentManage)
                    || _userInfo.InstitutionID.Value != entity.InstitutionId)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            if (model.SchoolYear.HasValue)
            {
                if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear.Value))
                {
                    throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
                }
            }

            entity.PersonId = model.PersonId;
            entity.Description = model.Description;
            entity.Series = model.Series;
            entity.FactoryNumber = model.FactoryNumber;
            entity.RegNumberTotal = model.RegNumberTotal;
            entity.RegNumber = model.RegNumber;
            entity.IssueDate = model.IssueDate;
            entity.DeliveryDate = model.DeliveryDate;
            entity.InstitutionId = model.InstitutionId ?? _userInfo?.InstitutionID;
            entity.BasicDocumentId = model.DocumentTypeId ?? throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            entity.SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(model.InstitutionId ?? _userInfo?.InstitutionID);

            ApiValidationResult validationResult = await ValidateOtherDocumentCreateOrUpdate(entity);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
            //await transaction.RollbackAsync();
        }

        public async Task Delete(int id)
        {
            OtherDocument entity = await _context.OtherDocuments
                .Include(x => x.OtherDocumentDocuments)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentOtherDocumentManage))
            {
                // Необходим достъп до Други документи  https://github.com/Neispuo/students/issues/1187
                if (!_userInfo.InstitutionID.HasValue || !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentOtherDocumentManage)
                    || _userInfo.InstitutionID.Value != entity.InstitutionId)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            ApiValidationResult validationResult = await ValidateOtherDocumentDeletion(entity);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Messages);
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            if (entity.OtherDocumentDocuments != null && entity.OtherDocumentDocuments.Any())
            {
                var docsIds = entity.OtherDocumentDocuments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.OtherDocumentDocuments.RemoveRange(entity.OtherDocumentDocuments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            _context.OtherDocuments.Remove(entity); // Това трябва да преди изтртиването на свързаните student.Document

            await SaveAsync();
        }
       
        #region Validation
        private void ValidateOtherDocumentCommonRules<T>(T model, ApiValidationResult validationResult) where T : IInstitution
        {
            validationResult.IsValid = true;
            if (model == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = Messages.EmptyModelError
                });
                validationResult.IsValid = false;

                return;
            }

            if (!_userInfo.IsSchoolDirector || !model.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = Messages.UnauthorizedMessageError
                });
                validationResult.IsValid = false;

                return;
            }
        }

        private async Task<ApiValidationResult> ValidateOtherDocumentCreateOrUpdate(OtherDocument entity)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            ValidateOtherDocumentCommonRules(entity, validationResult);

            if (BasicDocumentSequenceCheck
                && !BasicDocumentSequenceValidationException.Any(x => x == entity.BasicDocumentId))
            {
                await ValidateBasicDocumentSequence(entity, validationResult);
            };
            
            return validationResult;
        }

        private Task<ApiValidationResult> ValidateOtherDocumentDeletion(OtherDocument entity)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            ValidateOtherDocumentCommonRules(entity, validationResult);

            return Task.FromResult(validationResult);
        }


        private async Task ValidateBasicDocumentSequence(OtherDocument entity, ApiValidationResult validationResult)
        {
            /*
            Във валидатора нa дипломи трябва да се проверява дали за BasicDocument-a, който се създава,
            комбинацията от RegNumberTotal+RegNumberYear+RegDate+InstitutionId за учебната година (SchoolYear, не YearGraduated) може да се намери в document.BasicDocumentSequence.
            Същото трябва да се прави при запис в OtherDocument таблицата. Там полетата RegNumberTotal, RegNumberYear и RegDate може да се казват по друг начин.
            Проверката НЕ трябва да се прилага за документи със SchoolYear < 2022.
            */

            if (entity == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = Messages.EmptyEntityError
                });

                return;
            }

            if (entity.SchoolYear < 2022)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = Messages.EmptyEntityError
                });

                return;
            }

            var sequences = await _context.BasicDocumentSequences
               .Where(x => x.InstitutionId == entity.InstitutionId && x.SchoolYear == entity.SchoolYear
                   && x.BasicDocumentId == entity.BasicDocumentId)
               .Select(x => new
               {
                   x.RegNumberTotal,
                   x.RegNumberYear,
                   x.RegDate
               })
               .ToListAsync();

            int.TryParse(entity.RegNumberTotal, out int regNumberTotal);
            int.TryParse(entity.RegNumber, out int regNumberYear);

            if (!sequences.Any(x => x.RegNumberTotal == regNumberTotal
                && x.RegNumberYear == regNumberYear
                && x.RegDate == entity.IssueDate))
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = "Не е намерено съответствие между общия рег. номер, рег. номер за годината и датата на регистрация на документа с такива, взети от НЕИСПУО",
                    Data = JsonSerializer.Serialize(entity)
                });
            }

            var existingDiploma = await _context.Diplomas
                .Where(x => x.InstitutionId == entity.InstitutionId && x.SchoolYear == entity.SchoolYear
                    && x.BasicDocumentId == entity.BasicDocumentId
                    && x.RegistrationNumberTotal == entity.RegNumberTotal
                    && x.RegistrationNumberYear == entity.RegNumber
                    && x.RegistrationDate == entity.IssueDate.Value)
                .Select(x => new
                {
                    PersonName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    x.BasicDocumentName
                })
                .FirstOrDefaultAsync();

            if (existingDiploma != null)
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = $"Рег.номер е използван за създаването на документ от тип '{existingDiploma.BasicDocumentName}' на '{existingDiploma.PersonName}'",
                    Data = JsonSerializer.Serialize(entity)
                });

                return;
            }

            var existingOtherDocument = await _context.OtherDocuments
                .Where(x => x.InstitutionId == entity.InstitutionId && x.SchoolYear == entity.SchoolYear
                    && x.BasicDocumentId == entity.BasicDocumentId
                    && x.RegNumberTotal == entity.RegNumberTotal
                    && x.RegNumber == entity.RegNumber
                    && x.IssueDate == entity.IssueDate
                    && x.Id != entity.Id)
                .Select(x => new
                {
                    PersonName = x.Person.FirstName + " " + x.Person.MiddleName + " " + x.Person.LastName,
                    BasicDocumentName = x.BasicDocument.Name
                })
                .FirstOrDefaultAsync();

            if (existingOtherDocument != null)
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = $"Рег.номер е използван за създаването на документ от тип '{existingOtherDocument.BasicDocumentName}' на '{existingOtherDocument.PersonName}'",
                    Data = JsonSerializer.Serialize(entity)
                });

                return;
            }

        }
        #endregion
    }
}
