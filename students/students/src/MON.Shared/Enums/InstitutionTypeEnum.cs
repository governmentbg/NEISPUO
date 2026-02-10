namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum InstitutionTypeEnum
    {
        [Description("Училище")]
        School = 1,
        [Description("Детска градина")]
        KinderGarden = 2,
        [Description("Център за подкрепа за личностно развитие")]
        PersonalDevelopmentSupportCenter = 3,
        [Description("Център за специална образователна подкрепа")]
        CenterForSpecialEducationalSupport = 4,
        [Description("Специализирано обслужващо звено")]
        SpecializedServiceUnit = 5
    }
}
