using System.ComponentModel;

namespace Helpdesk.Shared.Enums
{
    public enum PriorityEnum
    {
        [Description("Нисък")]
        Low = 1, 
        [Description("Среден")]
        Medium = 2,
        [Description("Висок")]
        High = 3
    }
}
