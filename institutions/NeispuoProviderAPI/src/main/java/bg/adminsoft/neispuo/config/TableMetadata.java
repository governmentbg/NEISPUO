package bg.adminsoft.neispuo.config;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Map;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class TableMetadata {

    String schema;
    String tableName;
    String description;
    Map<String, String> columns;
    Map<String, Map<String, Object>> columnsDocumentation;

}
