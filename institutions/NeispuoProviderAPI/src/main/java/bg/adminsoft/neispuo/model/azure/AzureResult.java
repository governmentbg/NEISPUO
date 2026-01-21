package bg.adminsoft.neispuo.model.azure;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AzureResult {

    @JsonProperty("status")
    Long status;

    @JsonProperty("message")
    String message;

    @JsonProperty("payload")
    Object payload;

}
