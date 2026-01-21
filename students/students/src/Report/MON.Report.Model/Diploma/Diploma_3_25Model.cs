namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_25Model : DiplomaModel
    {
        public Diploma_3_25Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                MandatoryGrades = Grades;
                ElectiveGrades = Grades;
                OptionalGrades = Grades;
                DiplomaFilteredGrades = Grades;
                MandatoryGradesWithoutFL = MandatoryGrades.Where(i => i.SubjectId < 100 || i.SubjectId > 150).ToList();
            }
        }

        public List<DiplomaGradeModel> MandatoryGradesWithoutFL { get; set; }
        public List<DiplomaGradeModel> ElectiveGrades { get; set; }
        public List<DiplomaGradeModel> OptionalGrades { get; set; }
        public List<DiplomaGradeModel> DiplomaFilteredGrades { get; set; }

        public int? ElectiveBasicDocumentPartId { get; set; }
        public int? OptionalBasicDocumentPartId { get; set; }
    }
}
