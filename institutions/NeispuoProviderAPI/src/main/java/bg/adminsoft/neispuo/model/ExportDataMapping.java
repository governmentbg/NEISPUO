package bg.adminsoft.neispuo.model;

import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

import java.util.LinkedHashMap;
import java.util.Map;

@Getter
@Setter
public class ExportDataMapping {

    @JsonProperty
    Map<String, ExportMapping> exportMapping = new LinkedHashMap<>();

    @JsonAnySetter
    public void setParameter(String key, ExportMapping value) {
        exportMapping.put(key, value);
    }

}
