import { HasNeispuoAccess } from 'src/common/constants/enum/has-neispuo-access.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { DTO } from './dto.interface';

export class StudentResponseDTO implements DTO {
    sysUserID?: number;

    personID?: number;

    isAzureUser?: boolean;

    isAzureSynced?: boolean;

    username?: string;

    firstName?: string;

    middleName?: string;

    threeNames?: string;

    lastName?: string;

    schoolBooksCodesID?: string;

    institutionID?: string;

    institutionName?: string;

    townName?: string;

    municipalityName?: string;

    regionName?: string;

    positionName?: string;

    positionID?: number;

    personalID?: string;

    grade?: string;

    phone?: string;

    birthDate?: string;

    publicEduNumber?: string;

    hasNeispuoAccess?: HasNeispuoAccess;

    azureID?: string;

    additionalRole?: RoleEnum;
}
