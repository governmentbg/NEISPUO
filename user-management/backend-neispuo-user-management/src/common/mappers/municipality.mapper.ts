import { MunicipalityResponseDTO } from '../dto/responses/municipality-response.dto';

export class MunicipalityMapper {
    static transform(municipalityObjects: any[]) {
        const result: MunicipalityResponseDTO[] = [];
        for (const municipalityObject of municipalityObjects) {
            const elementToBeInserted: MunicipalityResponseDTO = {
                sysUserID: municipalityObject.sysUserID,
                isAzureUser: municipalityObject.isAzureUser,
                username: municipalityObject.username,
                name: municipalityObject.name,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
