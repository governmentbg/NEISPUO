namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum ReassessmentTypeEnum
    {
        [Description("изпит за промяна на годишна оценка (чл. 34, ал. 2 на Наредба 11/2016 г.)")]
        AnualGrade = 1,
        [Description("изпит за промяна на окончателна оценка за I ГЕ (чл. 34, ал. 3 на Наредба 11/2016 г.)")]
        FirstHighSchoolStage = 2,
        [Description("изпит за промяна на окончателна оценка за II ГЕ (чл. 34, ал. 3 на Наредба 11/2016 г.)")]
        SecondHighSchoolStage = 3
    }
}
