import { DTO } from './dto.interface';

export class SysUserSysRoleResponseDTO implements DTO {
    sysUserID?: number;

    sysRoleID?: number;

    institutionID?: number;

    budgetingInstitutionID?: number;

    municipalityID?: number;

    regionID?: number;
}
