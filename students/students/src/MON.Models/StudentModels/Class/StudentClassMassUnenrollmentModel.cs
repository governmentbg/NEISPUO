namespace MON.Models.StudentModels.Class
{
    using System.Collections.Generic;

    public class StudentClassMassUnenrollmentModel : StudentClassUnenrollmentModel
    {
        public List<int> SelectedStudents { get; set; }
    }
}
