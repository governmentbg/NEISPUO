namespace MON.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public enum PageOrientationEnum
    {
        [Description("Неспецифициран")]
        Unspecified = 0,
        [Description("Портрет")]
        Portrait = 1,
        [Description("Пейзаж")]
        Landscape = 2
    }
}
