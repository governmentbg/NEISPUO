namespace MON.Models.Administration
{
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    public class DataReferencesViewModel
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string EntityId { get; set; }
        public int TakeTop { get; set; }
        public bool OnlyWithDependecies { get; set; }
        public List<ReferencesViewModel> Result { get; set; }
    }

    public class ReferencesViewModel
    {
        public string ReferencingTableSchema { get; set; }
        public string ReferencingTableName { get; set; }
        public string ReferencingColumnName { get; set; }
        public string ReferencedTableSchema { get; set; }
        public string ReferencedTableName { get; set; }
        public string TargetEntityId { get; set; }
        public int TakeTop { get; set; }
        public bool OnlyWithDependecies { get; set; }
        public bool HasDependencies { get; set; }
        public int Count { get; set; }
        public JArray Dependencies { get; set; }
    }
}
