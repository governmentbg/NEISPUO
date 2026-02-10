namespace NeispuoExtension.Database.Services.NeispuoDatabase
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Dapper;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Options;

    using Settings;
    using Settings.Database;
    using Exceptions.Database;

    using Messages = ExceptionMessages;

    public class NeispuoDatabaseService : INeispuoDatabaseService
    {
        private IDatabaseSettings settings;

        public NeispuoDatabaseService(IOptions<ApplicationSettings> options)
        {
            this.settings = options.Value.NeispuoDatabase;
        }

        public async Task<TModel> ExecuteFirstAsync<TModel>(string procedureName, object parameters = null)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(this.settings.ConnectionString);

                return await connection.QueryFirstOrDefaultAsync<TModel>(
                    procedureName,
                    parameters,
                    null,
                    this.settings.CommandTimeout,
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                string exceptionMessage = this.GenerateExceptionMessage(procedureName, parameters, ex.Message);

                throw new MsSqlException(exceptionMessage, ex);
            }
        }

        public async Task<IEnumerable<TModel>> ExecuteListAsync<TModel>(string procedureName, object parameters = null)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(this.settings.ConnectionString);

                return await connection.QueryAsync<TModel>(
                    procedureName,
                    parameters,
                    transaction: null,
                    commandTimeout: this.settings.CommandTimeout,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                string exceptionMessage = this.GenerateExceptionMessage(procedureName, parameters, ex.Message);
                throw new MsSqlException(exceptionMessage, ex);
            }
        }

        private string GenerateExceptionMessage(string procedureName, object parameters, string message)
        {
            string query = $"EXEC {procedureName}";

            if (!Object.Equals(parameters, null))
            {
                Type type = parameters.GetType();
                PropertyInfo[] properties = type.GetProperties();
                query += $" {String.Join(", ", properties.Select(x => x.GetValue(parameters) == null ? $"@{x.Name} = NULL" : $"@{x.Name} = '{x.GetValue(parameters)}'"))}";
            }

            return String.Format(Messages.StoredProcedureFailed, procedureName, message, query);
        }
    }
}
