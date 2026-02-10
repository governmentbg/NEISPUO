namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum GradeType
{
    [Description("Текуща оценка")]
    [LocalizationKey("{{GradeType.General}}")]
    General = 1,

    [Description("Контролна работа")]
    [LocalizationKey("{{GradeType.ControlExam}}")]
    ControlExam = 2,

    [Description("Класна работа")]
    [LocalizationKey("{{GradeType.ClassExam}}")]
    ClassExam = 3,

    [Description("Тест")]
    [LocalizationKey("{{GradeType.Test}}")]
    Test = 4,

    [Description("Домашна работа")]
    [LocalizationKey("{{GradeType.Homework}}")]
    Homework = 5,

    [Description("Проект")]
    [LocalizationKey("{{GradeType.Project}}")]
    Project = 6,

    [Description("Входно ниво")]
    [LocalizationKey("{{GradeType.EntryLevel}}")]
    EntryLevel = 11,

    [Description("Изходно ниво")]
    [LocalizationKey("{{GradeType.ExitLevel}}")]
    ExitLevel = 12,

    [Description("Срочна")]
    [LocalizationKey("{{GradeType.Term}}")]
    Term = 21,

    [Description("Годишна")]
    [LocalizationKey("{{GradeType.Final}}")]
    Final = 22,

    [Description("От друг клас")]
    [LocalizationKey("{{GradeType.OtherClass}}")]
    OtherClass = 98,

    [Description("От друго училище")]
    [LocalizationKey("{{GradeType.OtherSchool}}")]
    OtherSchool = 99,

    [Description("Срочна от друг клас")]
    [LocalizationKey("{{GradeType.OtherClassTerm}}")]
    OtherClassTerm = 100,

    [Description("Срочна от друго училище")]
    [LocalizationKey("{{GradeType.OtherSchoolTerm}}")]
    OtherSchoolTerm = 101,
}
