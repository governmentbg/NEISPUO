import { DTO } from './responses/dto.interface';

export class EntitiesInGenerationDTO implements DTO {
    identifier?: string;

    createdOn?: Date;
}
