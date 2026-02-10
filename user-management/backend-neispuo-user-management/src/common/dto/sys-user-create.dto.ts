import { DTO } from './responses/dto.interface';

export class SysUserCreateDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    username?: string;

    personID?: number;
}
