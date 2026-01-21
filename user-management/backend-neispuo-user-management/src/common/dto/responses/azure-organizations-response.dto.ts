import { DTO } from './dto.interface';

export class AzureOrganizationsResponseDTO implements DTO {
    rowID?: number;

    organizationID?: number;

    workflowType?: number;

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

    guid?: string;

    retryAttempts?: number;

    status?: string;

    username?: string;

    password?: string;

    personID?: number;

    sysUserID?: number;

    azureID?: string;

    telelinkResponseDto?: any;

    isForArchivation?: number;

    isForRestart?: number;

    inProgressResultCount?: number;
}
