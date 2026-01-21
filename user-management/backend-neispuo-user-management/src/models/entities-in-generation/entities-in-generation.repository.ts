import { Injectable } from '@nestjs/common';
import { EntitiesInGenerationDTO } from 'src/common/dto/entities-in-generation.dto';
import { EntitiesInGenerationMapper } from 'src/common/mappers/entities-in-generation.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class EntitiesInGenerationRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async deleteOldEntitiesInGeneration() {
        await this.connection.query(`
        DELETE FROM "azure_temp"."EntitiesInGeneration" with (
            rowlock,
            updlock,
            readpast
        )
        WHERE 
            1=1
            AND CreatedOn < DATEADD(DAY, -5, GETUTCDATE());
            `);
    }

    async deleteEntitiesInGenerationByIdentifiers(dtos: EntitiesInGenerationDTO[]) {
        const identifiers = dtos.map((dto) => `'${dto.identifier}'`).join(`,`);
        const result = await this.entityManager.query(
            `
            DELETE FROM azure_temp.EntitiesInGeneration with (
                rowlock,
                updlock,
                readpast
            )
            OUTPUT DELETED.Identifier as identifier
            WHERE 
                1=1
                AND Identifier IN (${identifiers})
            `,
        );
        const transformedResult: EntitiesInGenerationDTO[] = EntitiesInGenerationMapper.transform(result);
        return transformedResult[0];
    }

    async insertEntitiesInGeneration(dto: EntitiesInGenerationDTO, entityManager?: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `
            INSERT INTO
                azure_temp.EntitiesInGeneration 
                (
                    Identifier
                )
            OUTPUT 
                INSERTED.Identifier as identifier
            VALUES (
                @0
                );
            `,
            [dto.identifier],
        );
        const transformedResult: EntitiesInGenerationDTO[] = EntitiesInGenerationMapper.transform(result);
        return transformedResult[0];
    }

    async getEntitiesInGenerationByIdentifier(dto: EntitiesInGenerationDTO) {
        const result = await this.entityManager.query(
            `
            SELECT
                *
            FROM
                azure_temp.EntitiesInGeneration e
            WHERE
                e.Identifier = @0
            `,
            [dto.identifier],
        );
        const transformedResult: EntitiesInGenerationDTO[] = EntitiesInGenerationMapper.transform(result);
        return transformedResult;
    }
}
