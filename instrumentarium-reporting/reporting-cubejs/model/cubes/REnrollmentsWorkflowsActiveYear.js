/* eslint no-undef: "off" */

cube('REnrollmentsWorkflowsActiveYear', {
  sql: 'SELECT * FROM R_Enrollments_Workflows_Active_Year',
  title: 'Записвания за активната година - Синхронизации Azure',
  description:
    'Служи за извършване на справки свързани с данни за записвания за активната година - синхронизации Azure',

  // preAggregations: {
  //   orderByWorkflowType: {
  //     type: 'rollup',
  //     dimensions: [
  //       REnrollmentsWorkflowsActiveYear.workflowType,
  //       REnrollmentsWorkflowsActiveYear.status,
  //       REnrollmentsWorkflowsActiveYear.inProcessing,
  //       REnrollmentsWorkflowsActiveYear.userRole,
  //     ],
  //     measures: [
  //       REnrollmentsWorkflowsActiveYear.countRowID,
  //     ],
  //     indexes: {
  //       r_enrollments_workflows_active_year_index: {
  //         columns: [
  //           REnrollmentsWorkflowsActiveYear.workflowType,
  //           REnrollmentsWorkflowsActiveYear.status,
  //           REnrollmentsWorkflowsActiveYear.inProcessing,
  //           REnrollmentsWorkflowsActiveYear.userRole,
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
        workflowType,
        userAzureID,
        classAzureID,
        organizationAzureID,
        inProcessing,
        errorMessage,
        createdOn,
        updatedOn,
        guid,
        retryAttempts,
        status,
        inProgressResultCount,
        userPersonID,
        organizationPersonID,
        curriculumID,
        userRole,
        isForArchivation,
      ],
    },
  },

  dimensions: {
    rowID: {
      sql: 'rowID',
      type: 'number',
      title: 'Идентификатор на записа',
    },
    workflowType: {
      sql: 'workflowType',
      type: 'number',
      title: 'Тип на процеса',
    },
    userAzureID: {
      sql: 'userAzureID',
      type: 'string',
      title: 'Идентификатор на потребителя в Azure',
    },
    classAzureID: {
      sql: 'classAzureID',
      type: 'string',
      title: 'Идентификатор на класа в Azure',
    },
    organizationAzureID: {
      sql: 'organizationAzureID',
      type: 'string',
      title: 'Идентификатор на организацията в Azure',
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
    inProgressResultCount: {
      sql: 'inProgressResultCount',
      type: 'number',
      title: 'Брой проверки на статуса',
    },
    userPersonID: {
      sql: 'userPersonID',
      type: 'number',
      title: 'Идентификатор на потребителя',
    },
    organizationPersonID: {
      sql: 'organizationPersonID',
      type: 'number',
      title: 'Идентификатор на организацията',
    },
    curriculumID: {
      sql: 'curriculumID',
      type: 'number',
      title: 'Идентификатор на учебния план',
    },
    userRole: {
      sql: 'userRole',
      type: 'string',
      title: 'Роля на потребителя',
    },
    isForArchivation: {
      sql: 'isForArchivation',
      type: 'number',
      title: 'За архивиране',
    },
  },

  dataSource: 'default',
});
