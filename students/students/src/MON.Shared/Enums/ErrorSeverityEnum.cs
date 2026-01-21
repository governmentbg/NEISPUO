namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum ErrorSeverityEnum
    {
        [Description("Подробен")]
        Debug = 0,
        [Description("Информация")]
        Info = 1,
        [Description("Предупреждение")]
        Warning = 2,
        [Description("Грешка")]
        Error = 3,
        [Description("CSP Грешка")]
        CspError = 101
    }
}
