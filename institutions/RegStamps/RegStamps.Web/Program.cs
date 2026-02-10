namespace RegStamps.Web
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Identity;

    using Filters;

    using Data.Entities;

    using Settings;

    using Models.Stores;

    using Services.Stores;
    using Services.Neispuo;
    using Services.Entities;
    using Services.Pdf;
    using Services.DocumentFiles;
    using Services.Check;
    using Services.Validation;
    using Services.Signature;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
            builder.Services.AddDbContext<DataStampsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetValue<string>("AppSettings:Database:DataStampsDatabase:ConnectionString")));
            builder.Services.AddTransient<IUserStore<ApplicationUser>, UserStoreService>()
                            .AddTransient<IRoleStore<ApplicationRole>, RoleStoreService>()
                            .AddTransient<IRazorTemplateService, RazorTemplateService>()
                            .AddSingleton<IPdfService, PdfService>()
                            .AddTransient<IHttpService, HttpService>()
                            .AddTransient<INeispuoService, NeispuoService>()
                            .AddTransient<ITbStampService, TbStampService>()
                            .AddTransient<IRefRequestStampService, RefRequestStampService>()
                            .AddTransient<ITbRequestFileService, TbRequestFileService>()
                            .AddTransient<ITbKeeperService, TbKeeperService>()
                            .AddTransient<ITbKeepPlaceService, TbKeepPlaceService>()
                            .AddTransient<ICodeStampTypeService, CodeStampTypeService>()
                            .AddTransient<IDocumentFileService, DocumentFileService>()
                            .AddTransient<ICheckService, CheckService>()
                            .AddTransient<IValidationService, ValidationService>()
                            .AddTransient<ICertificateService, CertificateService>()
                            .AddTransient<ICodeFileTypeService, CodeFileTypeService>()
                            .AddTransient<ITbErrorLogService, TbErrorLogService>();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(config => 
            {
                config.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                config.SlidingExpiration = true;
                config.Cookie.Name = "RegStamps.IdentityCookie";
                config.LoginPath = "/Home/Main";
            });

            builder.Services
                .AddControllersWithViews(options => 
                {
                    options.Filters.Add<CustomExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
