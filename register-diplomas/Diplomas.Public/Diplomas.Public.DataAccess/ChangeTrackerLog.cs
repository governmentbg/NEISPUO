using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class ChangeTrackerLog
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public int? UserId { get; set; }
        public string RowId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ModuleName { get; set; }
        public string Activity { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string ChangeTrackerState { get; set; }
    }
}
