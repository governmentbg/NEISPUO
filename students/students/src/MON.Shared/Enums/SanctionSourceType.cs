namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum SanctionSourceType
    {
        [Description("Импорт на санкции от дневник")]
        SchoolBook = 1,
        [Description("Ръчно въвеждане")]
        Manual = 2
    }
}
