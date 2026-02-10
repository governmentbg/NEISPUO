using Microsoft.Extensions.Logging;
using Neispuo.Tools.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neispuo.Tools.Services.Implementations
{
    public class BaseService : IDisposable
    {
        protected readonly NeispuoContext _context;
        protected readonly ILogger _logger;

       /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userInfo"></param>
        /// <param name="logger"></param>
        /// <param name="moduleName"></param>
        public BaseService(NeispuoContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving object to DB.");
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
