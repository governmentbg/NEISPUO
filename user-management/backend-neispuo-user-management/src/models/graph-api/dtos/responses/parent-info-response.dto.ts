import { GraphApiResponseDTO } from 'src/common/dto/graph-api-response.dto';
import { WithExtras } from 'src/common/types/with-extras.type';

export class ParentInfoResponseDto {
    profileInfo?: WithExtras<GraphApiResponseDTO>;
}
