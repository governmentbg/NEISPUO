namespace MON.Shared.Enums.SchoolBooks
{
    using System.ComponentModel;
    public enum AbsenceType
    {
        [Description("Закъснение")]
        Late = 1,

        [Description("Отсъствие по неуважителни причини")]
        Unexcused = 2,

        [Description("Отсъствие по уважителни причини")]
        Excused = 3,
    }
}
