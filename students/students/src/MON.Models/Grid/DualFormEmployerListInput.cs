namespace MON.Models.Grid
{
    using MON.Shared.Interfaces;

    public class DualFormEmployerListInput : PagedListInput, IInstitution
    {
        public int? InstitutionId { get; set; }
        public short? SchoolYear { get; set; }
    }
}
