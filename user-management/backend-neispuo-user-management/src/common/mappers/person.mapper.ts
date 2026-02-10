// mappers are used to convert one object to another

import { PersonResponseDTO } from '../dto/responses/person-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class PersonMapper {
    static transform(personObjects: any[]) {
        const result: PersonResponseDTO[] = [];
        for (const personObject of personObjects) {
            const elementToBeInserted: PersonResponseDTO = {
                personID: personObject.personID,
                personalID: personObject.personalID,
                azureID: personObject.azureID,
                publicEduNumber: personObject.publicEduNumber,
                firstName: personObject.firstName,
                middleName: personObject.middleName,
                lastName: personObject.lastName,
                permanentAddress: personObject.permanentAddress,
                permanentTownID: personObject.permanentTownID,
                currentAddress: personObject.currentAddress,
                currentTownID: personObject.currentTownID,
                personalIDType: personObject.personalIDType,
                nationalityID: personObject.nationalityID,
                birthDate: personObject.birthDate,
                birthPlaceTownID: personObject.birthPlaceTownID,
                birthPlaceCountry: personObject.birthPlaceCountry,
                gender: personObject.gender,
                schoolBooksCodesID: personObject.schoolBooksCodesID,
                birthPlace: personObject.birthPlace,
                sysUserType: personObject.sysUserType,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
