namespace Helpdesk.Services.Interfaces
{
    using Helpdesk.Models.Identity;
    using Helpdesk.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAuthenticationService
    {
        Task<UserInfoViewModel> GetUserInfo();
        Task<bool> DemandPermissionsForIssueAsync(int? issueId, PermissionEnum permission);
        Task<bool> DemandPermissionsForQuestionAsync(int? questionId, PermissionEnum permission);
        Task<bool> DemandPermissionsForStatisticsAsync(StatisticsEnum permission);
    }
}
