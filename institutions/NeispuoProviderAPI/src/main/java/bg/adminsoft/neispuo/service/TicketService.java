package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.model.*;

public interface TicketService {

    boolean checkInstitutionCurrentYear(TicketData ticket);
    boolean checkListTemplatePeriod(TicketData ticket);
    boolean checkNewYearTransitionPeriod(TicketData ticket);
    String create(Long institutionId, Integer schoolYear, ProviderInfo provider,
                  String data, TicketTypeEnum ticketType, TicketSubTypeEnum ticketSubType,
                  String remoteIpAddress, String userAgent);
    TicketStatusResponse status(String ticketId, boolean withDetails, TicketTypeEnum ticketType);
    TicketData getTicket(String ticketId, TicketTypeEnum ticketType);
    String data(TicketData ticket, TicketTypeEnum ticketType);
}
