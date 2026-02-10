import { DTO } from './dto.interface';

export class AzureClassesResponseDTO implements DTO {
    rowID?: number;

    classID?: string;

    workflowType?: number;

    title?: string;

    classCode?: string;

    orgID?: string;

    termID?: number;

    termName?: string;

    termStartDate?: string;

    termEndDate?: string;

    inProcessing?: number;

    errorMessage?: string;

    createdOn?: Date;

    updatedOn?: Date;

    guid?: string;

    retryAttempts?: number;

    status?: string;

    azureID?: string;

    telelinkResponseDto?: any;

    isForArchivation?: number;

    isForRestart?: number;

    inProgressResultCount?: number;
}
