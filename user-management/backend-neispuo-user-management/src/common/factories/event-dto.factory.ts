import { UserRoleType } from '../constants/enum/role-type-enum';
import { WorkflowType } from '../constants/enum/workflow-type.enum';
import { AzureClassesResponseDTO } from '../dto/responses/azure-classes-response.dto';
import { AzureUsersResponseDTO } from '../dto/responses/azure-users-response.dto';
import { AzureEnrollmentsResponseDTO } from '../dto/responses/azure-enrollments-response.dto';
import { AzureOrganizationsResponseDTO } from '../dto/responses/azure-organizations-response.dto';
import { AddressEventDto } from '../dto/telelink/address-event-dto';
import { ClassEventDto } from '../dto/telelink/class-event-dto';
import { DeleteClassEventDto } from '../dto/telelink/delete-class-event-dto';
import { DeleteOrgEventDto } from '../dto/telelink/delete-org-event-dto';
import { DeleteUserEventDto } from '../dto/telelink/delete-user-event-dto';
import { EnrollmentEventDto } from '../dto/telelink/enrollment-event-dto';
import { EventDto } from '../dto/telelink/event-dto';
import { OrgEventDto } from '../dto/telelink/org-event-dto';
import { TermEventDto } from '../dto/telelink/term-event-dto';
import { UserEventDto } from '../dto/telelink/user-event-dto';

export class EventDtoFactory {
    static createFromUserResponseDTO(userResponseDTO: AzureUsersResponseDTO) {
        const eventDto = new EventDto();
        let attributes;
        eventDto.rowID = userResponseDTO.rowID;
        eventDto.id = userResponseDTO.guid;
        eventDto.type = WorkflowType[userResponseDTO.workflowType];

        switch (userResponseDTO.workflowType) {
            case WorkflowType.USER_UPDATE: {
                attributes = new UserEventDto();
                attributes.id = userResponseDTO.userID?.toString();
                attributes.firstName = userResponseDTO.firstName;
                attributes.middleName = userResponseDTO.middleName;
                attributes.surname = userResponseDTO.surname;
                attributes.grade = userResponseDTO.grade;
                attributes.birthDate = userResponseDTO.birthDate;
                attributes.hasSisAccess = !!userResponseDTO.hasNeispuoAccess;
                attributes.sisAccessRole = userResponseDTO.additionalRole;
                attributes.sisAccessSecondaryRole = userResponseDTO.sisAccessSecondaryRole;
                attributes.role = UserRoleType[userResponseDTO.userRole];
                attributes.accountEnabled = !!userResponseDTO.accountEnabled;
                attributes.azureId = userResponseDTO.azureID;
                attributes.sisAccountantSchools = userResponseDTO.assignedAccountantSchools?.split(',');
                break;
            }
            case WorkflowType.USER_CREATE: {
                attributes = new UserEventDto();
                attributes.id = userResponseDTO.userID?.toString();
                attributes.identifier = userResponseDTO.identifier;
                attributes.firstName = userResponseDTO.firstName;
                attributes.middleName = userResponseDTO.middleName;
                attributes.surname = userResponseDTO.surname;
                attributes.password = userResponseDTO.password;
                attributes.email = userResponseDTO.email;
                attributes.schoolId = userResponseDTO.schoolId;
                attributes.phone = userResponseDTO.phone;
                attributes.grade = userResponseDTO.grade;
                attributes.birthDate = userResponseDTO.birthDate;
                attributes.role = UserRoleType[userResponseDTO.userRole];
                attributes.hasSisAccess = !!userResponseDTO.hasNeispuoAccess;
                attributes.sisAccessRole = userResponseDTO.additionalRole;
                attributes.sisAccessSecondaryRole = userResponseDTO.sisAccessSecondaryRole;
                attributes.accountEnabled = !!userResponseDTO.accountEnabled;
                attributes.username = userResponseDTO.username;
                break;
            }
            case WorkflowType.USER_DELETE: {
                attributes = new DeleteUserEventDto();
                attributes.role = userResponseDTO.userRole;
                attributes.azureId = userResponseDTO.azureID;
                break;
            }
            default: {
                break;
            }
        }
        eventDto.attributes = attributes;
        return eventDto;
    }

