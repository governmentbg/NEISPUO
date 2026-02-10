namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_54_aBModel : DiplomaDuplicateModel
    {
        public Diploma_3_54_aBModel(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                RecognizedProfessionalSkills = "Професионални знания, умения и компетентности";
                DOSRegulationNo = "A-201";
                DOSRegulationDate = "22.11.2022";
                DOSRegulationStateGazetteNo = "Б-301";
                DOSRegulationStateGazetteDate = "23.11.2022";

                Competencies = new List<Competency>()
                {
                    new Competency()
                    {
                        Code = "1001",
                        Name = "Компетентност 1"
                    },
                    new Competency()
                    {
                        Code = "1002",
                        Name = "Компетентност 2"
                    },
                    new Competency()
                    {
                        Code = "1003",
                        Name = "Компетентност 3 - задълбочаване и тестване на компетенциите с цел да излизат по-добре на редовете"
                    },
                    new Competency()
                    {
                        Code = "1004",
                        Name = "Компетентност 4 - задълбочаване и тестване на компетенциите с цел да излизат по-добре на редовете (четвърта част)"
                    },
                    new Competency()
                    {
                        Code = "1005",
                        Name = "Компетентност 5 - задълбочаване и тестване на компетенциите с цел да излизат по-добре на редовете"
                    },
                    new Competency()
                    {
                        Code = "1006",
                        Name = "Компетентност 6 - задълбочаване и тестване на компетенциите с цел да излизат по-добре на редовете (шеста част)"
                    },
                };

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

        // Признати професионални умения
        public string RecognizedProfessionalSkills { get; set; }
        public string DOSRegulationNo { get; set; }
        public string DOSRegulationDate { get; set; }
        public string DOSRegulationStateGazetteNo { get; set; }
        public string DOSRegulationStateGazetteDate { get; set; }
        public decimal? StateExamQualificationGrade { get; set; }
        public string StateExamQualificationGradeText { get; set; }
        public int? StateExamBasicDocumentPartId { get; set; }

        public List<DiplomaGradeModel> StateExamQualificationGrades { get; set; }
        public List<Competency> Competencies { get; set; }
        public string RecognizedKnowledge { get; set; }
    }
}
