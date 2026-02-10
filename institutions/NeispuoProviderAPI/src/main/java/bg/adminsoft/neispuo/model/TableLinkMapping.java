package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class TableLinkMapping {

    @JsonProperty
    private String mainTable;
    @JsonProperty
    private String mainTableKey;
    @JsonProperty
    private String mainLinkTable;
    @JsonProperty
    private String mainLinkTableKey;
    @JsonProperty
    private String childTable;
    @JsonProperty
    private String childTableKey;
    @JsonProperty
    private String childLinkTableKey;

}
