namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum AbsenceType
{
    [Description("Закъснение")]
    [LocalizationKey("{{AbsenceType.Late}}")]
    Late = 1,

    [Description("Отсъствие по неуважителни причини")]
    [LocalizationKey("{{AbsenceType.Unexcused}}")]
    Unexcused = 2,

    [Description("Отсъствие по уважителни причини")]
    [LocalizationKey("{{AbsenceType.Excused}}")]
    Excused = 3,

    [Description("Отсъствие")]
    [LocalizationKey("{{AbsenceType.DplrAbsence}}")]
    DplrAbsence = 4,

    [Description("Присъствие")]
    [LocalizationKey("{{AbsenceType.DplrAttendance}}")]
    DplrAttendance = 5,
}
