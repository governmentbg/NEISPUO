namespace MON.Models.StudentModels
{
    using System.Collections.Generic;

    public class StudentScholarshipsViewModel
    {
        public bool HasStudentClassInCurrentYear => ScholarshipDetails != null && ScholarshipDetails.Count > 0;
        public List<ScholarshipViewModel> ScholarshipDetails { get; set; }
    }
}
