package bg.adminsoft.neispuo.web.controller;

import bg.adminsoft.neispuo.config.MetadataConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.*;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import bg.adminsoft.neispuo.service.ImportService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.Map;

@Slf4j
@RestController
@RequestMapping({ "/import" })
public class ImportController extends BaseController {

    private final MetadataConfig metadataConfig;
    private final ImportService importService;

    public ImportController(MetadataConfig metadataConfig, ImportService importService,
                            DBTicketRepository dbTicketRepository, DBAuditLogRepository dbAuditLogRepository) {
        super(dbTicketRepository, dbAuditLogRepository);
        this.metadataConfig = metadataConfig;
        this.importService = importService;
    }

    @PostMapping(value = "institution/json", consumes = {"multipart/form-data"}, produces = MediaType.APPLICATION_JSON_VALUE)
    public Map<ValidationTypeEnum, List<String>> importInstitutionJson(@RequestParam(value = "ticket") String ticket,
                                                       @RequestParam(value = "validation", required = false) MultipartFile validation,
                                                       @RequestParam(value = "mapping", required = false) MultipartFile mapping,
                                                       @RequestParam(value = "validateData", required = false) Boolean validateData,
                                                       @RequestParam(value = "importData", required = false) Boolean importData,
                                                       @RequestHeader(value = "X-Client-Cert", required = false) String clientCertificate,
                                                       @RequestHeader("host") String host,
                                                       @RequestHeader(value = "user-agent", required = false) String userAgent) {
        //checkCertificate(clientCertificate); t o d o

        String validationJson = "";
        try {
            if (validation != null) {
                validationJson = new String(validation.getBytes());
            } else if (metadataConfig.getValidationFile() != null) {
                validationJson = new String(metadataConfig.getValidationFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
            } else {
                log.error("Wrong or missing VALIDATION parameter");
                dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, "Wrong or missing VALIDATION parameter");
                throw new InternalServerErrorException("Wrong or missing VALIDATION parameter");
            }
        } catch (IOException e) {
            log.error(e.getMessage());
            dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, e.getMessage());
            throw new InternalServerErrorException("Wrong or missing VALIDATION parameter");
        }
        String mappingJson = "";
        try {
            if (mapping != null) {
                mappingJson = new String(mapping.getBytes());
            } else if (metadataConfig.getJsonMappingFile() != null) {
                mappingJson = new String(metadataConfig.getJsonMappingFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
            } else {
                log.error("Wrong or missing MAPPING parameter");
                dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, "Wrong or missing MAPPING parameter");
                throw new InternalServerErrorException("Wrong or missing MAPPING parameter");
            }
        } catch (IOException e) {
            log.error(e.getMessage());
            dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, e.getMessage());
            throw new InternalServerErrorException("Wrong or missing MAPPING parameter");
        }
        return this.importService.importInstitutionJson(new TicketData(ticket), validationJson, mappingJson,
                validateData == null || validateData, importData == null || importData);
    }

    @PostMapping(value = "institution/schema", consumes = {"multipart/form-data"}, produces = MediaType.APPLICATION_JSON_VALUE)
    public Map<ValidationTypeEnum, List<String>> importInstitutionSchema(@RequestParam(value = "ticket") String ticket,
                                                     @RequestParam(value = "mapping", required = false) MultipartFile mapping,
                                                     @RequestHeader(value = "X-Client-Cert", required = false) String clientCertificate,
                                                     @RequestHeader("host") String host,
                                                     @RequestHeader(value = "user-agent", required = false) String userAgent) {
        //checkCertificate(clientCertificate); t o d o

        String mappingJson = null;
        try {
            if (mapping != null) {
                mappingJson = new String(mapping.getBytes());
            } else if (metadataConfig.getSchemaMappingFile() != null) {
                mappingJson = new String(metadataConfig.getSchemaMappingFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
            } else {
                log.error("Wrong or missing MAPPING parameter");
                dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, "Wrong or missing MAPPING parameter");
                throw new InternalServerErrorException("Wrong or missing MAPPING parameter");
            }
        } catch (IOException e) {
            log.error(e.getMessage());
            dbAuditLogRepository.logDBError("", ApiOperationEnum.IMPORT, e.getMessage());
            throw new InternalServerErrorException("Wrong or missing MAPPING parameter");
        }
        return this.importService.importInstitutionSchema(new TicketData(ticket), mappingJson);
    }
}
