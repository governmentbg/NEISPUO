namespace MON.Shared.Enums.AspIntegration
{
    using System.ComponentModel;

    public enum MonConfirmationFlagEnum
    {
        [Description("НЯМА ИНФОРМАЦИЯ")]
        NoInformation = 0,
        [Description("ПОТВЪРЖДАВАМ")]
        Confirm = 1,
        [Description("ОТКАЗВАМ, НЯМА ОСНОВАНИЯ")]
        Refuse = 2
    }
}
