namespace MON.DataAccess
{
    public partial class InstitutionTrustBoard
    {
        public int InstitutionTrustBoardId { get; set; }
        public int InstitutionId { get; set; }
        public int RegistryStatusId { get; set; }
        public string Name { get; set; }
        public int? EstablishedYear { get; set; }
        public string DocumentHistory { get; set; }
        public string ChairmanFirstName { get; set; }
        public string ChairmanMiddleName { get; set; }
        public string ChairmanFamilyName { get; set; }
        public string ChairmanEmail { get; set; }
        public string ChairmanPhone { get; set; }
        public int? TownId { get; set; }
        public int? SysUserId { get; set; }

        public virtual InstitutionDetail Institution { get; set; }
        public virtual Town Town { get; set; }
    }
}
