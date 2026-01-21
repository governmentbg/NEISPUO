using System.ComponentModel;

namespace Helpdesk.Shared.Enums
{
    public enum StatusEnum
    {
        [Description("Нов")]
        New = 1, 
        [Description("В процес на обработка")]
        Assigned = 2,
        [Description("Разрешен")]
        Resolved = 3
    }
}
