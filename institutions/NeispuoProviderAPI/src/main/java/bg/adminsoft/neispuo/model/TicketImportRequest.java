package bg.adminsoft.neispuo.model;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class TicketImportRequest {

    @Schema(required = true, description = "Institution identifier (ID)")
    Long institutionId;
    @Schema(required = true, description = "School year")
    Integer schoolYear;
    @Schema(required = true, description = "Base64 encoded json data")
    String json;

}
