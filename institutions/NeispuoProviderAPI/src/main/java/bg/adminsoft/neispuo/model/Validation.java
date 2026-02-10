package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.math.BigDecimal;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Set;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class Validation {

    @JsonProperty
    private Boolean required;
    @JsonProperty
    private Boolean isArray;
    @JsonProperty
    private Integer minOccurs;
    @JsonProperty
    private Integer maxOccurs;
    @JsonProperty
    private ColumnValidation[] rules;
    @JsonProperty
    Map<String, Validation> children = new LinkedHashMap<>();

    @JsonAnySetter
    public void setParameter(String key, Validation value) {
        children.put(key, value);
    }

    @Getter
    @Setter
    public static class ColumnValidation {
        @JsonProperty
        private String column;
        @JsonProperty
        private ColumnType dataType;
        @JsonProperty
        private Boolean required;
        @JsonProperty
        private String format;
        @JsonProperty
        private String pattern;
        @JsonProperty
        private Integer minLenght;
        @JsonProperty
        private Integer maxLenght;
        @JsonProperty
        private BigDecimal minValue;
        @JsonProperty
        private BigDecimal maxValue;
        @JsonProperty
        private Set<String> values;
        @JsonProperty
        private ValidationTypeEnum type;
    }
}
