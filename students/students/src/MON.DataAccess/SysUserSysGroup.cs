// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MON.DataAccess
{
    public partial class SysUserSysGroup
    {
        public int? SysUserId { get; set; }
        public int? SysGroupId { get; set; }

        public virtual SysGroup SysGroup { get; set; }
        public virtual SysUser SysUser { get; set; }
    }
}
