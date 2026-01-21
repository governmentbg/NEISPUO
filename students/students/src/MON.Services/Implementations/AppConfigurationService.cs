namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models.Cache;
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System.Linq;
    using System.Threading.Tasks;

    public class AppConfigurationService : BaseService<AppConfigurationService>, IAppConfigurationService
    {
        private readonly ICacheService _cache;

        public AppConfigurationService(DbServiceDependencies<AppConfigurationService> dependencies,
            ICacheService cache)
            : base(dependencies)
        {
            _cache = cache;
        }

        private string GetCacheKey(string key)
        {
            return $"{CacheKeys.AppSettings}_{key}";
        }

        public async Task<string> GetValueByKey(string key)
        {
            string cacheKey = GetCacheKey(key);

            AppSetttingsCacheModel appSettingsCacheModel = await _cache.GetAsync<AppSetttingsCacheModel>(key);
            if (appSettingsCacheModel != null)
            {
                return appSettingsCacheModel.Value;
            }

            string appSettingsValue = await _context.AppSettings.AsNoTracking()
                .Where(x => x.Key == key)
                .Select(x => x.Value)
                .SingleOrDefaultAsync();

            await _cache.SetAsync(key, new AppSetttingsCacheModel
            {
                Value = appSettingsValue
            });

            return appSettingsValue;
        }

        public async Task DeleteValueByKey(string key)
        {
            AppSetting appSetting = await _context.AppSettings
                .SingleOrDefaultAsync(x => x.Key == key);

            if (appSetting != null)
            {
                appSetting.Value = "";
                await SaveAsync();
                await _cache.RemoveAsync(GetCacheKey(key));
            }
        }

        public async Task AddOrUpdate(string key, string contents)
        {
            if (key.IsNullOrEmpty())
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AppSetting entity = await _context.AppSettings
                .SingleOrDefaultAsync(x => x.Key == key);

            if (entity == null)
            {
                entity = new AppSetting
                {
                    Key = key
                };
                _context.AppSettings.Add(entity);
            }

            entity.Value = contents ?? "";
            await SaveAsync();
            await _cache.RemoveAsync(GetCacheKey(key));
        }
    }
}
