namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum SpecialNeedsGrade
{
    [Description("среща затруднения")]
    [LocalizationKey("среща затруднения")]
    HasDificulty = 1,

    [Description("справя се")]
    [LocalizationKey("справя се")]
    DoingOk = 2,

    [Description("постига изискванията")]
    [LocalizationKey("постига изискванията")]
    MeetsExpectations = 3,
}
