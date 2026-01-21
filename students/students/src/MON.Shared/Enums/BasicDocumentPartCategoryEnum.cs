namespace MON.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public enum BasicDocumentPartCategoryEnum
    {
        [Description("Mandatory Part")]
        MandatoryPart = 1,
        [Description("ZIPProf Part")]
        ZIPProfPart = 2,
        [Description("ZIPNoProf Part")]
        ZIPNoProfPart = 3,
        [Description("SIP Part")]
        SIPPart = 4,
        [Description("Elective Part")]
        ElectivePart = 5,
        [Description("Optional Part")]
        OptionalPart = 6,
        [Description("MandatoryDZI Part")]
        MandatoryDZIPart = 7,
        [Description("AdditionalDZI Part")]
        AdditionalDZIPart = 8,
        [Description("Faculty Part")]
        FacultyPart = 9,
        [Description("NVO Part")]
        NVOPart = 10,
    }
}
