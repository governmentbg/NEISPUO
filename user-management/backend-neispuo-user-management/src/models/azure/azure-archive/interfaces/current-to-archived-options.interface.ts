import { ArchivationMode } from '../enums/archivation-mode.enum';
import { BaseArchivationOptions } from './base-archivation-options.interface';

export interface CurrentToArchivedOptions extends BaseArchivationOptions {
    mode: ArchivationMode.CURRENT_TO_ARCHIVED;
    upToDate: Date;
}
