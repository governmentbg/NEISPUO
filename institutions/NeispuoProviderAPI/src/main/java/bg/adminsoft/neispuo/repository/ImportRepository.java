package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.ValidationResult;

import java.sql.Connection;
import java.util.List;

public interface ImportRepository {

    List<ValidationResult> importInstitutionJson(TicketData ticket, String validationJson, String mappingJson,
                                       boolean validateData, boolean importData);
    boolean validateStaging(TicketData ticket);
    List<ValidationResult> importInstitutionSchema(TicketData ticket, String mappingJson);

}
