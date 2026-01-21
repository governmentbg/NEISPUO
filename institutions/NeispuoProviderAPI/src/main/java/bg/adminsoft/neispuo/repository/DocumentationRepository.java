package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.TicketStatusResponse;
import bg.adminsoft.neispuo.model.TicketSubTypeEnum;
import bg.adminsoft.neispuo.model.TicketTypeEnum;

public interface DocumentationRepository {

    String exportDocument(String mapping);

}
