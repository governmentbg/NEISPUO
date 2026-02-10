import { DTO } from './responses/dto.interface';

export class GraphApiClassResponseDTO implements DTO {
    id: string;

    description: string;

    displayName: string;

    mailNickname: string;

    externalName: string;

    externalId: string;
}
