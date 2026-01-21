namespace MON.Shared.Enums
{
    using System.ComponentModel;

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
