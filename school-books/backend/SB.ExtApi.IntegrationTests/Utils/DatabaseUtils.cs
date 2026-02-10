namespace SB.ExtApi.IntegrationTests;

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable VSTHRD012 // Provide JoinableTaskFactory where allowed

public static class DatabaseUtils
{
    private static readonly string DbIP = Environment.GetEnvironmentVariable("SB__Data__DbIP") ?? throw new Exception("SB__Data__DbIP environment variable is not set.");
    private static readonly string DbPort = Environment.GetEnvironmentVariable("SB__Data__DbPort") ?? throw new Exception("SB__Data__DbPort environment variable is not set.");
    private static readonly string DbUser = Environment.GetEnvironmentVariable("SB__Data__DbUser") ?? throw new Exception("SB__Data__DbUser environment variable is not set.");
    private static readonly string DbName = Environment.GetEnvironmentVariable("SB__Data__DbName") ?? throw new Exception("SB__Data__DbName environment variable is not set.");
    private static readonly string DbPass = Environment.GetEnvironmentVariable("SB__Data__DbPass") ?? throw new Exception("SB__Data__DbPass environment variable is not set.");
    public static readonly string MasterDbConnectionString = $"Server={DbIP},{DbPort};User Id={DbUser};Password={DbPass};Initial Catalog=master;Encrypt=False;Pooling=False;";
    public static readonly string ConnectionString = $"Server={DbIP},{DbPort};User Id={DbUser};Password={DbPass};Initial Catalog={DbName};Encrypt=False;Pooling=False;";
    // use Poolling=False to avoid connection errors as the restore operation drops all existing connections
    public static readonly string WebApplicationFactoryConnectionString = $"Server={DbIP},{DbPort};User Id={DbUser};Password={DbPass};Initial Catalog={DbName};Encrypt=False;App=sb-api;Pooling=False;";

    private static AsyncLazy<string> DataPath =
        new(async () =>
            {
                await using SqlConnection connection = new(MasterDbConnectionString);
                await connection.OpenAsync();

                using SqlCommand cmd = new("SELECT serverproperty('InstanceDefaultDataPath')", connection);
                return (string)(await cmd.ExecuteScalarAsync() ?? throw new Exception("InstanceDefaultDataPath is null"));
            });

    private static AsyncLazy<(string logicalName, string physicalName)[]> DataFiles =
        new(async () =>
            {
                await using SqlConnection connection = new(MasterDbConnectionString);
                await connection.OpenAsync();

                using SqlCommand cmd = new("""
                    SELECT
                        d.name AS DatabaseName,
                        m.name AS LogicalName,
                        m.physical_name AS PhysicalName,
                        size AS FileSize
                    FROM sys.master_files m
                    INNER JOIN sys.databases d ON m.database_id = d.database_id
                    WHERE d.name = @DatabaseName AND m.type_desc != 'LOG'
                    ORDER BY LogicalName
                    """,
                    connection);
                cmd.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        new("@DatabaseName", DbName),
                    });

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                List<(string logicalName, string physicalName)> result = new();
                while (await reader.ReadAsync())
                {
                    result.Add((
                        logicalName: reader["LogicalName"].ToString() ?? string.Empty,
                        physicalName: reader["PhysicalName"].ToString() ?? string.Empty
                    ));
                }

