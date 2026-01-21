import { ParentChildAccessResponseDTO } from '../dto/responses/parent-child-access-response.dto';

export class ParentChildAccessMapper {
    static transform(parentChildAccessObjects: any[]) {
        const result: ParentChildAccessResponseDTO[] = [];
        for (const parentChildAccessObject of parentChildAccessObjects) {
            const elementToBeInserted: ParentChildAccessResponseDTO = {
                parentChildSchoolBookAccessID: parentChildAccessObject.parentChildSchoolBookAccessID,
                parentID: parentChildAccessObject.parentID,
                childID: parentChildAccessObject.childID,
                action: parentChildAccessObject.action,
                hasAccess: parentChildAccessObject.hasAccess,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
