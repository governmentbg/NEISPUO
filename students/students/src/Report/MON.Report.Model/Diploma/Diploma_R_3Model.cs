namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_R_3Model: DiplomaModel
    {
        public Diploma_R_3Model(bool dummy): base(dummy)
        {
            if (dummy)
            {
                RUO = "Пазарджик";
                LevelOfGraduation = "Завършен първи гимназиален етап";
                PersonFullIdentifier = "ЕГН 1234567890, 18.02.2010, българин";
                ExternalDocNumber = "A102-18";
                ExternalDocDate = "27.02.2024";
                IssuedBy = "London School Of Economics";
            }
        }

        public string RUO { get; set; }
        public string LevelOfGraduation { get; set; }
        public string PersonFullIdentifier { get; set; }
        public string ExternalDocType { get; set; }
        public string ExternalDocNumber { get; set; }
        public string ExternalDocDate { get; set; }
        public string ExternalDocYear { get; set; }
        public string IssuedBy { get; set; }
    }
}
