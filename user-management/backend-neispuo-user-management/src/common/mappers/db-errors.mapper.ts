import { DBErrorsDTO } from '../dto/responses/db-errors.dto';

export class DBErrorsMapper {
    static transform(logObjects: any[]) {
        const result: DBErrorsDTO[] = [];
        for (const DBErrorsDTO of logObjects) {
            const elementToBeInserted: DBErrorsDTO = {
                errorProcedure: DBErrorsDTO.errorProcedure,
                data: DBErrorsDTO.data,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
