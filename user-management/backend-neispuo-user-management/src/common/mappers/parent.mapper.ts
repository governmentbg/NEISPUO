import { ParentResponseDTO } from '../dto/responses/parent-response.dto';

export class ParentMapper {
    static transform(parentObjects: any[]) {
        const result: ParentResponseDTO[] = [];
        for (const parentObject of parentObjects) {
            const elementToBeInserted: ParentResponseDTO = {
                parentID: parentObject.parentID,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
