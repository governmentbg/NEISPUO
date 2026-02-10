package bg.adminsoft.neispuo.web.controller;

import bg.adminsoft.neispuo.exception.BadRequestException;
import bg.adminsoft.neispuo.exception.InvalidImportPeriodException;
import bg.adminsoft.neispuo.exception.UnauthorizedException;
import bg.adminsoft.neispuo.model.*;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import bg.adminsoft.neispuo.service.TicketService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.Base64;

@Slf4j
@RestController
@RequestMapping({ "/ticket" })
public class TicketImportController extends BaseController {

    private final TicketService ticketService;

    public TicketImportController(TicketService ticketService, DBTicketRepository dbTicketRepository,
                                  DBAuditLogRepository dbAuditLogRepository) {
        super(dbTicketRepository, dbAuditLogRepository);
        this.ticketService = ticketService;
    }

    @Operation(summary = "Creates ticket for institution data import")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Created ticket",
                    content = { @Content(mediaType = "text/plain",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for institution",
                    content = @Content),
            @ApiResponse(responseCode = "403", description = "Invalid school year or Invalid institution status for sending data",
                    content = @Content),
            @ApiResponse(responseCode = "400",
                    description = "Missing DATA parameter or Invalid json format for DATA parameter or Another ticket in status PENDING or IN PROGRESS exists",
                    content = @Content) })
    @PostMapping(value = "import/create", consumes = MediaType.MULTIPART_FORM_DATA_VALUE,
            produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseEntity<String> create(@RequestParam(value = "institutionId") Long institutionId,
                                 @RequestParam(value = "schoolYear") Integer schoolYear,
                                 @RequestParam(value = "data") MultipartFile data,
                                 @RequestHeader("X-Client-Cert") String clientCertificate,
                                 @RequestHeader("host") String host,
                                 @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", institutionId, schoolYear, host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.IMPORT);

        if (! ticketService.checkInstitutionCurrentYear(ticket)) {
            String message = "Create import ticket: Invalid school year (" + institutionId + ", " + schoolYear + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid school year");
        }

        if (ticketService.checkListTemplatePeriod(ticket)) {
            String message = "Create import ticket: Invalid institution status for sending data (" + institutionId + ", " + schoolYear + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid institution status for sending data");
        }

        if (ticketService.checkNewYearTransitionPeriod(ticket)) {
            String message = "Create import ticket: Invalid institution status for sending data (" + institutionId + ", " + schoolYear + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid institution status for sending data");
        }

        String dataStr = "";
        try {
            dataStr = new String(data.getBytes());
        } catch (IOException e) {
            String message = "Create import ticket: Missing DATA parameter (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new BadRequestException("Missing DATA parameter");
        }
        if (!isJSONValid(dataStr)) {
            String message = "Create import ticket: Invalid json format for DATA parameter (" + institutionId + ", " + schoolYear + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new BadRequestException("Invalid json format for DATA parameter");
        }
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(this.ticketService.create(institutionId, schoolYear, provider, dataStr,
                        TicketTypeEnum.IMPORT, TicketSubTypeEnum.ALL,
                        host != null && !"".equals(host) ? host : "*",
                        userAgent != null && !"".equals(userAgent) ? userAgent : "*"));
    }

    @Operation(summary = "Creates ticket for institution data import")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Created ticket",
                    content = { @Content(mediaType = "text/plain",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for institution",
                    content = @Content),
            @ApiResponse(responseCode = "403", description = "Invalid school year or Invalid institution status for sending data",
                    content = @Content),
            @ApiResponse(responseCode = "400",
                    description = "Missing DATA parameter or Invalid json format for DATA parameter or Another ticket in status PENDING or IN PROGRESS exists",
                    content = @Content) })
    @PostMapping(value = "import/createJson", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseEntity<String> createJson(@RequestBody TicketImportRequest request,
                                             @RequestHeader("X-Client-Cert") String clientCertificate,
                                             @RequestHeader("host") String host,
                                             @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", request.getInstitutionId(), request.getSchoolYear(), host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.IMPORT);

        if (! ticketService.checkInstitutionCurrentYear(ticket)) {
            String message = "Create import ticket: Invalid school year (" + request.getInstitutionId() + ", " + request.getSchoolYear() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid school year");
        }

        if (ticketService.checkListTemplatePeriod(ticket)) {
            String message = "Create import ticket: Invalid institution status for sending data (" +
                    request.getInstitutionId() + ", " + request.getSchoolYear() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid institution status for sending data");
        }

        if (ticketService.checkNewYearTransitionPeriod(ticket)) {
            String message = "Create import ticket: Invalid institution status for sending data (" +
                    request.getInstitutionId() + ", " + request.getSchoolYear() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid institution status for sending data");
        }

        String dataStr = request.getJson();
        if (dataStr == null || dataStr.isEmpty()) {
            String message = "Create import ticket: Missing DATA parameter (" + request.getInstitutionId() + ", " + request.getSchoolYear() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new BadRequestException("Missing DATA parameter");
        }

        dataStr = new String(Base64.getDecoder().decode(dataStr));

        if (!isJSONValid(dataStr)) {
            String message = "Create import ticket: Invalid json format for DATA parameter (" + request.getInstitutionId() + ", " + request.getSchoolYear() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new BadRequestException("Invalid json format for DATA parameter");
        }
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(this.ticketService.create(request.getInstitutionId(), request.getSchoolYear(),
                        provider, dataStr, TicketTypeEnum.IMPORT, TicketSubTypeEnum.ALL,
                        host != null && !"".equals(host) ? host : "*",
                        userAgent != null && !"".equals(userAgent) ? userAgent : "*"));
    }

    @Operation(summary = "Get ticket status")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Ticket status",
                    content = { @Content(mediaType = "application/json",
                            schema = @Schema(implementation = TicketStatusResponse.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for ticket",
                    content = @Content) })
    @GetMapping(path = "import/status", produces = MediaType.APPLICATION_JSON_VALUE)
    public TicketStatusResponse status(@RequestParam(value = "ticketId") String ticketId,
                                       @RequestHeader("X-Client-Cert") String clientCertificate) {
        TicketData ticket = this.ticketService.getTicket(ticketId, TicketTypeEnum.IMPORT);
        if (ticket != null) {
            checkCertificate(clientCertificate, ticket, ApiOperationEnum.IMPORT);
        } else {
            throw new UnauthorizedException("Invalid request");
        }
        return this.ticketService.status(ticketId, true, TicketTypeEnum.IMPORT);
    }

}
