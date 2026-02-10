namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_54_BModel : DiplomaDuplicateModel
    {
        public Diploma_3_54_BModel(bool dummy) : base(dummy)
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

        public List<Competency> Competencies { get; set; }
        public List<DiplomaGradeModel> StateExamQualificationGrades { get; set; }
        public Competency FirstCompetency { get { return Competencies.FirstOrDefault(); } }

        public string RecognizedKnowledge { get; set; }
    }

    public class Competency
    {
        public Competency()
        {
            Code = null;
            Name = null;
        }

        public Competency(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}
