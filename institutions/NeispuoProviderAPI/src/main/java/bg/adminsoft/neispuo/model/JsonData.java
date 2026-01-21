package bg.adminsoft.neispuo.model;

import com.jayway.jsonpath.internal.JsonContext;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class JsonData {

    JsonDefaultData defaultData;
    String jsonData;
    JsonContext jsonContext;
}
