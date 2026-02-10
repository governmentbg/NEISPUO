namespace MON.Models
{
    public class ClassGroupCacheModel
    {
        public int? RegionId { get; set; }
        public int? ParentClassId { get; set; }
        public int InstitutionId { get; set; }
        public bool IsValid { get; set; }
        public int? ClassTypeId { get; set; }
    }
}
