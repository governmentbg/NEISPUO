namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models.Identity;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared;
    using Helpdesk.Shared.Enums;
    using Helpdesk.Shared.Extensions;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AuthenticationService : BaseService, IAuthenticationService
    {
        public AuthenticationService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<AuthenticationService> logger)
            : base(context, userInfo, logger)
        {
        }

        public async Task<bool> DemandPermissionsForIssueAsync(int? issueId, PermissionEnum permission)
        {
            if (issueId != null)
            {
                var issue = await _context.Issues.FirstOrDefaultAsync(i => i.Id == issueId);

                if (issue == null) return false;

                var result = permission switch
                {
                    PermissionEnum.Read => ((_userInfo.SysUserID == issue.SubmitterSysUserId)
                                || (_userInfo.IsMon || _userInfo.IsCIOO || _userInfo.IsMonExpert || _userInfo.IsRuo || _userInfo.IsRuoExpert || _userInfo.IsConsortium )),
                    PermissionEnum.Update => ((_userInfo.SysUserID == issue.SubmitterSysUserId)
                                || (_userInfo.IsMon || _userInfo.IsCIOO || _userInfo.IsMonExpert || _userInfo.IsRuo || _userInfo.IsRuoExpert || _userInfo.IsConsortium)),
                    PermissionEnum.Reopen => (_userInfo.IsConsortium || _userInfo.IsMon),
                    _ => false
                };

                return result;
            }
            else
            {
                var result = permission switch
                {
                    PermissionEnum.Create => (_userInfo.IsSchoolDirector || _userInfo.IsMon || _userInfo.IsCIOO || _userInfo.IsMonExpert || _userInfo.IsRuo || _userInfo.IsRuoExpert || _userInfo.IsMunicipality || _userInfo.IsConsortium),
                    _ => false
                };

                return result;
            }
        }

        public async Task<bool> DemandPermissionsForStatisticsAsync(StatisticsEnum permission)
        {

            var result = permission switch
            {
                StatisticsEnum.Category
                 => _userInfo.IsConsortium,
                _ => false
            };

            return result;
        }

        public async Task<bool> DemandPermissionsForQuestionAsync(int? questionId, PermissionEnum permission)
        {
            if (questionId.HasValue)
            {
                var question = await (
                    from i in _context.Faquestions
                    where i.Id == questionId
                    select i).FirstOrDefaultAsync();

                if (question == null) return false;

                var result = permission switch
                {
                    PermissionEnum.Read => (_userInfo.IsSchoolDirector || _userInfo.IsMon || _userInfo.IsCIOO || _userInfo.IsMonExpert || _userInfo.IsRuo || _userInfo.IsRuoExpert || _userInfo.IsConsortium || _userInfo.IsMunicipality),
                    PermissionEnum.Update => _userInfo.IsConsortium,
                    _ => false
                };

                return result;
            }
            else
            {
                var result = permission switch
                {
                    PermissionEnum.Create => _userInfo.IsConsortium,
                    _ => false
                };

                return result;
            }
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

            var institutionData = await _context.InstitutionSchoolYears
                .Where(x => x.InstitutionId == _userInfo.InstitutionID)
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new
                {
                    x.Name,
                    x.BaseSchoolTypeId,
                    Town = x.Town.Name,
                    x.Town.MunicipalityId,
                    Municipality = x.Town.Municipality.Name,
                    x.Town.Municipality.RegionId,
                    Region = x.Town.Municipality.Region.Name,
                })
                .FirstOrDefaultAsync();


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
                Institution = institutionData?.Name,
                MunicipalityID = municipality?.MunicipalityId ?? institutionData?.MunicipalityId,
                Municipality = municipality?.Name ?? institutionData?.Municipality,
                RegionID = region?.RegionId ?? institutionData?.RegionId,
                Region = region?.Name ?? institutionData?.Region,
                BaseSchoolTypeId = institutionData?.BaseSchoolTypeId
            };

            if (_userInfo.InstitutionID.HasValue)
            {
            }

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
