namespace MON.Shared.Enums.AspIntegration
{
    using System.ComponentModel;

    public enum SessionInfoTypeEnum
    {
        [Description("ЗАПИТВАНЕ")]
        AspAsking,
        [Description("ПОТВЪРЖДЕНИЕ")]
        AspConfirmation,
        [Description("САНКЦИОНИРАНИ")]
        AspSanctioned,
        [Description("ЗАПИСАНИ_ОСН")]
        MonEnrolledMain,
        [Description("ЗАПИСАНИ_ДОП")]
        MonEnrolledAddition,
        [Description("ЗАПИСАНИ_КОР")]
        MonEnrolledCorrection,
        [Description("ОТСЪСТВИЯ")]
        MonAbsence,
        [Description("ПОТВЪРЖДЕНИЕ")]
        MonConfirmation,
        [Description("ОРЕС")]
        MonOres,
        [Description("НОМ_УЧИЛИЩА")]
        MonNomSchool,
        [Description("НОМ_ФОРМИ")]
        MonNomForm,
        [Description("НОМ_КЛАСОВЕ")]
        MonNomClass
    }
}
