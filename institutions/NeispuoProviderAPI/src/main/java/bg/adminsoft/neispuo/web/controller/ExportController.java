package bg.adminsoft.neispuo.web.controller;

import bg.adminsoft.neispuo.config.MetadataConfig;
import bg.adminsoft.neispuo.exception.BadRequestException;
import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.ProviderInfo;
import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import bg.adminsoft.neispuo.service.ExportService;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.nio.charset.StandardCharsets;

@Slf4j
@RestController
@RequestMapping({ "/export" })
public class ExportController extends BaseController {

    private final MetadataConfig metadataConfig;
    private final ExportService exportService;

    public ExportController(MetadataConfig metadataConfig, ExportService exportService,
                            DBTicketRepository dbTicketRepository, DBAuditLogRepository dbAuditLogRepository) {
        super(dbTicketRepository, dbAuditLogRepository);
        this.metadataConfig = metadataConfig;
        this.exportService = exportService;
    }

    @PostMapping(value = "institution", consumes = {"multipart/form-data"}, produces = MediaType.APPLICATION_JSON_VALUE)
    public String exportInstitution(@RequestParam(value = "institutionId") Long institutionId,
                                    @RequestParam(value = "schoolYear") Integer schoolYear,
                                    @RequestParam(value = "mapping", required = false) MultipartFile mapping,
                                    @RequestHeader("X-Client-Cert") String clientCertificate,
                                    @RequestHeader("host") String host,
                                    @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", institutionId, schoolYear, host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);

        String mappingStr = "";
        try {
            if (mapping != null) {
                mappingStr = new String(mapping.getBytes());
            } else if (metadataConfig.getExportMappingFile() != null) {
                mappingStr = new String(metadataConfig.getExportMappingFile().getInputStream().readAllBytes(), StandardCharsets.UTF_8);
            } else {
                String message = "Wrong or missing MAPPING parameter (" + institutionId + ", " + schoolYear + ")";
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
                throw new BadRequestException("Wrong or missing MAPPING parameter");
            }
        } catch (IOException e) {
            String message = "Wrong or missing MAPPING parameter (" + institutionId + ", " + schoolYear + ") (" +
                    e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new BadRequestException("Wrong or missing MAPPING parameter");
        }
        if (!isJSONValid(mappingStr)) {
            String message = "Invalid json format for MAPPING parameter (" + institutionId + ", " + schoolYear + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new BadRequestException("Invalid json format for MAPPING parameter");
        }
        return this.exportService.exportInstitution(institutionId, schoolYear, mappingStr);
    }
}
