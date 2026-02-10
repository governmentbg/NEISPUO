using MON.Shared.Interfaces;

namespace MON.Models
{

    public class InstitutionListInput : PagedListInput, IInstitution
    {
        public InstitutionListInput()
        {
            SortBy = "Id desc";
        }
        public int? InstitutionId { get; set; }

        public int? RegionId { get; set; }

        public int? InstTypeId { get; set; }

        public string SelectedClasses { get; set; }
    }
}
