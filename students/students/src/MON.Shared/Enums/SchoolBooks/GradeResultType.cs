namespace MON.Shared.Enums.SchoolBooks
{
    using System.ComponentModel;

    public enum GradeResultType
    {
        [Description("Завършва")]
        CompletesGrade = 1,
        [Description("Полага изпити")]
        MustRetakeExams = 2,
        [Description("Повтаря")]
        RepeatsGrade = 3,
    }
}
