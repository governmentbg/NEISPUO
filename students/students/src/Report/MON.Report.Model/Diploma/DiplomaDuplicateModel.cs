namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DiplomaDuplicateModel : DiplomaModel
    {
        public DiplomaDuplicateModel()
        {

        }

        public DiplomaDuplicateModel(bool dummyData) :
            base(dummyData)
        {
            if (dummyData)
            {
                Original = new DiplomaDocumentOriginal()
                {
                    Series = "СС-22",
                    FactoryNumber = "123456",
                    RegNumberTotal = "1824",
                    RegNumberYear = "32",
                    RegDate = "05.09.2022",
                    Institution = "Частна профилирана гимназия с чуждоезиково обучение \"Меридиан 22\"",
                    InstitutionLocalArea = "р-н Младост",
                    InstitutionRegion = "София-град",
                    InstitutionTown = "София",
                    InstitutionMunicipality = "Столична",
                    InstitutionAddress = "София, р-н Възраждане"
                };
            }
        }

        public DiplomaDocumentOriginal Original { get; set; }
    }

    public class DiplomaDocumentOriginal
    {
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegNumberTotal { get; set; }
        public string RegNumberYear { get; set; }
        public string RegDate { get; set; }
        public string Institution { get; set; }
        public string InstitutionLocalArea { get; set; }
        public string InstitutionRegion { get; set; }
        public string InstitutionTown { get; set; }
        public string InstitutionMunicipality { get; set; }
        public string InstitutionAddress { get; set; }
        public int? BasicDocumentId { get; set; }
        public string BasicDocument { get; set; }
        public bool IsValidation { get; set; }
        // Вид документ: завършване, валидиране или друго
        public string GraduationType { get; set; }
    }
}