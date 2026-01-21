using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.DataAccess.Dto;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.EduState;
using MON.Models.Enums;
using MON.Models.Enums.UserManagement;
using MON.Models.Grid;
using MON.Models.Ores;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Search;
using MON.Models.StudentModels.Update;
using MON.Models.UserManagement;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class StudentService : BaseService<StudentService>, IStudentService
    {
        private const int StudentsCount = 100;
        private const int STUDENT_POSITION_ID = 3;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IUserManagementService _userManagementService;
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly EduStateCacheService _eduStateCacheService;
        private readonly IInstitutionService _institutionService;
        private readonly IAdmissionDocumentService _admissionDocumentService;

        public StudentService(DbServiceDependencies<StudentService> dependencies,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IUserManagementService userManagementService,
            ILodFinalizationService lodFinalizationService,
            EduStateCacheService eduStateCacheService,
            IInstitutionService institutionService,
            IAdmissionDocumentService admissionDocumentService)
            : base(dependencies)
        {
            _blobServiceConfig = blobServiceConfig.Value;
            _userManagementService = userManagementService;
            _lodFinalizationService = lodFinalizationService;
            _eduStateCacheService = eduStateCacheService;
            _institutionService = institutionService;
            _admissionDocumentService = admissionDocumentService;
        }


        #region Private members
        private IQueryable<Person> GetAllStudents()
        {
            // Всички, които са в позиция ученик (или нямат позиция) и имат идентификатор
            return _context.People
                .Where(x => x.PersonalId != null && x.EducationalStates.All(y => y.PositionId == STUDENT_POSITION_ID));
        }
        private async Task UserManagementCreateStudent(Person person)
        {
            StudentRequestDto studentDto = new StudentRequestDto
            {
                PersonId = person.PersonId
            };

            await _userManagementService.CreateAsync(studentDto);
        }

        private async Task UserManagementUpdateStudent(Person person)
        {
            StudentRequestDto studentDto = new StudentRequestDto
            {
                PersonId = person.PersonId
            };

            await _userManagementService.UpdateAsync(studentDto);
        }

        private IQueryable<StudentSearchDto> GetStudentSearchQuery(StudentSearchModelExtended studentSearchModel = null)
        {
			return null;
        }

        private bool CheckStudentStateInInstitution(List<EduStateModel> eduStates, InstitutionCacheModel institution)
        {
            bool isStudent = false;
            var institutionType = (InstitutionTypeEnum)institution.InstTypeId;

            switch (institutionType)
            {
                case InstitutionTypeEnum.School:
                case InstitutionTypeEnum.KinderGarden:
                    isStudent = eduStates.Where(x => x.InstitutionId == institution.Id).Any(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds);
                    break;
                case InstitutionTypeEnum.PersonalDevelopmentSupportCenter:
                case InstitutionTypeEnum.SpecializedServiceUnit:
                    isStudent = eduStates.Where(x => x.InstitutionId == institution.Id).Any(x => x.PositionId == (int)PositionType.StudentPersDevelopmentSupport);
                    break;
                case InstitutionTypeEnum.CenterForSpecialEducationalSupport:
                    isStudent = eduStates.Where(x => x.InstitutionId == institution.Id).Any(x => x.PositionId == (int)PositionType.StudentOtherInstitution);
                    break;
                default:
                    isStudent = false;
                    break;
            }

            return isStudent;
        }

        private IQueryable<StudentClass> MainStudentSlasses(int personId, bool? isCurrent = true, int? schoolYear = null, int? institutionId = null, bool? includeIsNotPresentForm = null)
        {
            IQueryable<StudentClass> query = _context.StudentClasses
                .Where(x => x.PersonId == personId)
                .Where(x => (includeIsNotPresentForm == true && x.IsNotPresentForm == includeIsNotPresentForm)
                    ||
                    (x.Class.ClassType.ClassKind == (int)ClassKindEnum.Basic
                    && (
                        (x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport && x.PositionId == (int)PositionType.StudentOtherInstitution)
                        ||
                        ((x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.School || x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.KinderGarden)
                            && x.PositionId == (int)PositionType.Student)
                    )
                    && (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentOtherInstitution)));

            if (isCurrent.HasValue)
            {
                query = query.Where(x => x.IsCurrent == isCurrent.Value);
            }

            if (schoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == schoolYear.Value);
            }

            if (institutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == institutionId.Value);
            }

            return query;
        }

        #endregion

        public async Task<StudentSummaryModel> GetSummaryByIdAsync(int id, CancellationToken cancellationToken)
        {
            bool hasPermissionForPersonalDataRead = await _authorizationService.HasPermissionForStudent(id, DefaultPermissions.PermissionNameForStudentPersonalDataRead);
            bool hasPermissionForPartialPersonalDataRead = await _authorizationService.HasPermissionForStudent(id, DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead);

            var summary = await _context.VStudentSummaries
                .Where(s => s.PersonId == id)
                .Select(s => new StudentSummaryModel
                {
                    PersonId = s.PersonId,
                    FullName = s.FullName,
                    Gender = hasPermissionForPersonalDataRead ? s.Gender : "",
                    PIN = s.Pin,
                    PINType = s.PinType,
                    PintTypeId = s.PintTypeId,
                    Age = s.Age ?? 0,
                    Email = hasPermissionForPersonalDataRead ? s.Email : "",
                    MobilePhone = hasPermissionForPersonalDataRead ? s.MobilePhone : "",
                    CurrentClass = new StudentClassSummaryModel
                    {
                        ClassSpecialityName = hasPermissionForPersonalDataRead ? s.ClassSpecialityName : "",
                        SchoolYear = s.SchoolYear ?? 0,
                        StudentClassId = hasPermissionForPersonalDataRead ? s.StudentClassId ?? 0 : 0,
                        BasicClassId = hasPermissionForPersonalDataRead ? s.BasicClassId ?? 0 : 0,
                        ClassName = hasPermissionForPersonalDataRead ? s.ClassName : "",
                        InstitutionId = hasPermissionForPersonalDataRead ? s.InstitutionId ?? 0 : 0,
                        InstitutionName = hasPermissionForPersonalDataRead ? s.InstitutionName : "",
                        EnrollmentDate = s.EnrollmentDate ?? DateTime.MinValue,
                        ClassEduFormId = hasPermissionForPersonalDataRead ? s.ClassEduFormId ?? 0 : 0,
                        ClassEduFormName = hasPermissionForPersonalDataRead ? s.ClassEduFormName : "",
                        IsCurrent = hasPermissionForPersonalDataRead ? s.IsCurrent : null,
                        Status = hasPermissionForPersonalDataRead ? s.Status : null
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (summary != null)
            {
                if (!hasPermissionForPersonalDataRead)
                {
                    var names = summary.FullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    summary.FullName = $"{names[0]} {String.Join(' ', names.Skip(1).Select(name => name.First() + "."))}";

                    summary.PIN = hasPermissionForPartialPersonalDataRead
                        ? $"{(summary.PIN ?? "")[0..^4]}XXXX" 
                        : GlobalConstants.PIN_ANONYMIZATION_STRING;
                }
                else
                {
                    // Групите паралелките са видими само при наличие на права.
                    IQueryable<StudentClass> query = _context.StudentClasses
                        .Where(x => x.PersonId == id && x.IsCurrent);
                    //if (_userInfo.InstitutionID.HasValue)
                    //{
                    //    int instId = _userInfo.InstitutionID.Value;
                    //    query = query.Where(x => x.InstitutionId == instId);
                    //    summary.WaitingToBeDischarged = await _context.VToBeDischargeds
                    //        .AsNoTracking()
                    //        .Where(x => x.PersonId == id && x.OldStudentClassid.HasValue && x.OldInstitutionId == instId)
                    //        .Select(x => new WaitingToBeDischarged
                    //        {
                    //            StudentClassId = x.OldStudentClassid.Value
                    //        })
                    //        .ToListAsync();
                    //}

                    summary.AllCurrentClasses = await query
                        .OrderBy(x => x.PositionId)
                        .ThenByDescending(x => x.Id)
                        .Select(x => new StudentClassSummaryModel
                        {
                            ClassId = x.ClassId,
                            StudentClassId = x.Id,
                            SchoolYear = x.SchoolYear,
                            BasicClassId = x.Class.BasicClassId ?? 0,
                            ClassName = x.Class.ClassName,
                            InstitutionId = x.Class.InstitutionId,
                            InstitutionName = x.Class.InstitutionSchoolYear.Name,
                            ClassEduFormId = x.Class.ClassEduFormId ?? 0,
                            ClassEduFormName = x.Class.ClassEduForm.Name,
                            ClassSpecialityName = x.Class.ClassSpeciality.Name,
                            EnrollmentDate = x.EnrollmentDate,
                            IsCurrent = x.IsCurrent,
                            Status = x.Status,
                            PositionId = x.PositionId,
                            Position = x.Position.Name,
                            ClassKind = x.Class.ClassType.ClassKind
                        })
                        .ToListAsync(cancellationToken);
                }

                summary.IsLodApproved = await _lodFinalizationService.IsLodApproved(summary.PersonId, (short)summary.CurrentClass.SchoolYear, cancellationToken);
                summary.IsLodFinalized = await _lodFinalizationService.IsLodFInalized(summary.PersonId, (short)summary.CurrentClass.SchoolYear, cancellationToken);
            }
            return summary;
        }

        public async Task<int> AddAsync(StudentCreateModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            Person person = new Person()
            {
                FirstName = model.FirstName?.Trim(),
                MiddleName = model.MiddleName?.Trim(),
                LastName = model.LastName?.Trim(),
                PersonalId = model.Pin,
                PersonalIdtype = model.PinTypeId,
                NationalityId = model.NationalityId,
                BirthDate = model.BirthDate,
                BirthPlaceCountry = model.BirthPlaceCountryId,
                BirthPlaceTownId = model.BirthPlaceId,
                PermanentAddress = model.PermanentAddress,
                CurrentAddress = model.CurrentAddress,
                SysUserType = (int)SysUserTypeEnum.Neispuo,
                Student = new Student
                {
                    MobilePhone = model.PhoneNumber,
                    Email = model.Email,
                },
                Gender = model.GenderId,
                BirthPlace = model.BirthPlace
            };

            // По някаква причина за определни ЕГН-та се връщата навалидни ЕКАТТЕ кодове за населините места.
            // Пример 97029 => ГР.СОФИЯ,КВ.ДРАГАЛЕВЦИ
            // Ще правим проверка дали този код съществува в номенклатурата location.Town.
            if (model.PermanentResidenceId.HasValue && await _context.Towns.AnyAsync(x => x.TownId == model.PermanentResidenceId.Value))
            {
                person.PermanentTownId = model.PermanentResidenceId.Value;
            }

            if (model.UsualResidenceId.HasValue && await _context.Towns.AnyAsync(x => x.TownId == model.UsualResidenceId.Value))
            {
                person.CurrentTownId = model.UsualResidenceId.Value;
            }

            _context.People.Add(person);
            await SaveAsync();

            await UserManagementCreateStudent(person);

            return person.PersonId;
        }

        public async Task UpdateAsync(StudentCreateModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            Person person = await _context.People
                .Include(c => c.Student)
                .SingleOrDefaultAsync(x => x.PersonId == model.Id);

            if (person == null)
            {
                throw new ArgumentNullException(nameof(Person), "Entity cant be null!");
            }

            if (!await _authorizationService.HasPermissionForStudent(person.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDataManage)
                || !await CanEditStudentPersonalDetails(person.PersonId))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (person.Student == null)
            {
                person.Student = new Student();
            }

            person.FirstName = model.FirstName;
            person.MiddleName = model.MiddleName;
            person.LastName = model.LastName;
            person.PersonalId = model.Pin;
            person.PersonalIdtype = model.PinTypeId;
            person.Gender = model.GenderId;
            person.BirthDate = model.BirthDate;
            person.BirthPlaceTownId = model.BirthPlaceId != 0 ? model.BirthPlaceId : null;
            person.BirthPlaceCountry = model.BirthPlaceCountryId != 0 ? model.BirthPlaceCountryId : null;
            person.PermanentTownId = model.PermanentResidenceId != 0 ? model.PermanentResidenceId : null;
            person.PermanentAddress = model.PermanentAddress;
            person.CurrentTownId = model.UsualResidenceId != 0 ? model.UsualResidenceId : null;
            person.CurrentAddress = model.CurrentAddress;
            person.NationalityId = model.NationalityId != 0 ? model.NationalityId : null;
            person.Student.MobilePhone = model.PhoneNumber;
            person.Student.Email = model.Email;
            person.BirthPlace = model.BirthPlace;

            // По някаква причина за определни ЕГН-та се връщата навалидни ЕКАТТЕ кодове за населините места.
            // Пример 97029 => ГР.СОФИЯ,КВ.ДРАГАЛЕВЦИ
            // Ще правим проверка дали този код съществува в номенклатурата location.Town.
            if (model.PermanentResidenceId.HasValue && await _context.Towns.AnyAsync(x => x.TownId == model.PermanentResidenceId.Value))
            {
                person.PermanentTownId = model.PermanentResidenceId.Value;
            }
            else
            {
                person.PermanentTownId = null;
            }

            if (model.UsualResidenceId.HasValue && await _context.Towns.AnyAsync(x => x.TownId == model.UsualResidenceId.Value))
            {
                person.CurrentTownId = model.UsualResidenceId.Value;
            }
            else
            {
                person.CurrentTownId = null;
            }

            await SaveAsync();

            await UserManagementUpdateStudent(person);
        }

        public IQueryable<StudentViewModel> Find(Expression<Func<StudentViewModel, bool>> expression)
        {
            return GetAll().Where(expression);
        }

        public IQueryable<StudentViewModel> GetAll()
        {
            return GetAllStudents()
                .Select(i => new StudentViewModel()
                {
                    Id = i.PersonId,
                    Pin = _userInfo.UserRole != UserRoleEnum.School ? GlobalConstants.PIN_ANONYMIZATION_STRING : i.PersonalId,
                    PinType = i.PersonalIdtypeNavigation.Name,
                    FirstName = i.FirstName,
                    MiddleName = i.MiddleName,
                    LastName = i.LastName,
                    BirthDate = i.BirthDate,
                    CurrentAddress = i.PermanentAddress,
                    Gender = i.GenderNavigation.Name,
                    PhoneNumber = i.Student.MobilePhone,
                    BirthPlace = i.BirthPlaceTown.Name,
                    NationalityId = -1,
                    InternationalProtectionStatus = i.InternationalProtections.Any(),
                    PermanentResidenceAddress = new AddressViewModel()
                    {
                        Municipality = "PermanentMun",
                        City = "PermanentCity",
                        District = "PermanentDisc"
                    },
                    UsualResidenceAddress = new AddressViewModel()
                    {
                        Municipality = "UsualMun",
                        City = "UsualCity",
                        District = "UsualDisc"
                    },
                    Education = new EducationViewModel()
                    {
                        BasicClassId = 33,
                        EducationForm = "testForm",
                        Grade = "testGrade",
                        Group = 1,
                        ReceptionAfter = "testReceipt",
                        RecordedInBookSubjectsPage = 1,
                        RecordedInBookSubjectsNumber = 4,
                        Specialty = "testSpec",
                        TrainingType = "testType"
                    },
                    Institutions = i.EducationalStates
                            .Where(x => x.InstitutionId.HasValue)
                            .Select(x => new InstitutionCacheModel()
                            {
                                Id = x.InstitutionId ?? 0,
                                Name = x.Institution.Name
                            }),
                    HasIndividualStudyPlan = i.StudentClasses.SingleOrDefault(x => x.SchoolYear == DateTime.Now.Year).IsIndividualCurriculum,
                    NumberInClass = 111
                }).AsQueryable();
        }

        public async Task<StudentPersonalDataModel> GetPersonDataByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be possitive number!");
            }

            bool hasPermissionForPersonalDataRead = await _authorizationService.HasPermissionForStudent(id, DefaultPermissions.PermissionNameForStudentPersonalDataRead);
            bool hasPermissionForPartialPersonalDataRead = await _authorizationService.HasPermissionForStudent(id, DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead);

            if (!hasPermissionForPersonalDataRead && !hasPermissionForPartialPersonalDataRead)
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            StudentPersonalDataModel model = await _context.People
                .Where(x => x.PersonId == id)
                .Select(x => new StudentPersonalDataModel
                {
                    Id = x.PersonId,
                    Pin = x.PersonalId,
                    PinType = new DropdownViewModel
                    {
                        Name = x.PersonalIdtypeNavigation.Name,
                        Value = x.PersonalIdtype ?? 0,
                        Text = x.PersonalIdtypeNavigation.Name
                    },
                    PublicEduNumber = x.PublicEduNumber,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    BirthDate = x.BirthDate,
                    Gender = new DropdownViewModel
                    {
                        Name = x.GenderNavigation.Name,
                        Value = x.Gender ?? 0,
                        Text = x.GenderNavigation.Name
                    },
                    GenderId = x.Gender ?? 0,
                    CurrentAddress = x.CurrentAddress,
                    PermanentAddress = x.PermanentAddress,
                    PhoneNumber = x.Student.MobilePhone,
                    Email = x.Student.Email,
                    BirthPlaceId = x.BirthPlaceTownId,
                    BirthPlaceCountryId = x.BirthPlaceCountry,
                    PermanentResidenceId = x.PermanentTownId,
                    UsualResidenceId = x.CurrentTownId,
                    NationalityId = x.NationalityId,
                    BirthPlace = x.BirthPlace
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model != null && !hasPermissionForPersonalDataRead)
            {
                model.Pin = $"{(model.Pin ?? "")[0..^4]}XXXX";
                model.PublicEduNumber = "";
                model.GenderId = 0;
                model.Gender = new DropdownViewModel { };
                model.CurrentAddress = "";
                model.PermanentAddress = "";
                model.PhoneNumber = "";
                model.Email = "";
                model.BirthPlaceId = null;
                model.BirthPlaceCountryId = null;
                model.BirthPlace = "";
                model.UsualResidenceId = null;
                model.NationalityId = null;
            }

            return model;
        }

        public async Task<StudentViewModel> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be possitive number!");
            }

            int currentYear = DateTime.Now.CurrentSchoolYear();

            var student = await _context.People
                 .Include(p => p.EducationalStates)
                     .ThenInclude(e => e.Position)
                 .Where(i => i.PersonId == id
                    // TODO - временно изключваме, защото в мигртираните дании има несъответсвия
                    // && i.EducationalStates.All(y => y.Position.PositionId == STUDENT_POSITION_ID)
                    )
                 .Select(i =>
                     new StudentViewModel
                     {
                         Id = i.PersonId,
                         Pin = i.PersonalId,
                         PinType = i.PersonalIdtypeNavigation.Name,
                         // При липса на PersonalIdType, предполагаме ЕГН
                         PinTypeId = i.PersonalIdtype != null ? i.PersonalIdtype.Value : 0,
                         PublicEduNumber = i.PublicEduNumber,
                         FirstName = i.FirstName,
                         MiddleName = i.MiddleName,
                         LastName = i.LastName,
                         BirthDate = i.BirthDate,
                         CurrentAddress = i.CurrentAddress,
                         Gender = i.GenderNavigation.Name,
                         BirthPlaceId = i.BirthPlaceTownId,
                         BirthPlace = i.BirthPlaceTown != null ? i.BirthPlaceTown.Name: i.BirthPlace,
                         BirthPlaceCountryId = i.BirthPlaceCountry,
                         PhoneNumber = i.Student.MobilePhone,
                         Email = i.Student.Email,
                         InternationalProtectionStatus = i.InternationalProtections.Any(),
                         GPName = i.Student.Gpname,
                         GPPhone = i.Student.Gpphone,
                         PermanentResidenceAddress = new AddressViewModel()
                         {

                         },
                         UsualResidenceAddress = new AddressViewModel()
                         {

                         },
                         Institutions = i.EducationalStates
                                     .Where(x => x.InstitutionId.HasValue)
                                     .Select(x => new InstitutionCacheModel()
                                     {
                                         Id = x.InstitutionId ?? 0,
                                         Name = x.Institution.Name
                                     }),
                         //Active = i.Active,
                         UsualResidenceId = i.CurrentTownId,
                         PermanentResidenceId = i.PermanentTownId,
                         NationalityId = i.NationalityId,
                         Nationality = i.Nationality.Name,
                         CommuterType = i.StudentClasses.FirstOrDefault(x => x.SchoolYear == currentYear).CommuterTypeId,
                         LivesWithFosterFamily = (bool?)i.Student.LivesWithFosterFamily ?? false,
                         HasParentConsent = (bool?)i.Student.HasParentConsent ?? false,
                         Sop = i.SpecialNeedsYears.Any(),
                         PersonId = i.PersonId,
                         NumberInClass = 333,
                         InternationalProtections = i.InternationalProtections
                            .Select(s => new InternationalProtectionModel
                            {
                                Id = s.Id,
                                ProtectionStatus = s.ProtectionStatus,
                                DocNumber = s.DocNumber,
                                DocDate = s.DocDate
                            })
                     }
                 ).FirstOrDefaultAsync();


            if (student == null)
            {
                _logger.LogWarning($"No student found with the provided id:{id}. Possible hack attempt!");
                throw new ArgumentException($"No student found with the provided id:{id}. Possible hack attempt!");
            }

            var studentClass = await _context.StudentClasses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PersonId == student.PersonId && x.IsCurrent && x.SchoolYear == currentYear);

            if (studentClass != null)
            {
                student.HasIndividualStudyPlan = studentClass.IsIndividualCurriculum;
                student.IsNotForSubmissionToNRA = !(studentClass.IsForSubmissionToNra ?? true);
                student.Repeater = studentClass.RepeaterId;
                student.HasSuportiveEnvironment = studentClass.HasSuportiveEnvironment;
                student.SupportiveEnvironment = studentClass.SupportiveEnvironment;
                student.HasStudentClassInCurrentYear = true;
            }
            else
            {
                student.HasStudentClassInCurrentYear = false;
            }

            student.Education = await _context.StudentClasses
                .Where(x => x.PersonId == student.PersonId)
                .Select(x => new EducationViewModel
                {
                    Id = x.Id,
                    BasicClassId = x.Class.BasicClassId,
                    EducationForm = x.Class.ClassEduForm.Name,
                    Grade = "-",
                    ReceptionAfter = "-",
                    RecordedInBookSubjectsPage = 1,
                    RecordedInBookSubjectsNumber = 4,
                    Specialty = x.Class.ClassSpeciality.Name,
                    TrainingType = x.Class.ClassType.Name,
                    SchoolYear = x.SchoolYear
                })
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            DropdownViewModel usualResidenceCity;
            if (student.UsualResidenceId.HasValue)
            {
                usualResidenceCity = await _context.Towns
                    .Where(c => c.TownId == student.UsualResidenceId)
                    .Include(c => c.Municipality)
                        .ThenInclude(m => m.Region)
                    .Select(a => new DropdownViewModel
                    {
                        Value = a.TownId,
                        Text = string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}"),
                        Name = string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}")
                    }).FirstAsync();
            }
            else
            {
                usualResidenceCity = new DropdownViewModel()
                {
                    Text = string.Empty,
                    Name = string.Empty
                };
            }

            DropdownViewModel permanentResidenceCity;
            if (student.PermanentResidenceId.HasValue)
            {
                permanentResidenceCity = await _context.Towns
                    .Where(c => c.TownId == student.PermanentResidenceId)
                    .Include(c => c.Municipality)
                        .ThenInclude(m => m.Region)
                    .Select(a => new DropdownViewModel
                    {
                        Value = a.TownId,
                        Text = string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}"),
                        Name = string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}")
                    }).FirstAsync();
            }
            else
            {
                permanentResidenceCity = new DropdownViewModel()
                {
                    Text = string.Empty,
                    Name = string.Empty
                };
            }

            student.PermanentResidenceAddress.DropdownModel = permanentResidenceCity;
            student.UsualResidenceAddress.DropdownModel = usualResidenceCity;

            if (student.BirthPlaceId.HasValue)
            {
                student.BirthPlace = await _context.Towns
                  .Where(c => c.TownId == student.BirthPlaceId)
                  .Include(c => c.Municipality)
                      .ThenInclude(m => m.Region).Select(a => string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}")).FirstAsync();
            }

            return student;
        }

        public void Remove(StudentCreateModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cant be null!");
            }

            _context.Remove(entity);
        }

        public void RemoveRange(IEnumerable<StudentCreateModel> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities cant be null!");
            }

            _context.RemoveRange(entities);
        }

        public async Task DeleteAzureAccount(int studentId)
        {
            if (!await _authorizationService.HasPermissionForStudent(studentId, DefaultPermissions.PermissionNameForAzureAccountManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            await _userManagementService.DeleteAsync(new StudentDeleteDisableRequestDto
            {
                PersonId = studentId,
                PositionId = (int)DeletionType.Temporary
            });
        }

        public async Task ArchiveAsync(int id)
        {
            var student = _context.Students.Single(x => x.PersonId == id);
            //student.Active = false;

            await SaveAsync();
        }

        public async Task UpdateInternationalProtection(StudentAdditionalDetailsModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentInternationalProtectionManage)
                || !await CanEditStudentPersonalDetails(model.PersonId))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (model.InternationalProtections == null)
            {
                throw new ArgumentNullException(nameof(model.InternationalProtections), Messages.EmptyModelError);
            }

            int personId = model.PersonId;

            Person person = await _context.People
                .Include(c => c.Student)
                .Include(x => x.InternationalProtections)
                .FirstOrDefaultAsync(x => x.PersonId == personId);

            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "Person cant be null!");
            }

            if (person.Student == null)
            {
                person.Student = new Student();
            }

            ICollection<InternationalProtection> filteredInternationalProtections = person.InternationalProtections;
            IEnumerable<InternationalProtectionModel> internationalProtectionModels = model.InternationalProtections;

            if (internationalProtectionModels.Count() == 0)
            {
                _context.InternationalProtections.RemoveRange(filteredInternationalProtections);
            }
            else
            {
                foreach (var internationalProtectionToRemove in filteredInternationalProtections)
                {
                    if (internationalProtectionModels.FirstOrDefault(i => i.Id == internationalProtectionToRemove.Id) == null)
                    {
                        _context.InternationalProtections.Remove(internationalProtectionToRemove);
                    }
                }

                foreach (var internationalProtectionUpdateModel in internationalProtectionModels)
                {
                    if (!internationalProtectionUpdateModel.Id.HasValue)
                    {
                        person.InternationalProtections.Add(new InternationalProtection
                        {
                            PersonId = personId,
                            ProtectionStatus = internationalProtectionUpdateModel.ProtectionStatus,
                            DocDate = internationalProtectionUpdateModel.DocDate,
                            DocNumber = internationalProtectionUpdateModel.DocNumber,
                            ValidFrom = internationalProtectionUpdateModel.ValidFrom,
                            ValidTo = internationalProtectionUpdateModel.ValidTo
                        });
                    }
                    else
                    {
                        InternationalProtection internationalProtectionToUpdate = filteredInternationalProtections.Single(i => i.Id == internationalProtectionUpdateModel.Id);
                        internationalProtectionToUpdate.ProtectionStatus = internationalProtectionUpdateModel.ProtectionStatus;
                        internationalProtectionToUpdate.DocDate = internationalProtectionUpdateModel.DocDate;
                        internationalProtectionToUpdate.DocNumber = internationalProtectionUpdateModel.DocNumber;
                        internationalProtectionToUpdate.ValidTo = internationalProtectionUpdateModel.ValidTo;
                        internationalProtectionToUpdate.ValidFrom = internationalProtectionUpdateModel.ValidFrom;
                    }
                }
            }

            await SaveAsync();
        }

        public async Task<IPagedList<StudentSearchViewModel>> GetBySearch(StudentSearchModelExtended studentSearchModel)
        {
            if (studentSearchModel == null)
            {
                throw new ArgumentNullException(nameof(studentSearchModel), Messages.EmptyModelError);
            }

            studentSearchModel.Pin = studentSearchModel.Pin?.Trim();
            studentSearchModel.FirstName = studentSearchModel.FirstName?.Trim();
            studentSearchModel.MiddleName = studentSearchModel.MiddleName?.Trim();
            studentSearchModel.LastName = studentSearchModel.LastName?.Trim();

            IQueryable<StudentSearchDto> query = GetStudentSearchQuery(studentSearchModel).AsNoTracking();
            int totalCount = await query.CountAsync();
            totalCount = Math.Min(totalCount, 100);

            List<StudentSearchViewModel> students = query != null
                 ? await query
                    .OrderBy(studentSearchModel.SortBy)
                    .PagedBy(studentSearchModel.PageIndex, Math.Min(studentSearchModel.PageSize, 100))
                    .Select(x => new StudentSearchViewModel
                    {
                        Id = x.PersonId,
                        PersonId = x.PersonId,
                        Pin = GlobalConstants.PIN_ANONYMIZATION_STRING, //_userInfo.UserRole != UserRoleEnum.School ? GlobalConstants.PIN_ANONYMIZATION_STRING : x.Pin,
                        FirstName = x.FirstName,
                        MiddleName = _userInfo.InstitutionID == x.InstitutionID ? x.MiddleName : (x.MiddleName != null ? x.MiddleName.Substring(0, Math.Min(1, x.MiddleName.Length)) + "." : ""),
                        LastName = _userInfo.InstitutionID == x.InstitutionID ? x.LastName : (x.LastName != null ? x.LastName.Substring(0, Math.Min(1, x.LastName.Length)) + "." : ""),
                        PublicEduNumber = x.PublicEduNumber,
                        Municipality = x.Municipality,
                        District = x.District,
                        School = x.School,
                        Age = x.Age ?? -1,
                        PositionID = x.PositionID,
                        InstitutionID = x.InstitutionID,
                        PinType = x.PinType,
                    })
                    .ToListAsync()
                    : new List<StudentSearchViewModel>();

            foreach (var student in students)
            {
                student.Uid = Guid.NewGuid().ToString();
                student.IsOwner = _userInfo.InstitutionID.HasValue && student.InstitutionID == _userInfo.InstitutionID.Value;
            }

            IPagedList<StudentSearchViewModel> result = students.ToPagedList(totalCount);

            return result;
        }

        public async Task<IPagedList<StudentSearchViewModel>> ListAsync(StudentListInput input, CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(StudentListInput), "Input model cant be null!");

            DbFunctions dbFunctions = null;
            IQueryable<StudentSearchViewModel> query = GetAllStudents()
                .AsNoTracking()
                .FilterByInstitution(_userInfo)
                .Select(x => new StudentSearchViewModel
                {
                    Id = x.PersonId,
                    Pin = _userInfo.UserRole != UserRoleEnum.School ? GlobalConstants.PIN_ANONYMIZATION_STRING : x.PersonalId,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Age = x.BirthDate != null ? dbFunctions.DateDiffYear(x.BirthDate.Value, DateTime.Now.Date) : -1
                })
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Pin.Contains(input.Filter)
                   || predicate.FirstName.Contains(input.Filter)
                   || predicate.MiddleName.Contains(input.Filter)
                   || predicate.LastName.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            IList<StudentSearchViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public Task<StudentSearchViewModel> CheckPinUniqueness(string pin)
        {
            return _context.VStudentLists
                .Where(x => x.Pin == pin)
                .Select(x => new StudentSearchViewModel
                {
                    FullName = x.FullName,
                    School = x.School
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<EducationViewModel>> GetStudentEducationAsync(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEducationRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.StudentClasses
                .AsNoTracking()
                .Where(x => x.PersonId == personId && x.IsCurrent) // Филтриране по IsCurrent
                .OrderByDescending(x => x.Id)
                .Select(x => new EducationViewModel
                {
                    Id = x.Id,
                    BasicClassId = x.BasicClassId,
                    ClassId = x.ClassId,
                    ClassName = x.Class.ClassName,
                    NumberInClass = x.ClassNumber,
                    PositionId = x.PositionId,
                    EducationForm = x.StudentEduForm.Name,
                    SchoolYear = x.SchoolYear,
                    Specialty = x.StudentSpeciality.Name,
                    Grade = "-",
                    ReceptionAfter = "-",
                    RecordedInBookSubjectsPage = 1,
                    RecordedInBookSubjectsNumber = 4,
                    TrainingType = x.Class.ClassType.Name,
                    School = x.InstitutionSchoolYear.Name,
                    IsOwn = _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID
                })
                .OrderByDescending(x => x.IsOwn)
                .ToListAsync();
        }

        public async Task<StudentAdditionalDetailsModel> GetStudentInternationalProtectionAsync(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentInternationalProtectionRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.People.AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new StudentAdditionalDetailsModel
                {
                    PersonId = x.PersonId,
                    HasInternationalProtectionStatus = x.InternationalProtections.Any(),
                    InternationalProtections = x.InternationalProtections
                            .Select(s => new InternationalProtectionModel
                            {
                                Id = s.Id,
                                ProtectionStatus = s.ProtectionStatus,
                                DocNumber = s.DocNumber,
                                DocDate = s.DocDate,
                                ValidFrom = s.ValidFrom,
                                ValidTo = s.ValidTo
                            })
                })
                .FirstOrDefaultAsync();
        }

        public Task<bool> CanManageStudent(int personId)
        {
            return GetStudentSearchQuery()
                .AsNoTracking()
                .AnyAsync(x => x.PersonId == personId);
        }

        public async Task<bool> IsStudentInCurrentInstitution(int personId)
        {
            return await IsStudentInInstitution(personId, _userInfo.InstitutionID ?? int.MinValue);
        }

        public async Task<bool> IsStudentInInstitution(int personId, int institutionId, bool isDischarge = false)
        {
            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(institutionId);
            if (institution == null)
            {
                return false;
            }

            IEnumerable<EduStateModel> eduStates = (await _eduStateCacheService.GetEduStatesForStudent(personId))
                .Where(x => x.InstitutionId == institution.Id);
            InstitutionTypeEnum institutionType = (InstitutionTypeEnum)institution.InstTypeId;

            bool isStudent;

            switch (institutionType)
            {
                case InstitutionTypeEnum.School:
                case InstitutionTypeEnum.KinderGarden:
                    isStudent = eduStates.Any(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds || (!isDischarge || x.PositionId == (int)PositionType.StudentPersDevelopmentSupport));
                    break;
                case InstitutionTypeEnum.PersonalDevelopmentSupportCenter:
                case InstitutionTypeEnum.SpecializedServiceUnit:
                    isStudent = eduStates.Any(x => x.PositionId == (int)PositionType.StudentPersDevelopmentSupport);
                    break;
                case InstitutionTypeEnum.CenterForSpecialEducationalSupport:
                    isStudent = eduStates.Any(x => x.PositionId == (int)PositionType.StudentOtherInstitution);
                    break;
                default:
                    isStudent = false;
                    break;
            }

            return isStudent;
        }

        public async Task<int> GetAttendedInstitution(int personId, int submittedInstitutionId)
        {
            var studentClasses = await _context.StudentClasses
               .Where(x => x.PersonId == personId && x.IsCurrent
                && x.ClassType.ClassKind == (int)ClassKindEnum.Basic
                && (
                    (x.PositionId == (int)PositionType.Student && (x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.School || x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.KinderGarden))
                    ||
                    (x.PositionId == (int)PositionType.StudentOtherInstitution
                        && x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport)))
               .Select(x => new
               {
                   x.PositionId,
                   x.InstitutionId,
                   x.EnrollmentDate
               })
               .ToListAsync();

            if (!studentClasses.Any())
            {
                return submittedInstitutionId;
            }

            if (studentClasses.All(x => x.PositionId == (int)PositionType.Student)
                || studentClasses.All(x => x.PositionId == (int)PositionType.StudentOtherInstitution))
            {
                return studentClasses.OrderByDescending(x => x.EnrollmentDate).FirstOrDefault().InstitutionId;
            }

            return studentClasses.OrderByDescending(x => x.PositionId).FirstOrDefault().InstitutionId;
        }

        public async Task<bool> CanEditStudentPersonalDetails(int personId)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                // Само институция може да редактира личните данни на детето
                return false;
            }

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID ?? default);
            if (institution == null || !institution.InstTypeId.HasValue)
            {
                return false;
            }

            List<EduStateModel> eduStates = await _eduStateCacheService.GetEduStatesForStudent(personId);

            bool canEdit;
            InstitutionTypeEnum institutionType = (InstitutionTypeEnum)institution.InstTypeId;
            switch (institutionType)
            {
                case InstitutionTypeEnum.School:
                case InstitutionTypeEnum.KinderGarden:
                case InstitutionTypeEnum.CenterForSpecialEducationalSupport:
                    canEdit = eduStates.Any(x => x.InstitutionId == institution.Id);
                    break;
                case InstitutionTypeEnum.PersonalDevelopmentSupportCenter:
                case InstitutionTypeEnum.SpecializedServiceUnit:
                default:
                    canEdit = false;
                    break;
            }

            return canEdit;
        }

        public async Task<string> GetCurrentClassName(int personId, int schoolYear)
        {
            var sc = await _context.StudentClasses.Where(x =>
                x.Class.ClassType.ClassKind == (int)ClassKindEnum.Basic &&
                (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds) &&
                x.InstitutionSchoolYear.SchoolYear == schoolYear &&
                x.PersonId == personId)
                .OrderByDescending(c => c.IsCurrent).ThenByDescending(c => c.EnrollmentDate)
                .Select(c => new
                {
                    c.Class.ClassName
                }).FirstOrDefaultAsync();

            return sc?.ClassName;

        }

        public async Task<StudentClassViewModel> GetCurrentClass(int personId, int schoolYear)
        {
            var sc = await _context.StudentClasses.Where(x =>
                x.Class.ClassType.ClassKind == (int)ClassKindEnum.Basic &&
                (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds) &&
                x.SchoolYear == schoolYear &&
                x.PersonId == personId)
                .OrderByDescending(c => c.IsCurrent).ThenByDescending(c => c.EnrollmentDate)
                .Select(i => new StudentClassViewModel()
                {
                    Id = i.Id,
                    ClassId = i.ClassId,
                    BasicClassId = i.BasicClassId,
                    BasicClassName = i.BasicClass.Name,
                    IsCurrent = i.IsCurrent
                }).FirstOrDefaultAsync();

            return sc;
        }

        public async Task<StudentClassViewModel> GetCurrentClass(int personId)
        {
            var sc = await _context.StudentClasses.Where(x =>
                x.Class.ClassType.ClassKind == (int)ClassKindEnum.Basic &&
                (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds) &&
                x.PersonId == personId)
                .OrderByDescending(c => c.IsCurrent).ThenByDescending(c => c.EnrollmentDate)
                .Select(i => new StudentClassViewModel()
                {
                    Id = i.Id,
                    ClassId = i.ClassId,
                    BasicClassId = i.BasicClassId,
                    BasicClassName = i.BasicClass.Name,
                    StudentEduFormId = i.StudentEduFormId,
                    StudentSpecialityId = i.StudentSpecialityId,
                    SelectedClassTypeId = i.ClassTypeId,
                    StudentProfessionId = i.StudentSpeciality.ProfessionId,
                    SchoolYear = i.SchoolYear,
                    InstitutionId = i.InstitutionId
                }).FirstOrDefaultAsync();

            return sc;

        }

        /// <summary>
        /// В картата с детайлите на ученик следва да се появи бутон за запис в учебна група/паралелка при определени условия.
        /// 1. Няма документи за записване с видими бутон за запис в група/паралелка.
        /// 2. В EduState е с позиция <> 2,9, а в StudentClass няма запис с IsCurrent = 1.
        /// 
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<InitialEnrollmentVisibilityCheckResultModel> InitialEnrollmentSecretBtnVisibilityCheck(int personId)
        {
            InitialEnrollmentVisibilityCheckResultModel result = new InitialEnrollmentVisibilityCheckResultModel()
            {
                IsVisible = false
            };

            if (!_userInfo.InstitutionID.HasValue
                || !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                return result;
            }

            IEnumerable<AdmissionDocumentViewModel> admissionDocs = await _admissionDocumentService.GetByPersonId(personId);
            if (admissionDocs.Any(x => x.CanBeEnrolled && x.Status == (int)DocumentStatus.Final))
            {
                return result;
            }

            var eduState = await _context.EducationalStates
                .Where(x => x.PersonId == personId && x.InstitutionId == _userInfo.InstitutionID
                    && x.PositionId != (int)PositionType.Staff && x.PositionId != (int)PositionType.Discharged)
                .Select(x => new
                {
                    x.EducationalStateId,
                    x.PositionId
                })
                .ToListAsync();

            if (eduState.Any() && !await _context.StudentClasses.AnyAsync(x => x.PersonId == personId && x.InstitutionId == _userInfo.InstitutionID && x.IsCurrent))
            {
                // В EduState е с позиция <> 2,9, а в StudentClass няма запис с IsCurrent = 1.
                result.IsVisible = true;
                result.EntrollmentPosition = eduState.OrderByDescending(x => x.EducationalStateId).Select(x => x.PositionId).FirstOrDefault();
            }

            return result;
        }

        public async Task<StudentClassViewModel> GetMainStudentClass(int personId, bool? isCurrent = true, int? schoolYear = null, int? institutionId = null)
        {
            return await MainStudentSlasses(personId, isCurrent, schoolYear, institutionId)
                .OrderByDescending(x => x.IsCurrent)
                .ThenBy(x => x.Position)
                .Select(x => new StudentClassViewModel()
                {
                    Id = x.Id,
                    ClassId = x.ClassId,

                    BasicClassId = x.BasicClassId,
                    BasicClassName = x.BasicClass.Name,
                    IsCurrent = x.IsCurrent,
                    IsNotPresentForm = x.IsNotPresentForm,
                    SchoolYear = x.SchoolYear,
                    InstitutionId = x.InstitutionId,
                    BasicClassRomeName = x.BasicClass.RomeName,
                    ClassGroupName = x.Class.ClassName,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<StudentClassViewModel>> GetMainStudentClasses(int personId, int? schoolYear = null, bool? includeIsNotPresentForm = null)
        {

            FormattableString queryString = $"select * from student.fn_StudentsMainClasses({personId}, {(schoolYear.HasValue ? schoolYear.ToString() : "")}, {(includeIsNotPresentForm ?? false).GetHashCode()})";

            IQueryable<StudentsMainClassesDto> query = _context.Set<StudentsMainClassesDto>()
                .FromSqlInterpolated(queryString);

            return await query
                .OrderByDescending(x => x.IsCurrent)
                .ThenBy(x => x.PositionId)
                .Select(x => new StudentClassViewModel()
                {
                    Id = x.Id,
                    ClassId = x.ClassId,
                    BasicClassId = x.BasicClassId,
                    BasicClassName = x.BasicClassName,
                    IsCurrent = x.IsCurrent,
                    IsNotPresentForm = x.IsNotPresentForm,
                    SchoolYear = x.SchoolYear,
                    InstitutionId = x.InstitutionId,
                    BasicClassRomeName = x.BasicClassRomeName,
                    ClassGroupName = x.ClassGroupName,
                    SchoolYearName = x.SchoolYearName,
                    InstitutionAbbreviation = x.InstitutionAbbreviation,
                    IsLodFinalized = x.IsLodFinalized
                })
                .ToListAsync();
        }

        public async Task<IPagedList<StudentEmployerDeatilsListModel>> EmployerDetailsList(DualFormEmployerListInput input, GridFilter[] filters, CancellationToken cancellationToken)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input), nameof(DualFormEmployerListInput));

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDualFormRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            IQueryable<VStudentDualFormDetail> query = _context.VStudentDualFormDetails
                .Where(x => new int[] { 11, 12 }.Contains(x.BasicClassId)
                    && x.StudentEduFormId == GlobalConstants.DualEduFormId && x.ClassTypeId == GlobalConstants.DualClassTypeId
                    && x.IsDeleted != true);

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            }

            if (_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.InstitutionRegionId == _userInfo.RegionID.Value);
            }

            if (_userInfo.LeadTeacherClasses?.Count > 0)
            {
                query = query.Where(x => _userInfo.LeadTeacherClasses.Contains(x.ClassId) || _userInfo.LeadTeacherClasses.Contains(x.ParentClassId ?? 0));
            }

            query = query.Filter(filters);

            IQueryable<StudentEmployerDeatilsListModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.PinTypeName.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.InstitutionCode.Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.InstitutionTown.Contains(input.Filter)
                   || predicate.InstitutionMunicipality.Contains(input.Filter)
                   || predicate.InstitutionRegion.Contains(input.Filter)
                   || predicate.BasicClassName.Contains(input.Filter)
                   || predicate.ClassName.Contains(input.Filter)
                   || predicate.CompanyUic.Contains(input.Filter)
                   || predicate.CompanyName.Contains(input.Filter)
                   || predicate.Profession.Contains(input.Filter))
                .Select(x => new StudentEmployerDeatilsListModel
                {
                    Id = x.StudentClassId,
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    Pin = x.Pin,
                    PinTypeName = x.PinTypeName,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearName,
                    InstitutionId = x.InstitutionId,
                    InstitutionCode = x.InstitutionCode,
                    InstitutionName = x.InstitutionName,
                    BasicClassId = x.BasicClassId,
                    BasicClassName = x.BasicClassName,
                    ClassName = x.ClassName,
                    EduFormName = x.EduFormName,
                    InstitutionTown = x.InstitutionTown,
                    InstitutionMunicipality = x.InstitutionMunicipality,
                    InstitutionRegion = x.InstitutionRegion,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CompanyUic = x.CompanyUic,
                    CompanyName = x.CompanyName,
                    Profession = x.Profession,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "InstitutionId asc, BasicClassId asc, FullName asc" : input.SortBy);


            int totalCount = await listQuery.CountAsync(cancellationToken);
            IList<StudentEmployerDeatilsListModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize)
                .ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }
    }
}
