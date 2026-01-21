namespace MON.Services.Implementations
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Dashboards;

    using MON.Models.Institution;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Class;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Enums.AspIntegration;
    using SQLitePCL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class ClassGroupService : BaseService<ClassGroupService>, IClassGroupService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IMemoryCache _cache;
        private readonly IInstitutionService _institutionService;

        public ClassGroupService(DbServiceDependencies<ClassGroupService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IMemoryCache cache,
            IInstitutionService institutionService)
                : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _cache = cache;
            _institutionService = institutionService;
        }

        public async Task<ClassGroupViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _context.ClassGroups
                .Where(x => x.ClassId == id)
                .Select(x => new ClassGroupViewModel
                {
                    Id = x.ClassId,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    ClassEduFormName = x.ClassEduForm.Name,
                    ClassEduFormId = x.ClassEduFormId,
                    ClassName = x.ClassName,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    ClassSpecialityName = x.ClassSpecialityId == -1 ? "" : x.ClassSpeciality.Name,
                    ClassSpecialityId = x.ClassSpecialityId == -1 ? null : x.ClassSpecialityId,
                    ClassProfessionId = x.ClassSpeciality.ProfessionId == -1 ? (int?)null : x.ClassSpeciality.ProfessionId,
                    ClassProfessionName = x.ClassSpeciality.ProfessionId == -1 ? "" : x.ClassSpeciality.Profession.Name,
                    BasicClassId = x.BasicClassId,
                    BasicClassName = x.BasicClass.Name,
                    BasicClassRomeName = x.BasicClass.RomeName,
                    BasicClassDescription = x.BasicClass.Description,
                    ClassTypeName = x.ClassType.Name,
                    IsNotPresentForm = x.IsNotPresentForm,
                    IsValid = x.IsValid ?? true, // В базата има default(1)
                    ClassTypeId = x.ClassTypeId,
                    ClassKindId = x.ClassType.ClassKind,
                    IsCurrent = x.InstitutionSchoolYear.IsCurrent,
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        
        public async Task<List<StudentInfoExternalModel>> GetStudents(int classId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.HasPermissionForClass(classId, DefaultPermissions.PermissionNameForClassStudentsRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }
            bool showInitialPassword = _userInfo != null && (_userInfo.IsSchoolDirector || _userInfo.IsTeacher);

            var currentSchoolYear = await _institutionService.GetCurrentYear(null);

            var result = await (from x in _context.VClassStudents
                                join lf in _context.Lodfinalizations on x.LodFinalizationId equals lf.Id into temp
                                from lodFin in temp.DefaultIfEmpty()
                                where x.ClassId == classId && (x.IsCurrent || x.Status == (int)MovementStatusEnum.Enrolled || (x.SchoolYear < currentSchoolYear && (x.Status == (int)MovementStatusEnum.Discharged || x.Status == (int)MovementStatusEnum.Moved)))
                                orderby x.ClassNumber, x.FullName
                                select new StudentInfoExternalModel
                                {
                                    Id = x.Id,
                                    PersonId = x.PersonId,
                                    FullName = x.FullName,
                                    Pin = x.Pin,
                                    PinType = x.PinType,
                                    Usernames = x.IsAzureUser == true ? x.Username : "",
                                    InitialPasswords = showInitialPassword ? x.InitialPassword : "",
                                    ClassId = x.ClassId,
                                    ClassName = x.ClassName,
                                    SchoolYear = x.SchoolYear,
                                    IsLodApproved = x.IsLodApproved,
                                    IsLodFinalized = x.IsLodFinalized,
                                    Document = lodFin.Document.ToViewModel(_blobServiceConfig),
                                    MainClassName = x.MainClassName,
                                    EduFormName = x.EduFormName,
                                    PositionName = x.PositionName,
                                    IsCurrent = x.IsCurrent
                                })
                .ToListAsync(cancellationToken);

            HashSet<int> personIds = result.Select(x => x.PersonId).ToHashSet();
            short schoolYear = result.Select(x => x.SchoolYear).FirstOrDefault();

            var unfinishedLods = await _context.VStudentLodFinalizations
                .Where(x => personIds.Contains(x.PersonId) && x.SchoolYear == schoolYear && !x.IsFinalized)
                .Select(x => new
                {
                    x.PersonId,
                    x.SchoolYearName
                })
                .ToListAsync(cancellationToken);

            foreach (var item in result)
            {
                item.LodNotFinalizationYears = string.Join("|", unfinishedLods.Where(l => l.PersonId == item.PersonId).Select(l => l.SchoolYearName));
            }

            return result;
        }

        public async Task<List<StudentForAdmissionModel>> GetStudentsForEnrollment(int classId, CancellationToken cancellationToken)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (!await _authorizationService.HasPermissionForClass(classId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            IQueryable<VStudentsToEnroll> query = _context.VStudentsToEnrolls
                .Where(x => x.RowNum == 1 && x.InstitutionId == _userInfo.InstitutionID.Value);

            return await query.OrderBy(x => x.FullName)
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
                }).ToListAsync(cancellationToken);
        }

        public async Task<List<StudentForAdmissionModel>> GetStudentsForMassEnrollment(int classId, CancellationToken cancellationToken)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (!await _authorizationService.HasPermissionForClass(classId, DefaultPermissions.PermissionNameForStudentToClassEnrollment))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            ClassGroupViewModel classDetails = await GetById(classId, cancellationToken);

            FormattableString queryString = $"select * from student.fn_ClassStudentsToEnroll({classId})";
            IQueryable<ClassStudentsToEnrollDto> query = _context.Set<ClassStudentsToEnrollDto>()
                .FromSqlInterpolated(queryString);

            return await query.OrderBy(x => x.FullName)
                .Select(x => new StudentForAdmissionModel
                {
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    PinType = x.PinType,
                    Pin = x.Pin,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionName,
                    PositionId = x.PositionId,
                    PositionName = x.PositionName,
                    AdmissionDocumentId = x.AdmissionDocumentId,
                    NoteDate = x.AdmissionDocumentNoteDate,
                    PersonAge = x.Age,
                    PersonBirthDate = x.BirthDate,
                    MainClassName = x.MainClassName,
                    MainClassId = x.MainClassId,
                }).ToListAsync(cancellationToken);
        }

        public Task<ClassGroupCacheModel> GetClassGroupCache(int classId)
        {
            return _cache.GetOrCreateAsync($"{CacheKeys.ClassGroupCacheModels}_{classId}", entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.ClassGroups
                    .Where(x => x.ClassId == classId)
                    .Select(x => new ClassGroupCacheModel
                    {
                        ParentClassId = x.ParentClassId,
                        InstitutionId = x.InstitutionId,
                        RegionId = x.InstitutionSchoolYear.Town.Municipality.RegionId,
                        IsValid = x.IsValid ?? true, // В базата има default(1)
                        ClassTypeId = x.ClassTypeId
                    })
                    .SingleOrDefaultAsync();
            });
        }
    }
}
