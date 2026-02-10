namespace NeispuoExtension.Services.Core.UserIdentity
{
    using DependencyInjection;

    public interface IUserIdentityService : IScopedService
    {
        public string Username { get; set; }

        public int SysUserId { get; set; }

        public int SysRoleId { get; set; }

        public int InstitutionId { get; set; }
    }
}
