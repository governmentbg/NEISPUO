namespace MON.Models.StudentModels.StoredProcedures
{
    using System.Collections.Generic;

    public class StudentTermGradeViewModel
    {
        public bool GradesLoadedFromSchoolbook { get; set; }
        public IEnumerable<StudentTermGradesModel> TermGrades { get; set; }
    }
}
