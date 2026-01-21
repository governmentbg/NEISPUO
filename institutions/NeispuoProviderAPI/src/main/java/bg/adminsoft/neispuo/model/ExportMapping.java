package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

import java.util.LinkedHashMap;
import java.util.Map;

@Getter
@Setter
public class ExportMapping {

    @JsonProperty
    private String schema;
    @JsonProperty
    private String table;
    @JsonProperty
    private String primaryKey;
    @JsonProperty
    private String foreignKey;
    @JsonProperty
    private ColumnMapping[] columnMapping;
    @JsonProperty
    String filter;
    @JsonProperty
    Map<String, ExportMapping> children = new LinkedHashMap<>();

    @JsonAnySetter
    public void setParameter(String key, ExportMapping value) {
        children.put(key, value);
    }

    @Getter
    @Setter
    public static class ColumnMapping {
        @JsonProperty
        private String fromColumn;
        @JsonProperty
        private String toAttribute;
    }
}
