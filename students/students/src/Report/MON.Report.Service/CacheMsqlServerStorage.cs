using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Telerik.Reporting.Cache.Interfaces;

namespace MON.Report.Service
{
    public class CacheMsSqlServerStorage : IStorage2, IScopedService
    {
        private interface IDeleteBehavior
        {
            void DeleteMasterKey(string key);

            void DeleteSet(string key);
        }

        private class AutoDeleteBehavior : IDeleteBehavior
        {
            private readonly CacheMsSqlServerStorage owner;

            public AutoDeleteBehavior(CacheMsSqlServerStorage owner)
            {
                this.owner = owner;
            }

            void IDeleteBehavior.DeleteMasterKey(string key)
            {
                try
                {
                    UseBehavior(new ProcedureDeleteBehavior(this.owner));
                }
                catch (Exception)
                {
                    UseBehavior(new QueryDeleteBehavior(this.owner));
                }

                void UseBehavior(IDeleteBehavior newBehavior)
                {
                    newBehavior.DeleteMasterKey(key);
                    owner.deleteBehavior = newBehavior;
                }
            }

            void IDeleteBehavior.DeleteSet(string key)
            {
                try
                {
                    UseBehavior(new ProcedureDeleteBehavior(this.owner));
                }
                catch (Exception)
                {
                    UseBehavior(new QueryDeleteBehavior(this.owner));
                }

                void UseBehavior(IDeleteBehavior newBehavior)
                {
                    newBehavior.DeleteSet(key);
                    owner.deleteBehavior = newBehavior;
                }
            }
        }

        private class ProcedureDeleteBehavior : IDeleteBehavior
        {
            private const string DeleteSetSP = "sp_tr_DeleteSet";
            private const string DeleteLikeSP = "sp_tr_DeleteLike";
            private readonly CacheMsSqlServerStorage owner;

            public ProcedureDeleteBehavior(CacheMsSqlServerStorage owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Deletes a set of values denoted by the given key.
            /// </summary>
            /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.DeleteInSet"/>
            public void DeleteSet(string key)
            {
                owner.WithStoredProc(DeleteSetSP, key, cmd => cmd.ExecuteNonQuery());
            }

            public void DeleteMasterKey(string key)
            {
                owner.WithStoredProc(DeleteLikeSP, key, cmd => cmd.ExecuteNonQuery());
            }
        }

        private class QueryDeleteBehavior : IDeleteBehavior
        {
            private const string DeleteSetQuery =
                "DELETE FROM [STUDENT].[tr_Set] WHERE [Id] = @Key";
            private const string DeleteMasterKeyQuery =
                @"DELETE FROM [STUDENT].[tr_String] WHERE [Id] LIKE @Key + '%' " +
                @"DELETE FROM [STUDENT].[tr_Object] WHERE [Id] LIKE @Key + '%' " +
                @"DELETE FROM [STUDENT].[tr_Set] WHERE [Id] LIKE @Key + '%' ";
            private readonly CacheMsSqlServerStorage owner;

            public QueryDeleteBehavior(CacheMsSqlServerStorage owner)
            {
                this.owner = owner;
            }

            void IDeleteBehavior.DeleteMasterKey(string key)
            {
                this.owner.WithCommand(DeleteMasterKeyQuery, key, cmd => cmd.ExecuteNonQuery());
            }

            void IDeleteBehavior.DeleteSet(string key)
            {
                this.owner.WithCommand(DeleteSetQuery, key, cmd => cmd.ExecuteNonQuery());
            }
        }

