export class AzureEnrollmentsResponseDTO {
    enrollemntID?: number;

    workflowTypeID?: number;

    userAzureID?: number;

    classAzureID?: number;

    organizationAzureID?: number;

    inProcessing?: number;

    errorMessage?: string;

    createdOn?: Date;

    updatedOn?: Date;

    updatedBy?: string;

    guid?: string;

    retryAttempts?: number;

    status?: string;
}
