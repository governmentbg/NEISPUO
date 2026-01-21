package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.exception.BadRequestException;
import bg.adminsoft.neispuo.exception.ForegnKeyViolationException;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.*;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.jayway.jsonpath.JsonPath;
import com.jayway.jsonpath.PathNotFoundException;
import com.jayway.jsonpath.internal.JsonContext;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import net.minidev.json.JSONArray;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;
import java.sql.Connection;
import java.sql.Savepoint;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

@Component
@Slf4j
@RequiredArgsConstructor
public class ImportRepositoryImpl implements ImportRepository {

    private final DatabaseConfig databaseConfig;
    private final DBTicketRepository dbTicketRepository;
    private final DBRepository dbRepository;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public List<ValidationResult> importInstitutionJson(TicketData ticket, String validationJson, String mappingJson,
                                              boolean validateData, boolean importData) {

        JsonValidation validations = null;
        JsonDataMapping dataMapping = null;
        try {
            validations = new ObjectMapper().readValue(validationJson, JsonValidation.class);
            dataMapping = new ObjectMapper().readValue(mappingJson, JsonDataMapping.class);
        } catch (Exception e) {
            String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException(e.getMessage());
        }

        JsonData jsonData = dbTicketRepository.getJsonByTicket(ticket);
        if (jsonData == null || jsonData.getJsonData().isEmpty()) {
            String message = ticket.getTicketId() + ": Не е наличен json обект за заявката";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new BadRequestException("Не е наличен json обект за заявката");
        }
        jsonData.setJsonContext((JsonContext) JsonPath.parse(jsonData.getJsonData()));

        Connection connection = databaseConfig.dbConnection();
        // todo ????
        dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.IN_PROGRESS, connection);
        List<ValidationResult> validateResult = new ArrayList<>();
        try {
            // todo ????
            if (validateData) {
                validateJson(jsonData, validations.getValidations(), "", validateResult);
                if (validateResult.stream()
                        .anyMatch(v -> v.getType() == ValidationTypeEnum.ERROR)) {
                    dbTicketRepository.saveIntegrityCheckResult(ticket, validateResult, true, connection);
                    // todo ????
                    dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.TECHNICAL_VALIDATION_FAILED, connection);
                    databaseConfig.closeConnectionWithCommit(connection);
                    return validateResult;
                }
            }

