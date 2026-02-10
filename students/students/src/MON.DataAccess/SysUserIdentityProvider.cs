// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MON.DataAccess
{
    public partial class SysUserIdentityProvider
    {
        public int SysUserIdentityProviderId { get; set; }
        public int? SysUserId { get; set; }
        public int? IdentityProviderId { get; set; }

        public virtual IdentityProvider IdentityProvider { get; set; }
        public virtual SysUser SysUser { get; set; }
    }
}
