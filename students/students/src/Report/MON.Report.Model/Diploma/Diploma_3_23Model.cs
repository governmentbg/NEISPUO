namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_23Model : DiplomaModel
    {
        public Diploma_3_23Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                MandatoryGrades = Grades;
                ElectiveGrades = Grades;
                OptionalGrades = Grades;
                MandatoryGradesWithoutFL = MandatoryGrades.Where(i => i.SubjectId < 100 || i.SubjectId > 150).ToList();
                DiplomaFilteredGrades = Grades;
                BasicClassRomeNameNext = "X";
            }
        }

        public List<DiplomaGradeModel> MandatoryGradesWithoutFL { get; set; }
        public List<DiplomaGradeModel> ElectiveGrades { get; set; }
        public List<DiplomaGradeModel> OptionalGrades { get; set; }
        public List<DiplomaGradeModel> DiplomaFilteredGrades { get; set; }

        public string BasicClassRomeNameNext { get; set; }
        public int? OptionalBasicDocumentPartId { get; set; }
        public int? ElectiveBasicDocumentPartId { get; set; }
    }
}
