import { AccountantResponseDTO } from '../dto/responses/accountant-response.dto';

export class AccountantMapper {
    static transform(monObjects: any[]) {
        const result: AccountantResponseDTO[] = [];
        for (const monObject of monObjects) {
            const elementToBeInserted: AccountantResponseDTO = {
                sysUserID: monObject.sysUserID,
                isAzureUser: monObject.isAzureUser,
                username: monObject.username,
                firstName: monObject.firstName,
                middleName: monObject.middleName,
                lastName: monObject.lastName,
                threeNames: `${monObject.firstName}${` ${monObject.middleName}`}${` ${monObject.lastName}`}`,
                personID: monObject.personID,
                institutionID: monObject.institutionID,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
