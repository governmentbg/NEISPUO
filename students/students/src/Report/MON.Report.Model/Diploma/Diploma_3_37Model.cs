namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_37Model : DiplomaModel
    {

        public Diploma_3_37Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                ProfEduReason = "Разширяване на инструментариума, използван при разработване на ДОС за придобиване на квалификация по професии";
                CourseName = "========== Elapsed 00:03.950 ==========";
                CourseDuration = 321.7m;
                StateExamQualificationGradeText = "Много Добър";
                StateExamQualificationGrade = 5.42m;
                StateExamBasicDocumentPartId = 93;

                StateExamQualificationGrades = new List<DiplomaGradeModel>
                {
                    new DiplomaGradeModel()
                    {
                        SubjectName = "Практика",
                        Grade = 5.32m,
                        GradeText = "Много Добър",
                        DocumentPartId = 93,
                    },
                    new DiplomaGradeModel()
                    {
                        SubjectName = "Теория",
                        Grade = 5.23m,
                        GradeText = "Много Добър",
                        DocumentPartId = 93,
                    }
                };
            }
        }

        public string ProfEduReason { get; set; }
        public string CourseName { get; set; }
        public decimal CourseDuration { get; set; }
        public string StateExamQualificationGradeText { get; set; }
        public decimal? StateExamQualificationGrade { get; set; }
        public int? StateExamBasicDocumentPartId { get; set; }

        public List<DiplomaGradeModel> StateExamQualificationGrades { get; set; }
    }
}
