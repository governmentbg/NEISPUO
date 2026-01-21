namespace MON.DataAccess
{
    public partial class ClassStudent
    {
        public int Id { get; set; }
        public int SchoolYear { get; set; }
        public int ClassId { get; set; }
        public int PersonId { get; set; }
        public int? StudentSpecialityId { get; set; }
        public int StudentEduFormId { get; set; }
        public int ClassNumber { get; set; }
        public int Status { get; set; }

        public virtual Person Person { get; set; }
    }
}
