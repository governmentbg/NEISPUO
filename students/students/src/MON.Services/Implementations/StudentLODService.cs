namespace MON.Services.Implementations
{
    using Domain;
    using Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models.Configuration;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Lod;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.Enums.AspIntegration;
    using MON.Shared.Enums.SchoolBooks;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Extensions;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class StudentLODService : BaseService<StudentLODService>, IStudentLODService
    {
        private readonly IWordTemplateService _wordTemplateService;
        private readonly IStudentService _studentService;
        private readonly ILodAssessmentService _lodAssessmentService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IInstitutionService _institutionService;
        private readonly IDischargeDocumentService _dischargeDocumentService;

        public StudentLODService(DbServiceDependencies<StudentLODService> dependencies,
            IWordTemplateService wordTemplateService,
            IStudentService studentService,
            ILodAssessmentService lodAssessmentService,
            IInstitutionService institutionService,
            IDischargeDocumentService dischargeDocumentService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _wordTemplateService = wordTemplateService;
            _studentService = studentService;
            _lodAssessmentService = lodAssessmentService;
            _institutionService = institutionService;
            _dischargeDocumentService = dischargeDocumentService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<IPagedList<VStudentGeneralTrainingDatum>> GetGeneralTrainingDataList(StudentGeneralTrainingDataListInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);

            if (!await _authorizationService.HasPermissionForStudent(input.StudentId ?? default, DefaultPermissions.PermissionNameForStudentGeneralTrainingDataRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            IQueryable<VStudentGeneralTrainingDatum> query = _context.VStudentGeneralTrainingData
                .AsNoTracking()
                .Where(x => x.PersonId == input.StudentId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.PersonFullName.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.Institution.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<VStudentGeneralTrainingDatum> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<StudentGeneralTrainingDataDetails> GetGeneralTrainingDataDetails(int studentId, int classId)
        {
            VStudentGeneralTrainingDatum entity = await _context.VStudentGeneralTrainingData
                .Where(x => x.Id == classId && x.PersonId == studentId)
                .AsNoTracking()
                .FirstOrDefaultAsync();


            if (entity == null) throw new ArgumentNullException(nameof(classId));

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentGeneralTrainingDataRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            StudentGeneralTrainingDataDetails model = new StudentGeneralTrainingDataDetails
            {
                StudentId = entity.PersonId,
                ClassId = entity.Id,
                Institution = entity.Institution,
                PositionId = entity.PositionId,
                Position = entity.Position,
                AdmissionDocumentId = entity.AdmissionDocumentId,
                AdmissionRelocationDocumentId = entity.AdmissionRelocationDocumentId,
                SchoolYear = entity.SchoolYear
            };

            // Да се потърси има ли издаден RelocationDocument (с дата след записването) 
            // за преместване от тази институция, който е използван за записване в друга. 
            // Ако има, показва се следното:
            model.RelocationDocuments = await _context.RelocationDocuments
                .AsNoTracking()
                .Where(x => (entity.AdmissionDate == null || x.NoteDate > entity.AdmissionDate)
                    && x.CurrentStudentClass.Class.InstitutionId == entity.InstitutionId
                    && x.PersonId == entity.PersonId
                    && _context.AdmissionDocuments.Any(ad => ad.RelocationDocumentId != null
                        && ad.RelocationDocumentId == x.Id
                        && ad.PersonId == entity.PersonId
                        && ad.InstitutionId != entity.InstitutionId))
                .Select(x => new StudentMovementDocumentBasicDetails
                {
                    Id = x.Id,
                    NoteDate = x.NoteDate,
                    NoteNumber = x.NoteNumber,
                    SendingInstitution = x.CurrentStudentClass.Class.InstitutionSchoolYear.Name,
                    HostInstitution = x.HostInstitution.Name,
                    DocumentType = "RelocationDocument",
                    RuoOrderDate = x.RuoorderDate,
                    RuoOrderNumber = x.RuoorderNumber
                }).ToListAsync();


            // Документи за отписване свързани с дадения StudentClass (DischargeDocument.CurrentStudentClassId)
            model.DischargeDocumentsIds = await _context.DischargeDocuments
                .AsNoTracking()
                .Where(x => x.CurrentStudentClassId == entity.Id)
                .Select(x => x.Id)
                .ToListAsync();

            // Където RelocationDocumentId е намереният в търсенето на предишната стъпка. 
            // Да се има предвид, че може да се намери повече от едно удостоверение за преместване, 
            // ако по-късно детето пак се е върнало в същото училище и майка му пак е решила да го мести. 
            // Ако се случи това извращение, може би е правилно да се вземе удостоверението с най-малка дата.

            return model;
        }

        public Task<List<VCurriculumStudentDetail>> GetCurriculumDetailsByStudentClass(int studentClassId)
        {
            return _context.VCurriculumStudentDetails
                .AsNoTracking()
                .Where(x => x.StudentClassId == studentClassId
                    && x.IsValid) // Todo: все още не се знае как ще разграничаваме IsValid = false когато записът е променен чрез преместване в нова паралелка или е "изтрит", примерно през модул Институции.
                                  // Не се позват записите за нетекущите групи, туй като те са с IsValid = false.
                .OrderByDescending(x => x.IsValid)
                .ThenBy(x => x.SubjectTypeName)
                .ThenBy(x => x.SubjectName)
                .ToListAsync();
        }

        public async Task<IPagedList<LodFinalizationViewModel>> ListFinalizations(LodFinalizationListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(NotesListInput)));
            }

            IQueryable<VStudentLodFinalization> query = _context.VStudentLodFinalizations.TagWith("Use hint: RECOMPILE");

            if (input.PersonId.HasValue)
            {
                if (!await _authorizationService.HasPermissionForStudent(input.PersonId.Value, DefaultPermissions.PermissionNameForStudentLodFinalizationRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                query = query.Where(x => x.PersonId == input.PersonId);
            }
            else
            {
                if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodFinalizationRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                if (_userInfo.InstitutionID.HasValue)
                {
                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                }

                if (_userInfo.RegionID.HasValue)
                {
                    query = query.Where(x => x.InstitutionRegionId == _userInfo.RegionID.Value);
                }
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.FullName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter));

            IQueryable<LodFinalizationViewModel> listQuery = (from x in query
                                                              join lf in _context.Lodfinalizations on x.LodFinalizationId equals lf.Id into temp
                                                              from lodFin in temp.DefaultIfEmpty()
                                                              select new LodFinalizationViewModel
                                                              {
                                                                  PersonId = x.PersonId,
                                                                  FullName = x.FullName,
                                                                  Pin = x.Pin,
                                                                  PinType = x.PinType,
                                                                  SchoolYear = x.SchoolYear,
                                                                  SchoolYearName = x.SchoolYearName,
                                                                  InstitutionId = x.InstitutionId,
                                                                  InstitutionName = x.InstitutionAbbreviation,
                                                                  IsApproved = x.IsApproved,
                                                                  IsFinalized = x.IsFinalized,
                                                                  Document = lodFin.Document.ToViewModel(_blobServiceConfig),
                                                                  CanBeUndone = x.CreatedBySysUserId == _userInfo.SysUserID
                                                                    || x.ModifiedBySysUserId == _userInfo.SysUserID
                                                                    || (x.InstitutionId == _userInfo.InstitutionID)
                                                              })
                                                              .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc" : input.SortBy);


            int totalCount = await listQuery.CountAsync(cancellationToken);

            IList<LodFinalizationViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<byte[]> GeneratePersonalFileForStayAsync(LodGeneratorModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VLodStudentClass> studentClassesQuery = _context.VLodStudentClasses
                .AsNoTracking()
                .Where(x => x.PersonId == model.PersonId);


            if (model.ClassId.HasValue)
            {
                if (!await _authorizationService.HasPermissionForClass(model.ClassId ?? 0, DefaultPermissions.PermissionNameForClassStudentsRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                var classGroup = await _context.ClassGroups
                    .AsNoTracking()
                    .Where(x => x.ClassId == model.ClassId.Value)
                    .Select(x => new
                    {
                        x.ClassId,
                        x.InstitutionId,
                        x.SchoolYear
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);

                if(classGroup.InstitutionId != _userInfo.InstitutionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                studentClassesQuery = studentClassesQuery
                    .Where(x => x.InstitutionId == classGroup.InstitutionId && x.SchoolYear <= classGroup.SchoolYear);

            } else
            {
                if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDataRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            var studentSummary = await _studentService.GetSummaryByIdAsync(model.PersonId, cancellationToken);
            var studentClasses = await studentClassesQuery
                .OrderByDescending(x => x.EnrollmentDate)
                .ToListAsync(cancellationToken);

            // Взима периода на обучение в институцията на последная StudentClass(подреден по EnrollmentDate),
            // без да е местен в друга институция.
            // Случва се да е местен в друга институция в учебната година на последная StudentClass.
            // В този случай staySchoolYears има само една учебна година, примерно 2023, но в нея е бил в повече от една институция.
            // В генерирания ЛОД ще се появят Общи данни за обучението за повече от една институция.
            VLodStudentClass lastClass = studentClasses.FirstOrDefault();
            VLodStudentClass firstClass = studentClasses.FirstOrDefault();
            if (lastClass != null)
            {
                int? currentInstitution = lastClass.InstitutionId;
                foreach (var sc in studentClasses)
                {
                    if (sc.InstitutionId == currentInstitution)
                    {
                        firstClass = sc;
                    }
                    else
                    {
                        break;
                    }
                }

                List<int> staySchoolYears = Enumerable.Range(firstClass.SchoolYear, (lastClass.SchoolYear - firstClass.SchoolYear) + 1).ToList();
                model.SchoolYears = staySchoolYears;
            }
            else
            {
                // Няма активен StudentClaSss, генерираме личен картон за текущата учебна година на институцията или за глобална такава
                short currentSchoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);
                model.SchoolYears = new List<int>() { currentSchoolYear };
            }

            return await GeneratePersonalFileInternal(model, cancellationToken);
        }

        public async Task<byte[]> GeneratePersonalFile(LodGeneratorModel model, CancellationToken cancellationToken)
        {
            if (model.ClassId.HasValue)
            {
                if (!await _authorizationService.HasPermissionForClass(model.ClassId ?? 0, DefaultPermissions.PermissionNameForClassStudentsRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                var classGroup = await _context.ClassGroups
                    .AsNoTracking()
                    .Where(x => x.ClassId == model.ClassId.Value)
                    .Select(x => new
                    {
                        x.ClassId,
                        x.InstitutionId,
                        x.SchoolYear,
                        HasAny = x.StudentClasses.Any(sc => sc.PersonId == model.PersonId && (sc.IsCurrent || sc.Status == (int)MovementStatusEnum.Enrolled))
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);

                if (classGroup.InstitutionId != _userInfo.InstitutionID || !classGroup.HasAny)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else
            {
                if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? 0, DefaultPermissions.PermissionNameForStudentPersonalDataRead))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            return await GeneratePersonalFileInternal(model, cancellationToken);
        }

        public async Task<byte[]> GeneratePersonalFileInternal(LodGeneratorModel model, CancellationToken cancellationToken)
        {
            int personId = model?.PersonId ?? 0;
            List<int> schoolYears = model?.SchoolYears ?? new List<int>();

            var student = await _studentService.GetByIdAsync(personId);

            var studentClassess = await _context.VLodStudentClasses
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.EnrollmentDate)
                .ToListAsync(cancellationToken);

            var currentInstitutionData = await
                (from i in _context.InstitutionSchoolYears
                 where i.InstitutionId == _userInfo.InstitutionID
                 orderby i.SchoolYear descending
                 select new
                 {
                     schoolName = i.Name,
                     schoolSettlement = i.Town.Name,
                     schoolMunicipality = i.Town.Municipality.Name,
                     schoolRegion = i.LocalArea.Name,
                     schoolDistrict = i.Town.Municipality.Region.Name
                 }).FirstOrDefaultAsync(cancellationToken);

            var studentBirthPlace = student.BirthPlaceId != null ?
                await
                (from i in _context.Towns.AsNoTracking()
                 where i.TownId == student.BirthPlaceId
                 select new
                 {
                     studentBirthPlace = i.Name,
                     studentMunicipality = i.Municipality.Name,
                     studentDistrict = i.Municipality.Region.Name,
                     studentAddress = student.CurrentAddress,
                 }).FirstOrDefaultAsync(cancellationToken)
                 :
                 new
                 {
                     studentBirthPlace = $"{student.BirthPlace}, {(await _context.Countries.FirstOrDefaultAsync(i => i.CountryId == student.BirthPlaceCountryId))?.Name}",
                     studentMunicipality = "",
                     studentDistrict = "",
                     studentAddress = student.CurrentAddress,
                 };

            var diplomas = await _context.Diplomas
                .Where(x => x.PersonId == personId && !x.IsCancelled)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .Select(x => new
                {
                    x.BasicDocumentName,
                    x.InstitutionName,
                    x.Series,
                    x.FactoryNumber,
                    x.RegistrationNumberYear,
                    x.RegistrationNumberTotal,
                    x.RegistrationDate
                })
                .ToListAsync(cancellationToken);

            var otherDocument = await _context.OtherDocuments
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .Select(x => new
                {
                    BasicDocumentName = x.BasicDocument.Name,
                    InstitutionName = x.InstitutionSchoolYear.Name ?? x.InstitutionName,
                    x.Series,
                    x.FactoryNumber,
                    RegistrationNumberYear = x.RegNumber,
                    RegistrationNumberTotal = x.RegNumberTotal,
                    RegistrationDate = x.IssueDate
                })
                .ToListAsync(cancellationToken);

            if (model.DischargeDocument != null)
            {
                int? currentStudentClassId = await _dischargeDocumentService.GetCurrentStudentClass(model.DischargeDocument.PersonId, model.DischargeDocument.InstitutionId ?? _userInfo.InstitutionID ?? int.MinValue, model.DischargeDocument.StudentClassId);
                VLodStudentClass studentClass = studentClassess.FirstOrDefault(x => x.StudentClassId == currentStudentClassId);
                if (studentClass != null && !studentClass.DischargeDocDischargeDate.HasValue)
                {
                    studentClass.DischargeDocNoteNumber = model.DischargeDocument.NoteNumber;
                    studentClass.DischargeDocNoteDate = model.DischargeDocument.NoteDate;
                    studentClass.DischargeDocDischargeDate = model.DischargeDocument.DischargeDate;
                }
            }
            else if (model.RelocationDocument != null)
            {
                VLodStudentClass studentClass = studentClassess.FirstOrDefault(x => x.StudentClassId == model.RelocationDocument.CurrentStudentClassId);
                if (studentClass != null && !studentClass.DischargeDocDischargeDate.HasValue)
                {
                    studentClass.DischargeDocNoteNumber = model.RelocationDocument.NoteNumber;
                    studentClass.DischargeDocNoteDate = model.RelocationDocument.NoteDate;
                    studentClass.DischargeDocDischargeDate = model.RelocationDocument.DischargeDate;
                }
            }

            LodFileModel lodModel = new LodFileModel
            {
                schoolName = currentInstitutionData?.schoolName,
                schoolSettlement = currentInstitutionData?.schoolSettlement,
                schoolMunicipality = currentInstitutionData?.schoolMunicipality,
                schoolRegion = currentInstitutionData?.schoolRegion,
                schoolDistrict = currentInstitutionData?.schoolDistrict,
                studentName = $"{student.FirstName} {student.MiddleName} {student.LastName}",
                studentEGN = student.PinTypeId == 0 ? student.Pin : null,
                studentLNC = student.PinTypeId == 1 ? student.Pin : null,
                studentOtherIdentifier = student.PinTypeId == 2 ? student.Pin : null,
                studentNationality = student.Nationality,
                studentBirthDate = student.BirthDateString,
                studentBirthPlace = studentBirthPlace?.studentBirthPlace,
                studentBirthMunicipality = studentBirthPlace?.studentMunicipality,
                studentBirthDistrict = studentBirthPlace?.studentDistrict,
                studentAddress = $"{student.UsualResidenceAddress?.DropdownModel?.Name} {student.CurrentAddress}",
                studentPhone = student.PhoneNumber,
                studentClasses = studentClassess.Select(x => new StudentClassModel
                {
                    institutionName = x.InstitutionName,
                    admissionDocumentDetails = $"{x.AdmissionDocNoteNumber ?? "-"} / {(x.AdmissionDocNoteDate.HasValue ? x.AdmissionDocNoteDate.Value.ToString("dd.MM.yyyy") : "-")}",
                    admissionDocumentDate = x.AdmissionDocDate.HasValue ? $"{x.AdmissionDocDate:dd.MM.yyyy}" : "-",
                    dischargeDocumentDetails = $"{x.DischargeDocNoteNumber ?? "-"} / {(x.DischargeDocNoteDate.HasValue ? x.DischargeDocNoteDate.Value.ToString("dd.MM.yyyy") : "-")}",
                    dischargeDocumentDate = x.DischargeDocDischargeDate.HasValue ? $"{x.DischargeDocDischargeDate:dd.MM.yyyy}" : "-",
                    className = x.ClassName,
                    basicClassName = x.BasicClassId > 0 ? x.BasciClassName : "-",
                    schoolYear = x.SchoolYearName,
                    enrollmentDate = x.EnrollmentDate < GlobalConstants.StudentClassEnrollmentMigrationDate ? "" : $"{x.EnrollmentDate:dd.MM.yyyy}",
                    eduForm = x.EduFormName,
                    classType = x.ClassTypeName,
                    profession = x.Profession,
                    speciality = x.Speciality,

                }).ToList(),
                documents = diplomas.Union(otherDocument)
                    .OrderBy(x => x.RegistrationDate)
                    .Select(x => new DocumentModel
                    {
                        type = x.BasicDocumentName ?? "-",
                        issuer = x.InstitutionName ?? "-",
                        series = x.Series ?? "-",
                        factoryNumber = x.FactoryNumber ?? "-",
                        registrationNumber = $"{x.RegistrationNumberTotal} - {x.RegistrationNumberYear}",
                        registrationDate = x.RegistrationDate.HasValue ? $"{x.RegistrationDate.Value:dd.MM.yyyy}" : "-"
                    }).ToList()
            };

            await LoadAssessments(personId, lodModel, schoolYears, cancellationToken);
            await LoadReassessments(personId, lodModel, schoolYears, cancellationToken);
            await LoadExternalEvaluations(personId, lodModel, schoolYears, cancellationToken);
            await LoadAbsences(personId, lodModel, schoolYears, cancellationToken);
            await LoadPersonalDevelopmentSupport(personId, schoolYears, lodModel, cancellationToken);
            await LoadSops(personId, lodModel, schoolYears, cancellationToken);
            await LoadResourceSupports(personId, lodModel, schoolYears, cancellationToken);
            await LoadScholarships(personId, lodModel, schoolYears, cancellationToken);
            await LoadAwards(personId, lodModel, schoolYears, cancellationToken);
            await LoadSanctions(personId, lodModel, schoolYears, cancellationToken);
            await LoadSeldGovernment(personId, lodModel, schoolYears, cancellationToken);
            await LoadInternationalMobility(personId, lodModel, schoolYears, cancellationToken);
            await LoadPreSchoolReadinessResults(personId, lodModel, schoolYears, cancellationToken);

            using var ms = new MemoryStream();
            await _wordTemplateService.TransformAsync(
                "Lod",
                JsonSerializer.Serialize(lodModel),
                ms,
                cancellationToken);

            return ms.ToArray();
        }

        private async Task LoadPreSchoolReadinessResults(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            string contentDescription = await _context.PreSchoolReadinesses.Where(w => w.PersonId == personId).Select(s => s.Contents).FirstOrDefaultAsync(cancellationToken);

            var query = await _context.PreSchoolEvaluations
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .Select(x => new
                {
                    x.BasicClassId,
                    BasicClassDescription = x.BasicClass.Description,
                    SchoolYear = x.SchoolYear.ToString(),
                    subjectId = x.Subject.SubjectName,
                    schoolYear = x.SchoolYear.ToString(),
                    endOfYearEvaluation = x.EndOfYearEvaluation,
                }).ToListAsync(cancellationToken);


            if (query.Count == 0 || query.IsNullOrEmpty())
            {
                return;
            }

            var preSchoolEvaluations = (
                from x in query
                group x by new { x.BasicClassId, x.BasicClassDescription, x.SchoolYear } into g
                select new PreSchoolReadinessResultsSingleYearModel()
                {
                    basicClassId = g.Key.BasicClassId,
                    basicClassDescription = g.Key.BasicClassDescription ?? "-",
                    schoolYear = g.Key.SchoolYear.IsNullOrEmpty() ? "-" : $"{g.Key.SchoolYear}/{Convert.ToInt32(g.Key.SchoolYear) + 1}",
                    subjects = g.Select(i => new PreSchoolReadinessResultsModel()
                    {
                        subjectName = i.subjectId,
                        endOfYearEvaluation = i.endOfYearEvaluation,
                    }).ToList()
                }).ToList();

            model.preSchoolReadinessResults.AddRange(preSchoolEvaluations);
            model.contentDescription = contentDescription;
        }

        private async Task LoadAssessments(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<StudentLodAssessmentListModel> assessmentsList = await _context.VStudentLodAsssessmentLists
                .Where(x => x.BasicClassId.HasValue && x.PersonId == personId && (schoolYears.IsNullOrEmpty() || schoolYears.Contains(x.SchoolYear)))
                .Select(x => new StudentLodAssessmentListModel
                {
                    BasicClassId = x.BasicClassId ?? 0,
                    SchoolYear = x.SchoolYear,
                    IsSelfEduForm = x.IsSelfEduForm ?? false,
                    SchooYearName = x.SchoolYearName,
                    BasicClassDescription = x.BasicClassName
                })
                .ToListAsync(cancellationToken);

            if (assessmentsList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var al in assessmentsList)
            {
                List<LodAssessmentCurriculumPartModel> curriculumPartAsessments = await _lodAssessmentService.GetPersonAssessments(personId, al.BasicClassId, al.SchoolYear, al.IsSelfEduForm, null, null, cancellationToken);
                if (curriculumPartAsessments.IsNullOrEmpty())
                {
                    continue;
                }

                SchoolYearAssessmentsModel schoolYearAsssessmentsModel = new SchoolYearAssessmentsModel
                {
                    schoolYear = al.SchooYearName,
                    basicClass = al.BasicClassDescription
                };


                foreach (var cpa in curriculumPartAsessments)
                {
                    CurriculumPartAssessmentsModel cpm = new CurriculumPartAssessmentsModel
                    {
                        curriculumPart = cpa.CurriculumPart
                    };

                    foreach (LodAssessmentCreateModel s in cpa.SubjectAssessments)
                    {
                        cpm.subjectsAssessments.Add(new SubjectAssessmentsModel
                        {
                            subjectName = $"{s.SubjectName} / {s.SubjectTypeName}",
                            subjectType = s.SubjectTypeName,
                            horarium = s.AnnualHorarium?.ToString(),
                            fisrtTermGrade = GetGradeText(s.SubjectTypeId, s.FirstTermAssessments),
                            secondTermGrade = GetGradeText(s.SubjectTypeId, s.SecondTermAssessments),
                            annualGrade = GetGradeText(s.SubjectTypeId, s.AnnualTermAssessments),
                            firstRemedialSessionGrade = GetGradeText(s.SubjectTypeId, s.FirstRemedialSession),
                            secondRemedialSessionGrade = GetGradeText(s.SubjectTypeId, s.SecondRemedialSession),
                            additionalRemedialSessionGrade = GetGradeText(s.SubjectTypeId, s.AdditionalRemedialSession),

                            //fisrtTermGrade = s.FirstTermAssessments != null ? string.Join(", ", s.FirstTermAssessments.Where(x => !x.GradeText.IsNullOrWhiteSpace()).Select(g => g.GradeText)) : "",
                            //secondTermGrade = s.SecondTermAssessments != null ? string.Join(", ", s.SecondTermAssessments.Where(x => !x.GradeText.IsNullOrWhiteSpace()).Select(g => g.GradeText)) : "",
                            //annualGrade = s.AnnualTermAssessments != null ? string.Join(", ", s.AnnualTermAssessments.Where(x => !x.GradeText.IsNullOrWhiteSpace()).Select(g => s.Modules.IsNullOrEmpty() ? g.GradeText : g.DecimalGrade?.ToString("0.00") ?? "")) : "",
                            //firstRemedialSessionGrade = s.FirstRemedialSession != null ? string.Join(", ", s.FirstRemedialSession.Where(x => !x.GradeText.IsNullOrWhiteSpace()).Select(g => g.GradeText)) : "",
                            //secondRemedialSessionGrade = s.SecondRemedialSession != null ? string.Join(", ", s.SecondRemedialSession.Where(x => !x.GradeText.IsNullOrWhiteSpace()).Select(g => g.GradeText)) : "",
                            //additionalRemedialSessionGrade = s.AdditionalRemedialSession != null ? string.Join(", ", s.AdditionalRemedialSession.Where(x => !x.GradeText.IsNullOrWhiteSpace()).Select(g => g.GradeText)) : "",
                        });

                        if (!s.Modules.IsNullOrEmpty())
                        {
                            foreach (var m in s.Modules)
                            {
                                cpm.subjectsAssessments.Add(new SubjectAssessmentsModel
                                {
                                    subjectName = $"    * {m.SubjectName} / {m.SubjectTypeName}",
                                    subjectType = m.SubjectTypeName,
                                    horarium = m.AnnualHorarium?.ToString(),
                                    fisrtTermGrade = GetGradeText(m.SubjectTypeId, m.FirstTermAssessments),
                                    secondTermGrade = GetGradeText(m.SubjectTypeId, m.SecondTermAssessments),
                                    annualGrade = GetGradeText(m.SubjectTypeId, m.AnnualTermAssessments),
                                    firstRemedialSessionGrade = GetGradeText(m.SubjectTypeId, m.FirstRemedialSession),
                                    secondRemedialSessionGrade = GetGradeText(m.SubjectTypeId, m.SecondRemedialSession),
                                    additionalRemedialSessionGrade = GetGradeText(m.SubjectTypeId, m.AdditionalRemedialSession),
                                });
                            }
                        }
                    }

                    schoolYearAsssessmentsModel.curriculumParts.Add(cpm);
                }

                if (al.IsSelfEduForm)
                {
                    model.schoolYearSelfEduFormAsssessments.Add(schoolYearAsssessmentsModel);
                }
                else
                {
                    model.schoolYearAssessments.Add(schoolYearAsssessmentsModel);
                }
            }
        }

        /// <summary>
        /// Изпити за промяна на оценка.
        /// Изпитите за промяна на окончателна оценка(за I или II гимназиален етап) нямат BasicClass
        /// и не се показват в оценките от обучението.
        /// Затова ще се отделят в нова секция.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="model"></param>
        /// <param name="schoolYears"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private async Task LoadReassessments(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            var grades = await _context.ReassessmentDetails
                .Where(x => x.Reassessment.PersonId == personId && !x.Reassessment.BasicClassId.HasValue
                    && (schoolYears.IsNullOrEmpty() || schoolYears.Contains(x.Reassessment.SchoolYear)))
                .Select(x => new
                {
                    x.Reassessment.SchoolYear,
                    SchoolYearName = x.Reassessment.SchoolYearNavigation.Name,
                    x.SubjectId,
                    x.SubjectTypeId,
                    x.Subject.SubjectName,
                    SubjectTypeName = x.SubjectType.Name,
                    x.Position,
                    Type = x.Reassessment.Reason.Description,
                    Grade = new
                    {
                        x.Grade,
                        x.OtherGrade,
                        x.SpecialNeedsGrade,
                        x.GradeCategory,
                        GradeCategoryName = x.GradeCategoryNavigation.Name,
                        x.Position
                    }
                })
                .ToListAsync(cancellationToken);

            if (grades.Count == 0)
            {
                return;
            }

            var gradeNoms = await _context.GradeNoms
                .Select(x => new
                {
                    x.Id,
                    x.GradeTypeId,
                    x.Description,
                    x.Name
                }).ToListAsync(cancellationToken);

            foreach (var grade in grades.OrderBy(x => x.SchoolYear).ThenBy(x => x.SubjectId))
            {
                SubjectAssessmentsModel reassessmentModel = new SubjectAssessmentsModel
                {
                    schoolYear = grade.SchoolYearName ?? "",
                    subjectName = $"{grade.SubjectName} / {grade.SubjectTypeName}",
                    subjectType = grade.Type ?? "",
                    horarium = "",
                };

                switch (grade.Grade.GradeCategory)
                {
                    case (int)GradeCategoryEnum.Normal:
                        reassessmentModel.annualGrade = grade.Grade.Grade.HasValue
                            ? string.Format(grade.Grade.Grade.Value % 1 == 0 ? "{0:0}" : "{0:0.00}", grade.Grade.Grade.Value)
                            : "";
                        break;
                    case (int)GradeCategoryEnum.SpecialNeeds:
                        var nom = gradeNoms.FirstOrDefault(x => grade.Grade.SpecialNeedsGrade.HasValue && x.Id == (int)grade.Grade.SpecialNeedsGrade.Value);
                        reassessmentModel.annualGrade = nom?.Name ?? "";
                        break;
                    case (int)GradeCategoryEnum.Other:
                        var nom1 = gradeNoms.FirstOrDefault(x => grade.Grade.OtherGrade.HasValue && x.Id == (int)grade.Grade.OtherGrade.Value);
                        reassessmentModel.annualGrade = nom1?.Name ?? "";
                        break;
                    case (int)GradeCategoryEnum.Qualitative:
                        var nom2 = gradeNoms.FirstOrDefault(x => grade.Grade.Grade.HasValue && x.Id == (int)grade.Grade.Grade.Value);
                        reassessmentModel.annualGrade = nom2?.Name ?? "";
                        break;
                    default:
                        break;
                }

                model.reassessments.Add(reassessmentModel);
            }
        }

        private string GetGradeText(int subjectTypeId, List<LodAssessmentGradeCreateModel> models)
        {
            if (models.IsNullOrEmpty())
            {
                return "";
            }

            List<string> grades = new List<string>();

            foreach (LodAssessmentGradeCreateModel model in models)
            {
                // Профилиращ предмет.
                // Ще покажем десетичната оценка, ако има такава
                string grade = GlobalConstants.SubjectTypesOfProfileSubject.Contains(subjectTypeId) && model.DecimalGrade.HasValue
                ? $"{GradeUtils.GetDecimalGradeText(model.DecimalGrade)} {string.Format(model.DecimalGrade.Value % 1 == 0 ? "{0:0}" : "{0:0.00}", model.DecimalGrade.Value)}"
                    : model.GradeText;
                if (!grade.IsNullOrWhiteSpace())
                {
                    if (!model.CategoryAbbreviation.IsNullOrEmpty())
                    {
                        grade = $"{grade} ({model.CategoryAbbreviation})";
                    }
                    grades.Add(grade);
                }
            }

            return string.Join(", ", grades);
        }

        private async Task LoadExternalEvaluations(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            IQueryable<ExternalEvaluation> query = _context.ExternalEvaluations
                .Where(x => x.PersonId == personId && !x.ExternalEvaluationType.IsUnofficial);

            if (!schoolYears.IsNullOrEmpty())
            {
                query = query.Where(x => x.SchoolYear.HasValue && schoolYears.Contains(x.SchoolYear.Value));
            }

            model.schoolYearExternalEvaluations = await query
                .Select(x => new SchoolYearExternalEvaluationsModel()
                {
                    schoolYear = x.SchoolYearNavigation.Name,
                    name = x.ExternalEvaluationType.Name,
                    externalEvaluationItems = x.ExternalEvaluationItems.Select(eei => new ExternalEvaluationItemSubjectModel()
                    {
                        subjectName = eei.SubjectNavigation.SubjectName,
                        grade = (eei.Grade != null && eei.Grade != 0) ? eei.Grade.Value.ToString("N2") : "",
                        points = eei.Points.ToString("N2"),
                        originalPoints = eei.OriginalPoints.ToString("N2"),
                        flLevel = eei.Fllevel
                    }).ToList()
                }).ToListAsync();
        }

        private async Task LoadAbsences(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<VLodStudentAbsence> absences = await _context.VLodStudentAbsences
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .ToListAsync(cancellationToken);

            if (absences.IsNullOrEmpty())
            {
                return;
            }

            foreach (var g in absences.OrderByDescending(x => x.SchoolYear).GroupBy(x => x.SchoolYearName))
            {
                decimal? firstTermExcused = g.Where(x => x.Term == (int)SchoolTerm.TermOne).Sum(x => x.Excused);
                decimal? firstTermUnexcused = g.Where(x => x.Term == (int)SchoolTerm.TermOne).Sum(x => x.Unexcused);
                decimal? secondTermExcused = g.Where(x => x.Term == (int)SchoolTerm.TermTwo).Sum(x => x.Excused);
                decimal? secondTermUnexcused = g.Where(x => x.Term == (int)SchoolTerm.TermTwo).Sum(x => x.Unexcused);
                decimal? totalExcused = firstTermExcused + secondTermExcused + g.Where(x => x.Term == null).Sum(x => x.Excused);
                decimal? totalUnexcused = firstTermUnexcused + secondTermUnexcused + g.Where(x => x.Term == null).Sum(x => x.Unexcused);

                model.schoolYearAbsences.Add(new SchoolYearAbsencesModel
                {
                    schoolYear = g.Key,
                    firstTermExcused = firstTermExcused > 0 ? $"{firstTermExcused:0.##}" : "",
                    firstTermUnexcused = firstTermUnexcused > 0 ? $"{firstTermUnexcused:0.##}" : "",
                    secondTermExcused = secondTermExcused > 0 ? $"{secondTermExcused:0.##}" : "",
                    secondTermUnexcused = secondTermUnexcused > 0 ? $"{secondTermUnexcused:0.##}" : "",
                    totalExcused = totalExcused > 0 ? $"{totalExcused:0.##}" : "",
                    totalTermUnexcused = totalUnexcused > 0 ? $"{totalUnexcused:0.##}" : "",
                }); ;
            }
        }

        private async Task LoadInternationalMobility(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<InternationalMobilityViewModel> internationalMobilityModels = await _context.InternationalMobilities
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new InternationalMobilityViewModel
                {
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    ReceivingInstitution = x.ReceivingInstitution,
                    MainObjectives = x.MainObjectives,
                    Project = x.Project,
                    Country = x.Country.Name,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                }).ToListAsync(cancellationToken);


            if (internationalMobilityModels.IsNullOrEmpty())
            {
                return;
            }

            model.internationalMobilities.AddRange(internationalMobilityModels.Select(x => new SchoolYearInternationalMobilityModel
            {
                schoolYear = x.SchoolYearName,
                project = x.Project ?? "-",
                country = x.Country ?? "-",
                receivingInstitution = x.ReceivingInstitution ?? "-",
                mainObjectives = x.MainObjectives ?? "-",
                startDate = x.FromDate.HasValue ? $"{x.FromDate:dd.MM.yyyy}" : "-",
                endDate = x.ToDate.HasValue ? $"{x.ToDate:dd.MM.yyyy}" : "-",
            }));
        }

        private async Task LoadSeldGovernment(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<SelfGovernmentViewModel> selfGovernmentModels = await _context.SelfGovernments
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SelfGovernmentViewModel
                {
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    Institution = x.InstitutionSchoolYear.Name,
                    Participation = x.Participation.Name,
                    ParticipationAdditionalInformation = x.ParticipationAdditionalInformation,
                    Position = x.Position.Name,
                    MobilePhone = x.Person.Student.MobilePhone,
                    Email = x.Person.Student.Email,
                    AdditionalInformation = x.AdditionalInformation,
                }).ToListAsync(cancellationToken);

            if (selfGovernmentModels.IsNullOrEmpty())
            {
                return;
            }

            model.selfGovernments.AddRange(selfGovernmentModels.Select(x => new SchoolYearSelfGovernmentModel
            {
                schoolYear = x.SchoolYearName,
                institution = x.Institution ?? "-",
                participation = x.Participation ?? "-",
                participationAdditionalInformation = x.ParticipationAdditionalInformation ?? "-",
                position = x.Position ?? "-",
                mobilePhone = x.MobilePhone ?? "-",
                email = x.Email ?? "-",
                аdditionalInformation = x.AdditionalInformation ?? "-"
            }));
        }

        private async Task LoadSanctions(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<SchoolYearSanctionModel> sanctionsModels = await _context.Sanctions
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearSanctionModel
                {
                    schoolYear = x.SchoolYearNavigation.Name,
                    orderDate = $"{x.OrderDate:dd.MM.yyyy}",
                    orderNumber = x.OrderNumber ?? "-",
                    ruoOrderDate = x.RuoOrderDate.HasValue ? $"{x.RuoOrderDate:dd.MM.yyyy}" : "-",
                    ruoOrderNumber = x.RuoOrderNumber ?? "-",
                    type = x.SanctionType.Description,
                    institution = x.InstitutionSchoolYear.Name,
                    startDate = $"{x.StartDate:dd.MM.yyyy}",
                    endDate = x.EndDate.HasValue ? $"{x.EndDate:dd.MM.yyyy}" : "-",
                    cancelationOrderDate = x.CancelOrderDate.HasValue ? $"{x.CancelOrderDate:dd.MM.yyyy}" : "-",
                    cancelationOrderNumber = x.CancelOrderNumber,
                    description = x.Description,
                    cancelationReason = x.CancelReason

                }).ToListAsync(cancellationToken);

            model.schoolYearSanctions.AddRange(sanctionsModels);
        }

        private async Task LoadAwards(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<SchoolYearAwardModel> awardModels = await _context.Awards
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearAwardModel
                {
                    schoolYear = x.InstitutionSchoolYear.SchoolYearNavigation.Name ?? "-",
                    institution = x.InstitutionSchoolYear.Name ?? "-",
                    category = x.AwardCategory.Name ?? "-",
                    categoryAdditionalInformation = x.AdditionalInformation ?? "-",
                    type = x.AwardType.Name ?? "-",
                    founder = x.Founder.Name ?? "-",
                    awardReason = x.AwardReason.Name ?? "-",
                    orderNumber = x.OrderNumber ?? "-",
                    orderDate = $"{x.Date:dd.MM.yyyy}",
                    description = x.Description ?? "-"
                }).ToListAsync(cancellationToken);

            model.schoolYearAwards.AddRange(awardModels);
        }

        private async Task LoadScholarships(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<SchoolYearScholarshipModel> scholarshipModels = await _context.ScholarshipStudents
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearScholarshipModel
                {
                    schoolYear = x.InstitutionSchoolYear.SchoolYearNavigation.Name ?? "-",
                    institution = x.InstitutionSchoolYear.Name ?? "-",
                    scholarshipFinancingOrgName = x.FinancingOrgan.Description ?? "-",
                    amountRate = x.AmountRate > 0 ? $"{x.AmountRate:#.##}" : "-",
                    amountRateType = x.ScholarshipAmount.Name ?? "-",
                    reasonType = x.ScholarshipType.Name ?? "-",
                    periodicity = x.Periodicity == 1
                        ? "Месечна"
                        : x.Periodicity == 2
                            ? "Еднократна"
                            : "-",
                    startDate = $"{x.ValidFrom:dd.MM.yyyy}",
                    endDate = x.ValidTo.HasValue ? $"{x.ValidTo:dd.MM.yyyy}" : "-",
                    orderNumber = x.OrderNumber ?? "-",
                    orderDate = x.OrderDate.HasValue ? $"{x.OrderDate:dd.MM.yyyy}" : "-",
                    proposalNumber = x.CommissionNumber ?? "-",
                    proposalDate = x.CommissionDate.HasValue ? $"{x.CommissionDate:dd.MM.yyyy}" : "-",
                    description = x.Description ?? "-",
                }).ToListAsync(cancellationToken);

            model.schoolYearScholarships.AddRange(scholarshipModels);
        }

        private async Task LoadResourceSupports(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<SchoolYearResourceSupportDocumentModel> resourceSupportModels = await _context.ResourceSupportReports
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearResourceSupportDocumentModel
                {
                    schoolYear = x.SchoolYearNavigation.Name ?? "-",
                    reportNumber = x.ReportNumber ?? "-",
                    reportDate = $"{x.ReportDate:dd.MM.yyyy}",
                    resourceSupports = x.ResourceSupports.Select(r => new Domain.Models.ResourceSupportModel
                    {
                        type = r.ResourceSupportType.Name ?? "-",
                        suspensionDate = r.AdditionalPersonalDevelopmentSupportItem.SuspensionDate.HasValue ? $"{r.AdditionalPersonalDevelopmentSupportItem.SuspensionDate:dd.MM.yyyy}" : "-",
                        suspensionReason = r.AdditionalPersonalDevelopmentSupportItem.SuspensionReason ?? "-",
                        specialists = r.ResourceSupportSpecialists.Select(s => new Domain.Models.ResourceSupportSpecialistModel
                        {
                            type = s.ResourceSupportSpecialistType.Name ?? "-",
                            workplace = s.WorkPlace.Name ?? "-",
                        }).ToList()
                    }).ToList(),
                }).ToListAsync(cancellationToken);

            model.schoolYearResourceSupportDocuments.AddRange(resourceSupportModels);
        }

        private async Task LoadSops(int personId, LodFileModel model, List<int> schoolYears, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            List<SchoolYearSopModel> sopModels = await _context.SpecialNeedsYears
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearSopModel
                {
                    schoolYear = x.SchoolYearNavigation.Name ?? "-",
                    sops = x.SpecialNeeds.Select(s => new SopModel
                    {
                        type = s.SpecialNeedsType.Name ?? "-",
                        subType = s.SpecialNeedsSubType.Name ?? "-",
                    }).ToList()
                }).ToListAsync(cancellationToken);

            model.schoolYearSops.AddRange(sopModels);
        }

        private async Task LoadPersonalDevelopmentSupport(int personId, List<int> schoolYears, LodFileModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(LodFileModel)));
            }

            model.schoolYearCommonPersonDevelopmentSupports = await _context.CommonPersonalDevelopmentSupports
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearCommonPersonDevelopmentSupportModel
                {
                    schoolYear = x.SchoolYearNavigation.Name ?? "-",
                    items = x.CommonPersonalDevelopmentSupportItems
                        .Select(i => new PersonDevelopmentSupportModel
                        {
                            type = i.Type.Name ?? "-",
                            description = i.Details ?? "",
                        }).ToList()
                }).ToListAsync(cancellationToken);

            // Todo: Стар начин. Да се изтрие, когато Надграждане на Приобщаващо образование се приеме
            List<SchoolYearAdditionalPersonDevelopmentSupportModel> models = await _context.AdditionalPersonalDevelopmentSupports
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Where(!schoolYears.IsNullOrEmpty(), x => schoolYears.Contains(x.SchoolYear))
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new SchoolYearAdditionalPersonDevelopmentSupportModel
                {
                    schoolYear = x.SchoolYearNavigation.Name ?? "-",
                    finalSchoolYear = x.FinalSchoolYearNavigation.Name ?? "",
                    period = $"{x.PeriodType.Name ?? "-"}{(x.FinalSchoolYear.HasValue ? $" - до {x.FinalSchoolYearNavigation.Name}" : "")}" ,
                    personDescription = x.StudentType.Name ?? "-",
                    orderNumber = x.OrderNumber ?? "-",
                    orderDate = $"{x.OrderDate:dd.MM.yyyy}",
                    items = x.AdditionalPersonalDevelopmentSupportItems.Select(s => new PersonDevelopmentSupportModel
                    {
                        type = s.Type.Name ?? "-",
                        description = s.Details ?? "",
                        suspensionDate = s.SuspensionDate.HasValue ? $"{s.SuspensionDate:dd.MM.yyyy}" : "-",
                        suspensionReason = s.SuspensionReason ?? "-"
                    }).ToList(),
                }).ToListAsync(cancellationToken);

            model.schoolYearAdditionalPersonDevelopmentSupports.AddRange(models);
        }
    }
}
