using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface ISignalRNotificationService
    {
        Task StudentHubTest(string msg, int? personId);
        Task StudentFinalizedLodsReloaded(int personId, List<short> finalizedLods);
        Task ContextualInformationReloaded(Dictionary<string, string> contextualInformation);
        Task NotifyUser(string userEmail, string message);
        Task ChangePersonMessages(string username, int notReadMessages);
        Task AbsenceCampaignModified(int id);
        Task DocManagementCampaignModified(int id);
        Task DocManagementAdditionalCampaignModified(int id, int? parentId);
        Task AspCampaignModified(int id);
        Task ResourceSupportModified(int personId, int id);
        Task PersonalDevelopmentModified(int personId, int id);
        Task RefugeeEnrolledInInstitution(int personId, int institutionId, int? regionId);
        Task StudentClassEduStateChange(int personId);
    }
}
