using System.ComponentModel;

namespace MON.Shared.Enums
{
    public enum ExamTypeEnum
    {
        [Description("Писмен")]
        Written = 1,
        [Description("Устен")]
        Oral = 2,
        [Description("Практически")]
        Practical = 3,
        [Description("Комбинация от писмена и устна част")]
        WrittenOral = 4,
        [Description("Комбинация от писмена и практическа част")]
        WrittenPractical = 5,
        [Description("Изпит за определяне на срочна оценка")]
        TermGrade = 6,
        [Description("Изпит за определяне на крайна оценка")]
        FinalGrade = 7,
        [Description("Изпит за определяне на годишна оценка")]
        AnnualGrade = 8
    }
}
