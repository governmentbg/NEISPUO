namespace MON.Report.Service
{
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Shared.Interfaces;

    public class DbReportServiceDependencies<T> : IScopedService
    {
        public DbReportServiceDependencies(MONContext context, ILogger<T> logger, IUserInfo userInfo)
        {
            Context = context;
            Logger = logger;
            UserInfo = userInfo;
        }

        public MONContext Context { get; }
        public ILogger<T> Logger { get; }
        public IUserInfo UserInfo { get; set; }
    }
}
