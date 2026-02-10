import { DTO } from './dto.interface';

export class SysUserResponseDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    isAzureSynced?: number;

    username?: string;

    personID?: number;

    deletedOn?: Date;
}
