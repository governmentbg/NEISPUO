import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';

export class TelelinkCheckEventResponseDto {
    response: any;

    status: TelelinkCheckStatusResponseEnum;

    statusCode: TelelinkStatusCodeEnum;
}
