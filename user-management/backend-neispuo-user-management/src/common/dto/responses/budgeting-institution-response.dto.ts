import { DTO } from './dto.interface';

export class BudgetingInstitutionResponseDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    username?: string;

    name?: string;
}
