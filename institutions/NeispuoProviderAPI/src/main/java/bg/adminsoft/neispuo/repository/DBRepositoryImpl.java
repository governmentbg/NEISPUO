package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.config.TableMetadata;
import bg.adminsoft.neispuo.exception.BadRequestException;
import bg.adminsoft.neispuo.exception.ForegnKeyViolationException;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.namedparam.MapSqlParameterSource;
import org.springframework.jdbc.core.namedparam.SqlParameterSource;
import org.springframework.jdbc.core.simple.SimpleJdbcCall;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;
import java.sql.*;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.*;

@Component
@Slf4j
@RequiredArgsConstructor
public class DBRepositoryImpl implements DBRepository {

    private final DatabaseConfig databaseConfig;
    private final JdbcTemplate jdbcTemplate;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public StagingTableKey getJsonPrimaryKey(JsonMapping table, LinkedHashMap row, String ticket) {
        if (table.getTable() == null) {
            return null;
        }

        String externalId = null;
        String externalParentId = null;
        String externalParentId2 = null;
        Long recordId = null;
        for (JsonMapping.AttributeMapping column : table.getAttributeMapping()) {
            if (column.getToColumn().equals(table.getPrimaryKey())) {
                Object rowValue = row.get(column.getFromAttribute());
                recordId = rowValue != null ? Long.valueOf(rowValue.toString()) : null;
            } else if (column.getToColumn().equals("ExternalID")) {
                externalId = (String) row.get(column.getFromAttribute());
            } else if (column.getToColumn().equals("ExternalParentID")) {
                externalParentId = (String) row.get(column.getFromAttribute());
            } else if (column.getToColumn().equals("ExternalParentID2")) {
                externalParentId2 = (String) row.get(column.getFromAttribute());
            }
        }
        return new StagingTableKey(externalId, externalParentId, externalParentId2, recordId, null, null);
    }

