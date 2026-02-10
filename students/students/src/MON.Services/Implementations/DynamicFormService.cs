using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MON.DataAccess.Dto;
using MON.Models;
using MON.Models.Dynamic;
using MON.Models.Enums;
using MON.Models.Grid;
using MON.Services.Interfaces;
using MON.Shared;
using MON.Shared.Extensions.Dynamic;
using MON.Shared.Interfaces;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{

    public class DynamicFormService : BaseService<DynamicFormService>, IDynamicFormService
    {
        private readonly INeispuoAuthorizationService _permissionService;
        private Dictionary<string, DynamicEntity> _schemasByEntityType;
        private readonly IAppConfigurationService _configurationService;

        public DynamicFormService(DbServiceDependencies<DynamicFormService> dependencies,
            INeispuoAuthorizationService permissionService,
            IAppConfigurationService configurationService)
            : base(dependencies)
        {
            _permissionService = permissionService;
            _configurationService = configurationService;
        }

        public string GetJsonSchema()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema schema = generator.Generate(typeof(DynamicEntities));

            return JsonSerializer.Serialize(schema);
        }

        public async Task<DynamicEntities> GetEntitiesJsonDescription()
        {
            string contents = await _configurationService.GetValueByKey("Nomenclatures");

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            DynamicEntities model = string.IsNullOrWhiteSpace(contents)
                ? null
                : JsonSerializer.Deserialize<DynamicEntities>(contents, options);

            MergeSecurity(model);

            return model;
        }

        public async Task<List<DropdownViewModel>> GetEntityTypesDropdowns()
        {
            DynamicEntities entities = await GetEntitiesJsonDescription();

            return entities == null || entities.Entities == null
                ? new List<DropdownViewModel>()
                : entities.Entities.Select(x => new DropdownViewModel
                {
                    Name = x.Name,
                    Text = x.Title,
                    Code = x.Name
                }).ToList();
        }

        public async Task<List<DynamicGridHeader>> GetGridHeaders(string entityTypeName)
        {
            DynamicEntities schema = await GetEntitiesJsonDescription();
            if (schema == null || schema.Entities == null) return null;


            DynamicEntity dynamicEntity = schema.Entities.FirstOrDefault(x => x.Name == entityTypeName);
            if (dynamicEntity == null || dynamicEntity.Sections == null || !dynamicEntity.Sections.Any()) return null;

            try
            {
                await CheckSecurity(dynamicEntity, OperationEnum.Read);
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }

            List<DbTableInfoDto> tableInfo = await GetDbTableInfo(dynamicEntity.DbTableName, dynamicEntity.DbSchemaName);
            List<DynamicGridHeader> headers = dynamicEntity.Sections
                .Where(x => x.Visible)
                .SelectMany(x => x.Items)
                .Where(x => x.Visible)
                .Select(x => new DynamicGridHeader
                {
                    Text = x.Label,
                    Value = x.ColumnName.ToCamelCase(),
                    Sortable = true,
                    IsPrimaryKey = tableInfo.FirstOrDefault(t => t.ColumnName.Equals(x.ColumnName, StringComparison.OrdinalIgnoreCase))?.IsPrimaryKey ?? false,
                })
                .ToList();

            return headers;
        }

        public async Task<IPagedList<string>> List(DynamicEntitiesListInput input)
        {
            DynamicEntity entitySchema = await GetEntitySchemaByName(input?.EnityName);

            await CheckSecurity(entitySchema, OperationEnum.Read);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            string sqlStr = await GenerateListSqlSring(entitySchema, input, sqlParameters);

            List<JsonStrResult> results = await _context.JsonStrResults
                .FromSqlRaw(sqlStr, sqlParameters.ToArray())
                .ToListAsync();

            return new PagedList<string>
            {
                TotalCount = results.FirstOrDefault()?.TotalCount ?? 0,
                ItemsAsJsonStr = results.FirstOrDefault()?.JsonStr ?? "[]"
            };
        }

        public async Task<string> GetEntityModel(string entityTypeName, string entityId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sqlStr = await GenerateFindSqlString(entityTypeName, entityId, parameters);

            List<JsonStrResult> results = await _context.JsonStrResults
                .FromSqlRaw(sqlStr, parameters.ToArray())
                .ToListAsync();

            var str = results.FirstOrDefault()?.JsonStr;
            return str;
        }

        public async Task Create(dynamic model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            DynamicEntity entitySchema = await GetEntitySchemaByName(DynamicUtils.GetPropertyValue(model, "entityTypeName"));

            await CheckSecurity(entitySchema, OperationEnum.Create);

            List<DbTableInfoDto> dbTableInfo = await GetDbTableInfo(entitySchema?.DbTableName, entitySchema.DbSchemaName);
            IEnumerable<DynamicEntityItem> editableSchemaItems = entitySchema.Sections.SelectMany(x => x.Items).Where(x => x.Editable);

            ValidateModel(dbTableInfo, editableSchemaItems, model);

            DbTableInfoDto[] dbColsToInsertInto = dbTableInfo.Where(c => !c.IsPrimaryKey && editableSchemaItems.Any(i => i.ColumnName.Equals(c.ColumnName, StringComparison.OrdinalIgnoreCase))).ToArray();

            string insertStatement = $"INSERT INTO {dbTableInfo.First().TableSchema}.{dbTableInfo.First().TableName}" +
                $"({string.Join(", ", dbColsToInsertInto.Select(x => $"[{x.ColumnName}]"))}) " +
                $" VALUES({string.Join(", ", dbColsToInsertInto.Select(c => GetSqlParameterName(c.ColumnName)))});";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            foreach (DbTableInfoDto dbCol in dbColsToInsertInto)
            {
                DynamicEntityItem editableItem = editableSchemaItems.FirstOrDefault(x => x.ColumnName.Equals(dbCol.ColumnName, StringComparison.OrdinalIgnoreCase));

                // Db колоната не е описана в json схемата
                if (editableItem == null) throw new ArgumentNullException(nameof(editableItem), nameof(DynamicEntityItem));

                if (!dbCol.IsNulable && !DynamicUtils.PropertyExists(model, editableItem.Id))
                {
                    // Not nullable колона в базата, но липсващо за нея свойтсво в модела.
                    throw new InvalidOperationException("Missing not nullable db column in the input model");
                }

                sqlParameters.Add(GetSqlParameter(dbCol, editableItem.Id, model));
            }

            await _context.Database.ExecuteSqlRawAsync(insertStatement, sqlParameters.ToArray());
        }

        public async Task Update(dynamic model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            DynamicEntity entitySchema = await GetEntitySchemaByName(DynamicUtils.GetPropertyValue(model, "entityTypeName"));

            await CheckSecurity(entitySchema, OperationEnum.Update);

            List<DbTableInfoDto> dbTableInfo = await GetDbTableInfo(entitySchema?.DbTableName, entitySchema.DbSchemaName);
            IEnumerable<DynamicEntityItem> editableSchemaItems = entitySchema.Sections.SelectMany(x => x.Items).Where(x => x.Editable);

            ValidateModel(dbTableInfo, editableSchemaItems, model);

            DbTableInfoDto[] dbColsToUpdate = dbTableInfo.Where(c => !c.IsPrimaryKey && editableSchemaItems.Any(i => i.ColumnName.Equals(c.ColumnName, StringComparison.OrdinalIgnoreCase))).ToArray();

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<string> updateSetStrings = new List<string>();
            foreach (DbTableInfoDto dbCol in dbColsToUpdate)
            {
                DynamicEntityItem editableItem = editableSchemaItems.FirstOrDefault(x => x.ColumnName.Equals(dbCol.ColumnName, StringComparison.OrdinalIgnoreCase));

                // Db колоната не е описана в json схемата
                if (editableItem == null) throw new ArgumentNullException(nameof(editableItem), nameof(DynamicEntityItem));

                if (!dbCol.IsNulable && !DynamicUtils.PropertyExists(model, editableItem.Id))
                {
                    // Not nullable колона в базата, но липсващо за нея свойтсво в модела.
                    throw new InvalidOperationException("Missing not nullable db column in the update model");
                }

                updateSetStrings.Add($"[{dbCol.ColumnName}] = {GetSqlParameterName(dbCol.ColumnName)}");
                sqlParameters.Add(GetSqlParameter(dbCol, editableItem.Id, model));
            }

            DbTableInfoDto pkColumn = dbTableInfo?.FirstOrDefault(x => x.IsPrimaryKey) ?? throw new ArgumentException(nameof(pkColumn));
            string pkColumnFieldId = entitySchema.Sections.SelectMany(x => x.Items).FirstOrDefault(x => x.ColumnName.Equals(pkColumn.ColumnName))?.Id;
            string pkValue = DynamicUtils.GetPropertyValue(model, pkColumnFieldId);

            string whereStatement = $" WHERE {pkColumn.ColumnName} = @pkValue";
            sqlParameters.Add(GetSqlParameter(pkColumn.DataType, false, "@pkValue", pkValue));

            string updateStatement = $" UPDATE {dbTableInfo.First().TableSchema}.{dbTableInfo.First().TableName} SET {string.Join(", ", updateSetStrings)} {whereStatement}";

            await _context.Database.ExecuteSqlRawAsync(updateStatement, sqlParameters.ToArray());
        }

        public async Task Delete(dynamic model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            DynamicEntity entitySchema = await GetEntitySchemaByName(model.entityTypeName.Value);

            await CheckSecurity(entitySchema, OperationEnum.Delete);

            List<DbTableInfoDto> dbTableInfo = await GetDbTableInfo(entitySchema?.DbTableName, entitySchema.DbSchemaName);
            DbTableInfoDto pkColumn = dbTableInfo?.FirstOrDefault(x => x.IsPrimaryKey) ?? throw new ArgumentException(nameof(pkColumn));
            string pkColumnFieldId = entitySchema.Sections.SelectMany(x => x.Items).FirstOrDefault(x => x.ColumnName.Equals(pkColumn.ColumnName))?.Id;
            string pkValue = DynamicUtils.GetPropertyValue(model, pkColumnFieldId);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            string whereStatement = $" WHERE {pkColumn.ColumnName} = @pkValue";
            sqlParameters.Add(GetSqlParameter(pkColumn.DataType, false, "@pkValue", pkValue));

            string deleteStatement = $" DELETE FROM {dbTableInfo.First().TableSchema}.{dbTableInfo.First().TableName} {whereStatement} ";

            await _context.Database.ExecuteSqlRawAsync(deleteStatement, sqlParameters.ToArray());
        }

        private async Task CheckSecurity(DynamicEntity entitySchema, OperationEnum operation)
        {
            if (entitySchema == null) throw new ArgumentNullException(nameof(entitySchema), nameof(DynamicEntity));

            HashSet<string> requiredCreatePermission = null;
            switch (operation)
            {
                case OperationEnum.Create:
                    if (!entitySchema.AllowCreate) throw new UnauthorizedAccessException($"{OperationEnum.Create} is now allowed!");

                    requiredCreatePermission = entitySchema.Security?.RequiredRermissions?.Create;

                    break;
                case OperationEnum.Read:
                    requiredCreatePermission = entitySchema.Security?.RequiredRermissions?.Read;

                    break;
                case OperationEnum.Update:
                    if (!entitySchema.AllowUpdate) throw new UnauthorizedAccessException($"{OperationEnum.Update} is now allowed!");

                    requiredCreatePermission = entitySchema.Security?.RequiredRermissions?.Update;

                    break;
                case OperationEnum.Delete:
                    if (!entitySchema.AllowDelete) throw new UnauthorizedAccessException($"{OperationEnum.Delete} is now allowed!");

                    requiredCreatePermission = entitySchema.Security?.RequiredRermissions?.Delete;

                    break;
                default:
                    break;
            }

            if (requiredCreatePermission != null && requiredCreatePermission.Count > 0)
            {
                // Необходими за определени права
                bool isPermissionsGranted = await _permissionService.AuthorizeUser(requiredCreatePermission.ToArray());
                if (!isPermissionsGranted)
                {
                    throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
                }
            }
        }

        /// <summary>
        /// Проверява за not nullable колони в базата, за които в модела липсва свойство.
        /// </summary>
        /// <param name="tableInfo">Метаданни за колоните в базата данни.</param>
        /// <param name="editableItems">Елементи от json схемата на дадено entity, маркирани като editable.</param>
        /// <param name="model">Модел за създаване на запис.</param>
        private void ValidateModel(List<DbTableInfoDto> tableInfo, IEnumerable<DynamicEntityItem> editableItems, dynamic model)
        {
            List<DbTableInfoDto> missingNotNullableDbColumnInTheModel = new List<DbTableInfoDto>();
            foreach (DbTableInfoDto dbCol in tableInfo.Where(x => !x.IsPrimaryKey && !x.IsNulable))
            {
                DynamicEntityItem editableItem = editableItems.FirstOrDefault(x => x.ColumnName.Equals(dbCol.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (!DynamicUtils.PropertyExists(model, editableItem?.ColumnName))
                {
                    missingNotNullableDbColumnInTheModel.Add(dbCol);
                }
            }

            if (missingNotNullableDbColumnInTheModel.Any())
            {
                // Not nullable колони в базата, но липсващи за тях свойтсва в модела.
                throw new InvalidOperationException($"Missing not nullable column in the model: {string.Join(",", missingNotNullableDbColumnInTheModel.Select(x => x.ColumnName))}");
            }
        }

        /// <summary>
        /// Връща <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="dbCol">Мета информация за колона от таблица от базата данни.</param>
        /// <param name="propertyName">Име на свойство.</param>
        /// <param name="model">Модел с входните данни.</param>
        /// <returns></returns>
        private SqlParameter GetSqlParameter(DbTableInfoDto dbCol, string propertyName, dynamic model)
        {
            string paramName = GetSqlParameterName(dbCol?.ColumnName);
            string paramValue = DynamicUtils.GetPropertyValue(model, propertyName);

            return GetSqlParameter(dbCol.DataType, dbCol.IsNulable, paramName, paramValue);
        }

        /// <summary>
        /// Връща <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="dbColumnDataType"></param>
        /// <param name="dbColumnIsNullable"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private SqlParameter GetSqlParameter(string dbColumnDataType, bool dbColumnIsNullable, string paramName, string paramValue)
        {
            if (dbColumnIsNullable && paramName.IsNullOrWhiteSpace()) paramValue = null;

            SqlParameter sqlParameter;
            switch (dbColumnDataType)
            {
                case "bit":
                    sqlParameter = new SqlParameter(paramName, SqlDbType.Bit) { IsNullable = dbColumnIsNullable };
                    bool? boolVal = paramValue.ToNullableBool();
                    sqlParameter.Value = boolVal ?? (object)DBNull.Value;
                    break;
                case "int":
                    sqlParameter = new SqlParameter(paramName, SqlDbType.Int) { IsNullable = dbColumnIsNullable };
                    int? intVal = paramValue.ToNullableInt();
                    sqlParameter.Value = intVal ?? (object)DBNull.Value;
                    break;
                case "datetime2":
                    sqlParameter = new SqlParameter(paramName, SqlDbType.DateTime2) { IsNullable = dbColumnIsNullable };
                    DateTime? datetimeVal = paramValue.ToNullableDateTime();
                    sqlParameter.Value = datetimeVal ?? (object)DBNull.Value;
                    break;
                case "bigint":
                    sqlParameter = new SqlParameter(paramName, SqlDbType.BigInt) { IsNullable = dbColumnIsNullable };
                    long? longVal = paramValue.ToNullableLong();
                    sqlParameter.Value = longVal ?? (object)DBNull.Value;
                    break;
                case "nvarchar":
                    sqlParameter = new SqlParameter(paramName, SqlDbType.NVarChar) { IsNullable = dbColumnIsNullable };
                    sqlParameter.Value = !paramValue.IsNullOrWhiteSpace() ? paramValue : (object)DBNull.Value;
                    break;
                case "varchar":
                    sqlParameter = new SqlParameter(paramName, SqlDbType.VarChar) { IsNullable = dbColumnIsNullable };
                    sqlParameter.Value = !paramValue.IsNullOrWhiteSpace() ? paramValue : (object)DBNull.Value;
                    break;
                default:
                    sqlParameter = new SqlParameter(paramName, paramValue) { IsNullable = dbColumnIsNullable };
                    break;
            }

            return sqlParameter;
        }

        private string GetSqlParameterName(string columnName)
        {
            if (columnName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(columnName), nameof(DbTableInfoDto));
            return $"@{columnName}";
        }

        /// <summary>
        /// Проверява дали дадена колона съществува в дадена таблица на базата данни.
        /// </summary>
        /// <param name="tableName">Информация за таблицата в базата данние. Ако не е подадена ще се зареди.</param>
        /// <param name="tableName">Име на таблица</param>
        /// <param name="columnName">Име на колона</param>
        /// <returns></returns>
        private async Task<bool> ColumnExists(List<DbTableInfoDto> tableInfo, string tableName, string schemaNama, string columnName)
        {
            if (tableName.IsNullOrWhiteSpace() || columnName.IsNullOrWhiteSpace()) return false;

            tableName = tableName.Trim();
            columnName = columnName.Trim();
            // Зарежда информация за таблицата в базата ако такава не е подадена
            tableInfo ??= await GetDbTableInfo(tableName, schemaNama);

            return tableInfo != null && tableInfo.Any(x => x.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Връща мета информация за колоните на дадена таблица от базата данни(student.fn_TableInfo()).
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        private async Task<List<DbTableInfoDto>> GetDbTableInfo(string entityName, string schemaName = null)
        {
            FormattableString queryString = $"select * from student.fn_TableInfo({entityName})";
            IQueryable<DbTableInfoDto> query = _context.Set<DbTableInfoDto>()
                .FromSqlInterpolated(queryString);

            if (!schemaName.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.TableSchema == schemaName);
            }

            return (await query.FromCacheAsync(new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(60) })).ToList();
        }

        /// <summary>
        /// Генерира raw sql string за листване на записи от дадена таблица по описана json схема.
        /// </summary>
        /// <param name="input">Модел с параметри подавани от грида на UI клиента(филтриране, сортиране, странициране)</param>
        /// <param name="alias">Db alias. "d" if not supplied.</param>
        /// <returns></returns>
        private async Task<string> GenerateListSqlSring(DynamicEntity entitySchema, DynamicEntitiesListInput input, List<SqlParameter> parameters, string alias = "d")
        {
            string fromStatement = GetFromStatement(entitySchema, alias);
            string selectStatement = GetSelectStatement(entitySchema, alias, true);
            // Todo: да се добави WHERE. Да се внимава за Sql injection.
            string filterStatement = await GetFilterStatement(entitySchema, input.Filter, parameters, alias);
            string orderStatement = await GetOrderStatement(entitySchema, input.SortBy, alias);
            string offsetStatement = GetOffsetStatement(input.PageIndex, input.PageSize);

            string mainSqlStr = $"{selectStatement} {fromStatement} {filterStatement} {orderStatement} {offsetStatement} FOR JSON AUTO";

            // Трябва ни за тотал-а на грида, с който си смята странницирането.
            string countSqlStr = $"SELECT COUNT(1) {fromStatement}";

            string sqlStr = $"SELECT TOP 1 ({mainSqlStr}) AS JsonStr, ({countSqlStr}) AS TotalCount;";

            return sqlStr;
        }

        /// <summary>
        /// Генерира WHERE клаузата за извличане на данните за запис по неговата PK стойност.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="parameters"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private async Task<string> GenerateFindSqlString(string entityName, string entityId, List<SqlParameter> parameters, string alias = "d")
        {
            if (parameters == null) parameters = new List<SqlParameter>();

            DynamicEntity entitySchema = await GetEntitySchemaByName(entityName);

            string fromStatement = GetFromStatement(entitySchema, alias);
            string selectStatement = GetSelectStatement(entitySchema, alias, false);

            List<DbTableInfoDto> tableInfo = await GetDbTableInfo(entitySchema.DbTableName, entitySchema.DbSchemaName);
            DbTableInfoDto pkColumn = tableInfo?.FirstOrDefault(x => x.IsPrimaryKey) ?? throw new ArgumentException(nameof(pkColumn));

            string pkStr = $"{alias}.{pkColumn.ColumnName}";
            string whereStatement = $" WHERE {pkStr} = @pkValue";

            parameters.Add(GetSqlParameter(pkColumn.DataType, false, "@pkValue", entityId));

            string sqlStr = $"{selectStatement} {fromStatement} {whereStatement} FOR JSON AUTO";

            return $"SELECT TOP 1 ({sqlStr}) AS JsonStr, 0 AS TotalCount;";
        }

        /// <summary>
        /// Генерира ORDER BY клаузата, според подадените от грида в UI клиента параметри.
        /// </summary>
        /// <param name="entitySchema"></param>
        /// <param name="sortByIntput"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private async Task<string> GetFilterStatement(DynamicEntity entitySchema, string filterVal, List<SqlParameter> parameters, string alias)
        {
            if (filterVal.IsNullOrWhiteSpace()) return null;
            if (entitySchema == null) throw new ArgumentNullException(nameof(entitySchema), nameof(DynamicEntity));
            if (entitySchema.Sections == null) throw new ArgumentNullException(nameof(entitySchema.Sections), nameof(List<DynamicEntitySection>));

            IEnumerable<DynamicEntityItem> cols = entitySchema.Sections.Where(x => x.Items != null).SelectMany(x => x.Items);
            if (!cols.Any()) throw new ArgumentNullException(nameof(List<DynamicEntityItem>));

            DynamicEntityItem[] visibleItems = cols.Where(x => x.Visible && !x.ColumnName.IsNullOrWhiteSpace()).ToArray();
            if (!visibleItems.Any())
            {
                return "";
            }

            List<DbTableInfoDto> tableInfo = await GetDbTableInfo(entitySchema.DbTableName, entitySchema.DbSchemaName);
            List<string> filterStrings = new List<string>();
            filterVal = $"%{filterVal}%";
            string paramName = "@filterVal";
            foreach (DynamicEntityItem item in visibleItems)
            {
                DbTableInfoDto dbColumnInfo = tableInfo.FirstOrDefault(x => x.ColumnName.Equals(item.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (dbColumnInfo == null) continue;

                if (dbColumnInfo.DataType == "datetime2" || dbColumnInfo.DataType == "datetime" || dbColumnInfo.DataType == "date")
                {
                    filterStrings.Add($"CONVERT(nvarchar, {alias}.{dbColumnInfo.ColumnName}, {item.Format ?? "104"}) LIKE {paramName}");
                    continue;
                }

                if (dbColumnInfo.DataType == "int" || dbColumnInfo.DataType == "bigint" || dbColumnInfo.DataType == "decimal" || dbColumnInfo.DataType == "fload")
                {
                    filterStrings.Add($"CAST({alias}.{dbColumnInfo.ColumnName} AS nvarchar) LIKE {paramName}");
                    continue;
                }

                if (dbColumnInfo.DataType == "nvarchar" || dbColumnInfo.DataType == "varchar")
                {
                    filterStrings.Add($"{alias}.{dbColumnInfo.ColumnName} LIKE {paramName}");
                    continue;
                }

                if (dbColumnInfo.DataType == "bit")
                {
                    // Todo
                }
            }

            if (!filterStrings.Any())
            {
                return "";
            }

            parameters.Add(new SqlParameter(paramName, filterVal));
            return $" WHERE {string.Join(" OR ", filterStrings)}";
        }

        /// <summary>
        /// Генерира SELECT клаузата по дадена описателна json схема.
        /// </summary>
        /// <param name="entitySchema">Json схема на ентитито.</param>
        /// <param name="alias"></param>
        /// <param name="hasToFormatColumns">Определя дали да форматира booleanfield и datefield колоните. booleanfield => Да/Не, datefield - от схемата взима ако е опсаин формар, в общите случаи 104. </param>
        /// <returns></returns>
        private string GetSelectStatement(DynamicEntity entitySchema, string alias, bool hasToFormatColumns)
        {
            if (entitySchema == null) throw new ArgumentNullException(nameof(entitySchema), nameof(DynamicEntity));
            if (entitySchema.Sections == null) throw new ArgumentNullException(nameof(entitySchema.Sections), nameof(List<DynamicEntitySection>));

            IEnumerable<DynamicEntityItem> cols = entitySchema.Sections.Where(x => x.Items != null).SelectMany(x => x.Items);
            if (!cols.Any()) throw new ArgumentNullException(nameof(List<DynamicEntityItem>));

            StringBuilder selectStatementSb = new StringBuilder(" SELECT ");

            DynamicEntityItem[] validCols = cols.Where(x => !x.ColumnName.IsNullOrWhiteSpace()).ToArray();
            for (int i = 0; i < validCols.Length; i++)
            {
                DynamicEntityItem col = validCols[i];
                string str = "";
                string colName = col.ColumnName.ToCamelCase().SafeSqlStringLiteral();

                if (hasToFormatColumns)
                {
                    str = col.Type.ToLower() switch
                    {
                        "booleanfield" => $"CASE WHEN {alias}.{colName} IS NOT NULL AND {alias}.{colName} = 1 THEN N'Да' ELSE N'Не' END AS {colName}",
                        "datefield" => col.Format.IsNullOrWhiteSpace()
                            ? $"{alias}.{colName}"
                            : $"CONVERT(nvarchar, {alias}.{colName}, {col.Format}) AS {colName}",
                        _ => $"{alias}.{colName}",
                    };
                }
                else
                {
                    str = $"{alias}.{colName}";
                }

                selectStatementSb.Append($"{(i == 0 ? " " : ", ")}{str} ");
            }

            if (selectStatementSb.Length == 0) throw new ArgumentNullException("No visible items");

            return selectStatementSb.ToString();
        }

        /// <summary>
        /// Генерира ORDER BY клаузата, според подадените от грида в UI клиента параметри.
        /// </summary>
        /// <param name="entitySchema"></param>
        /// <param name="sortByIntput"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private async Task<string> GetOrderStatement(DynamicEntity entitySchema, string sortByIntput, string alias)
        {
            if (entitySchema == null) throw new ArgumentNullException(nameof(entitySchema), nameof(DynamicEntity));
            if (entitySchema.Sections == null) throw new ArgumentNullException(nameof(entitySchema.Sections), nameof(List<DynamicEntitySection>));

            IEnumerable<DynamicEntityItem> cols = entitySchema.Sections.Where(x => x.Items != null).SelectMany(x => x.Items);
            if (!cols.Any()) throw new ArgumentNullException(nameof(List<DynamicEntityItem>));


            List<string> sortResults = new List<string>();

            if (!sortByIntput.IsNullOrWhiteSpace())
            {
                string[] sorts = sortByIntput.Split(",", StringSplitOptions.RemoveEmptyEntries);
                string[] allowedSortTypes = new string[] { "asc", "desc" };
                List<DbTableInfoDto> tableInfo = await GetDbTableInfo(entitySchema.DbTableName, entitySchema.DbSchemaName);

                foreach (string sort in sorts)
                {
                    string[] strSplit = sort.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    string columnName = strSplit.FirstOrDefault()?.Trim();
                    string orderType = strSplit.LastOrDefault()?.Trim();

                    if (await ColumnExists(tableInfo, entitySchema.DbTableName, entitySchema.DbSchemaName, columnName))
                    {
                        // Проверяваме дали колонота за сортиране съществува в базата. Така се пазим от sql инжекция.
                        sortResults.Add($" {columnName} {(!orderType.IsNullOrWhiteSpace() && allowedSortTypes.Any(x => x.Equals(orderType, StringComparison.OrdinalIgnoreCase)) && !columnName.Equals(orderType, StringComparison.OrdinalIgnoreCase) ? orderType : "")} ");
                    }
                }
            }

            if (sortResults.Any())
            {
                return $" ORDER BY {string.Join(",", sortResults)}";
            }
            else
            {
                // Задължително е да има сортиране заради страницирането.
                return !entitySchema.DefaultOrderBy.IsNullOrWhiteSpace()
                    ? $" ORDER BY {alias}.{entitySchema.DefaultOrderBy.SafeSqlStringLiteral()} "
                    : $" ORDER BY {alias}.{cols.OrderBy(x => x.Order).First().ColumnName.SafeSqlStringLiteral()} ";
            }
        }

        /// <summary>
        /// Генерира sql OFFSET / FETCH клауза, според подадените от грида в UI клиента параметри.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetOffsetStatement(int? pageIndex, int? pageSize)
        {
            pageIndex ??= 1;
            pageSize ??= 10;
            int indexFrom = 0;
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }
            if (pageSize < 0) // Значи всички
            {
                pageSize = int.MaxValue;
            }

            string offsetStatement = $" OFFSET {((pageIndex - indexFrom) * pageSize)} ROWS FETCH NEXT {pageSize} ROWS ONLY ";

            return offsetStatement;
        }

        /// <summary>
        /// Генера rawSql FROM клаузата по дадена описателна json схема.
        /// </summary>
        /// <param name="entitySchema"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private string GetFromStatement(DynamicEntity entitySchema, string alias)
        {
            if (entitySchema == null) throw new ArgumentNullException(nameof(entitySchema), nameof(DynamicEntity));
            if (entitySchema.Sections == null) throw new ArgumentNullException(nameof(entitySchema.Sections), nameof(List<DynamicEntitySection>));

            string fromStatement = $" FROM {entitySchema.DbSchemaName.SafeSqlStringLiteral()}.{entitySchema.DbTableName.SafeSqlStringLiteral()} {alias} ";

            return fromStatement;
        }

        /// <summary>
        /// Връща схемата <see cref="DynamicEntity"/> за дадено entityName <see cref="DynamicEntity"/>.Name
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        private async Task<DynamicEntity> GetEntitySchemaByName(string entityName)
        {
            _schemasByEntityType ??= new Dictionary<string, DynamicEntity>();

            if (_schemasByEntityType.ContainsKey(entityName)) return _schemasByEntityType[entityName];

            DynamicEntities schemas = await GetEntitiesJsonDescription();
            _schemasByEntityType[entityName] = schemas?.Entities?.FirstOrDefault(x => x.Name == entityName);

            return _schemasByEntityType[entityName];
        }

        /// <summary>
        /// Добавя необходимите права от глобалното <see cref="DynamicEntities"/>.Security към това на всяко отделно <see cref="DynamicEntity"/>
        /// </summary>
        /// <param name="model"></param>
        private void MergeSecurity(DynamicEntities model)
        {
            if (model == null || model.Entities == null || model.Security == null || model.Security.RequiredRermissions == null) return;

            foreach (var entity in model.Entities)
            {
                if (entity.Security == null) entity.Security = new DynamicEntitySecurity();
                if (entity.Security.RequiredRermissions == null) entity.Security.RequiredRermissions = new RequredPermission();

                if (model.Security.RequiredRermissions.Read != null && model.Security.RequiredRermissions.Read.Count > 0)
                {
                    entity.Security.RequiredRermissions.Read = (entity.Security.RequiredRermissions.Read ?? new HashSet<string>())
                        .Concat(model.Security.RequiredRermissions.Read)
                        .ToHashSet();
                }

                if (model.Security.RequiredRermissions.Create != null && model.Security.RequiredRermissions.Create.Count > 0)
                {
                    entity.Security.RequiredRermissions.Create = (entity.Security.RequiredRermissions.Create ?? new HashSet<string>())
                        .Concat(model.Security.RequiredRermissions.Create)
                        .ToHashSet();
                }

                if (model.Security.RequiredRermissions.Update != null && model.Security.RequiredRermissions.Update.Count > 0)
                {
                    entity.Security.RequiredRermissions.Update = (entity.Security.RequiredRermissions.Update ?? new HashSet<string>())
                        .Concat(model.Security.RequiredRermissions.Update)
                        .ToHashSet();
                }

                if (model.Security.RequiredRermissions.Delete != null && model.Security.RequiredRermissions.Delete.Count > 0)
                {
                    entity.Security.RequiredRermissions.Delete = (entity.Security.RequiredRermissions.Delete ?? new HashSet<string>())
                        .Concat(model.Security.RequiredRermissions.Delete)
                        .ToHashSet();
                }

            }
        }
    }
}
