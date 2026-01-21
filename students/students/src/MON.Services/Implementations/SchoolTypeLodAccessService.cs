using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Models.SchoolTypeLodAccess;
using MON.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class SchoolTypeLodAccessService : BaseService<SchoolTypeLodAccessService>, ISchoolTypeLodAccessService
    {
        private readonly ICacheService _cache;

        public SchoolTypeLodAccessService(DbServiceDependencies<SchoolTypeLodAccessService> dependencies, ICacheService cache)
            : base(dependencies)
        {
            _cache = cache;
        }

        public async Task<IEnumerable<SchoolTypeLodAccessModel>> GetAllAsync()
        {
            return await _context.DetailedSchoolTypes
                .AsNoTracking()
                .Where(d => d.IsValid)
                .Select(d => new SchoolTypeLodAccessModel
                {
                    DetailedSchoolTypeId = d.DetailedSchoolTypeId,
                    DetailedSchoolTypeName = d.Name,
                    IsLodAccessAllowed = _context.SchoolTypeLodAccesses.FirstOrDefault(x => x.DetailedSchoolTypeId == d.DetailedSchoolTypeId).IsLodAccessAllowed
                })
                .ToListAsync();
        }

        public async Task<SchoolTypeLodAccessModel> GetByIdAsync(int detailedSchoolTypeId)
        {
            return await _context.DetailedSchoolTypes
                .AsNoTracking()
                .Where(d => d.IsValid && d.DetailedSchoolTypeId == detailedSchoolTypeId)
                .Select(d => new SchoolTypeLodAccessModel
                {
                    DetailedSchoolTypeId = d.DetailedSchoolTypeId,
                    DetailedSchoolTypeName = d.Name,
                    IsLodAccessAllowed = _context.SchoolTypeLodAccesses.FirstOrDefault(x => x.DetailedSchoolTypeId == d.DetailedSchoolTypeId).IsLodAccessAllowed
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(SchoolTypeLodAccessModel model)
        {
            ApiValidationResult validationResult = ValidateUpdate(model);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Messages);
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            SchoolTypeLodAccess schoolTypeLodAccessToUpdate = _context.SchoolTypeLodAccesses.FirstOrDefault(x => x.DetailedSchoolTypeId == model.DetailedSchoolTypeId);
            if (schoolTypeLodAccessToUpdate == null)
            {
                _context.SchoolTypeLodAccesses.Add(new SchoolTypeLodAccess
                {
                    DetailedSchoolTypeId = model.DetailedSchoolTypeId,
                    IsLodAccessAllowed = model.IsLodAccessAllowed
                });
            }
            else
            {
                schoolTypeLodAccessToUpdate.IsLodAccessAllowed = model.IsLodAccessAllowed;
            }

            await SaveAsync();
            _cache?.ClearCache();
        }

        private ApiValidationResult ValidateUpdate(SchoolTypeLodAccessModel model)
        {
            ApiValidationResult validationResult = new ApiValidationResult
            {
                IsValid = true
            };

            if (model == null)
            {
                validationResult.IsValid = false;
                validationResult.Messages.Add($"Model {nameof(SchoolTypeLodAccessModel)} cant be null!");
            }

            return validationResult;
        }
    }
}
