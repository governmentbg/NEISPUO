using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Models.Enums;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Class;
using MON.Services.Infrastructure.Validators;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using MON.Shared.Extensions;
using System.Threading;
using System.Linq.Dynamic.Core;

namespace MON.Services.Implementations
{
    public class StudentClassService : MovementDocumentBaseService<StudentClassService>, IStudentClassService
    {
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly StudentClassValidationContext _validator;

        public StudentClassService(DbServiceDependencies<StudentClassService> dependencies,
            MovementDocumentServiceDependencies<StudentClassService> movementDocumentServiceDependencies,
            StudentClassValidationContext validator,
            ILodFinalizationService lodFinalizationService)
            : base(dependencies, movementDocumentServiceDependencies)
        {
            _validator = validator;
            _lodFinalizationService = lodFinalizationService;
        }

        #region Private members
        /// <summary>
        /// EntryDate се използва само в детските градини.
        /// В другите случай може да се появи грешка: Датата на постъпване не може да е преди датата на записване в група/паралелка.
        /// Дата на промяна на данни в StudentClass #1319
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task EntryDateFixture(StudentClassModel model)
        {
            if (model == null)
            {
                return;
            }

            if (model.EntryDate != model.EnrollmentDate)
            {
                var institution = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID ?? 0);
                if (institution != null && !institution.IsKinderGarden)
                {
                    model.EntryDate = model.EnrollmentDate;
                }
            }
        }
        private void ClearSupportiveEnvironment(StudentClass entity)
        {
            _context.AvailableArchitectures.RemoveRange(entity.AvailableArchitectures);
            _context.SpecialEquipments.RemoveRange(entity.SpecialEquipments);
            _context.BuildingRooms.RemoveRange(entity.BuildingRooms);
            _context.BuildingAreas.RemoveRange(entity.BuildingAreas);
        }

        private int GetMaxClassNumber(int classId)
        {
            // Да се махне автоматичното номериране на децата в клас #1048
            return -1;

            //return (await _context.StudentClasses
            //    .AsNoTracking()
            //    .Where(x => x.ClassId == classId)
            //    .Select(x => x.ClassNumber.Value)
            //    .ToListAsync())
            //    .DefaultIfEmpty()
            //    .Max();
        }

        /// <summary>
        /// Възможност за въвеждане на ЕИК на работодател при записване в паралелка #1258
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        private void ProcessDualFormCompanies(StudentClass entity, StudentClassModel model)
        {
            /*
                • Възможност за въвеждане на ЕИК на работодател при записване в паралелка на ученици от XI и XII клас във форма на професионално обучение чрез работа (дуална система на обучение);
                • Полето ще е достъпно само при записване в професионални паралелки (ClassType = 20), 11-ти и 12-ти клас, в обучение чрез работа;
                • Възможност за въвеждане на ЕИК на работодател за вече записани в 11-ти и 12-ти клас ученици в професионални паралелки, дуална система на обучение.
                • При въвеждане на ЕИК/БУЛТАТ на работодател ще се извършва проверка в Търговски регистър по отношение коректността на ЕИК/БУЛСТАТ – чрез средата за междурегистров обмен (RegiX). Получената от справката информация за наименование на дружеството ще се визуализира в потребителския интерфейс на НЕИСПУО.
                • При грешен ЕИК/БУЛСТАТ няма да се позволява запис на данните.
                • Възможност за промяна на работодател в процеса на обучение в 11-ти и 12-ти клас;
                • Списък на ученици в дуална система на обучение, наличен на Директорското табло в модул „Деца и ученици“, с информация за работодателя. С това институцията ще разполага с информация за ученици, за които не са въведени данни за работодателя;
                • Списъкът ще е достъпен и от таблата на потребители с роля РУО, РУО експерт, МОН, МОН експерт, ЦИОО и Консорциум.
             */
            if (entity == null || model == null)
            {
                return;
            }

            // • Възможност за въвеждане на ЕИК на работодател при записване в паралелка на ученици от XI и XII клас във форма на професионално обучение чрез работа (дуална система на обучение);
            int[] allowedBasicClasses = new int[] { 11, 12 };

            if (!allowedBasicClasses.Contains(entity.BasicClassId)
                 || entity.StudentEduFormId != GlobalConstants.DualEduFormId
                 || entity.ClassTypeId != GlobalConstants.DualClassTypeId)
            {
                // Всички съществуващи се изтриват
                foreach (var item in entity.StudentClassDualFormCompanies.Where(x => !x.IsDeleted))
                {
                    item.IsDeleted = true;
                }

                return;
            }

            var models = model.DualFormCompanies ?? Array.Empty<StudentClassDualFormCompanyModel>();

            var toDelete = entity.StudentClassDualFormCompanies
                .Where(x => !x.IsDeleted && !models.Any(m => x.Id == m.Id));
            foreach (var item in toDelete)
            {
                item.IsDeleted = true;
            }

            var toAdd = models.Where(x => !x.Id.HasValue);
            foreach (var company in toAdd)
            {
                entity.StudentClassDualFormCompanies.Add(new StudentClassDualFormCompany
                {
                    CompanyName = company.Name,
                    CompanyUic = company.Uic,
                    CompanyCountry = company.Country,
                    CompanyDistrict = company.District,
                    CompanyMunicipality = company.Municipality,
                    CompanySettlement = company.Settlement,
                    CompanyArea = company.Area,
                    CompanyAddress = company.Address,
                    CompanyPhone = company.Phone,
                    CompanyEmail = company.Email,
                    CompanyUrl = company.Url,
                    StartDate = company.StartDate,
                    EndDate = company.EndDate,
                    IsDeleted = false
                });
            }

            var toUpdate = entity.StudentClassDualFormCompanies
                .Where(x => !x.IsDeleted && models.Any(m => m.Id.HasValue && x.Id == m.Id));
            foreach (var company in toUpdate)
            {
                var source = models.FirstOrDefault(x => x.Id == company.Id);
                if (source == null)
                {
                    continue;
                }

                company.CompanyName = source.Name;
                company.CompanyUic = source.Uic;
                company.CompanyCountry = source.Country;
                company.CompanyDistrict = source.District;
                company.CompanyMunicipality = source.Municipality;
                company.CompanySettlement = source.Settlement;
                company.CompanyAddress = source.Address;
                company.CompanyPhone = source.Phone;
                company.CompanyEmail = source.Email;
                company.CompanyUrl = source.Url;
                company.StartDate = source.StartDate;
                company.EndDate = source.EndDate;
                company.IsDeleted = false;
            }
        }
        #endregion

