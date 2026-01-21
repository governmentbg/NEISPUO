namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum RefugeeProtectionStatusEnum
    {
        [Description("Международна защита")]
        InternationalProtection = 1,
        [Description("Временна защита")]
        TemporaryProtection = 2,
        [Description("Без защита")]
        NoProtection = 3
    }
}
