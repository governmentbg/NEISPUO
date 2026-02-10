namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using MON.DataAccess;
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BacodeYearService : BaseService<BacodeYearService>, IBarcodeYearService
    {
        private readonly IMemoryCache _cache;

        public BacodeYearService(DbServiceDependencies<BacodeYearService> dependencies, IMemoryCache cache) 
            : base(dependencies)
        {
            _cache = cache;
        }

        public async Task AddBarcodeYearAsync(BarcodeYearModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model cannot be null!");
            }

            var newBarcodeYear = new BarcodeYear
            {
                BasicDocumentId = model.BasicDocumentId,
                Edition = model.Edition,
                HeaderPage = model.HeaderPage,
                InternalPage = model.InternalPage,
                SchoolYear = model.SchoolYear,
            };

            _context.BarcodeYears.Add(newBarcodeYear);

            await SaveAsync();

            ClearCache(newBarcodeYear.BasicDocumentId);
        }

        public async Task DeleteBarcodeYearAsync(int barcodeYearId)
        {
            if (barcodeYearId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(barcodeYearId), "BarcodeYearId must be possitive number!");
            };

            var entity = await _context.BarcodeYears
                .SingleOrDefaultAsync(x => x.Id == barcodeYearId);


            if (entity != null)
            {
                int basicDocumentId = entity.BasicDocumentId;

                _context.BarcodeYears.Remove(entity);
                await SaveAsync();

                ClearCache(basicDocumentId);
            }
        }

        public async Task<BarcodeYearModel> GetBarcodeYearByIdAsync(int barcodeYearId)
        {
            if (barcodeYearId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(barcodeYearId), "BarcodeYearId must be possitive number!");
            }

            BarcodeYearModel barcodeYear = await _context.BarcodeYears
                .AsNoTracking()
                .Where(x => x.Id == barcodeYearId)
                .Select(x => new BarcodeYearModel
                {
                    Id = x.Id,
                    Edition = x.Edition,
                    HeaderPage = x.HeaderPage,
                    InternalPage = x.InternalPage,
                    SchoolYear = x.SchoolYear,
                }).SingleOrDefaultAsync();

            return barcodeYear;
        }

        public async Task<IEnumerable<BarcodeYearModel>> GetBarcodesByYear(int basicDocumentId, short schoolYear)
        {
            BarcodeYearListViewModel barcodes = await GetBarcodeYearsAsync(basicDocumentId);
            if (barcodes?.BarcodeYears != null)
            {
                return barcodes.BarcodeYears.Where(i => i.SchoolYear == schoolYear);
            }
            else
            {
                return null;
            }
        }

        public async Task<BarcodeYearListViewModel> GetBarcodeYearsAsync(int basicDocumentId)
        {
            if (basicDocumentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(basicDocumentId), "BasicDocumentId must be possitive number!");
            }

            return await _cache.GetOrCreateAsync($"{CacheKeys.BasicDocumentBarcodeYearOptions}_{basicDocumentId}", entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.BasicDocuments
                    .AsNoTracking()
                    .Where(x => x.Id == basicDocumentId)
                    .Select(d => new BarcodeYearListViewModel
                    {
                        DiplomaTypeName = d.Name,
                        BarcodeYears = d.BarcodeYears.Select(x => new BarcodeYearModel
                        {
                            Id = x.Id,
                            Edition = x.Edition,
                            HeaderPage = x.HeaderPage,
                            InternalPage = x.InternalPage,
                            SchoolYear = x.SchoolYear
                        })
                    })
                    .SingleOrDefaultAsync();
            });
        }

        public async Task UpdateBarcodeYearAsync(BarcodeYearModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            if (!await _context.BasicDocuments.AnyAsync(x => x.Id == model.BasicDocumentId))
            {
                throw new ArgumentNullException(nameof(model), "BasicDocumen cant be null!");
            }

            var barcodeYear = await _context.BarcodeYears.SingleOrDefaultAsync(x => x.Id == model.Id);

            if (barcodeYear == null) throw new ArgumentNullException(nameof(barcodeYear), "BarcodeYear cant be null!");

            barcodeYear.Edition = model.Edition;
            barcodeYear.HeaderPage = model.HeaderPage;
            barcodeYear.InternalPage = model.InternalPage;
            barcodeYear.SchoolYear = model.SchoolYear;

            await SaveAsync();

            ClearCache(barcodeYear.BasicDocumentId);
        }

        private void ClearCache(int basicDocumentId)
        {
            if (_cache != null)
            {
                _cache.Remove($"{CacheKeys.BasicDocumentBarcodeYearOptions}_{basicDocumentId}");
            }
        }
    }
}
