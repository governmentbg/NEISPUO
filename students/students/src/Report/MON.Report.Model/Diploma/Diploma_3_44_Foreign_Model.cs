namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    
    /// <summary>
    /// Модел за приложенията на чужд език към Диплома 3-44 (използва се за всичките - 'multi-purpose')
    /// </summary>
    public class Diploma_3_44_Foreign_Model : DiplomaModel
    {
        public Diploma_3_44_Foreign_Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                FullNameLatin = "Vasil Ivanov Kunchev";
                ProfessionForeign = "Technician of hoisting and lifting equipment";
                SpecialityForeign = "Violin";
                GpaTextForeign = "Very Good";
                Ministry = "Ministry of Education and Science";
                NationalityForeign = "Bulgaria";
                Principal = "Velichka Prisova";
                Deputy = "Velichka Prisova"; 
                InstitutionRegionForeign = "Lovech";
                InstitutionMunicipalityForeign = "Kaloferski Iztok";
                InstitutionTownForeign = "Kalofer";
                InstitutionLocalAreaForeign = "Kalofer";
                InstitutionForeign = "'35' SOU Dobri Voinikov'";
                EduFormForeign = "daily";
                ClassType = "Profile: Mathematics";
                StateExamQualificationGrade = 5.57m;
                StateExamQualificationGradeText = "Excellent";

                FirstHighSchoolLevel = new AdditionalDocumentModel()
                {
                    Series = "Д-20",
                    FactoryNumber = "123456",
                    RegNumberTotal = "1489",
                    RegNumberYear = "156",
                    RegDate = "11.09.2022",
                    Institution = "Sofia High School of Mathematics"
                };

                SubjectTypesDetails = new List<SubjectTypeDetail>()
                {
                    new SubjectTypeDetail(){ Name = "GES", Description= "taught as general education subject" },
                    new SubjectTypeDetail(){ Name = "PS", Description= "studied as a profiling subject" },
                };
            }
        }

        public string SpecialityForeign { get; set; }
        public string ProfessionForeign { get; set; }
        public string GpaTextForeign { get; set; }
        public string FullNameLatin { get; set; }
        public decimal? StateExamQualificationGrade { get; set; }
        public string StateExamQualificationGradeText { get; set; }
        public string NationalityForeign { get; set; }
        public string EduFormForeign { get; set; }
        public string FirstNameLatin { get; set; }
        public string MiddleNameLatin { get; set; }
        public string LastNameLatin { get; set; }

        public List<SubjectTypeDetail> SubjectTypesDetails { get; set; }
        public AdditionalDocumentModel FirstHighSchoolLevel { get; set; }
    }
}
