namespace MON.Shared.Enums.SchoolBooks
{
    using System.ComponentModel;

    public enum GradeTypeEnum
    {
        [Description("Текуща оценка")]
        General = 1,

        [Description("Контролна работа")]
        ControlExam = 2,

        [Description("Класна работа")]
        ClassExam = 3,

        [Description("Тест")]
        Test = 4,

        [Description("Домашна работа")]
        Homework = 5,

        [Description("Проект")]
        Project = 6,

        [Description("Входно ниво")]
        EntryLevel = 11,

        [Description("Изходно ниво")]
        ExitLevel = 12,

        [Description("Срочна")]
        Term = 21,

        [Description("Годишна")]
        Final = 22,

        [Description("От друг клас")]
        OtherClass = 98,

        [Description("От друго училище")]
        OtherSchool = 99,

        [Description("Срочна от друг клас")]
        OtherClassTerm = 100,

        [Description("Срочна от друго училище")]
        OtherSchoolTerm = 101,
    }
}
