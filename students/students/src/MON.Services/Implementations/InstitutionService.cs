namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Enums;
    using MON.Models.Institution;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Dynamic.Core;
    using MON.Shared.ErrorHandling;
    using MON.Models.Grid;
    using MON.Shared.Extensions;
    using MON.Models.Cache;
    using MON.Shared.Enums.AspIntegration;

    public class InstitutionService : BaseService<InstitutionService>, IInstitutionService
    {
        private const int STUDENT_POSITION_ID = 3;
        private readonly IMemoryCache _cache;
        private readonly ICacheService _cacheService;
        private readonly IAppConfigurationService _configurationService;
        private bool? _externalSoProviderLimitationsCheck = null;
        private HashSet<int> _externalSoProviderClassTypeEnrollmentLimitation = null;

        public InstitutionService(DbServiceDependencies<InstitutionService> dependencies,
            IMemoryCache cache,
            ICacheService cacheService,
            IAppConfigurationService configurationService)
            : base(dependencies)
        {
            _cache = cache;
            _configurationService = configurationService;
            _cacheService = cacheService;
        }


        #region Private members

        public HashSet<int> ExternalSoProviderClassTypeEnrollmentLimitation
        {
            get
            {
                if (_externalSoProviderClassTypeEnrollmentLimitation == null)
                {
                    string config = _configurationService.GetValueByKey("ExternalSoProviderClassTypeEnrollmentLimitation").Result;
                    if (config.IsNullOrWhiteSpace())
                    {
                        _externalSoProviderClassTypeEnrollmentLimitation = new HashSet<int>();
                    } else
                    {
                        _externalSoProviderClassTypeEnrollmentLimitation = JsonConvert.DeserializeObject<HashSet<int>>(config ?? "");
                    }
                }

                return _externalSoProviderClassTypeEnrollmentLimitation;
            }
        }

        public bool ExternalSoProviderLimitationsCheck
        {
            get
            {
                if (_externalSoProviderLimitationsCheck == null)
                {
                    string config = _configurationService.GetValueByKey("ExternalSoProviderLimitationsCheck").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _externalSoProviderLimitationsCheck = result;
                }

                return _externalSoProviderLimitationsCheck.Value;
            }
        }

        private async Task<short> CurrentSchoolYear()
        {
            return await _context.CurrentYears
                .Where(x => x.IsValid == true)
                .Select(x => x.CurrentYearId)
                .FirstOrDefaultAsync();
        }

        private async Task<short> CurrentSchoolYearForInstitution(int institutionId)
        {
            return await _context.InstitutionSchoolYears
                .Where(x => x.InstitutionId == institutionId && x.IsCurrent == true)
                .Select(x => x.SchoolYear)
                .FirstOrDefaultAsync();
        }

        private IQueryable<Institution> GetInstitutionsListQuery()
        {
            IQueryable<Institution> query = _context.Institutions;

            return _userInfo.SysRoleID switch
            {
                (int)UserRoleEnum.School => query.Where(i => i.InstitutionId == _userInfo.InstitutionID),
                (int)UserRoleEnum.Mon => query,
                (int)UserRoleEnum.MonExpert => query,
                (int)UserRoleEnum.ExternalExpert => query,
                (int)UserRoleEnum.MonOBGUM => query,
                (int)UserRoleEnum.MonOBGUM_Finance => query,
                (int)UserRoleEnum.MonHR => query,
                (int)UserRoleEnum.Consortium => query,
                (int)UserRoleEnum.CIOO => query,
                (int)UserRoleEnum.Ruo => query.Where(i => i.Town.Municipality.RegionId == _userInfo.RegionID),
                (int)UserRoleEnum.RuoExpert => query.Where(i => i.Town.Municipality.RegionId == _userInfo.RegionID),
                (int)UserRoleEnum.Teacher => query.Where(i => i.InstitutionId == _userInfo.InstitutionID && (_userInfo.IsLeadTeacher.HasValue && _userInfo.IsLeadTeacher.Value)),
                _ => query.Where(i => i.InstitutionId == int.MinValue),
            };
        }


        private IQueryable<ClassGroup> GetClassGroupListQuery()
        {
            IQueryable<ClassGroup> query = _context.ClassGroups;

            return _userInfo.SysRoleID switch
            {
                (int)UserRoleEnum.School => query.Where(i => i.InstitutionId == _userInfo.InstitutionID),
                (int)UserRoleEnum.Mon => query,
                (int)UserRoleEnum.MonExpert => query,
                (int)UserRoleEnum.ExternalExpert => query,
                (int)UserRoleEnum.MonOBGUM => query,
                (int)UserRoleEnum.MonOBGUM_Finance => query,
                (int)UserRoleEnum.MonHR => query,
                (int)UserRoleEnum.Consortium => query,
                (int)UserRoleEnum.CIOO => query,
                (int)UserRoleEnum.Ruo => query.Where(i => i.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID),
                (int)UserRoleEnum.RuoExpert => query.Where(i => i.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID),
                (int)UserRoleEnum.Teacher => query.Where(i => i.InstitutionId == _userInfo.InstitutionID
                && (_userInfo.IsLeadTeacher.HasValue && _userInfo.IsLeadTeacher.Value) && (_userInfo.LeadTeacherClasses != null
                && ((i.ParentClassId != null && _userInfo.LeadTeacherClasses.Contains(i.ParentClassId.Value))
                    || (i.ParentClassId == null && i.IsNotPresentForm.HasValue && i.IsNotPresentForm.Value && _userInfo.LeadTeacherClasses.Contains(i.ClassId))
                    )
                )),
                _ => query.Where(i => false),
            };
        }

        private IQueryable<VInstitutionList> FilterForUserRole(IQueryable<VInstitutionList> query)
        {
            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.School:
                case UserRoleEnum.Teacher:
                case UserRoleEnum.InstitutionAssociate:
                case UserRoleEnum.SchoolDirector:
                case UserRoleEnum.CPLRDirector:
                case UserRoleEnum.CSOPDirector:
                case UserRoleEnum.SOZDirector:
                    return query.Where(i => _userInfo.InstitutionID.HasValue && i.Id == _userInfo.InstitutionID);
                case UserRoleEnum.Consortium:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.Mon:
                case UserRoleEnum.MonExpert:
                case UserRoleEnum.MonOBGUM:
                case UserRoleEnum.MonOBGUM_Finance:
                case UserRoleEnum.MonHR:
                case UserRoleEnum.ExternalExpert:
                    return query;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    return query.Where(i => i.RegionId != null && i.RegionId == _userInfo.RegionID);
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                case UserRoleEnum.Accountant:
                case UserRoleEnum.KindergartenDirector:
                default:
                    return query.Where(i => i.Id == int.MinValue);
            }
        }

        #endregion

        /// <summary>
        /// Текуща учебна година за институцията
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        public async Task<short> GetCurrentYear(int? institutionId)
        {

            string key = institutionId.HasValue
                ? $"{CacheKeys.InstitutionCurrentSchoolYear}_{institutionId}"
                 : CacheKeys.CurrentSchoolYear;

            CurrentSchoolYearCacheModel currentSchoolYearCacheModel = await _cacheService.GetAsync<CurrentSchoolYearCacheModel>(key);
            if (currentSchoolYearCacheModel != null)
            {
                return currentSchoolYearCacheModel.Value;
            }

            short currentSchoolYear = institutionId.HasValue
                ? await CurrentSchoolYearForInstitution(institutionId.Value)
            : await CurrentSchoolYear();

            await _cacheService.SetAsync(key, new CurrentSchoolYearCacheModel
            {
                Value = currentSchoolYear
            });

            return currentSchoolYear;
        }

        public async Task<IPagedList<StudentInfoExternalModel>> ListInstitutionExternalDetails(InstitutionListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!input.InstitutionId.HasValue)
            {
                throw new ApiException(Messages.InvalidIdentificatorError);
            }


            // Оптимизация на заявката 2022-11-07 rnikolov
            // С цел да покажам всички StudentClass-ове, които не са в моята институция, първо филтрираме по x.InstitutionId != input.InstitutionId.
            // Това е бавно защото ще наптави  WHERE EXISTS(SELCT 1 FROM StudentClass WHERE IsCurrent = 1 AND InstitutionId == {моята институция}), но на много редове.
            // С цел оптимизация първо ще вземем PersonId-тата на учениците в моята институция и ще филтрираме по тях.
            // Да се внимава за броя в IN клаузата. Работи за около 30 000, а не следва да имаме толкова ученици в дадено училище.

            HashSet<int> personIds = (await _context.StudentClasses
                .Where(x => x.IsCurrent && x.InstitutionId == input.InstitutionId.Value)
                .Select(x => x.PersonId)
                .ToListAsync()
                ).ToHashSet();

            IQueryable<StudentClass> listQuery = _context.StudentClasses
                .Where(x => x.IsCurrent && x.InstitutionId != input.InstitutionId
                    && personIds.Contains(x.PersonId))
                .AsNoTracking();

            if (_userInfo.SysRoleID == (int)UserRoleEnum.Teacher)
            {
                listQuery = listQuery.Where(x => x.Class.ParentClassId.HasValue && _userInfo.LeadTeacherClasses.Contains(x.Class.ParentClassId.Value));
            }

            listQuery = listQuery.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Person.FirstName.Contains(input.Filter)
                    || predicate.Person.MiddleName.Contains(input.Filter)
                    || predicate.Person.LastName.Contains(input.Filter)
                    || predicate.Person.PersonalId.Contains(input.Filter)
                    || predicate.InstitutionId.ToString().Contains(input.Filter)
                    || predicate.InstitutionSchoolYear.Name.Contains(input.Filter)
                    || predicate.InstitutionSchoolYear.Town.Name.Contains(input.Filter)
                    || predicate.Class.ClassName.Contains(input.Filter)
                    || predicate.Position.Name.Contains(input.Filter));

            IQueryable<StudentInfoExternalModel> query = listQuery
                .Select(x => new StudentInfoExternalModel
                {
                    Id = x.Id,
                    FullName = x.Person.FirstName + " " + x.Person.MiddleName + " " + x.Person.LastName,
                    PersonId = x.PersonId,
                    Pin = x.Person.PersonalId,
                    PinType = x.Person.PersonalIdtypeNavigation.Name,
                    PositionName = x.Position.Name,
                    BasicClassId = x.BasicClassId,
                    ClassName = x.Class.ClassName,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    InstitutionTown = x.InstitutionSchoolYear.Town.Name
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) || input.SortBy == "Id desc" ? "FullName asc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<StudentInfoExternalModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);

        }

        public async Task<IPagedList<StudentInfoFullModel>> ListStudents(InstitutionListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForInstitution(input.InstitutionId ?? default, DefaultPermissions.PermissionNameForInstitutionStudentsRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VStudentClassDetail> listQuery = _context.VStudentClassDetails
                .AsNoTracking()
                .Where(x => x.IsCurrent && x.InstitutionId == input.InstitutionId.Value
                    && x.PositionId != 9 && x.PositionId != 2);

            if (_userInfo.SysRoleID == (int)UserRoleEnum.Teacher)
            {
                listQuery = listQuery.Where(x => x.ParentClassId.HasValue && _userInfo.LeadTeacherClasses.Contains(x.ParentClassId.Value));
            }

            if (!input.SelectedClasses.IsNullOrWhiteSpace())
            {
                string[] splitStr = input.SelectedClasses.Split("|", StringSplitOptions.RemoveEmptyEntries);
                HashSet<int> selectedIds = splitStr.ToHashSet<int>();
                if (selectedIds.Count > 0)
                {
                    listQuery = listQuery.Where(x => selectedIds.Contains(x.ClassId));
                }
            }

            listQuery = listQuery.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                    || predicate.Pin.Contains(input.Filter)
                    || predicate.PinType.Contains(input.Filter)
                    || predicate.Position.Contains(input.Filter)
                    || predicate.EduFormName.Contains(input.Filter)
                    || predicate.ClassTypeName.Contains(input.Filter)
                    || predicate.Profession.Contains(input.Filter)
                    || predicate.Speciality.Contains(input.Filter));

            IQueryable<StudentInfoFullModel> query = listQuery
                .Select(x => new StudentInfoFullModel
                {
                    FullName = x.FullName,
                    PersonId = x.PersonId,
                    Pin = x.Pin,
                    PinType = x.PinType,
                    Position = x.Position,
                    BasicClassId = x.BasicClassId,
                    BasicClassName = x.BasicClassRomeName,
                    ClassName = x.ClassName,
                    ClassTypeName = x.ClassTypeName,
                    Profession = x.Profession,
                    Speciality = x.Speciality,
                    EduFormName = x.EduFormName,
                    ParentBasicClassId = x.ParentBasicClassId,
                    ParentBasicClassName = x.ParentBasicClassName,
                    HasSpecialNeeds = x.HasSpecialNeeds,
                    ResourceSupportStatus = x.ResourceSupportStatus,
                    MainClassName = x.MainClassName,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) || input.SortBy == "Id desc" ? "ParentBasicClassId asc, ClassName asc, FullName asc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<StudentInfoFullModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<InstitutionDropdownViewModel> GetDropdownModelByIdAsync(int institutionId)
        {
            return await _context.Institutions.AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .Select(x => new InstitutionDropdownViewModel()
                {
                    Value = x.InstitutionId,
                    Text = x.Name,
                    Name = x.Name,
                    BaseSchoolTypeId = x.BaseSchoolTypeId,
                    Details = $"{x.InstitutionId}.{x.Name} гр./с.{x.Town.Name} общ.{x.Town.Municipality.Name} обл.{x.Town.Municipality.Region.Name}",
                })
                .SingleOrDefaultAsync();
        }

        public async Task<IPagedList<ClassGroupBaseModel>> GetClassGroupsForInstitutionAsync(ClassesListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input)));
            }

            if (!await _authorizationService.HasPermissionForInstitution(input.InstitutionId ?? 0, DefaultPermissions.PermissionNameForInstitutionClassesRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!input.SchoolYear.HasValue)
            {
                input.SchoolYear = await GetCurrentYear(input.InstitutionId);
            }

            IQueryable<ClassGroup> query = GetClassGroupListQuery().AsNoTracking()
                .Where(c => c.InstitutionId == input.InstitutionId && c.SchoolYear == input.SchoolYear
                    && c.IsClosed != true
                    && (c.IsNotPresentForm == true || c.ParentClassId != null)
                    //&& (c.IsValid != false || c.StudentClasses.Any(sc => sc.IsCurrent))); // Има случаи, при които групата не е валидна, но пък има ученици в нея. Това не е редно да се случва. Показваме ги за да имаме достъп до учениците.
                    && (((c.ParentClass.IsValid ?? true) && (c.IsValid ?? true)) || c.StudentClasses.Any(sc => sc.IsCurrent))); // Има случаи, при които групата не е валидна, но пък има ученици в нея. Това не е редно да се случва. Показваме ги за да имаме достъп до учениците.

            if (input.BasicClass.HasValue)
            {
                query = query.Where(x => x.BasicClassId == input.BasicClass.Value);
            }

            IQueryable<ClassGroupBaseModel> listQuery = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.ClassName.Contains(input.Filter)
                   || predicate.BasicClass.Name.Contains(input.Filter)
                   || predicate.BasicClass.Description.Contains(input.Filter)
                   || predicate.ClassType.Name.Contains(input.Filter)
                   || predicate.ClassEduForm.Name.Contains(input.Filter)
                   | predicate.ClassSpeciality.Name.Contains(input.Filter)
                   | predicate.ClassSpeciality.Profession.Name.Contains(input.Filter))
                .Select(c => new ClassGroupBaseModel()
                {
                    Id = c.ClassId,
                    ClassId = c.ClassId,
                    Name = c.ClassName,
                    ParentBasicClassId = c.ParentClass.BasicClassId,
                    ParentBasicClassName = c.ParentClass.BasicClass.RomeName,
                    BasicClassId = c.BasicClassId,
                    BasicClassName = c.BasicClass.Name,
                    BasicClassDescription = c.BasicClass.Description,
                    ClassTypeName = c.ClassType.Name,
                    EduFormName = c.ClassEduForm.Name,
                    Profession = c.ClassSpeciality.Profession.Name,
                    Speciality = c.ClassSpeciality.Name,
                    StudentCountPlaces = c.StudentCountPlaces,
                    Count = c.StudentClasses.Count(i => i.IsCurrent || i.Status == (int)MovementStatusEnum.Enrolled),
                    IsValid = c.ParentClass.IsValid ?? c.IsValid ?? true, // В базата има default(1)
                    IsClosed = c.IsClosed,
                    SchoolYear = c.SchoolYear,
                    IsBasicClass = c.ClassType.ClassKind == (int)ClassKindEnum.Basic,
                    ParentClassId = c.ParentClassId,
                    ParenClassName = c.ParentClass.ClassName

                })
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "BasicClassId asc, Name asc" : input.SortBy);

            int totalCount = listQuery.Count();
            IList<ClassGroupBaseModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        /// <summary>
        /// Модели на групи/паралелки за запис в неучебна паралелка.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="selectedValues"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<List<ClassGroupDropdownViewModel>> GetClassGroupsForAdditionalEnrollment(int personId, short schoolYear, string selectedValues)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<int?> allowedClassKind = new List<int?> { (int)ClassKindEnum.Cdo, (int)ClassKindEnum.Other, (int)ClassKindEnum.Professional };

            List<ClassGroupDropdownViewModel> items = await _context.ClassGroups
                .Where(x => x.InstitutionId == _userInfo.InstitutionID.Value && x.IsValid == true
                    && x.IsClosed != true
                    && x.SchoolYear == schoolYear && x.ParentClassId.HasValue
                    && allowedClassKind.Contains(x.ClassType.ClassKind))
                .OrderBy(x => x.BasicClassId).ThenBy(x => x.ClassName)
                .Select(c => new ClassGroupDropdownViewModel
                {
                    Name = $"{c.ClassName} - {c.BasicClass.RomeName} - {c.ClassEduForm.Name} - {c.ClassEduForm.Name} - {c.ClassType.Name} - /{c.StudentClasses.Where(i => i.IsCurrent).Count()} деца/",
                    Value = c.ClassId,
                    Text = $"{c.ClassName} - {c.BasicClass.RomeName} - {c.ClassEduForm.Name} - {c.ClassType.Name} - /{c.StudentClasses.Where(i => i.IsCurrent).Count()} деца/",
                    Disabled = c.StudentClasses.Any(sc => sc.PersonId == personId && sc.IsCurrent),
                    RelatedObjectId = c.ParentClassId.Value,
                    ClassTypeId = c.ClassTypeId,
                    ClassKindId = c.ClassType.ClassKind,
                    IsValid = c.IsValid ?? true
                })
                .ToListAsync();

            // Проверка за ограничения в ClassType на групите/паралелките при избран външен доставчик на СО
            if (ExternalSoProviderLimitationsCheck && await HasExternalSoProviderForLoggedInstitution())
            {
                HashSet<int> classTypeLimitation = ExternalSoProviderClassTypeEnrollmentLimitation;
                foreach (var item in items.Where(x => x.ClassTypeId.HasValue))
                {
                    item.HasExternalSoProviderLimitation = classTypeLimitation.Contains(item.ClassTypeId.Value);
                }
            }

            return items;
        }

        public async Task<List<InstitutionInfoModel>> GetInstitutionsAsync()
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await GetInstitutionsListQuery()
                .AsNoTracking()
                .Select(i => new InstitutionInfoModel()
                {
                    Id = i.InstitutionId,
                    Name = i.Name,
                    Region = i.Town.Municipality.Region.Name,
                    RegionId = i.Town.Municipality.RegionId ?? 0,
                    Municipality = i.Town.Municipality.Name,
                    MunicipalityId = i.Town.MunicipalityId,
                    Town = i.Town.Name,
                    TownId = i.TownId
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetInstitutionsDropdownModelAsync()
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await GetInstitutionsListQuery()
                .AsNoTracking()
                .Select(x => new DropdownViewModel()
                {
                    Value = x.InstitutionId,
                    Text = $"{x.InstitutionId}.{x.Name} гр./с.{x.Town.Name} общ.{x.Town.Municipality.Name} обл.{x.Town.Municipality.Region.Name}",
                    Name = $"{x.InstitutionId}.{x.Name} гр./с.{x.Town.Name} общ.{x.Town.Municipality.Name} обл.{x.Town.Municipality.Region.Name}",
                })
                .ToListAsync();
        }

        public async Task AutoNumberAsync(int classId)
        {
            if (!await _authorizationService.HasPermissionForClass(classId, DefaultPermissions.PermissionNameForClassManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var group = await (_context.StudentClasses
            .Where(c => c.ClassId == classId && c.IsCurrent)
               .Select(c => new
               {
                   c.Person.FirstName,
                   StudentClass = c
               })).ToListAsync();

            var orderedGroup = group.OrderBy(i => i.FirstName)
                .Select((c, index) => new
                {
                    index = index + 1,
                    c.FirstName,
                    c.StudentClass
                });

            foreach (var c in orderedGroup)
            {
                c.StudentClass.ClassNumber = c.index;
            }

            await SaveAsync();
        }

        public async Task<InstitutionCacheModel> GetByIdAsync(int? institutionId)
        {
            return await _context.Institutions.AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .Select(x => new InstitutionCacheModel
                {
                    Id = x.InstitutionId,
                    Name = x.Name
                })
                .SingleOrDefaultAsync();
        }

        public async Task<IPagedList<InstitutionDetailsModel>> List(InstitutionListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForInstitutionRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IQueryable<VInstitutionList> query = _context.VInstitutionLists.AsNoTracking();
            query = FilterForUserRole(query);

            if (input.RegionId.HasValue)
            {
                query = query.Where(x => x.RegionId == input.RegionId.Value);
            }

            if (input.InstTypeId.HasValue)
            {
                query = query.Where(x => x.InstTypeId == input.InstTypeId.Value);
            }

            string filter = input.Filter;
            if (!filter.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Id.ToString().Contains(filter)
                    || x.Name.Contains(filter)
                    || x.Abbreviation.Contains(filter)
                    || x.TownName.Contains(filter)
                    || x.MunicipalityName.Contains(filter)
                    || x.RegionName.Contains(filter)
                    || x.InstTypeName.Contains(filter)
                    || x.InstTypeAbbreviation.Contains(filter)
                    || x.BaseSchoolTypeName.Contains(filter)
                    || x.DetailedSchoolTypeName.Contains(filter)
                    || x.FinancialSchoolTypeName.Contains(filter)
                    || x.BudgetingSchoolTypeName.Contains(filter));
            }

            IQueryable<InstitutionDetailsModel> listQuery = query
                .Select(x => new InstitutionDetailsModel
                {
                    Id = x.Id,
                    Abbreviation = x.Abbreviation,
                    Town = x.TownName,
                    Municipality = x.MunicipalityName,
                    Region = x.RegionName,
                    InstTypeName = x.InstTypeName,
                    InstTypeAbbreviation = x.InstTypeAbbreviation,
                    BaseSchoolTypeName = x.BaseSchoolTypeName,
                    DetailedSchoolTypeName = x.DetailedSchoolTypeName,
                    FinancialSchoolTypeName = x.FinancialSchoolTypeName,
                    BudgetingSchoolTypeName = x.BudgetingSchoolTypeName

                })
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id asc" : input.SortBy);

            int totalCount = listQuery.Count();
            IList<InstitutionDetailsModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<InstitutionDetailsModel> GetFullDetails(int? id)
        {
            string currentSchoolYear = await _context.CurrentYears
                .Where(x => x.IsValid == true)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            var institution = await _context.Institutions
                .AsNoTracking()
                .Where(x => x.InstitutionId == id)
                .Select(x => new InstitutionDetailsModel
                {
                    Id = x.InstitutionId,
                    Name = x.Name,
                    Abbreviation = x.Abbreviation,
                    Bulstat = x.Bulstat,
                    SchoolYear = currentSchoolYear,
                    BaseSchoolType = x.BaseSchoolType.Name,
                    BaseSchoolTypeId = x.BaseSchoolTypeId,
                    DetailedSchoolType = x.DetailedSchoolType.Name,
                    FinancialSchoolType = x.FinancialSchoolType.Name,
                    BudgetingSchoolType = x.BudgetingSchoolType.Name,
                    Country = x.Country.Name,
                    Town = x.Town.Name,
                    LocalArea = x.LocalArea.Name,
                    District = x.Town.Municipality.Region.Name,
                    Municipality = x.Town.Municipality.Name,
                    Email = x.InstitutionDetail.Email,
                    Website = x.InstitutionDetail.Website
                })
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (institution == null)
            {
                return institution;
            }

            institution.Phones = await _context.InstitutionPhones
                .AsNoTracking()
                .Where(x => x.InstitutionId == institution.Id)
                .Select(x => new PhoneDetails
                {
                    Type = x.PhoneType.Name,
                    ContactKind = x.ContactKind,
                    Code = x.PhoneCode,
                    Number = x.PhoneNumber
                })
                .ToListAsync();

            institution.Departments = await _context.InstitutionDepartments
                .AsNoTracking()
                .Where(x => x.InstitutionId == institution.Id)
                .Select(x => new InstitutionDepartmentDetails
                {
                    Id = x.InstitutionDepartmentId,
                    Name = x.Name,
                    Country = x.Country.Name,
                    Town = x.Town.Name,
                    District = x.Town.Municipality.Region.Name,
                    Municipality = x.Town.Municipality.Name,
                    Address = x.Address,
                    Phones = x.InstitutionPhones.Select(p => new PhoneDetails
                    {
                        Type = p.PhoneType.Name,
                        ContactKind = p.ContactKind,
                        Code = p.PhoneCode,
                        Number = p.PhoneNumber
                    }).ToList()
                }).ToListAsync();

            institution.PublicCouncil = await _context.InstitutionPublicCouncils
                .AsNoTracking()
                .Where(x => x.InstitutionId == institution.Id)
                .Select(x => new InstitutionPublicCouncilDetails
                {
                    Id = x.InstitutionPublicCouncilId,
                    // Данните за обществените съвети ги скриваме, заради опасения относно личните данните
                    //Name = (x.FirstName ?? "") + " " + (x.MiddleName ?? "") + " " + (x.FamilyName ?? ""),
                    //Type = x.PublicCouncilType.Name,
                    //Role = x.PublicCouncilRole.Name,
                    //Email = x.Email,
                    //Phone = x.PhoneNumber
                })
                .ToListAsync();

            return institution;
        }

        public async Task<int?> GetCurrentForStudent(int? studentId)
        {
            return await _context.EducationalStates
                .Where(x => x.PersonId == studentId && x.PositionId == STUDENT_POSITION_ID)
                .Select(x => x.InstitutionId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CanManageInstitution(int institutionId)
        {
            return await GetInstitutionsListQuery()
                .AsNoTracking()
                .AnyAsync(x => x.InstitutionId == institutionId);
        }

        public async Task<bool> CanManageClass(int classId)
        {
            IQueryable<ClassGroup> query = _context.ClassGroups;

            query = _userInfo.SysRoleID switch
            {
                (int)UserRoleEnum.School => query.Where(i => i.InstitutionId == _userInfo.InstitutionID),
                //case (int)UserRoleEnum.Mon: query = query; break;
                (int)UserRoleEnum.Ruo => query.Where(i => i.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID),
                _ => query.Where(i => i.InstitutionId == int.MinValue),
            };
            return await query
                .AsNoTracking()
                .AnyAsync(x => x.ClassId == classId);
        }

        public async Task<int> GetInstitutionDetailedSchoolTypeId(int? institutionId)
        {
            return await _context.Institutions
                .Where(x => x.InstitutionId == institutionId)
                .Select(x => x.DetailedSchoolTypeId)
                .SingleOrDefaultAsync();
        }

        public async Task<InstitutionDropdownViewModel> GetLoggedUserInstitution()
        {
            int instId = _userInfo.InstitutionID ?? int.MinValue;

            return await _context.Institutions.AsNoTracking()
                .Where(x => x.InstitutionId == instId)
                .Select(x => new InstitutionDropdownViewModel
                {
                    Value = x.InstitutionId,
                    Text = $"{x.InstitutionId}.{x.Name} гр./с.{x.Town.Name} общ.{x.Town.Municipality.Name} обл.{x.Town.Municipality.Region.Name}",
                    Name = $"{x.InstitutionId}.{x.Name} гр./с.{x.Town.Name} общ.{x.Town.Municipality.Name} обл.{x.Town.Municipality.Region.Name}",
                    ClearName = x.Name,
                    BaseSchoolTypeId = x.BaseSchoolTypeId,
                    LocalArea = x.LocalArea.Name,
                    Municipality = x.Town.Municipality.Name,
                    Region = x.Town.Municipality.Region.Name,
                    Town = x.Town.Name
                })
                .SingleOrDefaultAsync();
        }

        public async Task<InstitutionCacheModel> GetInstitutionCache(int institutionId)
        {
            return await _cache.GetOrCreateAsync($"{CacheKeys.InstitutionCacheModels}_{institutionId}", entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Institutions
                    .Where(x => x.InstitutionId == institutionId)
                    .Select(x => new InstitutionCacheModel
                    {
                        Id = x.InstitutionId,
                        Name = x.Name,
                        InstTypeId = x.DetailedSchoolType.InstType,
                        RegionId = x.Town.Municipality.RegionId ?? default,
                        DetailedSchoolTypeId = x.DetailedSchoolTypeId,
                        BaseSchoolTypeId = x.BaseSchoolTypeId,
                        SchoolYear = x.InstitutionConfData.Select(x => x.SchoolYear).FirstOrDefault(),
                        SchoolYearName = x.InstitutionConfData.Select(x => x.SchoolYearNavigation.Name).FirstOrDefault()
                    })
                    .SingleOrDefaultAsync();
            });
        }

        public async Task<HashSet<int>> GetAllowedStudentPositions(int institutionId)
        {
            InstitutionCacheModel institutionBaseMode = await GetInstitutionCache(institutionId);

            if (institutionBaseMode == null || !institutionBaseMode.InstTypeId.HasValue) return new HashSet<int>();

            string instTypeToPositionLimitStr = await _configurationService.GetValueByKey("InstTypeToPositionLimit");
            Dictionary<int, HashSet<int>> instTypeToPositionLimit = JsonConvert.DeserializeObject<Dictionary<int, HashSet<int>>>(instTypeToPositionLimitStr ?? "");

            if (instTypeToPositionLimit == null || !instTypeToPositionLimit.ContainsKey(institutionBaseMode.InstTypeId ?? 0))
            {
                return new HashSet<int>();
            }

            return instTypeToPositionLimit[institutionBaseMode.InstTypeId ?? 0];
        }

        public async Task<InstTypeToClassTypeConfiguration> GetAllowedClasssKindsEnrollmentLimit(int institutionId)
        {
            InstitutionCacheModel institutionBaseMode = await GetInstitutionCache(institutionId);

            if (institutionBaseMode == null || !institutionBaseMode.InstTypeId.HasValue) return null;

            string instTypeToClassKindLimitStr = await _configurationService.GetValueByKey("InstTypeToClassKindEnrollmentLimit");
            Dictionary<int, InstTypeToClassTypeConfiguration> instTypeToClassKindLimit = JsonConvert.DeserializeObject<Dictionary<int, InstTypeToClassTypeConfiguration>>(instTypeToClassKindLimitStr ?? "");

            if (instTypeToClassKindLimit == null || !instTypeToClassKindLimit.ContainsKey(institutionBaseMode.InstTypeId ?? 0))
            {
                return null;
            }

            return instTypeToClassKindLimit[institutionBaseMode.InstTypeId ?? 0];
        }

        public async Task<bool> HasExternalSoProvider(int institutionId, int schoolYear)
        {
            return await _cache.GetOrCreateAsync($"{CacheKeys.InstitutionExtSoCheck}_{institutionId}_{schoolYear}", entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                if (ExternalSoProviderLimitationsCheck)
                {
                    return _context.InstitutionConfData.AnyAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.SoextProviderId != null);

                } else
                {
                    return Task.FromResult(false);
                }

            });
        }

        public async Task<bool> HasExternalSoProviderForLoggedInstitution()
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                return false;
            }

            int institutionCurrentSchoolYear = await GetCurrentYear(_userInfo.InstitutionID.Value);
            return await HasExternalSoProvider(_userInfo.InstitutionID.Value, institutionCurrentSchoolYear);
        }
    }
}
