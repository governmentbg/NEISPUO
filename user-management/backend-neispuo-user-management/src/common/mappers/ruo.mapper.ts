import { RUOResponseDTO } from '../dto/responses/ruo-response.dto';

export class RUOMapper {
    static transform(RUOObjects: any[]) {
        const result: RUOResponseDTO[] = [];
        for (const RUOObject of RUOObjects) {
            const elementToBeInserted: RUOResponseDTO = {
                sysUserID: RUOObject.sysUserID,
                isAzureUser: RUOObject.isAzureUser,
                username: RUOObject.username,
                firstName: RUOObject.firstName,
                middleName: RUOObject.middleName,
                lastName: RUOObject.lastName,
                threeNames: `${RUOObject.firstName}${` ${RUOObject.middleName}`}${` ${RUOObject.lastName}`}`,
                roleName: RUOObject.roleName,
                regionName: RUOObject.regionName,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
