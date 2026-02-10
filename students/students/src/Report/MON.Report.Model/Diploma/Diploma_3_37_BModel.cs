namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_37_BModel : DiplomaDuplicateModel
    {
        public Diploma_3_37_BModel(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                RecognizedProfessionalSkills = "Професионални знания, умения и компетентности";
                DOSRegulationNo = "A-201";
                DOSRegulationDate = "22.11.2022";
                DOSRegulationStateGazetteNo = "Б-301";
                DOSRegulationStateGazetteDate = "23.11.2022";

                ExamGrade = 5.15m;
                ExamGradeText = "Много добър";

                ValidationOrderNumber = "В-101";
                ValidationOrderDate = "24.11.2022";

                AmmendmentOrderNumber = "Г-501";
                AmmendmentOrderDate = "25.11.2022";
                RecognizedKnowledge = "Признат резултат от ученето";

                ExamGrades = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel
                    {
                        SubjectName = "Практика",
                        GradeText = "Много Добър",
                        Grade = 5.00m
                    },

                    new DiplomaGradeModel
                    {
                        SubjectName = "Теория",
                        GradeText = "Много Добър",
                        Grade = 5.47m
                    }
                };
            }
        }

        // Признати професионални умения
        public string RecognizedProfessionalSkills { get; set; }
        public string DOSRegulationNo { get; set; }
        public string DOSRegulationDate { get; set; }
        public string DOSRegulationStateGazetteNo { get; set; }
        public string DOSRegulationStateGazetteDate { get; set; }
        // Количествен показател
        public decimal ExamGrade { get; set; }
        // Качествен показател
        public string ExamGradeText { get; set; }
        // Заповед номер
        public string ValidationOrderNumber { get; set; }
        // Заповед дата
        public string ValidationOrderDate { get; set; }
        // Изменение номер
        public string AmmendmentOrderNumber { get; set; }
        // Изменение дата
        public string AmmendmentOrderDate { get; set; }
        public string RecognizedKnowledge { get; set; }

        public List<DiplomaGradeModel> ExamGrades { get; set; }

    }
}
