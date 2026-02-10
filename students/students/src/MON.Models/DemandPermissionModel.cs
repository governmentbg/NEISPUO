namespace MON.Models
{
    public class DemandPermissionModel
    {
        public string Permission { get; set; }
        public int? StudentId { get; set; }
        public int? InstitutionId { get; set; }
        public int? ClassId { get; set; }
    }
}
