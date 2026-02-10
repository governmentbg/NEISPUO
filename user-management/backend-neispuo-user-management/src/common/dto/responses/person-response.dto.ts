import { DTO } from './dto.interface';

export class PersonResponseDTO implements DTO {
    personID?: number;

    personalID?: string;

    azureID?: string;

    publicEduNumber?: string;

    firstName?: string;

    middleName?: string;

    lastName?: string;

    permanentAddress?: string;

    permanentTownID?: number;

    currentAddress?: string;

    currentTownID?: number;

    personalIDType?: string;

    nationalityID?: number;

    birthDate?: Date;

    birthPlaceTownID?: number;

    birthPlaceCountry?: string;

    gender?: string;

    schoolBooksCodesID?: number;

    birthPlace?: string;

    sysUserType?: string;
}
