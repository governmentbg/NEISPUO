package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.config.TableMetadata;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.ExportDataMapping;
import bg.adminsoft.neispuo.model.ExportMapping;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.text.SimpleDateFormat;
import java.util.LinkedHashMap;
import java.util.Map;

@Component
@Slf4j
@RequiredArgsConstructor
public class DocumentationRepositoryImpl implements DocumentationRepository {

    private final DatabaseConfig databaseConfig;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public String exportDocument(String mapping) {

        ExportDataMapping exportDataMapping;
        try {
            exportDataMapping = new ObjectMapper().readValue(mapping, ExportDataMapping.class);
        } catch (Exception e) {
            log.error(e.getMessage());
            dbAuditLogRepository.logDBError("", ApiOperationEnum.DOCUMENTATION, e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }

        try {
            Map<String, Object> document = new LinkedHashMap<>();
            exportJsonDocument(exportDataMapping.getExportMapping(), document);
            ObjectMapper objectMapper = new ObjectMapper();
            objectMapper.setDateFormat(new SimpleDateFormat("yyyy-MM-dd"));
            return objectMapper.writeValueAsString(document);
        } catch (Exception e) {
            log.error(e.getMessage());
            dbAuditLogRepository.logDBError("", ApiOperationEnum.DOCUMENTATION, e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }
    }

    private void exportJsonDocument(Map<String, ExportMapping> exportMapping,
                                    Map<String, Object> data) {
        for (Map.Entry<String, ExportMapping> mapping : exportMapping.entrySet()) {

            TableMetadata metadata = databaseConfig.institutionSchemaMetadata().get(mapping.getValue().getTable());
            if (metadata != null) {
                Map<String, Object> definitions = new LinkedHashMap<>();
                definitions.put("description", metadata.getDescription());
                for (ExportMapping.ColumnMapping columnMapping : mapping.getValue().getColumnMapping()) {
                    definitions.put(columnMapping.getToAttribute(),
                            metadata.getColumnsDocumentation().get(columnMapping.getFromColumn()));
                }
                data.put(mapping.getKey(), definitions);
            } else {
                String message = "Таблицата не е конфигурирана: " + mapping.getValue().getTable();
                log.error(message);
                dbAuditLogRepository.logDBError("", ApiOperationEnum.DOCUMENTATION, message);
            }

            if (mapping.getValue().getChildren() != null && !mapping.getValue().getChildren().isEmpty()) {
                exportJsonDocument(mapping.getValue().getChildren(), data);
            }
        }
    }

}
