package bg.adminsoft.neispuo.config;

import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.jdbc.core.JdbcTemplate;

import java.sql.*;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;

@Configuration
//@Lazy(value = true)
@Getter
@RequiredArgsConstructor
@Slf4j
public class DatabaseConfig {

    private final JdbcTemplate jdbcTemplate;

    @Value("${com.adminsoft.neispuo.staging.connectionUrl}")
    private String connectionUrl;
    @Value("${com.adminsoft.neispuo.staging.schema}")
    private String stagingSchema;
    @Value("${com.adminsoft.neispuo.staging.tables}")
    private String stagingTablesList;
    @Value("${com.adminsoft.neispuo.core.tables}")
    private String coreTablesList;
    @Value("${com.adminsoft.neispuo.inst_basic.tables}")
    private String instBasicTablesList;
    @Value("${com.adminsoft.neispuo.inst_year.tables}")
    private String instYearTablesList;
    @Value("${com.adminsoft.neispuo.student.tables}")
    private String studentTablesList;
    @Value("${com.adminsoft.neispuo.family.tables}")
    private String familyTablesList;

    public Connection dbConnection() {
        Connection connection = null;
        try {
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
            connection = DriverManager.getConnection(connectionUrl);
            connection.setAutoCommit(false);
        } catch (Exception e) {
            log.error("Error connecting Database ! " + e.getMessage());
            throw new InternalServerErrorException("Няма връзка с базата данни! Обърнете се за съдействие към системния администратор!");
        }
        return connection;
    }

    public void closeConnection(Connection connection) {
        if (connection != null) {
            try { connection.close(); } catch (SQLException ignored) { }
        }
    }

    public void closeConnectionWithCommit(Connection connection) {
        if (connection != null) {
            try {
                connection.commit();
            } catch (SQLException e) {
                log.error("Error committing transaction ! " + e.getMessage());
                throw new InternalServerErrorException("Транзакцията не може да бъде потвърдена! Обърнете се за съдействие към системния администратор!");
            }
            try { connection.close(); } catch (SQLException ignored) { }
        }
    }

    public void closeConnectionWithRollback(Connection connection) {
        if (connection != null) {
            try { connection.rollback(); } catch (SQLException ignored) {  }
            try { connection.close(); } catch (SQLException ignored) { }
        }
    }

    @Bean(name="stagingSchemaMetadata")
    public Map<String, TableMetadata> stagingSchemaMetadata() {
        String [] stagingTables = stagingTablesList.split("[ ,]+");
        Map<String, TableMetadata> metadata;
        Connection connection = null;
        try {
            connection = dbConnection();
            metadata = getMetadata(stagingSchema, stagingTables, connection);
        } finally {
            closeConnection(connection);
        }
        return metadata;
    }

    @Bean(name="institutionSchemaMetadata")
    public Map<String, TableMetadata> institutionSchemaMetadata() {
        String [] coreTables = coreTablesList.split("[ ,]+");
        String [] instBasicTables = instBasicTablesList.split("[ ,]+");
        String [] instYearTables = instYearTablesList.split("[ ,]+");
        String [] studentTables = studentTablesList.split("[ ,]+");
        String [] familyTables = familyTablesList.split("[ ,]+");
        Map<String, TableMetadata> coreMetadata;
        Connection connection = null;
        try {
            connection = dbConnection();
            coreMetadata = getMetadata("core", coreTables, connection);
            Map<String, TableMetadata> instBasicMetadata = getMetadata("inst_basic", instBasicTables, connection);
            Map<String, TableMetadata> instYearMetadata = getMetadata("inst_year", instYearTables, connection);
            Map<String, TableMetadata> studentMetadata = getMetadata("student", studentTables, connection);
            Map<String, TableMetadata> familyMetadata = getMetadata("family", familyTables, connection);
            coreMetadata.putAll(instBasicMetadata);
            coreMetadata.putAll(instYearMetadata);
            coreMetadata.putAll(studentMetadata);
            coreMetadata.putAll(familyMetadata);
        } finally {
            closeConnection(connection);
        }
        return coreMetadata;
    }

