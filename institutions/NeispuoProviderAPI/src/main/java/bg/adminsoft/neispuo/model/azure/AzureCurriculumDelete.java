package bg.adminsoft.neispuo.model.azure;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AzureCurriculumDelete {

    @JsonProperty("curriculumID")
    Long curriculumId;

}
