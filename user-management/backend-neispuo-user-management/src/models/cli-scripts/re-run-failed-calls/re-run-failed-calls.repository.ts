import { Injectable } from '@nestjs/common';
import { DBErrorsDTO } from 'src/common/dto/responses/db-errors.dto';
import { DBErrorsMapper } from 'src/common/mappers/db-errors.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class ReRunFailedCallsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllFailedCalls() {
        const result = await getManager().query(
            `
            SELECT
                ErrorProcedure as errorProcedure,
                Data as data
            FROM
                logs.DB_Errors de
            WHERE
                1=1
                AND UserName = 'AzureCall'
                AND (ErrorMessage LIKE '%operation timed%' OR ErrorMessage IS NULL)
                AND ErrorProcedure IS NOT NULL
            ORDER BY
                ErrorID ASC
            `,
        );
        const transformedResult: DBErrorsDTO[] = DBErrorsMapper.transform(result);
        return transformedResult;
    }
}
