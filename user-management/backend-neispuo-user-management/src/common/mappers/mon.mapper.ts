import { MonResponseDTO } from '../dto/responses/mon-response.dto';

export class MonMapper {
    static transform(monObjects: any[]) {
        const result: MonResponseDTO[] = [];
        for (const monObject of monObjects) {
            const elementToBeInserted: MonResponseDTO = {
                sysUserID: monObject.sysUserID,
                isAzureUser: monObject.isAzureUser,
                username: monObject.username,
                firstName: monObject.firstName,
                middleName: monObject.middleName,
                lastName: monObject.lastName,
                threeNames: `${monObject.firstName}${` ${monObject.middleName}`}${` ${monObject.lastName}`}`,
                roleName: monObject.roleName,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