    static createFromOrganizationsResponseDTO(organizationResponseDTO: AzureOrganizationsResponseDTO) {
        const eventDto = new EventDto();
        let attributes;
        eventDto.rowID = organizationResponseDTO.rowID;
        eventDto.id = organizationResponseDTO.guid;
        eventDto.type = WorkflowType[organizationResponseDTO.workflowType];

        switch (organizationResponseDTO.workflowType) {
            case WorkflowType.SCHOOL_UPDATE: {
                attributes = new OrgEventDto();
                const address = new AddressEventDto();
                attributes.id = organizationResponseDTO.organizationID?.toString();
                attributes.name = organizationResponseDTO.name;
                attributes.description = organizationResponseDTO.description;
                attributes.principalId = organizationResponseDTO.principalId;
                attributes.principalName = organizationResponseDTO.principalName;
                attributes.principalEmail = organizationResponseDTO.principalEmail;
                attributes.highestGrade = organizationResponseDTO.highestGrade;
                attributes.lowestGrade = organizationResponseDTO.lowestGrade;
                attributes.azureId = organizationResponseDTO.azureID;
                address.city = organizationResponseDTO.city;
                address.area = organizationResponseDTO.area;
                address.country = organizationResponseDTO.country;
                address.postalCode = organizationResponseDTO.postalCode;
                address.street = organizationResponseDTO.street;
                attributes.address = address;
                break;
            }
            case WorkflowType.SCHOOL_CREATE: {
                attributes = new OrgEventDto();
                const address = new AddressEventDto();
                attributes.id = organizationResponseDTO.organizationID?.toString();
                attributes.name = organizationResponseDTO.name;
                attributes.description = organizationResponseDTO.description;
                attributes.principalId = organizationResponseDTO.principalId;
                attributes.principalName = organizationResponseDTO.principalName;
                attributes.principalEmail = organizationResponseDTO.principalEmail;
                attributes.highestGrade = organizationResponseDTO.highestGrade;
                attributes.lowestGrade = organizationResponseDTO.lowestGrade;
                address.city = organizationResponseDTO.city;
                address.area = organizationResponseDTO.area;
                address.country = organizationResponseDTO.country;
                address.postalCode = organizationResponseDTO.postalCode;
                address.street = organizationResponseDTO.street;
                attributes.address = address;
                break;
            }
            case WorkflowType.SCHOOL_DELETE: {
                attributes = new DeleteOrgEventDto();
                attributes.azureId = organizationResponseDTO.azureID;
                break;
            }
            default: {
                break;
            }
        }
        eventDto.attributes = attributes;
        return eventDto;
    }

    static createFromEnrollmentResponseDTO(enrollmentResponseDto: AzureEnrollmentsResponseDTO) {
        const attributes = new EnrollmentEventDto();
        const eventDto = new EventDto();

        eventDto.rowID = enrollmentResponseDto.rowID;
        attributes.userAzureId = enrollmentResponseDto.userAzureID;
        attributes.classAzureId = enrollmentResponseDto.classAzureID;
        attributes.orgAzureId = enrollmentResponseDto.organizationAzureID;

        eventDto.id = enrollmentResponseDto.guid?.toString();
        eventDto.type = WorkflowType[enrollmentResponseDto.workflowType];
        eventDto.attributes = attributes;
        return eventDto;
    }

    static createFromClassResponseDTO(azureClassesResponseDTO: AzureClassesResponseDTO) {
        const term = new TermEventDto();
        const eventDto = new EventDto();
        let attributes;
        eventDto.rowID = azureClassesResponseDTO.rowID;
        eventDto.id = azureClassesResponseDTO.guid;
        eventDto.type = WorkflowType[azureClassesResponseDTO.workflowType];

        switch (azureClassesResponseDTO.workflowType) {
            case WorkflowType.CLASS_UPDATE: {
                attributes = new ClassEventDto();
                attributes.id = azureClassesResponseDTO.classID?.toString();
                attributes.orgId = azureClassesResponseDTO.orgID;
                attributes.azureId = azureClassesResponseDTO.azureID;
                attributes.title = azureClassesResponseDTO.title;
                term.id = azureClassesResponseDTO.termID?.toString();
                term.name = azureClassesResponseDTO.termName;
                term.startDate = azureClassesResponseDTO.termStartDate;
                term.endDate = azureClassesResponseDTO.termEndDate;
                attributes.term = term;
                break;
            }
            case WorkflowType.CLASS_CREATE: {
                attributes = new ClassEventDto();
                attributes.id = azureClassesResponseDTO.classID?.toString();
                attributes.title = azureClassesResponseDTO.title;
                attributes.classCode = azureClassesResponseDTO.classCode;
                attributes.orgId = azureClassesResponseDTO.orgID;
                term.id = azureClassesResponseDTO.termID?.toString();
                term.name = azureClassesResponseDTO.termName;
                term.startDate = azureClassesResponseDTO.termStartDate;
                term.endDate = azureClassesResponseDTO.termEndDate;
                attributes.term = term;
                break;
            }
            case WorkflowType.CLASS_DELETE: {
                attributes = new DeleteClassEventDto();
                attributes.azureId = azureClassesResponseDTO.azureID;
                break;
            }
            default: {
                break;
            }
        }
        eventDto.attributes = attributes;
        return eventDto;
    }
}
