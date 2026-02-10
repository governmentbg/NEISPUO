import { ArchivationStatus } from '../enums/archivation-status.enum';

export interface ArchivationResult {
    processedCount: number;
    batchCount: number;
    totalRecords: number;
    executionTime: number;
    status: ArchivationStatus;
    error?: string;
}
