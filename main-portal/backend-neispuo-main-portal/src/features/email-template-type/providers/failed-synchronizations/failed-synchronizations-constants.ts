import { ViewConfig } from './interfaces/view-config.interface';
import { ViewName } from './types/view-name.type';

export const VIEW_CONFIG: Record<ViewName, ViewConfig> = {
  AzureOrganizationsView: {
    from: 'azure_temp.AzureOrganizationsView',
    alias: 'organizations',
    workflowTypes: [1, 2, 3],
    extraWhere: [
      `
      NOT EXISTS (
        SELECT 1
        FROM azure_temp.AzureOrganizationsView o2
        WHERE o2.CreatedOn BETWEEN :start AND :end
          AND o2.organizationID = organizations.organizationID
          AND o2.WorkflowType = organizations.WorkflowType
          AND o2.Status = 4
      )`,
    ],
    errorCategoryCase: `
      CASE
        WHEN organizations.ErrorMessage LIKE '%"AddPrincipalToSchool","status":"FAILED"%' 
             THEN 'Failed to add principal to School.'
        WHEN organizations.ErrorMessage LIKE
             '%The DELETE statement conflicted with the REFERENCE constraint "FK_InstitutionPhone_Institution"%' 
             THEN 'The DELETE statement conflicted with the REFERENCE constraint "FK_InstitutionPhone_Institution"'
        ELSE organizations.ErrorMessage
      END`,
  },
  AzureUsersView: {
    from: 'azure_temp.AzureUsersView',
    alias: 'users',
    workflowTypes: [7, 8, 9],
    extraWhere: [
      `
      NOT EXISTS (
        SELECT 1
        FROM azure_temp.AzureUsersView u2
        WHERE u2.CreatedOn BETWEEN :start AND :end
          AND u2.PersonID = users.PersonID
          AND u2.WorkflowType = users.WorkflowType
          AND u2.Status = 4
      )`,
    ],
    errorCategoryCase: `
      CASE
        WHEN users.ErrorMessage LIKE '%An error occurred sending the request.%'
             THEN 'An error occurred sending the request.'
        WHEN users.ErrorMessage LIKE '%Invalid request parameters provided for user%'
             THEN 'Invalid request parameters provided for user'
        WHEN users.ErrorMessage LIKE '%EXCEPTION: {}%' THEN 'EXCEPTION: {}'
        WHEN users.ErrorMessage LIKE '%"Azure Id" must not be empty.%'
             THEN '"Azure Id" must not be empty.'
        WHEN users.ErrorMessage LIKE
             '%AuthenticationFailed: IDX10223: Lifetime validation failed. The token is expired.%'
             THEN 'AuthenticationFailed: IDX10223: Lifetime validation failed. The token is expired.'
        WHEN users.ErrorMessage LIKE
             '%Client network socket disconnected before secure TLS connection was established%'
             THEN 'Client network socket disconnected before secure TLS connection was established'
        WHEN users.ErrorMessage LIKE '%operation timed out for an unknown reason%'
             THEN 'operation timed out for an unknown reason'
        WHEN users.ErrorMessage LIKE '%Cannot read property "sysUserID" of undefined%'
             THEN 'Cannot read property "sysUserID" of undefined'
        WHEN users.ErrorMessage LIKE
             '%User with provided id%not found in AzureAD!%'
             THEN 'User with provided id AZURE_ID not found in AzureAD!'
        WHEN users.ErrorMessage LIKE
             '%Cannot insert duplicate key row in object "core.SysUser" with unique index "uqSysUserUsername".%'
             THEN 'Cannot insert duplicate key row in object "core.SysUser" with unique index "uqSysUserUsername".'
        ELSE users.ErrorMessage
      END`,
  },
  AzureClassesView: {
    from: 'azure_temp.AzureClassesView',
    alias: 'classes',
    workflowTypes: [4, 5, 6],
    extraWhere: [
      `
      NOT EXISTS (
        SELECT 1
        FROM azure_temp.Classes c2
        WHERE c2.CreatedOn BETWEEN :start AND :end
          AND c2.ClassID = classes.ClassID
          AND c2.WorkflowType = classes.WorkflowType
          AND c2.Status = 4
      )`,
    ],
    errorCategoryCase: `
        CASE
        WHEN classes.ErrorMessage LIKE '%Another object with the same value for property mailNickname already exists.%' THEN 'Another object already exists.'
        WHEN classes.ErrorMessage LIKE '%StartDate must be before or equal to EndDate.%' THEN 'StartDate must be before or equal to EndDate.'
        WHEN classes.ErrorMessage LIKE '%More than 3 retries encountered while sending the request.%' THEN 'More Than 3 Retries'
        WHEN classes.ErrorMessage LIKE '%Sequence contains more than one element%' THEN 'Sequence contains more than one element'
        WHEN classes.ErrorMessage LIKE '%Message: Failed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest.%Multiple conflicting Requests for the same group groupid%' THEN 'Failed to Execute Templates; Multiple Conflicting Requests'
        WHEN classes.ErrorMessage LIKE '%Message: Failed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest.%The group provisioning is in Progress%' THEN 'Failed to Execute Templates; The Group Provisioning is in Progress'
        WHEN classes.ErrorMessage LIKE '%Message: Failed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest.%The group is already provisioned%' THEN 'Failed to Execute Templates; The Group is Already Provisioned'
        WHEN classes.ErrorMessage LIKE '%Unable to perform operation as "101" would exceed the maximum quota count "100" for forward-link "owners".%' THEN 'Unable to Perform Operation'
        WHEN classes.ErrorMessage LIKE '%Message: The request timed out.%' THEN 'The Request Timed Out'
        WHEN classes.ErrorMessage LIKE '%Failed to execute backend request.%' THEN 'Failed to Execute Backend Request'
        WHEN classes.ErrorMessage LIKE '%Cannot create a team from group%' THEN 'Cannot Create a Team'
        WHEN classes.ErrorMessage LIKE '%Message: An error occurred sending the request.%' THEN 'An Error Occurred'
        WHEN classes.ErrorMessage LIKE '%Message: A conflicting object with one or more of the specified property values is present in the directory.%' THEN 'User Conflict During Addition'
        WHEN classes.ErrorMessage LIKE '%Message: The group must have at least one owner, hence this owner cannot be removed.%' THEN 'Group Owner Removal Denied'
        WHEN classes.ErrorMessage LIKE '%Message: One or more removed object references do not exist for the following modified properties: "members"%' THEN 'Group Member Reference Not Found'
        WHEN classes.ErrorMessage LIKE '%Message: One or more removed object references do not exist for the following modified properties: "owners"%' THEN 'Group Owner Reference Not Found'
        WHEN classes.ErrorMessage LIKE '%EducationClass with provided id%not found in AzureAD!%' THEN 'Education Class Not Found in Azure AD (DLQ)'
        WHEN classes.ErrorMessage LIKE '%EducationUser with provided id%not found in AzureAD!%' THEN 'Education User Not Found in Azure AD (DLQ)'
        WHEN classes.ErrorMessage LIKE '%User with provided id%not found in AzureAD!%' THEN 'User Not Found in Azure AD (DLQ)'
        WHEN classes.ErrorMessage LIKE '%EducationUser with provided id%and role Teacher already exists%' THEN 'Teacher Role Already Assigned'
        WHEN classes.ErrorMessage LIKE '%EducationUser with provided id%and role Student already exists as member!%' THEN 'Student Role Already Assigned'
        WHEN classes.ErrorMessage LIKE '%Message: A conflicting object with one or more of the specified property values is present%' THEN 'General Conflict Error'
        WHEN classes.ErrorMessage LIKE '%Message: Resource%does not exist or one of its queried reference-property objects%' THEN 'Resource or Reference Not Found'
        WHEN classes.ErrorMessage LIKE '%"message":"Client network socket disconnected before secure TLS connection was established"%' THEN 'Client Network Connection Issue'
        ELSE classes.ErrorMessage
    END
    `,
  },
  AzureEnrollmentsView: {
    from: 'azure_temp.AzureEnrollmentsView',
    alias: 'enrollments',
    extraSelect: ['enrollments.isForArchivation'],
    extraGroupBy: ['enrollments.isForArchivation'],
    workflowTypes: [11, 12, 13, 14, 15],
    extraWhere: [
      `
      NOT EXISTS (
        SELECT 1
        FROM azure_temp.Enrollments e2
        WHERE e2.CreatedOn BETWEEN :start AND :end
          AND e2.userPersonID = enrollments.userPersonID
          AND e2.curriculumID = enrollments.curriculumID
          AND e2.WorkflowType = enrollments.WorkflowType
          AND e2.Status = 4
      )`,
    ],
    errorCategoryCase: `
     CASE
        WHEN enrollments.ErrorMessage LIKE '%More than 3 retries encountered while sending the request.%'
             THEN 'More than 3 retries encountered while sending the request.'
        WHEN enrollments.ErrorMessage LIKE '%Message: Failed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest.%Multiple conflicting Requests for the same group groupid%'
             THEN 'Failed to Execute Templates; Multiple Conflicting Requests'
        WHEN enrollments.ErrorMessage LIKE '%Message: Failed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest.%The group provisioning is in Progress%'
             THEN 'Failed to Execute Templates; The Group Provisioning is in Progress'
        WHEN enrollments.ErrorMessage LIKE '%Message: Failed to execute Templates backend request CreateTeamFromGroupWithTemplateRequest.%The group is already provisioned%'
             THEN 'Failed to Execute Templates; The Group is Already Provisioned'
        WHEN enrollments.ErrorMessage LIKE '%Unable to perform operation as "101" would exceed the maximum quota count "100" for forward-link "owners".%'
             THEN 'Unable to Perform Operation'
        WHEN enrollments.ErrorMessage LIKE '%Message: The request timed out.%'
             THEN 'The Request Timed Out'
        WHEN enrollments.ErrorMessage LIKE '%Failed to execute backend request.%'
             THEN 'Failed to Execute Backend Request'
        WHEN enrollments.ErrorMessage LIKE '%Cannot create a team from group%'
             THEN 'Cannot Create a Team'
        WHEN enrollments.ErrorMessage LIKE '%Message: An error occurred sending the request.%'
             THEN 'An Error Occurred'
        WHEN enrollments.ErrorMessage LIKE '%Message: A conflicting object with one or more of the specified property values is present in the directory.%'
             THEN 'User Conflict During Addition'
        WHEN enrollments.ErrorMessage LIKE '%Message: The group must have at least one owner, hence this owner cannot be removed.%'
             THEN 'Group Owner Removal Denied'
        WHEN enrollments.ErrorMessage LIKE '%Message: One or more removed object references do not exist for the following modified properties: "members"%'
             THEN 'Group Member Reference Not Found'
        WHEN enrollments.ErrorMessage LIKE '%Message: One or more removed object references do not exist for the following modified properties: "owners"%'
             THEN 'Group Owner Reference Not Found'
        WHEN enrollments.ErrorMessage LIKE '%EducationClass with provided id%not found in AzureAD!%'
             THEN 'Education Class Not Found in Azure AD (DLQ)'
        WHEN enrollments.ErrorMessage LIKE '%EducationUser with provided id%not found in AzureAD!%'
             THEN 'Education User Not Found in Azure AD (DLQ)'
        WHEN enrollments.ErrorMessage LIKE '%User with provided id%not found in AzureAD!%'
             THEN 'User Not Found in Azure AD (DLQ)'
        WHEN enrollments.ErrorMessage LIKE '%EducationUser with provided id%and role Teacher already exists%'
             THEN 'Teacher Role Already Assigned'
        WHEN enrollments.ErrorMessage LIKE '%EducationUser with provided id%and role Student already exists as member!%'
             THEN 'Student Role Already Assigned'
        WHEN enrollments.ErrorMessage LIKE '%Message: A conflicting object with one or more of the specified property values is present%'
             THEN 'General Conflict Error'
        WHEN enrollments.ErrorMessage LIKE '%Message: Resource%does not exist or one of its queried reference-property objects%'
             THEN 'Resource or Reference Not Found'
        WHEN enrollments.ErrorMessage LIKE '%"message":"Client network socket disconnected before secure TLS connection was established"%'
             THEN 'Client Network Connection Issue'
        ELSE enrollments.ErrorMessage
    END
    `,
  },
};
