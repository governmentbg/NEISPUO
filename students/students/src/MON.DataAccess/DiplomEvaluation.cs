namespace MON.DataAccess
{
    public partial class DiplomEvaluation
    {
        public int Id { get; set; }
        public decimal? Evaluation { get; set; }
        public bool? HasPassed { get; set; }
        public int? ClassId { get; set; }
        public int? SysUserId { get; set; }

        public virtual Class Class { get; set; }
        public virtual SysUser SysUser { get; set; }
    }
}
