namespace MON.DataAccess
{
    public partial class InstitutionProjectPartner
    {
        public int InstitutionProjectPatnerId { get; set; }
        public int? InstitutionProjectId { get; set; }
        public int? ProjectPartnerTypeId { get; set; }
        public string Eik { get; set; }
        public string Name { get; set; }
        public bool? IsCoordinator { get; set; }
        public int? SysUserId { get; set; }

        public virtual InstitutionProject InstitutionProject { get; set; }
    }
}
