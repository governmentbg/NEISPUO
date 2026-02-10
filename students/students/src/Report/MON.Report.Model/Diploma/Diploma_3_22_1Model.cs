namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_22_1Model : DiplomaModel
    {
        public Diploma_3_22_1Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                HighSchoolLevel = "втори";
            }
        }

        // Завършен гимназиален етап
        public string HighSchoolLevel { get; set; }
    }
}
