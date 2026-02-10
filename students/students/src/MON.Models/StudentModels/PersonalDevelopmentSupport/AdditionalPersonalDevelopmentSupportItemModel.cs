namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using System;
    using System.Collections.Generic;

    public class AdditionalPersonalDevelopmentSupportItemModel
    {
        public int? Id { get; set; }
        public int TypeId { get; set; }
        public string Details { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? SuspensionDate { get; set; }
        public string SuspensionReason { get; set; }
        public IEnumerable<ResourceSupportModel> ResourceSupport { get; set; } = Array.Empty<ResourceSupportModel>();
        public string Uid => Guid.NewGuid().ToString();

        public IEnumerable<DocumentModel> SuspensionDocuments { get; set; } = Array.Empty<DocumentModel>();
    }
}
