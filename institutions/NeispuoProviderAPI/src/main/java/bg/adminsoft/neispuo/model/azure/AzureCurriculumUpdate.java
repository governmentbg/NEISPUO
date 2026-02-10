package bg.adminsoft.neispuo.model.azure;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.List;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AzureCurriculumUpdate {

    @JsonProperty("curriculumID")
    Long curriculumId;

    @JsonProperty("personIDsToDelete")
    List<Long> personIDsToDelete;

    @JsonProperty("personIDsToCreate")
    List<Long> personIDsToCreate;

    @JsonProperty("institutionID")
    Long institutionId;

}
