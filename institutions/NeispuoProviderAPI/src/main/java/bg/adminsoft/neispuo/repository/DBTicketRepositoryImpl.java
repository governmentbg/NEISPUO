package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.exception.BadRequestException;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.exception.InvalidStatusException;
import bg.adminsoft.neispuo.model.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataAccessException;
import org.springframework.dao.EmptyResultDataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Component;

import java.sql.*;
import java.util.*;

@Component
@Slf4j
@RequiredArgsConstructor
public class DBTicketRepositoryImpl implements DBTicketRepository {

    private final DatabaseConfig databaseConfig;
    private final JdbcTemplate jdbcTemplate;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public boolean checkInstitutionCurrentYear(TicketData ticket) {
        String checkSql = "SELECT SchoolYear " +
                " FROM core.InstitutionConfData " +
                "WHERE InstitutionID = ? ";

        try {
            List<Integer> years = jdbcTemplate.query(
                    checkSql,
                    (rs, rowNum) -> rs.getInt("SchoolYear"),
                    ticket.getInstitutionId()
            );
            return years.size() > 0 && years.get(0).equals(ticket.getSchoolYear());
        } catch (DataAccessException e) {
            String message = "Проверка за текуща година на институция. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public boolean checkListTemplatePeriod(TicketData ticket) {
        String checkSql = "SELECT IsLocked " +
                " FROM inst_nom.InstIsLocked " +
                "WHERE InstitutionID = ? ";

        try {
            List<Integer> locks = jdbcTemplate.query(
                    checkSql,
                    (rs, rowNum) -> rs.getInt("IsLocked"),
                    ticket.getInstitutionId()
            );
            return locks.size() > 0 && locks.get(0) == 1;
        } catch (DataAccessException e) {
            String message = "Проверка за валиден период на институция. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public boolean checkNewYearTransitionPeriod(TicketData ticket) {
        String checkSql = "SELECT ch.ChangeYearStatusID " +
                " FROM inst_year.ChangeYearData ch, " +
                "      core.InstitutionConfData cnf " +
                "WHERE ch.ToSchoolYear = cnf.SchoolYear " +
                "  AND ch.InstitutionID = cnf.InstitutionID " +
                "  AND cnf.InstitutionID = ? ";

        try {
            List<Integer> changeYearStatusIDs = jdbcTemplate.query(
                    checkSql,
                    (rs, rowNum) -> rs.getInt("ChangeYearStatusID"),
                    ticket.getInstitutionId()
            );
            return changeYearStatusIDs.size() > 0 && changeYearStatusIDs.get(0) != 2;
        } catch (DataAccessException e) {
            String message = "Проверка за валиден период на институция. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public String createTicket(Long institutionID, Integer schoolYear, ProviderInfo provider,
                               String data, TicketTypeEnum ticketType, TicketSubTypeEnum ticketSubType,
                               String remoteIpAddress, String userAgent) {
        Connection connection = null;
        String ticket = null;
        try {
            connection = databaseConfig.dbConnection();
            String checkSql = "SELECT TicketID, StatusID FROM " +
                    databaseConfig.getStagingSchema() + ".Tickets " +
                    "WHERE InstitutionID = ? " +
                    "  AND Type = ? " +
                    // "  AND SubType = ? " +
                    "  AND StatusID IN (0, 1)";
            PreparedStatement checkStmt = connection.prepareStatement(checkSql);
            checkStmt.setLong(1, institutionID);
            checkStmt.setString(2, ticketType.name());
            // checkStmt.setString(3, ticketSubType.name());
            ResultSet checkResultSet = checkStmt.executeQuery();
            if (checkResultSet.next()) {
                String existingTicket = checkResultSet.getString(1);
                int status = checkResultSet.getInt(2);
                switch (status) {
                    case 0:
                        throw new InvalidStatusException("Ticket in status PENDING exists: " + existingTicket);
                    case 1:
                        throw new InvalidStatusException("Ticket in status IN PROGRESS exists: " + existingTicket);
                }
            }

            String insertSql = "INSERT INTO " + databaseConfig.getStagingSchema() + ".Tickets " +
                    "(TicketID, InstitutionID, SchoolYear, StatusID, " +
                    " FiledTS, FiledJSON, ProviderID, LastUpdatedStatusTS, Type, SubType, " +
                    " SysUserID, RemoteIpAddress, UserAgent) " +
                    "Output Inserted.TicketID " +
                    "VALUES (NEWID(), ?, ?, 0, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            Timestamp time = new Timestamp(Calendar.getInstance().getTimeInMillis());
            PreparedStatement insertStmt = connection.prepareStatement(insertSql);
            insertStmt.setLong(1, institutionID);
            insertStmt.setInt(2, schoolYear);
            insertStmt.setTimestamp(3, time);
            insertStmt.setString(4, data);
            insertStmt.setInt(5, provider.getProviderId());
            insertStmt.setTimestamp(6, time);
            insertStmt.setString(7, ticketType.name());
            insertStmt.setString(8, ticketSubType.name());
            insertStmt.setInt(9, provider.getSysUserId());
            insertStmt.setString(10, remoteIpAddress);
            insertStmt.setString(11, userAgent);
            ResultSet insertResultSet = insertStmt.executeQuery();
            if (insertResultSet.next()) {
                ticket = insertResultSet.getString(1);
            }
            connection.commit();
        } catch (SQLException e) {
            try { connection.rollback(); } catch (SQLException ignored) { }

            String message = "Създаване на тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(new TicketData("", institutionID, schoolYear,
                            remoteIpAddress, userAgent),
                    ticketType == TicketTypeEnum.IMPORT ? ApiOperationEnum.IMPORT :
                            ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(message);
        } finally {
            databaseConfig.closeConnection(connection);
        }
        return ticket;
    }

    @Override
    public TicketStatusResponse getTicketStatus(String ticketId, boolean withDetails,
                                                TicketTypeEnum ticketType) {
        try {
            String checkSql = "SELECT TOP 1 StatusID FROM " +
                    databaseConfig.getStagingSchema() + ".Tickets " +
                    "WHERE TicketID = ?" +
                    "  AND Type = ?";
            Integer status = jdbcTemplate.queryForObject(
                    checkSql,
                    (rs, rowNum) -> rs.getInt("StatusID"),
                    ticketId, ticketType.name()
            );
            TicketStatusEnum statusEnum = TicketStatusEnum.getByStatus(status == null ? 0 : status);

            // todo optimize
            Map<ValidationTypeEnum, List<String>> details = null;
            if (withDetails && statusEnum != TicketStatusEnum.PENDING
                    && statusEnum != TicketStatusEnum.IN_PROGRESS) {
                details = new HashMap<>();
                details.put(ValidationTypeEnum.ERROR, new ArrayList<>());
                details.put(ValidationTypeEnum.WARNING, new ArrayList<>());
                details.put(ValidationTypeEnum.INFO, new ArrayList<>());

                String integritySql = "SELECT ResultType, Message FROM " +
                        databaseConfig.getStagingSchema() + ".IntegrityCheckResults " +
                        "WHERE TicketID = ? AND ResultType = ?";
                List<String> errors = jdbcTemplate.query(
                        integritySql,
                        (rs, rowNum) -> rs.getString("Message"),
                        ticketId, ValidationTypeEnum.ERROR.name()
                );
                details.get(ValidationTypeEnum.ERROR).addAll(errors);
                List<String> warnings = jdbcTemplate.query(
                        integritySql,
                        (rs, rowNum) -> rs.getString("Message"),
                        ticketId, ValidationTypeEnum.WARNING.name()
                );
                details.get(ValidationTypeEnum.WARNING).addAll(warnings);

                String validitySql = "SELECT v.ValidityCheckType, r.Message FROM " +
                        databaseConfig.getStagingSchema() + ".ValidityCheckResults r, " +
                        "      sovalidity.ValidityCheckDetails d, " +
                        "      sovalidity.ValidityCheck v " +
                        "WHERE v.ValidityCheckID = d.ValidityCheckID " +
                        "  AND d.ValidityCheckDetailID = r.ValidityCheckDetailID " +
                        "  AND r.TicketID = ? AND v.ValidityCheckType = ?";
                List<String> validityErrors = jdbcTemplate.query(
                        validitySql,
                        (rs, rowNum) -> rs.getString("Message"),
                        ticketId, ValidationTypeEnum.ERROR.getValue()
                );
                details.get(ValidationTypeEnum.ERROR).addAll(validityErrors);
                List<String> validityWarnings = jdbcTemplate.query(
                        validitySql,
                        (rs, rowNum) -> rs.getString("Message"),
                        ticketId, ValidationTypeEnum.WARNING.getValue()
                );
                details.get(ValidationTypeEnum.WARNING).addAll(validityWarnings);
                List<String> validityInfos = jdbcTemplate.query(
                        validitySql,
                        (rs, rowNum) -> rs.getString("Message"),
                        ticketId, ValidationTypeEnum.INFO.getValue()
                );
                details.get(ValidationTypeEnum.INFO).addAll(validityInfos);
            }

            return new TicketStatusResponse(statusEnum, statusEnum.getDescription(), details);

        } catch (DataAccessException e) {
            String message = ticketId + ": Четене на статус на тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticketId,
                    ticketType == TicketTypeEnum.IMPORT ? ApiOperationEnum.IMPORT :
                            ApiOperationEnum.EXPORT, message);
            if (e instanceof EmptyResultDataAccessException) {
                throw new BadRequestException("Несъществуващ тикет.");
            } else {
                throw new InternalServerErrorException(message);
            }
        }
    }

    @Override
    public TicketData getTicketData(String ticketId, TicketTypeEnum ticketType) {
        String checkSql = "SELECT InstitutionID, SchoolYear, ProviderID, StatusID, SubType, " +
                " SysUserID, RemoteIpAddress, UserAgent, FiledTS " +
                " FROM " + databaseConfig.getStagingSchema() + ".Tickets " +
                "WHERE TicketID = ? " +
                "  AND Type = ?";
        try {
            return jdbcTemplate.queryForObject(
                    checkSql,
                    (rs, rowNum) -> new TicketData (ticketId,
                            rs.getLong("InstitutionID"),
                            rs.getInt("SchoolYear"),
                            rs.getInt("ProviderID"),
                            TicketStatusEnum.getByStatus(rs.getInt("StatusID")),
                            ticketType,
                            rs.getString("SubType") != null ?
                                    TicketSubTypeEnum.valueOf(rs.getString("SubType")) : null,
                            rs.getInt("SysUserID"),
                            rs.getString("RemoteIpAddress"),
                            rs.getString("UserAgent"),
                            rs.getTimestamp("FiledTS")),
                    ticketId, ticketType.name()
            );
        } catch (DataAccessException e) {
            String message = ticketId + ": Четене на тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticketId,
                    ticketType == TicketTypeEnum.IMPORT ? ApiOperationEnum.IMPORT :
                            ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public TicketData getTicketByStatus(TicketStatusEnum status, TicketTypeEnum ticketType) {
        String ticketsSql = "SELECT TOP 1 TicketID, InstitutionID, SchoolYear, ProviderID, " +
                " StatusID, SubType, SysUserID, RemoteIpAddress, UserAgent, FiledTS " +
                " FROM " + databaseConfig.getStagingSchema() + ".Tickets " +
                "WHERE StatusID = ? " +
                // todo ????
                "  AND Type = ? " +
                " ORDER BY FiledTS";
        try {
            return jdbcTemplate.queryForObject(
                    ticketsSql,
                    (rs, rowNum) -> new TicketData (rs.getString("TicketID"),
                            rs.getLong("InstitutionID"),
                            rs.getInt("SchoolYear"),
                            rs.getInt("ProviderID"),
                            TicketStatusEnum.getByStatus(rs.getInt("StatusID")),
                            ticketType,
                            rs.getString("SubType") != null ?
                                    TicketSubTypeEnum.valueOf(rs.getString("SubType")) : null,
                            rs.getInt("SysUserID"),
                            rs.getString("RemoteIpAddress"),
                            rs.getString("UserAgent"),
                            rs.getTimestamp("FiledTS")),
                    // todo ????
                    status.getStatus(), ticketType.name()
            );
        } catch (EmptyResultDataAccessException e) {
            return new TicketData();
        } catch (DataAccessException e) {
            String message = "Търсене на тикет по статус. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("",
                    ticketType == TicketTypeEnum.IMPORT ? ApiOperationEnum.IMPORT :
                            ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public TicketData getTicketForAzureSync() {
        String ticketsSql = "SELECT TOP 1 TicketID, InstitutionID, SchoolYear, ProviderID, " +
                " StatusID, Type, SubType, SysUserID, RemoteIpAddress, UserAgent, FiledTS " +
                " FROM " + databaseConfig.getStagingSchema() + ".Tickets " +
                "WHERE StatusID = ? " +
                "  AND Type = ? " +
                "  AND (AzureSync IS NULL OR AzureSync = ?) " +
                " ORDER BY FiledTS";
        try {
            return jdbcTemplate.queryForObject(
                    ticketsSql,
                    (rs, rowNum) -> new TicketData (rs.getString("TicketID"),
                            rs.getLong("InstitutionID"),
                            rs.getInt("SchoolYear"),
                            rs.getInt("ProviderID"),
                            TicketStatusEnum.getByStatus(rs.getInt("StatusID")),
                            TicketTypeEnum.valueOf(rs.getString("Type")),
                            rs.getString("SubType") != null ?
                                    TicketSubTypeEnum.valueOf(rs.getString("SubType")) : null,
                            rs.getInt("SysUserID"),
                            rs.getString("RemoteIpAddress"),
                            rs.getString("UserAgent"),
                            rs.getTimestamp("FiledTS")),
                    TicketStatusEnum.COMPLETED.getStatus(),
                    TicketTypeEnum.IMPORT.name(),
                    TicketAzureSync.PENDING.getStatus()
            );
        } catch (EmptyResultDataAccessException e) {
            return new TicketData();
        } catch (DataAccessException e) {
            String message = "Търсене на Azure тикет по статус. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void setTicketStatus(TicketData ticket, TicketStatusEnum status, Connection connection) {
        try {
            String updateSql = "UPDATE " + databaseConfig.getStagingSchema() + ".Tickets " +
                    "SET StatusID = ?, LastUpdatedStatusTS = ? " +
                    "WHERE TicketID = ?";
            PreparedStatement updateStmt = connection.prepareStatement(updateSql);
            updateStmt.setInt(1, status.getStatus());
            updateStmt.setTimestamp(2, new java.sql.Timestamp(Calendar.getInstance().getTimeInMillis()));
            updateStmt.setString(3, ticket.getTicketId());
            updateStmt.executeUpdate();
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Промяна на статус на тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void setTicketStatus(TicketData ticket, TicketStatusEnum status) {
        String updateSql = "UPDATE " + databaseConfig.getStagingSchema() + ".Tickets " +
                "SET StatusID = ?, LastUpdatedStatusTS = ? " +
                "WHERE TicketID = ?";
        try {
            jdbcTemplate.update(updateSql, status.getStatus(),
                    new java.sql.Timestamp(Calendar.getInstance().getTimeInMillis()),
                    ticket.getTicketId());
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Промяна на статус на тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void setTicketAzureSync(TicketData ticket, TicketAzureSync status) {
        String updateSql = "UPDATE " + databaseConfig.getStagingSchema() + ".Tickets " +
                "SET AzureSync = ?, LastUpdatedStatusTS = ? " +
                "WHERE TicketID = ?";
        try {
            jdbcTemplate.update(updateSql, status.getStatus(),
                    new java.sql.Timestamp(Calendar.getInstance().getTimeInMillis()),
                    ticket.getTicketId());
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Промяна на Azure статус на тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void setTicketJson(TicketData ticket, String json) {
        String updateSql = "UPDATE " + databaseConfig.getStagingSchema() + ".Tickets " +
                "SET FiledJSON = ? " +
                "WHERE TicketID = ?";
        try {
            jdbcTemplate.update(updateSql, json, ticket.getTicketId());
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Запис на json (експорт) в тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public JsonData getJsonByTicket(TicketData ticket) {
        String checkSql = "SELECT InstitutionID, SchoolYear, FiledJSON " +
                "FROM " + databaseConfig.getStagingSchema() + ".Tickets " +
                "WHERE TicketID = ?";
        try {
            return jdbcTemplate.queryForObject(
                    checkSql,
                    (rs, rowNum) -> new JsonData(new JsonDefaultData(rs.getLong("InstitutionID"),
                            rs.getInt("SchoolYear")),
                            rs.getString("FiledJSON"),
                            null),
                    ticket.getTicketId());
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Четене на json (експорт) от тикет. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void saveIntegrityCheckResult(TicketData ticket, List<ValidationResult> validateResult,
                                         boolean deleteExisting, Connection connection) {
        try {
            if (deleteExisting) {
                String deleteSql = "DELETE FROM " + databaseConfig.getStagingSchema() + ".IntegrityCheckResults " +
                        " WHERE TicketID = ?";
                PreparedStatement deleteStmt = connection.prepareStatement(deleteSql);
                deleteStmt.setString(1, ticket.getTicketId());
                deleteStmt.executeUpdate();
            }

            String insertSql = "INSERT INTO " + databaseConfig.getStagingSchema() + ".IntegrityCheckResults " +
                    "(TicketID, Message, ResultType) " +
                    "VALUES (?, ?, ?)";
            PreparedStatement insertStmt = connection.prepareStatement(insertSql);
            for (ValidationResult result : validateResult) {
                insertStmt.setString(1, ticket.getTicketId());
                insertStmt.setString(2, result.getMessage());
                insertStmt.setString(3, result.getType().name());
                insertStmt.executeUpdate();
            }
        } catch (SQLException e) {
            String message = ticket.getTicketId() + ": Запис на резултати от валидация. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public ProviderInfo checkAccess(String certificateThumbprint, TicketData ticket) {
        String checkSql = "SELECT c.ExtSystemID, s.SysUserID " +
                    " FROM core.ExtSystemAccess a, " +
                    "      core.ExtSystemCertificate c, " +
                    "      core.ExtSystem s, " +
                    "      core.InstitutionConfData i " +
                    " WHERE a.ExtSystemID = c.ExtSystemID " +
                    "   AND c.ExtSystemID = s.ExtSystemID " +
                    "   AND i.SOExtProviderID = c.ExtSystemID " +
                    "   AND c.IsValid = 1" +
                    "   AND a.ExtSystemType = 2 " +
                    "   AND c.Thumbprint = ?" +
                    "   AND i.InstitutionID = ?";
        try {
            List<ProviderInfo> providerInfos = jdbcTemplate.query(
                    checkSql,
                    (rs, rowNum) -> new ProviderInfo (rs.getInt("ExtSystemID"),
                            rs.getInt("SysUserID")),
                    certificateThumbprint, ticket.getInstitutionId()
            );
            if (providerInfos.size() > 0) {
                return providerInfos.get(0);
            } else {
                return null;
            }
        } catch (DataAccessException e) {
            String message = "Проверка на сертификат за достъп. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, null, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public void stopProcessingTickets() {
        // todo ????
        String updateSql = "UPDATE " + databaseConfig.getStagingSchema() + ".Tickets " +
                "SET StatusID = ?, LastUpdatedStatusTS = ? " +
                "WHERE StatusID = ?";
        try {
            jdbcTemplate.update(updateSql, TicketStatusEnum.STOPPED.getStatus(),
                    new java.sql.Timestamp(Calendar.getInstance().getTimeInMillis()),
                    TicketStatusEnum.IN_PROGRESS.getStatus());
        } catch (DataAccessException e) {
            String message = "Изчистване на статуса на незавършили тикети при рестарт. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(message);
            throw new InternalServerErrorException(message);
        }
    }

}
