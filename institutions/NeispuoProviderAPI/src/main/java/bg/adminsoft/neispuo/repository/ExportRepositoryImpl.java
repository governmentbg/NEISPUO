package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.*;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.sql.Connection;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Component
@Slf4j
@RequiredArgsConstructor
public class ExportRepositoryImpl implements ExportRepository {

    private final DatabaseConfig databaseConfig;
    private final DBRepository dbRepository;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public String exportInstitution(TicketData ticket, String mapping) {

        ExportDataMapping exportDataMapping = null;
        try {
            exportDataMapping = new ObjectMapper().readValue(mapping, ExportDataMapping.class);
        } catch (Exception e) {
            String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException(e.getMessage());
        }

        Connection connection = databaseConfig.dbConnection();
        try {
            Map<String, Object> data = new HashMap<>();
            exportJson(ticket,null, exportDataMapping.getExportMapping(), data, connection);
            ObjectMapper objectMapper = new ObjectMapper();
            objectMapper.setDateFormat(new SimpleDateFormat("yyyy-MM-dd"));
            return objectMapper.writeValueAsString(data);
        } catch (Exception e) {
            //log.error(e.getMessage());
            //dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        } finally {
            databaseConfig.closeConnection(connection);
        }
    }

    private void exportJson(TicketData ticket, Long parentKey, Map<String,
            ExportMapping> exportMapping, Map<String, Object> data, Connection connection) {
        for (Map.Entry<String, ExportMapping> mapping : exportMapping.entrySet()) {
            List<SchemaTableData> tableData =
                    dbRepository.getSchemaTableData(mapping.getValue(), parentKey, ticket, connection);
            data.put(mapping.getKey(), new ArrayList<>());
            for (SchemaTableData item : tableData) {
                ((List) data.get(mapping.getKey())).add(item.getAttributes());
                if (mapping.getValue().getChildren() != null && !mapping.getValue().getChildren().isEmpty()) {
                    exportJson(ticket, item.getTableKey().getValue(),
                            mapping.getValue().getChildren(), item.getAttributes(), connection);
                }
            }
        }
    }

}
