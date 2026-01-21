namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_44Model: DiplomaModel
    {
        public Diploma_3_44Model(bool dummy): base(dummy)
        {
            if (dummy)
            {
                FirstHighSchoolLevel = new AdditionalDocumentModel()
                {
                    Series = "Д-20",
                    FactoryNumber = "123456",
                    RegNumberTotal = "1489",
                    RegNumberYear = "156",
                    RegDate = "11.09.2022",
                    Institution = "51 СОУ Иван Вазов",
                    IsRecognition = true
                };

                StateExamQualificationGrade = 4.790m;
                StateExamQualificationGradeText = "Много добър";

                SubjectTypesDetails = new List<SubjectTypeDetail>()
                {
                    new SubjectTypeDetail(){ Name = "ООП", Description= "изучаван като общообразователен предмет" },
                    new SubjectTypeDetail(){ Name = "ПП", Description= "изучаван като профилиращ предмет" },
                };

                ExternalEvaluationGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel()
                    {
                        Id = 187943,
                        SubjectName = "Теория и практика на професията",
                        Grade = 3.90m,
                        Points = 49.50m,
                        SubjectId = 93,
                    }
                };
            }
        }
        
        public decimal? StateExamQualificationGrade { get; set; }
        public string StateExamQualificationGradeText { get; set; }
        public AdditionalDocumentModel FirstHighSchoolLevel { get; set; }
        public List<SubjectTypeDetail> SubjectTypesDetails { get; set; }
        public List<DiplomaGradeModel> ExternalEvaluationGrades { get; set; }
    }
}
