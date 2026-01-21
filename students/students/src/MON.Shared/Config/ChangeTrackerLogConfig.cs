using System.Collections.Generic;

namespace MON.Shared
{
    public class ChangeTrackerLogConfig
    {
        public bool Active { get; set; }
        public HashSet<string> SkippedTables { get; set; }
        public HashSet<string> SkippedColumns { get; set; }
        public HashSet<string> GridHiddenModules { get; set; }
        public HashSet<string> GridHiddenTables { get; set; }
    }
}
