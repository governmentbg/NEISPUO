package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.*;

import java.sql.Connection;
import java.util.List;

public interface DBTicketRepository {

    boolean checkInstitutionCurrentYear(TicketData ticket);
    boolean checkListTemplatePeriod(TicketData ticket);
    boolean checkNewYearTransitionPeriod(TicketData ticket);

    String createTicket(Long institutionId, Integer schoolYear, ProviderInfo provider,
                        String data, TicketTypeEnum ticketType, TicketSubTypeEnum ticketSubType,
                        String remoteIpAddress, String userAgent);
    TicketStatusResponse getTicketStatus(String ticketId, boolean withDetails, TicketTypeEnum ticketType);
    TicketData getTicketForAzureSync();
    TicketData getTicketData(String ticketId, TicketTypeEnum ticketType);

    TicketData getTicketByStatus(TicketStatusEnum statusEnum, TicketTypeEnum ticketType);
    void setTicketStatus(TicketData ticket, TicketStatusEnum status, Connection connection);
    void setTicketStatus(TicketData ticket, TicketStatusEnum status);
    void setTicketAzureSync(TicketData ticket, TicketAzureSync status);
    void setTicketJson(TicketData ticket, String json);
    JsonData getJsonByTicket(TicketData ticket);

    void saveIntegrityCheckResult(TicketData ticket, List<ValidationResult> validateResult,
                                  boolean deleteExisting, Connection connection);
    ProviderInfo checkAccess(String certificateThumbprint, TicketData ticket);

    void stopProcessingTickets();
}