    private Map<String, TableMetadata> getMetadata(String schema, String [] tables, Connection connection) {
        Map<String, TableMetadata> metadata = new HashMap<String, TableMetadata>();
        try {
            String sqlMetadata =
                    "SELECT TABLE_SCHEMA AS schemaName, " +
                    "       TABLE_NAME AS tableName, " +
                    "       COLUMN_NAME AS columnName, " +
                    "       DATA_TYPE AS typeName, " +
                    "       CHARACTER_MAXIMUM_LENGTH AS maxLength, " +
                    "       NUMERIC_PRECISION AS precision, " +
                    "       NUMERIC_SCALE AS scale, " +
                    "       IS_NULLABLE AS isNullable" +
                    "  FROM INFORMATION_SCHEMA.COLUMNS " +
                    " WHERE TABLE_SCHEMA = '" + schema + "' " +
                    "   AND TABLE_NAME = ? " +
                    " ORDER BY ORDINAL_POSITION";
            PreparedStatement stmt = connection.prepareStatement(sqlMetadata);
            for (String table : tables) {
                stmt.setString(1, table);
                ResultSet rsMetadata = stmt.executeQuery();
                TableMetadata tableMetadata = new TableMetadata(schema, table, null,
                        new LinkedHashMap<>(), new LinkedHashMap<>());
                while (rsMetadata.next()) {
                    String columnName = rsMetadata.getString("columnName");

                    String typeName = rsMetadata.getString("typeName");
                    tableMetadata.getColumns().put(columnName, typeName);

                    String maxLength = rsMetadata.getString("maxLength");
                    Integer precision = rsMetadata.getInt("precision");
                    Integer scale = rsMetadata.getInt("scale");
                    String isNullable = rsMetadata.getString("isNullable");
                    tableMetadata.getColumnsDocumentation().put(columnName, new HashMap<>());
                    Map<String, Object> column = tableMetadata.getColumnsDocumentation().get(columnName);
                    column.put("typeName", typeName);
                    column.put("maxLength", maxLength);
                    column.put("precision", precision);
                    column.put("scale", scale);
                    column.put("isNullable", isNullable);
                }
                metadata.put(table, tableMetadata);
            }

            String sqlMetadataDoc =
                    "SELECT t.name as tableName, " +
                    "       c.name as columnName, " +
                    "       td.value as tableDescription, " +
                    "       cd.value as columnDescription " +
                    "  FROM sys.schemas s " +
                    " INNER JOIN sys.tables t ON t.schema_id = s.schema_id " +
                    " INNER JOIN sys.columns c ON c.object_id = t.object_id " +
                    "  LEFT JOIN sys.extended_properties td " +
                    "    ON td.major_id = t.object_id AND td.minor_id = 0 AND td.name = 'MS_Description' " +
                    "  LEFT JOIN sys.extended_properties cd " +
                    "    ON cd.major_id = c.object_id AND cd.minor_id = c.column_id  AND cd.name = 'MS_Description' " +
                    " WHERE t.name = ? " +
                    "   AND s.name = '" + schema + "' ";
            PreparedStatement stmtDoc = connection.prepareStatement(sqlMetadataDoc);
            for (String table : tables) {
                stmtDoc.setString(1, table);
                ResultSet rsMetadata = stmtDoc.executeQuery();
                TableMetadata tableMetadata = metadata.get(table);
                while (rsMetadata.next()) {
                    String tableDescription = rsMetadata.getString("tableDescription");
                    tableMetadata.setDescription(tableDescription);
                    String columnName = rsMetadata.getString("columnName");
                    String columnDescription = rsMetadata.getString("columnDescription");
                    Map<String, Object> column = tableMetadata.getColumnsDocumentation().get(columnName);
                    column.put("description", columnDescription);
                }
                metadata.put(table, tableMetadata);
            }
        }
        catch (SQLException e) {
            log.error("Error loading metadata ! " + e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }
        return metadata;
    }
}
