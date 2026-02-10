package bg.adminsoft.neispuo.model;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Map;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class SchemaTableDescription {

    String schemaName;
    String tableName;
    SchemaTableKey tableKey;
    Map<String, Object> columns;
    Map<String, Object> attributes;
}
