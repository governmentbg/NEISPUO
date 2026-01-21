// mappers are used to convert one object to another

import { SysUserResponseDTO } from '../dto/responses/sys-user.response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class SysUserMapper {
    static transform(sysUserObjects: any[]) {
        const result: SysUserResponseDTO[] = [];
        for (const sysUserObject of sysUserObjects) {
            result.push({
                sysUserID: sysUserObject.sysUserID,
                isAzureUser: sysUserObject.isAzureUser,
                isAzureSynced: sysUserObject.isAzureSynced,
                username: sysUserObject.username,
                personID: sysUserObject.personID,
                deletedOn: sysUserObject.deletedOn,
            });
        }
        return result;
    }
}
