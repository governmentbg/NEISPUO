using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.Identity;
using MON.Services.Implementations;
using MON.Services.Interfaces;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    [ExcludeFromCodeCoverage]
    public class NeispuoAuthenticationService : BaseService<NeispuoAuthenticationService>, INeispuoAuthenticationService
    {
        private readonly IInstitutionService _institutionService;
        public NeispuoAuthenticationService(DbServiceDependencies<NeispuoAuthenticationService> dependencies,
            IInstitutionService institutionService)
            : base(dependencies)
        {
            _institutionService = institutionService;
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            var municipality = _userInfo.MunicipalityID.HasValue
                ? await _context.Municipalities
                    .Where(x => x.MunicipalityId == _userInfo.MunicipalityID.Value)
                    .Select(x => new
                    {
                        x.MunicipalityId,
                        x.Name
                    })
                    .SingleOrDefaultAsync()
                : null;

            var region = _userInfo.RegionID.HasValue
                ? await _context.Regions
                    .Where(x => x.RegionId == _userInfo.RegionID.Value)
                    .Select(x => new
                    {
                        x.RegionId,
                        x.Name
                    })
                    .SingleOrDefaultAsync()
                : null;

            InstitutionCacheModel institutionCacheData = null;
            if (_userInfo.InstitutionID.HasValue)
            {
                institutionCacheData = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);
            }

            var institutionData = await _context.InstitutionSchoolYears
                .Where(x => x.InstitutionId == _userInfo.InstitutionID)
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new
                {
                    Town = x.Town.Name,
                    x.Town.MunicipalityId,
                    Municipality = x.Town.Municipality.Name,
                    x.Town.Municipality.RegionId,
                    Region = x.Town.Municipality.Region.Name
                })
                .FirstOrDefaultAsync();

            short globalCurrentYear = default;
            if (!_userInfo.InstitutionID.HasValue)
            {
                globalCurrentYear = (await _context.CurrentYears.FirstOrDefaultAsync(i => i.IsValid.HasValue && i.IsValid.Value)).CurrentYearId;
            }

            string address = "";
            if (institutionData != null)
            {
                if (!institutionData.Town.IsNullOrWhiteSpace()) address += $" гр./с.{institutionData.Town}";
                if (!institutionData.Municipality.IsNullOrWhiteSpace()) address += $" общ. {institutionData.Municipality}";
                if (!institutionData.Region.IsNullOrWhiteSpace()) address += $" обл. {institutionData.Region}";
            }
            else
            {
                if (!municipality?.Name.IsNullOrWhiteSpace() ?? false) address += $" общ. {municipality?.Name}";
                if (!region?.Name.IsNullOrWhiteSpace() ?? false) address += $" обл. {region?.Name}";
            }

            var model = new UserInfoViewModel()
            {
                SysUserID = _userInfo.SysUserID,
                SysRoleID = _userInfo.SysRoleID,
                RoleName = _userInfo.UserRole.GetEnumDescription(),
                Address = address.Trim(),
                BudgetingInstitutionID = _userInfo.BudgetingInstitutionID,
                IsLeadTeacher = _userInfo.IsLeadTeacher,
                LeadTeacherClasses = _userInfo.LeadTeacherClasses,
                InstitutionID = _userInfo.InstitutionID,
                Institution = institutionCacheData?.Name,
                MunicipalityID = municipality?.MunicipalityId ?? institutionData?.MunicipalityId,
                Municipality = municipality?.Name ?? institutionData?.Municipality,
                RegionID = region?.RegionId ?? institutionData?.RegionId,
                Region = region?.Name ?? institutionData?.Region,
                BaseSchoolTypeId = institutionCacheData?.BaseSchoolTypeId,
                CurrentSchoolYear = institutionCacheData?.SchoolYear ?? globalCurrentYear,
                InstTypeId = institutionCacheData?.InstTypeId ?? default,
                InstType = institutionCacheData != null && institutionCacheData.InstTypeId.HasValue 
                    ? ((InstitutionTypeEnum)institutionCacheData.InstTypeId).GetEnumDescription()
                    : default,
                SchoolYearName = institutionCacheData?.SchoolYearName,
            };

            if (model.IsLeadTeacher != null && model.IsLeadTeacher.Value)
            {
                model.LeadTeacherClassGroups = await
                    (from cg in _context.ClassGroups
                     where _userInfo.LeadTeacherClasses.Contains(cg.ClassId)
                     select new ClassGroupInfoModel()
                     {
                         Id = cg.ClassId,
                         Name = cg.ClassName
                     }).ToListAsync();
            }

            return model;
        }
    }
}
