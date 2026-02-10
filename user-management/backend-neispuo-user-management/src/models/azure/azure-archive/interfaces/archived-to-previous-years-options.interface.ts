import { ArchivationMode } from '../enums/archivation-mode.enum';
import { BaseArchivationOptions } from './base-archivation-options.interface';

export interface ArchivedToPreviousYearsOptions extends BaseArchivationOptions {
    mode: ArchivationMode.ARCHIVED_TO_PREVIOUS_YEARS;
    upToDate: Date;
}
