using Domain;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.Diploma;
using MON.Models.Diploma.Import;
using MON.Models.Dropdown;
using MON.Models.Grid;
using MON.Models.Interfaces;
using MON.Services.Implementations.DiplomaCode;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using MON.Shared.Extensions;
using MON.Shared.Extensions.Dynamic;
using MON.Shared.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class DiplomaService : BaseService<DiplomaService>, IDiplomaService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IInstitutionService _institutionService;
        private readonly IBarcodeService _barcodeService;
        private readonly IBarcodeYearService _barcodeYearService;
        private readonly IBasicDocumentService _basicDocumentService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IImageService _imageService;
        private readonly ILookupService _lookupService;
        private readonly IDiplomaTemplateService _diplomaTemplateService;
        private readonly IDiplomaImportValidationExclusionService _diplomaImportValidationExclusionService;
        private readonly ICertificateService _certificateService;
        private readonly IWordTemplateService _wordTemplateService;

        private Dictionary<int, IDiplomaCode> _validationServices;

        public DiplomaService(DbServiceDependencies<DiplomaService> dependencies,
            IBlobService blobService,
            IInstitutionService institutionService,
            IBarcodeService barcodeService,
            IBarcodeYearService barcodeYearService,
            IBasicDocumentService basicDocumentService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IServiceProvider serviceProvider,
            IImageService imageService,
            ILookupService lookupService,
            IDiplomaTemplateService diplomaTemplateService,
            IDiplomaImportValidationExclusionService diplomaImportValidationExclusionService,
            ICertificateService certificateService,
            IWordTemplateService wordTemplateService)
            : base(dependencies)
        {
            _blobService = blobService;
            _institutionService = institutionService;
            _barcodeService = barcodeService;
            _barcodeYearService = barcodeYearService;
            _blobServiceConfig = blobServiceConfig.Value;
            _basicDocumentService = basicDocumentService;
            _serviceProvider = serviceProvider;
            _imageService = imageService;
            _lookupService = lookupService;
            _diplomaTemplateService = diplomaTemplateService;
            _diplomaImportValidationExclusionService = diplomaImportValidationExclusionService;
            _certificateService = certificateService;
            _wordTemplateService = wordTemplateService;
        }

        #region Private members
        private async Task<IDiplomaCode> GetDiplomaCodeAsync(int basicDocumentId)
        {
            BasicDocumentModel basicDocument = await _basicDocumentService.GetByIdAsync(basicDocumentId);
            if (!string.IsNullOrWhiteSpace(basicDocument.CodeClassName))
            {
                Type diplomaCodeType = ReflectionUtils.GetType(basicDocument.CodeClassName);
                IDiplomaCode diplomaService = _serviceProvider.GetRequiredService(diplomaCodeType) as IDiplomaCode;

                return diplomaService;
            }
            else
            {
                return null;
            }
        }

        private async Task<IDiplomaCode> GetValidationService(int basicDocumentId)
        {
            if (_validationServices == null) _validationServices = new Dictionary<int, IDiplomaCode>();

            if (!_validationServices.ContainsKey(basicDocumentId))
            {
                _validationServices.Add(basicDocumentId, await GetDiplomaCodeAsync(basicDocumentId));
            }

            return _validationServices[basicDocumentId];
        }

        private ApiValidationResult ReadDiplomasArchive(IFormFile file, List<DiplomaImportModel> output)
        {
            if (file == null || file.Length <= 0) return null;
            if (output == null) output = new List<DiplomaImportModel>();

            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Diploma";
            xRoot.IsNullable = true;
            XmlSerializer ser = new XmlSerializer(typeof(DiplomaImportParseModel), xRoot);
            //ser.UnknownElement += Serializer_UnknownElement;

            ApiValidationResult deserializationResult = new ApiValidationResult();

            using Stream archiveStream = file.OpenReadStream();
            using ZipArchive archive = new ZipArchive(archiveStream);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    DiplomaImportModel model = new DiplomaImportModel
                    {
                        ArchiveName = file.FileName,
                        FileName = entry.Name,
                    };

                    using Stream stream = entry.Open();
                    using StreamReader reader = new StreamReader(stream);
                    try
                    {
                        model.RawData = reader.ReadToEnd();

                        // 1. Валидиране по схема
                        ValidateXsd(deserializationResult, model.RawData, entry.Name);

                        if (deserializationResult != null && !deserializationResult.HasErrors)
                        {
                            // 2. Сериализиране на данните, ако xml-a е преминал валидация
                            using TextReader tReader = new StringReader(model.RawData);
                            model.ParseModel = (DiplomaImportParseModel)ser.Deserialize(tReader);
                            output.Add(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        deserializationResult.Errors.Add(ex.Message, ID: entry.Name);
                        if (ex.InnerException != null)
                        {
                            deserializationResult.Errors.Add(ex.GetInnerMostException().Message, ID: entry.Name);
                        }
                    }
                }
            }

            return deserializationResult;
        }

        private void ValidateXsd(ApiValidationResult validationResult, string xml, string entryName)
        {
            if (validationResult == null) return;

            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, XmlReader.Create(Path.Combine(AppContext.BaseDirectory, "Schema", "import.xsd")));
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (o, args) =>
           {
               switch (args.Severity)
               {
                   case XmlSeverityType.Error:
                       validationResult.Errors.Add(new ValidationError()
                       {
                           Message = $"Грешка (Ред: {args.Exception?.LineNumber} / Позиция: {args.Exception.LinePosition}): {args.Message}" + AddXmlValidationErrorMessages(args),
                           ID = entryName
                       });
                       break;
                   case XmlSeverityType.Warning:
                       validationResult.Errors.Add(new ValidationError()
                       {
                           Message = $"Предупреждение (Ред: {args.Exception?.LineNumber} / Позиция: {args.Exception.LinePosition}): {args.Message}" + AddXmlValidationErrorMessages(args),
                           ID = entryName
                       });
                       break;
               }
           };
            using TextReader tReader = new StringReader(xml);
            XmlReader reader = XmlReader.Create(tReader, settings);

            while (reader.Read()) ;
        }

        private string AddXmlValidationErrorMessages(ValidationEventArgs args)
        {
            if (args == null) return "";

            string message = args.Message ?? "";
            if (message.Contains("is invalid according to its datatype 'SchoolYearType'", StringComparison.OrdinalIgnoreCase))
            {
                return " Позволените стойноти са [1900-2100].";
            }

            if (message.Contains("The Pattern constraint failed", StringComparison.OrdinalIgnoreCase)
                && message.Contains("The 'Series' element is invalid", StringComparison.OrdinalIgnoreCase))
            {
                return " Позволени са големи букви на Кирилица, цифри и '-' в примерен формат 'Б', 'ББ', 'Б-99', 'ББ-99'.";
            }

            if (message.Contains("The Pattern constraint failed", StringComparison.OrdinalIgnoreCase)
                && message.Contains("The 'FactoryNumber' element is invalid", StringComparison.OrdinalIgnoreCase))
            {
                return " Позволено е число с дължина 6(шест) цифри.";
            }

            if (message.Contains("The Pattern constraint failed", StringComparison.OrdinalIgnoreCase)
                && message.Contains("The 'ECTS' element is invalid", StringComparison.OrdinalIgnoreCase))
            {
                return " Позволени са следните големи латински букви: A,B,C,D,E.";
            }

            return "";
        }

        private async Task<(bool hasManagePermission, bool filterByInstitution)> CheckForManagePermission(int? personId)
        {
            if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage))
            {
                // Имаме глобални администраторски права
                return (true, false);
            }

            bool filterByInstitution = personId.HasValue == false && _userInfo.InstitutionID.HasValue; // Грида в меню Дипломи на главото меню на институциите

            bool hasDiplomaManagePermission = personId.HasValue
                ? await _authorizationService.HasPermissionForStudent(personId.Value, DefaultPermissions.PermissionNameForStudentDiplomaManage)
                : await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionDiplomaManage);

            if (!hasDiplomaManagePermission && personId.HasValue)
            {
                // Нямаме manage permission. Ако е грида с дипломи в ЛОД-а на ученика проверяваме дали имаме право за редакция получено чрез
                // създаването на заявление за създаване на документ/диплома.
                bool hasPermisison = await _authorizationService.HasPermissionForStudent(personId.Value, DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestManage);
                hasDiplomaManagePermission = hasPermisison;
                if (hasPermisison)
                {
                    filterByInstitution = true;
                }
            }

            return (hasDiplomaManagePermission, filterByInstitution);
        }

        private async Task<(bool hasDiplomaReadPermission, bool filterByInstitution)> CheckForReadPermission(int? personId)
        {
            if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaRead))
            {
                // Имаме глобални администраторски права
                return (true, false);
            }

            if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaRead))
            {
                // Имаме глобални права за четене на всички дипломи (роля ЧРАО)
                return (true, false);
            }

            bool filterByInstitution = personId.HasValue == false; // Грида в меню Дипломи на главото меню на институциите

            bool hasDiplomaReadPermission = personId.HasValue
                ? await _authorizationService.HasPermissionForStudent(personId.Value, DefaultPermissions.PermissionNameForStudentDiplomaRead)
                : await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionDiplomaRead);

            if (!hasDiplomaReadPermission && personId.HasValue)
            {
                // Нямаме read permission. Ако е грида с дипломи в ЛОД-а на ученика проверяваме дали имаме право за четене получено чрез
                // създаването на заявление за създаване на документ/диплома.
                bool hasPermisison = await _authorizationService.HasPermissionForStudent(personId.Value, DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestRead);
                hasDiplomaReadPermission = hasPermisison;
                if (hasPermisison)
                {
                    filterByInstitution = true;
                }
            }

            return (hasDiplomaReadPermission, filterByInstitution);
        }

        private async Task<bool> HasDiplomaManagePermission(int? personId)
        {
            return await _authorizationService.HasPermissionForStudent(personId ?? 0, DefaultPermissions.PermissionNameForStudentDiplomaManage)
                || await _authorizationService.HasPermissionForStudent(personId ?? 0, DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestManage)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionDiplomaManage)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaManage)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage);
        }

        private async Task<bool> HasDiplomaReadPermission(int? personId)
        {
            return await _authorizationService.HasPermissionForStudent(personId ?? 0, DefaultPermissions.PermissionNameForStudentDiplomaRead)
                || await _authorizationService.HasPermissionForStudent(personId ?? 0, DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestRead)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionDiplomaRead)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaRead)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaRead)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage);
        }

        private async Task ValidateDiplomaManagePermissionExclusions(DiplomaViewModel diploma)
        {
            if (diploma.InstitutionId != _userInfo.InstitutionID
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage))
            {
                // Институцията не съвпада с тази на логнатия потребител или той няма такава (примерно ЧРАО)

                if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaManage))
                {
                    // ЧРАО права. Тряба да проверим, че дипломата е създадена от потребител с ЧРАО (MonHR) роля.
                    if (!diploma.CreatedByMonHrRole)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }
                }
                else if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage))
                {
                    // RUO права за документи за приравняване.
                    if (!(diploma.IsRuoDocBasicDocument && diploma.RuoRegId.HasValue && diploma.RuoRegId.Value == _userInfo.RegionID))
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }
                }
                else
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

            }
        }

        private async Task ValidateDiplomaManagePermissionExclusions(Diploma diploma)
        {
            var diplomaDetails = await _context.VDiplomaLists
                    .Where(x => x.Id == diploma.Id)
                    .Select(x => new DiplomaViewModel
                    {
                        Id = x.Id,
                        CreatedByMonHrRole = x.CreatedByMonHrRole,
                        IsRuoDocBasicDocument = x.IsRuoDocBasicDocument,
                        RuoRegId = x.RuoRegId,
                        InstitutionId = x.InstitutionId
                    })
                    .SingleOrDefaultAsync();

            await ValidateDiplomaManagePermissionExclusions(diplomaDetails);
        }

        private async Task CheckDiplomaImageUploadPermission(DiplomaViewModel diploma)
        {
            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (diploma.IsCancelled)
            {
                throw new ApiException(Messages.DiplomaАlreadyАnulled);
            }

            if (diploma.IsSigned && !diploma.IsEditable)
            {
                throw new ApiException(Messages.DiplomaUpdateError);
            }

            bool hasManagePermisson = await HasDiplomaManagePermission(diploma.PersonId);
            if (!hasManagePermisson)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            await ValidateDiplomaManagePermissionExclusions(diploma);
        }

        private async Task CheckDiplomaDeletePermission(Diploma diploma)
        {
            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (diploma.IsCancelled)
            {
                throw new ApiException(Messages.DiplomaАlreadyАnulled);
            }

            if (diploma.IsSigned)
            {
                throw new ApiException(Messages.DiplomaUpdateError);
            }

            bool hasManagePermisson = await HasDiplomaManagePermission(diploma.PersonId);
            if (!hasManagePermisson)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            await ValidateDiplomaManagePermissionExclusions(diploma);
        }

        private async Task CheckDiplomaAnnulmentPermission(Diploma diploma)
        {
            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (diploma.IsCancelled)
            {
                throw new ApiException(Messages.DiplomaАlreadyАnulled);
            }

            if (!(diploma.IsSigned || diploma.IsMigrated))
            {
                throw new ApiException("Дипломата не е подписана!");
            }

            bool hasManagePermisson = await HasDiplomaManagePermission(diploma.PersonId);
            if (!hasManagePermisson)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            await ValidateDiplomaManagePermissionExclusions(diploma);
        }

        private async Task CheckDiplomaSetAsEditablePermission(Diploma diploma)
        {
            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (diploma.IsCancelled)
            {
                throw new ApiException(Messages.DiplomaАlreadyАnulled);
            }

            if (!(diploma.IsSigned || diploma.IsMigrated))
            {
                throw new ApiException("Дипломата не е подписана!");
            }

            if (diploma.IsEditable)
            {
                throw new ApiException("Вече е маркирана за редакция!");
            }

            bool hasManagePermisson = await HasDiplomaManagePermission(diploma.PersonId);
            if (!hasManagePermisson)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            await ValidateDiplomaManagePermissionExclusions(diploma);
        }

        private async Task CheckDiplomaFinalizationPermission(DiplomaViewModel diploma)
        {
            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            bool hasManagePermisson = await HasDiplomaManagePermission(diploma.PersonId);
            if (!hasManagePermisson)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            await ValidateDiplomaManagePermissionExclusions(diploma);
        }

        /// <summary>
        /// Обединява предметите на дадена диплома с дефинираните към типа на дипломата.
        /// Всички предмети в BasicDocumentSubjects трябва да са налични в DiplomaSubjects.
        /// </summary>
        /// <param name="source">Предметите в BasicDocumentSubjects т.е. задължителните за тип документ.</param>
        /// <param name="targer">Предметите в DiplomaSubjects т.е. предметите на дадена диплома.</param>
        private DiplomaSectionsSubjectsModel MergeSubjects(DiplomaSectionsSubjectsModel source, DiplomaSectionsSubjectsModel target)
        {
            if (source == null || target == null || source.Sections == null) return null;

            DiplomaSectionsSubjectsModel result = new DiplomaSectionsSubjectsModel
            {
                DiplomaId = source.DiplomaId,
                BasicDocumentId = source.BasicDocumentId,
                Sections = new List<DiplomaSectionModel>()
            };

            foreach (var sourceSection in source.Sections)
            {
                DiplomaSectionModel targetSection = target.Sections.SingleOrDefault(x => x.Id == sourceSection.Id)
                    ?? new DiplomaSectionModel
                    {
                        Id = sourceSection.Id,
                        Name = sourceSection.Name
                    };

                sourceSection.Subjects ??= new List<DiplomaSubjectModel>();
                targetSection.Subjects ??= new List<DiplomaSubjectModel>();

                List<DiplomaSubjectModel> resultSubjects = sourceSection.Subjects.FullOuterJoin(                 // BasicDocumentSubjects  
                    targetSection.Subjects,                                             // DiplomaSubjects 
                    s => s.BasicDocumentSubjectId,                                      // PK  
                    t => t.BasicDocumentSubjectId,                                      // FK  
                    (s, t) => new { BasicDocumentSubject = s, DiplomaSubject = t })     // Result Collection
                    .Select(x => new DiplomaSubjectModel
                    {
                        Id = x.DiplomaSubject?.Id,
                        BasicDocumentPartId = x.BasicDocumentSubject?.BasicDocumentPartId ?? x.DiplomaSubject?.BasicDocumentPartId,
                        BasicDocumentSubjectId = x.BasicDocumentSubject?.BasicDocumentSubjectId ?? x.DiplomaSubject.BasicDocumentSubjectId,
                        DiplomaId = source?.DiplomaId,
                        Grade = x.DiplomaSubject?.Grade,
                        GradeText = x.DiplomaSubject?.GradeText,
                        Horarium = x.DiplomaSubject?.Horarium,
                        Position = x.DiplomaSubject?.Position ?? x.BasicDocumentSubject?.Position,
                        Uid = Guid.NewGuid().ToString(),
                        SubjectDropDown = x.DiplomaSubject?.SubjectDropDown ?? x.BasicDocumentSubject.SubjectDropDown,
                        LockedForEdit = x.BasicDocumentSubject != null,
                        SubjectCanChange = x.BasicDocumentSubject?.SubjectCanChange ?? x.DiplomaSubject.SubjectCanChange,
                        IsHorariumHidden = x.BasicDocumentSubject?.IsHorariumHidden ?? x.DiplomaSubject.IsHorariumHidden
                    })
                    .OrderBy(x => x.Position)
                    .ToList();

                targetSection.Subjects = resultSubjects;

                result.Sections.Add(targetSection);
            }

            return result;


            //target.Sections ??= new List<DiplomaSectionModel>();

            //foreach (DiplomaSectionModel sourceSection in source.Sections)
            //{
            //    DiplomaSectionModel targetSection = target.Sections.SingleOrDefault(x => x.Id == sourceSection.Id);
            //    if (targetSection == null)
            //    {
            //        // В дипломата липсва задължителната секция дефинирана в BasicDocumentParts
            //        target.Sections.Add(new DiplomaSectionModel
            //        {
            //            Id = sourceSection.Id,
            //            Name = sourceSection.Name,
            //            Subjects = sourceSection.Subjects.Select(s => new DiplomaSubjectModel
            //            {
            //                Id = null,
            //                BasicDocumentPartId = sourceSection.Id,
            //                BasicDocumentSubjectId = s.BasicDocumentSubjectId,
            //                DiplomaId = target.DiplomaId,
            //                Position = s.Position,
            //                Uid = s.Uid,
            //                SubjectDropDown = s.SubjectDropDown,
            //            })
            //            .ToList()
            //        });
            //    }
            //    else
            //    {
            //        // В дипломата присъсвта задължителната секция дефинирана в BasicDocumentParts. Ще добавяме предметите, ако има.
            //        if (sourceSection.Subjects == null || !sourceSection.Subjects.Any()) continue;

            //        targetSection.Subjects ??= new List<DiplomaSubjectModel>();

            //        foreach (DiplomaSubjectModel sourceSubject in sourceSection.Subjects)
            //        {
            //            if (sourceSubject == null || sourceSubject.SubjectDropDown == null) continue;

            //            DiplomaSubjectModel targetSubject = targetSection.Subjects.FirstOrDefault(x => x.SubjectDropDown != null &&
            //                x.SubjectDropDown.Value == sourceSubject.SubjectDropDown.Value);
            //            if (targetSubject == null)
            //            {
            //                targetSection.Subjects.Add(new DiplomaSubjectModel
            //                {
            //                    Id = null,
            //                    BasicDocumentPartId = sourceSubject.BasicDocumentPartId,
            //                    BasicDocumentSubjectId = sourceSubject.BasicDocumentSubjectId,
            //                    DiplomaId = target.DiplomaId,
            //                    Position = sourceSubject.Position,
            //                    Uid = sourceSubject.Uid,
            //                    SubjectDropDown = sourceSubject.SubjectDropDown,
            //                    LockedForEdit = true
            //                });
            //            }
            //            else
            //            {
            //                // Презаписваме името на предета и позицията. Предметите дефинирани в BasicDocumentSubjects не трябва да се променят.
            //                targetSubject.LockedForEdit = true;
            //                targetSubject.Position = sourceSubject.Position;
            //                targetSubject.SubjectDropDown = sourceSubject.SubjectDropDown;
            //            }
            //        }
            //    }
            //}
        }

        private async Task CreateOrUpdate(List<DiplomaImportModel> models, ApiValidationResult validationResult, string dynamicContent = null, int? diplomaId = null)
        {
            foreach (DiplomaImportModel importModel in models)
            {
                DiplomaImportParseModel model = importModel.ParseModel;

                Diploma diploma = null;
                bool isNew = false;
                if (importModel.IsManualCreateOrUpdate)
                {
                    if (!diplomaId.HasValue)
                    {
                        // Нова диплома.
                        diploma = new Diploma();
                        _context.Add(diploma);
                        isNew = true;
                    }
                    else
                    {
                        diploma = await _context.Diplomas
                            .SingleOrDefaultAsync(x => x.Id == diplomaId.Value);
                    }
                }
                else
                {
                    // На редакция подлежи документ за даден ученик (PersonalId и PersonalIdType),
                    // от даден тип (BasicDocumentId),
                    // който не е канселиран
                    // и не е подписан или е подписан, но е маркиран за редакция.
                    IQueryable<Diploma> diplomaQuery = _context.Diplomas
                        .Where(x => x.PersonalId == model.Person.PersonalId
                            && x.PersonalIdtype == model.Person.PersonalIdType
                            && x.BasicDocumentId == model.Document.BasicDocumentType
                            && !x.IsCancelled
                            && (!x.IsSigned || x.IsEditable));

                    if (!model.Document.RegNumber1.IsNullOrEmpty())
                    {
                        diplomaQuery = diplomaQuery.Where(x => x.RegistrationNumberTotal == model.Document.RegNumber1);
                    }

                    if (!model.Document.RegNumber2.IsNullOrEmpty())
                    {
                        diplomaQuery = diplomaQuery.Where(x => x.RegistrationNumberYear == model.Document.RegNumber2);
                    }

                    diploma = await diplomaQuery.FirstOrDefaultAsync();
                    if (diploma == null)
                    {
                        // Нова диплома.
                        diploma = new Diploma();
                        _context.Add(diploma);
                        isNew = true;
                    }

                }

                diploma.Contents = dynamicContent;

                using var tran = _context.Database.BeginTransaction();
                try
                {
                    await UpdateDiplomaEntity(model, diploma);
                    await UpdateDiplomaSubjects(model, diploma);

                    if (!importModel.IsManualCreateOrUpdate)
                    {
                        // Само при създаване/редакция на диплома чрез импорт ще обновим DiplomaDocuments.
                        // При ръчно създаване/редакция сканираните изображения не се подават с модела, а отделно.
                        await UpdateDiplomaDocuments(model, diploma);
                        IDiplomaCode diplomaCodeService = _serviceProvider.GetRequiredService<CodeService>();
                        diploma.Contents = await diplomaCodeService.AutoFillDynamicContent(model.Document.BasicDocumentType, model, "");
                    }

                    await UpdateDiplomaAdditionalDocuments(model, diploma);
                    await UpdateCommissionMembers(model, diploma);
                    await SaveAsync();

                    if (isNew && importModel.IsManualCreateOrUpdate)
                    {
                        int basicDocumentId = model.Document.BasicDocumentType;
                        await _context.DiplomaCreateRequests
                            .Where(x => !x.Deleted && x.IsGranted
                                && x.PersonId == diploma.PersonId
                                && x.RequestingInstitutionId == _userInfo.InstitutionID
                                && x.BasicDocumentId == basicDocumentId
                                && x.DiplomaId == null)
                            .UpdateAsync(x => new DiplomaCreateRequest { DiplomaId = diploma.Id });
                    }

                    await tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();

                    validationResult.Errors.Add(new ValidationError()
                    {
                        Message = ex.GetInnerMostException().Message,
                        ControlID = importModel.ArchiveName,
                        ID = importModel.FileName,
                    });
                }
            }
        }

        private async Task ValidateImport(List<DiplomaImportModel> models, ApiValidationResult validationResult, CancellationToken cancellationToken)
        {
            foreach (DiplomaImportModel importModel in models)
            {
                DiplomaImportParseModel model = importModel.ParseModel;
                int basicDocumentId = model?.Document?.BasicDocumentType ?? default;

                IDiplomaCode validationService = await GetValidationService(basicDocumentId);

                if (validationService == null)
                {
                    validationResult.Errors.Add(new ValidationError()
                    {
                        Message = $"Missing validation service for basic document: {basicDocumentId}",
                        ControlID = importModel.ArchiveName,
                        ID = importModel.FileName,
                        Data = model.Document.ToJson(new JsonSerializerSettings { ContractResolver = new JsonIgnoreResolver(new[] { "Ministry", "MinistrySpecified", "Principal", "Deputy" }) })
                    });

                    continue;
                }

                ApiValidationResult modelValidationResult = await validationService.ValidateImportModel(importModel, cancellationToken);
                validationResult.Merge(modelValidationResult);

                validationResult.DiplomaPerformaceStats.Add(new DiplomaPerformaceStats
                {
                    Type = "Import",
                    PersonalId = model.Person?.PersonalId,
                    PerformanceStats = modelValidationResult.DiplomaPerformaceStats.SelectMany(x => x.PerformanceStats).ToList(),
                });
                _logger.LogInformation(new Exception(JsonConvert.SerializeObject(validationResult.DiplomaPerformaceStats)), "Diploma import stats");

                if (modelValidationResult.HasErrors)
                {
                    // Текущият модел не се валидира. Не продължаваме с промяната или създаването на диплома.
                    continue;
                }
            }
        }

        private async Task<ApiValidationResult> ValidateSign(Diploma diploma)
        {
            ApiValidationResult validationResult = new ApiValidationResult();
            if (diploma == null)
            {
                return validationResult;
            }

            IDiplomaCode validationService = await GetValidationService(diploma.BasicDocumentId);
            if (validationService == null)
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = $"Missing validation service for basic document: {diploma.BasicDocumentId}",
                });

                throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
            }
            ;

            validationResult = await validationService.ValidateSignModel(new DiplomaViewModel
            {
                Id = diploma.Id,
                PersonalId = diploma.PersonalId,
                PersonalIdType = diploma.PersonalIdtype,
                SchoolYear = diploma.SchoolYear,
                FirstName = diploma.FirstName,
                MiddleName = diploma.MiddleName,
                LastName = diploma.LastName,
                BasicDocumentId = diploma.BasicDocumentId,
                IsRuoDocBasicDocument = diploma.BasicDocument.IsRuoDoc,
                InstitutionId = diploma.InstitutionId,
                InstitutionName = diploma.InstitutionName,
                RegistrationNumberTotal = diploma.RegistrationNumberTotal,
                RegistrationNumberYear = diploma.RegistrationNumberYear,
                RegistrationDate = diploma.RegistrationDate,
                AdditionalDocuments = diploma.DiplomaAdditionalDocumentDiplomas
                    .Select(ad => new DiplomaAdditionalDocumentViewModel
                    {
                        BasicDocumentId = ad.BasicDocumentId,
                        Series = ad.Series,
                        FactoryNumber = ad.FactoryNumber,
                        RegistrationNumber = ad.RegistratioNumber,
                        RegistrationNumberYear = ad.RegistrationNumberYear,
                        RegistrationDate = ad.RegistrationDate,
                    })
            });

            validationResult.DiplomaPerformaceStats.Add(new DiplomaPerformaceStats
            {
                Type = "Sign",
                PersonalId = diploma.PersonalId,
                PerformanceStats = validationResult.DiplomaPerformaceStats.SelectMany(x => x.PerformanceStats).ToList(),
            });
            _logger.LogInformation(new Exception(JsonConvert.SerializeObject(validationResult.DiplomaPerformaceStats)), "Diploma sign stats");

            if (validationResult.HasErrors)
            {
                throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
            }

            return validationResult;
        }

        private async Task UpdateDiplomaEntity(DiplomaImportParseModel model, Diploma diploma)
        {
            if (diploma == null) throw new ArgumentNullException(nameof(diploma));
            if (model == null) throw new ArgumentNullException(nameof(model));

            bool isNew = diploma.Id <= 0;

            var person = await _context.People
                    .Where(x => x.PersonalId == model.Person.PersonalId
                        && x.PersonalIdtype == model.Person.PersonalIdType)
                    .Select(x => new
                    {
                        x.PersonId,
                        x.Gender,
                        x.BirthDate
                    })
                    .SingleOrDefaultAsync();

            if (isNew)
            {
                diploma.PersonId = person.PersonId;
                diploma.Gender = person.Gender.Value; // Очакваме в core.Person всички записи да имат Gender. Инак ще изгърми.
                diploma.BirthDate = person.BirthDate;
            }

            // Ресет на флаговете
            diploma.ResetSigningAttrs();
            diploma.ResetIsEditableAttrs();
            diploma.IsFinalized = false;
            diploma.IsPublic = false;

            // Xml секция Person
            diploma.PersonalId = model.Person.PersonalId;
            diploma.PersonalIdtype = model.Person.PersonalIdType;
            diploma.FirstName = model.Person.FirstName;
            diploma.FirstNameLatin = model.Person.FirstNameLatin;
            diploma.MiddleName = model.Person.MiddleName;
            diploma.MiddleNameLatin = model.Person.MiddleNameLatin;
            diploma.LastName = model.Person.LastName;
            diploma.LastNameLatin = model.Person.LastNameLatin;
            diploma.BirthPlaceTown = model.Person.BirthPlaceTown;
            diploma.BirthPlaceMunicipality = model.Person.BirthPlaceMunicipality;
            diploma.BirthPlaceRegion = model.Person.BirthPlaceRegion;

            diploma.Nationality = model.Person.NationalityName;
            if (!string.IsNullOrWhiteSpace(model.Person.Nationality))
            {
                var country = await _context.Countries.FirstOrDefaultAsync(c => c.Code.ToUpper() == model.Person.Nationality.ToUpper());
                diploma.NationalityId = country.CountryId;
            }


            if (model.Document?.BasicDocumentIsRuoDoc ?? false)
            {
                if (_userInfo.RegionID.HasValue)
                {
                    diploma.RuoRegId = _userInfo.RegionID.Value;
                }
                else
                {
                    throw new InvalidOperationException($"{Messages.InvalidRuoDistrictCodeError}");
                }
            }
            else
            {
                // Xml секция Institution
                if (int.TryParse(model.Institution?.InstitutionCode, out int instituttionId))
                {
                    diploma.InstitutionId = instituttionId;
                    diploma.InstitutionName = model.Institution.InstitutionName;
                }
                else
                {
                    throw new InvalidOperationException($"{Messages.InvalidInstitutionCodeError} {model.Institution.InstitutionCode}");
                }

            }

            // Xml секция Document
            diploma.BasicDocumentId = model.Document.BasicDocumentType;
            diploma.BasicDocumentName = model.Document.BasicDocumentTypeName;
            diploma.Series = model.Document.Series;
            diploma.FactoryNumber = model.Document.FactoryNumber;
            diploma.RegistrationNumberTotal = model.Document.RegNumber1;
            diploma.RegistrationNumberYear = model.Document.RegNumber2;
            diploma.RegistrationDate = model.Document.RegDate;
            diploma.ProtocolNumber = model.Document.ProtocolNumber;
            diploma.ProtocolDate = model.Document.ProtocolDate;
            diploma.Principal = model.Document.Principal;
            diploma.Deputy = model.Document.Deputy;
            diploma.MinistryName = model.Document.MinistryName;
            diploma.MinistryId = model.Document.MinistrySpecified
                ? model.Document.Ministry
                : null;
            diploma.LeadTeacher = model.Document.LeadTeacher;

            // Xml секция Education
            diploma.StateExamQualificationGradeText = model.Education.StateExamQualificationGradeText;
            diploma.SchoolYear = model.Education.SchoolYear != default
                ? model.Education.SchoolYear
                : await _institutionService.GetCurrentYear(diploma.InstitutionId);
            diploma.YearGraduated = model.Education.YearGraduated;

            diploma.Gpa = model.Education.Gpa;
            diploma.Gpatext = model.Education.GpaText;

            diploma.EduFormName = model.Education.EducationFormName;
            diploma.EduFormId = model.Education.EducationFormSpecified
                ? model.Education.EducationForm
                : null;

            diploma.EducationTypeId = model.Education.EducationTypeSpecified
                ? model.Education.EducationType
                : null;

            diploma.Vetqualification = model.Education.Qualification;
            diploma.Description = model.Education.Description;

            diploma.ClassTypeName = model.Education.ProfileName;
            diploma.ClassTypeId = model.Education.ProfileSpecified
                ? model.Education.Profile
                : null;


            diploma.SppooprofessionName = model.Education.ProfessionName;
            diploma.SppooprofessionId = model.Education.ProfessionSpecified
                ? model.Education.Profession
                : null;

            diploma.SppoospecialityName = model.Education.SpecialityName;
            diploma.SppoospecialityId = model.Education.SpecialitySpecified
                ? model.Education.Speciality
                : null;

            diploma.EduDuration = model.Education.DurationSpecified
                ? model.Education.Duration
                : null;

            diploma.Nkr = model.Education.NKRSpecified
                ? model.Education.NKR
                : null;

            diploma.Ekr = model.Education.EKRSpecified
                ? model.Education.EKR
                : null;

            diploma.VetLevel = model.Education.VetLevelSpecified
                ? model.Education.VetLevel
                : null;

            diploma.BasicClassId = model.Education.BasicClass;

            diploma.ProfessionPart = model.Education.ProfessionSpecified
                ? model.Education.ProfessionPart
                : null;

            diploma.ItlevelId = model.Education.ITLevelSpecified
                ? model.Education.ITLevel
                : null;

            diploma.StateExamQualificationGrade = model.Education.StateExamQualificationGradeSpecified
                ? model.Education.StateExamQualificationGrade
                : null;

            if (!string.IsNullOrWhiteSpace(model.Education.FLGELevel))
            {
                var flge = await _context.Fllevels.FirstOrDefaultAsync(c => c.Name.ToUpper() == model.Education.FLGELevel.ToUpper());
                diploma.FlgelevelId = flge.FllevelId;
            }

            // Xml секция Commission
            diploma.CommissionOrderNumber = model.Commission?.OrderNo;
            diploma.CommissionOrderData = model.Commission?.OrderDate;
        }

        private async Task UpdateDiplomaSubjects(DiplomaImportParseModel model, Diploma diploma)
        {
            if (diploma == null) throw new ArgumentNullException(nameof(diploma));
            if (model == null) throw new ArgumentNullException(nameof(model));


            // Първо изтриваме всички DiplomaSubjects, ако има такива
            await _context.DiplomaSubjects
                .Where(x => x.DiplomaId == diploma.Id)
                .DeleteAsync();

            // Има документи, които нямат оценки (според Радосвета).
            // Предварително сме направили валидация дали дадения BasicDocument е такъв.
            if (model.Subjects == null || !model.Subjects.Any())
            {
                return;
            }

            BasicDocumentDetailsModel basicDocument = await _lookupService.GetBasicDocumentDetails(model.Document?.BasicDocumentType ?? 0);
            if (basicDocument == null)
            {
                throw new ArgumentException("Непознат тип документ", nameof(basicDocument));
            }

            for (int i = 0; i < model.Subjects.Length; i++)
            {
                Models.Diploma.Import.SubjectType subjectModel = model.Subjects[i];
                DiplomaSubject subjectToAdd = CreateDiplomaSubjectAsync(subjectModel, basicDocument, i);

                if (subjectModel.Modules != null)
                {
                    for (int j = 0; j < subjectModel.Modules.Count(); j++)
                    {
                        MON.Models.Diploma.Import.SubjectType subjectModuleModel = subjectModel.Modules[j];
                        subjectModuleModel.BasicDocumentPartSpecified = true;
                        subjectModuleModel.BasicDocumentPart = subjectToAdd.BasicDocumentPartId;

                        DiplomaSubject subjectModuleToAdd = CreateDiplomaSubjectAsync(subjectModuleModel, basicDocument, j);
                        subjectToAdd.InverseParent.Add(subjectModuleToAdd);
                        diploma.DiplomaSubjects.Add(subjectModuleToAdd);
                    }
                }

                diploma.DiplomaSubjects.Add(subjectToAdd);
            }
        }

        private DiplomaSubject CreateDiplomaSubjectAsync(Models.Diploma.Import.SubjectType subjectModel, BasicDocumentDetailsModel basicDocument, int index)
        {
            DiplomaSubject subjectToAdd = new DiplomaSubject
            {
                SubjectName = subjectModel.SubjectName,
                SubjectTypeId = subjectModel.SubjectType1,
                FlLevel = subjectModel.FlLevel,
                FlSubjectName = subjectModel.FlSubjectName,
                Position = subjectModel.PositionSpecified
                     ? (subjectModel.Position ?? (index + 1))
                     : index + 1
            };

            switch (subjectModel.Item)
            {
                case Models.Diploma.Import.GradeType normalGrade:
                    subjectToAdd.GradeCategory = (int)GradeCategoryEnum.Normal;
                    subjectToAdd.Grade = normalGrade.Grade == 0m ? (decimal?)null : normalGrade.Grade;
                    subjectToAdd.GradeText = normalGrade.Grade == 0m ? null : normalGrade.GradeText;
                    break;
                case SpecialNeedsGradeType specialNeedsGrade:
                    subjectToAdd.GradeCategory = (int)GradeCategoryEnum.SpecialNeeds;
                    subjectToAdd.SpecialNeedsGrade = specialNeedsGrade.Grade;
                    break;
                case OtherGradeType otherGrade:
                    subjectToAdd.GradeCategory = (int)GradeCategoryEnum.Other;
                    subjectToAdd.OtherGrade = otherGrade.Grade;
                    break;
                case NoGradeType noGrade:
                    subjectToAdd.GradeCategory = (int)GradeCategoryEnum.SubSection;
                    break;
                case QualitativeGradeType qualitativeGrade:
                    subjectToAdd.GradeCategory = (int)GradeCategoryEnum.Qualitative;
                    subjectToAdd.QualitativeGrade = qualitativeGrade.Grade;
                    break;
                case null:
                    break;
            }

            subjectToAdd.SubjectId = subjectModel.SubjectIdSpecified && subjectToAdd.GradeCategory != (int)GradeCategoryEnum.SubSection
                ? (subjectModel.SubjectId != 0 ? subjectModel.SubjectId : null)
                : null;

            subjectToAdd.Horarium = subjectModel.HorariumSpecified
                ? subjectModel.Horarium
                : null;

            subjectToAdd.FlSubjectId = subjectModel.FlSubjectIdSpecified
               ? (int?)subjectModel.FlSubjectId
               : null;

            subjectToAdd.FlHorarium = subjectModel.FlHorariumSpecified
                ? subjectModel.FlHorarium
                : null;

            subjectToAdd.Nvopoints = subjectModel.PointsSpecified
               ? (decimal?)subjectModel.Points
               : null;

            //subjectToAdd.FlSubjectId = subjectModel.FlSubjectIdSpecified
            //   ? (int?)subjectModel.FlSubjectId
            //   : null;

            subjectToAdd.Ects = subjectModel.ECTS;

            if (subjectModel.BasicDocumentPartSpecified)
            {
                subjectToAdd.BasicDocumentPartId = subjectModel.BasicDocumentPart.Value;
            }
            else
            {
                // Определяне на BasicDocumentPartId в случай, че не е подаден
                int? basicDocumentPartId = null;
                if (subjectModel.GradeTypeSpecified && subjectModel.GradeType.HasValue)
                {
                    BasicDocumentPartDetailsModel docPart = basicDocument.DocumetPartsDetails.FirstOrDefault(x => x.ExternalEvaluationTypes != null
                        && x.ExternalEvaluationTypes.Any(e => e == subjectModel.GradeType.Value));
                    basicDocumentPartId = docPart?.Id;
                }
                else
                {
                    BasicDocumentPartDetailsModel docPart = basicDocument.DocumetPartsDetails.FirstOrDefault(x => x.SubjectTypes != null
                       && x.SubjectTypes.Any(e => e == subjectModel.SubjectType1));
                    basicDocumentPartId = docPart?.Id;
                }
                subjectToAdd.BasicDocumentPartId = basicDocumentPartId ?? throw new ArgumentException($"Не е възможно да се определи секцията за оценка => SubjectId: {subjectModel.SubjectId} / SubjectType: {subjectModel.SubjectType1} / GradeType: {subjectModel.GradeType}");
            }

            return subjectToAdd;
        }

        private async Task UpdateDiplomaAdditionalDocuments(DiplomaImportParseModel model, Diploma diploma)
        {
            if (diploma == null) throw new ArgumentNullException(nameof(diploma));
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Първо изтриваме всички DiplomaAdditionalDocuments, ако има такива
            await _context.DiplomaAdditionalDocuments
                .Where(x => x.DiplomaId == diploma.Id)
                .DeleteAsync();

            var basicDocuments = await _context.BasicDocuments
                .Select(x => new
                {
                    x.Id,
                    x.MainBasicDocuments
                })
                .ToListAsync();

            if (model.AdditionalDocuments != null && model.AdditionalDocuments.Any())
            {
                foreach (var addDoc in model.AdditionalDocuments)
                {
                    string instCode = addDoc.Institution?.InstitutionCode;
                    bool hasCorrectInstCode = false;
                    int institutionId = default;
                    if (!string.IsNullOrWhiteSpace(instCode) && int.TryParse(instCode, out institutionId))
                    {
                        hasCorrectInstCode = await _context.Institutions.AnyAsync(x => x.InstitutionId == institutionId);
                    }

                    diploma.DiplomaAdditionalDocumentDiplomas.Add(new DataAccess.DiplomaAdditionalDocument
                    {
                        BasicDocumentId = basicDocuments.Any(bd => bd.Id == (addDoc.BasicDocumentType ?? int.MinValue)) ? addDoc.BasicDocumentType : null,
                        BasicDocumentName = addDoc.BasicDocumentTypeName,
                        MainDiplomaId = addDoc.MainDiploma,
                        InstitutionId = hasCorrectInstCode ? institutionId : (int?)null,
                        InstitutionName = addDoc.Institution?.InstitutionName,
                        InstitutionAddress = addDoc.Institution?.InstitutionAddress,
                        Town = addDoc.Institution?.Town,
                        Municipality = addDoc.Institution?.Municipality,
                        Region = addDoc.Institution?.Region,
                        LocalArea = addDoc.Institution?.LocalArea,
                        Series = addDoc.Series,
                        FactoryNumber = addDoc.FactoryNumber,
                        RegistratioNumber = addDoc.RegNumber1,
                        RegistrationNumberYear = addDoc.RegNumber2,
                        RegistrationDate = addDoc.RegDate
                    });
                }
            }

            string[] splitStr = (basicDocuments.FirstOrDefault(x => x.Id == diploma.BasicDocumentId)?.MainBasicDocuments ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> ids = splitStr.ToHashSet<int>();

            diploma.OriginalDiplomaId = diploma.DiplomaAdditionalDocumentDiplomas
                .FirstOrDefault(x => x.MainDiplomaId.HasValue && x.BasicDocumentId.HasValue && ids.Contains(x.BasicDocumentId.Value))?.MainDiplomaId;
        }

        private async Task UpdateCommissionMembers(DiplomaImportParseModel model, Diploma diploma)
        {
            if (diploma == null) throw new ArgumentNullException(nameof(diploma));
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Първо изтриваме всички DiplomaAdditionalDocuments, ако има такива
            await _context.GraduationCommissionMembers
                .Where(x => x.DiplomaId == diploma.Id)
                .DeleteAsync();

            if (model.Commission != null && !model.Commission.Member.IsNullOrEmpty())
            {
                foreach (var member in model.Commission.Member)
                {
                    diploma.GraduationCommissionMembers.Add(new DataAccess.GraduationCommissionMember
                    {
                        Position = member.Position,
                        FullName = member.Name.Truncate(1000),
                        FullNameLatin = member.NameLatin.Truncate(1000)
                    });
                }
            }
        }

        private async Task UpdateDiplomaDocuments(DiplomaImportParseModel model, Diploma diploma)
        {
            if (diploma == null) throw new ArgumentNullException(nameof(diploma));
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Първо изтриваме всички DiplomaAdditionalDocuments, ако има такива
            await _context.DiplomaDocuments
                .Where(x => x.DiplomaId == diploma.Id)
                .DeleteAsync();

            if (model.Images != null && model.Images.Any(x => !x.Contents.IsNullOrWhiteSpace()))
            {
                foreach (var doc in model.Images.Where(x => !x.Contents.IsNullOrWhiteSpace()))
                {
                    var imageContents = Convert.FromBase64String(doc.Contents);
                    imageContents = await _imageService.Resize(imageContents, 2000, 2000);
                    imageContents = await _imageService.Compress(imageContents, ImageCompressionLevelEnum.High);
                    var blobDO = await _blobService.UploadFileAsync(imageContents, $"position_{doc.Position}", doc.ContentType);
                    if (blobDO.IsError)
                    {
                        throw new InvalidOperationException($"Blob server error: {blobDO.Message}");
                    }

                    diploma.DiplomaDocuments.Add(new DataAccess.DiplomaDocument
                    {
                        BlobId = blobDO.Data.BlobId,
                        Position = doc.Position,
                        Description = blobDO.Data.Name
                    });
                }
            }
        }


        #endregion

        /// <summary>
        /// Списък с дипломи или документи за валидиране
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IPagedList<DiplomaViewModel>> List(DiplomaListInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            (bool hasDiplomaManagePermission, bool filterByInstitution) = await CheckForManagePermission(input.PersonId);

            if (!hasDiplomaManagePermission)
            {
                // Липсва право за редакция. Ще проверим за право за четене.
                (bool hasDiplomaReadPermission, bool filterByInstitution1) = await CheckForReadPermission(input.PersonId);
                filterByInstitution = filterByInstitution || filterByInstitution1;

                if (!hasDiplomaReadPermission)
                {
                    // Липсва и право за четене
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            IQueryable<VDiplomaList> query = _context.VDiplomaLists
                .AsNoTracking();

            if (input.IsValidation.HasValue)
            {
                query = query.Where(x => x.IsValidationBasicDocument == input.IsValidation.Value);
            }

            if (input.IsEqualization.HasValue)
            {
                query = query.Where(x => x.IsRuoDocBasicDocument == input.IsEqualization.Value);
            }

            if (input.Year.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.Year.Value);
            }

            if (input.PersonId.HasValue)
            {
                query = query.Where(x => x.PersonId == input.PersonId.Value);
            }

            if (input.IsSigned.HasValue)
            {
                query = query.Where(x => x.IsSigned == input.IsSigned.Value);
            }

            if (filterByInstitution)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            }

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => (x.RegionId != null && x.RegionId == _userInfo.RegionID)
                 || (x.RuoRegId != null));
            }

            if (input.BasicDocuments.Length > 0)
            {
                query = query.Where(x => input.BasicDocuments.Contains(x.BasicDocumentId));
            }

            if (!input.PersonalId.IsNullOrEmpty())
            {
                query.Where(x => x.PersonalId.Contains(input.PersonalId));
            }

            if (!input.PinFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("PersonalId", typeof(string), input.PinFilterOp, input.PinFilter);
            }

            if (!input.NameFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("FullName", typeof(string), input.NameFilterOp, input.NameFilter);
            }

            if (!input.SeriesFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("Series", typeof(string), input.SeriesFilterOp, input.SeriesFilter);
            }

            if (!input.FactoryNumbeFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("FactoryNumber", typeof(string), input.FactoryNumbeFilterOp, input.FactoryNumbeFilter);
            }

            if (!input.InstitutionIdFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("InstitutionId", typeof(int), input.InstitutionIdFilterOp, input.InstitutionIdFilter);
            }

            if (!input.BasicDocumentTypeFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("BasicDocumentName", typeof(string), input.BasicDocumentTypeFilterOp, input.BasicDocumentTypeFilter);
            }

            if (!input.SchoolYearFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("SchoolYearName", typeof(string), input.SchoolYearFilterOp, input.SchoolYearFilter);
            }

            if (!input.RegionNameFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("RuoRegName", typeof(string), input.RegionNameFilterOp, input.RegionNameFilter);
            }

            bool hasManagePermission = hasDiplomaManagePermission
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaManage)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage);
            bool hasDiplomaAdminManagePermission = await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage);

            if (input.FilterForSigning ?? false)
            {
                query = query.Where(x => hasManagePermission && !x.IsCancelled && !x.IsSigned
                    && ((_userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID.Value) || hasDiplomaAdminManagePermission || (x.CreatedByMonHrRole && _userInfo.IsMonHR)));
            }

            IQueryable<DiplomaViewModel> listQuery = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.BasicDocumentName.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.FullName.Contains(input.Filter)
                   || predicate.PersonalId.Contains(input.Filter)
                   || predicate.Series.Contains(input.Filter)
                   || predicate.FactoryNumber.Contains(input.Filter))
                .Select(x => new DiplomaViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    BasicDocumentId = x.BasicDocumentId,
                    BasicDocumentType = x.BasicDocumentName,
                    InstitutionId = x.InstitutionId,
                    YearGraduated = x.YearGraduated,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearName,
                    FullName = x.FullName,
                    PersonalId = x.PersonalId,
                    PersonalIdTypeName = x.PersonalIdTypeName,
                    Series = x.Series,
                    FactoryNumber = x.FactoryNumber,
                    ProtocolNumber = x.ProtocolNumber,
                    ProtocolDate = x.ProtocolDate,
                    RegistrationNumberTotal = x.RegistrationNumberTotal,
                    RegistrationNumberYear = x.RegistrationNumberYear,
                    RegistrationDate = x.RegistrationDate,
                    IsPublic = x.IsPublic,
                    IsSigned = x.IsSigned,
                    IsMigrated = x.IsMigrated,
                    IsCancelled = x.IsCancelled,
                    IsFinalized = x.IsFinalized,
                    CreateDate = x.CreateDate,
                    CreatorUsername = x.CreatorUsername,
                    ModifyDate = x.ModifyDate,
                    UpdaterUsername = x.UpdaterUsername,
                    SignedDate = x.SignedDate,
                    SignerUsername = x.SignerUsername,
                    IsEditable = x.IsEditable,
                    EditableSetDate = x.EditableSetDate,
                    EditableSetReason = x.EditableSetReason,
                    EditableSetUsername = x.EditableSetUsername,
                    ReportFormPath = x.ReportFormPath,
                    CreatedByMonHrRole = x.CreatedByMonHrRole,
                    RegionId = x.RegionId,
                    IsIncludedInRegister = x.IsIncludedInRegister,
                    RuoRegId = x.RuoRegId,
                    RuoRegCode = x.RuoRegCode,
                    RuoRegName = x.RuoRegName,
                })
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

            int totalCount = listQuery.Count();
            IList<DiplomaViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();


            foreach (DiplomaViewModel item in items)
            {
                item.CanBeSigned = !item.IsNotSignable && hasManagePermission && !item.IsCancelled && !item.IsSigned
                    && (
                        (_userInfo.InstitutionID.HasValue && item.InstitutionId == _userInfo.InstitutionID.Value)
                        || hasDiplomaAdminManagePermission
                        || (item.CreatedByMonHrRole && _userInfo.IsMonHR)
                        || ((_userInfo.IsRuo || _userInfo.IsRuoExpert) && item.RuoRegId.HasValue && item.RuoRegId.Value == _userInfo.RegionID)
                        );

                item.CanBeDeleted = hasManagePermission && !item.IsSigned && !item.IsFinalized && !item.IsCancelled
                    && (
                        (_userInfo.InstitutionID.HasValue && item.InstitutionId == _userInfo.InstitutionID.Value)
                        || hasDiplomaAdminManagePermission
                        || (item.CreatedByMonHrRole && _userInfo.IsMonHR)
                        || ((_userInfo.IsRuo || _userInfo.IsRuoExpert) && item.RuoRegId.HasValue && item.RuoRegId.Value == _userInfo.RegionID)
                        );

                item.CanBeEdit = hasManagePermission && (item.IsEditable || (!item.IsSigned && !item.IsFinalized && !item.IsCancelled))
                    && (
                        (_userInfo.InstitutionID.HasValue && item.InstitutionId == _userInfo.InstitutionID.Value)
                        || hasDiplomaAdminManagePermission
                        || (item.CreatedByMonHrRole && _userInfo.IsMonHR)
                        || ((_userInfo.IsRuo || _userInfo.IsRuoExpert) && item.RuoRegId.HasValue && item.RuoRegId.Value == _userInfo.RegionID)
                        );

                item.CanBeSetAsEditable = hasManagePermission && (item.IsMigrated || item.IsSigned) && !item.IsCancelled && !item.IsEditable
                    && (
                        (_userInfo.InstitutionID.HasValue && item.InstitutionId == _userInfo.InstitutionID.Value)
                        || hasDiplomaAdminManagePermission
                        || (item.CreatedByMonHrRole && _userInfo.IsMonHR)
                        || ((_userInfo.IsRuo || _userInfo.IsRuoExpert) && item.RuoRegId.HasValue && item.RuoRegId.Value == _userInfo.RegionID)
                        );

                if (item.IsCancelled)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.IsCancelled),
                        LocalizationKey = "diplomas.isAnulledStatus",
                        Color = "error"
                    });
                }

                if (item.IsPublic)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.IsPublic),
                        LocalizationKey = "diplomas.isPublicStatus",
                        Color = "success"
                    });
                }

                if (item.IsSigned)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.IsSigned),
                        LocalizationKey = "diplomas.isSignedStatus",
                        Color = "primary"
                    });
                }

                if (item.IsEditable)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.IsEditable),
                        LocalizationKey = "diplomas.isEditableStatus",
                        Color = "info"
                    });
                }
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<VStudentDiploma>> ListUnimported(DiplomaListInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            (bool hasDiplomaReadPermission, _) = await CheckForReadPermission(null);

            if (!hasDiplomaReadPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VStudentDiploma> query = _context.VStudentDiplomas
                .Where(x => (x.DiplomaId == null || x.DiplomaIsSigned != true)
                    && x.StudentClassIsCurrent == true)
                .AsNoTracking();

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.StudentClassInstitutionId == _userInfo.InstitutionID);
            }
            else
            {
                // Логнатия потребител няма InstitutionID или тя се различава от тази на интутцията.
                // Проверяваме за някакви администраторски права.
                string[] requiredPermissions = new string[]
                {
                    DefaultPermissions.PermissionNameForAdminDiplomaRead,
                    DefaultPermissions.PermissionNameForAdminDiplomaManage,
                    DefaultPermissions.PermissionNameForMonHrDiplomaRead,
                    DefaultPermissions.PermissionNameForMonHrDiplomaManage,
                };

                if (!await _authorizationService.AuthorizeUser(requiredPermissions, true))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            if (input.Year.HasValue)
            {
                query = query.Where(x => x.StudentClassSchoolYear == input.Year.Value);
            }

            if (!input.PersonalId.IsNullOrEmpty())
            {
                query.Where(x => x.PersonalId.Contains(input.PersonalId));
            }

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => x.RegionId != null && x.RegionId == _userInfo.RegionID);
            }

            IQueryable<VStudentDiploma> listQuery = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.BasicDocumentName.Contains(input.Filter)
                   || predicate.StudentClassSchoolYearName.Contains(input.Filter)
                   || predicate.FullName.Contains(input.Filter)
                   || predicate.PersonalId.Contains(input.Filter)
                   || predicate.PersonalIdTypeName.Contains(input.Filter)
                   || predicate.StudentClassInstitutionName.Contains(input.Filter)
                   || predicate.StudentClassInstitutionId.ToString().Contains(input.Filter))
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "StudentClassId asc" : input.SortBy);

            int totalCount = listQuery.Count();
            IList<VStudentDiploma> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<DiplomaBasicDetailsModel> GetBasicDetails(int diplomaId)
        {
            (bool hasDiplomaReadPermission, _) = await CheckForReadPermission(null);

            if (!hasDiplomaReadPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.Diplomas
                .Where(x => x.Id == diplomaId)
                .Select(x => new DiplomaBasicDetailsModel
                {
                    Id = x.Id,
                    IsSigned = x.IsSigned,
                    IsEditable = x.IsEditable,
                    IsFinalized = x.IsFinalized,
                    IsCancelled = x.IsCancelled,
                    AttachedImagesCountMin = x.BasicDocument.AttachedImagesCountMin,
                    AttachedImagesCountMax = x.BasicDocument.AttachedImagesCountMax
                })
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Типове книги
        /// </summary>
        /// <param name="regBookType"></param>
        /// <returns></returns>
        public async Task<IList<DropdownViewModel>> GetRegBookBasicDocuments(RegBookTypeEnum regBookType)
        {
            return regBookType switch
            {
                RegBookTypeEnum.RegBookQualification => await _context.VRegBookQualificationBasicDocuments
                                        .Select(x => new DropdownViewModel
                                        {
                                            Value = x.Id,
                                            Name = x.Name,
                                            Text = x.Name
                                        })
                                        .ToListAsync(),
                RegBookTypeEnum.RegBookQualificationDuplicate => await _context.VRegBookQualificationDuplicateBasicDocuments
                                        .Select(x => new DropdownViewModel
                                        {
                                            Value = x.Id,
                                            Name = x.Name,
                                            Text = x.Name
                                        })
                                        .ToListAsync(),
                RegBookTypeEnum.RegBookCertificate => await _context.VRegBookCertificateBasicDocuments
                                        .Select(x => new DropdownViewModel
                                        {
                                            Value = x.Id,
                                            Name = x.Name,
                                            Text = x.Name
                                        })
                                        .ToListAsync(),
                RegBookTypeEnum.RegBookCertificateDuplicate => await _context.VRegBookCertificateDuplicateBasicDocuments
                                        .Select(x => new DropdownViewModel
                                        {
                                            Value = x.Id,
                                            Name = x.Name,
                                            Text = x.Name
                                        })
                                        .ToListAsync(),
                _ => await Task.FromResult<IList<DropdownViewModel>>(null),
            };
        }

        /// <summary>
        /// Филтриране на регистрационни книги по общи колони
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="regBookList"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private IQueryable<T> FilterRegBooks<T>(IQueryable<T> regBookList, RegBookListInput input)
        {
            IQueryable<T> result = regBookList;
            if (!input.PinFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("PersonalId", typeof(string), input.PinFilterOp, input.PinFilter);
            }

            if (!input.RegistrationNumberTotalFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("RegistrationNumberTotal", typeof(string), input.RegistrationNumberTotalFilterOp, input.RegistrationNumberTotalFilter);
            }

            if (!input.RegistrationNumberYearFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("RegistrationNumberYear", typeof(string), input.RegistrationNumberYearFilterOp, input.RegistrationNumberYearFilter);
            }


            if (!input.SeriesFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("Series", typeof(string), input.SeriesFilterOp, input.SeriesFilter);
            }

            if (!input.FactoryNumberFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("FactoryNumber", typeof(string), input.FactoryNumberFilterOp, input.FactoryNumberFilter);
            }

            if (!input.InstitutionIdFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("InstitutionId", typeof(int), input.InstitutionIdFilterOp, input.InstitutionIdFilter);
            }

            if (!input.BasicDocumentTypeFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("BasicDocumentName", typeof(string), input.BasicDocumentTypeFilterOp, input.BasicDocumentTypeFilter);
            }

            if (!input.SchoolYearFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("SchoolYearName", typeof(string), input.SchoolYearFilterOp, input.SchoolYearFilter);
            }

            if (!input.PersonNameFilter.IsNullOrWhiteSpace())
            {
                result = regBookList.Where("FullName", typeof(string), input.PersonNameFilterOp, input.PersonNameFilter);
            }

            return result;
        }

        public async Task<IPagedList<RegBookDetailsModel>> GetRegBookList(RegBookListInput input)
        {
            var regBookQuery = await GetRegBookListAll(input);

            int totalCount = await regBookQuery.CountAsync();
            IList<RegBookDetailsModel> items = await regBookQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        /// <summary>
        /// Списък с дипломи от рег. книги
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IQueryable<RegBookDetailsModel>> GetRegBookListAll(RegBookListInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (!(input.Year.HasValue && input.BasicDocumentId.HasValue))
            {
                // Генерира празен списък, който имплементира IAsyncEnumerable
                return _context.VRegBookQualifications.Where(i => false).Select(i => new RegBookDetailsModel());
            }

            if (!_userInfo.InstitutionID.HasValue && !input.InstitutionId.HasValue)
            {
                // Генерира празен списък, който имплементира IAsyncEnumerable
                return _context.VRegBookQualifications.Where(i => false).Select(i => new RegBookDetailsModel());
            }

            IQueryable<VRegBookQualification> qualificationsViewQuery;
            IQueryable<VRegBookQualificationDuplicate> qualificationDuplicatesViewQuery;
            IQueryable<VRegBookCertificate> certificatesViewQuery;
            IQueryable<VRegBookCertificateDuplicate> certificateDuplicatesViewQuery;

            List<PersonalIdTypeEnum> personalIdTypes = new List<PersonalIdTypeEnum>
            {
                PersonalIdTypeEnum.NoID,
                PersonalIdTypeEnum.EGN,
                PersonalIdTypeEnum.LNCH,
                PersonalIdTypeEnum.IDN
            };

            switch (input.RegBookType)
            {
                case RegBookTypeEnum.RegBookQualification:
                    qualificationsViewQuery = _context.VRegBookQualifications.AsNoTracking();

                    if (_userInfo.InstitutionID.HasValue)
                    {
                        qualificationsViewQuery = qualificationsViewQuery.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    }

                    if (_userInfo.RegionID.HasValue)
                    {
                        qualificationsViewQuery = qualificationsViewQuery.Where(x => x.RegionId == _userInfo.RegionID);
                    }

                    if (input.Year.HasValue)
                    {
                        qualificationsViewQuery = qualificationsViewQuery.Where(x => x.SchoolYear == input.Year.Value);
                    }

                    if (input.PersonId.HasValue)
                    {
                        qualificationsViewQuery = qualificationsViewQuery.Where(x => x.PersonId == input.PersonId.Value);
                    }

                    if (input.InstitutionId.HasValue)
                    {
                        qualificationsViewQuery = qualificationsViewQuery.Where(x => x.InstitutionId == input.InstitutionId.Value);
                    }

                    qualificationsViewQuery = FilterRegBooks<VRegBookQualification>(qualificationsViewQuery, input);

                    IQueryable<RegBookDetailsModel> qualificationsList = (
                        from x in qualificationsViewQuery
                        join idType in _context.PersonalIdtypes on x.PersonalIdtype equals idType.PersonalIdtypeId
                        join basicDocument in _context.BasicDocuments on x.BasicDocumentId equals basicDocument.Id
                        join schoolYear in _context.CurrentYears on x.SchoolYear equals schoolYear.CurrentYearId
                        join edu in _context.EduForms on x.EduFormId equals edu.ClassEduFormId into eduFormTable
                        from eduForm in eduFormTable.DefaultIfEmpty()
                        join sppooSpecialty in _context.Sppoospecialities on x.SppoospecialityId equals sppooSpecialty.SppoospecialityId into specialty
                        from subSppooSpecialty in specialty.DefaultIfEmpty()
                        join sppooProfession in _context.Sppooprofessions on x.SppooprofessionId equals sppooProfession.SppooprofessionId
                        into profession
                        from subSppooProfession in profession.DefaultIfEmpty()
                        join classType in _context.ClassTypes on x.ClassTypeId equals classType.ClassTypeId
                        into classTypeName
                        from subClassType in classTypeName.DefaultIfEmpty()
                        where x.BasicDocumentId == input.BasicDocumentId
                            && (input.Filter == null || (basicDocument.Name.Contains(input.Filter) || x.InstitutionId.ToString().Contains(input.Filter) ||
                                schoolYear.Name.Contains(input.Filter) || x.FullName.Contains(input.Filter) ||
                                x.PersonalId.Contains(input.Filter) || x.Series.Contains(input.Filter) || x.FactoryNumber.Contains(input.Filter))
                            )
                        select new RegBookDetailsModel
                        {
                            Id = x.Id,
                            BasicDocumentAbbr = basicDocument.Abbreviation,
                            BasicDocumentName = basicDocument.Name,
                            RegistrationNumberTotal = x.RegistrationNumberTotal,
                            RegistrationNumberYear = x.RegistrationNumberYear,
                            RegistrationDate = x.RegistrationDate,
                            PersonFullName = x.FullName,
                            PersonalId = x.PersonalId,
                            PersonId = x.PersonId,
                            EduForm = eduForm != null ? eduForm.Name : "",
                            Gpa = x.Gpa,
                            Series = x.Series,
                            FactoryNumber = x.FactoryNumber,
                            Canceled = x.IsCancelled == true,
                            IsAnulledStatus = x.IsCancelled == false ? "Не" : "Да",
                            SchoolYear = x.SchoolYear,
                            SchoolYearName = schoolYear.Name,
                            PersonalIdTypeName = idType.Name,
                            EducationSpecialization = $"{subClassType.Name ?? subSppooProfession.Name + " / " + subSppooSpecialty.Name}",
                            Spacer = " "
                        })
                        .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

                    return qualificationsList;
                case RegBookTypeEnum.RegBookQualificationDuplicate:
                    qualificationDuplicatesViewQuery = _context.VRegBookQualificationDuplicates.AsNoTracking();

                    if (_userInfo.InstitutionID.HasValue)
                    {
                        qualificationDuplicatesViewQuery = qualificationDuplicatesViewQuery.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    }

                    if (_userInfo.RegionID.HasValue)
                    {
                        qualificationDuplicatesViewQuery = qualificationDuplicatesViewQuery.Where(x => x.RegionId == _userInfo.RegionID);
                    }

                    if (input.Year.HasValue)
                    {
                        qualificationDuplicatesViewQuery = qualificationDuplicatesViewQuery.Where(x => x.SchoolYear == input.Year.Value);
                    }

                    if (input.PersonId.HasValue)
                    {
                        qualificationDuplicatesViewQuery = qualificationDuplicatesViewQuery.Where(x => x.PersonId == input.PersonId.Value);
                    }

                    if (input.InstitutionId.HasValue)
                    {
                        qualificationDuplicatesViewQuery = qualificationDuplicatesViewQuery.Where(x => x.InstitutionId == input.InstitutionId.Value);
                    }

                    qualificationDuplicatesViewQuery = FilterRegBooks<VRegBookQualificationDuplicate>(qualificationDuplicatesViewQuery, input);

                    IQueryable<RegBookDetailsModel> qualificationDuplicatesList = (
                        from x in qualificationDuplicatesViewQuery
                        join basicDocument in _context.BasicDocuments on x.BasicDocumentId equals basicDocument.Id
                        join idType in _context.PersonalIdtypes on x.PersonalIdtype equals idType.PersonalIdtypeId
                        join schoolYear in _context.CurrentYears on x.SchoolYear equals schoolYear.CurrentYearId
                        join eduForm in _context.EduForms on x.EduFormId equals eduForm.ClassEduFormId
                        join sppooSpecialty in _context.Sppoospecialities on x.SppoospecialityId equals sppooSpecialty.SppoospecialityId
                        into specialty
                        from subSppooSpecialty in specialty.DefaultIfEmpty()
                        join sppooProfession in _context.Sppooprofessions on x.SppooprofessionId equals sppooProfession.SppooprofessionId
                        into profession
                        from subSppooProfession in profession.DefaultIfEmpty()
                        join classType in _context.ClassTypes on x.ClassTypeId equals classType.ClassTypeId
                        into classTypeName
                        from subClassType in classTypeName.DefaultIfEmpty()
                        where x.BasicDocumentId == input.BasicDocumentId
                            && (input.Filter == null || (basicDocument.Name.Contains(input.Filter) || x.InstitutionId.ToString().Contains(input.Filter) ||
                                schoolYear.Name.Contains(input.Filter) || x.FullName.Contains(input.Filter) ||
                                x.PersonalId.Contains(input.Filter) || x.Series.Contains(input.Filter) || x.FactoryNumber.Contains(input.Filter))
                            )
                        select new RegBookDetailsModel
                        {
                            Id = x.Id,
                            BasicDocumentAbbr = basicDocument.Abbreviation,
                            BasicDocumentName = basicDocument.Name,
                            RegistrationNumberTotal = x.RegistrationNumberTotal,
                            RegistrationNumberYear = x.RegistrationNumberYear,
                            RegistrationDate = x.RegistrationDate,
                            PersonFullName = x.FullName,
                            PersonalId = x.PersonalId,
                            PersonId = x.PersonId,
                            EduForm = eduForm.Name,
                            Series = x.Series,
                            FactoryNumber = x.FactoryNumber,
                            Canceled = x.IsCancelled == true,
                            IsAnulledStatus = x.IsCancelled == false ? "Не" : "Да",
                            SchoolYear = x.SchoolYear,
                            SchoolYearName = schoolYear.Name,
                            PersonalIdTypeName = idType.Name,
                            EducationSpecialization = $"{subClassType.Name ?? subSppooProfession.Name + " / " + subSppooSpecialty.Name}",
                            Spacer = "",
                            OriginalRegistrationNumberYear = x.OrigRegistrationNumberYear,
                            OriginalFactoryNumber = x.OrigFactoryNumber,
                            OriginalRegistrationDate = x.OrigRegistrationDate,
                            OriginalRegistrationNumber = x.OrigRegistrationNumber,
                            OriginalSeries = x.OrigSeries
                        })
                        .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

                    return qualificationDuplicatesList;
                case RegBookTypeEnum.RegBookCertificate:
                    certificatesViewQuery = _context.VRegBookCertificates.AsNoTracking();

                    if (_userInfo.InstitutionID.HasValue)
                    {
                        certificatesViewQuery = certificatesViewQuery.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    }

                    if (_userInfo.RegionID.HasValue)
                    {
                        certificatesViewQuery = certificatesViewQuery.Where(x => x.RegionId == _userInfo.RegionID);
                    }

                    if (input.Year.HasValue)
                    {
                        certificatesViewQuery = certificatesViewQuery.Where(x => x.SchoolYear == input.Year.Value);
                    }

                    if (input.PersonId.HasValue)
                    {
                        certificatesViewQuery = certificatesViewQuery.Where(x => x.PersonId == input.PersonId.Value);
                    }

                    if (input.InstitutionId.HasValue)
                    {
                        certificatesViewQuery = certificatesViewQuery.Where(x => x.InstitutionId == input.InstitutionId.Value);
                    }

                    certificatesViewQuery = FilterRegBooks<VRegBookCertificate>(certificatesViewQuery, input);

                    IQueryable<RegBookDetailsModel> certificationsList = (
                        from x in certificatesViewQuery
                        join basicDocument in _context.BasicDocuments on x.BasicDocumentId equals basicDocument.Id
                        join idType in _context.PersonalIdtypes on x.PersonalIdtype equals idType.PersonalIdtypeId
                        join schoolYear in _context.CurrentYears on x.SchoolYear equals schoolYear.CurrentYearId
                        join edu in _context.EduForms on x.EduFormId equals edu.ClassEduFormId into eduFormTable
                        from eduForm in eduFormTable.DefaultIfEmpty()
                        where x.BasicDocumentId == input.BasicDocumentId
                            && (input.Filter == null || (basicDocument.Name.Contains(input.Filter) || x.InstitutionId.ToString().Contains(input.Filter) ||
                                schoolYear.Name.Contains(input.Filter) || x.FullName.Contains(input.Filter) ||
                                x.PersonalId.Contains(input.Filter) || x.Series.Contains(input.Filter) || x.FactoryNumber.Contains(input.Filter))
                            )
                        select new RegBookDetailsModel
                        {
                            Id = x.Id,
                            BasicDocumentAbbr = basicDocument.Abbreviation,
                            BasicDocumentName = basicDocument.Name,
                            RegistrationNumberTotal = x.RegistrationNumberTotal,
                            RegistrationNumberYear = x.RegistrationNumberYear,
                            RegistrationDate = x.RegistrationDate,
                            PersonFullName = x.FullName,
                            PersonalId = x.PersonalId,
                            PersonalIdTypeName = idType.Name,
                            PersonId = x.PersonId,
                            EduForm = eduForm != null ? eduForm.Name : "",
                            Series = x.Series,
                            FactoryNumber = x.FactoryNumber,
                            Canceled = x.IsCancelled == true,
                            IsAnulledStatus = x.IsCancelled == false ? "Не" : "Да",
                            SchoolYear = x.SchoolYear,
                            SchoolYearName = schoolYear.Name,
                            Spacer = " ",
                        })
                        .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

                    return certificationsList;
                case RegBookTypeEnum.RegBookCertificateDuplicate:
                    certificateDuplicatesViewQuery = _context.VRegBookCertificateDuplicates.AsNoTracking();

                    if (_userInfo.InstitutionID.HasValue)
                    {
                        certificateDuplicatesViewQuery = certificateDuplicatesViewQuery.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    }

                    if (_userInfo.RegionID.HasValue)
                    {
                        certificateDuplicatesViewQuery = certificateDuplicatesViewQuery.Where(x => x.RegionId == _userInfo.RegionID);
                    }

                    if (input.Year.HasValue)
                    {
                        certificateDuplicatesViewQuery = certificateDuplicatesViewQuery.Where(x => x.SchoolYear == input.Year.Value);
                    }

                    if (input.PersonId.HasValue)
                    {
                        certificateDuplicatesViewQuery = certificateDuplicatesViewQuery.Where(x => x.PersonId == input.PersonId.Value);
                    }

                    if (input.InstitutionId.HasValue)
                    {
                        certificateDuplicatesViewQuery = certificateDuplicatesViewQuery.Where(x => x.InstitutionId == input.InstitutionId.Value);
                    }

                    certificateDuplicatesViewQuery = FilterRegBooks<VRegBookCertificateDuplicate>(certificateDuplicatesViewQuery, input);

                    IQueryable<RegBookDetailsModel> certificationDuplicatesList = (
                        from x in certificateDuplicatesViewQuery
                        join basicDocument in _context.BasicDocuments on x.BasicDocumentId equals basicDocument.Id
                        join idType in _context.PersonalIdtypes on x.PersonalIdtype equals idType.PersonalIdtypeId
                        join schoolYear in _context.CurrentYears on x.SchoolYear equals schoolYear.CurrentYearId
                        where x.BasicDocumentId == input.BasicDocumentId
                            && (input.Filter == null || (basicDocument.Name.Contains(input.Filter) || x.InstitutionId.ToString().Contains(input.Filter) ||
                                schoolYear.Name.Contains(input.Filter) || x.FullName.Contains(input.Filter) ||
                                x.PersonalId.Contains(input.Filter))
                            )
                        select new RegBookDetailsModel
                        {
                            Id = x.Id,
                            BasicDocumentAbbr = basicDocument.Abbreviation,
                            BasicDocumentName = basicDocument.Name,
                            RegistrationNumberTotal = x.RegistrationNumberTotal,
                            RegistrationNumberYear = x.RegistrationNumberYear,
                            RegistrationDate = x.RegistrationDate,
                            PersonFullName = x.FullName,
                            PersonalId = x.PersonalId,
                            PersonId = x.PersonId,
                            Canceled = x.IsCancelled == true,
                            IsAnulledStatus = x.IsCancelled == false ? "Не" : "Да",
                            SchoolYear = x.SchoolYear,
                            SchoolYearName = schoolYear.Name,
                            PersonalIdTypeName = idType.Name,
                            Spacer = " ",
                            OriginalRegistrationNumberYear = x.OrigRegistrationNumberYear,
                            OriginalRegistrationDate = x.OrigRegistrationDate,
                            OriginalRegistrationNumber = x.OrigRegistrationNumber,
                        })
                        .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

                    return certificationDuplicatesList;
                default:
                    // Генерира празен списък, който имплементира IAsyncEnumerable
                    return _context.VRegBookQualifications.Where(i => false).Select(i => new RegBookDetailsModel());
            }
        }

        public async Task<SignedDiploma> ConstructDiplomaByIdAsync(int id)
        {
            var diploma = await (
                from d in _context.Diplomas.AsNoTracking()
                where d.Id == id
                select new
                {
                    DocumentModels = d.DiplomaDocuments.Select(i => i.ToDocumentModel(_blobServiceConfig)),
                    Diploma = new SignedDiploma()
                    {
                        Version = 1,
                        Contents = d.Contents,
                        FirstName = d.FirstName,
                        MiddleName = d.MiddleName,
                        LastName = d.LastName,
                        PersonalId = d.PersonalId,
                        PersonalIdType = d.PersonalIdtype,
                        Series = d.Series,
                        FactoryNumber = d.FactoryNumber,
                        RegNumberTotal = d.RegistrationNumberTotal,
                        RegNumberYear = d.RegistrationNumberYear,
                        RegDate = d.RegistrationDate,
                        SchoolYear = d.SchoolYear
                    }
                }).FirstOrDefaultAsync();

            if (diploma != null)
            {
                using (var client = new HttpClient())
                using (var sha512 = new SHA512Managed())
                {
                    foreach (var document in diploma.DocumentModels)
                    {
                        var location = $"{document.BlobServiceUrl}/{document.BlobId}?t={document.UnixTimeSeconds}&h={document.Hmac}";
                        var response = await client.GetAsync(location);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var contents = await response.Content.ReadAsByteArrayAsync();
                            diploma.Diploma.ImageHashes.Add(Convert.ToBase64String(sha512.ComputeHash(contents)));
                        }
                    }
                }
                diploma.Diploma.ImageHashes.Sort();
                return diploma.Diploma;
            }
            else
            {
                return await Task.FromResult<SignedDiploma>(null);
            }
        }

        public async Task<string> ConstructDiplomaByIdAsXmlAsync(int id)
        {
            SignedDiploma signedDiploma = await ConstructDiplomaByIdAsync(id);
            XmlSerializer serializer = new XmlSerializer(signedDiploma.GetType());
            StringBuilder sb = new StringBuilder();
            using var xmlWriter = XmlWriter.Create(sb);
            serializer.Serialize(xmlWriter, signedDiploma);
            return sb.ToString();
        }

        public async Task<ApiValidationResult> Create(DiplomaCreateModel model)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null)
            {
                validationResult.Errors.Add(Messages.EmptyModelError);
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            model.BasicDocument = model.TemplateId.HasValue
                ? await _context.Templates
                    .Where(x => x.Id == model.TemplateId.Value)
                    .Select(x => new BasicDocumentModel
                    {
                        Id = x.BasicDocumentId,
                        Name = x.BasicDocument.Name,
                        IsValidation = x.BasicDocument.IsValidation,
                        IsRuoDoc = x.BasicDocument.IsRuoDoc
                    })
                    .SingleOrDefaultAsync()
                : await _context.BasicDocuments
                    .Where(x => x.Id == model.BasicDocumentId)
                    .Select(x => new BasicDocumentModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsValidation = x.IsValidation,
                        IsRuoDoc = x.IsRuoDoc
                    })
                    .SingleOrDefaultAsync();


            bool hasManagePermission = await HasDiplomaManagePermission(model?.PersonId);
            if (!hasManagePermission)
            {
                validationResult.Errors.Add(Messages.UnauthorizedMessageError);
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            List<DiplomaImportModel> importModels = new List<DiplomaImportModel>();
            try
            {
                IEnumerable<GradeDropdownViewModel> gradeNomenclature = await _lookupService.GetGradeOptions(null, null);
                DiplomaImportModel importModel = model.ToImportModel(gradeNomenclature);
                importModel.IsManualCreateOrUpdate = true;

                importModels.Add(importModel);
            }
            catch (Exception ex)
            {
                validationResult.Errors.Add(ex.Message, model.BasicDocument?.Name, model.BasicDocument?.Id.ToString());
                if (ex.InnerException != null)
                {
                    validationResult.Errors.Add(ex.GetInnerMostException().Message, model.BasicDocument?.Name, model.BasicDocument?.Id.ToString());
                }
            }

            await ValidateImport(importModels, validationResult, CancellationToken.None);
            // Todo: Не валидираме по схема. Валидирането по схема става в ReadDiplomasArchive()

            validationResult.DiplomaPerformaceStats.Add(new DiplomaPerformaceStats
            {
                Type = "CreateValidation",
                PersonalId = importModels.FirstOrDefault()?.ParseModel?.Person?.PersonalId,
                PerformanceStats = validationResult.DiplomaPerformaceStats.SelectMany(x => x.PerformanceStats).ToList(),
            });
            _logger.LogInformation(new Exception(JsonConvert.SerializeObject(validationResult.DiplomaPerformaceStats)), "Diploma create stats");

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            await CreateOrUpdate(importModels, validationResult, model.Contents, null);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            return validationResult;
        }

        //public async Task<ApiValidationResult> Validate(int diplomaId)
        //{
        //    var model = await GetUpdateModel(diplomaId);

        //    ApiValidationResult validationResult = new ApiValidationResult();

        //    if (model == null)
        //    {
        //        validationResult.Errors.Add(Messages.EmptyModelError);
        //        throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
        //    }

        //    model.BasicDocument = await _context.BasicDocuments
        //        .Where(x => x.Id == model.BasicDocumentId)
        //        .Select(x => new BasicDocumentModel
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            IsValidation = x.IsValidation
        //        })
        //        .SingleOrDefaultAsync();

        //    bool hasManagePermission = await HasDiplomaManagePermission(model?.PersonId);
        //    if (!hasManagePermission)
        //    {
        //        validationResult.Errors.Add(Messages.UnauthorizedMessageError);
        //        throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
        //    }

        //    if (!_userInfo.InstitutionID.HasValue || model.InstitutionId != _userInfo.InstitutionID.Value)
        //    {
        //        if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage))
        //        {
        //            DiplomaViewModel diplomaDetails = await _context.VDiplomaLists
        //                .Where(x => x.Id == model.DiplomaId)
        //                .Select(x => new DiplomaViewModel
        //                {
        //                    CreatedByMonHrRole = x.CreatedByMonHrRole
        //                })
        //                .SingleOrDefaultAsync();

        //            // ЧРАО права. Тряба да проверим, че дипломата е създадена от потребител с ЧРАО (MonHR) роля.
        //            if (!diplomaDetails.CreatedByMonHrRole)
        //            {
        //                validationResult.Errors.Add(Messages.UnauthorizedMessageError);
        //                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
        //            }
        //        }
        //    }

        //    List<DiplomaImportModel> importModels = new List<DiplomaImportModel>();
        //    try
        //    {
        //        IEnumerable<GradeDropdownViewModel> gradeNomenclature = await _lookupService.GetGradeOptions(null, null);
        //        DiplomaImportModel importModel = model.ToImportModel(gradeNomenclature);
        //        importModel.IsManualCreateOrUpdate = false;
        //        importModel.DiplomaId = model.DiplomaId; // Редактираме диплома. model.DiplomaId е необходимо за да знаме, че редактираме.


        //        // Добавяме изображенията към модела
        //        var dbDiplomaImages = await (
        //            from i in _context.DiplomaDocuments
        //            where i.DiplomaId == model.DiplomaId
        //            select new
        //            {
        //                Image = i.ToDocumentModel(_blobServiceConfig),
        //                Position = i.Position,
        //            }).ToListAsync();

        //        List<DiplomaImage> diplomaImages = new List<DiplomaImage>();
        //        using (var client = new HttpClient())
        //        {
        //            foreach (var document in dbDiplomaImages)
        //            {
        //                var location = $"{document.Image.BlobServiceUrl}/{document.Image.BlobId}?t={document.Image.UnixTimeSeconds}&h={document.Image.Hmac}";
        //                var response = await client.GetAsync(location);
        //                if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //                {
        //                    var contents = await response.Content.ReadAsByteArrayAsync();
        //                    var diplomaImage = new DiplomaImage()
        //                    {
        //                        Contents = Convert.ToBase64String(contents),
        //                        Position = document.Position
        //                    };
        //                    diplomaImages.Add(diplomaImage);
        //                }
        //            }
        //        }
        //        importModel.ParseModel.Images = diplomaImages.OrderBy(i => i.Position).ToArray();

        //        importModels.Add(importModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        validationResult.Errors.Add(ex.Message, model.BasicDocument?.Name, model.BasicDocument?.Id.ToString());
        //        if (ex.InnerException != null)
        //        {
        //            validationResult.Errors.Add(ex.GetInnerMostException().Message, model.BasicDocument?.Name, model.BasicDocument?.Id.ToString());
        //        }
        //    }

        //    await ValidateImport(importModels, validationResult);

        //    return validationResult;
        //}

        public async Task<ApiValidationResult> Update(DiplomaUpdateModel model)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null)
            {
                validationResult.Errors.Add(Messages.EmptyModelError);
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            model.BasicDocument = await _context.BasicDocuments
                .Where(x => x.Id == model.BasicDocumentId)
                .Select(x => new BasicDocumentModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsValidation = x.IsValidation,
                    IsRuoDoc = x.IsRuoDoc
                })
                .SingleOrDefaultAsync();

            bool hasManagePermission = await HasDiplomaManagePermission(model?.PersonId);
            if (!hasManagePermission)
            {
                validationResult.Errors.Add(Messages.UnauthorizedMessageError);
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            if (model.InstitutionId != _userInfo.InstitutionID
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage))
            {
                DiplomaViewModel diplomaDetails = await _context.VDiplomaLists
                        .Where(x => x.Id == model.DiplomaId)
                        .Select(x => new DiplomaViewModel
                        {
                            CreatedByMonHrRole = x.CreatedByMonHrRole,
                            IsRuoDocBasicDocument = x.IsRuoDocBasicDocument,
                            RuoRegId = x.RuoRegId
                        })
                        .SingleOrDefaultAsync();

                if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaManage))
                {
                    // ЧРАО права. Тряба да проверим, че дипломата е създадена от потребител с ЧРАО (MonHR) роля.
                    if (!diplomaDetails.CreatedByMonHrRole)
                    {
                        validationResult.Errors.Add(Messages.UnauthorizedMessageError);
                        throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
                    }
                }
                else if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage))
                {
                    // RUO права за документи за приравняване diplomaDetails
                    if (!(diplomaDetails.IsRuoDocBasicDocument && diplomaDetails.RuoRegId.HasValue && diplomaDetails.RuoRegId.Value == _userInfo.RegionID))
                    {
                        validationResult.Errors.Add(Messages.UnauthorizedMessageError);
                        throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
                    }
                }
                else
                {
                    validationResult.Errors.Add(Messages.UnauthorizedMessageError);
                    throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
                }
            }

            List<DiplomaImportModel> importModels = new List<DiplomaImportModel>();
            try
            {
                IEnumerable<GradeDropdownViewModel> gradeNomenclature = await _lookupService.GetGradeOptions(null, null);
                DiplomaImportModel importModel = model.ToImportModel(gradeNomenclature);
                if (importModel.ParseModel?.Subjects != null)
                {
                    // Предмети, които имат Id, но нямат SubjectName - слагаме съответния SubjectName
                    foreach (var subject in importModel.ParseModel.Subjects.Where(i => i.SubjectId.HasValue && String.IsNullOrWhiteSpace(i.SubjectName)))
                    {
                        var dbSubject = _context.Subjects.FirstOrDefault(i => i.SubjectId == subject.SubjectId);
                        subject.SubjectName = dbSubject.SubjectName;
                    }
                }
                importModel.IsManualCreateOrUpdate = true;
                importModel.DiplomaId = model.DiplomaId; // Редактираме диплома. model.DiplomaId е необходимо за да знаме, че редактираме.

                importModels.Add(importModel);
            }
            catch (Exception ex)
            {
                validationResult.Errors.Add(ex.Message, model.BasicDocument?.Name, model.BasicDocument?.Id.ToString());
                if (ex.InnerException != null)
                {
                    validationResult.Errors.Add(ex.GetInnerMostException().Message, model.BasicDocument?.Name, model.BasicDocument?.Id.ToString());
                }
            }

            await ValidateImport(importModels, validationResult, CancellationToken.None);
            // Todo: Не валидираме по схема. Валидирането по схема става в ReadDiplomasArchive()

            validationResult.DiplomaPerformaceStats.Add(new DiplomaPerformaceStats
            {
                Type = "UpdateValidation",
                PersonalId = importModels.FirstOrDefault()?.ParseModel?.Person?.PersonalId,
                PerformanceStats = validationResult.DiplomaPerformaceStats.SelectMany(x => x.PerformanceStats).ToList(),
            });
            _logger.LogInformation(new Exception(JsonConvert.SerializeObject(validationResult.DiplomaPerformaceStats)), "Diploma update stats");

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            await CreateOrUpdate(importModels, validationResult, model.Contents, model.DiplomaId);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            return validationResult;
        }

        public async Task Delete(int id)
        {
            Diploma diploma = await _context.Diplomas
                .SingleOrDefaultAsync(x => x.Id == id);

            await CheckDiplomaDeletePermission(diploma);

            _context.Diplomas.Remove(diploma);
            await SaveAsync();
        }

        public async Task<byte[]> GenerateApplicationFileAsync(int diplomaId)
        {
            CancellationToken stopped = new CancellationToken();

            var applicationFile = await (
                from d in _context.Diplomas
                where d.Id == diplomaId
                select new
                {
                    basicDocument = new
                    {
                        isDuplicate = d.BasicDocument.IsDuplicate
                    },
                    application = new ApplicationFileModel
                    {
                        institutionName = d.InstitutionSchoolYear.Name,
                        institutionAddress = $"гр./с.{d.InstitutionSchoolYear.Town.Name} общ.{d.InstitutionSchoolYear.Town.Municipality.Name} обл.{d.InstitutionSchoolYear.Town.Municipality.Region.Name}",
                        personName = $"{d.FirstName} {d.MiddleName} {d.LastName}",
                        basicDocumentName = d.BasicDocument.Name,
                        regDate = d.RegistrationDate != null ? d.RegistrationDate.Value.ToString("dd.MM.yyyy") : null,
                        regNumber = $"{d.RegistrationNumberTotal}-{d.RegistrationNumberYear}",
                        series = d.Series != null ? d.Series : "-",
                        factoryNumber = d.FactoryNumber != null ? d.FactoryNumber : "-"
                    }
                }).FirstOrDefaultAsync();

            if (applicationFile == null)
            {
                // Не е намерен в дипломи, затова търсим в други документи
                applicationFile = await (
                                from d in _context.OtherDocuments
                                where d.Id == diplomaId
                                select new
                                {
                                    basicDocument = new
                                    {
                                        isDuplicate = d.BasicDocument.IsDuplicate
                                    },
                                    application = new ApplicationFileModel
                                    {
                                        institutionName = d.InstitutionSchoolYear.Name,
                                        institutionAddress = $"гр./с.{d.InstitutionSchoolYear.Town.Name} общ.{d.InstitutionSchoolYear.Town.Municipality.Name} обл.{d.InstitutionSchoolYear.Town.Municipality.Region.Name}",
                                        personName = $"{d.Person.FirstName} {d.Person.MiddleName} {d.Person.LastName}",
                                        basicDocumentName = d.BasicDocument.Name,
                                        regDate = d.IssueDate != null ? d.IssueDate.Value.ToString("dd.MM.yyyy") : null,
                                        regNumber = $"{d.RegNumberTotal}-{d.RegNumber}",
                                        series = d.Series != null ? d.Series : "-",
                                        factoryNumber = d.FactoryNumber != null ? d.FactoryNumber : "-"
                                    }
                                }).FirstOrDefaultAsync();
            }

            using (var ms = new MemoryStream())
            {
                await _wordTemplateService.TransformAsync(
                    applicationFile.basicDocument.isDuplicate ? "applicationDuplicatesFile" : "applicationFile",
                    JsonConvert.SerializeObject(applicationFile.application),
                    ms,
                    stopped); ;

                return ms.ToArray();
            }
        }

        public async Task<IEnumerable<DiplomaDocumentModel>> GetDiplomaDocumentsAsync(int diplomadId)
        {
            (bool hasDiplomaReadPermission, _) = await CheckForReadPermission(null);

            if (!hasDiplomaReadPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<DiplomaDocumentModel> documents = await (
                from i in _context.DiplomaDocuments
                where i.DiplomaId == diplomadId
                orderby i.Position
                select i.ToDocumentModel(_blobServiceConfig))
            .ToListAsync();

            return documents;
        }

        public async Task<ResultModel<int>> UploadDiplomaDocumentAsync(byte[] contents, int diplomaId, string description, string fileName, string contentType, CancellationToken cancellationToken)
        {
            DiplomaViewModel diploma = await _context.VDiplomaLists
                .Where(x => x.Id == diplomaId)
                .Select(x => new DiplomaViewModel
                {
                    PersonId = x.PersonId,
                    PersonalIdType = x.PersonalIdType,
                    PersonalId = x.PersonalId,
                    SchoolYear = x.SchoolYear,
                    BasicDocumentId = x.BasicDocumentId,
                    InstitutionId = x.InstitutionId,
                    IsCancelled = x.IsCancelled,
                    IsSigned = x.IsSigned,
                    IsEditable = x.IsEditable,
                    CreatedByMonHrRole = x.CreatedByMonHrRole,
                    Series = x.Series,
                    FactoryNumber = x.FactoryNumber,
                })
                .SingleOrDefaultAsync();

            await CheckDiplomaImageUploadPermission(diploma);

            var resizedImageBytes = await _imageService.Resize(contents, 2000, 2000);
            var compressedImageBytes = await _imageService.Compress(resizedImageBytes, ImageCompressionLevelEnum.High);

            bool skipImageCheck = false;
            List<DiplomaImportValidationExclusionModel> exclusionList = await _diplomaImportValidationExclusionService.List();
            if (exclusionList.Any(x => x.PersonalIdTypeId == diploma.PersonalIdType && x.PersonalId == diploma.PersonalId
                && x.Series == diploma.Series && x.FactoryNumber == diploma.FactoryNumber
                && (!x.InstitutonId.HasValue || x.InstitutonId == diploma.InstitutionId)))
            {
                // Скипваме проверката поради въведено изключение
                skipImageCheck = true;
            }

            if (!skipImageCheck)
            {
                IDiplomaCode diplomaValidationService = _serviceProvider.GetRequiredService<CodeService>();
                if (diplomaValidationService == null)
                {
                    throw new ApiException("Не е заредена услуга за проверка на баркодовете");
                }

                bool hasBarcode = await _context.BasicDocuments
                    .Where(x => x.Id == diploma.BasicDocumentId)
                    .Select(x => x.HasBarcode)
                    .SingleOrDefaultAsync();
                
                ApiValidationResult imageValidationResult = await diplomaValidationService.ValidateImageDetails(contents, diploma.BasicDocumentId, "", "", null);
                if (imageValidationResult.HasErrors)
                {
                    throw new ApiException($"Възникна грешка при обработката на изображението! {String.Join("\r\n", imageValidationResult.Errors)}", 500, imageValidationResult.Errors);
                }

                ApiValidationResult barcodeValidationResult = await diplomaValidationService.ValidateImageBarcodes(contents, diploma.SchoolYear,
                    diploma.BasicDocumentId, hasBarcode, "", "", null, cancellationToken);

                if (barcodeValidationResult.HasErrors)
                {
                    throw new ApiException("Възникна грешка при разчитане на баркода от изображението!", 500, barcodeValidationResult.Errors);
                }

            }

            //if (!skipBarcodeCheck)
            //{
            //    var barcodeYear = await _barcodeYearService.GetBarcodeYearAsync(diploma.BasicDocumentId, diploma.SchoolYear);
            //    if (barcodeYear != null)
            //    {
            //        try
            //        {
            //            ZXing.Result result = await _barcodeService.DecodeTryHarderAsync(contents);
            //            if (barcodeYear != null)
            //            {
            //                if (result == null || result.Text.IsNullOrWhiteSpace() || !(barcodeYear.HeaderPage.PadLeft(result.Text.Length, '0').Equals(result.Text) || barcodeYear.InternalPage.PadLeft(result.Text.Length, '0').Equals(result.Text)))
            //                {
            //                    ZXing.Result resultIMB = await _barcodeService.DecodeTryHarderAsync(contents, new List<ZXing.BarcodeFormat> { BarcodeFormat.IMB });
            //                    if (resultIMB == null || !(barcodeYear.HeaderPage.PadLeft(resultIMB.Text.Length, '0').Equals(resultIMB.Text) || barcodeYear.InternalPage.PadLeft(resultIMB.Text.Length, '0').Equals(resultIMB.Text)))
            //                    {
            //                        return new ErrorResultModel<int>(0, "Баркодът не е прочетен или не съвпада!");
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                return new ErrorResultModel<int>(0, "Баркод за тази година не съществува!");
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            return new ErrorResultModel<int>(0, "Проблем при разчитане на баркода от изображението!");
            //        }
            //    }
            //}

            var blobDO = await _blobService.UploadFileAsync(compressedImageBytes, fileName, contentType);

            if (blobDO.IsError)
            {
                return new ErrorResultModel<int>(0, blobDO.Message);
            }

            int maxPosition = (await _context.DiplomaDocuments.Where(i => i.DiplomaId == diplomaId).MaxAsync(i => (int?)i.Position)) ?? 0;

            var diplomaDocument = new DataAccess.DiplomaDocument()
            {
                BlobId = blobDO.Data.BlobId,
                Description = description,
                DiplomaId = diplomaId,
                Position = maxPosition + 1
            };

            _context.DiplomaDocuments.Add(diplomaDocument);

            await SaveAsync();
            return new OKResultModel<int>(diplomaDocument.Id);
        }

        public async Task RemoveDiplomaDocumentAsync(int id)
        {
            var diplomaDocument = await _context.DiplomaDocuments.SingleOrDefaultAsync(i => i.Id == id);

            int? diplomaId = diplomaDocument?.DiplomaId;

            DiplomaViewModel diploma = await _context.Diplomas
                .Where(x => x.Id == diplomaId)
                .Select(x => new DiplomaViewModel
                {
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    BasicDocumentId = x.BasicDocumentId,
                    IsValidation = x.BasicDocument.IsValidation,
                    InstitutionId = x.InstitutionId,
                    IsCancelled = x.IsCancelled,
                    IsSigned = x.IsSigned,
                    IsEditable = x.IsEditable
                })
                .SingleOrDefaultAsync();

            await CheckDiplomaImageUploadPermission(diploma);

            _context.DiplomaDocuments.Remove(diplomaDocument);

            await SaveAsync();
        }

        public async Task ReorderDiplomaDocumentsAsync(DiplomaOrderDocuments model)
        {
            var diploma = await _context.Diplomas
                .Where(x => x.Id == model.Id)
                .Select(x => new DiplomaViewModel
                {
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    BasicDocumentId = x.BasicDocumentId,
                    IsValidation = x.BasicDocument.IsValidation,
                    InstitutionId = x.InstitutionId,
                    IsCancelled = x.IsCancelled,
                    IsSigned = x.IsSigned,
                    IsEditable = x.IsEditable
                })
                .SingleOrDefaultAsync();

            await CheckDiplomaImageUploadPermission(diploma);

            var documents = await (
                from i in _context.DiplomaDocuments
                where i.DiplomaId == model.Id
                select i)
            .ToListAsync();

            foreach (var document in documents)
            {
                document.Position = model.DocumentPositions.FirstOrDefault(i => i.Id == document.Id).Position;
            }

            await SaveAsync();
        }

        public async Task<DiplomaSigningData> GetDiplomaSigningDataAsync(int id)
        {
            DiplomaSigningData signingData = await (
                from d in _context.Diplomas
                where d.Id == id
                select new DiplomaSigningData()
                {
                    Signature = d.Signature,
                    SignedDate = d.SignedDate,
                    IsSigned = d.IsSigned
                }).FirstOrDefaultAsync();

            if (signingData != null)
            {
                signingData.IsValid = CertificateExtensions.VerifyXmlString(signingData.Signature);
                X509Certificate2 cert = CertificateExtensions.ExtractCertificate(signingData.Signature);
                signingData.Certificate = JsonConvert.SerializeObject(
                        cert, new JsonSerializerSettings
                        {
                            Formatting = Newtonsoft.Json.Formatting.Indented,
                            // Ignore serialization errors
                            Error = (s, a) => a.ErrorContext.Handled = true,
                            ContractResolver = new DefaultContractResolver
                            {
                                // Ensures all properties are serialized
                                IgnoreSerializableInterface = true
                            }
                        }
                    );
            }
            return signingData;
        }

        public async Task UpdateDiplomaFinalizationStepsAsync(DiplomaFinalizationUpdateModel model)
        {
            var origDiploma = await _context.VDiplomaLists
               .Where(x => x.Id == model.DiplomaId)
               .Select(x => new DiplomaViewModel
               {
                   Id = x.Id,
                   PersonId = x.PersonId,
                   BasicDocumentId = x.BasicDocumentId,
                   IsPublic = x.IsPublic,
                   InstitutionId = x.InstitutionId,
                   CreatedByMonHrRole = x.CreatedByMonHrRole
               })
               .SingleOrDefaultAsync();

            await CheckDiplomaFinalizationPermission(origDiploma);

            Diploma diploma = await _context.Diplomas
                .Include(d => d.DiplomaAdditionalDocumentDiplomas)
                .Include(d => d.BasicDocument)
                .SingleOrDefaultAsync(d => d.Id == model.DiplomaId);
            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            await ValidateSign(diploma);

            switch (model.ConfirmedStepNumber)
            {
                case (int)DiplomaFinaliziationStepEnum.IsSignedDiplomaStepNumber:
                    diploma.IsFinalized = true;
                    diploma.FinalizedDate = diploma.FinalizedDate == null ? DateTime.Now : diploma.FinalizedDate;
                    diploma.IsSigned = true;
                    diploma.SignedDate = diploma.SignedDate == null ? DateTime.Now : diploma.SignedDate;
                    diploma.SignedBySysUserId = _userInfo?.SysUserID;
                    diploma.IsEditable = false;

                    string signature = model.Signature;

                    try
                    {
                        byte[] data = Convert.FromBase64String(model.Signature);
                        signature = Encoding.UTF8.GetString(data);
                    }
                    catch
                    {
                        // Не е base64 Или не сме успели да декодираме. Записваме каквото е дошло
                        signature = model.Signature;
                    }

                    diploma.Signature = signature;
                    diploma.IsPublic = true;

                    try
                    {
                        var certificateResult = await _certificateService.VerifyXml(diploma.Signature);
                        if (!certificateResult.IsValid)
                        {
                            _logger.LogWarning($"Грешка при подписване: {JsonConvert.SerializeObject(certificateResult)}");
                            throw new ApiException($"Грешка при подписване. Проверете сертификата! {certificateResult.Errors}", 500, new ValidationErrorCollection()
                            {
                                new ValidationError(certificateResult.Errors)
                            });
                        }
                        else
                        {
                            var certificateValidationResult = await _certificateService.VerifyXmlWithInstitution(diploma.Signature, _userInfo.InstitutionID);
                            if (!certificateValidationResult.IsValid)
                            {
                                _logger.LogWarning($"Грешка при проверка на сертификата с данните на институцията: {JsonConvert.SerializeObject(certificateValidationResult)}");
                                throw new ApiException($"Грешка при проверка на сертификата с данните на институцията {certificateValidationResult.Errors}", 500, new ValidationErrorCollection()
                                {
                                    new ValidationError(certificateValidationResult.Errors)
                                });
                            }
                        }
                    }
                    catch (ApiException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Грешка при подписване");
                        throw new ApiException("Грешка при подписване", ex);
                    }

                    break;
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (diploma.IsPublic && diploma.IsPublic != origDiploma.IsPublic)
                {
                    var diplomaService = await GetDiplomaCodeAsync(origDiploma.BasicDocumentId);
                    if (diplomaService != null)
                    {
                        await diplomaService.AfterFinalization(diploma.Id);
                    }
                }

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DiplomaFinalizationViewModel> GetDiplomaFinalizationDetailsByIdAsync(int diplomaId)
        {
            var diploma = await _context.VDiplomaLists
                .Where(x => x.Id == diplomaId)
                .Select(x => new DiplomaViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    IsFinalized = x.IsFinalized,
                    IsSigned = x.IsSigned,
                    IsPublic = x.IsPublic,
                    SignedDate = x.SignedDate,
                    InstitutionId = x.InstitutionId
                })
                .SingleOrDefaultAsync();

            await CheckDiplomaFinalizationPermission(diploma);

            int currentStepNumber = (int)DiplomaFinaliziationStepEnum.ResetStepsNumber;

            if (diploma.IsFinalized)
            {
                currentStepNumber = (int)DiplomaFinaliziationStepEnum.IsFinalizedDiplomaStepNumber;
            }
            if (diploma.IsSigned)
            {
                currentStepNumber = (int)DiplomaFinaliziationStepEnum.IsSignedDiplomaStepNumber;
            }
            if (diploma.IsPublic)
            {
                currentStepNumber = (int)DiplomaFinaliziationStepEnum.IsPublicDiplomaStepNumber;
            }

            return new DiplomaFinalizationViewModel
            {
                CurrentStepNumber = currentStepNumber,
                SignedDate = diploma.SignedDate,
                FinalizedDate = diploma.FinalizedDate,
                IsPublic = diploma.IsPublic,
                IsSigned = diploma.IsSigned,
                IsFinalized = diploma.IsFinalized
            };
        }

        public async Task AnnulDiploma(DiplomaAnullationModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            Diploma diploma = await _context.Diplomas
                .SingleOrDefaultAsync(x => x.Id == model.DiplomaId);

            await CheckDiplomaAnnulmentPermission(diploma);

            if (model.CancellationReason.IsNullOrWhiteSpace())
            {
                throw new ApiException("Необходимо е посочването на причина.");
            }

            diploma.IsCancelled = true;
            diploma.CancelledBySysUserId = _userInfo.SysUserID;
            diploma.CancellationReason = model.CancellationReason;
            diploma.CancellationDate = DateTime.UtcNow;
            diploma.IsPublic = false;

            await SaveAsync();
        }

        public async Task SetAsEditable(DiplomaSetAsEditableModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            Diploma diploma = await _context.Diplomas
                .SingleOrDefaultAsync(x => x.Id == model.DiplomaId);

            await CheckDiplomaSetAsEditablePermission(diploma);

            if (model.Reason.IsNullOrWhiteSpace())
            {
                throw new ApiException("Необходимо е посочването на причина.");
            }

            diploma.IsEditable = true;
            diploma.EditableSetBySysUserId = _userInfo.SysUserID;
            diploma.EditableSetDate = DateTime.UtcNow;
            diploma.EditableSetReason = model.Reason;
            diploma.ResetSigningAttrs();

            await SaveAsync();
        }

        public async Task Import(IFormFile file, CancellationToken cancellationToken)
        {
            (bool hasDiplomaManagePermission, _) = await CheckForManagePermission(null);

            if (!hasDiplomaManagePermission)
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (file == null || file.Length <= 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // Прочита архива, парсва файловете в модели и валидира по схема.
            List<DiplomaImportModel> models = new List<DiplomaImportModel>();
            ApiValidationResult deserializationResult = ReadDiplomasArchive(file, models);

            if (deserializationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, deserializationResult.Errors);
            }

            ApiValidationResult validationResult = new ApiValidationResult();
            await ValidateImport(models, validationResult, cancellationToken);
            if (validationResult.HasErrors)
            {
                throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
            }

            await CreateOrUpdate(models, validationResult);

            if (validationResult.HasErrors)
            {
                throw new ApiException("Съществуват валидационни грешки", 500, validationResult.Errors);
            }
        }

        public async Task<DiplomaCreateModel> GetCreateModel(int? personId, int? templateId, int? basicDocumentId, int? basicClassId, CancellationToken cancellationToken)
        {
            if (!templateId.HasValue && !basicDocumentId.HasValue)
            {
                throw new ApiException(Messages.ArgumentError);
            }

            DiplomaCreateModel model = new DiplomaCreateModel { PersonId = personId };

            if (templateId.HasValue)
            {
                model.BasicDocumentTemplate = await _diplomaTemplateService.GetById(templateId.Value, cancellationToken);
                model.BasicDocumentTemplate.Schema = await _context.Templates
                    .Where(x => x.Id == templateId)
                    .Select(x => x.BasicDocument.Contents)
                    .SingleOrDefaultAsync();
            }
            else if (basicDocumentId.HasValue)
            {
                model.BasicDocumentTemplate = await _diplomaTemplateService.GetForBasicDocument(basicDocumentId.Value, cancellationToken);
                model.BasicDocumentTemplate.Schema = await _context.BasicDocuments
                    .Where(x => x.Id == basicDocumentId.Value)
                    .Select(x => x.Contents)
                    .SingleOrDefaultAsync();
            }

            if (model.BasicDocumentTemplate == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            model.Contents = model.BasicDocumentTemplate.DynamicContent;

            bool hasManagePermission = await HasDiplomaManagePermission(personId);
            if (!hasManagePermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            // Опитваме се да вземем клас описан в колона CodeClassName на таблицата BasicDocument
            IDiplomaCode diplomaCodeService = await GetValidationService(model.BasicDocumentTemplate.BasicDocumentId);
            if (diplomaCodeService == null)
            {
                // Ако нищо не е описано взимаме базовия клас CodeService
                diplomaCodeService = _serviceProvider.GetRequiredService<CodeService>();
            }
            ;

            if (basicClassId.HasValue)
            {
                diplomaCodeService.SetBasicClassIds(new List<int> { basicClassId.Value });
            }
            else if (model.BasicDocumentTemplate.BasicClassId.HasValue)
            {
                diplomaCodeService.SetBasicClassIds(new List<int> { model.BasicDocumentTemplate.BasicClassId.Value });
            }

            model.Contents = await diplomaCodeService.AutoFillDynamicContent(model.BasicDocumentTemplate.BasicDocumentId, personId, model.Contents);

            // При създаване на диплома не трябва да е видима опцията за избор на випуск в компонента за подрубрика 
            foreach (var part in model.BasicDocumentTemplate.Parts)
            {
                foreach (var subject in part.Subjects)
                {
                    subject.IsProfSubjectHeader = false;
                    if (!subject.Modules.IsNullOrEmpty())
                    {
                        foreach (var module in subject.Modules.Where(x => x.IsProfSubjectHeader))
                        {
                            module.IsProfSubjectHeader = false;
                        }
                    }
                }
            }

            // Попълване на свързани документи
            await diplomaCodeService.FillAdditionalDocuments(model);

            // При зареждане на модел за създаване на диплома разделите и предметите се зареждат от шаблона, ако го има,
            // или от базовия документ в BasicDocumentTemplate.Parts.
            // В UI-а при зареждане model.BasicDocumentTemplate.Parts се копират в model.Parts.
            // Именно model.Parts се използва за създаването на модел за импортиране, от който се създава диплома.
            // Оценките от ЛОД-a следва да ги заредим в model.BasicDocumentTemplate.Parts.
            await diplomaCodeService.FillGrades(model);

            await diplomaCodeService.AfterFillGrades(model);

            diplomaCodeService.CalcProfSubjectsHorarium(model);

            return model;
        }

        public async Task<DiplomaUpdateModel> GetUpdateModel(int diplomaId)
        {
            var diploma = await _context.Diplomas
                .Where(x => x.Id == diplomaId)
                .Select(x => new
                {
                    x.Id,
                    x.InstitutionId,
                    x.TemplateId,
                    x.BasicDocumentId,
                    x.PersonId,
                    x.IsCancelled,
                    x.IsFinalized,
                    x.IsSigned,
                    x.IsEditable,
                    x.BasicDocument.IsValidation,
                    x.Contents,
                    x.CommissionOrderNumber,
                    x.CommissionOrderData,
                    x.YearGraduated,
                    AdditionalDocument = x.DiplomaAdditionalDocumentDiplomas.Select(d => new
                    {
                        d.Id,
                        d.MainDiplomaId,
                        d.BasicDocumentId,
                        d.InstitutionId,
                        d.InstitutionName,
                        d.InstitutionAddress,
                        d.Town,
                        d.Municipality,
                        d.Region,
                        d.LocalArea,
                        d.Series,
                        d.FactoryNumber,
                        d.RegistratioNumber,
                        d.RegistrationNumberYear,
                        d.RegistrationDate
                    })
                })
                .SingleOrDefaultAsync();

            if (diploma == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            bool hasDiplomaReadPermission = await HasDiplomaReadPermission(diploma.PersonId);
            if (!hasDiplomaReadPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            // Логнатия потребител няма InstitutionID или тя се различава от тази на дипломата.
            // Проверяваме за някакви администраторски права.
            if (!_userInfo.InstitutionID.HasValue || _userInfo.InstitutionID.Value != diploma.InstitutionId)
            {
                string[] requiredPermissions = new string[]
                {
                    DefaultPermissions.PermissionNameForAdminDiplomaRead,
                    DefaultPermissions.PermissionNameForAdminDiplomaManage,
                    DefaultPermissions.PermissionNameForMonHrDiplomaRead,
                    DefaultPermissions.PermissionNameForMonHrDiplomaManage,
                };

                if (!await _authorizationService.AuthorizeUser(requiredPermissions, true))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            DiplomaUpdateModel model = new DiplomaUpdateModel
            {
                DiplomaId = diploma.Id,
                PersonId = diploma.PersonId,
                TemplateId = diploma.TemplateId,
                BasicDocumentId = diploma.BasicDocumentId,
                Contents = diploma.Contents,
                InstitutionId = diploma.InstitutionId,
                CommissionOrderNumber = diploma.CommissionOrderNumber,
                CommissionOrderData = diploma.CommissionOrderData,
                AdditionalDocuments = diploma.AdditionalDocument
                    .OrderBy(x => x.Id)
                    .Select(x => new DiplomaAdditionalDocumentModel
                    {
                        Id = x.Id,
                        MainDiplomaId = x.MainDiplomaId,
                        BasicDocumentId = x.BasicDocumentId,
                        InstitutionId = x.InstitutionId,
                        InstitutionName = x.InstitutionName,
                        InstitutionAddress = x.InstitutionAddress,
                        Town = x.Town,
                        Municipality = x.Municipality,
                        Region = x.Region,
                        LocalArea = x.LocalArea,
                        Series = x.Series,
                        FactoryNumber = x.FactoryNumber,
                        RegistrationNumber = x.RegistratioNumber,
                        RegistrationNumberYear = x.RegistrationNumberYear,
                        RegistrationDate = x.RegistrationDate,
                    })
                    .ToList(),
                BasicDocumentTemplate = await _diplomaTemplateService.GetForDiploma(diploma.Id)
            };
            model.BasicDocumentTemplate.Schema = await _context.BasicDocuments
                .Where(x => x.Id == diploma.BasicDocumentId)
                .Select(x => x.Contents)
                .SingleOrDefaultAsync();


            if (model.BasicDocumentTemplate == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            // Опитваме се да вземем клас описан в колона CodeClassName на таблицата BasicDocument
            IDiplomaCode diplomaCodeService = await GetValidationService(model.BasicDocumentTemplate.BasicDocumentId);
            if (diplomaCodeService == null)
            {
                // Ако нищо не е описано взимаме базовия клас CodeService
                diplomaCodeService = _serviceProvider.GetRequiredService<CodeService>();
            }
            ;

            if (string.IsNullOrWhiteSpace(model.Contents))
            {
                model.Contents = await diplomaCodeService.AutoFillDynamicContent(model.BasicDocumentTemplate.BasicDocumentId, diploma.PersonId, model.Contents);
            }

            if (GlobalConstants.BasicDocumentsWithDippk.Contains(diploma.BasicDocumentId))
            {
                model.Contents = await diplomaCodeService.FillDippkDynamicContent(model.BasicDocumentTemplate.BasicDocumentId, diploma.PersonId, model.Contents);
            }

            model.Contents = await diplomaCodeService.FixDynamicContent(diploma.Id, model.Contents);

            var basicDocumentPartsExternalEvaluations = model.BasicDocumentTemplate.Parts.Where(i => i.HasExternalEvaluationLimit);
            foreach (var basicDocumentPart in basicDocumentPartsExternalEvaluations)
            {
                if (basicDocumentPart.Subjects.IsNullOrEmpty())
                {
                    var externalEvaluations = await
                        (from i in _context.ExternalEvaluationItems.AsNoTracking()
                         where i.ExternalEvaluation.PersonId == model.PersonId &&
                            basicDocumentPart.ExternalEvaluationTypes.Contains(i.ExternalEvaluation.ExternalEvaluationTypeId)
                         select new
                         {
                             ExternalEvaluationTypeName = i.ExternalEvaluation.ExternalEvaluationType.Name,
                             i.SubjectNavigation.SubjectId,
                             i.SubjectNavigation.SubjectName,
                             i.OriginalPoints,
                             i.Points,
                             i.Grade
                         }).ToListAsync();

                    if (externalEvaluations.Any())
                    {
                        DiplomaMessage diplomaMessage = new DiplomaMessage();
                        diplomaMessage.Message += "В документа автоматично бяха попълнени следните резултати от НВО/ДЗИ, за които в НЕИСПУО е постъпила информация от ЕИСИП:\r\n";
                        foreach (var externalEvaluation in externalEvaluations)
                        {
                            diplomaMessage.Message += $"{externalEvaluation.ExternalEvaluationTypeName} {externalEvaluation.SubjectName}: {externalEvaluation.Points}т., оценка: {externalEvaluation.Grade}\r\n";
                        }
                        model.Messages.Add(diplomaMessage);
                    }
                }
            }
            diplomaCodeService.FillExternalEvalGrades(model);


            return model;
        }

        public async Task<IEnumerable<DiplomaAdditionalDocumentViewModel>> GetOriginalDocuments(int? personId, string? personalId, int? personalIdType, int[] mainBasicDocuments, CancellationToken cancellationToken)
        {
            IQueryable<Diploma> query = _context.Diplomas
                .Where(x => !x.IsCancelled && x.IsSigned
                    && mainBasicDocuments.Contains(x.BasicDocumentId));

            if (personId.HasValue)
            {
                query = query.Where(x => x.PersonId == personId.Value);
            }
            else if (!personalId.IsNullOrWhiteSpace() && personalIdType.HasValue)
            {
                query = query.Where(x => x.PersonalId == personalId && x.PersonalIdtype == personalIdType.Value);
            }
            else
            {
                return Array.Empty<DiplomaAdditionalDocumentViewModel>();
            }

            return await query.Select(x => new DiplomaAdditionalDocumentViewModel
            {
                MainDiplomaId = x.Id,
                BasicDocumentId = x.BasicDocumentId,
                BasicDocumentName = x.BasicDocument.Name,
                InstitutionId = x.InstitutionId,
                InstitutionDetails = $"{x.InstitutionId}.{x.InstitutionName} гр./с.{x.InstitutionSchoolYear.Town.Name} общ.{x.InstitutionSchoolYear.Town.Municipality.Name} обл.{x.InstitutionSchoolYear.Town.Municipality.Region.Name}",
                Series = x.Series,
                FactoryNumber = x.FactoryNumber,
                RegistrationNumber = x.RegistrationNumberTotal,
                RegistrationNumberYear = x.RegistrationNumberYear,
                RegistrationDate = x.RegistrationDate,
            })
            .ToListAsync(cancellationToken);
        }

        public async Task<DiplomaSectionsSubjectsModel> GetDiplomaSubjectsById(int diplomaId)
        {
            var diploma = await _context.Diplomas
                .Where(x => x.Id == diplomaId)
                .Select(x => new
                {
                    x.PersonId,
                    x.BasicDocument.IsValidation
                })
                .SingleOrDefaultAsync();

            (bool hasDiplomaReadPermission, _) = await CheckForReadPermission(diploma?.PersonId);
            if (!hasDiplomaReadPermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            // Веведените оценки за дадена диплома
            DiplomaSectionsSubjectsModel diplomaSubjects = await _context.Diplomas
                .AsNoTracking()
                .Where(x => x.Id == diplomaId)
                .Select(x => new DiplomaSectionsSubjectsModel
                {
                    DiplomaId = x.Id,
                    BasicDocumentId = x.BasicDocumentId,
                    Sections = x.BasicDocument.BasicDocumentParts.OrderBy(x => x.Position)
                        .Select(p => new DiplomaSectionModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            IsHorariumHidden = p.IsHorariumHidden,
                            BasicSubjectType = p.BasicSubjectType.Name,
                            SubjectTypes = (p.SubjectTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                            GradeTypes = (p.ExternalEvaluationTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                            TemplatePartsWithSubjects = JsonConvert.DeserializeObject<List<DiplomaTemplateSubjectPartModel>>(x.Template.SubjectContents),
                            Subjects = p.DiplomaSubjects.Where(x => x.DiplomaId == diplomaId).OrderBy(x => x.Position).Select(s => new DiplomaSubjectModel
                            {
                                Id = s.Id,
                                BasicDocumentPartId = p.Id,
                                BasicDocumentSubjectId = s.BasicDocumentSubjectId,
                                DiplomaId = s.DiplomaId,
                                Grade = s.Grade,
                                GradeText = s.GradeText,
                                Horarium = s.Horarium,
                                Position = s.Position,
                                SubjectCanChange = true,
                                IsHorariumHidden = p.IsHorariumHidden,
                                ParentId = s.ParentId,
                                SubjectDropDown = new DropdownViewModel
                                {
                                    Value = s.Subject.SubjectId,
                                    Text = s.Subject.SubjectName,
                                    Name = s.Subject.SubjectName,
                                },
                            }).ToList()
                        }).ToList()
                })
                .SingleOrDefaultAsync();

            foreach (var section in diplomaSubjects.Sections)
            {
                foreach (var templateSection in section.TemplatePartsWithSubjects)
                {
                    if (section.Id == templateSection.BasicDocumentPartId && section.Name == templateSection.BasicDocumentPartName)
                    {
                        foreach (var templateSubject in templateSection.Subjects)
                        {
                            if (!section.Subjects.Any(i => i.Position == templateSubject.Position))
                            {
                                section.Subjects.Add(new DiplomaSubjectModel
                                {
                                    BasicDocumentPartId = templateSection.BasicDocumentPartId,
                                    DiplomaId = diplomaId,
                                    Grade = null,
                                    IsHorariumHidden = templateSection.IsHorariumHidden,
                                    Position = templateSubject.Position,
                                    SubjectDropDown = templateSubject.SubjectDropdown,
                                    SubjectCanChange = templateSubject.SubjectCanChange,
                                });
                            }
                        }
                    }
                }
            }

            if (diplomaSubjects == null)
            {
                throw new ArgumentNullException(nameof(diplomaSubjects), nameof(DiplomaSectionsSubjectsModel));
            }

            // Предефинираните оценки за документ от типа на дадена диплома
            DiplomaSectionsSubjectsModel predefinedSubjects = await _context.BasicDocuments
                .AsNoTracking()
                .Where(x => x.Id == diplomaSubjects.BasicDocumentId)
                .Select(x => new DiplomaSectionsSubjectsModel
                {
                    DiplomaId = diplomaId,
                    BasicDocumentId = x.Id,
                    Sections = x.BasicDocumentParts.OrderBy(x => x.Position)
                        .Select(p => new DiplomaSectionModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            IsHorariumHidden = p.IsHorariumHidden,
                            Subjects = p.BasicDocumentSubjects.OrderBy(x => x.Position).Select(s => new DiplomaSubjectModel
                            {
                                BasicDocumentPartId = p.Id,
                                BasicDocumentSubjectId = s.Id,
                                Position = s.Position,
                                SubjectCanChange = s.SubjectCanChange,
                                IsHorariumHidden = p.IsHorariumHidden,
                                SubjectDropDown = new DropdownViewModel
                                {
                                    Value = s.Subject.SubjectId,
                                    Text = s.Subject.SubjectName,
                                    Name = s.Subject.SubjectName
                                },
                            }).ToList()
                        }).ToList()
                })
                .SingleOrDefaultAsync();

            return MergeSubjects(predefinedSubjects, diplomaSubjects);
        }

        public async Task<DiplomaViewModel> GetAdditionalDocumentDetails(int personId, int basicDocumentId)
        {
            bool hasManagePermission = await HasDiplomaManagePermission(personId);
            if (!hasManagePermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<DiplomaViewModel> models = await _context.Diplomas
                .Where(x => x.PersonId == personId && x.BasicDocumentId == basicDocumentId && !x.IsCancelled)
                .Select(x => new DiplomaViewModel
                {
                    Series = x.Series,
                    FactoryNumber = x.FactoryNumber,
                    RegistrationNumberTotal = x.RegistrationNumberTotal,
                    RegistrationNumberYear = x.RegistrationNumberYear,
                    RegistrationDate = x.RegistrationDate,
                    BasicDocumentType = x.BasicDocumentName
                })
                .ToListAsync();

            if (models.Count > 1)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException($"Намерени са повече от един документи от вид \"${models.First().BasicDocumentType}\""));
            }

            return models.FirstOrDefault();
        }
    }
}
