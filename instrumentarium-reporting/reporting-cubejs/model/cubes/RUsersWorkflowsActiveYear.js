/* eslint no-undef: "off" */

cube('RUsersWorkflowsActiveYear', {
  sql: 'SELECT * FROM R_Users_Workflows_Active_Year',
  title: 'Потребители за активната година - Синхронизации Azure',
  description:
    'Служи за извършване на справки свързани с данни за потребители за активната година - синхронизации Azure',

  // preAggregations: {
  //   orderByWorkflowType: {
  //     type: 'rollup',
  //     dimensions: [
  //       RUsersWorkflowsActiveYear.workflowType,
  //       RUsersWorkflowsActiveYear.status,
  //       RUsersWorkflowsActiveYear.inProcessing,
  //       RUsersWorkflowsActiveYear.userRole,
  //     ],
  //     measures: [
  //       RUsersWorkflowsActiveYear.countRowID,
  //     ],
  //     indexes: {
  //       r_users_workflows_active_year_index: {
  //         columns: [
  //           RUsersWorkflowsActiveYear.workflowType,
  //           RUsersWorkflowsActiveYear.status,
  //           RUsersWorkflowsActiveYear.inProcessing,
  //           RUsersWorkflowsActiveYear.userRole,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countRowID: {
      type: 'count',
      sql: 'rowID',
      title: 'Брой записи',
      drillMembers: [
        rowID,
        userID,
        workflowType,
        identifier,
        firstName,
        middleName,
        surname,
        password,
        email,
        phone,
        grade,
        schoolId,
        birthDate,
        userRole,
        accountEnabled,
        inProcessing,
        errorMessage,
        createdOn,
        updatedOn,
        guid,
        retryAttempts,
        username,
        status,
        personID,
        deletionType,
        additionalRole,
        hasNeispuoAccess,
        assignedAccountantSchools,
        azureID,
        inProgressResultCount,
        isForArchivation,
        sisAccessSecondaryRole,
        createdBy,
      ],
    },
  },

  dimensions: {
    rowID: {
      sql: 'rowID',
      type: 'number',
      title: 'Идентификатор на записа',
    },
    userID: {
      sql: 'userID',
      type: 'string',
      title: 'Идентификатор на потребителя',
    },
    workflowType: {
      sql: 'workflowType',
      type: 'number',
      title: 'Тип на процеса',
    },
    identifier: {
      sql: 'identifier',
      type: 'string',
      title: 'Идентификатор',
    },
    firstName: {
      sql: 'firstName',
      type: 'string',
      title: 'Име',
    },
    middleName: {
      sql: 'middleName',
      type: 'string',
      title: 'Презиме',
    },
    surname: {
      sql: 'surname',
      type: 'string',
      title: 'Фамилия',
    },
    password: {
      sql: 'password',
      type: 'string',
      title: 'Парола',
    },
    email: {
      sql: 'email',
      type: 'string',
      title: 'Имейл',
    },
    phone: {
      sql: 'phone',
      type: 'string',
      title: 'Телефон',
    },
    grade: {
      sql: 'grade',
      type: 'string',
      title: 'Клас',
    },
    schoolId: {
      sql: 'schoolId',
      type: 'string',
      title: 'Идентификатор на училището',
    },
    birthDate: {
      sql: 'birthDate',
      type: 'time',
      title: 'Дата на раждане',
    },
    userRole: {
      sql: 'userRole',
      type: 'string',
      title: 'Роля на потребителя',
    },
    accountEnabled: {
      sql: 'accountEnabled',
      type: 'number',
      title: 'Активен',
    },
    inProcessing: {
      sql: 'inProcessing',
      type: 'number',
      title: 'В процес на обработка',
    },
    errorMessage: {
      sql: 'errorMessage',
      type: 'string',
      title: 'Текст на грешката',
    },
    createdOn: {
      sql: 'createdOn',
      type: 'time',
      title: 'Дата на създаване',
    },
    updatedOn: {
      sql: 'updatedOn',
      type: 'time',
      title: 'Дата на последно обновяване',
    },
    guid: {
      sql: 'guid',
      type: 'string',
      title: 'Идентификатор на събитието',
    },
    retryAttempts: {
      sql: 'retryAttempts',
      type: 'number',
      title: 'Брой повторения',
    },
    username: {
      sql: 'username',
      type: 'string',
      title: 'Потребителско име',
    },
    status: {
      sql: 'status',
      type: 'number',
      title: 'Статус',
    },
    personID: {
      sql: 'personID',
      type: 'number',
      title: 'Идентификатор (PersonID)',
    },
    deletionType: {
      sql: 'deletionType',
      type: 'number',
      title: 'Тип на изтриване',
    },
    additionalRole: {
      sql: 'additionalRole',
      type: 'number',
      title: 'Допълнителна роля',
    },
    hasNeispuoAccess: {
      sql: 'hasNeispuoAccess',
      type: 'number',
      title: 'Достъп до НЕИСПУО',
    },
    assignedAccountantSchools: {
      sql: 'assignedAccountantSchools',
      type: 'string',
      title: 'Училища на счетоводител',
    },
    azureID: {
      sql: 'azureID',
      type: 'string',
      title: 'Идентификатор в Azure',
    },
    inProgressResultCount: {
      sql: 'inProgressResultCount',
      type: 'number',
      title: 'Брой проверки на статуса',
    },
    isForArchivation: {
      sql: 'isForArchivation',
      type: 'number',
      title: 'За архивиране',
    },
    sisAccessSecondaryRole: {
      sql: 'sisAccessSecondaryRole',
      type: 'number',
      title: 'Код на допълнителна роля',
    },
    createdBy: {
      sql: 'createdBy',
      type: 'string',
      title: 'Създаден от',
    },
  },

  dataSource: 'default',
});
