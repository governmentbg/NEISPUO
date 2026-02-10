import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { GraphApiClassResponseDTO } from '../graph-api-class-response.dto';

export class GraphApiGetUserClassEnrollmentsResponseDto {
    response: GraphApiClassResponseDTO[];

    status: GraphApiResponseEnum;
}
