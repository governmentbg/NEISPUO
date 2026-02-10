package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class DBLinkMapping {

    @JsonProperty
    private String fromTable;
    @JsonProperty
    private String fromColumn;
    @JsonProperty
    private String toTable;

}
