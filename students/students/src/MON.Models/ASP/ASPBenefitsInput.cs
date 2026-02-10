namespace MON.Models.ASP
{
    public class ASPBenefitsInput : PagedListInput
    {
        public ASPBenefitsInput()
            : base()
        {
            SortBy = "CurrentInstitutionId desc";
        }

        public int ImportedFileId { get; set; }
        public int StatusFilter { get; set; }
        public short SchoolYear { get; set; }
        public short Month { get; set; }
    }
}
