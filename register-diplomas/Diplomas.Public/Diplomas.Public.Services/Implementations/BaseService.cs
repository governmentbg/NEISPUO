using Diplomas.Public.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diplomas.Public.Services.Implementations
{
    public class BaseService : IDisposable
    {
        protected readonly DiplomasContext _context;
        protected readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public BaseService(DiplomasContext context, ILogger logger)
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
                _logger.LogError(ex, "Error saving object to DB");
                //throw new Exception("Error saving object to DB.", ex.GetInnerMostException());
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
