import { DTO } from './dto.interface';

export class NonSyncedPersonResponseDTO implements DTO {
    personID?: number;

    personalID?: string;

    threeNames?: string;

    firstName?: string;

    middleName?: string;

    lastName?: string;
}
