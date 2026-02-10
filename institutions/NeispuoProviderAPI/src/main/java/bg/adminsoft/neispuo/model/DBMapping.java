package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

import java.util.LinkedHashMap;
import java.util.Map;

@Getter
@Setter
public class DBMapping {

    @JsonProperty
    private String schema;
    @JsonProperty
    private String primaryKey;
    @JsonProperty
    private String foreignKey;
    @JsonProperty
    private String parentForeignKey;
    @JsonProperty
    private String where;
    @JsonProperty
    private ColumnMapping[] columnMapping;
    @JsonProperty
    Map<String, DBMapping> children = new LinkedHashMap<>();

    @JsonAnySetter
    public void setParameter(String key, DBMapping value) {
        children.put(key, value);
    }

    @Getter
    @Setter
    public static class ColumnMapping {
        @JsonProperty
        private String fromColumn;
        @JsonProperty
        private String toColumn;
    }
}
