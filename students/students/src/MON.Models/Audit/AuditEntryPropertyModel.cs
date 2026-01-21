namespace MON.Models.Audit
{
    public class AuditEntryPropertyModel
    {
        public string RelationName { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool IsKey { get; set; }
    }
}
