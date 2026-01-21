import { DTO } from './dto.interface';

export class ParentChildAccessResponseDTO implements DTO {
    parentChildSchoolBookAccessID?: number;

    parentID?: number;

    childID?: number;

    hasAccess?: boolean;

    action?: string;
}
