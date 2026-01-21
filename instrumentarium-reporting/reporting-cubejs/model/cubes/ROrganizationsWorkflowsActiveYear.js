/* eslint no-undef: "off" */

cube('ROrganizationsWorkflowsActiveYear', {
  sql: 'SELECT * FROM R_Organizations_Workflows_Active_Year',
  title: 'Организации за активната година - Синхронизации Azure',
  description:
    'Служи за извършване на справки свързани с данни за организации за активната година - синхронизации Azure',

  // preAggregations: {
  //   orderByWorkflowType: {
  //     type: 'rollup',
  //     dimensions: [
  //       ROrganizationsWorkflowsActiveYear.workflowType,
  //       ROrganizationsWorkflowsActiveYear.status,
  //       ROrganizationsWorkflowsActiveYear.inProcessing,
  //     ],
  //     measures: [
  //       ROrganizationsWorkflowsActiveYear.countRowID,
  //     ],
  //     indexes: {
  //       r_organizations_workflows_active_year_index: {
  //         columns: [
  //           ROrganizationsWorkflowsActiveYear.workflowType,
  //           ROrganizationsWorkflowsActiveYear.status,
  //           ROrganizationsWorkflowsActiveYear.inProcessing,
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
        organizationID,
        workflowType,
        name,
        description,
        principalId,
        principalName,
        principalEmail,
        highestGrade,
        lowestGrade,
        phone,
        city,
        area,
        country,
        postalCode,
        street,
        inProcessing,
        errorMessage,
        createdOn,
        updatedOn,
        guid,
        retryAttempts,
        status,
        username,
        password,
        personID,
        isForArchivation,
        azureID,
        inProgressResultCount,
      ],
    },
  },

  dimensions: {
    rowID: {
      sql: 'rowID',
      type: 'number',
      title: 'Идентификатор на записа',
    },
    organizationID: {
      sql: 'organizationID',
      type: 'string',
      title: 'Идентификатор на организация',
    },
    workflowType: {
      sql: 'workflowType',
      type: 'number',
      title: 'Тип на процеса',
    },
    name: {
      sql: 'name',
      type: 'string',
      title: 'Име',
    },
    description: {
      sql: 'description',
      type: 'string',
      title: 'Описание',
    },
    principalId: {
      sql: 'principalId',
      type: 'string',
      title: 'Идентификатор на директора',
    },
    principalName: {
      sql: 'principalName',
      type: 'string',
      title: 'Име на директора',
    },
    principalEmail: {
      sql: 'principalEmail',
      type: 'string',
      title: 'Имейл на директора',
    },
    highestGrade: {
      sql: 'highestGrade',
      type: 'number',
      title: 'Най-висок клас',
    },
    lowestGrade: {
      sql: 'lowestGrade',
      type: 'number',
      title: 'Най-нисък клас',
    },
    phone: {
      sql: 'phone',
      type: 'string',
      title: 'Телефон',
    },
    city: {
      sql: 'city',
      type: 'string',
      title: 'Град',
    },
    area: {
      sql: 'area',
      type: 'string',
      title: 'Област',
    },
    country: {
      sql: 'country',
      type: 'string',
      title: 'Държава',
    },
    postalCode: {
      sql: 'postalCode',
      type: 'string',
      title: 'Пощенски код',
    },
    street: {
      sql: 'street',
      type: 'string',
      title: 'Улица',
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
    status: {
      sql: 'status',
      type: 'number',
      title: 'Статус',
    },
    username: {
      sql: 'username',
      type: 'string',
      title: 'Потребителско име',
    },
    password: {
      sql: 'password',
      type: 'string',
      title: 'Парола',
    },
    personID: {
      sql: 'personID',
      type: 'number',
      title: 'Идентификатор (PersonID)',
    },
    isForArchivation: {
      sql: 'isForArchivation',
      type: 'number',
      title: 'За архивиране',
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
  },

  dataSource: 'default',
});
