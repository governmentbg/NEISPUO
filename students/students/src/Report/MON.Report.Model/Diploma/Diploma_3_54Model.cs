namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_54Model: DiplomaModel
    {
        public Diploma_3_54Model(bool dummy): base(dummy)
        {
            if (dummy)
            {
                StateExamQualificationGrade = 4.790m;
                StateExamQualificationGradeText = "Много добър";

                StateExamQualificationGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        SubjectName = "Практика",
                        GradeText = "Отличен",
                        Grade = 5.50M
                    },
                    new DiplomaGradeModel()
                    {
                        SubjectName = "Теория",
                        GradeText = "Много добът",
                        Grade = 5.13M
                    }
                };
            }
        }
       
        public decimal? StateExamQualificationGrade { get; set; }
        public string StateExamQualificationGradeText { get; set; }
        public int? StateExamBasicDocumentPartId { get; set; }
        public List<DiplomaGradeModel> StateExamQualificationGrades { get; set; }
    }
}
