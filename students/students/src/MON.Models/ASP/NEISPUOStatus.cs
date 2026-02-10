using System.ComponentModel;

namespace MON.Models.ASP
{
    public enum NEISPUOStatus : int
    {
        [Description("За преглед")]
        UnderReview = 0,
        [Description("Потвърждавам (прекратяването на помощите/брой дни в ОРЕС)")]
        Confirmed = 1,
        [Description("Отказвам, няма основания")]
        Rejected = 2
    }
}
