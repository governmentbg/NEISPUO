namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.Extensions.Logging;
    using Helpdesk.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BaseService : IDisposable
    {
        protected readonly HelpdeskContext _context;
        protected readonly ILogger _logger;
        protected readonly IUserInfo _userInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userInfo"></param>
        /// <param name="logger"></param>
        public BaseService(HelpdeskContext context, IUserInfo userInfo, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _userInfo = userInfo;
        }

        public async Task SaveAsync(string description = null)
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving object to DB", ex.GetInnerMostException());
                throw new Exception("Error saving object to DB.", ex.GetInnerMostException());
            }
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
