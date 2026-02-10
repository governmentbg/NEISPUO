namespace SB.Domain;

using System.ComponentModel;

public enum StudentClassStatus
{
    [Description("Записан")]
    Enrolled = 1,

    [Description("Отписан")]
    Transferred = 2,
}
