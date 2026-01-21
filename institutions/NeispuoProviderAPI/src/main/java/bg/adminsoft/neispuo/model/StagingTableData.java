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
public class StagingTableData {

    String tableName;
    Short operationType;
    StagingTableKey tableKey;
    Map<String, Object> columns;
}
