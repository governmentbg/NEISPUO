package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

@Component
@Slf4j
@RequiredArgsConstructor
public class TicketRepositoryImpl implements TicketRepository {

    private final DBTicketRepository dbTicketRepository;

    @Override
    public boolean checkInstitutionCurrentYear(TicketData ticket) {
        return dbTicketRepository.checkInstitutionCurrentYear(ticket);
    }

    @Override
    public boolean checkListTemplatePeriod(TicketData ticket) {
        return dbTicketRepository.checkListTemplatePeriod(ticket);
    }

    @Override
    public boolean checkNewYearTransitionPeriod(TicketData ticket) {
        return dbTicketRepository.checkNewYearTransitionPeriod(ticket);
    }

    @Override
    public String create(Long institutionId, Integer schoolYear, ProviderInfo provider,
                         String data, TicketTypeEnum ticketType, TicketSubTypeEnum ticketSubType,
                         String remoteIpAddress, String userAgent) {
        return dbTicketRepository.createTicket(institutionId, schoolYear, provider,
                                               data, ticketType, ticketSubType,
                                               remoteIpAddress, userAgent);
    }

    @Override
    public TicketStatusResponse status(String ticketId, boolean withDetails, TicketTypeEnum ticketType) {
        return dbTicketRepository.getTicketStatus(ticketId, withDetails, ticketType);
    }

    @Override
    public TicketData getTicket(String ticketId, TicketTypeEnum ticketType) {
        return dbTicketRepository.getTicketData(ticketId, ticketType);
    }

    @Override
    public String data(TicketData ticket, TicketTypeEnum ticketType) {
        return dbTicketRepository.getJsonByTicket(ticket).getJsonData();
    }


}
