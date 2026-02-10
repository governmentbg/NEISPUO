namespace MON.Models
{
    public class PreSchoolEvalListInput : PagedListInput
    {
        public PreSchoolEvalListInput()
        {
            SortBy = "BasicClassId asc, SchoolYear desc, SubjectId asc";
        }

        public int? PersonId { get; set; }
    }
}
