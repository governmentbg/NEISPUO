import { DTO } from './dto.interface';

export class AccountantResponseDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    username?: string;

    firstName?: string;

    middleName?: string;

    threeNames?: string;

    lastName?: string;

    institutionID?: number;

    personID?: number;
}