        public async Task<StudentClassViewModel> GetById(int id)
        {
            var personId = await _context.StudentClasses
                .Where(x => x.Id == id)
                .Select(x => x.PersonId)
                .SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentClassRead)
                && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentClassUpdate))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            StudentClassViewModel model = await (
                from c in _context.StudentClasses.AsNoTracking()
                where c.Id == id
                select new StudentClassViewModel()
                {
                    Id = c.Id,
                    SchoolYear = c.SchoolYear,
                    BasicClassId = c.BasicClassId,
                    BasicClassName = c.BasicClass.Name,
                    SchoolYearName = c.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    ClassId = c.ClassId,
                    PersonId = c.PersonId,
                    StudentSpecialityId = c.StudentSpecialityId ?? (c.Class.ClassSpecialityId == -1 ? null : c.Class.ClassSpecialityId),
                    StudentSpeciality = c.StudentSpeciality.Name ?? c.Class.ClassSpeciality.Name,
                    StudentEduFormId = c.StudentEduFormId,
                    StudentEduFormName = c.StudentEduForm.Name ?? c.Class.ClassEduForm.Name,
                    StudentProfessionId = c.StudentSpeciality.ProfessionId,
                    StudentProfession = c.StudentSpeciality.Profession.Name ?? c.Class.ClassSpeciality.Profession.Name,
                    ClassNumber = c.ClassNumber,
                    Status = c.Status,
                    StatusName = ((StudentClassStatus)c.Status).ToString(),
                    HasIndividualStudyPlan = c.IsIndividualCurriculum,
                    HasSupportiveEnvironment = c.HasSuportiveEnvironment,
                    SupportiveEnvironment = c.SupportiveEnvironment,
                    IsNotForSubmissionToNra = !(c.IsForSubmissionToNra ?? true),
                    IsHourlyOrganization = c.IsHourlyOrganization,
                    RepeaterId = c.RepeaterId,
                    CommuterTypeId = c.CommuterTypeId ?? 0,
                    RepeaterReason = c.Repeater.Name,
                    CommuterTypeName = c.CommuterType.Name,
                    EnrollmentDate = c.EnrollmentDate,
                    EntryDate = c.EntryDate,
                    DischargeDate = c.DischargeDate,
                    PositionId = c.PositionId,
                    Position = c.Position.Name,
                    IsCurrent = c.IsCurrent,
                    OresTypeId = c.OrestypeId,
                    OresTypeName = c.Orestype.Name,
                    SelectedClassTypeId = c.ClassTypeId,
                    IsNotPresentForm = c.Class.IsNotPresentForm,
                    InstitutionId = c.InstitutionId,
                    ClassGroup = new ClassGroupViewModel
                    {
                        InstitutionId = c.Class.InstitutionId,
                        InstitutionName = c.Class.InstitutionSchoolYear.Name,
                        ClassEduFormName = c.Class.ClassEduForm.Name ?? c.StudentEduForm.Name,
                        ClassEduFormId = c.Class.ClassEduFormId,
                        ClassSpecialityId = c.Class.ClassSpecialityId == -1 ? null : c.Class.ClassSpecialityId,
                        ClassProfessionId = c.Class.ClassSpeciality.ProfessionId == -1 ? (int?)null : c.Class.ClassSpeciality.ProfessionId,
                        ClassName = c.Class.ClassName,
                        SchoolYear = c.Class.SchoolYear,
                        SchoolYearName = c.Class.InstitutionSchoolYear.SchoolYearNavigation.Name,
                        ClassSpecialityName = c.Class.ClassSpeciality.Name,
                        BasicClassId = (c.Class.IsNotPresentForm != null && c.Class.IsNotPresentForm.Value) ? c.BasicClassId : c.Class.BasicClassId,
                        BasicClassName = c.Class.BasicClass.Name,
                        ClassTypeId = c.Class.ClassTypeId,
                        ClassTypeName = c.Class.ClassType.Name ?? c.ClassType.Name,
                        IsNotPresentForm = c.Class.IsNotPresentForm,
                        StudentsInClass = c.Class.StudentClasses.Count(x => x.IsCurrent)
                    },
                    SpecialEquipment = c.SpecialEquipments.Select(x => x.EquipmentTypeId).ToList(),
                    AvailableArchitecture = c.AvailableArchitectures.Select(x => x.ModernizationDegreeId).ToList(),
                    BuildingAreas = c.BuildingAreas.Select(x => x.BuildingAreaTypeId).ToList(),
                    BuildingRooms = c.BuildingRooms.Select(x => x.BuildingRoomTypeId).ToList(),
                    DualFormCompanies = c.StudentClassDualFormCompanies.Where(x => !x.IsDeleted).Select(x => new StudentClassDualFormCompanyModel
                    {
                        Id = x.Id,
                        Uic = x.CompanyUic,
                        Name = x.CompanyName,
                        Email = x.CompanyEmail,
                        Phone = x.CompanyPhone,
                        Country = x.CompanyCountry,
                        District = x.CompanyDistrict,
                        Municipality = x.CompanyMunicipality,
                        Settlement = x.CompanySettlement,
                        Area = x.CompanyArea,
                        Address = x.CompanyAddress,
                        Url = x.CompanyUrl,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                    })
                    .ToArray(),
                })
                .SingleOrDefaultAsync();

            return model;
        }

        public async Task<List<StudentClassViewModel>> GetHistoryById(int id)
        {
            var classGroup = await _context.StudentClasses
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.InstitutionId
                })
                .SingleOrDefaultAsync();

            if (!await _authorizationService.HasPermissionForInstitution(classGroup?.InstitutionId ?? default, DefaultPermissions.PermissionNameForStudentClassHistoryRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await (
                from c in _context.StudentClassHistories.AsNoTracking()
                where c.StudentClassId == id && !c.IsDeleted
                orderby c.Id descending
                select new StudentClassViewModel()
                {
                    Id = c.Id,
                    SchoolYear = c.SchoolYear,
                    SchoolYearName = c.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    ClassId = c.ClassId,
                    PersonId = c.PersonId,
                    StudentSpecialityId = c.StudentSpecialityId,
                    StudentSpeciality = c.StudentSpeciality.Name ?? c.Class.ClassSpeciality.Name,
                    StudentEduFormId = c.StudentEduFormId,
                    StudentEduFormName = c.StudentEduForm.Name ?? c.Class.ClassEduForm.Name,
                    StudentProfessionId = c.StudentSpeciality.ProfessionId,
                    StudentProfession = c.StudentSpeciality.Profession.Name ?? c.Class.ClassSpeciality.Profession.Name,
                    ClassNumber = c.ClassNumber,
                    Status = c.Status,
                    StatusName = ((StudentClassStatus)c.Status).ToString(),
                    HasIndividualStudyPlan = c.IsIndividualCurriculum,
                    HasSupportiveEnvironment = c.HasSuportiveEnvironment,
                    SupportiveEnvironment = c.SupportiveEnvironment,
                    IsNotForSubmissionToNra = !(c.IsForSubmissionToNra ?? true),
                    IsHourlyOrganization = c.IsHourlyOrganization,
                    RepeaterId = c.RepeaterId,
                    CommuterTypeId = c.CommuterTypeId ?? 0,
                    RepeaterReason = c.Repeater.Name,
                    CommuterTypeName = c.CommuterType.Name,
                    EnrollmentDate = c.EnrollmentDate,
                    PositionId = c.PositionId,
                    Position = c.Position.Name,
                    IsCurrent = c.IsCurrent,
                    CreateDate = c.CreateDate,
                    CreatedBySysUser = c.CreatedBySysUser.Username,
                    ModifyDate = c.ModifyDate,
                    ModifiedBySysUser = c.ModifiedBySysUser.Username,
                    OresTypeId = c.OrestypeId,
                    OresTypeName = c.Orestype.Name,
                    ClassGroup = new ClassGroupViewModel()
                    {
                        InstitutionId = c.Class.InstitutionId,
                        InstitutionName = c.Class.InstitutionSchoolYear.Name,
                        ClassEduFormName = c.Class.ClassEduForm.Name,
                        ClassName = c.Class.ClassName,
                        SchoolYear = c.Class.SchoolYear,
                        ClassSpecialityName = c.Class.ClassSpeciality.Name,
                        BasicClassId = c.Class.BasicClassId ?? c.BasicClassId,
                        BasicClassName = c.Class.BasicClass.Name ?? c.BasicClass.Name,
                        ClassTypeName = c.Class.ClassType.Name ?? c.ClassType.Name
                    },
                })
                .ToListAsync();
        }

        public async Task<List<StudentClassViewModel>> GetByPersonId(StudentClassesTimelineInputModel input)
        {
            if (!await _authorizationService.HasPermissionForStudent(input?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentClassRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            InstitutionCacheModel currentInstitution = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID ?? default);
            bool isCurrentInstitutionCPLR = currentInstitution != null && currentInstitution.IsCPLR;

            IQueryable<VStudentClassTimeLineList> query = _context.VStudentClassTimeLineLists
                .Where(x => x.PersonId == input.PersonId && input.Positions.Contains(x.PositionId));

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.ClassKind.HasValue)
            {
                query = query.Where(x => x.StudentClassClassKindId == input.ClassKind || x.ClassGroupClassKindId == input.ClassKind);
            }

            List<StudentClassViewModel> studentClasses = await query
                .OrderByDescending(x => x.EnrollmentDate)
                .ThenByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.ValidTo)
                .ThenByDescending(x => x.Id)
                .Select(c => new StudentClassViewModel()
                {
                    Id = c.Id,
                    SchoolYear = c.SchoolYear,
                    SchoolYearName = c.SchoolYearName,
                    BasicClassId = c.BasicClassId,
                    BasicClassName = c.BasicClassName,
                    ClassId = c.ClassId,
                    PersonId = c.PersonId,
                    StudentSpecialityId = c.StudentSpecialityId,
                    StudentSpeciality = c.StudentClassSpecialityName ?? c.ClassGroupSpecialityName,
                    StudentEduFormId = c.StudentEduFormId,
                    StudentEduFormName = c.StudentClassEduFormName ?? c.ClassGroupEduFormName,
                    StudentProfessionId = c.StudentClassProfessionId,
                    StudentProfession = c.StudentClassProfessionName ?? c.ClassGroupProfessionName,
                    ClassNumber = c.ClassNumber,
                    Status = c.Status ?? 1,
                    StatusName = ((StudentClassStatus)c.Status).ToString(),
                    HasIndividualStudyPlan = c.IsIndividualCurriculum ?? false,
                    HasSupportiveEnvironment = c.HasSuportiveEnvironment ?? false,
                    SupportiveEnvironment = c.SupportiveEnvironment,
                    IsNotForSubmissionToNra = !(c.IsForSubmissionToNra ?? true),
                    IsHourlyOrganization = c.IsHourlyOrganization ?? false,
                    RepeaterId = c.RepeaterId,
                    CommuterTypeId = c.CommuterTypeId ?? 0,
                    OresTypeId = c.OrestypeId,
                    OresTypeName = c.OresTypeName,
                    RepeaterReason = c.RepeaterReasonName,
                    CommuterTypeName = c.CommuterTypeName,
                    EnrollmentDate = c.EnrollmentDate ?? DateTime.MinValue,
                    EntryDate = c.EntryDate,
                    DischargeDate = c.DischargeDate,
                    PositionId = c.PositionId,
                    Position = c.PositionName,
                    IsCurrent = c.IsCurrent,
                    HasHistory = c.HasHistory ?? false,
                    SelectedClassTypeId = c.ClassTypeId,
                    IsNotPresentForm = c.StudentClassIsNotPresentForm ?? false,
                    ClassGroup = new ClassGroupViewModel()
                    {
                        InstitutionId = c.InstitutionId,
                        InstitutionName = c.InstitutionAbbreviation,
                        ClassEduFormName = c.ClassGroupEduFormName ?? c.StudentClassEduFormName,
                        ClassEduFormId = c.ClassGroupEduFormId ?? c.StudentEduFormId,
                        ClassName = c.ClassName,
                        SchoolYear = c.SchoolYear,
                        ClassSpecialityName = c.ClassGroupSpecialityName ?? c.StudentClassSpecialityName,
                        ClassSpecialityId = c.ClassGroupSpecialityId == -1 ? null : c.ClassGroupSpecialityId,
                        ClassProfessionId = c.ClassGroupProfessionId == -1 ? (int?)null : c.ClassGroupProfessionId,
                        BasicClassId = c.BasicClassId,
                        BasicClassName = c.BasicClassName,
                        ClassTypeName = c.StudentClassClassTypeName ?? c.ClassGroupClassTypeName,
                        ClassTypeId = c.ClassGroupClassTypeId,
                        ClassKindId = c.ClassGroupClassKindId,
                        IsNotPresentForm = c.ClassGroupIsNotPresentForm ?? false
                    },
                    SpecialEquipmentsStr = c.SpecialEquipments,
                    AvailableArchitecturesStr = c.AvailableArchitectures,
                    BuildingAreasStr = c.BuildingAreas,
                    BuildingRoomsStr = c.BuildingRooms,
                    IsBasicClassForCurrentInstitution = c.InstitutionId == _userInfo.InstitutionID && (c.ClassGroupIsNotPresentForm == true || c.ClassGroupClassKindId == (int)ClassKindEnum.Basic),
                    IsAdditionalClass = (isCurrentInstitutionCPLR == true && (c.ClassGroupIsNotPresentForm == true || (c.ClassGroupClassKindId != null && c.ClassGroupClassKindId != (int)ClassKindEnum.Basic)))
                        || (isCurrentInstitutionCPLR == false && (c.ClassGroupIsNotPresentForm != true && c.ClassGroupClassKindId != (int)ClassKindEnum.Basic)),  // Класът е допълнителен ако classKindId != 1 (1 = общообразователна паралелка)
                                                                                                                                                                  // или isNotPresentForm === true (служебна паралелка).
                                                                                                                                                                  // Горното важи само за ЦПЛР.
                    CanAddCurriculum = c.ClassGroupIsNotPresentForm == true || (c.ClassGroupClassKindId != null && c.StudentClassIsNotPresentForm != true
                        && (c.ClassGroupClassKindId == (int)ClassKindEnum.Basic || c.ClassGroupClassKindId == (int)ClassKindEnum.Cdo)),
                    CanBeDeleted = c.InstitutionId == _userInfo.InstitutionID
                })
                .ToListAsync();

            foreach (StudentClassViewModel sc in studentClasses)
            {
                sc.SpecialEquipment = sc.SpecialEquipmentsStr.IsNullOrWhiteSpace()
                    ? Enumerable.Empty<int>()
                    : sc.SpecialEquipmentsStr.ToHashSet<int>('|');
                sc.AvailableArchitecture = sc.AvailableArchitecturesStr.IsNullOrWhiteSpace()
                    ? Enumerable.Empty<int>()
                    : sc.AvailableArchitecturesStr.ToHashSet<int>('|');
                sc.BuildingAreas = sc.BuildingAreasStr.IsNullOrWhiteSpace()
                    ? Enumerable.Empty<short>()
                    : sc.BuildingAreasStr.ToHashSet<short>('|');
                sc.BuildingRooms = sc.BuildingRoomsStr.IsNullOrWhiteSpace()
                    ? Enumerable.Empty<int>()
                    : sc.BuildingRoomsStr.ToHashSet<int>('|');

                if (sc.IsCurrent && sc.IsBasicClassForCurrentInstitution && sc.IsNotPresentForm != true)
                {
                    if (sc.PositionId == (int)PositionType.Student
                        && await _context.StudentClasses.AnyAsync(x => x.PersonId == sc.PersonId && x.SchoolYear == sc.SchoolYear
                            && x.IsCurrent && x.InstitutionId != sc.InstitutionId
                            && x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport
                            && x.ClassType.ClassKind == (int)ClassKindEnum.Basic))
                    {
                        sc.CanChangePosition = true;
                        sc.ChangeTargetPositionId = (int)PositionType.StudentSpecialNeeds;
                    }


                    if (sc.PositionId == (int)PositionType.StudentSpecialNeeds
                        && !await _context.StudentClasses.AnyAsync(x => x.PersonId == sc.PersonId && x.SchoolYear == sc.SchoolYear
                            && x.IsCurrent && x.InstitutionId != sc.InstitutionId
                            && x.InstitutionSchoolYear.DetailedSchoolType.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport
                            && x.ClassType.ClassKind == (int)ClassKindEnum.Basic))
                    {
                        sc.CanChangePosition = true;
                        sc.ChangeTargetPositionId = (int)PositionType.Student;
                    }
                }
            }

            if (_userInfo.InstitutionID.HasValue && _institutionService.ExternalSoProviderLimitationsCheck)
            {
                bool hasExternalSoProvider = await _institutionService.HasExternalSoProviderForLoggedInstitution();
                if (hasExternalSoProvider)
                {
                    HashSet<int> classTypeLimitation = _institutionService.ExternalSoProviderClassTypeEnrollmentLimitation;

                    foreach (var sc in studentClasses)
                    {
                        sc.HasExternalSoProvider = hasExternalSoProvider;
                        sc.HasExternalSoProviderClassTypeLimitation = sc.ClassGroup?.ClassTypeId != null && classTypeLimitation.Contains(sc.ClassGroup.ClassTypeId.Value);
                    }
                }
            }

            for (int i = 0; i < studentClasses.Count; i++)
            {
                if (i == 0) continue;
                var current = studentClasses[i];
                var prev = studentClasses[i - 1];
                if (current.Id == prev.Id && current.IsCurrent == true && current.IsCurrent == prev.IsCurrent)
                {
                    current.IsCurrent = false;
                    if (current.CanBeDeleted)
                    {
                        current.CanBeDeleted = false;
                    }
                }

            }

            return studentClasses;
        }

        public Task<List<StudentClassViewModel>> GetMainForPersonAndLoggedInstitution(int personId, short schoolYear)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
            return GetByPersonId(new StudentClassesTimelineInputModel
            {
                PersonId = personId,
                Positions = new HashSet<int?> { (int)PositionType.Student, (int)PositionType.StudentOtherInstitution, (int)PositionType.StudentSpecialNeeds },
                InstitutionId = _userInfo.InstitutionID.Value,
                SchoolYear = schoolYear,
                ClassKind = (int)ClassKindEnum.Basic,
            });
        }

        public async Task<List<ClassGroupDropdownViewModel>> GetDropdownOptionsForLoggedInstitution(int personId, short? schoolYear)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            HashSet<int> allowedPositions = new HashSet<int> { (int)PositionType.Student, (int)PositionType.StudentOtherInstitution, (int)PositionType.StudentSpecialNeeds };

            return await _context.StudentClasses
                .Where(x => x.PersonId == personId && x.InstitutionId == _userInfo.InstitutionID.Value && x.SchoolYear == schoolYear
                    && allowedPositions.Contains(x.PositionId)
                    && (x.Class.IsNotPresentForm == true || x.Class.ClassType.ClassKind == (int)ClassKindEnum.Basic))
                .Select(x => new ClassGroupDropdownViewModel
                {
                    Value = x.Id,
                    Text = x.Class.ClassName + " / " + x.BasicClass.Name,
                    BasicClassId = x.BasicClassId,
                    IsNotPresentForm = x.IsNotPresentForm,
                })
                .ToListAsync();
        }

        public async Task<List<PersonBasicStudentClassDetails>> GetPersonBasicClasses(int personId, bool? forCurrentInstitution)
        {
            return await _context.StudentClasses
                .Where(x => x.PersonId == personId && x.IsCurrent && x.ClassType.ClassKind == 1)
                .Select(x => new PersonBasicStudentClassDetails
                {
                    StudnetClassId = x.Id,
                    PersonId = x.PersonId,
                    BasicClassId = x.BasicClassId,
                    InstitutionId = x.InstitutionId,
                    Position = x.PositionId,
                })
                .ToListAsync();
        }


        #region Enrollment

        /// <summary>
        /// Записване в група/паралелка през документ за записване за институция от тип различен от ЦПЛР
        /// </summary>
        /// <param name="model"></param>
        /// <param name="withStudentEduForm"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ApiException"></exception>
        public async Task EnrollInClass(StudentClassModel model, bool withStudentEduForm)
        {
            // Проверяваме за PermissionNameForStudentToClassEntrollment. То трябва да идва от контекста ученик и от контекста клас
            // т.е. трябва да сме Owner на ученника и Ownew на класа.
            bool hasStudetPermission = await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment);
            bool hasClassPermission = await _authorizationService.HasPermissionForClass(model?.ClassId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment);
            if (!hasStudetPermission || !hasClassPermission)
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            await _validator.GetInitialEnrollmentTargetClassDetails(model);

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateEnrollment(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            StudentClass existingNotCurrentStrudenClass = await _context.StudentClasses
                .Include(x => x.Class)
                .Include(x => x.BuildingAreas)
                .Include(x => x.BuildingRooms)
                .Include(x => x.AvailableArchitectures)
                .Include(x => x.SpecialEquipments)
                .Include(x => x.StudentClassDualFormCompanies)
                .FirstOrDefaultAsync(x => x.PersonId == model.PersonId
                    && x.SchoolYear == model.SchoolYear
                    && x.ClassId == model.ClassId
                    && x.IsNotPresentForm != true
                    && !x.IsCurrent);

            await FixEnrollmentModelMissingAdmissionDocument(model, validationResult.TargetInstitutionId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                StudentClass studentClassToEnroll;
                if (existingNotCurrentStrudenClass != null)
                {
                    // Записваме го повторно в паралелка, в която вече е бил.
                    // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                    studentClassToEnroll = existingNotCurrentStrudenClass;
                    ClearSupportiveEnvironment(existingNotCurrentStrudenClass);
                    existingNotCurrentStrudenClass.UpdateFrom(model);
                    existingNotCurrentStrudenClass.IsCurrent = true;
                    existingNotCurrentStrudenClass.PositionId = validationResult.TargetPositionId;
                    existingNotCurrentStrudenClass.Status = (int)StudentClassStatus.Enrolled;
                    existingNotCurrentStrudenClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                    existingNotCurrentStrudenClass.AdmissionDocumentId = model.AdmissionDocumentId;
                    existingNotCurrentStrudenClass.RelocationDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeReasonId = null;
                    existingNotCurrentStrudenClass.DischargeDate = null;
                    existingNotCurrentStrudenClass.EntryDate = model.EntryDate;
                }
                else
                {
                    int maxClassNumber = GetMaxClassNumber(model.ClassId);
                    StudentClass studentClass = StudentClass.FromModel(model, validationResult, maxClassNumber + 1, withStudentEduForm);

                    var classGroup = await _context.ClassGroups.SingleAsync(x => x.ClassId == model.ClassId);

                    if (studentClass.StudentSpecialityId.Value == -1)
                    {
                        studentClass.StudentSpecialityId = classGroup.ClassSpecialityId;
                    }

                    if (studentClass.StudentEduFormId == -1)
                    {
                        studentClass.StudentEduFormId = classGroup.ClassEduFormId ?? -1;
                    }

                    _context.StudentClasses.Add(studentClass);
                    studentClassToEnroll = studentClass;
                }

                ProcessDualFormCompanies(studentClassToEnroll, model);
                await FixEduForm(studentClassToEnroll);
                await SaveInstitutionChange(model.SchoolYear, studentClassToEnroll.ClassId);
                await SaveAsync();

                await CreateCurriculumStudents(model.PersonId, model.SchoolYear, studentClassToEnroll.InstitutionId, studentClassToEnroll.Id);
                await UpdateAdmissionDocumentOnClassEnrollment(studentClassToEnroll.Id, model.AdmissionDocumentId);
                await transaction.CommitAsync();

                await _signalRNotificationService.StudentClassEduStateChange(model.PersonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        /// <summary>
        /// Записване в групи/паралелки през документ за записване за институция от тип ЦПЛР
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task EnrollInCplrClass(StudentClassModel model)
        {
            // Проверяваме за PermissionNameForStudentToClassEntrollment. То трябва да идва от контекста ученик и от контекста клас
            // т.е. трябва да сме Owner на ученника и Ownew на класа.
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (model.ClassGroups != null)
            {
                foreach (int classGroup in model.ClassGroups)
                {
                    if (!await _authorizationService.HasPermissionForClass(classGroup, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
                    {
                        throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
                    }
                }
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }
            await _validator.GetInitialCplrEnrollmentTargetClassDetails(model);

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateCprlEnrollment(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            // Съществуващи StudentClass за дадения ученик, учебна година и избрани от UI класове/групи.
            List<StudentClass> existingStudentClasses = await _context.StudentClasses
                .Include(x => x.Class)
                .Include(x => x.BuildingAreas)
                .Include(x => x.BuildingRooms)
                .Include(x => x.AvailableArchitectures)
                .Include(x => x.SpecialEquipments)
                .Where(x => x.PersonId == model.PersonId
                    && x.SchoolYear == model.SchoolYear
                    && model.ClassGroups.Contains(x.ClassId))
                .ToListAsync();

            // Съществуващи StudentClass за дадения ученик, учебна година и избрани от UI класове/групи,
            // които са текущи IsCurrent == true. Не е позволено записването е една и съща текуща паралелка за една учебна година.
            IEnumerable<StudentClass> existingCurrentClasses = existingStudentClasses.Where(x => x.IsCurrent);
            if (existingCurrentClasses.Any())
            {
                foreach (var existingClass in existingCurrentClasses)
                {
                    validationResult.Errors.Add($"Вече е записан в паралелка / група {existingClass.Class.ClassName} за учебната {existingClass.SchoolYear} година.");
                }

                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            await FixEnrollmentModelMissingAdmissionDocument(model, validationResult.TargetInstitutionId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Въртим всички избрани в UI-а групи/паралелки
                foreach (var classId in model.ClassGroups)
                {
                    model.ClassId = classId;

                    // Валидационен резултат за всяка една избрана група / паралелка.
                    EnrollmentTargetClass targetClass = validationResult.TargetClasses.FirstOrDefault(x => x.TargetClassId == classId);

                    StudentClass existingNotCurrentStrudenClass = existingStudentClasses.FirstOrDefault(x => x.ClassId == classId && x.IsNotPresentForm != true && !x.IsCurrent);
                    StudentClass studentClassToEnroll;

                    if (existingNotCurrentStrudenClass != null)
                    {
                        // Записваме го повторно в паралелка, в която вече е бил.
                        // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                        studentClassToEnroll = existingNotCurrentStrudenClass;
                        ClearSupportiveEnvironment(existingNotCurrentStrudenClass);
                        existingNotCurrentStrudenClass.UpdateFrom(model);
                        existingNotCurrentStrudenClass.IsCurrent = true;
                        existingNotCurrentStrudenClass.PositionId = validationResult.TargetPositionId;
                        existingNotCurrentStrudenClass.Status = (int)StudentClassStatus.Enrolled;
                        existingNotCurrentStrudenClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                        existingNotCurrentStrudenClass.AdmissionDocumentId = model.AdmissionDocumentId;
                        existingNotCurrentStrudenClass.RelocationDocumentId = null;
                        existingNotCurrentStrudenClass.DischargeDocumentId = null;
                        existingNotCurrentStrudenClass.DischargeReasonId = null;
                        existingNotCurrentStrudenClass.DischargeDate = null;
                    }
                    else
                    {
                        int maxClassNumber = GetMaxClassNumber(model.ClassId);
                        StudentClass studentClass = StudentClass.FromCplrModel(model, targetClass, maxClassNumber + 1);
                        _context.StudentClasses.Add(studentClass);
                        studentClassToEnroll = studentClass;
                    }

                    await SaveInstitutionChange(model.SchoolYear, studentClassToEnroll.ClassId);
                    await SaveAsync();
                    await CreateCurriculumStudents(model.PersonId, model.SchoolYear, studentClassToEnroll.InstitutionId, studentClassToEnroll.Id);
                    await UpdateAdmissionDocumentOnClassEnrollment(studentClassToEnroll.Id, model.AdmissionDocumentId);
                }

                await transaction.CommitAsync();

                await _signalRNotificationService.StudentClassEduStateChange(model.PersonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        public async Task EnrollInAdditionalClass(StudentClassBaseModel model)
        {
            // Проверяваме за PermissionNameForStudentToClassEntrollment. То трябва да идва от контекста ученик и от контекста клас
            // т.е. трябва да сме Owner на ученника и Ownew на класа.
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment)
                || !await _authorizationService.HasPermissionForClass(model?.ClassId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            await _validator.GetAdditionalEnrollmentTargetClassDetail(model);

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateAdditionalClassEnrollment(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            StudentClass existingNotCurrentStrudenClass = await _context.StudentClasses
               .FirstOrDefaultAsync(x => x.PersonId == model.PersonId
                    && x.SchoolYear == model.SchoolYear
                    && x.ClassId == model.ClassId
                    && x.IsNotPresentForm != true
                    && !x.IsCurrent);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (existingNotCurrentStrudenClass != null)
                {
                    // Записваме го повторно в паралелка, в която вече е бил.
                    // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                    existingNotCurrentStrudenClass.IsCurrent = true;
                    existingNotCurrentStrudenClass.PositionId = validationResult.TargetPositionId;
                    existingNotCurrentStrudenClass.Status = (int)StudentClassStatus.Enrolled;
                    existingNotCurrentStrudenClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                    existingNotCurrentStrudenClass.AdmissionDocumentId = null;
                    existingNotCurrentStrudenClass.RelocationDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeReasonId = null;
                    existingNotCurrentStrudenClass.DischargeDate = null;
                }
                else
                {
                    int maxClassNumber = GetMaxClassNumber(model.ClassId);
                    StudentClass studentClass = StudentClass.FromModel(model, validationResult, maxClassNumber + 1);
                    _context.StudentClasses.Add(studentClass);
                    existingNotCurrentStrudenClass = studentClass;
                }

                StudentClass studentClassToEnroll = existingNotCurrentStrudenClass;

                await SaveInstitutionChange(model.SchoolYear, studentClassToEnroll.ClassId);
                await SaveAsync();
                await CreateCurriculumStudents(model.PersonId, model.SchoolYear, studentClassToEnroll.InstitutionId, studentClassToEnroll.Id);

                await transaction.CommitAsync();

                await _signalRNotificationService.StudentClassEduStateChange(model.PersonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        public async Task EnrollInCplrAdditionalClass(StudentClassModel model)
        {
            // Проверяваме за PermissionNameForStudentToClassEntrollment. То трябва да идва от контекста ученик и от контекста клас
            // т.е. трябва да сме Owner на ученника и Ownew на класа.
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment)
                || !await _authorizationService.HasPermissionForClass(model?.ClassId ?? default, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new InvalidOperationException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            await _validator.GetCplrAdditionalEnrollmentTargetClassDetail(model);

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateCplrAdditionalClassEnrollment(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            StudentClass existingNotCurrentStrudenClass = await _context.StudentClasses
               .FirstOrDefaultAsync(x => x.PersonId == model.PersonId
                    && x.SchoolYear == model.SchoolYear
                    && x.ClassId == model.ClassId
                    && x.IsNotPresentForm != true
                    && !x.IsCurrent);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (existingNotCurrentStrudenClass != null)
                {
                    // Записваме го повторно в паралелка, в която вече е бил.
                    // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                    existingNotCurrentStrudenClass.IsCurrent = true;
                    existingNotCurrentStrudenClass.PositionId = validationResult.TargetPositionId;
                    existingNotCurrentStrudenClass.Status = (int)StudentClassStatus.Enrolled;
                    existingNotCurrentStrudenClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                    existingNotCurrentStrudenClass.AdmissionDocumentId = null;
                    existingNotCurrentStrudenClass.RelocationDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeReasonId = null;
                    existingNotCurrentStrudenClass.DischargeDate = null;
                }
                else
                {
                    int maxClassNumber = GetMaxClassNumber(model.ClassId);
                    StudentClass studentClass = StudentClass.FromCplrModel(model, validationResult.ToTargetClass(), maxClassNumber + 1);
                    _context.StudentClasses.Add(studentClass);
                    existingNotCurrentStrudenClass = studentClass;
                }

                StudentClass studentClassToEnroll = existingNotCurrentStrudenClass;

                await SaveInstitutionChange(model.SchoolYear, studentClassToEnroll.ClassId);
                await SaveAsync();
                await UpdateEduStateOnCplrEnrollment(model.PersonId, studentClassToEnroll.InstitutionId);
                await CreateCurriculumStudents(model.PersonId, model.SchoolYear, studentClassToEnroll.InstitutionId, studentClassToEnroll.Id);

                await transaction.CommitAsync();

                await _signalRNotificationService.StudentClassEduStateChange(model.PersonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        /// <summary>
        /// Запис в група/паралелка на много ученици. Използва се от ЦПЛР или от Училище за запис в общежитие.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task EnrollSelected(StudentClassMassEnrollmentModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError);
            }

            // Правото PermissionNameForStudentClassMassEnrolmentManage се дава на институция от тип ЦПЛР или ЦСОП
            // или Училище, Детска градина, но при запис в паралелка, чиито тип е описан в настройката MassEnrollmentAllowedClassTypes в базата.
            bool hasMassEntrollmentManagePermission = await _authorizationService.HasPermissionForClass(model.ClassId, DefaultPermissions.PermissionNameForStudentClassMassEnrolmentManage);

            if (!await _authorizationService.HasPermissionForClass(model.ClassId, DefaultPermissions.PermissionNameForClassManage)
                && !hasMassEntrollmentManagePermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.SelectedStudents.IsNullOrEmpty())
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model.SelectedStudents)));
            }

            var classGroup = await _context.ClassGroups
                .Where(x => x.ClassId == model.ClassId)
                .SingleOrDefaultAsync();

            if (classGroup == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(classGroup)));
            }

            short schoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

            List<StudentClass> existingStudentClasses = await _context.StudentClasses
                .Where(x => x.ClassId == model.ClassId
                    && x.SchoolYear == schoolYear
                    && x.IsNotPresentForm != null
                    && model.SelectedStudents.Contains(x.PersonId))
                .ToListAsync();

            ValidationErrorCollection validationErrors = new ValidationErrorCollection();
            await _validator.GetCplrAdditionalEnrollmentTargetClassDetail(new StudentClassBaseModel
            {
                ClassId = model.ClassId
            });

            InstitutionCacheModel instData = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);

            List<StudentClass> studentClassesToEnroll = new List<StudentClass>();
            foreach (int personId in model.SelectedStudents)
            {
                StudentClass existingStudentClass = existingStudentClasses.Where(x => x.PersonId == personId).FirstOrDefault();

                if (existingStudentClass != null && existingStudentClass.IsCurrent)
                {
                    // Вече е записван в тази паралелка.
                    continue;
                }

                StudentClassBaseModel validationModel = new StudentClassBaseModel
                {
                    PersonId = personId,
                    SchoolYear = schoolYear,
                    ClassId = model.ClassId,
                    SelectedClassTypeId = classGroup.ClassTypeId,
                    EnrollmentDate = model.EnrollmentDate,
                };

                StudentClassEnrollmentValidationResult validationResult = instData.IsCPLR
                    ?  await _validator.ValidateCplrAdditionalClassEnrollment(validationModel)
                    :  await _validator.ValidateAdditionalClassEnrollment(validationModel);

                if (validationResult.HasErrors)
                {
                    foreach (ValidationError error in validationResult.Errors)
                    {
                        validationErrors.Add(error.Message, "", $"{personId}");
                    }

                    continue;
                }

                StudentClass existingNotCurrentStrudenClass = existingStudentClass != null && !existingStudentClass.IsCurrent
                    ? existingStudentClass
                    : null;

                if (existingNotCurrentStrudenClass != null)
                {
                    // Записваме го повторно в паралелка, в която вече е бил.
                    // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                    existingNotCurrentStrudenClass.IsCurrent = true;
                    existingNotCurrentStrudenClass.PositionId = validationResult.TargetPositionId;
                    existingNotCurrentStrudenClass.Status = (int)StudentClassStatus.Enrolled;
                    existingNotCurrentStrudenClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                    existingNotCurrentStrudenClass.AdmissionDocumentId = null;
                    existingNotCurrentStrudenClass.RelocationDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeReasonId = null;
                    existingNotCurrentStrudenClass.DischargeDate = null;
                }
                else
                {
                    int maxClassNumber = GetMaxClassNumber(model.ClassId);
                    StudentClass studentClass = StudentClass.FromModel(validationModel, validationResult, maxClassNumber + 1);
                    _context.StudentClasses.Add(studentClass);
                    existingNotCurrentStrudenClass = studentClass;
                }

                studentClassesToEnroll.Add(existingNotCurrentStrudenClass);
            }

            if (validationErrors.Count > 0)
            {
                throw new ApiException(Messages.ValidationError, 500, validationErrors);
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (studentClassesToEnroll.Any())
                {
                    await SaveInstitutionChange(schoolYear, model.ClassId);

                }

                await SaveAsync();

                List<EducationalState> eduStatesToAdd = new List<EducationalState>();
                foreach (StudentClass studentClass in studentClassesToEnroll)
                {
                    await CreateCurriculumStudents(studentClass.PersonId, studentClass.SchoolYear, studentClass.InstitutionId, studentClass.Id);
                   
                    if (instData.IsCPLR)
                    {
                        await UpdateEduStateOnCplrEnrollment(studentClass.PersonId, studentClass.InstitutionId);
                    }
                    else
                    {
                        if (studentClass.PositionId == (int)PositionType.ProfessionalEducation)
                        {
                            await UpdateEduStateOnAdditionalEnrollment(studentClass.PersonId, studentClass.InstitutionId, PositionType.ProfessionalEducation);
                        } else
                        {
                            await UpdateEduStateOnAdditionalEnrollment(studentClass.PersonId, studentClass.InstitutionId);
                        }
                    }
                }

                await transaction.CommitAsync();

                foreach (int personId in model.SelectedStudents)
                {
                    await _signalRNotificationService.StudentClassEduStateChange(personId);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }
        #endregion


        #region Unenrollment
        /// <summary>
        /// Отписване от допълнителна(неосновна) група/паралелка.
        /// Отписването от основна група/паралелка става с от институцията чрез документ за отписване или преместване.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ApiException"></exception>
        public async Task UnenrollFromClass(StudentClassUnenrollmentModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(StudentClassUnenrollmentModel)));
            }

            StudentClass studentClass = await _context.StudentClasses
                .Include(x => x.Class)
                .SingleOrDefaultAsync(x => x.Id == model.ClassId);

            if (studentClass == null)
            {
                throw new ArgumentNullException(nameof(StudentClass));
            }

            // Проверяваме за PermissionNameForStudentToClassEntrollment. То трябва да идва от контекста ученик и от контекста клас
            // т.е. трябва да сме Owner на ученника и Ownew на класа.
            if (!await _authorizationService.HasPermissionForStudent(studentClass.PersonId, DefaultPermissions.PermissionNameForStudentToClassEnrollment)
                || !await _authorizationService.HasPermissionForClass(studentClass.ClassId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (await _lodFinalizationService.IsLodFInalized(studentClass.PersonId, studentClass.SchoolYear))
            {
                throw new InvalidOperationException(Messages.LodIsFinalizedError(studentClass?.SchoolYear));
            }

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateUnenrollment(studentClass, model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                studentClass.IsCurrent = false;
                studentClass.Status = (int)StudentClassStatus.Transferred;
                studentClass.DischargeDate = model.DischargeDate.Date;
                await SaveAsync();

                await DeleteCurriculumStudents(studentClass.PersonId, studentClass.SchoolYear, studentClass.Id, studentClass.InstitutionId);

                // За институция от тип ЦПЛР при отспиване от последната група/паралелка следва
                // да отпишем детето и от институцията.
                InstitutionCacheModel instData = await _institutionService.GetInstitutionCache(studentClass.InstitutionId);
                if (instData.IsCPLR && !await _context.StudentClasses.AnyAsync(x => x.PersonId == studentClass.PersonId
                                && x.InstitutionId == studentClass.InstitutionId
                                && x.IsCurrent && x.Id != studentClass.Id))
                {
                    await UpdateEduStateOnDischarge(studentClass.PersonId, studentClass.InstitutionId);
                }

                await SaveInstitutionChange(studentClass.SchoolYear, studentClass.ClassId);
                await SaveAsync();

                await transaction.CommitAsync();

                await _signalRNotificationService.StudentClassEduStateChange(studentClass.PersonId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message, ex);
            }
        }

        /// <summary>
        /// Масово отписване от допълнителна(неосновна) група/паралелка.
        /// Използва се от ЦПЛР-тата или от училище при отписване от общежитие.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task UnenrollSelected(StudentClassMassUnenrollmentModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            // Правото PermissionNameForStudentClassMassEnrolmentManage се дава на институция от тип ЦПЛР или ЦСОП
            // или Училище, Детска градина, но при запис в паралелка, чиито тип е описан в настройката MassEnrollmentAllowedClassTypes в базата.
            bool hasMassEntrollmentManagePermission = await _authorizationService.HasPermissionForClass(model.ClassId, DefaultPermissions.PermissionNameForStudentClassMassEnrolmentManage);

            if (!await _authorizationService.HasPermissionForClass(model.ClassId, DefaultPermissions.PermissionNameForClassManage)
                && !hasMassEntrollmentManagePermission)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!model.SelectedStudents.IsNullOrEmpty())
            {
                List<StudentClass> studentClasses = await _context.StudentClasses
                    .Include(x => x.Person)
                    .Where(x => x.ClassId == model.ClassId && model.SelectedStudents.Contains(x.PersonId)
                        && x.IsCurrent)
                    .ToListAsync();

                if (studentClasses.Count == 0)
                {
                    return;
                }

                InstitutionCacheModel instData = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);

                ValidationErrorCollection validationErrors = new ValidationErrorCollection();

                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    foreach (var studentClass in studentClasses)
                    {
                        StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateUnenrollment(studentClass, model);

                        if (validationResult.HasErrors)
                        {
                            foreach (ValidationError error in validationResult.Errors)
                            {
                                validationErrors.Add(error.Message, "", $"{studentClass.Person.FirstName} {studentClass.Person.MiddleName} {studentClass.Person.LastName}");
                            }
                        }
                        else
                        {
                            studentClass.IsCurrent = false;
                            studentClass.Status = (int)StudentClassStatus.Transferred;
                            studentClass.DischargeDate = model.DischargeDate.Date;

                            // При отписване от общежитие от институция от тип училище не изтриваме учебен план, при записване не го създаваме.
                            if (instData.IsCPLR || hasMassEntrollmentManagePermission)
                            {
                                await DeleteCurriculumStudents(studentClass.PersonId, studentClass.SchoolYear, studentClass.Id, studentClass.InstitutionId);
                            }

                            if (!await _context.StudentClasses.AnyAsync(x => x.PersonId == studentClass.PersonId
                                && x.InstitutionId == studentClass.InstitutionId
                                && x.IsCurrent && x.Id != studentClass.Id))
                            {
                                await UpdateEduStateOnDischarge(studentClass.PersonId, studentClass.InstitutionId);
                            }
                        }
                    }

                    if (validationErrors.Count > 0)
                    {
                        throw new ApiException(Messages.ValidationError, 500, validationErrors);
                    }

                    await SaveInstitutionChange(studentClasses.First().SchoolYear, studentClasses.First().ClassId);
                    await SaveAsync();

                    await transaction.CommitAsync();

                    foreach (var studentClass in studentClasses)
                    {
                        await _signalRNotificationService.StudentClassEduStateChange(studentClass.PersonId);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new ApiException(ex.GetInnerMostException().Message, ex);
                }
            }
        }
        #endregion

        public async Task<StudentClassSummaryModel> GetCurrentClassSummaryByIdAsync(int studentClassId)
        {
            StudentClassSummaryModel studentClassSummary = await _context.StudentClasses
                .Where(c => c.Id == studentClassId)
                .Select(c => new StudentClassSummaryModel
                {
                    SchoolYear = c.SchoolYear,
                    ClassSpecialityName = c.Class.ClassSpeciality.Name,
                    ClassEduFormName = c.Class.ClassEduForm.Name,
                    ClassName = c.Class.ClassName,
                    BasicClassId = c.Class.BasicClassId ?? 0,

                }).FirstOrDefaultAsync();

            return studentClassSummary;
        }

        public async Task<int> ChangeStudentClass(StudentClassModel model, bool withStudentEduForm)
        {
            // Текуща паралелка, от която преместваме ученика
            StudentClass oldStudentClass = await _context.StudentClasses
                .Include(x => x.Class)
                .SingleOrDefaultAsync(x => x.Id == model.CurrentStudentClassId);

            if (!await _authorizationService.HasPermissionForStudent(oldStudentClass.PersonId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            await EntryDateFixture(model);

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateChange(model, oldStudentClass);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            List<StudentClass> existingStudentClasses = await _context.StudentClasses
               .Include(x => x.StudentClassDualFormCompanies)
               .Where(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear
                    && x.ClassId == model.ClassId)
               .ToListAsync();

            if (validationResult.TargetClassIsNotPresentForm ?? false)
            {
                // Местим го в паралелка с IsNotPresentForm == true;
                StudentClass firstValid = existingStudentClasses?.FirstOrDefault(x => x.IsCurrent && x.BasicClassId == model.BasicClassId);
                if (firstValid != null)
                {
                    string msg = "За промяна на данните използвайте бутон Редакция. От бутон Преместване е задължително да се промени Випускът(Класът).";
                    ValidationErrorCollection errors = new ValidationErrorCollection
                    {
                        Messages.ExistingStudentClass(firstValid.Id, firstValid.SchoolYear, firstValid.PersonId, firstValid.ClassId)
                    };

                    throw new ApiException(msg, 400, errors);
                }
            }
            else
            {
                StudentClass firstValid = existingStudentClasses?.FirstOrDefault(x => x.IsCurrent);
                if (firstValid != null)
                {
                    string msg = "Не е позволено местенето в същата паралалка/група. За промяна на данните използвайте бутон Редакция.";
                    ValidationErrorCollection errors = new ValidationErrorCollection
                    {
                        Messages.ExistingStudentClass(firstValid.Id, firstValid.SchoolYear, firstValid.PersonId, firstValid.ClassId)
                    };

                    throw new ApiException(msg, 400, errors);
                }
            }

            StudentClass existingNotCurrentStudentClass = existingStudentClasses?.FirstOrDefault(x => !x.IsCurrent && x.IsNotPresentForm != true);

            await FixEnrollmentModelMissingAdmissionDocument(model, validationResult.TargetInstitutionId);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                StudentClass studentClassToEnroll;

                if (existingNotCurrentStudentClass != null)
                {
                    // Записваме го повторно в паралелка, в която вече е бил.
                    // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                    existingNotCurrentStudentClass.IsCurrent = true;
                    existingNotCurrentStudentClass.PositionId = validationResult.TargetPositionId;
                    existingNotCurrentStudentClass.Status = (int)StudentClassStatus.Enrolled;
                    existingNotCurrentStudentClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                    existingNotCurrentStudentClass.EntryDate = model.EntryDate;
                    existingNotCurrentStudentClass.AdmissionDocumentId = model.AdmissionDocumentId;
                    existingNotCurrentStudentClass.RelocationDocumentId = null;
                    existingNotCurrentStudentClass.DischargeDocumentId = null;
                    existingNotCurrentStudentClass.DischargeReasonId = null;
                    existingNotCurrentStudentClass.StudentSpecialityId = model.StudentSpecialityId;
                    existingNotCurrentStudentClass.StudentEduFormId = model.StudentEduFormId ?? -1;
                    existingNotCurrentStudentClass.DischargeDate = null;

                    studentClassToEnroll = existingNotCurrentStudentClass;
                }
                else
                {
                    int maxClassNumber = GetMaxClassNumber(model.ClassId);
                    studentClassToEnroll = StudentClass.FromModel(model, validationResult, maxClassNumber + 1, withStudentEduForm);
                    _context.StudentClasses.Add(studentClassToEnroll);
                }

                oldStudentClass.IsCurrent = false; // Текущата паралелка не е вече текуща
                oldStudentClass.Status = (int)StudentClassStatus.Transferred;
                oldStudentClass.DischargeDate = studentClassToEnroll.EnrollmentDate;

                // Данните за работодателите в стария клас се изтриват;
                await _context.StudentClassDualFormCompanies
                    .Where(x => x.StudentClassId == oldStudentClass.Id && !x.IsDeleted)
                    .UpdateAsync(x => new StudentClassDualFormCompany { IsDeleted = true });

                ProcessDualFormCompanies(studentClassToEnroll, model);
                await FixEduForm(studentClassToEnroll);
                await SaveAsync();

                await DeleteCurriculumStudents(oldStudentClass.PersonId, oldStudentClass.SchoolYear, oldStudentClass.Id, oldStudentClass.InstitutionId);
                await CreateCurriculumStudents(studentClassToEnroll.PersonId, studentClassToEnroll.SchoolYear, studentClassToEnroll.InstitutionId, studentClassToEnroll.Id);

                //Запазваме промените по институцията
                await SaveInstitutionChange(model.SchoolYear, studentClassToEnroll.ClassId);
                await SaveAsync();

                await UpdateAdmissionDocumentOnClassEnrollment(studentClassToEnroll.Id, model.AdmissionDocumentId);

                await transaction.CommitAsync();

                await _signalRNotificationService.StudentClassEduStateChange(model.PersonId);

                return studentClassToEnroll.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        /// <summary>
        /// Позволено е де са променя паралелката/групата. 
        /// Целта е да не се прави отписване от група и записване в нова,
        /// защото остава следа в timelina-а.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ApiException"></exception>
        public async Task UpdateAdditionalClass(StudentAdditionalClassChangeModel model)
        {
            // Текуща допълнителна паралелка
            StudentClass studentClass = await _context.StudentClasses
                .Include(x => x.Class).ThenInclude(x => x.ClassType)
                .SingleOrDefaultAsync(x => x.Id == model.Id) ?? throw new ApiException(Messages.EmptyEntityError);

            if (!await _authorizationService.HasPermissionForStudent(studentClass.PersonId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            if (studentClass.ClassId == model.ClassId && studentClass.EnrollmentDate.Date == (model.EnrollmentDate ?? DateTime.MinValue).Date)
            {
                // Няма какво да променяме.
                return;
            }

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateUpdate(model, studentClass);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (studentClass.ClassId != model.ClassId)
                {
                    studentClass.ClassId = model.ClassId;
                }
                if (model.EnrollmentDate.HasValue && studentClass.EnrollmentDate.Date != model.EnrollmentDate.Value.Date)
                {
                    studentClass.EnrollmentDate = model.EnrollmentDate.Value.Date;
                }
                await SaveAsync();

                await DeleteCurriculumStudents(studentClass.PersonId, studentClass.SchoolYear, studentClass.Id, studentClass.InstitutionId);
                await SaveAsync();

                await CreateCurriculumStudents(studentClass.PersonId, studentClass.SchoolYear, studentClass.InstitutionId, studentClass.Id);
                await SaveInstitutionChange(studentClass.SchoolYear, studentClass.ClassId);
                await SaveAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        public async Task<int> ChangeAdditionalClass(StudentAdditionalClassChangeModel model)
        {
            // Текуща допълнителна паралелка
            StudentClass oldStudentCLass = await _context.StudentClasses
                .Include(x => x.Class)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(oldStudentCLass.PersonId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            StudentClassEnrollmentValidationResult validationResult = await _validator.ValidateChange(model, oldStudentCLass);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            List<StudentClass> existingStudentClasses = await _context.StudentClasses
                .Where(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear
                    && x.ClassId == model.ClassId)
                .ToListAsync();

            StudentClass firstValid = existingStudentClasses?.FirstOrDefault(x => x.IsCurrent);
            if (firstValid != null)
            {
                throw new ApiException(Messages.ExistingStudentClass(firstValid.Id, firstValid.SchoolYear, firstValid.PersonId, firstValid.ClassId));
            }

            StudentClass existingNotCurrentStrudenClass = existingStudentClasses?.FirstOrDefault(x => !x.IsCurrent);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                StudentClass studentClassToEnroll;

                if (existingNotCurrentStrudenClass != null)
                {
                    // Записваме го повторно в паралелка, в която вече е бил.
                    // Няма да създаваме нов запис, а ще променим стария на IsCurrent = 1.
                    existingNotCurrentStrudenClass.IsCurrent = true;
                    existingNotCurrentStrudenClass.PositionId = validationResult.TargetPositionId;
                    existingNotCurrentStrudenClass.Status = (int)StudentClassStatus.Enrolled;
                    existingNotCurrentStrudenClass.EnrollmentDate = model.EnrollmentDate ?? DateTime.Now.Date;
                    existingNotCurrentStrudenClass.AdmissionDocumentId = null;
                    existingNotCurrentStrudenClass.RelocationDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeDocumentId = null;
                    existingNotCurrentStrudenClass.DischargeReasonId = null;
                    existingNotCurrentStrudenClass.DischargeDate = null;

                    studentClassToEnroll = existingNotCurrentStrudenClass;
                }
                else
                {
                    int maxClassNumber = GetMaxClassNumber(model.CurrentStudentClassId);
                    studentClassToEnroll = StudentClass.FromModel(model, validationResult, maxClassNumber + 1);
                    studentClassToEnroll.FromStudentClassId = model.CurrentStudentClassId;
                    _context.StudentClasses.Add(studentClassToEnroll);
                }

                oldStudentCLass.IsCurrent = false; // Текущата паралелка не е вече текуща
                oldStudentCLass.Status = (int)StudentClassStatus.Transferred;
                oldStudentCLass.DischargeDate = studentClassToEnroll.EnrollmentDate;
                await SaveAsync();

                await DeleteCurriculumStudents(oldStudentCLass.PersonId, oldStudentCLass.SchoolYear, oldStudentCLass.Id, oldStudentCLass.InstitutionId);

                //Запазваме промените по институцията 
                await SaveInstitutionChange(model.SchoolYear, oldStudentCLass.ClassId);

                await SaveAsync();
                await CreateCurriculumStudents(studentClassToEnroll.PersonId, studentClassToEnroll.SchoolYear, studentClassToEnroll.InstitutionId, studentClassToEnroll.Id);
                await transaction.CommitAsync();

                return studentClassToEnroll.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        public async Task Update(StudentClassModel model)
        {
            // Текуща паралелка, от която преместваме ученика
            StudentClass studentClass = await _context.StudentClasses
                .Include(x => x.Class).ThenInclude(x => x.ClassType)
                .Include(x => x.BuildingAreas)
                .Include(x => x.BuildingRooms)
                .Include(x => x.AvailableArchitectures)
                .Include(x => x.SpecialEquipments)
                .Include(x => x.StudentClassDualFormCompanies)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (studentClass == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForInstitution(studentClass.InstitutionId, DefaultPermissions.PermissionNameForStudentClassUpdate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, model.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(model?.SchoolYear));
            }

            await EntryDateFixture(model);

            ApiValidationResult validationResult = await _validator.ValidateUpdate(model, studentClass);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            ClearSupportiveEnvironment(studentClass);
            ProcessDualFormCompanies(studentClass, model);
            studentClass.UpdateFrom(model);

            try
            {
                //Запазваме промените по институцията
                await SaveInstitutionChange(model.SchoolYear, studentClass.ClassId);
                await SaveAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.GetInnerMostException().Message, e);
            }
        }


        public async Task ChangePosition(StudentPositionChangeModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            // Текуща паралелка, от която преместваме ученика
            StudentClass studentClass = await _context.StudentClasses
                .FirstOrDefaultAsync(x => x.Id == model.StudentClassId) ?? throw new ApiException(Messages.EmptyEntityError);

            if (!await _authorizationService.HasPermissionForInstitution(studentClass.InstitutionId, DefaultPermissions.PermissionNameForStudentClassUpdate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _lodFinalizationService.IsLodFInalized(studentClass.PersonId, studentClass.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(studentClass.SchoolYear));
            }

            ApiValidationResult validationResult = await _validator.ValidatePositionChange(model, studentClass);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            EducationalState eduState = await _context.EducationalStates
                .Where(x => x.PersonId == studentClass.PersonId && x.InstitutionId == studentClass.InstitutionId && x.PositionId == studentClass.PositionId)
                .FirstOrDefaultAsync();
            if (eduState != null)
            {
                eduState.PositionId = model.PositionId;
            }

            studentClass.PositionId = model.PositionId;

            await SaveAsync();
            await _eduStateCacheService?.ClearEduStatesForStudent(studentClass.PersonId);
        }

        public async Task DeleteHistoryRecord(int id)
        {
            var classGroup = await _context.StudentClassHistories
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.StudentClass.Class.InstitutionId,
                    x.SchoolYear,
                    x.PersonId
                })
                .SingleOrDefaultAsync();

            if (!await _authorizationService.HasPermissionForInstitution(classGroup?.InstitutionId ?? default, DefaultPermissions.PermissionNameForStudentClassHistoryDelete))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _lodFinalizationService.IsLodFInalized(classGroup.PersonId, classGroup.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(classGroup?.SchoolYear));
            }

            StudentClassHistory entity = await _context.StudentClassHistories.Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (entity.IsDeleted)
            {
                throw new ApiException(Messages.AlreadyDeleted);
            }

            entity.IsDeleted = true;
            await SaveAsync();
        }

        public async Task Delete(int id)
        {
            StudentClass studentClass = await _context.StudentClasses.SingleOrDefaultAsync(x => x.Id == id);

            if (studentClass == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(studentClass.PersonId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _lodFinalizationService.IsLodFInalized(studentClass.PersonId, studentClass.SchoolYear))
            {
                throw new ApiException(Messages.LodIsFinalizedError(studentClass?.SchoolYear));
            }

            if (!_userInfo.InstitutionID.HasValue || studentClass.InstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                SqlParameter in1 = new SqlParameter
                {
                    ParameterName = "StudentClassId",
                    DbType = System.Data.DbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = studentClass.Id
                };

                SqlParameter out1 = new SqlParameter
                {
                    ParameterName = "Error",
                    DbType = System.Data.DbType.String,
                    Direction = System.Data.ParameterDirection.Output,
                    Size = -1
                };

                var sql = "exec student.admin_sp_DeleteStudentClass @StudentClassId, @Error OUT";
                int result = await _context.Database.ExecuteSqlRawAsync(sql, in1, out1);

                string spErrors = out1.Value.ConvertFromDBVal<string>();
                if (!spErrors.IsNullOrWhiteSpace())
                {
                    throw new InvalidOperationException(spErrors);
                }

                // За институция от тип ЦПЛР при изтриване на последната група/паралелка следва
                // да отпишем детето и от институцията.
                InstitutionCacheModel instData = await _institutionService.GetInstitutionCache(studentClass.InstitutionId);
                if (instData.IsCPLR && !await _context.StudentClasses.AnyAsync(x => x.PersonId == studentClass.PersonId
                                && x.InstitutionId == studentClass.InstitutionId
                                && x.IsCurrent && x.Id != studentClass.Id))
                {
                    await UpdateEduStateOnDischarge(studentClass.PersonId, studentClass.InstitutionId);
                }

                await SaveAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }

        public async Task<bool> AddToNewClassBtnVisibilityCheck(int personId)
        {
            ApiValidationResult validationResult = await _validator.AddToNewClassBtnVisibilityCheck(personId);
            if (validationResult.HasErrors)
            {
                _logger.LogInformation(new ApiException(Messages.ValidationError, 500, validationResult.Errors), Messages.ValidationError);
            }

            return validationResult.HasErrors;
        }

        public async Task<StudentClassDualFormCompanyModel[]> GetDualFormCompanies(int studentClassId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDualFormRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var personId = await _context.StudentClasses
                .Where(x => x.Id == studentClassId)
                .Select(x => x.PersonId)
                .SingleOrDefaultAsync(cancellationToken);

            // Методът се използва при Details и Edit
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentClassRead)
                && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentClassUpdate))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.StudentClassDualFormCompanies
                .Where(x => x.StudentClassId == studentClassId && !x.IsDeleted)
                .Select(x => new StudentClassDualFormCompanyModel
                {
                    Id = x.Id,
                    Uic = x.CompanyUic,
                    Name = x.CompanyName,
                    Email = x.CompanyEmail,
                    Phone = x.CompanyPhone,
                    Country = x.CompanyCountry,
                    District = x.CompanyDistrict,
                    Municipality = x.CompanyMunicipality,
                    Settlement = x.CompanySettlement,
                    Area = x.CompanyArea,
                    Address = x.CompanyAddress,
                    Url = x.CompanyUrl,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                })
                .ToArrayAsync(cancellationToken);
        }
    }
}
