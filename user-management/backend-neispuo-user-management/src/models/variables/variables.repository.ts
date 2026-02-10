import { Injectable } from '@nestjs/common';
import { VariablesResponseDTO } from 'src/common/dto/responses/variables-response.dto';
import { VariablesMapper } from 'src/common/mappers/job-variables.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class VariablesRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getAllVariables() {
        const jobVariablesResult = await getManager().query(
            `
        SELECT 
            Name as name,
            Value as value
        FROM
            azure_temp.Variables
        `,
        );
        const transformedResult: VariablesResponseDTO[] = VariablesMapper.transform(jobVariablesResult);
        return transformedResult;
    }
}
