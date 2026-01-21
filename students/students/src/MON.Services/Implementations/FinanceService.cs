namespace MON.Services.Implementations
{
    using DocumentFormat.OpenXml.Spreadsheet;
    using MetadataExtractor.Formats.Tiff;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Update;
    using Microsoft.Extensions.Options;
    using MON.Models.Configuration;
    using MON.Models.Finance;
    using MON.Models.Grid;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using Neispuo.Tools.Services.Implementations;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FinanceService : BaseService<FinanceService>, IFinanceService
    {
        public FinanceService(DbServiceDependencies<FinanceService> dependencies)
            : base(dependencies)
        {
        }

        private async Task CheckPermissions(string permission)
        {
            if (!await _authorizationService.AuthorizeUser(permission))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        public async Task<List<Dictionary<string, object>>> GetNaturalIndicators(short schoolYear, int period)
        {
            if (_userInfo.IsSchoolDirector)
            {
                var reader = new DictionaryViewReader(_context);

                var data = await reader.ReadViewAsync("[finance].[NaturalIndicators]", JsonConvert.SerializeObject(new { institutionId = _userInfo.InstitutionID, schoolYear = schoolYear, period = period }));
                return data;
            }
            else
            {
                return new List<Dictionary<string, object>>();
            }
        }

        private (short schoolYear, int period) ParseYearPeriod(string combined)
        {
            if (string.IsNullOrEmpty(combined) || combined.Length < 5)
                throw new ArgumentException("Invalid year-period format. Expected format: YYYYP where YYYY is year and P is period", nameof(combined));

            if (!short.TryParse(combined.Substring(0, 4), out short year))
                throw new ArgumentException("Invalid year format", nameof(combined));

            if (!int.TryParse(combined.Substring(4), out int period))
                throw new ArgumentException("Invalid period format", nameof(combined));

            return (year, period);
        }


        public async Task<List<Dictionary<string, object>>> GetNaturalIndicators(List<string> periods, bool showItemPrice, bool showItemValue)
        {
            var result = new List<Dictionary<string, object>>();
            var reader = new DictionaryViewReader(_context);
            if (periods != null)
            {
                foreach (var period in periods)
                {
                    var (year, per) = ParseYearPeriod(period);
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                    if (_userInfo.IsSchoolDirector)
                    {
                        data = await reader.ReadViewAsync("[finance].[NaturalIndicators]", JsonConvert.SerializeObject(new { institutionId = _userInfo.InstitutionID, schoolYear = year, period = per }));
                    }
                    else if (_userInfo.IsMon || _userInfo.IsMonExpert)
                    {
                        data = await reader.ReadViewAsync("[finance].[NaturalIndicators]", JsonConvert.SerializeObject(new { schoolYear = year, period = per }));
                    }
                    else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
                    {
                        data = await reader.ReadViewAsync("[finance].[NaturalIndicators]", JsonConvert.SerializeObject(new { RegionId = _userInfo.RegionID, schoolYear = year, period = per }));
                    }
                    result.AddRange(data);

                    // Цени за съответната година
                    var prices = await _context.NaturalIndicatorsPrices.Where(p => p.SchoolYear == year).ToListAsync();
                    var pricesDictionary = new Dictionary<string, object>();
                    foreach (var item in data.First())
                    {
                        var price = prices.FirstOrDefault(p => p.ColumnName == item.Key);
                        if (price != null)
                        {
                            pricesDictionary.Add(item.Key, price.Price);
                        }
                    }
                    if (showItemPrice)
                    {
                        pricesDictionary.Add("SchoolYear", year);
                        pricesDictionary.Add("Period", per);
                        result.Add(pricesDictionary);
                    }

                    foreach (var institution in data)
                    {
                        // Общи стойности
                        var valuesDictionary = new Dictionary<string, object>();
                        foreach (var key in institution.Keys)
                        {
                            if (pricesDictionary.ContainsKey(key) &&
                                decimal.TryParse(institution[key]?.ToString(), out decimal quantity) &&
                                decimal.TryParse(pricesDictionary[key]?.ToString(), out decimal price))
                            {
                                valuesDictionary.Add(key, quantity * price);
                            }
                            else
                            {
                                valuesDictionary.Add(key, null);
                            }
                        }
                        if (showItemValue)
                        {
                            valuesDictionary["SchoolYear"] = year;
                            valuesDictionary["Period"] = per;
                            result.Add(valuesDictionary);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<Dictionary<string, object>>> GetResourceSupportData(List<string> periods)
        {
            var result = new List<Dictionary<string, object>>();
            var reader = new DictionaryViewReader(_context);
            if (periods != null)
            {
                foreach (var period in periods)
                {
                    var (year, per) = ParseYearPeriod(period);
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                    if (_userInfo.IsSchoolDirector)
                    {
                        data = await reader.ReadViewAsync("[finance].[ResourceSupportData]", JsonConvert.SerializeObject(new { institutionId = _userInfo.InstitutionID, schoolYear = year, period = per }));
                    }
                    else if (_userInfo.IsMon || _userInfo.IsMonExpert)
                    {
                        data = await reader.ReadViewAsync("[finance].[ResourceSupportData]", JsonConvert.SerializeObject(new { schoolYear = year, period = per }));
                    }
                    else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
                    {
                        data = await reader.ReadViewAsync("[finance].[ResourceSupportData]", JsonConvert.SerializeObject(new { RegionId = _userInfo.RegionID, schoolYear = year, period = per }));
                    }
                    result.AddRange(data);
                }
            }
            return result;
        }

        public async Task<IPagedList<Dictionary<string, object>>> ListPeriod(NaturalIndicatorsDataListInput input)
        {
            var items = await GetNaturalIndicators(input.Year ?? 0, input.Period ?? 0);
            return items.ToPagedList(items.Count);
        }

        public async Task<IPagedList<Dictionary<string, object>>> ListPeriods(NaturalIndicatorsDataListInput input)
        {
            var items = await GetNaturalIndicators(input.Periods, input.ShowItemPrice, input.ShowItemValue);
            return items.ToPagedList(items.Count);
        }

        public async Task<IPagedList<Dictionary<string, object>>> ListResourceSupportDataPeriods(ResourceSupportDataDataListInput input)
        {
            var items = await GetResourceSupportData(input.Periods);
            return items.ToPagedList(items.Count);
        }

        private async Task<short> GetCurrentSchoolYear()
        {
            short currentSchoolYear = await _context.CurrentYears
                .Where(x => x.IsValid == true)
                .Select(x => x.CurrentYearId).FirstOrDefaultAsync();
            return currentSchoolYear;
        }

        private static List<int> ParsePipeDetailedSchoolTypes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new List<int>();

            return input.Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out int value) ? value : 0)
                        .ToList();
        }

        public async Task<List<GridHeaderModel>> GetGridHeaders()
        {
            int? detailedSchoolType = null;

            if (_userInfo.InstitutionID != null)
            {
                short currentSchoolYear = await GetCurrentSchoolYear();
                var institution = await _context.InstitutionSchoolYears.FirstOrDefaultAsync(x => x.InstitutionId == _userInfo.InstitutionID && x.SchoolYear == currentSchoolYear);
                detailedSchoolType = institution?.DetailedSchoolTypeId;
            }

            var allHeaders = await (
                from c in _context.NaturalIndicatorsColumns
                select new
                {
                    c.ColumnName,
                    c.Description,
                    DetailedSchoolTypes = ParsePipeDetailedSchoolTypes(c.DetailedSchoolTypes)
                }).ToListAsync();

            var headers = allHeaders
                .Where(c => detailedSchoolType == null || c.DetailedSchoolTypes.Count == 0 || c.DetailedSchoolTypes.Contains(detailedSchoolType.Value))
                    .Select(c => new GridHeaderModel
                    {
                        value = c.ColumnName,
                        text = c.Description
                    });

            return headers.ToList();
        }

        public async Task<List<GridHeaderModel>> GetResourceSupportDataGridHeaders()
        {
            var headers = await _context.ResourceSupportDataColumns
                .Select(c => new GridHeaderModel
                {
                    value = c.ColumnName,
                    text = c.Description
                })
                .ToListAsync();
            return headers;
        }
    }
}
