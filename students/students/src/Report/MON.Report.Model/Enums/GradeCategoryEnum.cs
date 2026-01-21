namespace MON.Report.Model.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public enum GradeCategoryEnum
    {
        [Description("Рубрика")]
        SubSection = -1,
        [Description("Нормална оценка")]
        Normal = 1,
        [Description("СОП оценка")]
        SpecialNeeds = 2,
        [Description("Друга оценка")]
        Other = 3,
        [Description("Качествена оценка")]
        Qualitative = 4,
    }
}
