import { HasNeispuoAccess } from 'src/common/constants/enum/has-neispuo-access.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { DTO } from './dto.interface';

export class AzureUsersResponseDTO implements DTO {
    rowID?: number;

    userID?: string;

    workflowType?: number;

    identifier?: string;

    firstName?: string;

    middleName?: string;

    lastName?: string;

    surname?: string;

    password?: string;

    email?: string;

    phone?: string;

    grade?: string;

    schoolId?: string;

    birthDate?: string;

    userRole?: string;

    accountEnabled?: number;

    inProcessing?: number;

    errorMessage?: string;

    createdOn?: Date;

    updatedOn?: Date;

    deletionType?: number;

    guid?: string;

    retryAttempts?: number;

    username?: string;

    status?: string;

    personID?: number;

    additionalRole?: RoleEnum;

    sisAccessSecondaryRole?: RoleEnum;

    hasNeispuoAccess?: HasNeispuoAccess;

    azureID?: string;

    telelinkResponseDto?: any;

    assignedAccountantSchools?: string;

    sysUserID?: number;

    isForArchivation?: number;

    isForRestart?: number;

    inProgressResultCount?: number;
}
