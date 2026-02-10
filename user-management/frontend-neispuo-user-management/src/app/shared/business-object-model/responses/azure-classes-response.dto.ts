export class AzureClassesResponseDTO {
    classID?: number;

    workflowTypeID?: number;

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

    updatedBy?: string;

    guid?: string;

    retryAttempts?: number;

    status?: string;
}
