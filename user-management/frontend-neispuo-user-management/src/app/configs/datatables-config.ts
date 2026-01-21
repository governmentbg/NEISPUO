import { CondOperator } from '@nestjsx/crud-request';
import { AzureEnrollmentsResponseDTO } from '@shared/business-object-model/responses/azure-enrollments-response.dto';
import { AzureOrganizationResponseDTO } from '@shared/business-object-model/responses/azure-organizations-response.dto';
import { AzureUsersResponseDTO } from '@shared/business-object-model/responses/azure-users-response.dto';
import { BudgetingInstitutionResponseDTO } from '@shared/business-object-model/responses/budgeting-institution-response.dto';
import { ErrorsResponseDTO } from '@shared/business-object-model/responses/errors-response.dto';
import { InstitutionResponseDTO } from '@shared/business-object-model/responses/institution-response.dto';
import { LoginAuditResponseDTO } from '@shared/business-object-model/responses/login-audit.response.dto';
import { MonResponseDTO } from '@shared/business-object-model/responses/mon-response.dto';
import { MunicipalityResponseDTO } from '@shared/business-object-model/responses/municipality-response.dto';
import { NonSyncedPersonResponseDTO } from '@shared/business-object-model/responses/non-synced-person-response.dto';
import { OtherAzureUsersResponseDTO } from '@shared/business-object-model/responses/other-azure-users-response.dto';
import { RUOResponseDTO } from '@shared/business-object-model/responses/ruo-response.dto';
import { StudentResponseDTO } from '@shared/business-object-model/responses/student-response.dto';
import { TeacherClassesResponseDTO } from '@shared/business-object-model/responses/teacher-classes-response.dto';
import { TeacherResponseDTO } from '@shared/business-object-model/responses/teacher-response.dto';
import { AuditActionEnum } from '@shared/enums/audit-action.enum';
import { EventStatus } from '@shared/enums/event-status.enum';
import { HasAzureIDEnum } from '@shared/enums/has-azure-id.enum';
import { IsAzureSyncedEnum } from '@shared/enums/is-azure-synced.enum';
import { JobStatusEnum } from '@shared/enums/job-status.enum';
import { WorkflowType } from '@shared/enums/workflow-type.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { getAllEnumKeys } from 'enum-for';
import { SelectItem } from 'primeng/api';
import { RoleAudit } from '../layout/content-layout/components/pages/update-roles-page/role-audit/role-audit';

export const EVENT_STATUSES: SelectItem<any>[] = getAllEnumKeys(EventStatus).reduce((arr: any[], key) => {
    // checks if array already has the value
    arr.push({
        label: key,
        value: EventStatus[key as any],
    });
    return arr;
}, []);

export const WORKFLOW_TYPES: SelectItem<any>[] = getAllEnumKeys(WorkflowType).reduce((arr: any[], key: any) => {
    arr.push({
        label: key,
        value: WorkflowType[key],
    });
    return arr;
}, []);
export const AUDIT_ACTIONS: SelectItem<any>[] = getAllEnumKeys(AuditActionEnum).reduce((arr: any[], key: any) => {
    arr.push({
        label: key,
        value: AuditActionEnum[key],
    });
    return arr;
}, []);

export const JOB_STATUSES: SelectItem<any>[] = getAllEnumKeys(JobStatusEnum).reduce((arr: any[], key: any) => {
    if (arr.filter((e) => e.label === JobStatusEnum[key as any]).length === 0) {
        arr.push({
            label: key,
            value: JobStatusEnum[key],
        });
    }
    return arr;
}, []);

export const IS_AZURE_SYNCED: SelectItem<any>[] = getAllEnumKeys(IsAzureSyncedEnum).reduce((arr: any[], key: any) => {
    if (arr.filter((e) => e.label === IsAzureSyncedEnum[key as any]).length === 0) {
        arr.push({
            label: key,
            value: IsAzureSyncedEnum[key],
        });
    }
    return arr;
}, []);

