import { GraphApiResponseDTO } from 'src/common/dto/graph-api-response.dto';
import { GraphApiSchoolResponseDTO } from 'src/common/dto/graph-api-school-response.dto';
import { WithExtras } from 'src/common/types/with-extras.type';

export class OrganizationInfoResponseDto {
    schoolInfo?: WithExtras<GraphApiSchoolResponseDTO>;

    profileInfo?: WithExtras<GraphApiResponseDTO>;
}
