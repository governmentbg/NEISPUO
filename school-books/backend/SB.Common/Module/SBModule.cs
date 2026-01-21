namespace SB.Common;

using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public abstract class SBModule
{
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public virtual void ConfigureAutofacServices(ContainerBuilder builder, IConfiguration configuration)
    {
    }
}
