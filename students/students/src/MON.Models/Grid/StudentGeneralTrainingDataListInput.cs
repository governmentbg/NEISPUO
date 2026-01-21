namespace MON.Models.Grid
{
    public class StudentGeneralTrainingDataListInput : PagedListInput
    {
        public int? StudentId { get; set; }

        public StudentGeneralTrainingDataListInput()
        {
            SortBy = "SchoolYear desc, AdmissionDate desc";
        }
    }
}
