namespace MON.DataAccess
{
    public partial class InstitutionInnovation
    {
        public int InstitutionInnovationId { get; set; }
        public int InstitutionId { get; set; }
        public int InovationTypeId { get; set; }
        public string Notes { get; set; }
        public int? SysUserId { get; set; }

        public virtual InstitutionDetail Institution { get; set; }
    }
}
