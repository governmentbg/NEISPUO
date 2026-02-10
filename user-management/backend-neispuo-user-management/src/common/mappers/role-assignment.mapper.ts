import { RoleAssignmentResponseDTO } from '../dto/responses/role-assignment-response.dto';

export class RoleAssignmentMapper {
    static transform(roleAssignmentObjects: any[]) {
        const result: RoleAssignmentResponseDTO[] = [];
        for (const roleAssignmentObject of roleAssignmentObjects) {
            const elementToBeInserted: RoleAssignmentResponseDTO = {
                sysUserID: roleAssignmentObject.sysUserID,
                institutionID: roleAssignmentObject.institutionID,
                sysRoleID: roleAssignmentObject.sysRoleID,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
