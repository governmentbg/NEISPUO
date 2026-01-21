import { WorkflowType } from '@shared/enums/workflow-type.enum';

export class AzureOrganizationResponseDTO {
    organizationID?: number;

    workflowTypeID?: WorkflowType;

    name?: string;

    description?: string;

    principalId?: string;

    principalName?: string;

    principalEmail?: string;

    highestGrade?: number;

    lowestGrade?: number;

    phone?: string;

    city?: string;

    area?: string;

    country?: string;

    postalCode?: string;

    street?: string;

    inProcessing?: number;

    errorMessage?: string;

    createdOn?: Date;

    updatedOn?: Date;

    updatedBy?: string;

    guid?: string;

    retryAttempts?: number;

    status?: string;
}
