namespace MON.Models.DocManagement
{
    using System;
    using System.Collections.Generic;

    public class DocManagementCampaignAttachmentModel
    {
        public int CampaignId { get; set; }
        public IEnumerable<DocumentModel> Attachments { get; set; } = Array.Empty<DocumentModel>();
    }

    public class DocManagementCampaignAttachmentViewModel : DocManagementCampaignAttachmentModel
    {
        public new IEnumerable<DocumentViewModel> Attachments { get; set; } = Array.Empty<DocumentViewModel>();
    }
}
