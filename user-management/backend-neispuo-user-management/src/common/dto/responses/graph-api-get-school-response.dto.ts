import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { GraphApiSchoolResponseDTO } from '../graph-api-school-response.dto';

export class GraphApiGetSchoolResponseDto {
    response: GraphApiSchoolResponseDTO;

    status: GraphApiResponseEnum;
}
