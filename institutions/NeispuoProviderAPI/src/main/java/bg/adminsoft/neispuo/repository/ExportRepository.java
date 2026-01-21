package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.TicketData;

public interface ExportRepository {

    String exportInstitution(TicketData ticket, String mapping);

}
