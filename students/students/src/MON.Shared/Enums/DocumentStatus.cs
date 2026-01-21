using System.ComponentModel;

namespace MON.Shared.Enums
{
    public enum DocumentStatus
    {
        [Description("Чернова")]
        Draft = 1,
        [Description("Потвърден")]
        Final = 2
    }
}
