import { PersonnelSchoolBookAccessRequestDTO } from 'src/common/dto/requests/personnel-school-book-access.request.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';

export type EventAttributesMap = {
    [SIEMLogEventType.PRIVILEGE_PERMISSIONS_CHANGED]: null;
    [SIEMLogEventType.PARENT_CREATED]: AzureUsersResponseDTO;
    [SIEMLogEventType.USER_CREATED]: AzureUsersResponseDTO;
    [SIEMLogEventType.USER_UPDATED]: AzureUsersResponseDTO;
    [SIEMLogEventType.USER_DELETED]: AzureUsersResponseDTO;
    [SIEMLogEventType.STUDENT_DISABLE]: AzureUsersResponseDTO;
    [SIEMLogEventType.SCHOOL_BOOK_ACCESS_CHANGE]: PersonnelSchoolBookAccessRequestDTO;
};
