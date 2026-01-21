import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { DTO } from './dto.interface';

export class AvailableSchoolBooksResponseDTO implements DTO {
    schoolYear?: number;

    classBookID?: RoleEnum;

    institutionID?: number;

    fullBookName?: string;

    fullBookType?: string;
}
