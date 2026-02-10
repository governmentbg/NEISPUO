using System.ComponentModel;

namespace MON.Models.ASP
{
    public enum ASPStatusEnum : int
    {
        [Description("Отсъствие")]
        Absence = 0,
        [Description("Отписан")]
        Discharged = 1,
        [Description("ОРЕС")]
        NonVisiting = 2
    }
}
