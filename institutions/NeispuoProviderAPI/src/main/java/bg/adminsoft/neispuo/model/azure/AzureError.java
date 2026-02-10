package bg.adminsoft.neispuo.model.azure;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.sql.Timestamp;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AzureError {

    private Long id;

    private String address;

    private String json;

    private Timestamp errorDateTime;


}
