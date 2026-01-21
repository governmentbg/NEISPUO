namespace Helpdesk.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
