namespace MonProjects.Services
{
    using Configurations;
    using Microsoft.Extensions.Options;
    using MsSql;

    public class MsSqlNeispuoService : MsSqlService, IMsSqlNeispuoService
    {
        public MsSqlNeispuoService(IOptions<AppSettings> options) 
            : base(options?.Value.Database.Neispuo)
        {
        }
    }
}
