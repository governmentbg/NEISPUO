package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.config.MetadataConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.*;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import bg.adminsoft.neispuo.repository.ImportRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.nio.charset.StandardCharsets;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.Semaphore;
import java.util.stream.Collectors;

@Service
@Slf4j
@RequiredArgsConstructor
public class ImportServiceImpl extends BaseServiceImpl implements ImportService {

    public static final Semaphore semaphore = new Semaphore(1);

    private final DatabaseConfig databaseConfig;
    private final MetadataConfig metadataConfig;
    private final DBTicketRepository dbTicketRepository;
    private final ImportRepository importRepository;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public Map<ValidationTypeEnum, List<String>> importInstitutionJson(TicketData ticket, String validationJson, String mappingJson,
                                                                       boolean validateData, boolean importData) {
        if (!isJSONValid(validationJson)) {
            String message = ticket.getTicketId() + ": Invalid json format for VALIDATION parameter";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException("Invalid json format for VALIDATION parameter");
        }
        if (!isJSONValid(mappingJson)) {
            String message = ticket.getTicketId() + ": Invalid json format for MAPPING parameter";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException("Invalid json format for MAPPING parameter");
        }
        List<ValidationResult> validationResults = this.importRepository.importInstitutionJson(ticket, validationJson, mappingJson,
                validateData, importData);
        return transformResult(validationResults);
    }

    @Override
    public Map<ValidationTypeEnum, List<String>>  importInstitutionSchema(TicketData ticket, String mappingJson) {
        if (!isJSONValid(mappingJson)) {
            String message = ticket.getTicketId() + ": Invalid json format for MAPPING parameter";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException("Invalid json format for MAPPING parameter");
        }
        List<ValidationResult> validationResults = this.importRepository.importInstitutionSchema(ticket, mappingJson);
        return transformResult(validationResults);
    }

    @Override
    public void importInstitutionData() {
        try {
            TicketData ticket;
            while (true) {
                try {
                    semaphore.acquire();
                    ticket = dbTicketRepository.getTicketByStatus(TicketStatusEnum.PENDING,
                            TicketTypeEnum.IMPORT);
                    if (ticket != null && ticket.getTicketId() != null) {
                        // todo ????
                        dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.IN_PROGRESS);
                    }
                    semaphore.release();
                } catch (InternalServerErrorException e) {
                    semaphore.release();
                    Thread.sleep(60 * 1000L);
                    continue;
                }
                try {
                    if (ticket != null && ticket.getTicketId() != null) {
                        log.info("Begin processing ticket " + ticket.getTicketId() + " ...");
                        String validationJson = getValidationJson(ticket);
                        String jsonMappingJson = getJsonMappingJson(ticket);
                        String schemaMappingJson = getSchemaMappingJson(ticket);
                        try {
                            List<ValidationResult> validationResults =
                                    this.importRepository.importInstitutionJson(ticket, validationJson,
                                            jsonMappingJson, true, true);
                            // todo ????
                            if (validationResults.stream()
                                    .noneMatch(v -> v.getType() == ValidationTypeEnum.ERROR)) {
                                if (this.importRepository.validateStaging(ticket)) {
                                    this.importRepository.importInstitutionSchema(ticket, schemaMappingJson);
                                    JsonData jsonData = dbTicketRepository.getJsonByTicket(ticket);
                                    this.dbAuditLogRepository.logEvent(ticket, jsonData);
                                    log.info("Ticket " + ticket.getTicketId() + " processed successfully");
                                } else {
                                    log.error("Ticket " + ticket.getTicketId() + " failed");
                                }
                            } else {
                                log.error("Ticket " + ticket.getTicketId() + " failed");
                            }
                        } catch (Exception e) {
                            log.error("Ticket " + ticket.getTicketId() + " failed");
                        }
                    } else {
                        Thread.sleep(5 * 1000L); //t o d o
                    }
                } catch (InternalServerErrorException e) {
                    Thread.sleep(5 * 1000L); //t o d o
                }
            }
        } catch (InterruptedException e) {
            // t o d o
            String message = "Import task InterruptedException: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, message);
        } catch (Exception e) {
            // t o d o
            String message = "Import task Exception: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, message);
        }
    }

    private String getValidationJson(TicketData ticket) {
        try {
            if (metadataConfig.getValidationFile() != null) {
                String json = new String(metadataConfig.getValidationFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
                if (!isJSONValid(json)) {
                    String message = ticket.getTicketId() + ": Invalid json format for VALIDATION JSON";
                    log.error(message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                    throw new InternalServerErrorException("Invalid json format for VALIDATION JSON");
                }
                return json;
            } else {
                String message = ticket.getTicketId() + ": Wrong or missing VALIDATION JSON";
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException("Wrong or missing VALIDATION JSON");
            }
        } catch (Exception e) {
            String message = ticket.getTicketId() + ": Wrong or missing VALIDATION JSON (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException("Wrong or missing VALIDATION JSON");
        }
    }

    private String getJsonMappingJson(TicketData ticket) {
        try {
            if (metadataConfig.getJsonMappingFile() != null) {
                String json = new String(metadataConfig.getJsonMappingFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
                if (!isJSONValid(json)) {
                    String message = ticket.getTicketId() + ": Invalid json format for JSON MAPPING";
                    log.error(message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                    throw new InternalServerErrorException("Invalid json format for JSON MAPPING");
                }
                return json;
            } else {
                String message = ticket.getTicketId() + ": Wrong or missing JSON MAPPING";
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException("Wrong or missing JSON MAPPING");
            }

        } catch (Exception e) {
            String message = ticket.getTicketId() + ": Wrong or missing JSON MAPPING (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException("Wrong or missing JSON MAPPING");
        }
    }

    private String getSchemaMappingJson(TicketData ticket) {
        try {
            if (metadataConfig.getSchemaMappingFile() != null) {
                String json = new String(metadataConfig.getSchemaMappingFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
                if (!isJSONValid(json)) {
                    String message = ticket.getTicketId() + ": Invalid json format for SCHEMA MAPPING";
                    log.error(message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                    throw new InternalServerErrorException("Invalid json format for SCHEMA MAPPING");
                }
                return json;
            } else {
                String message = ticket.getTicketId() + ": Wrong or missing SCHEMA MAPPING";
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
                throw new InternalServerErrorException("Wrong or missing SCHEMA MAPPING");
            }
        } catch (Exception e) {
            String message = ticket.getTicketId() + ": Wrong or missing SCHEMA MAPPING (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InternalServerErrorException("Wrong or missing SCHEMA MAPPING");
        }
    }

    private Map<ValidationTypeEnum, List<String>> transformResult(List<ValidationResult> validationResults) {
        Map<ValidationTypeEnum, List<String>> result = new LinkedHashMap<>();
        if (validationResults != null) {
            result.put(ValidationTypeEnum.ERROR,
                    validationResults.stream()
                            .filter(v -> v.getType() == ValidationTypeEnum.ERROR)
                            .map(v -> v.getMessage())
                            .collect(Collectors.toList()));
            result.put(ValidationTypeEnum.WARNING,
                    validationResults.stream()
                            .filter(v -> v.getType() == ValidationTypeEnum.WARNING)
                            .map(v -> v.getMessage())
                            .collect(Collectors.toList()));
        }
        return  result;
    }
}
