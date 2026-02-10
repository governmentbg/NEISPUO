using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MON.DataAccess;
using MON.Models;
using MON.Models.ASP;
using MON.Models.Diploma;
using MON.Models.Dropdown;
using MON.Models.Enums;
using MON.Models.Institution;
using MON.Services.Extensions;
using MON.Services.Interfaces;
using MON.Shared;
using MON.Shared.Enums;
using MON.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class LookupsService : BaseService<LookupsService>, ILookupService
    {
        private readonly IMemoryCache _cache;
        private readonly IInstitutionService _institutionService;
        private readonly IAppConfigurationService _configurationService;

        public LookupsService(DbServiceDependencies<LookupsService> dependencies,
            IInstitutionService institutionService,
            IMemoryCache cache,
            IAppConfigurationService configurationService)
            : base(dependencies)
        {
            _institutionService = institutionService;
            _cache = cache;
            _configurationService = configurationService;
        }

        private bool UseCahce
        {
            get
            {
                string config = _configurationService.GetValueByKey("UseNomenclatureCache").Result;

                if (bool.TryParse(config ?? "", out bool useCache))
                {
                    return useCache;

                }
                else
                {
                    return false;
                }
            }
        }


        public async Task<IEnumerable<DropdownViewModel>> GetReasonsForEqualization()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.ReasonForEqualizationTypeOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.ReasonForEqualizationTypes
                        .Select(r => new DropdownViewModel
                        {
                            Value = r.Id,
                            Name = r.Name,
                            Text = r.Name,
                        }).ToListAsync();
                })
                : await _context.ReasonForEqualizationTypes
                .Select(r => new DropdownViewModel
                {
                    Value = r.Id,
                    Name = r.Name,
                    Text = r.Name,
                }).ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetPinTypesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.PinTypesOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.PersonalIdtypes.Select(p => new DropdownViewModel
                {
                    Name = p.Name,
                    Value = p.PersonalIdtypeId,
                    Text = p.Name
                }).ToListAsync();
            });
        }

        public MaintenanceModeModel GetMaintenanceMode()
        {
            return _cache.GetOrCreate<MaintenanceModeModel>(CacheKeys.MaintenanceMode, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                MaintenanceModeModel model = new MaintenanceModeModel() { Enabled = false };
                try
                {
                    string maintenanceMode = _configurationService.GetValueByKey("MaintenanceMode").Result;
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    model = string.IsNullOrWhiteSpace(maintenanceMode)
                       ? new MaintenanceModeModel() { Enabled = false }
                       : JsonSerializer.Deserialize<MaintenanceModeModel>(maintenanceMode, options);
                }
                catch
                {

                }
                return model;
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetGendersAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.GendersOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Genders.Select(g => new DropdownViewModel
                {
                    Name = g.Name,
                    Value = g.GenderId,
                    Text = g.Name
                }).ToListAsync();
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetGuardianTypesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.GuardianTypeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.GuardianTypes.Select(g => new DropdownViewModel
                {
                    Name = g.Name,
                    Value = g.Id,
                    Text = g.Name
                }).ToListAsync();
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetLanguagesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.LanguagesOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Languages.Select(g => new DropdownViewModel
                {
                    Name = g.Name,
                    Value = g.Id,
                    Text = g.Description
                }).ToListAsync();
            });
        }

        public async Task<List<DropdownViewModel>> GetEKRAsync()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.EkrOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval); ;
                    entry.Priority = _cachePriority;

                    return _context.Ekrtypes
                        .Select(g => new DropdownViewModel
                        {
                            Name = $"{g.Id}. {g.Name}",
                            Value = g.Id,
                            Text = $"{g.Id}. {g.Name}"
                        }).ToListAsync();
                })
                : await _context.Ekrtypes
                    .Select(g => new DropdownViewModel
                    {
                        Name = $"{g.Id}. {g.Name}",
                        Value = g.Id,
                        Text = $"{g.Id}. {g.Name}"
                    }).ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetNKRAsync()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.NkrOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.Nkrtypes
                        .Select(g => new DropdownViewModel
                        {
                            Name = $"{g.Id}. {g.Name}",
                            Value = g.Id,
                            Text = $"{g.Id}. {g.Name}"
                        }).ToListAsync();
                })
                : await _context.Nkrtypes.Select(g => new DropdownViewModel
                {
                    Name = $"{g.Id}. {g.Name}",
                    Value = g.Id,
                    Text = $"{g.Id}. {g.Name}"
                }).ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetCountriesAsync(string searchStr, string selectedValue)
        {
            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            IQueryable<Country> query = _context.Countries
                .AsNoTracking();

            if (selectedValue.IsNullOrWhiteSpace())
            {
                // Липсва избрана стойност
                return await query.Where(x => searchQuery == null || x.Name.Contains(searchQuery))
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.CountryId,
                        Text = x.Name,
                        Name = x.Name,
                        Code = x.Code
                    }).ToListAsync();
            }

            string[] splitStr = selectedValue.Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> selectedIds = splitStr.ToHashSet<int>();
            if (selectedIds.Count > 0)
            {
                // Подали сме Id-та
                return await query
                    .Where(x => selectedIds.Contains(x.CountryId) || searchQuery == null || x.Name.Contains(searchQuery))
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.CountryId,
                        Text = x.Name,
                        Name = x.Name,
                        Code = x.Code
                    }).ToListAsync();
            }

            HashSet<string> selectedNames = splitStr.ToHashSet();

            return await query
                .Where(x => selectedNames.Contains(x.Name) || selectedNames.Contains(x.Code) || searchQuery == null || x.Name.Contains(searchQuery))
                .Select(x => new DropdownViewModel
                {
                    Value = x.CountryId,
                    Text = x.Name,
                    Name = x.Name,
                    Code = x.Code
                }).ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetCountryOptions()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.CountryOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.Countries
                    .AsNoTracking()
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.CountryId,
                        Text = x.Name,
                        Code = x.Code
                    })
                    .ToListAsync();
                })
                : await _context.Countries
                    .AsNoTracking()
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.CountryId,
                        Text = x.Name,
                        Code = x.Code
                    })
                    .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetDistrictsAsync(string searchStr, string selectedValue)
        {
            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            IQueryable<Region> query = _context.Regions
                .AsNoTracking();

            if (selectedValue.IsNullOrWhiteSpace())
            {
                // Липсва избрана стойност
                return await query.Where(x => searchQuery == null || x.Name.Contains(searchQuery))
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.RegionId,
                        Text = x.Name
                    }).ToListAsync();
            }

            string[] splitStr = selectedValue.Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> selectedIds = splitStr.ToHashSet<int>();
            if (selectedIds.Count > 0)
            {
                // Подали сме Id-та
                return await query
                    .Where(x => selectedIds.Contains(x.RegionId) || searchQuery == null || x.Name.Contains(searchQuery))
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.RegionId,
                        Text = x.Name
                    }).ToListAsync();
            }

            HashSet<string> selectedNames = splitStr.ToHashSet();

            return await query
                .Where(x => selectedNames.Contains(x.Name) || searchQuery == null || x.Name.Contains(searchQuery))
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Value = x.RegionId,
                    Text = x.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetMunicipalitiesAsync(string searchStr, string selectedValue, int? regionId)
        {
            List<Municipality> minicipalities = await _cache.GetOrCreateAsync(CacheKeys.Municipalities, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Municipalities
                    .AsNoTracking()
                    .ToListAsync();
            });


            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            if (selectedValue.IsNullOrWhiteSpace())
            {
                // Липсва избрана стойност
                return minicipalities.Where(x => (searchQuery == null || x.Name.Contains(searchQuery))
                 && (!regionId.HasValue || regionId.Value == x.RegionId))
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.MunicipalityId,
                        Text = x.Name,
                    }).ToList();
            }

            string[] splitStr = selectedValue.Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> selectedIds = splitStr.ToHashSet<int>();
            if (selectedIds.Count > 0)
            {
                // Подали сме Id-та
                return minicipalities
                    .Where(x => selectedIds.Contains(x.MunicipalityId) || searchQuery == null || x.Name.Contains(searchQuery))
                    .Where(x => !regionId.HasValue || regionId.Value == x.RegionId)
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.MunicipalityId,
                        Text = x.Name
                    }).ToList();
            }

            HashSet<string> selectedNames = splitStr.ToHashSet();

            return minicipalities
                .Where(x => selectedNames.Contains(x.Name) || searchQuery == null || x.Name.Contains(searchQuery))
                .Where(x => !regionId.HasValue || regionId.Value == x.RegionId)
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Value = x.MunicipalityId,
                    Text = x.Name
                }).ToList();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetCitiesAsync(string searchStr, string selectedValue, int? municipalityId)
        {
            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            IQueryable<Town> query = _context.Towns
                .AsNoTracking();

            if (selectedValue.IsNullOrWhiteSpace())
            {
                // Липсва избрана стойност
                return await query.Where(x => (searchQuery == null || x.Name.Contains(searchQuery))
                    && (!municipalityId.HasValue || municipalityId.Value == x.MunicipalityId))
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.TownId,
                        Text = x.Name
                    }).ToListAsync();
            }

            string[] splitStr = selectedValue.Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> selectedIds = splitStr.ToHashSet<int>();
            if (selectedIds.Count > 0)
            {
                // Подали сме Id-та
                return await query
                    .Where(x => selectedIds.Contains(x.TownId) || searchQuery == null || x.Name.Contains(searchQuery))
                    .Where(x => !municipalityId.HasValue || municipalityId.Value == x.MunicipalityId)
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.TownId,
                        Text = x.Name
                    }).ToListAsync();
            }

            HashSet<string> selectedNames = splitStr.ToHashSet();

            return await query
                .Where(x => selectedNames.Contains(x.Name) || searchQuery == null || x.Name.Contains(searchQuery))
                .Where(x => !municipalityId.HasValue || municipalityId.Value == x.MunicipalityId)
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Value = x.TownId,
                    Text = x.Name
                }).ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetClassTypesAsync(int? basicClassId)
        {
            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            int institutionDetailedSchoolType = institution?.DetailedSchoolTypeId ?? default;

            IQueryable<ClassTypeLimit> classTypeLimits = _context.ClassTypeLimits
                .AsNoTracking()
                .Where(x => basicClassId.HasValue ? x.BasicClassId == basicClassId.Value && x.DetailedSchoolTypeId == institutionDetailedSchoolType && x.ClassKind == (int)ClassKindEnum.Basic
                    : x.DetailedSchoolTypeId == institutionDetailedSchoolType && x.ClassKind == (int)ClassKindEnum.Basic);

            List<DropdownViewModel> classTypes = await _context.ClassTypes
                .AsNoTracking()
                .Where(x => classTypeLimits.Any(ct => ct.ClassTypeId == x.ClassTypeId) && x.IsValid == true)
                .OrderBy(x => x.Name)
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Value = x.ClassTypeId,
                    Text = x.Name
                }).ToListAsync();

            return classTypes;
        }

        public async Task<List<DropdownViewModel>> GetClassTypesForLoggedUser(string searchStr, int? selectedValue)
        {
            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            int institutionDetailedSchoolType = institution?.DetailedSchoolTypeId ?? default;

            IQueryable<DropdownViewModel> query =
                from classTypeLimit in _context.ClassTypeLimits
                join classType in _context.ClassTypes on classTypeLimit.ClassTypeId equals classType.ClassTypeId into temp
                from classType in temp.DefaultIfEmpty()
                where (selectedValue.HasValue && classTypeLimit.ClassTypeId == selectedValue.Value)
                     || (classTypeLimit.DetailedSchoolTypeId == institutionDetailedSchoolType && (searchQuery == null || classType.Name.Contains(searchQuery) || classType.Description.Contains(searchQuery)))
                select new DropdownViewModel
                {
                    Name = $"{classType.Name} {classType.Description}",
                    Value = classTypeLimit.ClassTypeId,
                    Text = $"{classType.Name} {classType.Description}"
                };


            return await query.ToListAsync();
        }

        public Task<List<DropdownViewModel>> GetBasicClassOptions(string searchStr, int? selectedValue, int? minId, int? maxId)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            return _context.BasicClasses
                .AsNoTracking()
                .Where(x => x.IsValid == true
                    && (!minId.HasValue || x.BasicClassId >= minId.Value)
                    && (!maxId.HasValue || x.BasicClassId <= maxId.Value))
                .Where(x => (selectedValue.HasValue && x.BasicClassId == selectedValue.Value) || query == null
                    || x.Name.Contains(query) || x.Description.Contains(query)
                    || x.RomeName.Contains(query))
                .Select(x => new DropdownViewModel
                {
                    Value = x.BasicClassId,
                    Name = x.Name,
                    Text = x.Description,
                    Code = x.RomeName
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetBasicClassValidOptions(int? relatedObject, string searchStr, int? selectedValue, int? minId, int? maxId)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            int institutionDetailedSchoolType = institution?.DetailedSchoolTypeId ?? default;

            return await _context.BasicClasses.Where(g => g.IsValid == true
                    && (!minId.HasValue || g.BasicClassId >= minId.Value)
                    && (!maxId.HasValue || g.BasicClassId <= maxId.Value))
                 .Where(x => (selectedValue.HasValue && x.BasicClassId == selectedValue.Value) || query == null
                     || x.Name.Contains(query) || x.Description.Contains(query)
                     || x.RomeName.Contains(query))
                 .Join(
                 _context.BasicClassLimits
                 .Where(bcl => bcl.DetailedSchoolTypeId == institutionDetailedSchoolType && bcl.ClassKind == 1 && bcl.BasicClassId != 21), // 21 - разновъзрастов
                 basicClass => basicClass.BasicClassId,
                 basicClassLimit => basicClassLimit.BasicClassId,
                 (basicClass, basicClassLimit) => new DropdownViewModel
                 {
                     Name = $"{basicClass.Description}",
                     Value = basicClass.BasicClassId,
                     Text = $"{basicClass.Description}"
                 }
                 ).ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetBasicClassesForLoggedUser(string searchStr, int? selectedValue)
        {
            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            int institutionDetailedSchoolType = institution?.DetailedSchoolTypeId ?? default;

            IQueryable<DropdownViewModel> query = from basicClassLimit in _context.BasicClassLimits
                                                  join basicClass in _context.BasicClasses on basicClassLimit.BasicClassId equals basicClass.BasicClassId into temp
                                                  from basicClass in temp.DefaultIfEmpty()
                                                  where (selectedValue.HasValue && basicClassLimit.BasicClassId == selectedValue.Value)
                                                       || (basicClassLimit.DetailedSchoolTypeId == institutionDetailedSchoolType && (searchQuery == null || basicClass.Name.Contains(searchQuery) || basicClass.Description.Contains(searchQuery)))
                                                  select new DropdownViewModel
                                                  {
                                                      Name = $"{basicClass.Name} {basicClass.Description}",
                                                      Value = basicClassLimit.BasicClassId,
                                                      Text = $"{basicClass.Name} {basicClass.Description}"
                                                  };


            return await query.ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetPreSchoolBasicClassOptionsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.PreSchoolBasicClassOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.BasicClasses
                    .Where(g => g.IsValid != null && g.IsValid.Value && (g.BasicClassId == (int)PreSchoolBasicClass.ThirdGroup || g.BasicClassId == (int)PreSchoolBasicClass.FourthGroup))
                    .Select(g => new DropdownViewModel
                    {
                        Name = $"{g.Name} {g.Description}",
                        Value = g.BasicClassId,
                        Text = $"{g.Name} {g.Description}"
                    }).ToListAsync();
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetEducationFormOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.EducationFormOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.EduForms
                        .Where(g => g.ValidforStudent != null && g.ValidforStudent.Value && g.CanChoose == true)
                        .Select(g => new DropdownViewModel
                        {
                            Value = g.ClassEduFormId,
                            Name = g.Name,
                            Text = g.Name,
                            IsValid = g.IsValid ?? false
                        }).ToListAsync();
                })
                : await _context.EduForms
                        .Where(g => g.ValidforStudent != null && g.ValidforStudent.Value && g.CanChoose == true)
                        .Select(g => new DropdownViewModel
                        {
                            Value = g.ClassEduFormId,
                            Name = g.Name,
                            Text = g.Name,
                            IsValid = g.IsValid ?? false
                        }).ToListAsync();

            return allEntries.FilterByValid(valid);
        }

        public async Task<List<DropdownViewModel>> GetEducationFormDiplomaOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.EducationFormForDiplomaOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.EduForms
                        .Where(g => (g.ValidforDiploma == true && g.CanChoose == true))
                        .Select(g => new DropdownViewModel
                        {
                            Value = g.ClassEduFormId,
                            Name = g.Name,
                            Text = g.Name,
                            IsValid = g.IsValid ?? false
                        }).ToListAsync();
                })
                : await _context.EduForms
                .Where(g => (g.ValidforDiploma == true && g.CanChoose == true))
                .Select(g => new DropdownViewModel
                {
                    Value = g.ClassEduFormId,
                    Name = g.Name,
                    Text = g.Name,
                    IsValid = g.IsValid ?? false
                }).ToListAsync();

            return allEntries.FilterByValid(valid);
        }

        public Task<List<ClassGroupDropdownViewModel>> GetValidForStudentEducationFormOptionsAsync(bool? isNotPresentForm)
        {
            return _context.EduForms
                .Where(g => g.IsValid.HasValue && g.IsValid.Value && g.CanChoose == true && g.ValidforStudent == true
                        && (isNotPresentForm.HasValue == false ||
                            (g.IsNotPresentForm != null && g.IsNotPresentForm == isNotPresentForm.Value) ||
                            (isNotPresentForm.Value == false && g.IsNotPresentForm == null)))
                .Select(g => new ClassGroupDropdownViewModel
                {
                    Name = $"{g.Name}",
                    Value = g.ClassEduFormId,
                    Text = $"{g.Name}",
                    IsNotPresentForm = g.IsNotPresentForm
                }).ToListAsync();
        }

        public async Task<List<ClassGroupDropdownViewModel>> GetEduFormsForLoggedUser(bool? isNotPresentForm)
        {
            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            int instType = institution?.InstTypeId ?? default;

            if (isNotPresentForm == true)
            {
                return await _context.EduForms
                    .Where(x => x.IsNotPresentForm == true && x.IsValid == true && x.CanChoose == true)
                    .OrderBy(x => x.Name)
                    .Select(x => new ClassGroupDropdownViewModel
                    {
                        Name = x.Name,
                        Value = x.ClassEduFormId,
                        Text = x.Name,
                        IsNotPresentForm = x.IsNotPresentForm
                    })
                    .ToListAsync();
            }

            return await GetValidForStudentEducationFormOptionsAsync(isNotPresentForm);
        }

        public Task<List<CurrentClassDropdownViewModel>> GetCurrentStudentClassesForDischarge(int personId, int? selectedValue)
        {
            IQueryable<StudentClass> query = _context.StudentClasses.AsNoTracking();

            if (_userInfo.IsSchoolDirector)
            {
                query = query.Where(x => _userInfo.InstitutionID.HasValue
                    && x.InstitutionId == _userInfo.InstitutionID);
            }

            return query
                .Where(g => g.PersonId == personId
                    && ((selectedValue.HasValue && g.Id == selectedValue.Value)
                       ||
                       (g.IsCurrent && (g.PositionId != 3 || (g.ClassType.ClassKind == 1)))
                    ))
                .Select(g => new CurrentClassDropdownViewModel
                {
                    Value = g.Id,
                    Text = g.Class.ClassName,
                    SendingInstitution = g.Class.InstitutionSchoolYear.Name,
                    IsCurrent = g.IsCurrent
                }).ToListAsync();
        }

        /// <summary>
        /// Всички паралелки/ групи за институция (само подгрупи), дадена учебна година за базов клас
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="basicClass"></param>
        /// <returns></returns>
        public async Task<List<ClassGroupDropdownViewModel>> GetClassGroupsAsync(int institutionId, int schoolYear, short? basicClass,
            short? minBasicClass, int? personId, bool? isInitialEnrollment, bool? filterForTeacher)
        {
            // При записване в самостоятелна форма на обучение се използва служебна паралелка, която няма ParentClassId, но има IsNotPresentForm == true
            IQueryable<ClassGroup> query = _context.ClassGroups
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId
                    && x.SchoolYear == schoolYear
                    && (x.IsNotPresentForm == true
                        || (x.ParentClassId.HasValue && x.ClassTypeId.HasValue && x.ClassType.ClassKind == (int)ClassKindEnum.Basic))
                        );

            // Филтрираме ги за определен BasicClass т.е. за 1-ви, 2-ри т.н клас
            if (basicClass.HasValue)
            {
                query = query.Where(x => x.IsNotPresentForm == true || x.BasicClassId == basicClass.Value);
            }

            // Записване в основна паралелка
            if (isInitialEnrollment ?? false)
            {
                // Позволени класове по ClassKind спрямо типа на институтцията. 
                HashSet<int> allowedClassKinds = (await _institutionService.GetAllowedClasssKindsEnrollmentLimit(institutionId))?.SingleEnrollment?.AllowedClassKind;
                query = query.Where(x => x.IsNotPresentForm == true || !x.ClassType.ClassKind.HasValue || allowedClassKinds == null || allowedClassKinds.Contains(x.ClassType.ClassKind.Value));
            }

            if (filterForTeacher ?? false)
            {
                query = query.Where(x => x.ParentClassId.HasValue && (_userInfo.LeadTeacherClasses != null && _userInfo.LeadTeacherClasses.Contains(x.ParentClassId.Value)));
            }

            return await query
                .OrderBy(x => x.BasicClassId).ThenBy(x => x.ClassName)
                .Select(c => new ClassGroupDropdownViewModel
                {
                    Name = $"{c.ClassGroupNum} - {c.BasicClass.RomeName} - {c.ClassName} - {c.ClassEduForm.Name} - {c.ClassType.Name} - /{c.StudentClasses.Where(i => i.IsCurrent).Count()} деца/",
                    Value = c.ClassId,
                    Text = $"{c.ClassGroupNum} - {c.BasicClass.RomeName} - {c.ClassName} - {c.ClassEduForm.Name} - {c.ClassType.Name} - /{c.StudentClasses.Where(i => i.IsCurrent).Count()} деца/",
                    Disabled = c.StudentClasses.Any(sc => personId.HasValue && sc.PersonId == personId.Value && sc.IsCurrent),
                    RelatedObjectId = c.ParentClassId.Value,
                    IsNotPresentForm = c.IsNotPresentForm,
                    IsValid = (c.ParentClass.IsValid ?? true) && (c.IsValid ?? true), // В базата има default(1)
                    IsClosed = c.IsClosed,
                    ClassKindId = c.ClassType.ClassKind,
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetStaffAsync(string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();
            return await _context.StaffPositions
                .Where(x => x.InstitutionId == _userInfo.InstitutionID
                && ((selectedValue.HasValue && x.StaffPositionId == selectedValue.Value)
                || (query == null || (x.Person.FirstName + " " + x.Person.LastName).Contains(query))))
                .Select(a => new DropdownViewModel
                {
                    Value = a.StaffPositionId,
                    Text = a.Person.FirstName + " " + a.Person.LastName + " (" + a.CategoryStaffType.Name + ") " + ((a.CurrentlyValid != null && a.CurrentlyValid == false) ? "неактивен за тек. година" : ""),
                    Name = a.Person.FirstName + " " + a.Person.LastName + " (" + a.CategoryStaffType.Name + ") " + ((a.CurrentlyValid != null && a.CurrentlyValid == false) ? "неактивен за тек. година" : ""),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Всички паралелки/ групи за логнатата институция (само подгрупи) и дадена учебна година
        /// </summary>
        /// <param name="schoolYear"></param>
        /// /// <param name="personId">Проверява дали за ученика има активен запис в StudentClass. В UI-а тази опция излиза маркирана и невъзможна за избор.</param>
        /// <returns></returns>
        public async Task<List<ClassGroupDropdownViewModel>> GetClassGroupsForLoggedUser(int schoolYear, int? personId)
        {
            IQueryable<ClassGroup> query = _context.ClassGroups
               .AsNoTracking()
               .Where(x => x.InstitutionId == _userInfo.InstitutionID
                    && x.IsClosed != true
                   && x.SchoolYear == schoolYear
                   && (x.IsNotPresentForm == true || (x.ParentClassId.HasValue && x.ClassTypeId.HasValue)));

            List<ClassGroupDropdownViewModel> list = await query
                .OrderBy(x => x.BasicClassId).ThenBy(x => x.ClassName)
                .Select(c => new ClassGroupDropdownViewModel
                {
                    Name = $"{c.ClassName} - {c.BasicClass.RomeName} - {c.ClassEduForm.Name} - {c.ClassType.Name} - /{c.StudentClasses.Where(i => i.IsCurrent).Count()} деца/",
                    Value = c.ClassId,
                    Text = $"{c.ClassName} - {c.BasicClass.RomeName} - {c.ClassEduForm.Name} - {c.ClassType.Name} - /{c.StudentClasses.Where(i => i.IsCurrent).Count()} деца/",
                    Disabled = c.StudentClasses.Any(sc => personId.HasValue && sc.PersonId == personId.Value && sc.IsCurrent),
                    RelatedObjectId = c.ParentClassId.Value,
                    IsNotPresentForm = c.IsNotPresentForm,
                    BasicClassId = c.BasicClassId,
                    IsValid = c.IsValid ?? true, // В базата има default(1)
                    IsClosed = c.IsClosed,
                    ClassKindId = c.ClassType.ClassKind
                })
                .ToListAsync();

            return list.OrderByDescending(x => x.Disabled).ThenBy(x => x.Text).ToList();
        }


        public async Task<List<DropdownViewModel>> GetBasicClassesLimitForInstitution(int institutionId)
        {
            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
            int institutionDetailedSchoolType = institution?.DetailedSchoolTypeId ?? default;

            List<DropdownViewModel> items = await (
                from bcl in _context.BasicClassLimits
                join bc in _context.BasicClasses on bcl.BasicClassId equals bc.BasicClassId
                where bcl.DetailedSchoolTypeId == institutionDetailedSchoolType
                orderby bcl.BasicClassId
                select new DropdownViewModel
                {
                    Value = bcl.BasicClassId,
                    Text = bc.Name
                }).ToListAsync();

            return items;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetAddressesAsync(string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();
            return await _context.Towns
                .Where(x => (selectedValue.HasValue && x.TownId == selectedValue.Value) || query == null || x.Name.Contains(query))
                .Select(a => new DropdownViewModel
                {
                    Value = a.TownId,
                    Text = string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}"),
                    Name = string.Join(", ", $"гр./с.{a.Name}", $"общ.{a.Municipality.Name}", $"обл.{a.Municipality.Region.Name}")
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetAdmissionOptionsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.AdmissionReasonOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.AdmissionReasonTypes
                    .AsNoTracking()
                    .Where(x => x.IsValid)
                    .Select(x => new DropdownViewModel
                    {
                        Name = x.Name,
                        Text = x.Description,
                        Value = x.Id,
                        IsValid = x.IsValid
                    }).ToListAsync();
            });
        }

        public Task<List<DropdownViewModel>> GetDischargeOptionsAsync(bool? isForDischarge, bool? isForRelocation)
        {
            IQueryable<DischargeReasonType> query = _context.DischargeReasonTypes
                .AsNoTracking()
                .Where(x => !x.IsForInternalEnrollment && x.IsValid);

            if (isForDischarge.HasValue)
            {
                query = query.Where(x => x.IsForDischarge == isForDischarge.Value);
            }

            if (isForRelocation.HasValue)
            {
                query = query.Where(x => x.IsForRelocation == isForRelocation.Value);
            }

            return query.Select(x => new DropdownViewModel
            {
                Name = x.Name,
                Text = x.Description,
                Value = x.Id,
                IsValid = x.IsValid
            }).ToListAsync();
        }

        public Task<List<SubjectTypeDropdownViewModel>> GetSubjectTypeOptions(int? basicSubjectTypeId, int? partId, bool? showAll)
        {
            IQueryable<SubjectType> query = _context.SubjectTypes
                .AsNoTracking();

            bool showAllSelected = showAll ?? false;

            if (!showAllSelected)
            {
                query = query.Where(x => x.IsValid == true);
            }

            if (basicSubjectTypeId.HasValue)
            {
                query = query.Where(x => x.BasicSubjectTypeId == basicSubjectTypeId.Value);
            }

            if (partId.HasValue)
            {
                query = query.Where(x => x.PartId == partId.Value);
            }

            return (from subjectType in query.OrderBy(x => x.PartId).ThenByDescending(x => x.IsValid).ThenBy(x => x.Name)
                    join part in _context.CurriculumParts
                        on subjectType.PartId equals part.CurriculumPartId into parts
                    from p in parts.DefaultIfEmpty()
                    select new SubjectTypeDropdownViewModel
                    {
                        Value = subjectType.SubjectTypeId,
                        Name = subjectType.Name,
                        Text = subjectType.Name,
                        BasicSubjectTypeId = subjectType.BasicSubjectTypeId,
                        PartId = subjectType.PartId,
                        PartName = p.Name,
                        IsValid = subjectType.IsValid ?? false
                    })
                    .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetRepeaterReasonOptionsAsync()
        {
            var repeaterReasons = await _context.RepeaterReasons.Select(x => new DropdownViewModel
            {
                Name = x.Name,
                Text = x.Description,
                Value = x.Id
            }).ToListAsync();

            return repeaterReasons;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetCommuterOptionsAsync()
        {
            var commuterOptions = await _context.CommuterTypes
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Name = x.Name,
                    Text = x.Description
                }).ToListAsync();
            return commuterOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetScholarshipTypeOptionsAsync()
        {
            var scholarshipTypeOptions = await _context.ScholarshipTypes
                .Select(x => new DropdownViewModel
                {
                    Name = x.Description,
                    Text = x.Description,
                    Value = x.Id,
                    RelatedObjectId = x.Periodicity ?? 0
                }).ToListAsync();

            return scholarshipTypeOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetScholarshipAmountOptionsAsync()
        {
            var scholarshipAmountOptions = await _context.ScholarshipAmounts
                .Select(x => new DropdownViewModel { Name = x.Description, Text = x.Description, Value = x.Id }).ToListAsync();

            return scholarshipAmountOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetStudentRelativeTypeOptionsAsync()
        {
            var relativeTypeOptions = await _context.RelativeTypes
                .Select(x => new DropdownViewModel
                {
                    Value = x.RelativeTypeId,
                    Name = x.Name,
                    Text = x.Name
                }).ToListAsync();

            return relativeTypeOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetStudentRelativeWorkStatusOptionsAsync()
        {
            List<DropdownViewModel> workStatusOptions = await _context.WorkStatuses
                .Select(x => new DropdownViewModel
                {
                    Value = x.WorkStatusId,
                    Name = x.Name,
                    Text = x.Name
                }).ToListAsync();

            return workStatusOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSpecialNeedsTypeOptionsAsync()
        {
            var specialNeedsTypes = await _context.SpecialNeedsTypes
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return specialNeedsTypes;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSpecialNeedsSubTypesOptionsAsync()
        {
            var specialNeedsSubTypes = await _context.SpecialNeedsSubTypes
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                    RelatedObjectId = x.SpecialNeedsTypeId
                }).ToListAsync();

            return specialNeedsSubTypes;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetResourceSupportSpecialistTypeOptionsAsync()
        {
            var resourceSupportSpecialistTypes = await _context.ResourceSupportSpecialistTypes
                .Where(x => x.IsValid)
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return resourceSupportSpecialistTypes;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetResourceSupportTypeOptionsAsync()
        {
            var resourceSupportTypeOptions = await _context.ResourceSupportTypes
                .Where(x => x.IsValid)
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return resourceSupportTypeOptions;
        }

        public async Task<IEnumerable<InstitutionDropdownViewModel>> GetInstitutionsAsync(string searchStr, int? selectedValue, int? regionId)
        {
            List<InstitutionDropdownViewModel> institutions = await _cache.GetOrCreateAsync(CacheKeys.Institutions, entry =>
           {
               entry.SlidingExpiration = _slidingExpiration;
               entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
               entry.Priority = _cachePriority;

               return _context.Institutions
                   .AsNoTracking()
                   .Select(i => new InstitutionDropdownViewModel
                   {
                       Value = i.InstitutionId,
                       Text = $"{i.InstitutionId} - {i.Name} гр./с.{i.Town.Name} общ.{i.Town.Municipality.Name} обл.{i.Town.Municipality.Region.Name}",
                       Name = $"{i.InstitutionId} - {i.Name} гр./с.{i.Town.Name} общ.{i.Town.Municipality.Name} обл.{i.Town.Municipality.Region.Name}",
                       ClearName = i.Name,
                       BaseSchoolTypeId = i.BaseSchoolTypeId,
                       LocalArea = i.LocalArea.Name,
                       Municipality = i.Town.Municipality.Name,
                       Region = i.Town.Municipality.Region.Name,
                       RegionId = i.Town.Municipality.RegionId,
                       Town = i.Town.Name,
                       DetailedSchoolTypeId = i.DetailedSchoolTypeId,
                       DetailedSchoolTypeName = i.DetailedSchoolType.Name,
                   }).ToListAsync();
           });

            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            // Ако selectedValue има стойност то трябва да върнем поне тази опция, независимо от другите условия.
            // Ако regionId има стойност трябва да филтрираме.
            //IQueryable<Institution> listQuery = _context.Institutions
            //    .AsNoTracking()
            //    .Where(i => (selectedValue.HasValue && i.InstitutionId == selectedValue.Value) ||
            //    ((regionId == null || i.Town.Municipality.RegionId == regionId)
            //        && (query == null || i.Name.Contains(query) || i.InstitutionId.ToString().Contains(query))
            //    ));

            var listCachedQuery = institutions
                          .Where(i => (selectedValue.HasValue && i.Value == selectedValue.Value) ||
                          ((regionId == null || i.RegionId == regionId)
                              && (query == null || i.ClearName.Contains(query, StringComparison.OrdinalIgnoreCase) || i.Value.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
                          ));

            return listCachedQuery;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetStudentTypeOptionsAsync()
        {
            var studentTypeOptions = await _context.StudentTypes.Select(x => new DropdownViewModel
            {
                Name = x.Name,
                Text = x.Description,
                Value = x.Id,
            }).ToListAsync();

            return studentTypeOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSupportPeriodOptionsAsync()
        {
            var supportPeriodOptions = await _context.SupportPeriods
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return supportPeriodOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetEarlyEvaluationReasonOptionsAsync()
        {
            var earlyEvaluationReasonOptions = await _context.EarlyEvaluationReasons
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return earlyEvaluationReasonOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetCommonSupportTypeOptionsAsync()
        {
            var commonSupportTypeOptions = await _context.CommonSupportTypes
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return commonSupportTypeOptions;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetAdditionalSupportTypeOptionsAsync()
        {
            var additionalSupportType = await _context.AdditionalSupportTypes
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return additionalSupportType;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetEducationTypeOptionsAsync()
        {
            var additionalSupportType = await _context.EducationTypes
                .Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Description,
                    Value = x.Id,
                }).ToListAsync();

            return additionalSupportType;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSubjectOptionsAsync(string searchStr, int? selectedValue, int? pageSize)
        {
            List<DropdownViewModel> subjects = null;

            string query = (searchStr.IsNullOrWhiteSpace() || searchStr == "[object Object]") ? null : searchStr.Trim();

            IOrderedQueryable<DropdownViewModel> dbQuery;

            if (_userInfo.InstitutionID != null)
            {
                // в случай, че имаме институция избираме само тези предмети
                dbQuery = _context.SubjectInstitutions
                    .Where(s => s.InstitutionId == _userInfo.InstitutionID)
                    .Where(s => (selectedValue.HasValue && s.SubjectId == selectedValue.Value) || query == null || s.SubjectName.Contains(query) || s.SubjectId.ToString().Contains(query))
                    .Select(s => new DropdownViewModel
                    {
                        Value = s.SubjectId,
                        Text = s.SubjectName,
                        Name = s.SubjectName,
                        // IsAvailable го ползваме че да си подсигурим избрания елемент в dropdowna при Take(pageSize.Value)
                        IsAvailable = s.SubjectId == selectedValue
                    }).Take(200).OrderByDescending(x => x.IsAvailable).ThenBy(x => x.Value);
            }
            else
            {
                dbQuery = _context.Subjects
                    .Where(s => (selectedValue.HasValue && s.SubjectId == selectedValue.Value) || query == null || s.SubjectName.Contains(query) || s.SubjectId.ToString().Contains(query))
                    .Select(s => new DropdownViewModel
                    {
                        Value = s.SubjectId,
                        Text = s.SubjectName,
                        Name = s.SubjectName,
                        // IsAvailable го ползваме че да си подсигурим избрания елемент в dropdowna при Take(pageSize.Value)
                        IsAvailable = s.SubjectId == selectedValue
                    }).Take(200).OrderByDescending(x => x.IsAvailable).ThenBy(x => x.Value);
            }

            if (pageSize.HasValue)
            {
                dbQuery = dbQuery.Take(pageSize.Value).OrderByDescending(x => x.IsAvailable).ThenBy(x => x.Value);
            }

            subjects = await dbQuery.ToListAsync();
            if (!subjects.Any(i => selectedValue != null && i.Value == selectedValue))
            {
                // Ако избраният предмет не съществува в списъка, то го добавяме от общата номенклатура Subject
                var selectedSubject = await (
                    from s in _context.Subjects
                    where s.SubjectId == selectedValue
                    select new DropdownViewModel
                    {
                        Value = s.SubjectId,
                        Text = s.SubjectName,
                        Name = s.SubjectName,
                        // IsAvailable го ползваме че да си подсигурим избрания елемент в dropdowna при Take(pageSize.Value)
                        IsAvailable = s.SubjectId == selectedValue
                    }).FirstOrDefaultAsync();
                if (selectedSubject != null)
                {
                    subjects.Add(selectedSubject);
                    subjects = subjects.Take(200).OrderByDescending(x => x.IsAvailable).ThenBy(x => x.Value).ToList();
                }
            }

            return subjects;
        }

        public async Task<IEnumerable<SubjectDetailsDropdownViewModel>> GetSubjectDetailsOptionsAsync()
        {

            var subjectInstitutions = await _context.SubjectInstitutions
                .Where(s => s.InstitutionId == _userInfo.InstitutionID)
                .Select(x => new { x.SubjectId, x.SubjectName })
                .ToListAsync();

            var subjectTypes = await _context.SubjectTypes
                .AsNoTracking()
                .Where(st => st.PartId == (int)Models.Enums.CurriculumPart.SectionA || st.PartId == (int)Models.Enums.CurriculumPart.SectionB
                    || st.PartId == (int)Models.Enums.CurriculumPart.SectionV || st.PartId == (int)Models.Enums.CurriculumPart.SectionG)
                .Select(x => new { x.SubjectTypeId, x.Name, x.PartId })
                .ToListAsync();

            List<SubjectDetailsDropdownViewModel> result = new List<SubjectDetailsDropdownViewModel>();

            int index = 0;
            foreach (var subjectInstitution in subjectInstitutions)
            {
                foreach (var subjectType in subjectTypes)
                {
                    result.Add(new SubjectDetailsDropdownViewModel
                    {
                        Value = index,
                        SubjectId = subjectInstitution.SubjectId,
                        Text = $"{subjectType.Name} - {subjectInstitution.SubjectName}",
                        Name = $"{subjectType.Name} - {subjectInstitution.SubjectName}",
                        SubjectTypeId = subjectType.SubjectTypeId,
                        PartId = subjectType.PartId
                    });

                    index++;
                }
            }

            return result;
        }

        public Task<List<DropdownViewModel>> GetSubjectsForLoggedInstitution(string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();
            FormattableString queryString = $"select * from student.fn_subjects_for_institution({_userInfo.InstitutionID},{selectedValue})";

            return _context.SubjectsForInstitution.FromSqlInterpolated(queryString)
                .Where(s => (selectedValue.HasValue && s.SubjectId == selectedValue.Value) || query == null || s.SubjectName.Contains(query))
                .Select(s => new DropdownViewModel
                {
                    Value = s.SubjectId,
                    Text = s.SubjectName,
                    Name = s.SubjectName,
                }).ToListAsync();
        }

        public async Task<IEnumerable<NomenclatureViewModel>> GetNomenclatureDataAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("TableName cant be null or empty!");
            }

            List<NomenclatureViewModel> nomenclatureData = null;

            var result = await _context.NomenclatureTypes.FromSqlRaw($"select Id, Name, Description, IsValid, ValidFrom, ValidTo from student.{tableName}").ToListAsync();
            nomenclatureData = result.Select(c => new NomenclatureViewModel
            {
                Value = c.Id,
                Name = c.Name,
                Text = c.Description,
                IsValid = c.IsValid,
                ValidFrom = c.ValidFrom,
                ValidTo = c.ValidTo
            }).ToList();

            return nomenclatureData;
        }

        public async Task UpdateNomenclatureAsync(string tableName, NomenclatureViewModel nomenclature)
        {
            Type entityType = _context.Model.FindEntityType($"MON.DataAccess.{tableName}")?.ClrType;
            if (entityType == null) throw new ArgumentNullException(nameof(tableName), tableName);

            var entity = await _context.Query(entityType).Where($"x => x.Id == {nomenclature.Value}").SingleOrDefaultAsync();
            if (entity == null) throw new ArgumentNullException(nameof(tableName), tableName);

            UpdateEntity(entity, entityType, nomenclature);

            await SaveAsync();
        }

        public async Task DeleteNomenclatureAsync(string tableName, int id)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var entity = await _context.Query(tableName).Where($"x => x.Id == {id}").SingleOrDefaultAsync();

            _context.Remove(entity);
            await SaveAsync();
        }

        public async Task AddNomenclatureAsync(string tableName, NomenclatureViewModel nomenclature)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            Type entityType = _context.Model.FindEntityType($"MON.DataAccess.{tableName}").ClrType;
            if (entityType == null) throw new ArgumentNullException(nameof(tableName), tableName);

            object entity = Activator.CreateInstance(entityType);
            UpdateEntity(entity, entityType, nomenclature);

            _context.Add(entity);
            await SaveAsync();
        }

        private static void UpdateEntity(object entity, Type entityType, NomenclatureViewModel nomenclature)
        {
            IList<PropertyInfo> props = new List<PropertyInfo>(entityType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                switch (prop.Name)
                {
                    case "Name":
                        prop.SetValue(entity, nomenclature.Name);
                        break;
                    case "Description":
                        prop.SetValue(entity, nomenclature.Text);
                        break;
                    case "IsValid":
                        prop.SetValue(entity, nomenclature.IsValid);
                        break;
                    case "ValidFrom":
                        prop.SetValue(entity, nomenclature.ValidFrom);
                        break;
                    case "ValidTo":
                        prop.SetValue(entity, nomenclature.ValidTo);
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task<List<DropdownViewModel>> GetSchoolYears(int? institutionId, int? selectedValue)
        {
            if (institutionId.HasValue)
            {
                return (await _context.InstitutionSchoolYears
                    .AsNoTracking()
                    .Where(x => x.InstitutionId == institutionId.Value)
                    .OrderByDescending(x => x.SchoolYear)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.SchoolYear,
                        Text = x.SchoolYearNavigation.Name,
                        Name = x.SchoolYearNavigation.Name
                    })
                    .FromCacheAsync(new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = _slidingExpiration,
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval),
                        Priority = _cachePriority
                    }, "SchoolYearsNom")
                    ).ToList();
            }
            else
            {
                return await _cache.GetOrCreateAsync(CacheKeys.SchoolYearFromCurrentYearTable, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.SchoolYears
                     .FromSqlRaw("select CurrentYearID, [Name] from inst_basic.CurrentYear")
                     .AsNoTracking()
                     .OrderByDescending(x => x.CurrentYearID)
                     .Select(x => new DropdownViewModel
                     {
                         Value = x.CurrentYearID,
                         Text = x.Name,
                         Name = x.Name
                     })
                     .ToListAsync();
                });


            }
        }

        public Task<DropdownViewModel> GetRegionById(int? regionId)
        {
            return _context.Regions.AsNoTracking()
                .Where(x => x.RegionId == regionId)
                .Select(x => new DropdownViewModel
                {
                    Value = x.RegionId,
                    Text = x.Name,
                    Name = x.Name
                })
                .SingleOrDefaultAsync();
        }

        public Task<List<DropdownViewModel>> GetSchoolYearsForPerson(int personId)
        {
            IQueryable<int> schoolYearsFromStudentClasses = _context.StudentClasses
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => (int)x.SchoolYear)
                .Distinct();

            return _context.SchoolYears
                .FromSqlRaw("select CurrentYearID, [Name] from inst_basic.CurrentYear")
                .AsNoTracking()
                .Where(x => schoolYearsFromStudentClasses.Contains(x.CurrentYearID))
                .Select(x => new DropdownViewModel
                {
                    Value = x.CurrentYearID,
                    Text = x.Name,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GradeDropdownViewModel>> GetGradesAsync(string searchStr, int? selectedValue, bool? filterSpecialGrade)
        {
            string searchText = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();
            bool hasToFilterSpecialGrades = filterSpecialGrade ?? false;

            IEnumerable<GradeDropdownViewModel> grades = await _context.Grades
                .AsNoTracking()

                .Where(x => (selectedValue.HasValue && x.Id == selectedValue.Value) || searchText == null || x.Name.Contains(searchText) || x.Description.Contains(searchText))
                .Where(x => !hasToFilterSpecialGrades || !x.IsSpecialGrade)
                .Select(g => new GradeDropdownViewModel
                {
                    Name = g.Name,
                    Text = g.Description,
                    Value = g.Id,
                    IsSpecialGrade = g.IsSpecialGrade
                }).ToListAsync();

            return grades;
        }

        public async Task<IEnumerable<GradeDropdownViewModel>> GetFirstToThirdClassGradesAsync()
        {
            IEnumerable<GradeDropdownViewModel> grades = await _context.FirstToThirdClassGrades
                .AsNoTracking()
                .Select(g => new GradeDropdownViewModel
                {
                    Name = g.Name,
                    Text = g.Description,
                    Value = g.Id,
                    IsSpecialGrade = false
                })
                .ToListAsync();

            return grades;
        }

        public async Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetBasicDocumentTypes(string searchStr, bool? schemaSpecified,
            bool? isValidation, bool? isIncludedInRegister, bool? isAppendix, string selectedValue, bool? isRuoDoc, bool? filterByDetailedSchoolType, int[] mainBasicDocuments)
        {
            if (searchStr.IsNullOrWhiteSpace())
            {
                searchStr = null;
            }
            else
            {
                searchStr = searchStr.Trim();
            }

            IQueryable<BasicDocument> query = await GetBasicDocumentQuery(false);

            if (schemaSpecified == true)
            {
                query = query.Where(x => x.Contents != null);
            }

            if (isValidation.HasValue)
            {
                query = query.Where(x => x.IsValidation == isValidation.Value);
            }

            if (isIncludedInRegister.HasValue)
            {
                query = query.Where(x => x.IsIncludedInRegister == isIncludedInRegister.Value);
            }

            if (isAppendix.HasValue)
            {
                query = query.Where(x => x.IsAppendix == isAppendix.Value);
            }

            if (isRuoDoc.HasValue)
            {
                query = query.Where(x => x.IsRuoDoc == isRuoDoc.Value);
            }

            // https://github.com/Neispuo/students/issues/1373
            /* 
                • В BasicDocument да се добави ново поле „MainBasicDocumentId” за описание на BasicDocumentId-тата на оригиналните документи, свързани с дубликата или на BasicDocumentId-тата на основните документи, свързани с приложението. Може да се ползва едно поле, защото имаме полета за IsDuplicate и IsAppendix.
                • При Създаване на нов дубликат или приложение, в Свързани документи да може да се избират само настроените в новото поле документи.
             */
            if (!_userInfo.IsConsortium && (mainBasicDocuments ?? Array.Empty<int>()).Length > 0)
            {
                query = query.Where(x => mainBasicDocuments.Contains(x.Id));
            }

            /* https://github.com/Neispuo/students/issues/1377
                ДФ 7 - връзка между вида на документа и вида на институцията
                В BasicDocument да се добави поле DetailedSchoolTypes, което да носи информация кой документ може да се издава от институция с даден DetailedSchoolType. При създаване на нов документ в падащото меню за вид документ, да се показват само тези видове, които съответстват на вида на училището.
            */
            if (_userInfo.InstitutionID.HasValue && (filterByDetailedSchoolType ?? false))
            {
                InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(_userInfo?.InstitutionID ?? default);
                int? institutionDetailedSchoolType = institution?.DetailedSchoolTypeId;
                if (institutionDetailedSchoolType.HasValue)
                {
                    query = query.Where(x => x.DetailedSchoolTypes == null || x.DetailedSchoolTypes.Contains($"|{institutionDetailedSchoolType}|"));
                }

            }

            string[] splitStr = (selectedValue ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> selectedIds = splitStr.ToHashSet<int>();

            if (selectedIds.Count > 0)
            {
                query = query.Where(x => searchStr == null || x.Name.Contains(searchStr));

            }
            else
            {
                query = query.Where(x => selectedIds.Contains(x.Id) || searchStr == null || x.Name.Contains(searchStr));
            }


            return await query
                .Select(g => new DiplomaTemplateDropdownViewModel
                {
                    Value = g.Id,
                    Name = g.Name,
                    Text = g.Description,
                    BasicClassesStr = g.BasicClasses,
                })
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetBasicDocumentTypesWithJsonContent()
        {
            IQueryable<BasicDocument> query = await GetBasicDocumentQuery(true);

            var basicDocumentTypes = await (from bd in query.AsNoTracking()
                                            join t in _context.Templates.AsNoTracking() on new { ColA = bd.Id, ColB = bd.Name } equals new { ColA = t.BasicDocumentId, ColB = t.Name }
                                            into temp
                                            from tt in temp.DefaultIfEmpty()
                                            where bd.Contents != null
                                            select new DiplomaTemplateDropdownViewModel
                                            {
                                                Value = bd.Id,
                                                Name = bd.Name,
                                                Text = bd.Description,
                                                BasicDocumentContentsJsonStr = bd.Contents,
                                                DocumentTemplatePartsWithSubjects = !string.IsNullOrEmpty(tt.SubjectContents) ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<DiplomaTemplateSubjectPartModel>>(tt.SubjectContents) : new List<DiplomaTemplateSubjectPartModel>(),
                                                DocumentParts = bd.BasicDocumentParts.Select(x => new BasicDocumentPartModel
                                                {
                                                    Id = x.Id,
                                                    Name = x.Name,
                                                    IsHorariumHidden = x.IsHorariumHidden,
                                                    Position = x.Position,
                                                }),
                                            }).ToListAsync();

            foreach (var docType in basicDocumentTypes)
            {
                foreach (var part in docType.DocumentParts)
                {
                    var templatePart = docType.DocumentTemplatePartsWithSubjects.SingleOrDefault(x => x.BasicDocumentPartId == part.Id && x.BasicDocumentPartName == part.Name);

                    if (templatePart == null)
                    {
                        var list = docType.DocumentTemplatePartsWithSubjects.ToList();

                        list.Add(new DiplomaTemplateSubjectPartModel
                        {
                            BasicDocumentPartId = part.Id ?? 0,
                            BasicDocumentPartName = part.Name,
                            IsHorariumHidden = part.IsHorariumHidden ?? false,
                            Position = part.Position,
                            Subjects = part.BasicDocumentSubjects != null ? part.BasicDocumentSubjects.Select(x => new DiplomaTemplateSubjectModel
                            {
                                Position = x.Position,
                                SubjectCanChange = x.SubjectCanChange,
                                SubjectDropdown = new DropdownViewModel { Value = x.SubjectDropDown.Value, Text = x.SubjectDropDown.Text, Name = x.SubjectDropDown.Name },
                                // SubjectTypeDropdown = new DropdownViewModel { Value = x }
                            }).ToList() : new List<DiplomaTemplateSubjectModel>()
                        });

                        docType.DocumentTemplatePartsWithSubjects = list.OrderBy(x => x.Position);
                    }
                }
            }

            return basicDocumentTypes;
        }

        /// <summary>
        /// Връща IQueryable<BasicDocument.
        /// Ако ролята на логнатия потребител е "Институция (директор)" или "Учител" ще филтрираме само тези документи,
        /// за които има запис в таблица BasicDocumentLimits(свързваща таблица между BasicDocument и DetailedSchoolTypeId).
        /// </summary>
        /// <returns>IQueryable<BasicDocument</returns>
        private async Task<IQueryable<BasicDocument>> GetBasicDocumentQuery(bool filterByLimitTable)
        {
            IQueryable<BasicDocument> query = _context.BasicDocuments.Where(x => x.IsValid);

            if (filterByLimitTable && _userInfo.InstitutionID.HasValue)
            {
                int detailedSchoolTypeId = await _institutionService.GetInstitutionDetailedSchoolTypeId(_userInfo.InstitutionID.Value);

                query = query.Where(x => x.BasicDocumentLimits.Any(bd => bd.DetailedSchoolTypeId == detailedSchoolTypeId));
            }

            return query;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetLocalAreasOptions()
        {
            return await _context.LocalAreas
                .AsNoTracking()
                .Select(g => new DropdownViewModel
                {
                    Name = g.Name,
                    Text = g.Name,
                    Value = g.LocalAreaId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSpecialityOptions()
        {
            return await _context.Sppoospecialities
                .AsNoTracking()
                .Select(g => new DropdownViewModel
                {
                    Name = g.Name,
                    Text = g.Name,
                    Value = g.SppoospecialityId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetValidSpecialityOptions()
        {
            return await _context.Sppoospecialities
                .AsNoTracking()
                .Where(s => s.IsValid.Value == true && s.SppoospecialityId == -1)
                .Select(g => new DropdownViewModel
                {
                    Name = g.Name,
                    Text = g.Name,
                    Value = g.SppoospecialityId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetResourceSupportSpecialistWorkPlaces()
        {
            return await _context.ResourceSupportSpecialistWorkPlaceToResourceSupportTypes
                .OrderBy(x => x.ResourceSupportSpecialistWorkPlaceId)
                .ThenBy(x => x.ResourceSupportSpecialistWorkPlaceId)
                .Select(x => new DropdownViewModel
                {
                    Value = x.ResourceSupportSpecialistWorkPlaceId,
                    Name = x.ResourceSupportSpecialistWorkPlace.Name,
                    Text = x.ResourceSupportSpecialistWorkPlace.Name,
                    RelatedObjectId = x.ResourceSupportTypeId
                })
                .ToListAsync();
        }

        /// <summary>
        /// Шаблони, чиито BasicDocument е IsValidation == false т.е. не се използват за Валидация
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetCurrentUserDiplomaTemplates()
        {
            IQueryable<Institution> currentUserInstitutions = GetInstitutionsListQuery();

            return await _context.Templates
               .Where(t => t.BasicDocument.IsValidation == false
                    && currentUserInstitutions.Any(i => i.InstitutionId == t.InstitutionId))
               .AsNoTracking()
               .Select(t => new DiplomaTemplateDropdownViewModel
               {
                   Name = t.Name,
                   Text = t.Description,
                   Value = t.Id,
                   BasicDocumentContentsJsonStr = t.BasicDocument.Contents,
                   TemplateContentsJsonStr = t.Contents
               })
               .ToListAsync();
        }

        /// <summary>
        /// Шаблони, чиито BasicDocument е IsValidation == true т.е. използват се за Валидация
        /// </summary>
        /// <returns></returns>
        public Task<List<DiplomaTemplateDropdownViewModel>> GetCurrentUserValidationTemplates()
        {
            IQueryable<Institution> currentUserInstitutions = GetInstitutionsListQuery();

            return _context.Templates
               .Where(t => t.BasicDocument.IsValidation
                    && currentUserInstitutions.Any(i => i.InstitutionId == t.InstitutionId))
               .AsNoTracking()
               .Select(t => new DiplomaTemplateDropdownViewModel
               {
                   Name = t.Name,
                   Text = t.Description,
                   Value = t.Id,
                   BasicDocumentContentsJsonStr = t.BasicDocument.Contents,
                   TemplateContentsJsonStr = t.Contents
               })
               .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetLodEvaluationResults()
        {
            return await _context.LodEvaluationsResults
               .AsNoTracking()
               .Select(r => new DropdownViewModel
               {
                   Name = r.Name,
                   Text = r.Description,
                   Value = r.Id,
               })
               .ToListAsync();
        }

        private IQueryable<Institution> GetInstitutionsListQuery()
        {
            IQueryable<Institution> query = _context.Institutions;

            return _userInfo.SysRoleID switch
            {
                (int)UserRoleEnum.School => query.Where(i => i.InstitutionId == _userInfo.InstitutionID),
                (int)UserRoleEnum.Mon => query,
                (int)UserRoleEnum.Ruo => query.Where(i => i.Town.Municipality.RegionId == _userInfo.RegionID),
                _ => query.Where(i => i.InstitutionId == int.MinValue),
            };
        }

        public Task<List<DropdownViewModel>> GetBasicSubjectTypeOptions()
        {
            return _context.BasicSubjectTypes
                .AsNoTracking()
                .Where(x => x.IsValid == true)
                .Select(x => new DropdownViewModel
                {
                    Value = x.BasicSubjectTypeId,
                    Name = x.Name,
                    Text = x.Abrev
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetMinistryOptions()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.MinistryOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Ministries.AsNoTracking()
                    .Where(x => x.IsValid == true)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.Id,
                        Name = x.Name,
                        Text = x.Name
                    })
                    .ToListAsync();
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetLodEvaluationsProfileClasses(int personId, short schoolYear)
        {
            IEnumerable<DropdownViewModel> profileClasses = await _context.CurriculumStudents
                .AsNoTracking()
                .Where(x => x.PersonId == personId && x.Curriculum.SchoolYear == schoolYear && x.Curriculum.ParentCurriculumId.HasValue)
                .OrderBy(x => x.Curriculum.CurriculumPartId)
                .ThenBy(x => x.Curriculum.ParentCurriculumId)
                .Select(x => new DropdownViewModel
                {
                    Text = $"{x.Curriculum.SubjectType.Name} - {x.Curriculum.Subject.SubjectName}",
                    Value = x.CurriculumId
                })
                .ToListAsync();

            return profileClasses;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetStudentAwardTypes()
        {
            return await _context.AwardTypes
                .AsNoTracking()
                .Where(x => x.IsValid == true)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Name = x.Name,
                    Text = x.Description
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetNaturalIndicatorsPeriods()
        {
            if (_userInfo.IsMon || _userInfo.IsMonExpert)
            {
                var results = await _context.NaturalIndicators
                    .AsNoTracking()
                    .GroupBy(x => new { x.SchoolYear, x.Period })
                    .Select(g => new { g.Key.SchoolYear, g.Key.Period })
                    .OrderByDescending(x => x.SchoolYear)
                    .ThenBy(x => x.Period)
                    .ToListAsync();

                return results.Select(x => new DropdownViewModel
                {
                    Value = x.SchoolYear * 10 + x.Period,
                    Name = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период" + x.Period.ToString())}",
                    Text = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период " + x.Period.ToString())}"
                });
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var results = await _context.NaturalIndicators
                    .AsNoTracking()
                    .GroupBy(x => new { x.SchoolYear, x.Period })
                    .Select(g => new { g.Key.SchoolYear, g.Key.Period })
                    .OrderByDescending(x => x.SchoolYear)
                    .ThenBy(x => x.Period)
                    .ToListAsync();

                return results.Select(x => new DropdownViewModel
                {
                    Value = x.SchoolYear * 10 + x.Period,
                    Name = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период" + x.Period.ToString())}",
                    Text = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период " + x.Period.ToString())}"
                });
            }
            else if (_userInfo.IsSchoolDirector) 
            {            
                var results = await _context.NaturalIndicators
                    .AsNoTracking()
                    .Where(x => x.InstitutionId == _userInfo.InstitutionID.Value)
                    .GroupBy(x => new { x.SchoolYear, x.Period })
                    .Select(g => new { g.Key.SchoolYear, g.Key.Period })
                    .OrderByDescending(x => x.SchoolYear)
                    .ThenBy(x => x.Period)
                    .ToListAsync();

                return results.Select(x => new DropdownViewModel
                {
                    Value = x.SchoolYear * 10 + x.Period,
                    Name = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период" + x.Period.ToString())}",
                    Text = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период " + x.Period.ToString())}"
                });
            }
            else
            {
                return new List<DropdownViewModel>();
            }
        }

        public async Task<IEnumerable<DropdownViewModel>> GetResourceSupportDataPeriods()
        {
            if (_userInfo.IsMon || _userInfo.IsMonExpert)
            {
                var results = await _context.ResourceSupportData
                    .AsNoTracking()
                    .GroupBy(x => new { x.SchoolYear, x.Period })
                    .Select(g => new { g.Key.SchoolYear, g.Key.Period })
                    .OrderByDescending(x => x.SchoolYear)
                    .ThenBy(x => x.Period)
                    .ToListAsync();

                return results.Select(x => new DropdownViewModel
                {
                    Value = x.SchoolYear * 10 + x.Period,
                    Name = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период" + x.Period.ToString())}",
                    Text = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период " + x.Period.ToString())}"
                });
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var results = await _context.ResourceSupportData
                    .AsNoTracking()
                    .GroupBy(x => new { x.SchoolYear, x.Period })
                    .Select(g => new { g.Key.SchoolYear, g.Key.Period })
                    .OrderByDescending(x => x.SchoolYear)
                    .ThenBy(x => x.Period)
                    .ToListAsync();

                return results.Select(x => new DropdownViewModel
                {
                    Value = x.SchoolYear * 10 + x.Period,
                    Name = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период" + x.Period.ToString())}",
                    Text = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период " + x.Period.ToString())}"
                });
            }
            else if (_userInfo.IsSchoolDirector)
            {
                var results = await _context.ResourceSupportData
                    .AsNoTracking()
                    .Where(x => x.InstitutionId == _userInfo.InstitutionID.Value)
                    .GroupBy(x => new { x.SchoolYear, x.Period })
                    .Select(g => new { g.Key.SchoolYear, g.Key.Period })
                    .OrderByDescending(x => x.SchoolYear)
                    .ThenBy(x => x.Period)
                    .ToListAsync();

                return results.Select(x => new DropdownViewModel
                {
                    Value = x.SchoolYear * 10 + x.Period,
                    Name = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период" + x.Period.ToString())}",
                    Text = $"{x.SchoolYear} / {(x.Period == 0 ? "текущи данни" : "Период " + x.Period.ToString())}"
                });
            }
            else
            {
                return new List<DropdownViewModel>();
            }
        }

        public async Task<IEnumerable<DropdownViewModel>> GetStudentSanctionTypes()
        {
            return await _context.SanctionTypes
                .AsNoTracking()
                .Where(x => x.IsValid == true)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Name = x.Name,
                    Text = x.Description
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetAwardCategories()
        {
            return await _context.AwardCategories.AsNoTracking()
                           .Where(x => x.IsValid == true)
                           .Select(x => new DropdownViewModel
                           {
                               Value = x.Id,
                               Name = x.Name,
                               Text = x.Description
                           })
                           .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetFounders()
        {
            return await _context.Founders.AsNoTracking()
                           .Where(x => x.IsValid == true)
                           .Select(x => new DropdownViewModel
                           {
                               Value = x.Id,
                               Name = x.Name,
                               Text = x.Description
                           })
                           .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetAwardReasons()
        {
            return await _context.AwardReasons.AsNoTracking()
                           .Where(x => x.IsValid == true)
                           .Select(x => new DropdownViewModel
                           {
                               Value = x.Id,
                               Name = x.Name,
                               Text = x.Description
                           })
                           .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetScholarshipFinancingOrgans()
        {
            return await _context.ScholarshipFinancingOrgans.AsNoTracking()
                           .Where(x => x.IsValid == true)
                           .Select(x => new DropdownViewModel
                           {
                               Value = x.Id,
                               Name = x.Name,
                               Text = x.Description
                           })
                           .ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSupportingEquipment()
        {
            return await _context.EquipmentTypes.AsNoTracking()
                           .Where(x => x.IsValid == true && x.IsSpecial == true)
                           .Select(x => new DropdownViewModel
                           {
                               Value = x.EquipmentTypeId,
                               Name = x.EquipmentTypeName,
                               Text = x.EquipmentTypeName + " - " + x.EquipmentTypeId
                           })
                           .ToListAsync();
        }

        public Task<List<DropdownViewModel>> GetRecognitionEducationLevel(string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            return _context.RecognitionEducationLevels.AsNoTracking()
                .Where(x => x.IsValid == true)
                .Where(x => (selectedValue.HasValue && x.Id == selectedValue.Value) || query == null || x.Name.Contains(query))
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Name = x.Name,
                    Text = x.Name
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetSPPOOProfession(int? relatedObject, string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            var prof = await _context.Sppooprofessions.AsNoTracking()
                .Where(x => x.IsValid == true)
                .Where(x => relatedObject == null || x.ProfAreaId == relatedObject)
                .Where(x => (selectedValue.HasValue && x.SppooprofessionId == selectedValue.Value) || query == null || x.Name.Contains(query) || x.SppooprofessionCode.Contains(query))
                .Select(x => new DropdownViewModel
                {
                    Value = x.SppooprofessionId,
                    Name = x.Name,
                    Text = $"{x.SppooprofessionCode} - {x.Name}",
                    RelatedObjectId = x.ProfAreaId
                })
                .ToListAsync();

            return prof;
        }

        public Task<List<SPPOOSpecialityDropdownViewModel>> GetSPPOOSpeciality(int? relatedObject, string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            return _context.Sppoospecialities.AsNoTracking()
                .Where(x => x.IsValid == true)
                .Where(x => !relatedObject.HasValue || x.ProfessionId == relatedObject)
                .Where(x => (selectedValue.HasValue && x.SppoospecialityId == selectedValue.Value) || query == null || x.Name.Contains(query) || x.SppoospecialityCode.Contains(query))
                .Select(x => new SPPOOSpecialityDropdownViewModel
                {
                    Value = x.SppoospecialityId,
                    Name = x.Name,
                    Text = $"{x.SppoospecialityCode} - {x.Name}",
                    RelatedObjectId = x.ProfessionId,
                    VetLevel = x.Vetlevel
                })
                .ToListAsync();
        }


        public async Task<List<DropdownViewModel>> GetSPPOOSpecialityExtendedText(int? relatedObject, string searchStr, int? selectedValue)
        {
            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            var a = await _context.Sppoospecialities.AsNoTracking()
                .Where(x => x.IsValid == true)
                .Where(x => !relatedObject.HasValue || x.ProfessionId == relatedObject)
                .Where(x => (selectedValue.HasValue && x.SppoospecialityId == selectedValue.Value) || query == null || x.Name.Contains(query) || x.SppoospecialityCode.Contains(query))
                .Select(x => new DropdownViewModel
                {
                    Value = x.SppoospecialityId,
                    Name = x.Name,
                    Text = $"{x.SppoospecialityCode} {x.Name} СПК {x.Vetlevel}",
                    RelatedObjectId = x.ProfessionId
                })
                .ToListAsync();

            return a;
        }

        public Task<List<DropdownViewModel>> GetExamTypeOptions()
        {
            return Task.FromResult(new List<DropdownViewModel> {
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.Written,
                    Name = ExamTypeEnum.Written.ToString(),
                    Text = ExamTypeEnum.Written.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.Oral,
                    Name = ExamTypeEnum.Oral.ToString(),
                    Text = ExamTypeEnum.Oral.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.Practical,
                    Name = ExamTypeEnum.Practical.ToString(),
                    Text = ExamTypeEnum.Practical.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.WrittenOral,
                    Name = ExamTypeEnum.WrittenOral.ToString(),
                    Text = ExamTypeEnum.WrittenOral.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.WrittenPractical,
                    Name = ExamTypeEnum.WrittenPractical.ToString(),
                    Text = ExamTypeEnum.WrittenPractical.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.TermGrade,
                    Name = ExamTypeEnum.TermGrade.ToString(),
                    Text = ExamTypeEnum.TermGrade.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.FinalGrade,
                    Name = ExamTypeEnum.FinalGrade.ToString(),
                    Text = ExamTypeEnum.FinalGrade.GetEnumDescription()
                },
                new DropdownViewModel
                {
                    Value = (int)ExamTypeEnum.AnnualGrade,
                    Name = ExamTypeEnum.AnnualGrade.ToString(),
                    Text = ExamTypeEnum.AnnualGrade.GetEnumDescription()
                },

            });
        }

        public Task<List<DropdownViewModel>> GetBuildingAreas(string searchStr, int? selectedValue)
        {
            var query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            return _context.BuildingAreaTypes.AsNoTracking()
                .Where(x => x.IsValid == true)
                .Where(x => (selectedValue.HasValue && x.BuildingAreaTypeId == selectedValue.Value) || query == null || x.BuildingAreaTypeName.Contains(query))
                .Select(x => new DropdownViewModel
                {
                    Value = x.BuildingAreaTypeId,
                    Name = x.BuildingAreaTypeName,
                    Text = x.BuildingAreaTypeName
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetBuildingRooms(string searchStr, int? selectedValue, string buildingAreas)
        {
            HashSet<int> filters = (buildingAreas ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !x.IsNullOrWhiteSpace())
                .Select(x => int.TryParse(x, out int val) ? val : 0)
                .ToHashSet();

            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            var rooms = await _context.VRoomsAndEquipments.AsNoTracking()
                                                     .Where(x => filters.Contains(x.BuildingAreaTypeId))
                                                     .Where(x => (selectedValue.HasValue && x.BuildingAreaTypeId == selectedValue.Value) || query == null || x.Room.Contains(query))
                                                     .Select(x => new DropdownViewModel
                                                     {
                                                         Value = x.BuildingRoomTypeId,
                                                         Name = x.Room,
                                                         Text = x.Room,
                                                         RelatedObjectId = x.BuildingAreaTypeId
                                                     }).ToListAsync();

            return rooms;
        }

        public async Task<List<DropdownViewModel>> GetSpecialEquipment(string searchStr, int? selectedValue, string buildingRooms)
        {
            HashSet<int> filters = (buildingRooms ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !x.IsNullOrWhiteSpace())
                .Select(x => int.TryParse(x, out int val) ? val : 0)
                .ToHashSet();

            string query = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            var equipments = await _context.VRoomsAndEquipments.AsNoTracking()
                                                     .Where(x => filters.Contains(x.BuildingRoomTypeId))
                                                     .Where(x => (selectedValue.HasValue && x.BuildingRoomTypeId == selectedValue.Value) || query == null || x.Equipment.Contains(query))
                                                     .Select(x => new DropdownViewModel
                                                     {
                                                         Value = x.EquipmentTypeId,
                                                         Name = x.Equipment,
                                                         Text = x.Equipment,
                                                         RelatedObjectId = x.BuildingRoomTypeId,
                                                         IsAvailable = x.IsAvailable == 1
                                                     }).ToListAsync();

            return equipments;
        }

        public async Task<List<DropdownViewModel>> GetAvailableArchitecture()
        {
            return await _context.VAvailableArchitectures.AsNoTracking().Select(x => new DropdownViewModel
            {
                Value = x.ModernizationDegreeId,
                Name = x.Name,
                Text = x.Name
            })
            .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetFLLevelOptions()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.FlLevelOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.Fllevels
                    .AsNoTracking()
                    .Where(x => x.IsValid == true)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.FllevelId,
                        Name = x.Name,
                        Text = x.Name
                    })
                    .ToListAsync();
                })
                : await _context.Fllevels
                     .AsNoTracking()
                    .Where(x => x.IsValid == true)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.FllevelId,
                        Name = x.Name,
                        Text = x.Name
                    })
                    .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetITLevelOptions()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.ItLevelOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.Itlevels
                        .AsNoTracking()
                        .Where(x => x.IsValid)
                        .Select(x => new DropdownViewModel
                        {
                            Value = x.Id,
                            Name = x.Name,
                            Text = x.Name
                        })
                        .ToListAsync();
                })
                : await _context.Itlevels
                    .AsNoTracking()
                    .Where(x => x.IsValid)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.Id,
                        Name = x.Name,
                        Text = x.Name
                    })
                    .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetStudentParticipations()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.StudentParticipationOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Participations.AsNoTracking()
                    .Where(x => x.IsValid == true)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.Id,
                        Name = x.Name,
                        Text = x.Description
                    })
                    .ToListAsync();
            });
        }

        public async Task<List<DropdownViewModel>> GetStudentSelfGovernmentPositions()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.SelfGovernmentPositionOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.SelfGovernmentPositions.AsNoTracking()
                    .Where(x => x.IsValid == true)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.Id,
                        Name = x.Name,
                        Text = x.Description
                    })
                    .ToListAsync();
            });
        }

        public Task<List<DropdownViewModel>> GetMonStatusOptions()
        {
            var options = new List<DropdownViewModel>
            {
                new DropdownViewModel { Text = NEISPUOStatus.UnderReview.GetEnumDescription(), Value = (int)NEISPUOStatus.UnderReview, Name = NEISPUOStatus.UnderReview.GetEnumDescription() },
                new DropdownViewModel { Text = NEISPUOStatus.Rejected.GetEnumDescription(), Value = (int)NEISPUOStatus.Rejected,  Name = NEISPUOStatus.Rejected.GetEnumDescription() },
                new DropdownViewModel { Text = NEISPUOStatus.Confirmed.GetEnumDescription(), Value = (int)NEISPUOStatus.Confirmed, Name = NEISPUOStatus.Confirmed.GetEnumDescription() }
            };
            return Task.FromResult(options);
        }

        public Task<List<DropdownViewModel>> GetAspStatusOptions()
        {
            var options = new List<DropdownViewModel>
            {
                new DropdownViewModel { Text = ASPStatusEnum.Absence.GetEnumDescription(), Value = (int)ASPStatusEnum.Absence, Name = ASPStatusEnum.Absence.GetEnumDescription() },
                new DropdownViewModel { Text = ASPStatusEnum.NonVisiting.GetEnumDescription(), Value = (int)ASPStatusEnum.NonVisiting,  Name =  ASPStatusEnum.NonVisiting.GetEnumDescription() },
                new DropdownViewModel { Text = ASPStatusEnum.Discharged.GetEnumDescription(), Value = (int)ASPStatusEnum.Discharged, Name = ASPStatusEnum.Discharged.GetEnumDescription() }
            };
            return Task.FromResult(options);
        }

        public async Task<List<ExternalEvaluationTypeModel>> GetExternalEvaluationTypeOptions()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.ExternalEvaluationTypeOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.ExternalEvaluationTypes
                     .AsNoTracking()
                     .Where(x => x.IsValid)
                     .Select(x => new ExternalEvaluationTypeModel
                     {
                         Id = x.Id,
                         Name = x.Name,
                         BasicClassId = x.BasicClassId,
                     })
                     .ToListAsync();
                })
                : await _context.ExternalEvaluationTypes
                     .AsNoTracking()
                     .Where(x => x.IsValid)
                     .Select(x => new ExternalEvaluationTypeModel
                     {
                         Id = x.Id,
                         Name = x.Name,
                         BasicClassId = x.BasicClassId,
                     })
                     .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetORESTypesOptions()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.ORESTypeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.ORESTypes
                .Where(x => x.IsValid)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Name = x.Name,
                    Text = x.Name
                }).ToListAsync();
            });
        }

        public async Task<List<DropdownViewModel>> GetStudentPositionOptions()
        {
            List<int> unselectablePositions = new List<int> { (int)PositionType.Staff, (int)PositionType.Discharged };

            return await _cache.GetOrCreateAsync(CacheKeys.StudentPositionOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval); ;
                entry.Priority = _cachePriority;

                return _context.Positions
                .Where(x => !unselectablePositions.Contains(x.PositionId))
                .Select(x => new DropdownViewModel
                {
                    Value = x.PositionId,
                    Name = x.Name,
                    Text = x.Name
                })
                .ToListAsync();
            });
        }

        /// <summary>
        /// <seealso cref="DropdownViewModel"/> на <seealso cref="Position"/> в зависимост от типа на институцията InstType от db view-то <see cref="InstitutionAll"/>
        /// Ако логнатия потребител е с роля SchoolDirector или Teacher взимаме неговата институция. Ако ли не взимаме подадената институция.
        /// Второто е валидно ако сме с роля Mon или Ruo. Тогава при създаване на документ за записване може да избираме институцията.
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        public async Task<List<DropdownViewModel>> GetStudentPositionOptionsByCondition(int? selectedValue, int? institutionId, int? personId)
        {
            if (_userInfo.IsSchoolDirector || _userInfo.IsTeacher)
            {
                institutionId = _userInfo.InstitutionID;
            }
            else if (_userInfo.IsRuo || _userInfo.IsMon || _userInfo.IsRuoExpert || _userInfo.IsMonExpert || _userInfo.IsCIOO || _userInfo.IsConsortium)
            {
                // TODO - тук може би трябва да се върнат всички позиции
                return null;
            }

            if (!institutionId.HasValue)
            {
                throw new ArgumentNullException(nameof(institutionId));
            }

            HashSet<int> allowedPositions = await _institutionService.GetAllowedStudentPositions(institutionId.Value);

            // Aко съм InstType 1 или 2 и правя документ за записване, да не мога да избера Позиция 7, ако няма 3.
            if (personId.HasValue)
            {
                InstitutionCacheModel institutionBaseMode = await _institutionService.GetInstitutionCache(institutionId.Value);
                if (institutionBaseMode != null && (institutionBaseMode.IsSchool || institutionBaseMode.IsKinderGarden)
                    && allowedPositions.Contains((int)PositionType.StudentOtherInstitution))
                {
                    if (!await _context.EducationalStates.AnyAsync(x => x.PersonId == personId.Value && x.PositionId == (int)PositionType.Student))
                    {
                        allowedPositions.RemoveWhere(x => x == (int)PositionType.StudentOtherInstitution || x == (int)PositionType.StudentPersDevelopmentSupport);
                    }
                }
            }

            return await _context.Positions
                .Where(x => (selectedValue.HasValue && x.PositionId == selectedValue.Value) || allowedPositions.Contains(x.PositionId))
                .Select(x => new DropdownViewModel
                {
                    Value = x.PositionId,
                    Name = x.Name,
                    Text = x.Name
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetSpecialtiesForProfession(int professionId)
        {
            List<DropdownViewModel> SpecialtiesForProfession = await _context.Sppoospecialities
                .Where(s => s.ProfessionId == professionId)
              .Select(s => new DropdownViewModel
              {
                  Value = s.SppoospecialityId,
                  Name = s.Name,
                  Text = s.Name
              }).ToListAsync();

            return SpecialtiesForProfession;
        }

        public async Task<IEnumerable<AbsenceCampaignDropdownViewModel>> GetAbsenceCampaigns()
        {

            return await _context.AbsenceCampaigns
                .Select(ac => new AbsenceCampaignDropdownViewModel
                {
                    Value = ac.Id,
                    Name = ac.Name,
                    Text = ac.Name,
                    Month = ac.Month,
                    SchoolYear = ac.SchoolYear
                })
                .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetBasicDocumentOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = await _cache.GetOrCreateAsync(CacheKeys.BasicDocumentsOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.BasicDocuments.AsNoTracking()
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.Id,
                        Name = x.Name,
                        Text = x.Description,
                        IsValid = x.IsValid
                    })
                    .ToListAsync();
            });

            return allEntries.FilterByValid(valid);
        }

        public async Task<List<DropdownViewModel>> GetClassTypeOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = await _cache.GetOrCreateAsync(CacheKeys.ClassTypeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval); ;
                entry.Priority = _cachePriority;

                return _context.ClassTypes.AsNoTracking()
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.ClassTypeId,
                        Name = x.Name,
                        Text = $"{x.Description} - {x.Name}",
                        IsValid = x.IsValid ?? false
                    })
                    .ToListAsync();
            });

            return allEntries.FilterByValid(valid);
        }

        public async Task<List<DropdownViewModel>> GetClassTypeDiplomaOptions(ValidEnum valid)
        {
            var allEntries = await _cache.GetOrCreateAsync(CacheKeys.ClassTypeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.Priority = _cachePriority;

                return _context.ClassTypes
                    .AsNoTracking()
                    .Where(x => x.ClassKind == (int)ClassKindEnum.Basic)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.ClassTypeId,
                        Name = x.Name,
                        Text = $"{x.Description} - {x.Name}",
                        IsValid = x.IsValid ?? false
                    })
                    .ToListAsync();
            });

            return allEntries.FilterByValid(valid);
        }

        public async Task<List<DropdownViewModel>> GetSPPOOProfessionOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = await _cache.GetOrCreateAsync(CacheKeys.SPPOOProfessionOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Sppooprofessions.AsNoTracking()
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.SppooprofessionId,
                        Name = x.Name,
                        Text = x.Description,
                        IsValid = x.IsValid ?? false
                    })
                    .ToListAsync();
            });

            return allEntries.FilterByValid(valid);
        }

        public async Task<List<DropdownViewModel>> GetSPPOOSpecialityOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = await _cache.GetOrCreateAsync(CacheKeys.SPPOOSpecialityOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Sppoospecialities.AsNoTracking()
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.SppoospecialityId,
                        Name = x.Name,
                        Text = x.Description,
                        IsValid = x.IsValid ?? false
                    })
                    .ToListAsync();
            });

            return allEntries.FilterByValid(valid);
        }

        public async Task<DropdownViewModel> GetSubjectDetails(int id, ValidEnum valid)
        {
            return await _cache.GetOrCreateAsync($"_subjectdetails_{id}_{valid}", entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.Subjects.AsNoTracking()
                    .Where(x => x.SubjectId == id && (valid == ValidEnum.All || x.IsValid == Convert.ToBoolean(valid)))
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.SubjectId,
                        Name = x.SubjectName,
                        Text = x.SubjectNameShort
                    })
                    .SingleOrDefaultAsync();
            });
        }

        public async Task<BasicDocumentDetailsModel> GetBasicDocumentDetails(int id)
        {

            return UseCahce
                ? await _cache.GetOrCreateAsync($"{CacheKeys.BasicDocumentDetails}_{id}", entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval); ;
                    entry.Priority = _cachePriority;

                    return _context.BasicDocuments
                        .Where(x => x.Id == id)
                        .Select(x => new BasicDocumentDetailsModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            HasBarcode = x.HasBarcode,
                            HasFactoryNumber = x.HasFactoryNumber,
                            IsUniqueForStudent = x.IsUniqueForStudent,
                            HasSubjects = x.HasSubjects,
                            AttachedImagesCountMin = x.AttachedImagesCountMin,
                            AttachedImagesCountMax = x.AttachedImagesCountMax,
                            IsAppendix = x.IsAppendix,
                            IsDuplicate = x.IsDuplicate,
                            SeriesFormat = x.SeriesFormat,
                            MainBasicDocuments = x.MainBasicDocuments,
                            DetailedSchoolTypes = x.DetailedSchoolTypes,
                            IsIncludedInRegister = x.IsIncludedInRegister,
                            PageOrientation = x.PageOrientation != null ? (PageOrientationEnum)x.PageOrientation.Value : PageOrientationEnum.Unspecified,
                            DocumetPartsDetails = x.BasicDocumentParts
                            .Select(p => new BasicDocumentPartDetailsModel
                            {
                                Id = p.Id,
                                BasicDocumentId = p.BasicDocumentId,
                                SubjectTypesStr = p.SubjectTypesList,
                                ExternalEvaluationTypesStr = p.ExternalEvaluationTypesList
                            }).ToList()
                        })
                        .SingleOrDefaultAsync();
                })
                : await _context.BasicDocuments
                        .Where(x => x.Id == id)
                        .Select(x => new BasicDocumentDetailsModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            HasBarcode = x.HasBarcode,
                            HasFactoryNumber = x.HasFactoryNumber,
                            IsUniqueForStudent = x.IsUniqueForStudent,
                            HasSubjects = x.HasSubjects,
                            AttachedImagesCountMin = x.AttachedImagesCountMin,
                            AttachedImagesCountMax = x.AttachedImagesCountMax,
                            IsAppendix = x.IsAppendix,
                            IsDuplicate = x.IsDuplicate,
                            SeriesFormat = x.SeriesFormat,
                            MainBasicDocuments = x.MainBasicDocuments,
                            DetailedSchoolTypes = x.DetailedSchoolTypes,
                            IsIncludedInRegister = x.IsIncludedInRegister,
                            PageOrientation = x.PageOrientation != null ? (PageOrientationEnum)x.PageOrientation.Value : PageOrientationEnum.Unspecified,
                            DocumetPartsDetails = x.BasicDocumentParts
                            .Select(p => new BasicDocumentPartDetailsModel
                            {
                                Id = p.Id,
                                BasicDocumentId = p.BasicDocumentId,
                                SubjectTypesStr = p.SubjectTypesList,
                                ExternalEvaluationTypesStr = p.ExternalEvaluationTypesList
                            }).ToList()
                        })
                        .SingleOrDefaultAsync();

        }

        public async Task<List<DropdownViewModel>> GetDocumentEducationTypeOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.DocumentEducationTypeOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.DocumentEducationTypes
                        .AsNoTracking()
                        .Select(x => new DropdownViewModel
                        {
                            Value = x.Id,
                            Name = x.Name,
                            Text = x.Description,
                            IsValid = x.IsValid
                        })
                        .ToListAsync();
                })
                : await _context.DocumentEducationTypes
                        .AsNoTracking()
                        .Select(x => new DropdownViewModel
                        {
                            Value = x.Id,
                            Name = x.Name,
                            Text = x.Description,
                            IsValid = x.IsValid
                        })
                        .ToListAsync();

            return allEntries.FilterByValid(valid);
        }

        public async Task<List<DropdownViewModel>> GetFlOptions(ValidEnum valid = ValidEnum.True)
        {
            var allEntries = UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.FlOptions, entry =>
                  {
                      entry.SlidingExpiration = _slidingExpiration;
                      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval); ;
                      entry.Priority = _cachePriority;

                      return _context.Fls.AsNoTracking()
                          .Where(x => x.IsValid == true)
                          .Select(x => new DropdownViewModel
                          {
                              Value = x.Flid,
                              Name = x.Name,
                              Text = x.Name,
                              IsValid = x.IsValid ?? false
                          })
                          .ToListAsync();
                  })
                : await _context.Fls.AsNoTracking()
                    .Where(x => x.IsValid == true)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.Flid,
                        Name = x.Name,
                        Text = x.Name,
                        IsValid = x.IsValid ?? false
                    })
                    .ToListAsync();

            return allEntries.FilterByValid(valid).OrderBy(x => x.Name).ToList();
        }

        public async Task<List<DropdownViewModel>> GetInstitutionTypeOptions()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.InstTyppeOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.InstTypes.AsNoTracking()
                        .Where(x => x.InstTypeId != 5)
                        .Select(x => new DropdownViewModel
                        {
                            Value = x.InstTypeId,
                            Name = x.Name,
                            Text = x.Name,
                        })
                        .ToListAsync();
                })
                : await _context.InstTypes.AsNoTracking()
                    .Where(x => x.InstTypeId != 5)
                    .Select(x => new DropdownViewModel
                    {
                        Value = x.InstTypeId,
                        Name = x.Name,
                        Text = x.Name,
                    })
                    .ToListAsync();
        }

        public async Task<List<DropdownViewModel>> GetVetLevelOptions()
        {
            var options = new List<DropdownViewModel>
            {
                new DropdownViewModel { Text = VetLevelEnum.One.GetEnumDescription(), Value = (int)VetLevelEnum.One, Name = VetLevelEnum.One.GetEnumDescription() },
                new DropdownViewModel { Text = VetLevelEnum.Two.GetEnumDescription(), Value = (int)VetLevelEnum.Two,  Name = VetLevelEnum.Two.GetEnumDescription() },
                new DropdownViewModel { Text = VetLevelEnum.Three.GetEnumDescription(), Value = (int)VetLevelEnum.Three, Name = VetLevelEnum.Three.GetEnumDescription() },
                new DropdownViewModel { Text = VetLevelEnum.Four.GetEnumDescription(), Value = (int)VetLevelEnum.Four, Name = VetLevelEnum.Four.GetEnumDescription() }
            };
            return await Task.FromResult(options);
        }

        public async Task<IEnumerable<GradeDropdownViewModel>> GetGradeOptions(string searchStr, string selectedValue)
        {
            return await _cache.GetOrCreateAsync(CacheKeys.GradeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.GradeNoms
                .Where(x => x.IsValid == true)
                .Select(x => new GradeDropdownViewModel
                {
                    Value = x.Id,
                    Name = x.Name,
                    Text = x.Description,
                    GradeTypeId = x.GradeTypeId,
                    GradeTypeName = x.GradeType.Description
                }).ToListAsync();
            });
        }


        public async Task<IEnumerable<DropdownViewModel>> GetSpecialNeedGradeOptions(string searchStr, string selectedValue)
        {
            return await _cache.GetOrCreateAsync(CacheKeys.SpecialNeedGradeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.GradeNoms
                .Where(x => x.GradeTypeId == (int)GradeCategoryEnum.SpecialNeeds && x.IsValid == true)
                .Select(p => new DropdownViewModel
                {
                    Value = p.Id,
                    Name = p.Name,
                    Text = p.Name
                }).ToListAsync();
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetOtherGradeOptions(string searchStr, string selectedValue)
        {
            return await _cache.GetOrCreateAsync(CacheKeys.OtherGradeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.GradeNoms
                .Where(x => x.GradeTypeId == (int)GradeCategoryEnum.Other && x.IsValid == true)
                .Select(p => new DropdownViewModel
                {
                    Value = p.Id,
                    Name = p.Name,
                    Text = p.Name
                }).ToListAsync();
            });
        }

        public async Task<IEnumerable<DropdownViewModel>> GetQualitativeGradeOptions(string searchStr, string selectedValue)
        {
            return await _cache.GetOrCreateAsync(CacheKeys.QualitativeGradeOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.GradeNoms
                .Where(x => x.GradeTypeId == (int)GradeCategoryEnum.Qualitative && x.IsValid == true)
                .Select(p => new DropdownViewModel
                {
                    Value = p.Id,
                    Name = p.Name,
                    Text = p.Name
                }).ToListAsync();
            });
        }

        public async Task<List<DropdownViewModel>> GetCurriculumPartOptions(string searchStr, string selectedValue)
        {
            string searchQuery = searchStr.IsNullOrWhiteSpace() ? null : searchStr.Trim();

            List<DropdownViewModel> options = await _cache.GetOrCreateAsync(CacheKeys.CurriculumPartOptions, entry =>
            {
                entry.SlidingExpiration = _slidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_refreshInterval);
                entry.Priority = _cachePriority;

                return _context.CurriculumParts
                .Select(x => new DropdownViewModel
                {
                    Value = x.CurriculumPartId,
                    Name = x.Name,
                    Text = x.Description,
                    IsValid = x.IsValid ?? false
                }).ToListAsync();
            });

            if (selectedValue.IsNullOrWhiteSpace())
            {
                // Липсва избрана стойност
                return options
                    .Where(x => x.IsValid)
                    .Where(x => searchQuery == null || x.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || x.Text.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            string[] splitStr = selectedValue.Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> selectedIds = splitStr.ToHashSet<int>();
            if (selectedIds.Count > 0)
            {
                // Подали сме Id-та
                return options
                    .Where(x => selectedIds.Contains(x.Value) || (x.IsValid && (searchQuery == null || x.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || x.Text.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }
            else
            {
                return options
                    .Where(x => x.IsValid)
                    .Where(x => searchQuery == null || x.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || x.Text.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public async Task<IEnumerable<DropdownViewModel>> GetReasonsForReassessment()
        {
            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.ReasonForReassessmentTypeOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.ReasonForReassessmentTypes
                        .Select(r => new DropdownViewModel
                        {
                            Value = r.Id,
                            Name = r.Name,
                            Text = r.Name,
                        }).ToListAsync();
                })
                : await _context.ReasonForReassessmentTypes
                .Select(r => new DropdownViewModel
                {
                    Value = r.Id,
                    Name = r.Name,
                    Text = r.Name,
                }).ToListAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetStudentSessionOptionsAsync()
        {
            // Взимаме само записи с Id по голямо или равно на 3 от таблицата GradeCategory като записа "Годишна" се заменя с "Редовна"!
            int finalSessionId = (int)StudentSessionCategory.Final;
            string finalSessionStr = StudentSessionCategory.Final.GetEnumDescription();

            return UseCahce
                ? await _cache.GetOrCreateAsync(CacheKeys.SessionOptions, entry =>
                {
                    entry.SlidingExpiration = _slidingExpiration;
                    entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(_refreshInterval);
                    entry.Priority = _cachePriority;

                    return _context.GradeCategories
                        .Where(x => x.Id >= finalSessionId)
                        .Select(r => new DropdownViewModel
                        {
                            Value = r.Id,
                            Name = r.Id == finalSessionId ? finalSessionStr : r.Description,
                            Text = r.Id == finalSessionId ? finalSessionStr : r.Description,
                        }).ToListAsync();
                })
                : await _context.GradeCategories
                .Where(x => x.Id >= finalSessionId)
                .Select(r => new DropdownViewModel
                {
                    Value = r.Id,
                    Name = r.Id == finalSessionId ? finalSessionStr : r.Description,
                    Text = r.Id == finalSessionId ? finalSessionStr : r.Description,
                }).ToListAsync();
        }

        public void RemoveKey(string cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                _cache?.Remove(cacheKey);
            }
        }

        public void ClearCache()
        {
            RemoveKey(CacheKeys.PinTypesOptions);
            RemoveKey(CacheKeys.ExternalEvaluationTypeOptions);
            RemoveKey(CacheKeys.GendersOptions);
            RemoveKey(CacheKeys.LanguagesOptions);
            RemoveKey(CacheKeys.PreSchoolBasicClassOptions);
            RemoveKey(CacheKeys.EducationFormOptions);
            RemoveKey(CacheKeys.AdmissionReasonOptions);
            RemoveKey(CacheKeys.SubjectDetailsOptions);
            RemoveKey(CacheKeys.SchoolYearFromCurrentYearTable);
            RemoveKey(CacheKeys.GuardianTypeOptions);
            RemoveKey(CacheKeys.ORESTypeOptions);
            RemoveKey(CacheKeys.StudentPositionOptions);
            RemoveKey(CacheKeys.StudentParticipationOptions);
            RemoveKey(CacheKeys.SelfGovernmentPositionOptions);
            RemoveKey(CacheKeys.BasicDocumentsOptions);
            RemoveKey(CacheKeys.MinistryOptions);
            RemoveKey(CacheKeys.ClassTypeOptions);
            RemoveKey(CacheKeys.SPPOOProfessionOptions);
            RemoveKey(CacheKeys.SPPOOSpecialityOptions);
            RemoveKey(CacheKeys.DocumentEducationTypeOptions);
            RemoveKey(CacheKeys.ReasonForEqualizationTypeOptions);
            RemoveKey(CacheKeys.EkrOptions);
            RemoveKey(CacheKeys.NkrOptions);
            RemoveKey(CacheKeys.InstTyppeOptions);
            RemoveKey(CacheKeys.ItLevelOptions);
            RemoveKey(CacheKeys.FlLevelOptions);
            RemoveKey(CacheKeys.GradeOptions);
            RemoveKey(CacheKeys.SpecialNeedGradeOptions);
            RemoveKey(CacheKeys.OtherGradeOptions);
            RemoveKey(CacheKeys.CurriculumPartOptions);
            RemoveKey(CacheKeys.ReasonForReassessmentTypeOptions);
            RemoveKey(CacheKeys.SessionOptions);

            try
            {
                MemoryCache memoryCache = _cache as MemoryCache;
            }
            catch (Exception)
            {
                // Ignore
                throw;
            }
        }
    }
}