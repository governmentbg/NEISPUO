using MON.Models.Enums;
using MON.Shared;
using System;
using System.Collections.Generic;

namespace MON.Models.Absence
{
    public class AbsenceCampaignViewModel : AbsenceCampaignInputModel
    {
        public string SchoolYearName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Updater { get; set; }
        public bool IsActive { get; set; }
        public bool IsUpcoming => DateTime.UtcNow < FromDate;
        public int InstitutionStatus { get; set; }
        public int? ImportId { get; set; }
        public CampaignType CampaignType { get; set; }
        public string CampaignTypeName => CampaignType.GetEnumDescriptionAttrValue();
        public bool IsRelatedInstitutionImportSigned { get; set;  }
        public List<KeyValuePair<string, string>> Labels { get; set; }
        public bool HasAspAskingSession { get; set; }
    }
}
