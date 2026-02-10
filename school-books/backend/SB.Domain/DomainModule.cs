namespace SB.Domain;

using System;
using System.Data;
using System.Reflection;
using Autofac;
using Autofac.Features.AttributeFilters;
using FluentValidation;
using Lib.Net.Http.WebPush;
using Medallion.Threading;
using Medallion.Threading.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Polly;
using SB.Common;
using ZiggyCreatures.Caching.Fusion;

public class DomainModule : SBModule
{
    private Assembly domainAssembly = typeof(DomainModule).GetTypeInfo().Assembly;
    public const string DomainEmbeddedFileProviderRegistrationName = "DomainEmbeddedFileProvider";

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DomainOptions>()
            .Bind(configuration.GetSection("SB:Domain"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        DomainOptions domainOptions = new();
        configuration.GetSection("SB:Domain").Bind(domainOptions);

        services.AddTransient<BlobPublicUrlCreator>();
        services.AddTransient<ISchoolYearSettingsProvider, SchoolYearSettingsProvider>();
        services.AddTransient<ITopicPlanItemsExcelReaderWriter, TopicPlanItemsExcelReaderWriter>();

        // Mediatr
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(this.domainAssembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehaviour<,>));

        // FluentValidation
        services.AddValidatorsFromAssembly(
            this.domainAssembly,
            // exclude the CreateHisMedicalNoticesCommandValidator as we are
            // running it manually inside the CreateHisMedicalNoticesCommandHandler
            filter: t => t.ValidatorType != typeof(CreateHisMedicalNoticesCommandValidator));

        // FusionCache
        services.AddFusionCacheMetrics();
        services.AddStackExchangeRedisCache(options => {
            options.Configuration = domainOptions.RedisConnectionString;
        });
        services.AddFusionCacheSystemTextJsonSerializer();
        services.AddFusionCacheStackExchangeRedisBackplane(options => {
            options.Configuration = domainOptions.RedisConnectionString;
        });
        services.AddMemoryCache(options => {
            // as the size is an approximation and is not even considering
            // the size of the string used as a key, the MemoryCacheEntry
            // and other MemoryCache internals, in reality the size of the cache
            // could be at least 20-30% more when this limit is reached
            // so it should not be considered as a hard limit
            options.SizeLimit = domainOptions.MemoryCacheSizeLimitMB * 1024 * 1024;
            options.CompactionPercentage = 0.5;
            options.TrackStatistics = true;
        });
        services.AddFusionCache()
            .WithOptions(options =>
            {
                // disable the prepending of "v1" to all keys
                options.DistributedCacheKeyModifierMode = CacheKeyModifierMode.None;
            })
            .WithAllRegisteredPlugins()
            .WithRegisteredMemoryCache()
            .WithRegisteredSerializer()
            .WithRegisteredDistributedCache()
            .WithRegisteredBackplane();

        services.AddHttpClient<BlobServiceClient>(
            httpClient =>
            {
                httpClient.BaseAddress = new Uri(
                    configuration.GetValue<string?>("SB:Domain:BlobServicePublicUrl")
                    ?? throw new Exception($"Missing setting SB:Domain:BlobServicePublicUrl"));
            })
            .AddTransientHttpErrorPolicy(
                p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

        services.AddHttpClient<PushServiceClient>();

        services
            .AddMvcCore()
            .AddRazorViewEngine()
            .AddRazorRuntimeCompilation();
        services
            .AddOptions<MvcRazorRuntimeCompilationOptions>()
            .Configure<ILifetimeScope>((opts, lifetimeScope) =>
            {
                var embeddedFileProvider = lifetimeScope.ResolveKeyed<IFileProvider>(DomainEmbeddedFileProviderRegistrationName);
                opts.FileProviders.Add(embeddedFileProvider);
            });

        services.AddScoped<IViewRender, ViewRender>();
    }

    public override void ConfigureAutofacServices(ContainerBuilder builder, IConfiguration configuration)
    {
        builder.RegisterAssemblyTypes(this.domainAssembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(this.domainAssembly)
            .Where(t => t.Name.EndsWith("RedisRepository"))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(this.domainAssembly)
            .Where(t => t.Name.EndsWith("CachedQueryStore"))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        builder
            .Register(c => new ManifestEmbeddedFileProvider(this.domainAssembly))
            .Keyed<IFileProvider>(DomainEmbeddedFileProviderRegistrationName);

        builder.RegisterType<WordTemplateService>().As<IWordTemplateService>().InstancePerLifetimeScope().WithAttributeFiltering();

        builder.Register(
            (_, p) => new SqlDistributedLock(
                p.TypedAs<string>(),
                p.TypedAs<IDbTransaction>(),
                p.TypedAs<bool>()))
            .As<IDistributedLock>()
            .InstancePerDependency();

        builder.RegisterType<RedisConnectionMultiplexerAccessor>().As<IRedisConnectionMultiplexerAccessor>().SingleInstance();
        builder.RegisterType<ClassBookPrintService>().Keyed<IPrintService>(PrintType.ClassBook);
        builder.RegisterType<ClassBookStudentPrintService>().Keyed<IPrintService>(PrintType.StudentBook);
    }
}