export const HAS_AZURE_ID: SelectItem<any>[] = getAllEnumKeys(HasAzureIDEnum).reduce((arr: any[], key: any) => {
    if (arr.filter((e) => e.label === HasAzureIDEnum[key as any]).length === 0) {
        arr.push({
            label: key,
            value: HasAzureIDEnum[key],
        });
    }
    return arr;
}, []);

export const StudentTableColumnsConfig: ColumnServiceConfig<StudentResponseDTO> = {
    localStorageKey: 'students-list',
    allColumns: [
        {
            field: 'personID',
            header: 'Идентификатор',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'threeNames',
            header: 'Имена',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'publicEduNumber',
            header: 'ЛОН',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'azureID',
            header: 'Код в Azure',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'institutionID',
            header: 'Код по НЕИСПУО',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'institutionName',
            header: 'Име на институция',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'schoolBooksCodesID',
            header: 'Код за дневник',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'hasAzureID',
            header: 'Активен',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: HAS_AZURE_ID },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: [
        'threeNames',
        'publicEduNumber',
        'azureID',
        'institutionName',
        'institutionID',
        'schoolBooksCodesID',
        'hasAzureID',
        'options',
    ],
};

export const InstitutionTableColumnsConfig: ColumnServiceConfig<InstitutionResponseDTO> = {
    localStorageKey: 'institutions-list',
    allColumns: [
        {
            field: 'username',
            header: 'Потребителско име',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'isAzureUser',
            header: 'Azure синхронизиран',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: IS_AZURE_SYNCED },
        },
        {
            field: 'institutionID',
            header: 'Код по НЕИСПУО',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'institutionName',
            header: 'Име',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'baseSchoolTypeName',
            header: 'Вид',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'detailedSchoolTypeName',
            header: 'Вид - детайлен',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'financialSchoolTypeName',
            header: 'Вид - финансиране',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'regionName',
            header: 'Област',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'municipalityName',
            header: 'Община',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'townName',
            header: 'Населено място',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
    ],
    fixedVisibleColumnFields: [
        'username',
        'institutionName',
        'baseSchoolTypeName',
        'regionName',
        'municipalityName',
        'townName',
    ],
};

export const MonTableColumnsConfig: ColumnServiceConfig<MonResponseDTO> = {
    localStorageKey: 'mon-list',
    allColumns: [
        {
            field: 'username',
            header: 'Потребителско име',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'threeNames',
            header: 'Три имена',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'RoleName',
            header: 'Роля',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
    ],
    fixedVisibleColumnFields: ['username', 'threeNames', 'RoleName'],
};

export const ParentTableColumnsConfig: ColumnServiceConfig<MonResponseDTO> = {
    localStorageKey: 'parent-list',
    allColumns: [
        {
            field: 'personID',
            header: 'Идентификатор',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'username',
            header: 'Потребителско име',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'threeNames',
            header: 'Три имена',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'azureID',
            header: 'Код в Azure',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: ['personID', 'username', 'threeNames', 'azureID', 'options'],
};

export const RuoTableColumnsConfig: ColumnServiceConfig<RUOResponseDTO> = {
    localStorageKey: 'ruo-list',
    allColumns: [
        {
            field: 'username',
            header: 'Потребителско име',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'threeNames',
            header: 'Три имена',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'RoleName',
            header: 'Роля',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'regionName',
            header: 'Област',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
    ],
    fixedVisibleColumnFields: ['username', 'threeNames', 'RoleName', 'regionName'],
};

export const MunicipalityTableColumnsConfig: ColumnServiceConfig<MunicipalityResponseDTO> = {
    localStorageKey: 'municipality-list',
    allColumns: [
        {
            field: 'username',
            header: 'Потребителско име',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'municipalityName',
            header: 'Наименование',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
    ],
    fixedVisibleColumnFields: ['username', 'municipalityName'],
};

export const OtherAzureUsersTableColumnsConfig: ColumnServiceConfig<OtherAzureUsersResponseDTO> = {
    localStorageKey: 'others-list',
    allColumns: [
        {
            field: 'username',
            header: 'Потребителско име',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'RoleName',
            header: 'Роля',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
    ],
    fixedVisibleColumnFields: ['username', 'RoleName'],
};

export const BudgetingInstitutionsTableColumnsConfig: ColumnServiceConfig<BudgetingInstitutionResponseDTO> = {
    localStorageKey: 'budgeting-institutions-list',
    allColumns: [
        {
            field: 'sysUserID',
            header: 'Потребител',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'username',
            header: 'Потребителско име',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'budgetingInstitutionName',
            header: 'Наименование',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
    ],
    fixedVisibleColumnFields: ['sysUserID', 'username', 'budgetingInstitutionName'],
};

export const TeacherClassesTableColumnsConfig: ColumnServiceConfig<TeacherClassesResponseDTO> = {
    localStorageKey: 'teacher-classes-list',
    allColumns: [
        {
            field: 'PublicEduNumber',
            header: 'Личен образователен номер',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'FullName',
            header: 'Три имена',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'ClassGroup',
            header: 'Паралелка',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'schoolBooksCodesID',
            header: 'Код за дневник',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'PersonID',
            header: 'Идентификатор',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: ['PublicEduNumber', 'FullName', 'ClassGroup', 'schoolBooksCodesID', 'options'],
};
export const LoginAuditTableConfig: ColumnServiceConfig<LoginAuditResponseDTO> = {
    localStorageKey: 'login-audit-list',
    allColumns: [
        { field: 'LoginAuditID', header: 'Идентификатор', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'SysUserID', header: 'Потребител', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'Username', header: 'Потребителско име', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'SysRoleID', header: 'Роля', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'SysRoleName', header: 'Име на роля', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'InstitutionID', header: 'Институция', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        {
            field: 'InstitutionName',
            header: 'Име на институция',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        { field: 'RegionID', header: 'Област', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'RegionName', header: 'Име на област', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'MunicipalityID', header: 'Община', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        {
            field: 'MunicipalityName',
            header: 'Име на община',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'BudgetingInstitutionID',
            header: 'Финансираща институция',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'BudgetingInstitutionName',
            header: 'Име на финансираща институция',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        { field: 'PositionID', header: 'Позиция', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'IPSource', header: 'IP', filter: { type: 'text', operator: CondOperator.CONTAINS } },
        { field: 'CreatedOn', header: 'Дата', filter: { type: 'date', operator: CondOperator.BETWEEN } },
    ],
    fixedVisibleColumnFields: [
        'Username',
        'SysRoleName',
        'InstitutionID',
        'InstitutionName',
        'RegionName',
        'MunicipalityName',
        'BudgetingInstitutionName',
        'PositionID',
        'IPSource',
        'CreatedOn',
    ],
};

export const AzureClassesTableColumnsConfig: ColumnServiceConfig<AzureEnrollmentsResponseDTO> = {
    localStorageKey: 'workflows-classes-list',
    allColumns: [
        {
            field: 'classID',
            header: 'Клас',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'workflowType',
            header: 'Тип',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: WORKFLOW_TYPES },
        },
        {
            field: 'orgID',
            header: 'Училище',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'createdOn',
            header: 'Дата създаване',
            filter: { type: 'date', operator: CondOperator.BETWEEN },
        },
        {
            field: 'guid',
            header: 'GUID',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'retryAttempts',
            header: 'Брой опити',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'errorMessage',
            header: 'Грешка',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'status',
            header: 'Статус',
            filter: {
                type: 'select',
                operator: CondOperator.EQUALS,
                selectionOptions: EVENT_STATUSES,
            },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: ['classID', 'orgID', 'createdOn', 'guid', 'retryAttempts', 'status', 'options'],
};

export const AzureEnrollmentsTableColumnsConfig: ColumnServiceConfig<AzureEnrollmentsResponseDTO> = {
    localStorageKey: 'workflows-enrollments-list',
    allColumns: [
        {
            field: 'rowID',
            header: 'Идентификатор',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'workflowType',
            header: 'Тип',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: WORKFLOW_TYPES },
        },
        {
            field: 'classAzureID',
            header: 'Клас',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'organizationAzureID',
            header: 'Училище',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'userAzureID',
            header: 'Потребител',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'createdOn',
            header: 'Дата създаване',
            filter: { type: 'date', operator: CondOperator.BETWEEN },
        },
        {
            field: 'guid',
            header: 'GUID',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'retryAttempts',
            header: 'Брой опити',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'errorMessage',
            header: 'Грешка',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'status',
            header: 'Статус',
            filter: {
                type: 'select',
                operator: CondOperator.EQUALS,
                selectionOptions: EVENT_STATUSES,
            },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: [
        'workflowType',
        'classAzureID',
        'organizationAzureID',
        'userAzureID',
        'createdOn',
        'guid',
        'retryAttempts',
        'status',
        'options',
    ],
};

export const AzureOrganizationsTableColumnsConfig: ColumnServiceConfig<AzureOrganizationResponseDTO> = {
    localStorageKey: 'workflows-organizations-list',
    allColumns: [
        {
            field: 'organizationID',
            header: 'Номер',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'workflowType',
            header: 'Тип',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: WORKFLOW_TYPES },
        },
        {
            field: 'createdOn',
            header: 'Дата създаване',
            filter: { type: 'date', operator: CondOperator.BETWEEN },
        },
        {
            field: 'guid',
            header: 'GUID',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'retryAttempts',
            header: 'Брой опити',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'errorMessage',
            header: 'Грешка',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'status',
            header: 'Статус',
            filter: {
                type: 'select',
                operator: CondOperator.EQUALS,
                selectionOptions: EVENT_STATUSES,
            },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: [
        'workflowType',
        'classID',
        'organizationID',
        'userID',
        'createdOn',
        'guid',
        'retryAttempts',
        'status',
        'options',
    ],
};

export const ErrorsTableColumnsConfig: ColumnServiceConfig<ErrorsResponseDTO> = {
    localStorageKey: 'error-list',
    allColumns: [
        {
            field: 'id',
            header: 'Номер',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'timeStamp',
            header: 'Дата',
            filter: { type: 'date', operator: CondOperator.BETWEEN },
        },
        {
            field: 'messageTemplate',
            header: 'Шаблон',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'message',
            header: 'Съобщение',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'exception',
            header: 'Грешка',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'logEvent',
            header: 'Събитие',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
    ],
    fixedVisibleColumnFields: ['id', 'timeStamp', 'message'],
};

export const CreateUserTableColumnsConfig: ColumnServiceConfig<NonSyncedPersonResponseDTO> = {
    localStorageKey: 'create-user-list',
    allColumns: [
        {
            field: 'personID',
            header: 'Идентификатор',
            filter: { hidden: true, type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'firstName',
            header: 'Име',
            filter: { hidden: true, type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'middleName',
            header: 'Презиме',
            filter: { hidden: true, type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'lastName',
            header: 'Фамилия',
            filter: { hidden: true, type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'personalID',
            header: 'ЕГН/ЛНЧ',
            filter: { type: 'text', operator: CondOperator.EQUALS },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: ['personID', 'firstName', 'middleName', 'lastName', 'personalID', 'options'],
};
export const AzureUsersTableColumnsConfig: ColumnServiceConfig<AzureUsersResponseDTO> = {
    localStorageKey: 'workflows-users-list',
    allColumns: [
        {
            field: 'personID',
            header: 'Идентификатор',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'workflowType',
            header: 'Тип',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: WORKFLOW_TYPES },
        },
        {
            field: 'createdOn',
            header: 'Дата създаване',
            filter: { type: 'date', operator: CondOperator.BETWEEN },
        },
        {
            field: 'guid',
            header: 'GUID',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'retryAttempts',
            header: 'Брой опити',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'errorMessage',
            header: 'Грешка',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'status',
            header: 'Статус',
            filter: {
                type: 'select',
                operator: CondOperator.EQUALS,
                selectionOptions: EVENT_STATUSES,
            },
        },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: ['workflowType', 'createdOn', 'guid', 'retryAttempts', 'status', 'options'],
};

export const JobsTableColumnsConfig = [
    {
        field: 'name',
        header: 'Наименование',
        filter: { type: 'text', operator: CondOperator.CONTAINS, hidden: true },
    },
    {
        field: 'cron',
        header: 'Крон',
        filter: { type: 'text', operator: CondOperator.CONTAINS, hidden: true },
    },
    {
        field: 'isActive',
        header: 'Активен',
        filter: {
            type: 'select',
            operator: CondOperator.EQUALS,
            selectionOptions: JOB_STATUSES,
            hidden: true,
        },
    },
    {
        field: 'options',
        header: 'Контроли',
        notSortable: true,
    },
];

export const TeacherTableColumnsConfig: ColumnServiceConfig<TeacherResponseDTO> = {
    localStorageKey: 'teachers-list',
    allColumns: [
        {
            field: 'personID',
            header: 'Идентификатор',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'threeNames',
            header: 'Три имена',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'publicEduNumber',
            header: 'ЛОН',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'azureID',
            header: 'Код в Azure',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'institutionID',
            header: 'Код по НЕИСПУО',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'institutionName',
            header: 'Институция',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'regionName',
            header: 'Област',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'municipalityName',
            header: 'Община',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'townName',
            header: 'Населено място',
            filter: { type: 'text', operator: CondOperator.CONTAINS },
        },
        {
            field: 'hasAzureID',
            header: 'Активен',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: HAS_AZURE_ID },
        },
        // hotfix NUM-428
        // {
        //     field: 'staffTypeName',
        //     header: 'Вид персонал',
        //     filter: { type: 'text', operator: CondOperator.CONTAINS },
        // },
        {
            field: 'options',
            header: 'Контроли',
            notSortable: true,
        },
    ],
    fixedVisibleColumnFields: [
        'threeNames',
        'publicEduNumber',
        'azureID',
        'institutionID',
        'institutionName',
        'regionName',
        'municipalityName',
        'townName',
        'hasAzureID',
        // hotfix NUM-428
        // 'staffTypeName',
        'options',
    ],
};

export const RoleAuditTableConfig: ColumnServiceConfig<RoleAudit> = {
    localStorageKey: 'role-audit-list',
    allColumns: [
        {
            field: 'AuditId',
            header: 'Идентификатор',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'AssignedToSysUsername',
            header: 'Променен потребител',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'AssignedToSysUserID',
            header: 'Идентификатор на променен потребител',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },

        {
            field: 'Username',
            header: 'Променена от',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'SysUserID',
            header: 'Идентификатор на променящ потребител',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'AssignedSysRoleName',
            header: 'Роля',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
        {
            field: 'DateUtc',
            header: 'Добавена на',
            filter: {
                type: 'date',
                operator: CondOperator.BETWEEN,
            },
        },
        {
            field: 'Action',
            header: 'Операция',
            filter: { type: 'select', operator: CondOperator.EQUALS, selectionOptions: AUDIT_ACTIONS },
        },
        {
            field: 'AssignedInstitutionID',
            header: 'Към институция',
            filter: {
                type: 'text',
                operator: CondOperator.CONTAINS,
            },
        },
    ],
    fixedVisibleColumnFields: [
        'AssignedToSysUsername',
        'AssignedToSysUserID',
        'Username',
        'SysUserId',
        'AssignedSysRoleName',
        'DateUtc',
        'Action',
        'AssignedInstitutionID',
    ],
};
export const StudentTableItemsPerPage = 25;
export const TeacherTableItemsPerPage = 25;
export const InstitutionsTableItemsPerPage = 25;
export const MonTableItemsPerPage = 25;
export const MunicipalityTableItemsPerPage = 25;
export const RuoTableItemsPerPage = 25;
export const AzureClassesTableItemsPerPage = 25;
export const AzureEnrollmentsTableItemsPerPage = 25;
export const AzureOrganizationsTableItemsPerPage = 25;
export const AzureUsersTableItemsPerPage = 25;
export const BudgetingInstitutionsTableItemsPerPage = 25;
export const JobsTableItemsPerPage = 25;
