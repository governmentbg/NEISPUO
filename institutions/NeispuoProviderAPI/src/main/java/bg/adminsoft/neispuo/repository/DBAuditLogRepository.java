package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.JsonData;
import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.azure.AzureError;

import java.sql.Timestamp;
import java.util.List;

public interface DBAuditLogRepository {

    void logEvent(TicketData ticket, JsonData jsonData);
    void logDBError(String message);
    void logDBError(String ticket, ApiOperationEnum operation, String message);
    void logDBError(TicketData ticket, ApiOperationEnum operation, String message);
    void logAzureDBError(ApiOperationEnum operation, String address, String message, String data);
    List<AzureError> getAzureErrors(Timestamp date, Long id, String errorMessage);
}
