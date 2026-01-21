import { Logger } from '@nestjs/common';
import { ArchivableEntity } from '../enums/archivable-entity.enum';

export interface BaseArchivationOptions {
    entity: ArchivableEntity;
    batchSize?: number;
    maxExecutionTimeInSeconds?: number;
    logger?: Logger;
}
