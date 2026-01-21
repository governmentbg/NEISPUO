using System.ComponentModel;

namespace MON.Models.Enums
{
    public enum StudentClassStatus
    {
        [Description("Записан")]
        Enrolled = 1,
        [Description("Преместен")]
        Transferred = 2,
        [Description("Отписан")]
        Discharged = 3,
        [Description("Не е открит")]
        NotFound = 4
    }
}
