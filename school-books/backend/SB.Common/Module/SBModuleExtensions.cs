namespace SB.Common;

using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

public static class SBModuleExtensions
{
    private sealed class ModuleHelper : Module
    {
        private IConfiguration configuration;
        private Action<ContainerBuilder, IConfiguration> configureServices;
        public ModuleHelper(
            IConfiguration configuration,
            Action<ContainerBuilder, IConfiguration> configureServices)
            => (this.configuration, this.configureServices) = (configuration, configureServices);

        protected override void Load(ContainerBuilder builder)
        {
            this.configureServices(builder, this.configuration);
        }
    }

    public static void RegisterModules(this IServiceCollection services, IConfiguration configuration, params SBModule[] modules)
    {
        foreach (var module in modules)
        {
            module.ConfigureServices(services, configuration);
        }
    }

    public static void RegisterModules(this ContainerBuilder builder, IConfiguration configuration, params SBModule[] modules)
    {
        foreach (var module in modules)
        {
            builder.RegisterModule(
                new ModuleHelper(
                    configuration,
                    (builder, configuration) => module.ConfigureAutofacServices(builder, configuration)));
        }
    }
}
