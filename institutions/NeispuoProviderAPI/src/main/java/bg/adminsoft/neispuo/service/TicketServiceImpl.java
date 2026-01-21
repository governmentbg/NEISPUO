package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.model.*;
import bg.adminsoft.neispuo.repository.TicketRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

@Service
@Slf4j
@RequiredArgsConstructor
public class TicketServiceImpl implements TicketService {

    private final TicketRepository ticketRepository;

    @Override
    public boolean checkInstitutionCurrentYear(TicketData ticket) {
        return ticketRepository.checkInstitutionCurrentYear(ticket);
    }

    @Override
    public boolean checkListTemplatePeriod(TicketData ticket) {
        return ticketRepository.checkListTemplatePeriod(ticket);
    }

    @Override
    public boolean checkNewYearTransitionPeriod(TicketData ticket) {
        return ticketRepository.checkNewYearTransitionPeriod(ticket);
    }

    @Override
    public String create(Long institutionId, Integer schoolYear, ProviderInfo provider,
                         String data, TicketTypeEnum ticketType, TicketSubTypeEnum ticketSubType,
                         String remoteIpAddress, String userAgent) {
        return ticketRepository.create(institutionId, schoolYear, provider, data,
                                       ticketType, ticketSubType, remoteIpAddress, userAgent);
    }

    @Override
    public TicketStatusResponse status(String ticketId, boolean withDetails, TicketTypeEnum ticketType) {
        return this.ticketRepository.status(ticketId, withDetails, ticketType);
    }

    @Override
    public TicketData getTicket(String ticketId, TicketTypeEnum ticketType) {
        return this.ticketRepository.getTicket(ticketId, ticketType);
    }

    @Override
    public String data(TicketData ticket, TicketTypeEnum ticketType) {
        return this.ticketRepository.data(ticket, ticketType);
    }

}
