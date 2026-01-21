namespace MON.DataAccess
{
    public partial class Diplom
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] Contents { get; set; }
        public int TemplateId { get; set; }
        public int? SysUserId { get; set; }

        public virtual SysUser SysUser { get; set; }
        public virtual Template Template { get; set; }
    }
}
