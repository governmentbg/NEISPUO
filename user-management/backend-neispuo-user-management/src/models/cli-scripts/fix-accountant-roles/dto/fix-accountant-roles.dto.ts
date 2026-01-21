import { DTO } from 'src/common/dto/responses/dto.interface';

export class FixAccountantRolesDTO implements DTO {
    personID: string;

    firstName: string;

    middleName: string;

    lastName: string;

    birthDate: string;

    azureID: string;

    userID: string;

    institutionIDs: string;
}
