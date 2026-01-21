import { RelativesAccessResponseDTO } from '../dto/responses/relatives-access-response.dto';

export class RelativesAccessMapper {
    static transform(relativesAccessObjects: any[]) {
        const result: RelativesAccessResponseDTO[] = [];
        for (const relativesAccessObject of relativesAccessObjects) {
            const elementToBeInserted: RelativesAccessResponseDTO = {
                parentChildSchoolBookAccessID: relativesAccessObject.parentChildSchoolBookAccessID,
                fullName: relativesAccessObject.fullName,
                personID: relativesAccessObject.personID,
                username: relativesAccessObject.username,
                hasAccess: relativesAccessObject.hasAccess,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
