namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_44_AModel : DiplomaDuplicateModel
    {
        public Diploma_3_44_AModel(bool dummy) : base(dummy) 
        {
            if (dummy)
            {
                CommissionType = "Тестова комисия за диплома 3-44-А";
                Qualification = "Професионална тестова квалификация за тестване на дипломи";

                FirstHighSchoolLevel = new DiplomaDocumentOriginal()
                {
                    Series = "Д-20",
                    FactoryNumber = "123456",
                    RegNumberTotal = "1489",
                    RegNumberYear = "156",
                    RegDate = "11.09.2022",
                    Institution = "51 СОУ Иван Вазов"
                };
            }
        }

        public DiplomaDocumentOriginal FirstHighSchoolLevel { get; set; }
        public string CommissionType { get; set; } 
        public string Qualification { get; set; }
    }
}
