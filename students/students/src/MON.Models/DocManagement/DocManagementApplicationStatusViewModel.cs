namespace MON.Models.DocManagement
{
    using System.Collections.Generic;
    using System;
    using MON.Shared.Enums;
    using MON.Shared.Extensions;

    public class DocManagementApplicationStatusViewModel : DocManagementApplicationStatusModel
    {
        public int InstitutionId { get; set; }
        public string StatusName => Status.TryParseEnum<ApplicationStatusEnum>().GetEnumDescription();
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Updater { get; set; }
        public bool HasResponsePermission { get; set; }
        public IEnumerable<DocManagementApplicationStatusViewModel> Responses { get; set; } = Array.Empty<DocManagementApplicationStatusViewModel>();
    }
}
