namespace MON.Shared.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStudentHub
    {
        Task SendMessage(string msg);
        Task StudentFinalizedLodsReloaded(List<short> finalizedLods);
        Task ContextualInformationReloaded(Dictionary<string, string> contextualInformation);
        Task AbsenceCampaignModified(int id);
        Task AspCampaignModified(int id);
        Task ResourceSupportModified(int id);
        Task PersonalDevelopmentModified(int id);
        Task RefugeeEnrolledInInstitution(int personId, int institutionId, int? regionId);
        Task StudentClassEduStateChange(int personId);
        Task DocManagementCampaignModified(int id);
        Task DocManagementAdditionalCampaignModified(int id, int? parentId);
    }
}
