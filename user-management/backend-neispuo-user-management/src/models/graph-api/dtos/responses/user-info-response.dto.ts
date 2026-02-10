import { GraphApiClassResponseDTO } from 'src/common/dto/graph-api-class-response.dto';
import { GraphApiResponseDTO } from 'src/common/dto/graph-api-response.dto';
import { GraphApiSchoolResponseDTO } from 'src/common/dto/graph-api-school-response.dto';
import { WithExtras } from 'src/common/types/with-extras.type';

export class UserInfoResponseDto {
    profileInfo?: WithExtras<GraphApiResponseDTO>;

    userSchoolEnrollmentsInfo?: WithExtras<GraphApiSchoolResponseDTO>[];

    userClassEnrollmentsInfo?: WithExtras<GraphApiClassResponseDTO>[];
}
