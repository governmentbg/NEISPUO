namespace MON.Models.Grid
{
    using MON.Shared.Interfaces;

    public class ClassesListInput : PagedListInput, IInstitution
    {
        public int? InstitutionId { get; set; }

        public short? SchoolYear { get; set; }
        public int? BasicClass { get; set; }
    }
}
