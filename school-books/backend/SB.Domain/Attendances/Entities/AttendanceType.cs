namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum AttendanceType
{
    [Description("Присъствие")]
    [LocalizationKey("{{AttendanceType.Presence}}")]
    Presence = 1,

    [Description("Отсъствие по неуважителни причини")]
    [LocalizationKey("{{AttendanceType.UnexcusedAbsence}}")]
    UnexcusedAbsence = 2,

    [Description("Отсъствие по уважителни причини")]
    [LocalizationKey("{{AttendanceType.ExcusedAbsence}}")]
    ExcusedAbsence = 3,
}
