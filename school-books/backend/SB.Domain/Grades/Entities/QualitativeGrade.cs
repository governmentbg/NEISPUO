namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum QualitativeGrade
{
    [Description("Незадоволителен")]
    [LocalizationKey("Незадоволителен")]
    Poor = 2,

    [Description("Среден")]
    [LocalizationKey("Среден")]
    Fair = 3,

    [Description("Добър")]
    [LocalizationKey("Добър")]
    Good = 4,

    [Description("Мн.Добър")]
    [LocalizationKey("Мн.Добър")]
    VeryGood = 5,

    [Description("Отличен")]
    [LocalizationKey("Отличен")]
    Excellent = 6,
}
