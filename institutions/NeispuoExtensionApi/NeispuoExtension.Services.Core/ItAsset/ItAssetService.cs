namespace NeispuoExtension.Services.Core.ItAsset
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using CsvHelper;
    using CsvHelper.Configuration;
    using Microsoft.AspNetCore.Http;

    using Core.UserIdentity;
    using Database.Services.NeispuoDatabase;

    using Models.ItAsset.Request;
    using Models.ItAsset.Response;
    using Models.ItAsset.Database;

    public class ItAssetService : IItAssetService
    {
        private readonly IUserIdentityService userIdentityService;
        private readonly INeispuoDatabaseService neispuoDatabaseService;
        private Dictionary<string, IEnumerable<int>> allowedLookups = new Dictionary<string, IEnumerable<int>>();

        public ItAssetService(
            IUserIdentityService userIdentityService,
            INeispuoDatabaseService neispuoDatabaseService)
        {
            this.userIdentityService = userIdentityService;
            this.neispuoDatabaseService = neispuoDatabaseService;
        }

        public async Task<InsertItAssetFromExcelResponseModel> InsertFromExcelAsync(InsertItAssetFromExcelRequestModel request)
        {
            InsertItAssetFromExcelResponseModel response = new InsertItAssetFromExcelResponseModel();

            if (request.File == null || (request.File.Length == 0))
            {
                response.Errors.Add("Файлът е празен!");
                return response;
            }

            List<List<string>> fileRecords = await this.ReadCsvAsync(request.File);

            if (fileRecords.Count < 2)
            {
                response.Errors.Add("Файлът няма редове с данни!");
                return response;
            }

            response.Success = true;

            List<string> headers = fileRecords[0];
            List<List<string>> fileLines = fileRecords.Skip(1).ToList();

            IEnumerable<ITAssetColumnConfigDatabaseModel> configs = await this.GetITAssetColumnConfigAsync();

            int rowIndex = 1;
            var dataRows = new List<Dictionary<string, object>>();

            foreach (var fileLine in fileLines)
            {
                rowIndex++;
                var rowObject = new Dictionary<string, object>();

                foreach (var config in configs)
                {
                    string valueString = fileLine[config.ColumnIndex];

                    if (string.IsNullOrWhiteSpace(valueString) && config.IsRequired)
                    {
                        response.Success = false;
                        response.Errors.Add($"Ред {rowIndex}, Колона '{headers[config.ColumnIndex]}': полето е задължително!");
                        continue;
                    }

                    var result = this.TryParseValue(valueString, config);

                    if (!result.Success)
                    {
                        response.Success = false;
                        response.Errors.Add($"Ред {rowIndex}, Колона '{headers[config.ColumnIndex]}': невалиден формат на данните за „{valueString}“!");
                        continue;
                    }

                    if (!String.IsNullOrEmpty(config.LookupSource))
                    {
                        object parentId = null;

                        if (config.DependsOnColumnIndex.HasValue)
                        {
                            ITAssetColumnConfigDatabaseModel dependsConfig = configs
                                .Where(x => x.ColumnIndex == config.DependsOnColumnIndex.Value)
                                .FirstOrDefault();

                            if (Object.Equals(dependsConfig, default(ITAssetColumnConfigDatabaseModel)))
                            {
                                throw new InvalidOperationException("Невалидна конфигурация!");
                            }

                            string dependsValueString = fileLine[config.DependsOnColumnIndex.Value];

                            var dependsResult = this.TryParseValue(dependsValueString, dependsConfig);

                            if (!dependsResult.Success)
                            {
                                parentId = dependsResult.Value;
                            }
                        }

                        IEnumerable<int> allowedLookupIds = await this.GetITAssetAllowedLookupIdsAsync(config.LookupSource, parentId);

                        if (!allowedLookupIds.Any(x => x == (int)result.Value))
                        {
                            string error = String.Empty;

                            if (parentId == null)
                            {
                                error = $"Ред {rowIndex}, Колона '{headers[config.ColumnIndex]}': стойността „{valueString}“ не е сред позволените!";
                            }
                            else
                            {
                                error = $"Ред {rowIndex}, Колона '{headers[config.ColumnIndex]}': стойността „{valueString}“ не е валидна за избраната стойност в колона '{headers[config.DependsOnColumnIndex.Value]}'!";
                            }

                            response.Success = false;
                            response.Errors.Add(error);
                            continue;
                        }
                    }

                    rowObject[config.FieldName] = result.Value;
                }

                if (rowObject.Any())
                {
                    dataRows.Add(rowObject);
                }
            }

            if (response.Success)
            {
                var sysUserId = this.userIdentityService.SysUserId;
                int institutionId = this.userIdentityService.InstitutionId;
                string json = System.Text.Json.JsonSerializer.Serialize(dataRows);

                int result = await this.InsertITAssetsAsync(institutionId, request.InstitutionDepartmentId, sysUserId, json);

                if (result > 0)
                {
                    return response;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add($"Възникна грешка!");
                }
            }

            return response;
        }

        private (object Value, bool Success) TryParseValue(string valueString, ITAssetColumnConfigDatabaseModel config)
            => config.DataTypeId switch
            {
                1 => this.TryParseString(valueString, config.IsRequired),
                2 => this.TryParseInteger(valueString, config.IsRequired),
                3 => this.TryParseDecimal(valueString, config.IsRequired),
                4 => this.TryParseDate(valueString, config.IsRequired),
                5 => this.TryParseBoolean(valueString, config.IsRequired),
                _ => throw new InvalidOperationException("Невалиден тип данни!")
            };

        private (object Value, bool Success) TryParseString(string valueString, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(valueString))
            {
                if (isRequired)
                {
                    return (null, false);
                }

                return (null, true);
            }

            return (valueString.Trim(), true);
        }

        private (object Value, bool Success) TryParseInteger(string valueString, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(valueString))
            {
                if (isRequired)
                {
                    return (null, false);
                }

                return (null, true);
            }

            if (int.TryParse(valueString, out int result))
            {
                return (result, true);
            }

            return (null, false);
        }

        private (object Value, bool Success) TryParseDecimal(string valueString, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(valueString))
            {
                if (isRequired)
                {
                    return (null, false);
                }

                return (null, true);
            }

            if (decimal.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return (result, true);
            }

            return (null, false);
        }

        private (object Value, bool Success) TryParseDate(string valueString, bool isRequired, string format = "dd.MM.yyyy")
        {
            if (string.IsNullOrWhiteSpace(valueString))
            {
                if (isRequired)
                {
                    return (null, false);
                }

                return (null, true);
            }

            if (DateTime.TryParseExact(valueString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return (result, true);
            }

            return (null, false);
        }

        private (object Value, bool Success) TryParseBoolean(string valueString, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(valueString))
            {
                if (isRequired)
                {
                    return (null, false);
                }

                return (null, true);
            }

            valueString = valueString.ToUpper();

            if (valueString != "ДА" && valueString != "НЕ")
            {
                return (null, false);
            }

            bool result = valueString == "ДА" ? true : false;

            return (result, true);
        }

        public async Task<List<List<string>>> ReadCsvAsync(IFormFile file, string delimiter = ",")
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            var encoding = DetectUtf8OrCp1251(memoryStream);

            memoryStream.Position = 0;

            CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = delimiter,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            List<List<string>> rows = new List<List<string>>();

            using var reader = new StreamReader(memoryStream, encoding);

            using var csv = new CsvReader(reader, configuration);

            while (await csv.ReadAsync())
            {
                var row = new List<string>();

                for (int i = 0; csv.TryGetField<string>(i, out var value); i++)
                {
                    row.Add(value ?? string.Empty);
                }

                rows.Add(row);
            }

            return rows;
        }

        private static Encoding DetectUtf8OrCp1251(Stream stream)
        {
            stream.Position = 0;

            using (var streamReader = new StreamReader(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false), detectEncodingFromByteOrderMarks: true, leaveOpen: true))
            {
                var sample = streamReader.ReadToEnd();

                if (sample.IndexOf('\uFFFD') >= 0)
                {
                    return Encoding.GetEncoding(1251);
                }

                return streamReader.CurrentEncoding;
            }
        }

        private async Task<IEnumerable<ITAssetColumnConfigDatabaseModel>> GetITAssetColumnConfigAsync()
            => await this.neispuoDatabaseService.ExecuteListAsync<ITAssetColumnConfigDatabaseModel>("extension.ITAssetColumnConfigGet");

        private async Task<IEnumerable<int>> GetITAssetAllowedLookupIdsAsync(string lookupSource, object parentId)
        {
            string lookupKey = parentId == null ? lookupSource : String.Concat(lookupSource, "_", parentId);

            if (!this.allowedLookups.ContainsKey(lookupKey))
            {
                IEnumerable<int> allowedLookupIds = await this.neispuoDatabaseService.ExecuteListAsync<int>(
                    "extension.ITAssetAllowedLookupIdsGet",
                    new
                    {
                        @LookupSource = lookupSource,
                        @ParentId = parentId
                    });

                this.allowedLookups.Add(lookupKey, allowedLookupIds);
            }

            return this.allowedLookups[lookupKey];
        }

        private async Task<int> InsertITAssetsAsync(int institutionId, int institutionDepartmentId, int sysUserId, string json)
            => await this.neispuoDatabaseService.ExecuteFirstAsync<int>(
                "extension.ITAssetsInsert",
                new
                {
                    @InstitutionId = institutionId,
                    @InstitutionDepartmentId = institutionDepartmentId,
                    @SysUserId = sysUserId,
                    @ITAssetsJson = json
                });
    }
}
