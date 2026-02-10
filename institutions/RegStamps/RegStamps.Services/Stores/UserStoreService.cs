namespace RegStamps.Services.Stores
{
    using Microsoft.AspNetCore.Identity;

    using Models.Stores;
    using RegStamps.Services.Neispuo;
    using System.Threading;
    using System.Threading.Tasks;

    public class UserStoreService : IUserStore<ApplicationUser>
    {
        private readonly INeispuoService neispuoService;

        public UserStoreService(INeispuoService neispuoService)
        {
            this.neispuoService = neispuoService;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var school = await this.neispuoService.GetSchoolInfoAsync(Convert.ToInt32(userId));

            return new ApplicationUser
            {
                SchlMidName = school.SchlMidName,
                SchoolId = school.SchoolId,
            };
        }

        public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var schools = await this.neispuoService.GetAllSchoolsAsync();

            var school = schools.Where(x => x.SchlMidName == normalizedUserName).FirstOrDefault();

            if (school is null)
            {
                throw new ArgumentNullException(nameof(school));
            }

            return new ApplicationUser
            {
                SchlMidName = school.SchlMidName,
                SchoolId = school.SchoolId,
            };
        }

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.SchlMidName);

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.SchoolId.ToString());
        

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user?.SchlMidName);

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.SchlMidName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            user.SchlMidName = userName;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
