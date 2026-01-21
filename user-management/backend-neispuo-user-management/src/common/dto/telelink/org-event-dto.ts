import { AddressEventDto } from './address-event-dto';

export class OrgEventDto {
    id: string;

    name: string;

    description: string;

    principalId: string;

    principalName: string;

    principalEmail: string;

    highestGrade: number;

    lowestGrade: number;

    phone: string;

    azureId: string;

    address: AddressEventDto;
}