        private const string Schema = "[STUDENT].";
        private const string AcquireLockQuery = Schema + "sp_tr_AcquireLock";
        private const string ExistsQuery = Schema + "sp_tr_Exists";
        private const string GetStringQuery = Schema + "sp_tr_GetString";
        private const string SetStringQuery = Schema + "sp_tr_SetString";
        private const string GetBytesQuery = Schema + "sp_tr_GetBytes";
        private const string SetBytesQuery = Schema + "sp_tr_SetObject";
        private const string DeleteQuery = Schema + "sp_tr_Delete";
        private const string ExistsInSetQuery = Schema + "sp_tr_ExistsInSet";
        private const string AddInSetQuery = Schema + "sp_tr_AddInSet";
        private const string GetCountInSetQuery = Schema + "sp_tr_GetCountInSet";
        private const string GetAllMembersInSetQuery = Schema + "sp_tr_GetMembersInSet";
        private const string DeleteInSetQuery = Schema + "sp_tr_DeleteInSet";
        private const string ClearAllQuery = Schema + "sp_tr_ClearAll";
        private const int MinPoolSize = 30;
        private readonly int commandTimeout = 30; // seconds
        private IDeleteBehavior deleteBehavior;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Използва се през DI
        /// </summary>
        /// <param name="configuration"></param>
        public CacheMsSqlServerStorage(IConfiguration configuration)
        {
            this.deleteBehavior = new AutoDeleteBehavior(this);

            //connectionString = DataConnectionResolver.Default.GetConnectionString(connectionString, connectionString);

            //this.ConnectionString = AdjustConnectionString(connectionString);
            _configuration = configuration;
            var sqlConnStringBuilder = new SqlConnectionStringBuilder(_configuration.GetConnectionString("DefaultConnection"));
            var dbPass = Environment.GetEnvironmentVariable("ST__Data__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlConnStringBuilder.Password = dbPass;
            }

            this.ConnectionString = AdjustConnectionString(sqlConnStringBuilder.ConnectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheMsSqlServerStorage"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to the backend storage.
        /// <a href="http://msdn.microsoft.com/en-us/library/ms254978(v=vs.110).aspx">Connection Strings in ADO.NET</a>
        /// </param>
        public CacheMsSqlServerStorage(string connectionString)
        {
            this.deleteBehavior = new AutoDeleteBehavior(this);

            //connectionString = DataConnectionResolver.Default.GetConnectionString(connectionString, connectionString);

            //this.ConnectionString = AdjustConnectionString(connectionString);
            this.ConnectionString = AdjustConnectionString(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyMsSqlServerStorage"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to the backend storage.
        /// <a href="http://msdn.microsoft.com/en-us/library/ms254978(v=vs.110).aspx">Connection Strings in ADO.NET</a>
        /// </param>
        /// <param name="commandTimeout">
        /// Determines the CommandTimeout that will be used when executing database commands, in seconds. The default value is 30.
        /// <a href="https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand.commandtimeout">SqlCommand.CommandTimeout</a>
        /// </param>
        public CacheMsSqlServerStorage(string connectionString, int commandTimeout)
            : this(connectionString, cmdTimeout: commandTimeout)
        {
        }

        internal CacheMsSqlServerStorage(string connectionString, int? cmdTimeout)
            : this(connectionString)
        {
            this.commandTimeout = cmdTimeout ?? this.commandTimeout;
        }

        internal string ConnectionString { get; }

        // tests
        internal int CommandTimeout => this.commandTimeout;

        /// <summary>
        /// Acquires a lock on a named resource.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.AcquireLock"/>
        public IDisposable AcquireLock(string key)
        {
            return new TransactionLock(key, this);
        }

        /// <summary>
        /// Retrieves a value indicating if a single value (string or byte array)
        /// exists in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.Exists"/>
        public bool Exists(string key)
        {
            return this.WithStoredProc<bool>(
                ExistsQuery,
                key,
                cmd =>
                {
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.Add(returnValue);

                    cmd.ExecuteNonQuery();

                    return ((int)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Stores a string value under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.SetString"/>
        public void SetString(string key, string value)
        {
            this.WithStoredProc(
                SetStringQuery,
                key,
                cmd =>
                {
                    cmd.Parameters.Add(new SqlParameter("@Value", SqlDbType.NVarChar, 4000) { Value = value });
                    cmd.ExecuteNonQuery();
                });
        }

        /// <summary>
        /// Stores a byte array value under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.SetBytes"/>
        public void SetBytes(string key, byte[] value)
        {
            this.WithStoredProc(
                SetBytesQuery,
                key,
                cmd =>
                {
                    cmd.Parameters.Add(new SqlParameter("@Value", SqlDbType.Image) { Value = value });
                    cmd.ExecuteNonQuery();
                });
        }

        /// <summary>
        /// Retrieves a string value stored under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetString"/>
        public string GetString(string key)
        {
            return this.WithStoredProc<string>(
                GetStringQuery,
                key,
                cmd =>
                {
                    return (string)cmd.ExecuteScalar();
                });
        }

        /// <summary>
        /// Retrieves a byte array value stored under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetBytes"/>
        public byte[] GetBytes(string key)
        {
            return this.WithStoredProc<byte[]>(
                GetBytesQuery,
                key,
                cmd =>
                {
                    return (byte[])cmd.ExecuteScalar();
                });
        }

        /// <summary>
        /// Deletes a key with its value (string or byte array) from the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.Delete"/>
        public void Delete(string key)
        {
            this.WithStoredProc(DeleteQuery, key, cmd => cmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Retrieves a value indicating if a set of values
        /// exists in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.ExistsInSet"/>
        public bool ExistsInSet(string key, string value)
        {
            return this.WithStoredProc<bool>(
                ExistsInSetQuery,
                key,
                cmd =>
                {
                    var pValue = new SqlParameter("@Value", SqlDbType.VarChar, 255) { Value = value };
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.AddRange(new[] { pValue, returnValue });

                    cmd.ExecuteNonQuery();

                    return ((int)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Retrieves the count of the values in a set value stored in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetCountInSet"/>
        public long GetCountInSet(string key)
        {
            return this.WithStoredProc<long>(GetCountInSetQuery, key, cmd => { return (int)cmd.ExecuteScalar(); });
        }

        /// <summary>
        /// Retrieves all members in a set of string values.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetAllMembersInSet"/>
        public IEnumerable<string> GetAllMembersInSet(string key)
        {
            return this.WithStoredProc<List<string>>(
                GetAllMembersInSetQuery,
                key,
                cmd =>
                {
                    var result = new List<string>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }

                        reader.Close();
                    }

                    return result;
                });
        }

        /// <summary>
        /// Adds a single string value to a set of values denoted from the given key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.AddInSet"/>
        public void AddInSet(string key, string value)
        {
            this.WithStoredProc(
                AddInSetQuery,
                key,
                cmd =>
                {
                    var pValue = new SqlParameter("@Value", SqlDbType.VarChar, 255) { Value = value };
                    cmd.Parameters.Add(pValue);

                    cmd.ExecuteNonQuery();
                });
        }

        /// <summary>
        /// Deletes a single string value from a set of values denoted from the given key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.DeleteInSet"/>
        public bool DeleteInSet(string key, string value)
        {
            return this.WithStoredProc<bool>(
                DeleteInSetQuery,
                key,
                cmd =>
                {
                    var pValue = new SqlParameter("@Value", SqlDbType.VarChar, 255) { Value = value };
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.AddRange(new[] { pValue, returnValue });

                    cmd.ExecuteNonQuery();

                    return ((int)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Deletes a set of values denoted by the given key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.DeleteInSet"/>
        public void DeleteSet(string key)
        {
            this.deleteBehavior.DeleteSet(key);
        }

        public void DeleteMasterKey(string key)
        {
            this.deleteBehavior.DeleteMasterKey(key);
        }

        /// <summary>
        /// Utility method. Creates the data schema (tables and stored procedures) needed from the storage.
        /// </summary>
        public void CreateSchema()
        {
            var type = typeof(CacheMsSqlServerStorage);
            var assembly = type.Assembly;
            var resourceName = string.Format("{0}.Resources.{1}", type.Namespace, "MyMsSqlServerStorageDDL.sql");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var ddl = reader.ReadToEnd();

            using var conn = NewConnection();
            var cmd = new SqlCommand()
            {
                CommandType = CommandType.Text,
                Connection = conn,
                CommandTimeout = this.commandTimeout
            };

            foreach (var script in ddl.Split(new[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                cmd.CommandText = script;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Utility method. Clears all data from the storage data tables.
        /// </summary>
        public void ClearAllData()
        {
            using var conn = NewConnection();
            var cmd = new SqlCommand(ClearAllQuery, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = this.commandTimeout
            };

            cmd.ExecuteNonQuery();
        }

        private static string AdjustConnectionString(string connectionString)
        {
            var builder =
                new SqlConnectionStringBuilder(connectionString)
                {
                    ApplicationName = "TR",
                };

            if (!builder.ShouldSerialize("Min Pool Size") &&
                builder.MinPoolSize == 0 &&
                builder.MaxPoolSize >= MinPoolSize)
            {
                builder.MinPoolSize = MinPoolSize;
            }

            return builder.ConnectionString;
        }

        private static SqlParameter CreateKeyParameter(string key)
        {
            return new SqlParameter("@Key", SqlDbType.VarChar, 255)
            {
                Value = key,
            };
        }

        private static SqlParameter CreateReturnValueParameter()
        {
            return new SqlParameter()
            {
                Direction = ParameterDirection.ReturnValue,
            };
        }

        private void WithStoredProc(string query, string key, Action<SqlCommand> action)
        {
            using var conn = NewConnection();
            var cmd = new SqlCommand(query, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = commandTimeout
            };

            cmd.Parameters.Add(CreateKeyParameter(key));

            action(cmd);
        }

        private T WithStoredProc<T>(string query, string key, Func<SqlCommand, T> function)
        {
            using var conn = this.NewConnection();
            var cmd = new SqlCommand(query, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = commandTimeout
            };

            cmd.Parameters.Add(CreateKeyParameter(key));

            return function(cmd);
        }

        private void WithCommand(string query, string key, Action<SqlCommand> action)
        {
            using var conn = NewConnection();
            var cmd = new SqlCommand(query, conn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = commandTimeout
            };

            cmd.Parameters.Add(CreateKeyParameter(key));

            action(cmd);
        }

        private SqlConnection NewConnection()
        {
            var c = new SqlConnection(ConnectionString);
            c.Open();
            return c;
        }

        private class TransactionLock : IDisposable
        {
            public TransactionLock(string key, CacheMsSqlServerStorage owner)
            {
                this.Connection = owner.NewConnection();
                Transaction = this.Connection.BeginTransaction(IsolationLevel.Serializable);

                this.ExecuteAcquireCommand(key);
            }

            public SqlTransaction Transaction { get; private set; }

            public SqlConnection Connection { get; private set; }

            public void Dispose()
            {
                try
                {
                    this.Transaction.Rollback();
                }
                finally
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
            }

            private void ExecuteAcquireCommand(string key)
            {
                var cmd = new SqlCommand(AcquireLockQuery, this.Connection, this.Transaction)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddRange(new[] { CreateKeyParameter(key) });

                cmd.ExecuteNonQuery();
            }
        }
    }
}
