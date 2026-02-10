import { DTO } from './dto.interface';

export class MunicipalityResponseDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    username?: string;

    name?: string;
}
