using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MON.DataAccess
{
    public partial class IdentityProvider
    {
        public IdentityProvider()
        {
            SysUserIdentityProviders = new HashSet<SysUserIdentityProvider>();
        }

        public int IdentityProviderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<SysUserIdentityProvider> SysUserIdentityProviders { get; set; }
    }
}
