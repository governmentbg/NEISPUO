import { DTO } from './dto.interface';

export class RoleResponseDTO implements DTO {
    personID?: number;

    sysUserID?: number;

    sysRoleID?: number;

    sysRoleName?: string;

    isRoleFromEducationalState?: boolean;

    institutionID?: number;

    institutionName?: string;

    positionID?: number;

    municipalityID?: number;

    municipalityName?: string;

    regionID?: number;

    regionName?: string;

    budgetingInstitutionID?: number;

    budgetingInstitutionName?: string;
}
