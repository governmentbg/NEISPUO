namespace MON.DataAccess.Dto
{
    public class DataReferencesResult
    {
        public string ReferencingTableSchema { get; set; }
        public string ReferencingTableName { get; set; }
        public string ReferencingColumnName { get; set; }
        public string ReferencedTableSchema { get; set; }
        public string ReferencedTableName { get; set; }
        public string TargetEntityId { get; set; }
        public int Count { get; set; }
        public string JsonStr { get; set; }
    }
}
