namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum PersonalIdTypeEnum
    {
        [Description("Без идентификатор")]
        NoID = -1,
        [Description("ЕГН")]
        EGN = 0,
        [Description("ЛНЧ")]
        LNCH = 1,
        [Description("ИДН")]
        IDN = 2
    }
}
