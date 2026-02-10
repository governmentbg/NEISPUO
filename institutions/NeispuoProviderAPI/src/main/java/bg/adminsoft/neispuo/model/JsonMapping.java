package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

import java.util.LinkedHashMap;
import java.util.Map;

@Getter
@Setter
public class JsonMapping {

    @JsonProperty
    private DBOperationType operation;
    @JsonProperty
    private String schema;
    @JsonProperty
    private String table;
    @JsonProperty
    private String primaryKey;
    @JsonProperty
    private String foreignKey;
    @JsonProperty
    private String parentForeignKey;
    @JsonProperty
    private String schoolYear;
    @JsonProperty
    private String institutionLink;
    @JsonProperty
    private AttributeMapping[] attributeMapping;
    @JsonProperty
    Map<String, JsonMapping> children = new LinkedHashMap<>();

    @JsonAnySetter
    public void setParameter(String key, JsonMapping value) {
        children.put(key, value);
    }

    @Getter
    @Setter
    public static class AttributeMapping {
        @JsonProperty
        private String fromAttribute;
        @JsonProperty
        private String toColumn;
    }
}
