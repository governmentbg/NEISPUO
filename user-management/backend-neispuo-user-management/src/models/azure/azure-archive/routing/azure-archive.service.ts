import { Injectable, Logger } from '@nestjs/common';
import { AzureArchiveRepository } from '../azure-archive.repository';
import { ArchivationStatus } from '../enums/archivation-status.enum';
import { ArchivationResult } from '../interfaces/archivation-result.interface';
import { ArchivedToPreviousYearsOptions } from '../interfaces/archived-to-previous-years-options.interface';
import { ArchivationOptions } from '../types/archivation-options.type';

@Injectable()
export class AzureArchiveService {
    private static readonly BATCH_DELAY_MS = 100;

    private static readonly DEFAULT_BATCH_SIZE = 50000;

    private static readonly DEFAULT_TIMEOUT_SECONDS = 3480;

    constructor(private azureArchivationRepository: AzureArchiveRepository) {}

    async archiveEntities(options: ArchivationOptions): Promise<ArchivationResult> {
        const {
            entity,
            mode,
            batchSize = AzureArchiveService.DEFAULT_BATCH_SIZE,
            maxExecutionTimeInSeconds = AzureArchiveService.DEFAULT_TIMEOUT_SECONDS,
            logger = new Logger(AzureArchiveRepository.name),
        } = options;

        const startTime = new Date();
        let processedCount = 0;
        let batchCount = 0;
        let status: ArchivationStatus = ArchivationStatus.COMPLETED;

        try {
            const sourceTable = this.azureArchivationRepository.getSourceTable(entity, mode);
            const targetTable = this.azureArchivationRepository.getTargetTable(entity, mode);
            const whereClause = this.azureArchivationRepository.buildWhereClause(
                mode,
                options as ArchivedToPreviousYearsOptions,
            );

            const total = await this.azureArchivationRepository.getTotalRecords(sourceTable, whereClause);

            while (this.isWithinTimeLimit(startTime, maxExecutionTimeInSeconds)) {
                try {
                    if (!(await this.azureArchivationRepository.hasRecordsToProcess(sourceTable, whereClause))) break;

                    const batchResult = await this.azureArchivationRepository.processBatch(
                        sourceTable,
                        targetTable,
                        whereClause,
                        batchSize,
                    );

                    processedCount += batchResult.processedCount;
                    batchCount++;

                    logger.log(`Processed batch ${batchCount}. Total processed: ${processedCount} of ${total}`);

                    // Small delay to prevent overwhelming the system
                    await new Promise((resolve) => setTimeout(resolve, AzureArchiveService.BATCH_DELAY_MS));
                } catch (error) {
                    logger.error(`Error in batch ${batchCount}:`, error);
                }
            }

            if (!this.isWithinTimeLimit(startTime, maxExecutionTimeInSeconds)) {
                status = ArchivationStatus.TIMEOUT;
                logger.warn(
                    `${entity} '${mode}' archivation stopped due to timeout. Some records may still need processing.`,
                );
            }

            logger.log(
                `${entity} '${mode}' archivation completed. Processed ${processedCount} records in ${batchCount} batches.`,
            );

            return {
                processedCount,
                batchCount,
                totalRecords: total,
                executionTime: (new Date().getTime() - startTime.getTime()) / 1000,
                status,
            };
        } catch (error) {
            status = ArchivationStatus.ERROR;
            logger.error('Archivation failed:', error);
            return {
                processedCount,
                batchCount,
                totalRecords: 0,
                executionTime: (new Date().getTime() - startTime.getTime()) / 1000,
                status,
                error: error.message,
            };
        }
    }

    private isWithinTimeLimit(startTime: Date, maxExecutionTimeInSeconds: number): boolean {
        const elapsedSeconds = (new Date().getTime() - startTime.getTime()) / 1000;
        return elapsedSeconds < maxExecutionTimeInSeconds;
    }
}
