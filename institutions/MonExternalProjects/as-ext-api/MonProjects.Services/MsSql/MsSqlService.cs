namespace MonProjects.Services.MsSql
{
    using Configurations.Contracts;
    using Constants;
    using Dapper;
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public abstract class MsSqlService : IMsSqlService
    {
        private readonly IDatabaseDetailsSection database;
        public MsSqlService(IDatabaseDetailsSection database)
            => this.database = database;
        

        public async Task<TModel> ExecuteFirstAsync<TModel>(string procedureName, object parameters = null)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(this.database.ConnectionString);
                
                return await connection.QueryFirstOrDefaultAsync<TModel>(
                    procedureName,
                    parameters,
                    null,
                    this.database.CommandTimeout,
                    CommandType.StoredProcedure);
                
            }
            catch (SqlException ex)
            {
                string exceptionMessage = string.Format(AppExceptions.CannotExecuteProcedureExceptionMessage, procedureName);

                throw new MsSqlException(exceptionMessage, ex);
            }
            catch (Exception ex)
            {
                string exceptionMessage = string.Format(AppExceptions.CannotExecuteProcedureExceptionMessage, procedureName);

                throw new MsSqlException(exceptionMessage, ex);
            }
        }

        public async Task<IEnumerable<TModel>> ExecuteListAsync<TModel>(string procedureName, object parameters = null)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(this.database.ConnectionString);
                
                return await connection.QueryAsync<TModel>(
                    procedureName,
                    parameters,
                    null,
                    this.database.CommandTimeout,
                    CommandType.StoredProcedure);
                
            }
            catch (SqlException ex)
            {
                string exceptionMessage = string.Format(AppExceptions.CannotExecuteProcedureExceptionMessage, procedureName);

                throw new MsSqlException(exceptionMessage, ex);
            }
            catch (Exception ex)
            {
                string exceptionMessage = string.Format(AppExceptions.CannotExecuteProcedureExceptionMessage, procedureName);

                throw new MsSqlException(exceptionMessage, ex);
            }
        }
    }
}
