namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum GradeResultType
{
    [Description("Завършва")]
    [LocalizationKey("{{GradeResultType.CompletesGrade}}")]
    CompletesGrade = 1,

    [Description("Полага изпити")]
    [LocalizationKey("{{GradeResultType.MustRetakeExams}}")]
    MustRetakeExams = 2,

    [Description("Повтаря")]
    [LocalizationKey("{{GradeResultType.RepeatsGrade}}")]
    RepeatsGrade = 3,
}
