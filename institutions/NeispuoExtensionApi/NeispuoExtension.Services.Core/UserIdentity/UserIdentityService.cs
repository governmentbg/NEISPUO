namespace NeispuoExtension.Services.Core.UserIdentity
{
    public class UserIdentityService : IUserIdentityService
    {
        public string Username { get; set; }

        public int SysUserId { get; set; }

        public int SysRoleId { get; set; }

        public int InstitutionId { get; set; }
    }
}
