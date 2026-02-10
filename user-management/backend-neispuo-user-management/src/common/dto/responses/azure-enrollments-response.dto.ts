import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { DTO } from './dto.interface';

export class AzureEnrollmentsResponseDTO implements DTO {
    rowID?: number;

    workflowType?: number;

    userAzureID?: string;

    classAzureID?: string;

    organizationAzureID?: string;

    userRole?: UserRoleType;

    curriculumID?: number;

    userPersonID?: number;

    organizationPersonID?: number;

    inProcessing?: number;

    errorMessage?: string;

    createdOn?: Date;

    updatedOn?: Date;

    guid?: string;

    retryAttempts?: number;

    status?: string;

    telelinkResponseDto?: any;

    isForArchivation?: number;

    isForRestart?: number;

    inProgressResultCount?: number;
}
