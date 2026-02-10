namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_19Model : DiplomaModel
    {
        public Diploma_3_19Model(bool dummy) : base(dummy) 
        {
            if (dummy)
            {
                PreSchoolOrganization = "Целодневна";
            }
        }

        public string PreSchoolOrganization { get; set; }
    }
}
