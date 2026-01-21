using Microsoft.AspNetCore.SignalR;
using MON.Services.Hubs;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly IHubContext<StudentHub, IStudentHub> _studentHub;
        public SignalRNotificationService(IHubContext<StudentHub, IStudentHub> studentHub)
        {
            _studentHub = studentHub;
        }

        public Task StudentHubTest(string msg, int? personId)
        {
            return _studentHub.Clients.Group((personId ?? int.MinValue).ToString())
                .SendMessage(msg ?? nameof(StudentHubTest));
        }

        public Task NotifyUser(string username, string message)
        {
            return _studentHub.Clients.User(username).SendMessage(message ?? nameof(NotifyUser));
        }

        public Task StudentPermissionsReload(int? personId)
        {
            throw new NotImplementedException();
        }

        public Task StudentFinalizedLodsReloaded(int personId, List<short> finalizedLods)
        {
            return _studentHub.Clients.Group((personId).ToString())
                .StudentFinalizedLodsReloaded(finalizedLods);
        }

        public Task ContextualInformationReloaded(Dictionary<string, string> contextualInformation)
        {
            return _studentHub.Clients.All
                .ContextualInformationReloaded(contextualInformation);
        }

        public Task ChangePersonMessages(string username, int notReadMessages)
        {
            return _studentHub.Clients.User(username).SendMessage((notReadMessages).ToString());
        }

        public Task AbsenceCampaignModified(int id)
        {
            return _studentHub.Clients.All
                .AbsenceCampaignModified(id);
        }

        public Task DocManagementCampaignModified(int id)
        {
            return _studentHub.Clients.All
                .DocManagementCampaignModified(id);
        }

        public Task DocManagementAdditionalCampaignModified(int id, int? parentId)
        {
            return _studentHub.Clients.All
                .DocManagementAdditionalCampaignModified(id, parentId);
        }

        public Task ResourceSupportModified(int personId, int id)
        {
            return _studentHub.Clients.Group((personId).ToString())
                .ResourceSupportModified(id);
        }

        public Task PersonalDevelopmentModified(int personId, int id)
        {
            return _studentHub.Clients.Group((personId).ToString())
                .PersonalDevelopmentModified(id);
        }

        public Task RefugeeEnrolledInInstitution(int personId, int institutionId, int? regionId)
        {
            return _studentHub.Clients.All
                .RefugeeEnrolledInInstitution(personId, institutionId, regionId);
        }

        public Task AspCampaignModified(int id)
        {
            return _studentHub.Clients.All
                .AspCampaignModified(id);
        }

        public Task StudentClassEduStateChange(int personId)
        {
            return _studentHub.Clients.Group((personId).ToString())
                .StudentClassEduStateChange(personId);
        }
    }
}
