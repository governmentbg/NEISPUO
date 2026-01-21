using Microsoft.Extensions.Logging;
using MON.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Report.Service
{
    public class DbService : IScopedService
    {
        protected readonly MONContext _db;

        public DbService(MONContext db)
        {
            _db = db;
        }

        protected static void ThrowValidationException<V>(V dto) where V : class
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(V));
            }

            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto, null, null);
            if (!Validator.TryValidateObject(dto, context, results))
            {
                Exception[] exceptions = (from f in results select new ValidationException(f, null, dto)).ToArray();
                throw exceptions.Length == 1 ? exceptions[0] : new AggregateException(exceptions);
            }
        }

        protected async Task SaveAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                // TODO: Подобряване или логване на exception-а.
                throw;
            }
        }

        protected void Save()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (Exception)
            {
                // TODO: Подобряване или логване на exception-а.
                throw;
            }
        }
    }


    /// <summary>
    /// Този вариант спестява само двата реда за деклариране и инициализиране на _logger.
    /// </summary>
    public class DbService<T> : DbService where T : DbService
    {
        protected readonly ILogger<T> _logger;

        public DbService(MONContext db, ILogger<T> logger)
            : base(db)
        {
            _logger = logger;
        }
    }
}
