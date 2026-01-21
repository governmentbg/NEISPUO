import { InstitutionResponseDTO } from '../dto/responses/institution-response.dto';

export class InstitutionMapper {
    static transform(institutionObjects: any[]) {
        const result: InstitutionResponseDTO[] = [];
        for (const institutionObject of institutionObjects) {
            const elementToBeInserted: InstitutionResponseDTO = {
                // @TODO there are fields which contain the same information such as
                // sysuserID and principalID please fix these issues
                sysUserID: institutionObject.sysUserID,
                isAzureUser: institutionObject.isAzureUser,
                institutionID: institutionObject.institutionID,
                institutionName: institutionObject.institutionName,
                townName: institutionObject.townName,
                municipalityName: institutionObject.municipalityName,
                regionName: institutionObject.regionName,
                username: institutionObject.username,
                baseSchoolTypeName: institutionObject.baseSchoolTypeName,
                detailedSchoolTypeName: institutionObject.detailedSchoolTypeName,
                financialSchoolTypeName: institutionObject.financialSchoolTypeName,
                description: institutionObject.description,
                principalId: institutionObject.principalId,
                principalName: institutionObject.principalName,
                principalEmail: institutionObject.principalEmail,
                highestGrade: institutionObject.highestGrade,
                lowestGrade: institutionObject.lowestGrade,
                phone: institutionObject.phone,
                areaName: institutionObject.areaName,
                countryName: institutionObject.countryName,
                postalCode: institutionObject.postalCode,
                street: institutionObject.street,
                personID: institutionObject.personID,
                azureID: institutionObject.azureID,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
