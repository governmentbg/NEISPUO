namespace MON.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public enum ValidEnum
    {
        [Description("Невалидни")]
        False,
        [Description("Валидни")]
        True,
        [Description("Всички")]
        All
    }
}
