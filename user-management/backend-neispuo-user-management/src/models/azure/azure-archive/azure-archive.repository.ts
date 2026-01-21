import { Injectable } from '@nestjs/common';
import { Connection, EntityManager } from 'typeorm';
import { ArchivableEntity } from './enums/archivable-entity.enum';
import { ArchivationMode } from './enums/archivation-mode.enum';
import { ArchivedToPreviousYearsOptions } from './interfaces/archived-to-previous-years-options.interface';

@Injectable()
export class AzureArchiveRepository {
    constructor(private connection: Connection) {}

    getSourceTable(entity: ArchivableEntity, mode: ArchivationMode): string {
        return mode === ArchivationMode.CURRENT_TO_ARCHIVED
            ? `[azure_temp].[${entity}]`
            : `[azure_temp].[${entity}Archived]`;
    }

    getTargetTable(entity: ArchivableEntity, mode: ArchivationMode): string {
        return mode === ArchivationMode.CURRENT_TO_ARCHIVED
            ? `[azure_temp].[${entity}Archived]`
            : `[azure_temp].[${entity}ArchivedPreviousYears]`;
    }

    buildWhereClause(mode: ArchivationMode, options: ArchivedToPreviousYearsOptions): string {
        if (mode === ArchivationMode.CURRENT_TO_ARCHIVED) {
            const { upToDate } = options;
            return `WHERE (IsForArchivation = 1 OR CreatedOn <= '${upToDate.toISOString()}')`;
        } else {
            const { upToDate } = options;
            return `WHERE CreatedOn <= '${upToDate.toISOString()}'`;
        }
    }

    async getTotalRecords(sourceTable: string, whereClause: string): Promise<number> {
        const countQuery = `
            SELECT COUNT(*) as total
            FROM ${sourceTable}
            ${whereClause}
        `;
        const [{ total }] = await this.connection.query(countQuery);
        return total;
    }

    async hasRecordsToProcess(sourceTable: string, whereClause: string): Promise<boolean> {
        const query = `
            SELECT 
            TOP 1
            1 AS HasRecords
            FROM ${sourceTable}
            ${whereClause}
        `;
        const result = await this.connection.query(query);
        return result[0]?.HasRecords === 1;
    }

    async processBatch(
        sourceTable: string,
        targetTable: string,
        whereClause: string,
        batchSize: number,
    ): Promise<{ processedCount: number }> {
        return this.connection.transaction(async (transactionalEntityManager: EntityManager) => {
            const tempTableName = `#TempBatch_${Date.now()}`;

            const createAndInsertQuery = `
                SET NOCOUNT ON;
                
                DECLARE @ProcessedCount int = 0;

                SELECT TOP (${batchSize}) *
                INTO ${tempTableName}
                FROM ${sourceTable} WITH (UPDLOCK, READPAST, ROWLOCK)
                ${whereClause};

                SET @ProcessedCount = @@ROWCOUNT;

                IF @ProcessedCount > 0
                BEGIN
                    INSERT INTO ${targetTable}
                    SELECT * FROM ${tempTableName};

                    DELETE t
                    FROM ${sourceTable} t
                    INNER JOIN ${tempTableName} s ON t.RowID = s.RowID;

                    DROP TABLE ${tempTableName};
                END

                SELECT @ProcessedCount as ProcessedCount;
            `;

            const result = await transactionalEntityManager.query(createAndInsertQuery);
            const processedCount = result[0]?.ProcessedCount || 0;

            return {
                processedCount,
            };
        });
    }
}
