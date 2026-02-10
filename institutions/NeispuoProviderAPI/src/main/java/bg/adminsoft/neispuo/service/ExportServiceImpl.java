package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.config.MetadataConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.*;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import bg.adminsoft.neispuo.repository.ExportRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.core.io.FileSystemResource;
import org.springframework.stereotype.Service;

import java.nio.charset.StandardCharsets;
import java.util.concurrent.Semaphore;

@Service
@Slf4j
@RequiredArgsConstructor
public class ExportServiceImpl extends BaseServiceImpl implements ExportService {

    public static final Semaphore semaphore = new Semaphore(1);

    private final DatabaseConfig databaseConfig;
    private final MetadataConfig metadataConfig;
    private final DBTicketRepository dbTicketRepository;
    private final ExportRepository exportRepository;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public String exportInstitution(Long institutionId, Integer schoolYear, String mapping) {
        return this.exportRepository.exportInstitution(new TicketData("", institutionId, schoolYear), mapping);
    }

    @Override
    public void exportInstitutionData() {
        try {
            TicketData ticket;
            while (true) {
                try {
                    semaphore.acquire();
                    ticket = dbTicketRepository.getTicketByStatus(TicketStatusEnum.PENDING,
                            TicketTypeEnum.EXPORT);
                    if (ticket != null && ticket.getTicketId() != null) {
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
                        String exportJson = getExportJson(ticket);

                        try {
                            String export = this.exportRepository.exportInstitution(ticket, exportJson);
                            dbTicketRepository.setTicketJson(ticket, export);
                            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.COMPLETED);
                        } catch (Exception e) {
                            log.error("Ticket " + ticket.getTicketId() + " failed");
                            dbTicketRepository.setTicketStatus(ticket, TicketStatusEnum.LOAD_DATA_FAILED);
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
            String message = "Export task InterruptedException: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.EXPORT, message);
        } catch (Exception e) {
            // t o d o
            String message = "Export task Exception: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.EXPORT, message);
        }
    }

    private String getExportJson(TicketData ticket) {
        FileSystemResource exportMapping = (ticket.getTicketSubType() == TicketSubTypeEnum.ALL ?
                metadataConfig.getExportMappingFile() : metadataConfig.getExportIdMappingFile());
        try {
            if (exportMapping != null) {
                String json = new String(exportMapping.getInputStream().readAllBytes(), StandardCharsets.UTF_8);
                if (!isJSONValid(json)) {
                    String message = ticket.getTicketId() + ": Invalid json format for EXPORT JSON";
                    log.error(message);
                    dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
                    throw new InternalServerErrorException("Invalid json format for EXPORT JSON");
                }
                return json;
            } else {
                String message = ticket.getTicketId() + ": Wrong or missing EXPORT JSON";
                log.error(message);
                dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
                throw new InternalServerErrorException("Wrong or missing EXPORT JSON");
            }
        } catch (Exception e) {
            String message = ticket.getTicketId() + ": Wrong or missing EXPORT JSON (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.EXPORT, message);
            throw new InternalServerErrorException("Wrong or missing EXPORT JSON");
        }
    }
}
