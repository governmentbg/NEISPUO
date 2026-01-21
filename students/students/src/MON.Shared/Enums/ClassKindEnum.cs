namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum ClassKindEnum
    {
        [Description("Групи/Класове")]
        Basic = 1,
        [Description("Групи в ЦДО")]
        Cdo = 2,
        [Description("Други групи")]
        Other = 3,
        [Description("Група за ПОО")]
        Professional = 4

    }
}