            if (importData) {
                Savepoint savePoint = null;
                try {
                    savePoint = connection.setSavepoint();
                    // todo ????
                    importJson(jsonData, "", dataMapping.getJsonMapping(), ticket.getTicketId(), new StagingTableKey(), connection);
                } catch (Exception e) {
                    if (savePoint != null) {
                        connection.rollback(savePoint);
                    }
                    throw e;
                }
            }

        } catch (ForegnKeyViolationException | BadRequestException | InternalServerErrorException e) {
            validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR, e.getMessage()));
            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.TECHNICAL_VALIDATION_FAILED, connection);
        } catch (Exception e) {
            validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                    "Грешка при обработка на данните! Обърнете се за съдействие към системния администратор!"));
            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.TECHNICAL_VALIDATION_FAILED, connection);
            //databaseConfig.closeConnectionWithCommit(connection);
            String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            //throw new InternalServerErrorException(e.getMessage());
        }
        dbTicketRepository.saveIntegrityCheckResult(ticket, validateResult, true, connection);
        databaseConfig.closeConnectionWithCommit(connection);
        return validateResult;
    }

    @Override
    public boolean validateStaging(TicketData ticket) {
        Connection connection = databaseConfig.dbConnection();
        try {
            return dbRepository.validateStaging(ticket) == 0;
        } catch (InternalServerErrorException e) {
            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.LOAD_DATA_FAILED, connection);
        } catch (Exception e) {
            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.LOAD_DATA_FAILED, connection);
            String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
        } finally {
            databaseConfig.closeConnectionWithCommit(connection);
        }
        return false;
    }

    @Override
    public List<ValidationResult> importInstitutionSchema(TicketData ticket, String mappingJson) {
        DBDataMapping dbDataMapping = null;
        try {
            dbDataMapping = new ObjectMapper().readValue(mappingJson, DBDataMapping.class);
        } catch (Exception e) {
            String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException(e.getMessage());
        }

        Connection connection = databaseConfig.dbConnection();
        List<ValidationResult> validateResult = new ArrayList<>();
        try {
            Savepoint savePoint = null;
            try {
                savePoint = connection.setSavepoint();
                deleteTable(dbDataMapping.getTableMapping(), ticket, connection);
                importTable(dbDataMapping.getTableMapping(), dbDataMapping.getLinkMapping(),
                        ticket, null, new HashMap<>(), connection);
                dbRepository.incrementInstitutionVersionData(ticket, connection);
            } catch (Exception e) {
                //String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
                //log.error(message);
                //dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                if (savePoint != null) {
                    connection.rollback(savePoint);
                }
                throw e;
            }

            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.COMPLETED, connection);
        } catch (ForegnKeyViolationException | BadRequestException | InternalServerErrorException e) {
            validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR, e.getMessage()));
            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.LOAD_DATA_FAILED, connection);
        } catch (Exception e) {
            validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                    "Грешка при обработка на данните! Обърнете се за съдействие към системния администратор!"));
            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.LOAD_DATA_FAILED, connection);
            //databaseConfig.closeConnectionWithCommit(connection);
            String message = ticket.getTicketId() + ": (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            //throw new InternalServerErrorException(e.getMessage());
        }
        dbTicketRepository.saveIntegrityCheckResult(ticket, validateResult, false, connection);
        databaseConfig.closeConnectionWithCommit(connection);
        return validateResult;
    }

    private void validateJson(JsonData jsonData, Map<String, Validation> validations,
                              String path, List<ValidationResult> validateResult) {
        for (Map.Entry<String, Validation> validation : validations.entrySet()) {
            Object data = "";
            boolean isArray = false;
            int arrayLength = 0;
            try {
                data = jsonData.getJsonContext().read("$." + path + validation.getKey());
                if (data instanceof JSONArray) {
                    isArray = true;
                    arrayLength = ((JSONArray) data).size();
                }
            } catch (PathNotFoundException e) {
                if (validation.getValue().getRequired()) {
                    validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                            "Липсващ обект: " + path + validation.getKey()));
                }
                continue;
            }
            if (validation.getValue().getIsArray() && !isArray) {
                validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                        "Очаква се масив от обекти вместо обект: " + path + validation.getKey()));
                continue;
            }
            if (!validation.getValue().getIsArray() && isArray) {
                validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                        "Очаква се обект вместо масив от обекти: " + path + validation.getKey()));
                continue;
            }
            if (isArray) {
                if (validation.getValue().getMinOccurs() != null &&
                        arrayLength < validation.getValue().getMinOccurs()) {
                    validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                            "Грешна дължина на масив. Очакват се поне " + validation.getValue().getMinOccurs()
                                    + " елемента: " + path + validation.getKey()));
                }
                if (validation.getValue().getMaxOccurs() != null &&
                        arrayLength > validation.getValue().getMaxOccurs()) {
                    validateResult.add(new ValidationResult(ValidationTypeEnum.ERROR,
                            "Грешна дължина на масив. Очакват се максимум " + validation.getValue().getMaxOccurs()
                                    + " елемента: " + path + validation.getKey()));
                }
            }

            /*for (Validation.ColumnValidation cv : validation.getValue().getRules()) {
                if (!isArray) {
                    String keyPath = validation.getKey() + "." + cv.getColumn();
                    String str = getFieldAsString(jsonDataContext, "$." + path + keyPath);
                    validateField(path + keyPath, str, cv, validateResult);
                } else {
                    for (int i = 0; i < arrayLength; i++) {
                        String keyPath = validation.getKey() + "[" + i + "]." + cv.getColumn();
                        String str = getFieldAsString(jsonDataContext, "$." + path + keyPath);
                        validateField(path + keyPath, str, cv, validateResult);
                    }
                }
            }*/

            // No validation for Delete operation
            if (!isArray) {
                String operationTypePath = validation.getKey() + ".operationType";
                String operationTypeStr = getFieldAsString(jsonData.getJsonContext(), "$." + path + operationTypePath);
                // if (!"3".equals(operationTypeStr)) {
                    for (Validation.ColumnValidation cv : validation.getValue().getRules()) {
                        String keyPath = validation.getKey() + "." + cv.getColumn();
                        String str = getFieldAsString(jsonData.getJsonContext(), "$." + path + keyPath);
                        if (!"3".equals(operationTypeStr) || "operationType".equals(cv.getColumn())) {
                            validateField(path + keyPath, str, cv, validateResult);
                        }
                    }
                //}
            } else {
                for (int i = 0; i < arrayLength; i++) {
                    String operationTypePath = validation.getKey() + "[" + i + "].operationType";
                    String operationTypeStr = getFieldAsString(jsonData.getJsonContext(), "$." + path + operationTypePath);
                    //if (!"3".equals(operationTypeStr)) {
                        for (Validation.ColumnValidation cv : validation.getValue().getRules()) {
                            String keyPath = validation.getKey() + "[" + i + "]." + cv.getColumn();
                            String str = getFieldAsString(jsonData.getJsonContext(), "$." + path + keyPath);
                            if (!"3".equals(operationTypeStr) || "operationType".equals(cv.getColumn())) {
                                validateField(path + keyPath, str, cv, validateResult);
                            }
                        }
                    //}
                }
            }

            if (!isArray) {
                if (validation.getValue().getChildren() != null &&
                        !validation.getValue().getChildren().isEmpty()) {
                    validateJson(jsonData, validation.getValue().getChildren(), path +
                            validation.getKey() + ".", validateResult);
                }
            } else {
                for (int i = 0; i < arrayLength; i++) {
                    if (validation.getValue().getChildren() != null &&
                            !validation.getValue().getChildren().isEmpty()) {
                        validateJson(jsonData, validation.getValue().getChildren(), path +
                                validation.getKey() + "[" + i + "].", validateResult);
                    }
                }
            }
        }
    }

    private void validateField(String field, String value, Validation.ColumnValidation validation,
                               List<ValidationResult> validateResult) {

        ValidationTypeEnum validationType = validation.getType() != null ? validation.getType() : ValidationTypeEnum.WARNING;

        if (value == null || value.trim().isEmpty()) {
            if (validation.getRequired()) {
                validateResult.add(new ValidationResult(validationType,"Липсваща стойност за атрибут: " + field));
            }
            return;
        }

        switch (validation.getDataType()) {
            case STRING:
                if (validation.getPattern() != null && !validation.getPattern().isEmpty() &&
                        !isValidPattern(value, validation.getPattern())) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалидни символи в символен низ: (" + field + ", " + value + ")"));
                }
                if (validation.getMinLenght() != null && value.trim().length() < validation.getMinLenght()) {
                    validateResult.add(new ValidationResult(validationType,
                            "Дължина на символен низ по-малка от " + validation.getMinLenght() +
                                    " символа: (" + field + ", " + value + ")"));
                } else if (validation.getMaxLenght() != null && value.trim().length() > validation.getMaxLenght()) {
                    validateResult.add(new ValidationResult(validationType,
                            "Дължина на символен низ по-голяма от " + validation.getMaxLenght() +
                                    " символа: (" + field + ", " + value + ")"));
                }
                break;
            case NUMBER:
                if (validation.getPattern() != null && !validation.getPattern().isEmpty() &&
                        !isValidPattern(value, validation.getPattern())) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалиден формат на число: (" + field + ", " + value + ")"));
                }
                if (!isValidNumber(value)) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалиден формат на число: (" + field + ", " + value + ")"));
                } else {
                    if (validation.getMinValue() != null) {
                        BigDecimal valueNumber = new BigDecimal(value);
                        if (valueNumber.compareTo(validation.getMinValue()) < 0) {
                            validateResult.add(new ValidationResult(validationType,
                                    "Число по-малко от " + validation.getMinValue() +
                                            " : (" + field + ", " + value + ")"));
                        }
                    }
                    if (validation.getMaxValue() != null) {
                        BigDecimal valueNumber = new BigDecimal(value);
                        if (valueNumber.compareTo(validation.getMaxValue()) > 0) {
                            validateResult.add(new ValidationResult(validationType,
                                    "Число по-голямо от " + validation.getMaxValue() +
                                            " : (" + field + ", " + value + ")"));
                        }
                    }
                }
                break;
            case DATE:
                if (!isValidDate(value, validation.getFormat())) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалиден формат на дата: (" + field + ", " + value + ")"));
                }
                break;
            case EMAIL:
                if (!isValidEmail(value, validation.getPattern())) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалиден EMAIL адрес: (" + field + ", " + value + ")"));
                }
                break;
            case ENUMERATION:
                if (!validation.getValues().contains(value)) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалидна стойност от предефинирано множество: (" + field + ", " + value + ")"));
                }
                break;
            case FLAG:
                if (!"false".equalsIgnoreCase(value.trim()) &&
                        !"true".equalsIgnoreCase(value.trim())) {
                    validateResult.add(new ValidationResult(validationType,
                            "Невалидна стойност за флаг: (" + field + ", " + value + ")"));
                }
        }
    }

    private boolean isValidNumber(String value) {
        try {
            new BigDecimal(value);
        } catch (NumberFormatException e) {
            return false;
        }
        return true;
    }

    private boolean isValidDate(String dateStr, String format) {
        DateFormat sdf = new SimpleDateFormat(format);
        sdf.setLenient(false);
        try {
            sdf.parse(dateStr);
        } catch (ParseException e) {
            return false;
        }
        return true;
    }

    private boolean isValidEmail(String value, String pattern) {
        Pattern p = Pattern.compile(pattern != null && !pattern.isEmpty() ?
                pattern : "^[a-z0-9._%+-]+@[a-z0-9.-]+.[a-z]{2,4}$");
        Matcher matcher = p.matcher(value.toLowerCase());
        return matcher.matches();
    }

    private boolean isValidPattern(String value, String pattern) {
        Pattern p = Pattern.compile(pattern);
        Matcher matcher = p.matcher(value.toLowerCase());
        return matcher.matches();
    }

    private String getFieldAsString(JsonContext jsonDataContext, String path) {
        try {
            Object o = jsonDataContext.read(path);
            return (o != null ? o.toString() : null);
        } catch (PathNotFoundException e) {
            return null;
        }
    }

    private void importJson(JsonData jsonData, String path, Map<String, JsonMapping> jsonMapping,
                            String ticket, StagingTableKey parentKey, Connection connection) {
        for (Map.Entry<String, JsonMapping> table : jsonMapping.entrySet()) {
            //System.out.println("");
            //System.out.println("JsonMapping: " + "$." + path + table.getKey());
            //System.out.println("JsonMapping: " + table.getKey() + " >> " + table.getValue().getTable());
            Object data = null;
            try {
                data = jsonData.getJsonContext().read("$." + path + table.getKey());
            } catch (PathNotFoundException e) {
                continue;
            }
            if (data instanceof JSONArray) {
                for (int i = 0; i < ((JSONArray) data).size(); i++) {
                    LinkedHashMap row = (LinkedHashMap) ((JSONArray) data).get(i);
                    //debugMapping(table.getValue(), row);
                    StagingTableKey primaryKey = null;
                    if (table.getValue().getOperation() == DBOperationType.GET_PRIMARY_KEY) {
                        primaryKey = dbRepository.getJsonPrimaryKey(table.getValue(), row, ticket);
                    } else {
                        primaryKey = dbRepository.insertJsonIntoStaging(table.getValue(), row,
                                ticket, parentKey, jsonData.getDefaultData(), connection);
                    }
                    primaryKey.setParentExternalId(parentKey.getExternalId());
                    primaryKey.setParentRecordId(parentKey.getRecordId());
                    if (table.getValue().getChildren() != null && !table.getValue().getChildren().isEmpty()) {
                        importJson(jsonData, path + table.getKey() + "[" + i + "].",
                                table.getValue().getChildren(), ticket, primaryKey, connection);
                    }
                }
            } else {
                LinkedHashMap row = (LinkedHashMap) data;
                //debugMapping(table.getValue(), row);
                StagingTableKey primaryKey = null;
                if (table.getValue().getOperation() == DBOperationType.GET_PRIMARY_KEY) {
                    primaryKey = dbRepository.getJsonPrimaryKey(table.getValue(), row, ticket);
                } else {
                    primaryKey = dbRepository.insertJsonIntoStaging(table.getValue(), row,
                            ticket, parentKey, jsonData.getDefaultData(), connection);
                }
                primaryKey.setParentExternalId(parentKey.getExternalId());
                primaryKey.setParentRecordId(parentKey.getRecordId());
                if (table.getValue().getChildren() != null && !table.getValue().getChildren().isEmpty()) {
                    importJson(jsonData, path + table.getKey() + ".",
                            table.getValue().getChildren(), ticket, primaryKey, connection);
                }
            }
        }
    }

    private void importTable(Map<String, DBMapping> dbMapping, DBLinkMapping[] linkMapping, TicketData ticket,
                             StagingTableKey parentKey, Map<String, Map<String, Long>> tableIdentifiers,
                             Connection connection) {
        for (Map.Entry<String, DBMapping> table : dbMapping.entrySet()) {
            //System.out.println("");
            //System.out.println("DBMapping: " + table.getKey());
            List<StagingTableData> stagingTableDataList =
                    dbRepository.getStagingTableData(table.getKey(), table.getValue(), ticket,
                            parentKey != null ? parentKey.getExternalId() : null, connection);
            for (StagingTableData stagingTableData : stagingTableDataList) {
                if (stagingTableData.getTableKey() == null || stagingTableData.getTableKey().getExternalId() == null) {
                    String message = "Липсва стойност за ExternalId в таблица " + table.getKey();
                    log.error(ticket.getTicketId() + ": " + message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT,
                            ticket.getTicketId() + ": " + message);
                    throw new InternalServerErrorException(message);
                }
                //System.out.println("    ExternalId: " + stagingTableData.getTableKey().getExternalId());
                StagingTableKey primaryKey = null;
                switch (stagingTableData.getOperationType()) {
                    case 0: // No changes
                        // required PK for non changed data
                        primaryKey = stagingTableData.getTableKey();
                        if (primaryKey.getRecordId() == null) {
                            String message = "Липсва стойност за PK на непроменени данни в таблица " + table.getKey() +
                                    " (ExternalId = " + primaryKey.getExternalId() + ")";
                            log.error(ticket.getTicketId() + ": " + message);
                            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT,
                                    ticket.getTicketId() + ": " + message);
                            throw new BadRequestException(message);
                        }
                        dbRepository.updateStagingExternalIdIntoSchema(stagingTableData, table.getKey(),
                                table.getValue(), ticket, connection);
                        break;
                    case 1: // Insert
                        // PK should be null for insert operation
                        primaryKey = stagingTableData.getTableKey();
                        if (primaryKey.getRecordId() != null) {
                            String message = "PK трябва да бъде NULL за операция INSERT в таблица " + table.getKey() +
                                    " (ExternalId = " + primaryKey.getExternalId() + ")";
                            log.error(ticket.getTicketId() + ": " + message);
                            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT,
                                    ticket.getTicketId() + ": " + message);
                            throw new BadRequestException(message);
                        }
                        if (parentKey != null && parentKey.getRecordId() == null) {
                            String message = "Липсва стойност за FK към master обект при операция INSERT в таблица " + table.getKey() +
                                    " (ExternalId = " + primaryKey.getExternalId() + ")";
                            log.error(ticket.getTicketId() + ": " + message);
                            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT,
                                    ticket.getTicketId() + ": " + message);
                            throw new BadRequestException(message);
                        }
                        Long recordId = dbRepository.insertStagingIntoSchema(stagingTableData, table.getKey(),
                                table.getValue(), parentKey != null ? parentKey.getRecordId() : null,
                                parentKey != null ? parentKey.getParentRecordId() : null,
                                linkMapping, tableIdentifiers, ticket, connection);
                        primaryKey.setRecordId(recordId);
                        break;
                    case 2: // Update
                        // required PK for update operation
                        primaryKey = stagingTableData.getTableKey();
                        if (primaryKey == null || primaryKey.getRecordId() == null) {
                            String message = "PK не трябва да бъде NULL за операция UPDATE на таблица " + table.getKey() +
                                    " (ExternalId = " + primaryKey.getExternalId() + ")";
                            log.error(ticket.getTicketId() + ": " + message);
                            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT,
                                    ticket.getTicketId() + ": " + message);
                            throw new BadRequestException(message);
                        }
                        dbRepository.updateStagingIntoSchema(stagingTableData, table.getKey(),
                                table.getValue(), ticket, connection);
                        break;
                    case 3:
                        continue;
                }
                primaryKey.setParentExternalId(parentKey != null ? parentKey.getExternalId() : null);
                primaryKey.setParentRecordId(parentKey != null ? parentKey.getRecordId() : null);
                if (!tableIdentifiers.containsKey(table.getKey())) {
                    tableIdentifiers.put(table.getKey(), new HashMap<>());
                }
                tableIdentifiers.get(table.getKey())
                        .put(primaryKey.getExternalId(), primaryKey.getRecordId());
                if (table.getValue().getChildren() != null &&
                        !table.getValue().getChildren().isEmpty()) {
                    importTable(table.getValue().getChildren(), linkMapping, ticket,
                            primaryKey, tableIdentifiers, connection);
                }
            }
        }
    }

    private void deleteTable(Map<String, DBMapping> dbMapping, TicketData ticket, Connection connection) {
        for (Map.Entry<String, DBMapping> table : dbMapping.entrySet()) {
            //System.out.println("");
            //System.out.println("DBMapping: " + table.getKey());
            if (table.getValue().getChildren() != null && !table.getValue().getChildren().isEmpty()) {
                deleteTable(table.getValue().getChildren(), ticket, connection);
            }

            List<StagingTableData> stagingTableDataList =
                    dbRepository.getStagingTableData(table.getKey(), table.getValue(), ticket, null, connection);
            for (StagingTableData stagingTableData : stagingTableDataList) {
                if (stagingTableData.getTableKey() == null || stagingTableData.getTableKey().getExternalId() == null) {
                    String message = "Липсва стойност за ExternalId в таблица " + table.getKey();
                    log.error(ticket.getTicketId() + ": " + message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT,
                            ticket.getTicketId() + ": " + message);
                    throw new InternalServerErrorException(message);
                }
                if (stagingTableData.getOperationType() == 3) {
                    dbRepository.deleteSchema(stagingTableData, table.getKey(), table.getValue(), ticket, connection);
                }
            }
        }
    }

    private void debugMapping(JsonMapping table, LinkedHashMap row) {
        if (table.getAttributeMapping() != null) {
            for (JsonMapping.AttributeMapping column : table.getAttributeMapping()) {
                System.out.println(table.getTable() + ": " + column.getFromAttribute() + " >> " + column.getToColumn() +
                        "(" + row.get(column.getFromAttribute()) + ")");
            }
        }
    }

}
