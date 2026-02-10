using MON.Shared.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MON.Models.Refugee
{
    public class ApplicationViewModel : ApplicationModel
    {
        public string Region { get; set; }
        public string Nationality { get; set; }
        public string GuardianTypeName { get; set; }
        public string Town { get; set; }
        public string PersonalIdTypeName { get; set; }
        public new List<ApplicationChildViewModel> Children { get; set; }
        public string StatusName { get; set; }
        public int ChildrenCount => Children != null ? Children.Count : 0;
        public int ChildrenWithOrderCount => Children != null 
            ? Children.Count(x => !string.IsNullOrWhiteSpace(x.RuoDocNumber) 
                && x.RuoDocDate.HasValue && x.InstitutionId.HasValue)
            : 0;
        public bool CanBeDeleted => Status != (int)ApplicationStatusEnum.Completed
            && (Children == null || !Children.Any(x => x.Status == (int)ApplicationStatusEnum.Completed));
        public bool CanBeCancelled => Status != (int)ApplicationStatusEnum.Cancelled;
        public bool CanBeEdited => Status == (int)ApplicationStatusEnum.InProcess;
        public bool CanBeCompleted => Status == (int)ApplicationStatusEnum.InProcess
            && Children != null && !Children.Any(x => string.IsNullOrWhiteSpace(x.RuoDocNumber) || !x.RuoDocDate.HasValue ||!x.InstitutionId.HasValue);
        public bool CanBeSetAsEditable => Status != (int)ApplicationStatusEnum.InProcess;
    }
}
