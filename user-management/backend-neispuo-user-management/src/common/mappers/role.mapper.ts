import { RoleResponseDTO } from '../dto/responses/role-response.dto';

export class RoleMapper {
    static transform(roleObjects: any[]) {
        const result: RoleResponseDTO[] = [];
        for (const roleObject of roleObjects) {
            const elementToBeInserted: RoleResponseDTO = {
                personID: roleObject.personID,
                sysUserID: roleObject.sysUserID,
                sysRoleID: roleObject.sysRoleID,
                sysRoleName: roleObject.sysRoleName,
                isRoleFromEducationalState: roleObject.isRoleFromEducationalState,
                institutionID: roleObject.institutionID,
                institutionName: roleObject.institutionName,
                positionID: roleObject.positionID,
                municipalityID: roleObject.municipalityID,
                municipalityName: roleObject.municipalityName,
                regionID: roleObject.regionID,
                regionName: roleObject.regionName,
                budgetingInstitutionID: roleObject.budgetingInstitutionID,
                budgetingInstitutionName: roleObject.budgetingInstitutionName,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
