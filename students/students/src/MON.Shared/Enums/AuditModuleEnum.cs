namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum AuditModuleEnum
    {
        [Description("Деца и ученици")]
        Students = 201,
        [Description("Регистър Дипломи")]
        Diplomas = 202,
        [Description("Мобилно приложение")]
        MobileApp = 203,
        [Description("Система за поддръжка")]
        Helpdesk = 204
    }
}
