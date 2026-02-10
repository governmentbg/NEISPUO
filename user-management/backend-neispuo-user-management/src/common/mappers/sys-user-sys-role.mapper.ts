// mappers are used to convert one object to another

import { SysUserSysRoleResponseDTO } from '../dto/responses/sys-user-sys-role.response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class SysUserSysRoleMapper {
    static transform(sysUserSysRoleObjects: any[]) {
        const result: SysUserSysRoleResponseDTO[] = [];
        for (const sysUserSysRoleObject of sysUserSysRoleObjects) {
            result.push({
                sysUserID: sysUserSysRoleObject.sysUserID,
                sysRoleID: sysUserSysRoleObject.sysRoleID,
                institutionID: sysUserSysRoleObject.institutionID,
                budgetingInstitutionID: sysUserSysRoleObject.budgetingInstitutionID,
                municipalityID: sysUserSysRoleObject.municipalityID,
                regionID: sysUserSysRoleObject.regionID,
            });
        }
        return result;
    }
}