    @Override
    public int validateStaging(TicketData ticket) {
        try {
            SimpleJdbcCall simpleJdbcCall = new SimpleJdbcCall(jdbcTemplate)
                    .withSchemaName("staging")
                    .withProcedureName("doAPIValidityCheck");
            Map<String, Object> inParamMap = new HashMap<String, Object>();
            inParamMap.put("TicketID", ticket.getTicketId());
            SqlParameterSource in = new MapSqlParameterSource(inParamMap);
            Map<String, Object> simpleJdbcCallResult = simpleJdbcCall.execute(in);
            if (simpleJdbcCallResult.size() > 0) {
                return (Integer) simpleJdbcCallResult.values().toArray()[0];
            }
            return 1;
        } catch (DataAccessException e) {
            String message = "Логическа проверка на данните. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public StagingTableKey insertJsonIntoStaging(JsonMapping table, LinkedHashMap row, String ticket,
                                                 StagingTableKey parentKey, JsonDefaultData defaultData,
                                                 Connection connection) {
        if (table == null || table.getTable() == null) {
            return null;
        }

        /*if ("Student".equals(table.getTable())) { // for test
            System.out.println(table.getTable());
        }*/

        StagingTableKey primaryKey = new StagingTableKey();
        try {
            TableMetadata metadata = databaseConfig.stagingSchemaMetadata().get(table.getTable());

            String sqlColumns = "TicketID";
            String sqlValues = "CAST('" + ticket + "' AS UNIQUEIDENTIFIER)";

            if (table.getInstitutionLink() != null && !table.getInstitutionLink().isEmpty()) {
                sqlColumns += ", " + table.getInstitutionLink();
                sqlValues += ", " + defaultData.getInstitutionId();
            }

            if (table.getSchoolYear() != null && !table.getSchoolYear().isEmpty()) {
                sqlColumns += ", " + table.getSchoolYear();
                sqlValues += ", " + defaultData.getSchoolYear();
            }

            if (parentKey != null) {
                if (table.getForeignKey() != null && !"".equals(table.getForeignKey())) {
                    sqlColumns += ", " + table.getForeignKey();
                    sqlValues += ", " + parentKey.getRecordId();
                }
                if (metadata.getColumns().get("ExternalParentID") != null) {
                    sqlColumns += ", ExternalParentID";
                    sqlValues += ", '" + parentKey.getExternalId() + "'";
                }
                if (table.getParentForeignKey() != null && !"".equals(table.getParentForeignKey())) {
                    sqlColumns += ", " + table.getParentForeignKey();
                    sqlValues += ", '" + parentKey.getParentRecordId() + "'";
                    if (metadata.getColumns().get("ExternalParentID2") != null) {
                        sqlColumns += ", ExternalParentID2";
                        sqlValues += ", '" + parentKey.getParentExternalId() + "'";
                    }
                }
            }
            Object sqlPK = ""; // ""Output Inserted." + table.getPrimaryKey();
            Map<String, Object> params = new LinkedHashMap<String, Object>();
            for (JsonMapping.AttributeMapping column : table.getAttributeMapping()) {
                if (!column.getToColumn().equals(table.getForeignKey())) {
                    sqlColumns += ", " + column.getToColumn();
                    if ("uniqueidentifier".equals(metadata.getColumns().get(column.getToColumn()))) {
                        sqlValues += ", CAST(? AS UNIQUEIDENTIFIER)";
                    } else {
                        sqlValues += ", ?";
                    }
                }
            }

            String sql = "INSERT INTO [" + databaseConfig.getStagingSchema() +
                    "].[" + table.getTable() + "] (" +
                    sqlColumns + ") " + sqlPK + " VALUES (" + sqlValues + ")";

            PreparedStatement stmt = connection.prepareStatement(sql);
            int pos = 1;
            for (JsonMapping.AttributeMapping column : table.getAttributeMapping()) {
                String typeName = metadata.getColumns().get(column.getToColumn());
                if (typeName == null) {
                    String message = ticket + ": Липсва колона " + column.getToColumn() +
                            " в таблица " + databaseConfig.getStagingSchema() + "." + table.getTable();
                    log.error(message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                    throw new BadRequestException(message);
                }
                Object rowValue = row.get(column.getFromAttribute());
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    primaryKey.setRecordId(rowValue != null ? Long.valueOf(rowValue.toString()) : null);
                }
                if ("ExternalID".equals(column.getToColumn())) {
                    if (rowValue instanceof Long || rowValue instanceof Integer) {
                        primaryKey.setExternalId(rowValue.toString());
                    } else {
                        primaryKey.setExternalId((String) rowValue);
                    }
                }
                if (rowValue != null && !"".equals(rowValue)) {
                    if ("uniqueidentifier".equals(typeName)) {
                        stmt.setDate(pos, toSqlDate((String) rowValue));
                    } else if ("date".equals(typeName) ||
                            "datetime".equals(typeName) || "datetime2".equals(typeName)) {
                        stmt.setDate(pos, toSqlDate((String) rowValue));
                    } else if ("bit".equals(typeName) || "smallint".equals(typeName) ||
                            "int".equals(typeName) || "float".equals(typeName) ||
                            "decimal".equals(typeName) || "real".equals(typeName)) {
                        if (rowValue instanceof String) {
                            stmt.setBigDecimal(pos, new BigDecimal((String) rowValue));
                        } else if (rowValue instanceof Long) {
                            stmt.setBigDecimal(pos, new BigDecimal((Long) rowValue));
                        } else if (rowValue instanceof Integer) {
                            stmt.setBigDecimal(pos, new BigDecimal((Integer) rowValue));
                        } else if (rowValue instanceof Short) {
                            stmt.setBigDecimal(pos, new BigDecimal((Short) rowValue));
                        } else if (rowValue instanceof Double) {
                            stmt.setBigDecimal(pos, BigDecimal.valueOf((Double) rowValue));
                        } else if (rowValue instanceof Float) {
                            stmt.setBigDecimal(pos, BigDecimal.valueOf((Float) rowValue));
                        } else if (rowValue instanceof Boolean) {
                            stmt.setInt(pos, ((Boolean) rowValue ? 1 : 0));
                        }
                    } else if ("nvarchar".equals(typeName)) {
                        if (rowValue instanceof String) {
                            stmt.setString(pos, (String) rowValue);
                        } else {
                            stmt.setString(pos, rowValue.toString());
                        }
                    }
                } else {
                    if ("date".equals(typeName) ||
                            "datetime".equals(typeName) || "datetime2".equals(typeName)) {
                        stmt.setNull(pos, Types.DATE);
                    } else if ("real".equals(typeName)) {
                        stmt.setNull(pos, Types.REAL);
                    } else if ("float".equals(typeName)) {
                        stmt.setNull(pos, Types.FLOAT);
                    } else {
                        stmt.setNull(pos, Types.NULL);
                    }
                }
                pos++;
            }
            stmt.executeUpdate();
        }
        catch (SQLException e) {
            String message= ticket + ": Запис в staging таблица " + table.getTable() +
                    ", externalID = " + row.get("externalID") +
                    ". %s(" + e.getMessage() + ")";
            if (e.getMessage().contains("The INSERT statement conflicted with the FOREIGN KEY constraint ") ||
                    e.getMessage().contains("The INSERT statement conflicted with the REFERENCE constraint ")) {
                message = String.format(message, "Невалидна референция към таблица(номенклатура). ");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new ForegnKeyViolationException(message);
            } else {
                message = String.format(message, "");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException(message);
            }
        }
        return primaryKey;
    }

    @Override
    public List<StagingTableData> getStagingTableData(String tableName, DBMapping table,
                                                      TicketData ticket, String parentKey,
                                                      Connection connection) {
        List<StagingTableData> tableDataList = new ArrayList<>();
        try {
            TableMetadata metadata = databaseConfig.stagingSchemaMetadata().get(tableName);

            String columns = "OperationType, ExternalID, ExternalParentID";
            if (metadata.getColumns().containsKey("ExternalParentID2")) {
                columns += ", ExternalParentID2";
            }
            for (DBMapping.ColumnMapping column: table.getColumnMapping()) {
                columns += ", " + column.getFromColumn();
            }
            String sql = "SELECT " + columns + " " +
                    "FROM [" + databaseConfig.getStagingSchema() + "].[" + tableName +"] " +
                    "WHERE TicketID = ? ";
            if (parentKey != null && !"".equals(parentKey)) {
                sql += " AND ExternalParentID = ? ";
            }
            if (table.getWhere() != null && !"".equals(table.getWhere())) {
                sql += " AND " + table.getWhere();
            }

            PreparedStatement checkStmt = connection.prepareStatement(sql);
            checkStmt.setString(1, ticket.getTicketId());
            if (parentKey != null && !"".equals(parentKey)) {
                checkStmt.setString(2, parentKey);
            }
            ResultSet checkResultSet = checkStmt.executeQuery();
            while (checkResultSet.next()) {
                StagingTableData tableData = new StagingTableData(tableName, null, null, new HashMap<>());
                tableData.setTableName(tableName);
                tableData.setOperationType((Short) checkResultSet.getObject("OperationType"));
                String externalId = (String) checkResultSet.getObject("ExternalID");
                String externalParentId = (String) checkResultSet.getObject("ExternalParentID");
                String externalParentId2 = null;
                if (metadata.getColumns().containsKey("ExternalParentID2")) {
                    externalParentId2 = (String) checkResultSet.getObject("ExternalParentID2");
                }

                Object rowValue = checkResultSet.getObject(table.getPrimaryKey());
                Long recordId = rowValue != null ? Long.valueOf(rowValue.toString()) : null;
                tableData.setTableKey(new StagingTableKey(externalId, externalParentId, externalParentId2, recordId, null, null));
                for (DBMapping.ColumnMapping column: table.getColumnMapping()) {
                    Object columnValue = checkResultSet.getObject(column.getFromColumn());
                    tableData.getColumns().put(column.getFromColumn(), columnValue);
                }
                tableDataList.add(tableData);
            }
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Четене от staging таблица " + tableName +
                    ", ExternalParentID = " + parentKey +
                    ". (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException(message);
        }
        return tableDataList;
    }

    @Override
    public Long insertStagingIntoSchema(StagingTableData stagingData, String tableName,
                                            DBMapping table, Long parentKey, Long parentParentKey,
                                            DBLinkMapping[] linkMapping, Map<String, Map<String, Long>> tableIdentifiers,
                                            TicketData ticket, Connection connection) {
        if (table == null || table.getColumnMapping() == null) {
            return null;
        }

        try {
            TableMetadata metadata = databaseConfig.institutionSchemaMetadata().get(tableName);

            String sqlColumns = "";
            String sqlValues = "";
            if (table.getForeignKey() != null && !"".equals(table.getForeignKey())) {
                sqlColumns += ", " + table.getForeignKey();
                sqlValues += ", " + parentKey;
            }
            if (table.getParentForeignKey() != null && !"".equals(table.getParentForeignKey())) {
                sqlColumns += ", " + table.getParentForeignKey();
                sqlValues += ", " + parentParentKey;
            }
            if (stagingData.getTableKey().getExternalParentId2() != null &&
                    !"".equals(stagingData.getTableKey().getExternalParentId2()) &&
                    linkMapping != null) {
                for (DBLinkMapping dbLinkMapping : linkMapping)
                    if (dbLinkMapping.getFromTable() != null &&
                            dbLinkMapping.getFromTable().equals(tableName)) {
                        Map<String, Long> identifiers = tableIdentifiers.get(dbLinkMapping.getToTable());
                        String externalParentID2 = stagingData.getTableKey().getExternalParentId2();
                        if (identifiers != null && identifiers.containsKey(externalParentID2)) {
                            sqlColumns += ", " + dbLinkMapping.getFromColumn();
                            sqlValues += ", " + identifiers.get(externalParentID2);
                        }
                    }
            }
            Object sqlPK = "Output Inserted." + table.getPrimaryKey();
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (!column.getToColumn().equals(table.getPrimaryKey()) &&
                        !column.getToColumn().equals(table.getForeignKey())) {
                    sqlColumns += ", " + column.getToColumn();
                    if ("uniqueidentifier".equals(metadata.getColumns().get(column.getToColumn()))) {
                        sqlValues += ", CAST(? AS UNIQUEIDENTIFIER)";
                    } else {
                        sqlValues += ", ?";
                    }
                }
            }
            if (metadata.getColumns().containsKey("SysUserID")) {
                sqlColumns += ", SysUserID";
                sqlValues += ", " + ticket.getSysUserId();
            }

            String sql = "INSERT INTO [" + table.getSchema() + "].[" + tableName + "] (" +
                    sqlColumns.substring(2) + ") " + sqlPK + " VALUES (" + sqlValues.substring(2) + ")";

            PreparedStatement stmt = connection.prepareStatement(sql);
            int pos = 1;
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey()) ||
                        column.getToColumn().equals(table.getForeignKey())) {
                    continue;
                }
                String typeName = metadata.getColumns().get(column.getToColumn());
                if (typeName == null) {
                    String message = ticket.getTicketId() + ": Липсва колона " + column.getToColumn() +
                            " в таблица " + table.getSchema() + "." + tableName;
                    log.error(message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                    throw new BadRequestException(message);
                }
                Object rowValue = stagingData.getColumns().get(column.getFromColumn());
                if (rowValue != null && !"".equals(rowValue)) {
                    if ("uniqueidentifier".equals(typeName)) {
                        stmt.setDate(pos, toSqlDate((String) rowValue));
                    } else if ("date".equals(typeName) ||
                            "datetime".equals(typeName) || "datetime2".equals(typeName)) {
                        if (rowValue instanceof java.sql.Timestamp) {
                            java.sql.Timestamp rowTimestamp= (java.sql.Timestamp) rowValue;
                            stmt.setDate(pos, new java.sql.Date(rowTimestamp.getTime()));
                        } else {
                            stmt.setDate(pos, (java.sql.Date) rowValue);
                        }
                    } else if ("bit".equals(typeName) || "smallint".equals(typeName) ||
                            "int".equals(typeName) || "float".equals(typeName) ||
                            "decimal".equals(typeName) || "real".equals(typeName)) {
                        if (rowValue instanceof String) {
                            stmt.setBigDecimal(pos, new BigDecimal((String) rowValue));
                        } else if (rowValue instanceof Long) {
                            stmt.setBigDecimal(pos, new BigDecimal((Long) rowValue));
                        } else if (rowValue instanceof Integer) {
                            stmt.setBigDecimal(pos, new BigDecimal((Integer) rowValue));
                        } else if (rowValue instanceof Short) {
                            stmt.setBigDecimal(pos, new BigDecimal((Short) rowValue));
                        } else if (rowValue instanceof Double) {
                            stmt.setBigDecimal(pos, BigDecimal.valueOf((Double) rowValue));
                        } else if (rowValue instanceof Float) {
                            stmt.setBigDecimal(pos, BigDecimal.valueOf((Float) rowValue));
                        } else if (rowValue instanceof Boolean) {
                            stmt.setInt(pos, ((Boolean) rowValue ? 1 : 0));
                        } else {
                            stmt.setBigDecimal(pos, (BigDecimal) rowValue);
                        }
                    } else if ("nvarchar".equals(typeName)) {
                        if (rowValue instanceof String) {
                            stmt.setString(pos, (String) rowValue);
                        } else {
                            stmt.setString(pos, rowValue.toString());
                        }
                    } else {
                        stmt.setString(pos, rowValue.toString());
                    }
                } else {
                    if ("date".equals(typeName) ||
                            "datetime".equals(typeName) || "datetime2".equals(typeName)) {
                        stmt.setNull(pos, Types.DATE);
                    } else if ("real".equals(typeName)) {
                        stmt.setNull(pos, Types.REAL);
                    } else if ("float".equals(typeName)) {
                        stmt.setNull(pos, Types.FLOAT);
                    } else {
                        stmt.setNull(pos, Types.NULL);
                    }
                }
                pos++;
            }
            ResultSet insertResultSet = stmt.executeQuery();
            if (insertResultSet.next()) {
                Long insertedPrimaryKey = insertResultSet.getLong(1);
                String updateSql = "UPDATE [" + databaseConfig.getStagingSchema() + "].[" + tableName + "] " +
                        "   SET " + table.getPrimaryKey() + " = ? " +
                        " WHERE TicketID = ? " +
                        "   AND ExternalID = ? ";
                PreparedStatement updateStmt = connection.prepareStatement(updateSql);
                updateStmt.setLong(1, insertedPrimaryKey);
                updateStmt.setString(2, ticket.getTicketId());
                updateStmt.setString(3, stagingData.getTableKey().getExternalId());
                updateStmt.executeUpdate();
                return insertedPrimaryKey;
            }
        }
        catch (SQLException e) {
            String message = ticket.getTicketId() + ": Запис в таблица " + table.getSchema() + "." + tableName +
                    ", ExternalID = " + stagingData.getTableKey().getExternalId() +
                    ". %s(" + e.getMessage() + ")";
            if (e.getMessage().contains("The INSERT statement conflicted with the FOREIGN KEY constraint ") ||
                    e.getMessage().contains("The INSERT statement conflicted with the REFERENCE constraint ")) {
                message = String.format(message, "Невалидна референция към master обект. ");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new ForegnKeyViolationException(message);
            } else {
                message = String.format(message, "");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException(message);
            }
        }
        return null;
    }

    @Override
    public void updateStagingIntoSchema(StagingTableData stagingData, String tableName,
                                        DBMapping table, TicketData ticket,
                                        Connection connection) {
        if (table == null || table.getColumnMapping() == null) {
            return;
        }

        try {
            TableMetadata metadata = databaseConfig.institutionSchemaMetadata().get(tableName);

            String sqlSet = "SET ";
            String sqlFilter = "";
            boolean validUpdate = false;
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    sqlFilter = column.getToColumn() + " = ?";
                } else if (!column.getToColumn().equals(table.getForeignKey())) {
                    sqlSet += column.getToColumn() + " = ";
                    if ("uniqueidentifier".equals(metadata.getColumns().get(column.getToColumn()))) {
                        sqlSet += "CAST(? AS UNIQUEIDENTIFIER), ";
                    } else {
                        sqlSet += "?, ";
                    }
                    validUpdate = true;
                }
            }
            if (metadata.getColumns().containsKey("SysUserID")) {
                sqlSet += "SysUserID = " + ticket.getSysUserId() + ", ";
            }

            if (!validUpdate) {
                return;
            }

            String sql = "UPDATE [" + table.getSchema() + "].[" + tableName + "] " +
                         sqlSet.substring(0, sqlSet.length() - 2) +
                         " WHERE " + sqlFilter;

            PreparedStatement stmt = connection.prepareStatement(sql);
            int count = setSqlDataParameters(stmt, metadata, tableName, table, stagingData);

            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    Object rowValue = stagingData.getColumns().get(column.getFromColumn());
                    if (rowValue == null) {
                        String message = ticket.getTicketId() +
                                ": Липсва стойност за PK при операция UPDATE за таблица " +
                                table.getSchema() + "." + tableName;
                        log.error(message);
                        dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                        throw new BadRequestException(message);
                    }
                    stmt.setBigDecimal(count, new BigDecimal(rowValue.toString()));
                    break;
                }
            }

            stmt.executeUpdate();
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Запис в таблица " + table.getSchema() + "." + tableName +
                    ", ExternalID = " + stagingData.getColumns().get("ExternalID") +
                    ". %s(" + e.getMessage() + ")";
            if (e.getMessage().contains("The UPDATE statement conflicted with the FOREIGN KEY constraint ") ||
                    e.getMessage().contains("The UPDATE statement conflicted with the REFERENCE constraint ")) {
                message = String.format(message, "Невалидна референция към master обект. ");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new ForegnKeyViolationException(message);
            } else {
                message = String.format(message, "");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException(message);
            }
        }
    }

    @Override
    public void updateStagingExternalIdIntoSchema(StagingTableData stagingData, String tableName,
                                        DBMapping table, TicketData ticket,
                                        Connection connection) {
        if (table == null || table.getColumnMapping() == null) {
            return;
        }

        if (!stagingData.getColumns().containsKey("ExternalID")) {
            return;
        }

        try {
            String sqlFilter = "";
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    sqlFilter = column.getToColumn() + " = ?";
                    break;
                }
            }

            String sql = "UPDATE [" + table.getSchema() + "].[" + tableName + "] " +
                    "   SET ExternalID = ? " +
                    " WHERE " + sqlFilter;

            PreparedStatement stmt = connection.prepareStatement(sql);
            stmt.setString(1, (String) stagingData.getColumns().get("ExternalID"));

            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    Object rowValue = stagingData.getColumns().get(column.getFromColumn());
                    if (rowValue == null) {
                        String message = ticket.getTicketId() +
                                ": Липсва стойност за PK при операция UPDATE на ExternalID за таблица " +
                                table.getSchema() + "." + tableName;
                        log.error(message);
                        dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                        throw new BadRequestException(message);
                    }
                    stmt.setBigDecimal(2, new BigDecimal(rowValue.toString()));
                    break;
                }
            }

            stmt.executeUpdate();
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Запис на ExternalID в таблица " + table.getSchema() + "." + tableName +
                    ", ExternalID = " + stagingData.getColumns().get("ExternalID") +
                    ". (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void deleteSchema(StagingTableData stagingData, String tableName, DBMapping table,
                             TicketData ticket, Connection connection) {
        if (table == null || table.getColumnMapping() == null) {
            return;
        }

        try {
            TableMetadata metadata = databaseConfig.institutionSchemaMetadata().get(tableName);

            String sqlFilter = "";
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    sqlFilter = column.getToColumn() + " = ?";
                    break;
                }
            }

            String sql = "DELETE FROM [" + table.getSchema() + "].[" + tableName + "] " +
                         " WHERE " + sqlFilter;

            PreparedStatement stmt = connection.prepareStatement(sql);
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey())) {
                    Object rowValue = stagingData.getColumns().get(column.getFromColumn());
                    if (rowValue == null) {
                        String message = ticket.getTicketId() +
                                ": Липсва стойност за PK при операция DELETE за таблица " +
                                table.getSchema() + "." + tableName;
                        log.error(message);
                        dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                        throw new BadRequestException(message);
                    }
                    stmt.setBigDecimal(1, new BigDecimal(rowValue.toString()));
                    break;
                }
            }

            stmt.executeUpdate();
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Изтриване от таблица " + table.getSchema() + "." + tableName +
                    ", ExternalID = " + stagingData.getColumns().get("ExternalID") +
                    ". %s(" + e.getMessage() + ")";
            if (e.getMessage().contains("The DELETE statement conflicted with the FOREIGN KEY constraint ") ||
                    e.getMessage().contains("The DELETE statement conflicted with the REFERENCE constraint ")) {
                message = String.format(message, "Невалидна референция към master обект. ");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new ForegnKeyViolationException(message);
            } else {
                message = String.format(message, "Невалидна референция към master обект. ");
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException(message);
            }
        }
    }

    @Override
    public List<SchemaTableData> getSchemaTableData(ExportMapping mapping, Long parentKey,
                                                    TicketData ticket, Connection connection) {
        if (mapping == null || mapping.getTable() == null || mapping.getColumnMapping() == null) {
            return new ArrayList<>();
        }
        List<SchemaTableData> tableDataList = new ArrayList<>();
        try {
            TableMetadata metadata = databaseConfig.institutionSchemaMetadata().get(mapping.getTable());

            String sqlFilter = "WHERE 1=1 ";
            //if (mainKey != null && mapping.getPrimaryKey() != null && !mapping.getPrimaryKey().isEmpty()) {
            //    sqlFilter += " AND " + mapping.getPrimaryKey() + " = " + mainKey;
            //}
            if (parentKey != null) {
                sqlFilter += " AND " + mapping.getForeignKey() + " = " + parentKey;
            }
            if (mapping.getFilter() != null && !mapping.getFilter().isEmpty()) {
                String filter = mapping.getFilter();
                if (ticket.getInstitutionId() != null && mapping.getFilter().contains("@InstitutionID")) {
                    filter = filter.replaceAll("@InstitutionID", ticket.getInstitutionId().toString());
                }
                if (ticket.getInstitutionId() != null && mapping.getFilter().contains("@SchoolYear")) {
                    filter = filter.replaceAll("@SchoolYear", ticket.getSchoolYear().toString());
                }
                sqlFilter += " AND " + filter;
            }
            String sqlSelect = " SELECT ";
            for (ExportMapping.ColumnMapping column : mapping.getColumnMapping()) {
                if (!column.getFromColumn().isEmpty()) {
                    sqlSelect += column.getFromColumn() + ", ";
                }
            }

            String sql = sqlSelect.substring(0, sqlSelect.length() - 2) +
                    " FROM [" + mapping.getSchema() + "].[" + mapping.getTable() + "] " +
                    sqlFilter;

            //System.out.println("SQL = " + sql);

            PreparedStatement stmt = connection.prepareStatement(sql);
            ResultSet resultSet = stmt.executeQuery();
            while (resultSet.next()) {
                SchemaTableData tableData = new SchemaTableData(mapping.getSchema(), mapping.getTable(),
                        null, new LinkedHashMap<>(), new LinkedHashMap<>());
                String keyName = mapping.getPrimaryKey();
                Long keyValue = resultSet.getLong(mapping.getPrimaryKey());
                tableData.setTableKey(new SchemaTableKey(keyName, keyValue));
                for (ExportMapping.ColumnMapping column : mapping.getColumnMapping()) {
                    Object columnValue = resultSet.getObject(column.getFromColumn());
                    tableData.getColumns().put(column.getFromColumn(), columnValue);
                    for (ExportMapping.ColumnMapping attribute : mapping.getColumnMapping()) {
                        if (attribute.getFromColumn().equals(column.getFromColumn())) {
                            tableData.getAttributes().put(attribute.getToAttribute(), columnValue);
                            break;
                        }
                    }
                }
                tableDataList.add(tableData);
            }
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Четене от таблица " + mapping.getSchema() + "." + mapping.getTable() +
                    ". (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(message);
        }
        return tableDataList;
    }

    @Override
    public void incrementInstitutionVersionData(TicketData ticket, Connection connection) {
        try {
            String updateSql = "UPDATE core.InstitutionConfData " +
                    "   SET SOVersion = SOVersion + 1, " +
                    "       CBVersion = CBVersion + 1 " +
                    " WHERE InstitutionID = ?";
            PreparedStatement updateStmt = connection.prepareStatement(updateSql);
            updateStmt.setLong(1, ticket.getInstitutionId());
            updateStmt.executeUpdate();
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Промяна на версия на данните. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    private int setSqlDataParameters(PreparedStatement stmt, TableMetadata metadata,
                                     String tableName, DBMapping table,
                                     StagingTableData stagingData) throws SQLException {
        int pos = 1;
        //try {
            for (DBMapping.ColumnMapping column : table.getColumnMapping()) {
                if (column.getToColumn().equals(table.getPrimaryKey()) ||
                        column.getToColumn().equals(table.getForeignKey())) {
                    continue;
                }
                String typeName = metadata.getColumns().get(column.getToColumn());
                if (typeName == null) {
                    throw new BadRequestException("Липсва колона " + column.getToColumn() +
                            " в таблица " + table.getSchema() + "." + tableName);
                }
                Object rowValue = stagingData.getColumns().get(column.getFromColumn());
                if (rowValue != null && !"".equals(rowValue)) {
                    if ("uniqueidentifier".equals(typeName)) {
                        stmt.setDate(pos, toSqlDate((String) rowValue));
                    } else if ("date".equals(typeName) ||
                            "datetime".equals(typeName) || "datetime2".equals(typeName)) {
                        if (rowValue instanceof java.sql.Timestamp) {
                            java.sql.Timestamp rowTimestamp = (java.sql.Timestamp) rowValue;
                            stmt.setDate(pos, new java.sql.Date(rowTimestamp.getTime()));
                        } else {
                            stmt.setDate(pos, (java.sql.Date) rowValue);
                        }
                    } else if ("bit".equals(typeName) || "smallint".equals(typeName) ||
                            "int".equals(typeName) || "float".equals(typeName) ||
                            "decimal".equals(typeName) || "real".equals(typeName)) {
                        if (rowValue instanceof String) {
                            stmt.setBigDecimal(pos, new BigDecimal((String) rowValue));
                        } else if (rowValue instanceof Long) {
                            stmt.setBigDecimal(pos, new BigDecimal((Long) rowValue));
                        } else if (rowValue instanceof Integer) {
                            stmt.setBigDecimal(pos, new BigDecimal((Integer) rowValue));
                        } else if (rowValue instanceof Short) {
                            stmt.setBigDecimal(pos, new BigDecimal((Short) rowValue));
                        } else if (rowValue instanceof Double) {
                            stmt.setBigDecimal(pos, BigDecimal.valueOf((Double) rowValue));
                        } else if (rowValue instanceof Float) {
                            stmt.setBigDecimal(pos, BigDecimal.valueOf((Float) rowValue));
                        } else if (rowValue instanceof Boolean) {
                            stmt.setInt(pos, ((Boolean) rowValue ? 1 : 0));
                        } else {
                            stmt.setBigDecimal(pos, (BigDecimal) rowValue);
                        }
                    } else if ("nvarchar".equals(typeName)) {
                        if (rowValue instanceof String) {
                            stmt.setString(pos, (String) rowValue);
                        } else {
                            stmt.setString(pos, rowValue.toString());
                        }
                    } else {
                        stmt.setString(pos, rowValue.toString());
                    }
                } else {
                    if ("date".equals(typeName) ||
                            "datetime".equals(typeName) || "datetime2".equals(typeName)) {
                        stmt.setNull(pos, Types.DATE);
                    } else if ("real".equals(typeName)) {
                        stmt.setNull(pos, Types.REAL);
                    } else if ("float".equals(typeName)) {
                        stmt.setNull(pos, Types.FLOAT);
                    } else {
                        stmt.setNull(pos, Types.NULL);
                    }
                }
                pos++;
            }
        //} catch (SQLException e) {
        //    log.error(e.getMessage());
        //    throw new InternalServerErrorException(e.getMessage());
        //}
        return pos;
    }

    private java.sql.Date toSqlDate(String date) {

        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
        java.util.Date newDate;

        try {
            newDate = sdf.parse(date);
            if (sdf.format(newDate).equals(date.substring(0, 10))) {
                Calendar calendar = Calendar.getInstance();
                calendar.set(Calendar.DAY_OF_MONTH, Integer.parseInt(date.substring(8, 10)));
                calendar.set(Calendar.MONTH, Integer.parseInt(date.substring(5, 7)) - 1);
                calendar.set(Calendar.YEAR, Integer.parseInt(date.substring(0, 4)));
                calendar.set(Calendar.HOUR_OF_DAY, 0);
                calendar.set(Calendar.MINUTE, 0);
                calendar.set(Calendar.SECOND, 0);
                calendar.set(Calendar.MILLISECOND, 0);
                return new java.sql.Date(calendar.getTimeInMillis());
            } else {
                return null;
            }
        } catch (ParseException e) {
            return null;
        }
    }

}
