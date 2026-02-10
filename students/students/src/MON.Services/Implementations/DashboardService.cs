namespace MON.Services.Implementations
{
    using DocumentFormat.OpenXml.Bibliography;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.Configuration;
    using MON.Models.Dashboards;
    using MON.Models.Enums;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Services.Infrastructure.Validators;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading;
    using System.Threading.Tasks;

    public class DashboardService : BaseService<DashboardService>, IDashboardService
    {
        private readonly IInstitutionService _institutionService;
        private readonly IStudentClassService _studentClassService;
        private readonly IAbsenceCampaignService _absenceCampaignService;
        private readonly IAspService _aspService;
        private readonly StudentClassValidationContext _studentClassValidator;

        public DashboardService(DbServiceDependencies<DashboardService> dependencies,
            IInstitutionService institution,
            IStudentClassService studentClassService,
            IAbsenceCampaignService absenceCampaignService,
            IAspService aspService,
            StudentClassValidationContext studentClassValidator)
            : base(dependencies)
        {
            _institutionService = institution;
            _studentClassService = studentClassService;
            _studentClassValidator = studentClassValidator;
            _absenceCampaignService = absenceCampaignService;
            _aspService = aspService;
        }

        #region Private members
        private IQueryable<StudentClass> GetStudentClasses()
        {
            IQueryable<StudentClass> listQuery = _context.StudentClasses
                .Where(x => x.IsCurrent);

            if (_userInfo.IsMon || _userInfo.IsMonExpert || _userInfo.IsConsortium || _userInfo.IsCIOO)
            {
                // Всички данни
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                listQuery = listQuery.Where(x => _userInfo.RegionID.HasValue
                    && x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID.Value);
            }
            else if (_userInfo.IsSchoolDirector || _userInfo.IsInstitutionAssociate)
            {
                listQuery = listQuery.Where(x => _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID.Value);
            }
            else if (_userInfo.IsTeacher)
            {
                listQuery = listQuery.Where(x => _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID.Value);
                // Todo: Филтър само на класовете на класния
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return listQuery;
        }
        #endregion

        public async Task<IPagedList<StudentForAdmissionModel>> GetStudentsForAdmission(PagedListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(PagedListInput)));
            }

            IQueryable<VStudentsToEnroll> query = _context.VStudentsToEnrolls
                .Where(x => x.RowNum == 1);

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.Consortium:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.Mon:
                case UserRoleEnum.MonExpert:
                case UserRoleEnum.MonOBGUM:
                case UserRoleEnum.MonOBGUM_Finance:
                case UserRoleEnum.MonHR:
                    // Вижда всичко
                    break;
                case UserRoleEnum.School:
                case UserRoleEnum.InstitutionAssociate:
                case UserRoleEnum.SchoolDirector:
                case UserRoleEnum.KindergartenDirector:
                case UserRoleEnum.CPLRDirector:
                case UserRoleEnum.CSOPDirector:
                case UserRoleEnum.SOZDirector:
                    if (!_userInfo.InstitutionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    if (!_userInfo.RegionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.RegionId == _userInfo.RegionID.Value);

                    break;
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Teacher:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                case UserRoleEnum.ExternalExpert:
                case UserRoleEnum.Accountant:
                default:
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<StudentForAdmissionModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.RegionName.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "FullName asc" : input.SortBy)
                .Select(x => new StudentForAdmissionModel
                {
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    PinType = x.PinType,
                    Pin = x.Pin,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionName,
                    RegionName = x.RegionName,
                    PositionId = x.PositionId,
                    PositionName = x.PositionName,
                    AdmissionDocumentId = x.AdmissionDocumentId,
                    NoteDate = x.NoteDate
                });

            int totalCount = await query.CountAsync();
            List<StudentForAdmissionModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            foreach (var item in items)
            {
                AdmissionDocumentViewModel validationModel = new AdmissionDocumentViewModel
                {
                    UsedInClassEnrollment = false,
                    InstitutionId = item.InstitutionId ?? default,
                    Position = item.PositionId ?? default
                };

                // В UI-а определя дали да се покаже бутона за запис в клас
                var (showInitialEntollmentButtonCheck, showInitialEntollmentButtonCheckError) = await _studentClassValidator.ShowInitialEntollmentButton(validationModel);
                item.CanBeEnrolled = showInitialEntollmentButtonCheck;
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<StudentToBeDischargedModel>> GetStudentsForDischarge(PagedListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(PagedListInput)));
            }

            IQueryable<VStudentsToDischarge> listQuery = _context.VStudentsToDischarges
                .AsNoTracking();

            // Определя дали са видими бутоните за редакция, потвърждаване(промяна от черова на потвърден), изтриване
            // на документите за отписване и преместване, от които идва необходимостта за отписване на ученика.
            // Видими са само за потребителите с роля институция.
            bool showControlBths = false;

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.Consortium:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.Mon:
                case UserRoleEnum.MonExpert:
                case UserRoleEnum.MonOBGUM:
                case UserRoleEnum.MonOBGUM_Finance:
                case UserRoleEnum.MonHR:
                    // Вижда всичко
                    break;
                case UserRoleEnum.School:
                case UserRoleEnum.InstitutionAssociate:
                case UserRoleEnum.SchoolDirector:
                case UserRoleEnum.KindergartenDirector:
                case UserRoleEnum.CPLRDirector:
                case UserRoleEnum.CSOPDirector:
                case UserRoleEnum.SOZDirector:
                    if (!_userInfo.InstitutionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    showControlBths = true;
                    listQuery = listQuery.Where(x => x.OldInstitutionId == _userInfo.InstitutionID);
                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    if (!_userInfo.RegionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    listQuery = listQuery.Where(x => x.RegionId == _userInfo.RegionID);
                    break;
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Teacher:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                case UserRoleEnum.ExternalExpert:
                case UserRoleEnum.Accountant:
                default:
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<StudentToBeDischargedModel> query = listQuery
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                   || predicate.PersonalId.Contains(input.Filter)
                   || predicate.PinType.Contains(input.Filter)
                   || predicate.OldClassName.Contains(input.Filter)
                   || predicate.NewInstitution.Contains(input.Filter)
                   || predicate.OldInstitution.Contains(input.Filter)
                   || predicate.SelectionType.Contains(input.Filter))
                .Select(x => new StudentToBeDischargedModel
                {
                    AdmissionDate = x.AdmissionDate,
                    AdmissionDocumentId = x.AdmissionDocumentId,
                    NoteDate = x.NoteDate,
                    DischargeDate = x.DischargeDate,
                    FullName = x.FullName,
                    Pin = x.PersonalId,
                    PinType = x.PinType,
                    Gender = x.Gender,
                    NewInstitution = x.NewInstitution,
                    NewInstitutionId = x.NewInstitutionId,
                    NewPosition = x.NewPosition,
                    OldClassGroupId = x.OldClassGroupId,
                    OldClassName = x.OldClassName,
                    OldInstitution = x.OldInstitution,
                    OldInstitutionId = x.OldInstitutionId,
                    OldPosition = x.OldPosition,
                    OldStudentClassid = x.OldStudentClassid,
                    PersonId = x.PersonId,
                    RelocationDocumentId = x.RelocationDocumentId,
                    DischargeDocumentId = x.DischargeDocumentId,
                    SelectionType = x.SelectionType,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "AdmissionDocumentId asc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<StudentToBeDischargedModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            foreach (var item in items)
            {
                item.ShowStudentLodLinkBtn = await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDataRead)
                    || await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead);

                if (showControlBths && item.RelocationDocumentId.HasValue
                    && (await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentUpdate)
                        || await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentDelete)
                        )
                    )
                {
                    item.ShowRelocationDocumentEditBtn = true;
                    item.ShowRelocationDocumentConfirmBtn = true;
                    item.ShowRelocationDocumentDeleteBtn = true;
                }

                if (showControlBths && item.DischargeDocumentId.HasValue
                    && (await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentUpdate)
                        || await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentDischargeDocumentDelete)
                       )
                    )
                {
                    item.ShowDischargeDocumentEditBtn = true;
                    item.ShowDischargeDocumentConfirmBtn = true;
                    item.ShowDischargeDocumentDeleteBtn = true;
                }

                if (showControlBths && !item.DischargeDocumentId.HasValue && !item.RelocationDocumentId.HasValue
                    && await _authorizationService.HasPermissionForStudent(item.PersonId, DefaultPermissions.PermissionNameForStudentRelocationDocumentCreate))
                {
                    item.ShowRelocationDocumentCreateBtn = true;
                }
            }

            return items.ToPagedList(totalCount);
        }
        public async Task<List<AbsenceCampaignViewModel>> GetAllActiveCampaigns()
        {
            var absenceCampaigns = await _absenceCampaignService.GetActive(CancellationToken.None);
            var aspCampaigns = await _aspService.GetActiveCampaigns();

            return absenceCampaigns.Concat(aspCampaigns).OrderBy(x => x.ToDate).ToList();
        }

        public async Task AdmissionStudentsAsync(List<StudentClassModel> models)
        {
            foreach (var model in models)
            {
                await _studentClassService.EnrollInClass(model, true);
            }
        }

        public Task DischargeStudentsAsync(List<StudentToBeDischargedModel> models)
        {
            // Todo: 
            throw new NotImplementedException();
        }

        public async Task<DirectorDashboardModel> GetDirectorDashboardAsync(int? institutionId)
        {
            if (!_userInfo.IsSchoolDirector || !institutionId.HasValue || !_userInfo.InstitutionID.HasValue || _userInfo.InstitutionID.Value != institutionId.Value)
            {
                throw new UnauthorizedAccessException();
            }

            var directorDashboard = await _context.Institutions.AsNoTracking()
                .Where(x => x.InstitutionId == institutionId.Value).Select(x =>
                new DirectorDashboardModel
                {
                    InstitutionCity = x.Town.Name,
                    InstitutionName = x.Name,
                }).SingleAsync();


            return directorDashboard;
        }

        public async Task<IPagedList<SopEnrollmentDetailViewModel>> GetSopEnrollmentDetails(PagedListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(PagedListInput)));
            }

            IQueryable<SopEnrollmentDetailViewModel> query = GetBaseSopEntrollmentQuery()
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                       predicate => predicate.FullName.Contains(input.Filter)
                       || predicate.MainInstName.Contains(input.Filter)
                       || predicate.MainInstRegionName.Contains(input.Filter)
                       || predicate.CsopInstName.Contains(input.Filter)
                       || predicate.CsopRegionName.Contains(input.Filter))
                    .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "FullName asc" : input.SortBy)
                    .Select(x => new SopEnrollmentDetailViewModel
                    {
                        PersonId = x.PersonId,
                        FullName = x.FullName,
                        Age = x.Age,
                        MainInstName = x.MainInstName,
                        MainInstRegionName = x.MainInstRegionName,
                        CsopInstName = x.CsopInstName,
                        CsopRegionName = x.CsopRegionName,
                        Uid = Guid.NewGuid().ToString()
                    });

            int totalCount = await query.CountAsync();
            IList<SopEnrollmentDetailViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<int> GetInstitutiontSopEnrollmentsCount()
        {
            if (!_userInfo.InstitutionID.HasValue) return 0;

            return await _context.EducationalStates
                .CountAsync(x => x.InstitutionId == _userInfo.InstitutionID.Value
                    && x.PositionId == (int)PositionType.StudentSpecialNeeds);
        }

        public async Task<int> GetStudentsCount()
        {
            IQueryable<StudentClass> query = GetStudentClasses();
            return await query.Select(x => x.PersonId).Distinct().CountAsync();
        }

        public async Task<List<StudentStatsModel>> GetStudentsCountGroupByClassType()
        {
            IQueryable<StudentClass> query = GetStudentClasses();
            List<StudentStatsModel> models = await query
                .GroupBy(x => x.ClassTypeId)
                .Select(g => new StudentStatsModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ClassTypeId = g.Key,
                    Total = g.Count(),
                    Children = new List<StudentStatsModel>()
                })
                .ToListAsync();

            List<ClassType> classTypes = await _context.ClassTypes
                .ToListAsync();

            foreach (var model in models)
            {
                model.Name = classTypes.FirstOrDefault(x => x.ClassTypeId == model.ClassTypeId)?.Name ?? "-";
            }

            return models.OrderBy(x => x.Name).ToList();
        }

        public async Task<List<StudentStatsModel>> GetStudentsCountByClassType(int classTypeId)
        {
            IQueryable<StudentClass> query = GetStudentClasses()
                .Where(x => x.ClassTypeId == classTypeId);

            List<StudentStatsModel> models = await query
                .GroupBy(x => x.PositionId)
                .Select(g => new StudentStatsModel
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionId = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            List<Position> positions = await _context.Positions
                .ToListAsync();

            foreach (var model in models)
            {
                model.Name = positions.FirstOrDefault(x => x.PositionId == model.PositionId)?.Name;
            }

            return models.OrderBy(x => x.PositionId).ToList();
        }


        public async Task<ClassGroupStatsModel> GetClassGroupStats()
        {
            IQueryable<ClassGroup> query = _context.ClassGroups
                 .AsNoTracking()
                 .Where(x => x.IsValid != false)
                 .Where(x => x.ParentClassId.HasValue || (!x.ParentClassId.HasValue && x.IsNotPresentForm.HasValue && x.IsNotPresentForm.Value));

            if (_userInfo.IsMon || _userInfo.IsMonExpert || _userInfo.IsConsortium || _userInfo.IsCIOO)
            {
                // Всички данни
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => _userInfo.RegionID.HasValue && x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID);
            }
            else if (_userInfo.IsSchoolDirector || _userInfo.IsInstitutionAssociate)
            {
                short currentYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

                query = query.Where(x => x.InstitutionSchoolYear.SchoolYear == currentYear
                    && _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID);
            }
            else if (_userInfo.IsTeacher)
            {
                short currentYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

                query = query.Where(x => x.InstitutionSchoolYear.SchoolYear == currentYear
                    && _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID);
                // Todo: Филтър само на класовете на класния
            }
            else
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ClassGroupStatsModel model = new ClassGroupStatsModel
            {
                Total = await query.CountAsync(),
                ClassGroupsByType = await query
                    .Where(x => x.ClassType.Name != null)
                    .OrderBy(x => x.ClassType.Name)
                    .GroupBy(x => x.ClassType.Name)
                    .Select(x => new { ClassType = x.Key, Total = x.Count() })
                    .ToDictionaryAsync(x => x.ClassType, x => x.Total)
            };

            return model;
        }

        public async Task<DiplomaStatsModel> GetDiplomaStats()
        {
            IQueryable<Diploma> query = _context.Diplomas
                .Where(x => x.IsFinalized);

            if (_userInfo.IsMon || _userInfo.IsMonExpert || _userInfo.IsConsortium || _userInfo.IsCIOO)
            {
                // Всички данни
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => _userInfo.RegionID.HasValue && x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID);
            }
            else if (_userInfo.IsSchoolDirector || _userInfo.IsInstitutionAssociate)
            {
                query = query.Where(x => _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID);
            }
            else if (_userInfo.IsTeacher)
            {
                query = query.Where(x => _userInfo.InstitutionID.HasValue && x.InstitutionId == _userInfo.InstitutionID);
                // Todo: Филтър само на класовете на класния
            }
            else
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            short currentYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

            return new DiplomaStatsModel
            {
                Total = await query.CountAsync(),
                TotalForCurrentSchoolYear = await query.CountAsync(x => x.SchoolYear == currentYear)
            };
        }

        public async Task<IPagedList<StudentEnvironmentCharacteristicModel>> GetStudentEnvironmentCharacteristics(StudentEnvCharacteristicListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(StudentEnvCharacteristicListInput)));
            }

            IQueryable<VStudentEnvironmentCharacteristic> query = _context.VStudentEnvironmentCharacteristics;

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.Consortium:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.Mon:
                case UserRoleEnum.MonExpert:
                case UserRoleEnum.MonOBGUM:
                case UserRoleEnum.MonOBGUM_Finance:
                case UserRoleEnum.MonHR:
                    // Вижда всичко
                    break;
                case UserRoleEnum.School:
                case UserRoleEnum.InstitutionAssociate:
                case UserRoleEnum.SchoolDirector:
                case UserRoleEnum.KindergartenDirector:
                case UserRoleEnum.CPLRDirector:
                case UserRoleEnum.CSOPDirector:
                case UserRoleEnum.SOZDirector:
                    if (!_userInfo.InstitutionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    break;
                case UserRoleEnum.Teacher:
                    if (!_userInfo.InstitutionID.HasValue || _userInfo.LeadTeacherClasses == null)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value
                        && _userInfo.LeadTeacherClasses.Contains(x.ClassId));
                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    if (!_userInfo.RegionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.RegionId == _userInfo.RegionID.Value);

                    break;
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                case UserRoleEnum.ExternalExpert:
                case UserRoleEnum.Accountant:
                default:
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<StudentEnvironmentCharacteristicModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.RegionName.Contains(input.Filter)
                   || predicate.RelativeFullName.Contains(input.Filter)
                   || predicate.RelativeType.Contains(input.Filter)
                   || predicate.WorkStatus.Contains(input.Filter)
                   || predicate.EducationType.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "BasicClassId asc, ClassName asc, FullName asc" : input.SortBy)
                .Select(x => new StudentEnvironmentCharacteristicModel
                {
                    StudentClassId = x.StudentClassId,
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    Pin = x.Pin,
                    PinType = x.PinType,
                    ClassName = x.ClassName,
                    BasicClassId = x.BasicClassId,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionName,
                    RegionId = x.RegionId,
                    RegionName = x.RegionName,
                    RelativeFullName = x.RelativeFullName,
                    RelativeType = x.RelativeType,
                    WorkStatus = x.WorkStatus,
                    EducationType = x.EducationType,
                    HasParentConsent = x.HasParentConsent ?? false
                });

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<StudentEnvironmentCharacteristicModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }


        public async Task<IPagedList<VStudentExternalEvaluation>> GetStudentExernalEvaluationsList(DualFormEmployerListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input)));
            }

            IQueryable<VStudentExternalEvaluation> query = _context.VStudentExternalEvaluations;

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.Consortium:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.Mon:
                case UserRoleEnum.MonExpert:
                case UserRoleEnum.MonOBGUM:
                case UserRoleEnum.MonOBGUM_Finance:
                case UserRoleEnum.MonHR:
                    // Вижда всичко
                    break;
                case UserRoleEnum.School:
                case UserRoleEnum.InstitutionAssociate:
                case UserRoleEnum.SchoolDirector:
                case UserRoleEnum.KindergartenDirector:
                case UserRoleEnum.CPLRDirector:
                case UserRoleEnum.CSOPDirector:
                case UserRoleEnum.SOZDirector:
                    if (!_userInfo.InstitutionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
                    break;
                case UserRoleEnum.Teacher:
                    if (!_userInfo.InstitutionID.HasValue || _userInfo.LeadTeacherClasses == null)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value
                        && _userInfo.LeadTeacherClasses.Contains(x.ClassId));
                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    if (!_userInfo.RegionID.HasValue)
                    {
                        throw new ApiException(Messages.UnauthorizedMessageError, 401);
                    }

                    query = query.Where(x => x.RegionId == _userInfo.RegionID.Value);

                    break;
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                case UserRoleEnum.ExternalExpert:
                case UserRoleEnum.Accountant:
                default:
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            IQueryable<VStudentExternalEvaluation> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.FullName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionName.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.ClassName.Contains(input.Filter)
                   || predicate.RegionName.Contains(input.Filter)
                   || predicate.Type.Contains(input.Filter)
                   || predicate.SubjectName.Contains(input.Filter)
                   || predicate.SubjectTypeName.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<VStudentExternalEvaluation> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        private IQueryable<VSopEnrollmentDetail> GetBaseSopEntrollmentQuery()
        {
            IQueryable<VSopEnrollmentDetail> query = _context.VSopEnrollmentDetails.AsNoTracking();

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.School:
                case UserRoleEnum.InstitutionAssociate:
                    int instId = _userInfo.InstitutionID ?? int.MinValue;
                    query = query.Where(x => x.MainInstCode == instId);
                    break;
                case UserRoleEnum.Mon:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.MonExpert:
                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    int regionId = _userInfo.RegionID ?? int.MinValue;
                    query = query.Where(x => x.MainInstRegionId == regionId || x.CsopRegionId == regionId);
                    break;
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Teacher:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                case UserRoleEnum.ExternalExpert:
                default:
                    query.Where(x => false);
                    break;
            }

            return query;
        }
    }
}
