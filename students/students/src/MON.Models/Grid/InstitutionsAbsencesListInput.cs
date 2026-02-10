namespace MON.Models
{
    public class InstitutionsAbsencesListInput : PagedListInput
    {
        public short? SchoolYear { get; set; }
        public short? Month { get; set; }
    }
}
