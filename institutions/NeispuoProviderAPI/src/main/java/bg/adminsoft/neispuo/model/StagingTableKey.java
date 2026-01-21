package bg.adminsoft.neispuo.model;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class StagingTableKey {

    String externalId;
    String externalParentId;
    String externalParentId2;
    Long recordId;

    String parentExternalId;
    Long parentRecordId;

}
