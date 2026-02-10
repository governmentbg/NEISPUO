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

@Slf4j
@RestController
@RequestMapping({ "/ticket" })
public class TicketExportController extends BaseController {

    private final TicketService ticketService;

    public TicketExportController(TicketService ticketService, DBTicketRepository dbTicketRepository,
                                  DBAuditLogRepository dbAuditLogRepository) {
        super(dbTicketRepository, dbAuditLogRepository);
        this.ticketService = ticketService;
    }

    @Operation(summary = "Creates ticket for institution data export")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Created ticket",
                    content = { @Content(mediaType = "text/plain",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for institution",
                    content = @Content),
            @ApiResponse(responseCode = "403", description = "Invalid school year",
                    content = @Content),
            @ApiResponse(responseCode = "400", description = "Another ticket in status PENDING or IN PROGRESS exists",
                    content = @Content) })
    @PostMapping(value = "export/create", consumes = MediaType.MULTIPART_FORM_DATA_VALUE,
            produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseEntity<String> create(@RequestParam(value = "institutionId") Long institutionId,
                                         @RequestParam(value = "schoolYear") Integer schoolYear,
                                         @RequestHeader("X-Client-Cert") String clientCertificate,
                                         @RequestHeader("host") String host,
                                         @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", institutionId, schoolYear, host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);

        if (! ticketService.checkInstitutionCurrentYear(ticket)) {
            String message = "Create import ticket: Invalid school year (" + institutionId + ", " + schoolYear + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid school year");
        }

        return ResponseEntity.status(HttpStatus.CREATED)
                .body(this.ticketService.create(institutionId, schoolYear, provider, "{}",
                        TicketTypeEnum.EXPORT, TicketSubTypeEnum.ALL,
                        host != null && !"".equals(host) ? host : "*",
                        userAgent != null && !"".equals(userAgent) ? userAgent : "*"));
    }

    @Operation(summary = "Creates ticket for institution data export")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Created ticket",
                    content = { @Content(mediaType = "text/plain",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for institution",
                    content = @Content),
            @ApiResponse(responseCode = "403", description = "Invalid school year",
                    content = @Content),
            @ApiResponse(responseCode = "400", description = "Another ticket in status PENDING or IN PROGRESS exists",
                    content = @Content) })
    @PostMapping(value = "export/createJson", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseEntity<String> createJson(@RequestBody TicketExportRequest request,
                                             @RequestHeader("X-Client-Cert") String clientCertificate,
                                             @RequestHeader("host") String host,
                                             @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", request.getInstitutionId(),
                request.getSchoolYear(), host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);

        if (! ticketService.checkInstitutionCurrentYear(ticket)) {
            String message = "Create import ticket: Invalid school year (" + request.getInstitutionId() + ", " + request.getSchoolYear() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.IMPORT, message);
            throw new InvalidImportPeriodException("Invalid school year");
        }

        return ResponseEntity.status(HttpStatus.CREATED)
                .body(this.ticketService.create(request.getInstitutionId(), request.getSchoolYear(),
                        provider, "{}", TicketTypeEnum.EXPORT, TicketSubTypeEnum.ALL,
                        host != null && !"".equals(host) ? host : "*",
                        userAgent != null && !"".equals(userAgent) ? userAgent : "*"));
    }

    @Operation(summary = "Creates ticket for institution IDs export")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Created ticket",
                    content = { @Content(mediaType = "text/plain",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for institution",
                    content = @Content),
            @ApiResponse(responseCode = "400", description = "Another ticket in status PENDING or IN PROGRESS exists",
                    content = @Content) })
    @PostMapping(value = "export/ids/create", consumes = MediaType.MULTIPART_FORM_DATA_VALUE,
            produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseEntity<String> createIds(@RequestParam(value = "institutionId") Long institutionId,
                                            @RequestParam(value = "schoolYear") Integer schoolYear,
                                            @RequestHeader("X-Client-Cert") String clientCertificate,
                                            @RequestHeader("host") String host,
                                            @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", institutionId, schoolYear, host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);

        return ResponseEntity.status(HttpStatus.CREATED)
                .body(this.ticketService.create(institutionId, schoolYear, provider, "{}",
                        TicketTypeEnum.EXPORT, TicketSubTypeEnum.IDS,
                        host != null && !"".equals(host) ? host : "*",
                        userAgent != null && !"".equals(userAgent) ? userAgent : "*"));
    }

    @Operation(summary = "Creates ticket for institution IDs export")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Created ticket",
                    content = { @Content(mediaType = "text/plain",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for institution",
                    content = @Content),
            @ApiResponse(responseCode = "400", description = "Another ticket in status PENDING or IN PROGRESS exists",
                    content = @Content) })
    @PostMapping(value = "export/ids/createJson", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.TEXT_PLAIN_VALUE)
    public ResponseEntity<String> createIdsJson(@RequestBody TicketExportRequest request,
                                                @RequestHeader("X-Client-Cert") String clientCertificate,
                                                @RequestHeader("host") String host,
                                                @RequestHeader(value = "user-agent", required = false) String userAgent) {

        TicketData ticket = new TicketData("", request.getInstitutionId(),
                request.getSchoolYear(), host, userAgent);

        ProviderInfo provider = checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);

        return ResponseEntity.status(HttpStatus.CREATED)
                .body(this.ticketService.create(request.getInstitutionId(), request.getSchoolYear(),
                        provider, "{}", TicketTypeEnum.EXPORT, TicketSubTypeEnum.IDS,
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
    @GetMapping(path = "export/status", produces = MediaType.APPLICATION_JSON_VALUE)
    public TicketStatusResponse status(@RequestParam String ticketId,
                                       @RequestHeader("X-Client-Cert") String clientCertificate) {
        TicketData ticket = this.ticketService.getTicket(ticketId, TicketTypeEnum.EXPORT);
        if (ticket != null) {
            checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);
        } else {
            throw new UnauthorizedException("Invalid request");
        }
        return this.ticketService.status(ticketId, false, TicketTypeEnum.EXPORT);
    }

    @Operation(summary = "Get generated data for ticket")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Generated data for ticket",
                    content = { @Content(mediaType = "application/json",
                            schema = @Schema(implementation = String.class)) }),
            @ApiResponse(responseCode = "401", description = "Invalid client certificate for ticket",
                    content = @Content),
            @ApiResponse(responseCode = "400", description = "Ticket is not completed or completed with errors",
                    content = @Content) })
    @GetMapping(path = "export/data", produces = MediaType.APPLICATION_JSON_VALUE)
    public String data(@RequestParam String ticketId,
                       @RequestHeader("X-Client-Cert") String clientCertificate) {
        TicketData ticket = this.ticketService.getTicket(ticketId, TicketTypeEnum.EXPORT);
        if (ticket != null) {
            checkCertificate(clientCertificate, ticket, ApiOperationEnum.EXPORT);
            if (ticket.getStatus() != TicketStatusEnum.COMPLETED) {
                throw new BadRequestException(ticket.getStatus() == TicketStatusEnum.IN_PROGRESS ?
                        "Заявката е в процес на обработка. Моля опитайте по-късно" :
                        "Данните не са налични. Моля извикайте услугата за статус на заявка");
            }
        } else {
            throw new UnauthorizedException("Invalid request");
        }
        return this.ticketService.data(ticket, TicketTypeEnum.EXPORT);
    }
}
