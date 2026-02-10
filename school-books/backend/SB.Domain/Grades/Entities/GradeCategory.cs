namespace SB.Domain;

using SB.Common;

public enum GradeCategory
{
    [LocalizationKey("Количествена оценка")]
    Decimal = 1,

    [LocalizationKey("СОП оценка")]
    SpecialNeeds = 2,

    [LocalizationKey("Качествена оценка")]
    Qualitative = 3,
}
