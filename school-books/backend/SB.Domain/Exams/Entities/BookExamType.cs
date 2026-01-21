namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum BookExamType
{
    [Description("Контролна работа")]
    [LocalizationKey("{{BookExamType.ControlExam}}")]
    ControlExam = 1,

    [Description("Класна работа")]
    [LocalizationKey("{{BookExamType.ClassExam}}")]
    ClassExam = 2
}
