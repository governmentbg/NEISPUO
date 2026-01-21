using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Services.Interfaces;
using MON.Shared;
using System.Linq;
using MON.Shared.Interfaces;
using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MON.Services.Implementations
{
    public class BaseService<T> : IDisposable
    {
        protected readonly MONContext _context;
        protected readonly ILogger _logger;
        protected readonly IUserInfo _userInfo;
        protected readonly INeispuoAuthorizationService _authorizationService;

        protected readonly TimeSpan _slidingExpiration = TimeSpan.FromMinutes(30);
        protected readonly int _refreshInterval = 3600; // Време за рефреш на кеша в секунди
        protected readonly CacheItemPriority _cachePriority = CacheItemPriority.Normal;

        protected DateTime Now => DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userInfo"></param>
        /// <param name="logger"></param>
        /// <param name="moduleName"></param>
        public BaseService(DbServiceDependencies<T> dependencies)
        {
            _ = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _context = dependencies.Context;
            _logger = dependencies.Logger;
            _userInfo = dependencies.CurrentUserInfo;
            _authorizationService = dependencies.AuthorizationService;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            // Ако искаме при имперсониран да гърми, то разкоментираме долния ред
            //if (_userInfo.ImpersonatorSysUserID == null)
            //{
            //    throw new ApiException("Опитвате се да запишете данни, докато сте имперсонирани като друг потребител");
            //}
            //else
            {
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving object to DB", ex.GetInnerMostException());
                    throw new Exception("Error saving object to DB.", ex.GetInnerMostException());
                }
            }

        }

        protected async Task<IEnumerable<CurrencyModel>> GetCurrencies()
        {
            // Todo: да се кешира след тестовете

            return await _context.Currencies
                .Select(x => new CurrencyModel
                {
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    ExchangeRate = x.ExchangeRate,
                    IsMain = x.IsMain,
                })
                .ToListAsync();
        }

        protected async Task<CurrencyModel> GetMainCurrency()
        {
            return (await GetCurrencies())?.FirstOrDefault(x => x.IsMain)
                ?? throw new ArgumentException("Не е настроена основна валута");
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