                return result.ToArray();
            });

    private static AsyncLazy<string> SqlVersion =
        new(async () =>
            {
                await using SqlConnection connection = new(MasterDbConnectionString);
                await connection.OpenAsync();

                using SqlCommand cmd = new("SELECT @@VERSION", connection);
                return (string)(await cmd.ExecuteScalarAsync() ?? throw new Exception("Version is null"));
            });

    public static async Task<bool> IsAzureSqlEdgeAsync()
    {
        var sqlVersion = await SqlVersion.GetValueAsync();
        return sqlVersion.Contains("Microsoft Azure SQL Edge");
    }

    public static async Task CreateSnapshotAsync()
    {
        await using SqlConnection connection = new(MasterDbConnectionString);
        await connection.OpenAsync();

        string files =
            string.Join(", ",
                (await DataFiles.GetValueAsync())
                .Select(df => $"(NAME=[{df.logicalName}], FILENAME=''{Path.ChangeExtension(df.physicalName, ".snapshot")}'')")
            );

        using SqlCommand cmd = new();
        cmd.Connection = connection;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = $"""
            BEGIN
                SET NOCOUNT ON;
                DECLARE @sql nvarchar(max)
                SELECT @sql = 'CREATE DATABASE [' + @snapshotDatabaseName + ']' +
                                ' ON {files} ' +
                                ' AS SNAPSHOT OF [' + @databaseName + ']'
                EXEC(@sql)
            END
            """;
        cmd.Parameters.AddWithValue("@databaseName", DbName);
        cmd.Parameters.AddWithValue("@snapshotDatabaseName", $"{DbName}_test_snapshot");
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task RestoreSnapshotAsync()
    {
        await using SqlConnection connection = new(MasterDbConnectionString);
        await connection.OpenAsync();

        using SqlCommand cmd = new();
        cmd.Connection = connection;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = """
            BEGIN
                SET NOCOUNT ON;
                DECLARE @sql nvarchar(max)
                SET @sql  = 'ALTER DATABASE [' + @databaseName + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE'
                EXEC (@sql)
                RESTORE DATABASE @databaseName
                FROM DATABASE_SNAPSHOT = @snapshotDatabaseName
                SET @sql = 'ALTER DATABASE [' + @databaseName + '] SET MULTI_USER'
                EXEC (@sql)
            END
            """;
        cmd.Parameters.AddWithValue("@databaseName", DbName);
        cmd.Parameters.AddWithValue("@snapshotDatabaseName", $"{DbName}_test_snapshot");
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task TryDeleteSnapshotAsync()
    {
        await using SqlConnection connection = new(MasterDbConnectionString);
        await connection.OpenAsync();

        using SqlCommand cmd = new();
        cmd.Connection = connection;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = """
            IF EXISTS (SELECT name FROM sys.databases WHERE name = @snapshotDatabaseName)
            BEGIN
                SET NOCOUNT ON;
                DECLARE @sql nvarchar(max)
                SELECT @sql = 'DROP DATABASE [' + @snapshotDatabaseName + ']'
                EXEC(@sql)
            END
            """;
        cmd.Parameters.AddWithValue("@snapshotDatabaseName", $"{DbName}_test_snapshot");
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task CreateBackupAsync()
    {
        await using SqlConnection connection = new(MasterDbConnectionString);
        await connection.OpenAsync();

        using SqlCommand cmd = new();
        cmd.Connection = connection;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = """
            BEGIN
                SET NOCOUNT ON;
                DECLARE @sql nvarchar(max)
                SELECT @sql = 'BACKUP DATABASE [' + @databaseName + ']' +
                                ' TO DISK = ''' + @backupPath + @backupName + '.bak''' +
                                ' WITH COPY_ONLY'
                EXEC(@sql)
            END
            """;
        cmd.Parameters.AddWithValue("@databaseName", DbName);
        cmd.Parameters.AddWithValue("@backupPath", await DataPath.GetValueAsync());
        cmd.Parameters.AddWithValue("@backupName", $"{DbName}_test_backup");
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task RestoreBackupAsync()
    {
        await using SqlConnection connection = new(MasterDbConnectionString);
        await connection.OpenAsync();

        using SqlCommand cmd = new();
        cmd.Connection = connection;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = """
            BEGIN
                SET NOCOUNT ON;
                DECLARE @sql nvarchar(max)
                SET @sql  = 'ALTER DATABASE [' + @databaseName + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE'
                EXEC (@sql)

                SET @sql = 'RESTORE DATABASE [' + @databaseName + ']' +
                            ' FROM DISK = ''' + @backupPath + @backupName + '.bak''' +
                            ' WITH RECOVERY'
                EXEC (@sql)

                SET @sql = 'ALTER DATABASE [' + @databaseName + '] SET MULTI_USER'
                EXEC (@sql)
            END
            """;
        cmd.Parameters.AddWithValue("@databaseName", DbName);
        cmd.Parameters.AddWithValue("@backupPath", await DataPath.GetValueAsync());
        cmd.Parameters.AddWithValue("@backupName", $"{DbName}_test_backup");
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task TryDeleteBackupAsync()
    {
        await using SqlConnection connection = new(MasterDbConnectionString);
        await connection.OpenAsync();

        using SqlCommand cmd = new();
        cmd.Connection = connection;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = """
            IF EXISTS (SELECT 1 FROM sys.dm_os_enumerate_filesystem(@backupPath, @backupName + '.bak') WHERE is_directory = 0)
            BEGIN
                DECLARE @backupFullPath nvarchar(4000) = @backupPath + @backupName + '.bak';
                EXEC sys.xp_delete_files @backupFullPath
            END
            """;
        cmd.Parameters.AddWithValue("@backupPath", await DataPath.GetValueAsync());
        cmd.Parameters.AddWithValue("@backupName", $"{DbName}_test_backup");
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task CustomizeTestDataAsync()
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        using SqlCommand setCbExtProviderCmd = new();
        setCbExtProviderCmd.Connection = connection;
        setCbExtProviderCmd.CommandType = CommandType.Text;
        setCbExtProviderCmd.CommandText = """
            UPDATE [school_books].[ClassBookExtProvider]
            SET [ExtSystemId] = 1
            WHERE [InstId] IN (300125, 300110)
                AND [SchoolYear] IN (2022, 2023, 2024, 2025);

            UPDATE [school_books].[ClassBookExtProvider]
            SET [ScheduleExtSystemId] = 2
            WHERE [InstId] IN (2206409)
                AND [SchoolYear] IN (2022, 2023, 2024, 2025);
            """;
        await setCbExtProviderCmd.ExecuteNonQueryAsync();

        using SqlCommand clearSchoolYearSettingsCmd = new();
        clearSchoolYearSettingsCmd.Connection = connection;
        clearSchoolYearSettingsCmd.CommandType = CommandType.Text;
        clearSchoolYearSettingsCmd.CommandText = """
            UPDATE cbsys
            SET
                [SchoolYearSettingsId] = NULL,
                [SchoolYearStartDateLimit] = sysd.[OtherSchoolYearStartDateLimit],
                [SchoolYearStartDate] = sysd.[OtherSchoolYearStartDate],
                [FirstTermEndDate] = sysd.[OtherFirstTermEndDate],
                [SecondTermStartDate] = sysd.[OtherSecondTermStartDate],
                [SchoolYearEndDate] = sysd.[OtherSchoolYearEndDate],
                [SchoolYearEndDateLimit] = sysd.[OtherSchoolYearEndDateLimit],
                [HasFutureEntryLock] = 0,
                [PastMonthLockDay] = NULL
            FROM [school_books].[ClassBookSchoolYearSettings] cbsys
            JOIN [school_books].[ClassBook] cb ON cbsys.[SchoolYear] = cb.[SchoolYear] AND cbsys.[ClassBookId] = cb.[ClassBookId]
            JOIN [school_books].[SchoolYearSettingsDefault] sysd ON cbsys.[SchoolYear] = sysd.[SchoolYear]
            WHERE
                cb.[InstId] = 300125
                AND cb.[SchoolYear] = 2025;

            DELETE sysc
            FROM [school_books].[SchoolYearSettingsClass] sysc
                INNER JOIN [school_books].[SchoolYearSettings] sys ON sysc.[SchoolYear] = sys.[SchoolYear] AND sysc.[SchoolYearSettingsId] = sys.[SchoolYearSettingsId]
            WHERE sys.[InstId] = 300125
                AND sys.[SchoolYear] = 2025;

            DELETE FROM [school_books].[SchoolYearSettings]
            WHERE [InstId] = 300125
                AND [SchoolYear] = 2025
            """;
        await clearSchoolYearSettingsCmd.ExecuteNonQueryAsync();

        using SqlCommand clearTopicsCmd = new();
        clearTopicsCmd.Connection = connection;
        clearTopicsCmd.CommandType = CommandType.Text;
        clearTopicsCmd.CommandText = """
            DELETE tt
            FROM [school_books].[TopicTeacher] tt
            JOIN [school_books].[Topic] t ON tt.[SchoolYear] = t.[SchoolYear] AND tt.[TopicId] = t.[TopicId]
            JOIN [school_books].[ClassBook] cb ON t.[SchoolYear] = cb.[SchoolYear] AND t.[ClassBookId] = cb.[ClassBookId]
            WHERE cb.[InstId] = 300125
                AND cb.[SchoolYear] = 2025
                AND cb.[FullBookName] IN (N'четвърта - ПГ 5 и 6 годишни', N'5 - в')

            DELETE tt
            FROM [school_books].[TopicTitle] tt
            JOIN [school_books].[Topic] t ON tt.[SchoolYear] = t.[SchoolYear] AND tt.[TopicId] = t.[TopicId]
            JOIN [school_books].[ClassBook] cb ON t.[SchoolYear] = cb.[SchoolYear] AND t.[ClassBookId] = cb.[ClassBookId]
            WHERE cb.[InstId] = 300125
                AND cb.[SchoolYear] = 2025
                AND cb.[FullBookName] IN (N'четвърта - ПГ 5 и 6 годишни', N'5 - в')

            DELETE t
            FROM [school_books].[Topic] t
            JOIN [school_books].[ClassBook] cb ON t.[SchoolYear] = cb.[SchoolYear] AND t.[ClassBookId] = cb.[ClassBookId]
            WHERE cb.[InstId] = 300125
                AND cb.[SchoolYear] = 2025
                AND cb.[FullBookName] IN (N'четвърта - ПГ 5 и 6 годишни', N'5 - в')
            """;
        await clearTopicsCmd.ExecuteNonQueryAsync();
    }
}
