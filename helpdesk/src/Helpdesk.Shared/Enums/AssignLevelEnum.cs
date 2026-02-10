namespace Helpdesk.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum AssignLevelEnum
    {
        [Description("Без ограничение")]
        All = 0,
        [Description("Назначени на мен")]
        AssignedToMe = 1,
        [Description("Назначени НЕ на мен")]
        NotAssignedToMe = 2,
        [Description("Без назначение")]
        Unassigned = 3
    }
}
