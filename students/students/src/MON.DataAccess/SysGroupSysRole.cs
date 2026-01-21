// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MON.DataAccess
{
    public partial class SysGroupSysRole
    {
        public int? SysGroupId { get; set; }
        public int? SysRoleId { get; set; }

        public virtual SysGroup SysGroup { get; set; }
        public virtual SysRole SysRole { get; set; }
    }
}
