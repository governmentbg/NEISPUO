using System.ComponentModel;

namespace Helpdesk.Shared.Enums
{
    public enum PermissionEnum
    {
        [Description("Създаване")]
        Create = 0,
        [Description("Четене")]
        Read = 1, 
        [Description("Промяна")]
        Update = 2,
        [Description("Изтриване")]
        Delete = 4,
        [Description("Повторно отваряне")]
        Reopen = 5
    }
}
