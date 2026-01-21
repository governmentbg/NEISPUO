import { DTO } from './dto.interface';

export class RUOResponseDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    username?: string;

    firstName?: string;

    middleName?: string;

    threeNames?: string;

    lastName?: string;

    roleName?: string;

    regionName?: string;
}
