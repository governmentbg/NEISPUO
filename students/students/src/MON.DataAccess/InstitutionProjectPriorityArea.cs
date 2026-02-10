namespace MON.DataAccess
{
    public partial class InstitutionProjectPriorityArea
    {
        public int InstitutionProjectPriorityAreaId { get; set; }
        public int ProjectId { get; set; }
        public int ProjectPriorityAreaTypeId { get; set; }
        public int? SysUserId { get; set; }

        public virtual InstitutionProject Project { get; set; }
    }
}
