import { ActionTypeEnum } from 'src/common/constants/enum/action-type.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';
import { ActionClassDto } from './action-class-dto';
import { ActionSchoolDto } from './action-school-dto';
import { ActionUserDto } from './action-user-dto';

export class EventActionDto {
    action: ActionTypeEnum;

    statusCode: TelelinkStatusCodeEnum;

    statusMessage: string;

    data: ActionUserDto | ActionSchoolDto | ActionClassDto;
}
