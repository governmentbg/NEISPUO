package bg.adminsoft.neispuo.model;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Collections;
import java.util.List;
import java.util.Map;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class TicketStatusResponse {
    @Schema(description = "Ticket process status: " +
            "PENDING, IN_PROGRESS, TECHNICAL_VALIDATION_FAILED, LOGICAL_VALIDATION_FAILED, " +
            "LOAD_DATA_FAILED, STOPPED, COMPLETED")
    TicketStatusEnum code;
    @Schema(description = "Ticket process status description")
    String message;
    @Schema(description = "List of warnings if exist")
    List<String> warnings;
    @Schema(description = "List of errors if exist")
    List<String> errors;

    public TicketStatusResponse(TicketStatusEnum code, String message, Map<ValidationTypeEnum, List<String>> details) {
        this.code = code;
        this.message = message;
        this.warnings = details != null ? details.get(ValidationTypeEnum.WARNING) : Collections.emptyList();
        this.errors = details != null ? details.get(ValidationTypeEnum.ERROR) : Collections.emptyList();
    }
}
