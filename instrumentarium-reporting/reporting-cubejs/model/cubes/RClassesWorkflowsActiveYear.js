/* eslint no-undef: "off" */

cube('RClassesWorkflowsActiveYear', {
  sql: 'SELECT * FROM R_Classes_Workflows_Active_Year',
  title: 'Класове за активната година - Синхронизации Azure',
  description:
    'Служи за извършване на справки свързани с данни за класове за активната година - Синхронизации Azure',

  // preAggregations: {
  //   orderByWorkflowType: {
  //     type: 'rollup',
  //     dimensions: [
  //       RClassesWorkflowsActiveYear.workflowType,
  //       RClassesWorkflowsActiveYear.status,
  //       RClassesWorkflowsActiveYear.inProcessing,
  //     ],
  //     measures: [
  //       RClassesWorkflowsActiveYear.countRowID,
  //     ],
  //     indexes: {
  //       r_classes_workflows_active_year_index: {
  //         columns: [
  //           RClassesWorkflowsActiveYear.workflowType,
  //           RClassesWorkflowsActiveYear.status,
  //           RClassesWorkflowsActiveYear.inProcessing,
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
        classID,
        workflowType,
        title,
        classCode,
        orgID,
        termID,
        termName,
        termStartDate,
        termEndDate,
        inProcessing,
        errorMessage,
        createdOn,
        updatedOn,
        guid,
        retryAttempts,
        status,
        azureID,
        inProgressResultCount,
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
    classID: {
      sql: 'classID',
      type: 'string',
      title: 'Идентификатор на класа',
    },
    workflowType: {
      sql: 'workflowType',
      type: 'number',
      title: 'Тип на процеса',
    },
    title: {
      sql: 'title',
      type: 'string',
      title: 'Име на класа',
    },
    classCode: {
      sql: 'classCode',
      type: 'string',
      title: 'Код на класа',
    },
    orgID: {
      sql: 'orgID',
      type: 'string',
      title: 'Идентификатор на организацията',
    },
    termID: {
      sql: 'termID',
      type: 'number',
      title: 'Идентификатор на срока',
    },
    termName: {
      sql: 'termName',
      type: 'string',
      title: 'Име на срока',
    },
    termStartDate: {
      sql: 'termStartDate',
      type: 'time',
      title: 'Начална дата на срока',
    },
    termEndDate: {
      sql: 'termEndDate',
      type: 'time',
      title: 'Крайна дата на срока',
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
  },

  dataSource: 'default',
});
