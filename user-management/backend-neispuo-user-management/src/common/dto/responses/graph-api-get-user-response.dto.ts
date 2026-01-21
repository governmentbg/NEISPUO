import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { GraphApiResponseDTO } from '../graph-api-response.dto';

export class GraphApiGetUserResponseDto {
    response: GraphApiResponseDTO;

    status: GraphApiResponseEnum;
}
