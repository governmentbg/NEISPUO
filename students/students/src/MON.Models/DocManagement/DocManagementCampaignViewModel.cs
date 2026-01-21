namespace MON.Models.DocManagement
{
    using System;
    using System.Collections.Generic;

    public class DocManagementCampaignViewModel : DocManagementCampaignModel
    {
        public string InstitutionName { get; set; }
        public string SchoolYearName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Updater { get; set; }
        public bool IsActive
        {
            get
            {
                DateTime now = DateTime.Now;

                return IsManuallyActivated || (FromDate <= now && now < ToDate.AddDays(1));
            }
        }        
        public bool IsUpcoming => DateTime.UtcNow < FromDate;

        public List<KeyValuePair<string, string>> Labels
        {
            get
            {
                var labels = new List<KeyValuePair<string, string>>();
                if (IsActive) 
                {
                    labels.Add(new KeyValuePair<string, string>("Активна", "success"));
                }

                if (IsUpcoming)
                {
                    labels.Add(new KeyValuePair<string, string>("Предстояща", "info"));
                }

                if(HasApplication)
                {
                    labels.Add(new KeyValuePair<string, string>("Подадено заявление", "success"));
                }

                if(ParentId.HasValue)
                {
                    labels.Add(new KeyValuePair<string, string>("Допълнителна кампания", "light"));
                }

                return labels;
            }
        }

        public bool HasApplication { get; set; }
        public bool IsHidden { get; set; }
        public bool HasRuoAttachmentPermision { get; set; }

        public IEnumerable<DocumentViewModel> Attachments { get; set; } = Array.Empty<DocumentViewModel>();
    }
}
