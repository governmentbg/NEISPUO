package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.ValidationTypeEnum;

import java.util.List;
import java.util.Map;

public interface ImportService {

    Map<ValidationTypeEnum, List<String>> importInstitutionJson(TicketData ticket, String validationJson, String mappingJson,
                                                                boolean validateData, boolean importData);
    Map<ValidationTypeEnum, List<String>> importInstitutionSchema(TicketData ticket, String mappingJson);

    void importInstitutionData();
}
