package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.MetadataConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.JsonData;
import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.azure.AzureError;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Component;

import java.sql.Timestamp;
import java.util.Calendar;
import java.util.List;

@Component
@Slf4j
@RequiredArgsConstructor
public class DBAuditLogRepositoryImpl implements DBAuditLogRepository {

    private final MetadataConfig metadataConfig;
    private final JdbcTemplate jdbcTemplate;

    @Override
    public void logEvent(TicketData ticket, JsonData jsonData) {
        String insertSql = "INSERT INTO logs.Audit " +
                " (AuditModuleId, SysUserId, SysRoleId, RemoteIpAddress, UserAgent, " +
                "  DateUtc, SchoolYear, InstId, ObjectName, ObjectId, Action, Data) " +
                " VALUES (?, ?, ?, ?, ?, ?, ?, ?, 'Institution', ?, 'UPDATE', ?)";
        try {
            jdbcTemplate.update(insertSql, metadataConfig.getApiModuleId(),
                    ticket.getSysUserId(), metadataConfig.getApiUserRoleId(),
                    ticket.getRemoteIpAddress(), ticket.getUserAgent(),
                    ticket.getCreated(), ticket.getSchoolYear(), ticket.getInstitutionId(),
                    ticket.getInstitutionId(), jsonData.getJsonData());
        } catch (DataAccessException e) {
            String msg = ticket.getTicketId() + ": Запис в таблица logs.Audit. (" + e.getMessage() + ")";
            log.error(msg);
            throw new InternalServerErrorException(msg);
        }
    }

    @Override
    public void logDBError(String message) {
        logDBError(null, ApiOperationEnum.SOB_API.getUserName(),
                ApiOperationEnum.SOB_API.name(), message, null);
    }

    @Override
    public void logDBError(String ticket, ApiOperationEnum operation, String message) {
        logDBError(new TicketData(ticket), operation.getUserName(),
                operation.name(), message, null);
    }

    @Override
    public void logDBError(TicketData ticket, ApiOperationEnum operation, String message) {
        logDBError(ticket, operation.getUserName(), operation.name(), message, "");
    }

    @Override
    public void logAzureDBError(ApiOperationEnum operation, String address, String message, String data) {
        logDBError(null, operation.getUserName(), address, message, data);
    }

    private void logDBError(TicketData ticket, String userName, String operation, String message, String data) {
        String insertSql = "INSERT INTO logs.DB_Errors " +
                " (ErrorDateTime, UserName, ModuleId, ErrorProcedure, ErrorMessage, " +
                "  SchoolYear, InstId, SysUserId, SysRoleId, RemoteIpAddress, UserAgent, Data) " +
                " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        try {
            jdbcTemplate.update(insertSql,
                    new Timestamp(Calendar.getInstance().getTimeInMillis()),
                    (userName != null && !"".equals(userName) ? userName : "SOB API") +
                            (ticket != null && !"".equals(ticket.getTicketId()) ? " " + ticket.getTicketId() : ""),
                    metadataConfig.getApiModuleId(),
                    operation,
                    message,
                    ticket != null ? ticket.getSchoolYear() : null,
                    ticket != null ? ticket.getInstitutionId() : null,
                    ticket != null ? ticket.getSysUserId() : null,
                    metadataConfig.getApiUserRoleId(),
                    ticket != null ? ticket.getRemoteIpAddress() : null,
                    ticket != null ? ticket.getUserAgent() : null,
                    data);
        } catch (DataAccessException e) {
            String msg = ticket + ": Запис в таблица logs.DB_Errors. (" + e.getMessage() + ")";
            log.error(msg);
            // throw new InternalServerErrorException(msg);
        }
    }

    @Override
    public List<AzureError> getAzureErrors(Timestamp date, Long id, String errorMessage) {
        try {
            String curriculumSql = "SELECT TOP 100 ErrorID, ErrorProcedure, Data, ErrorDateTime " +
                    "  FROM logs.DB_Errors " +
                    " WHERE UserName like ? " +
                    "   AND ErrorMessage like ? " +
                    "   AND ErrorDateTime >= ? " +
                    "   AND ErrorID > ? " +
                    " ORDER BY ErrorDateTime";
            return jdbcTemplate.query(
                    curriculumSql,
                    (rs, rowNum) ->
                            new AzureError(
                                    rs.getLong("ErrorID"),
                                    rs.getString("ErrorProcedure"),
                                    rs.getString("Data"),
                                    rs.getTimestamp("ErrorDateTime")),
                    "AzureCall%", "%" + errorMessage + "%", date, id
            );
        } catch (DataAccessException e) {
            String message = "Четене на Azure грешки. (" + e.getMessage() + ")";
            log.error(message);
            logDBError(null, ApiOperationEnum.AZURE_RECALL.getUserName(), "", message, null);
            throw new InternalServerErrorException(message);
        }
    }
}
