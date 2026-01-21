namespace MON.Services
{
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;

    public class DbServiceDependencies<T>
    {
        public DbServiceDependencies(MONContext context,
            IUserInfo currentUserInfo,
            ILogger<T> logger,
            INeispuoAuthorizationService authorizationService)
        {
            Context = context;
            CurrentUserInfo = currentUserInfo;
            Logger = logger;
            AuthorizationService = authorizationService;
        }

        public MONContext Context { get; }
        public IUserInfo CurrentUserInfo { get; }
        public ILogger<T> Logger { get; }
        public INeispuoAuthorizationService AuthorizationService { get; }
    }
}
