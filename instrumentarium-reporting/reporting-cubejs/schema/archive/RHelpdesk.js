/* eslint no-undef: "off" */

cube('RHelpdesk', {
    sql: 'SELECT * FROM reporting.R_Helpdesk',
    title: 'Съпорт система',
    description: 'Служи за извършване на справки свързани с данни на Съпорт системата',
  
  
    preAggregations: {
      // Pre-Aggregations definitions go here
      // Learn more here: https://cube.dev/docs/caching/pre-aggregations/getting-started
    },
  
    joins: {},
  
    measures: {
      countIssueID: {
        sql: 'IssueID',
        title: 'Брой заявки',
        type: 'count',
        drillMembers: [
          IssueID, 
          IssueTitle, 
          PriorityName, 
          StatusName, 
          CategoryName, 
          SubCategoryName, 
          IssueIsEscalated, 
          IssuePhone, 
          IssueAssignedToUsername, 
          IssueSubmitterUsername, 
          IssueSubmitterRole, 
          IssueCreateDate, 
          IssueCreatedByUsername, 
          IssueModifyDate, 
          IssueModifiedByUsername, 
          InstitutionId, 
          InsitutionName, 
          IssueSchoolYear
        ]
      },
    },
  
    dimensions: {
      IssueID: {
        sql: 'IssueID',
        type: 'number',
        title: 'Код на заявка',
      },
      IssueTitle: {
        sql: 'IssueTitle',
        type: 'string',
        title: 'Заглавие на заявка',
      },
      IssueDescription: {
        sql: 'IssueDescription',
        type: 'string',
        title: 'Описание на заявка',
      },
      IssuePriorityId: {
        sql: 'IssuePriorityId',
        type: 'number',
        title: 'Приоритет ID',
        shown: false,
      },
      IssueStatusId: {
        sql: 'IssueStatusId',
        type: 'number',
        title: 'Статус ID',
        shown: false,
      },
      IssueCategoryId: {
        sql: 'IssueCategoryId',
        type: 'number',
        title: 'Категория ID',
        shown: false,
      },
      IssueSubcategoryId: {
        sql: 'IssueSubcategoryId',
        type: 'number',
        title: 'Подкатегория ID',
        shown: false,
      },
      PriorityName: {
        sql: 'PriorityName',
        type: 'string',
        title: 'Приоритет',
      },
      PriorityCode: {
        sql: 'PriorityCode',
        type: 'string',
        title: 'Приоритет код',
        shown: false,
      },
      StatusName: {
        sql: 'StatusName',
        type: 'string',
        title: 'Статус',
      },
      StatusCode: {
        sql: 'StatusCode',
        type: 'string',
        title: 'Статус код',
        shown: false,
      },
      CategoryName: {
        sql: 'CategoryName',
        type: 'string',
        title: 'Категория',
      },
      CategoryCode: {
        sql: 'CategoryCode',
        type: 'string',
        title: 'Категория код',
        shown: false,
      },
      SubCategoryName: {
        sql: 'SubCategoryName',
        type: 'string',
        title: 'Подкатегория',
      },
      SubCategoryCode: {
        sql: 'SubCategoryCode',
        type: 'string',
        title: 'Подкатегория код',
        shown: false,
      },
      IssueIsEscalated: {
        sql: 'IssueIsEscalated',
        type: 'string',
        title: 'Ескалирана заявка',
      },
      IssuePhone: {
        sql: 'IssuePhone',
        type: 'string',
        title: 'Телефон',
      },
      IssueAssignedToUsername: {
        sql: 'IssueAssignedToUsername',
        type: 'string',
        title: 'Възложено на',
      },
      IssueSubmitterUsername: {
        sql: 'IssueSubmitterUsername',
        type: 'string',
        title: 'Подател',
      },
      IssueSubmitterRole: {
        sql: 'IssueSubmitterRole',
        type: 'string',
        title: 'Роля на подател',
      },
      IssueCreateDate: {
        sql: 'IssueCreateDate',
        type: 'time',
        title: 'Дата на създаване',
      },
      IssueCreatedByUsername: {
        sql: 'IssueCreatedByUsername',
        type: 'string',
        title: 'Създател',
      }, 
      IssueModifyDate: {
        sql: 'IssueModifyDate',
        type: 'time',
        title: 'Дата на последна промяна',
      }, 
      IssueModifiedByUsername: {
        sql: 'IssueModifiedByUsername',
        type: 'string',
        title: 'Последно редактирал',
      }, 
      InstitutionId: {
        sql: 'InstitutionId',
        type: 'string',
        title: 'Код по админ',
      }, 
      InsitutionName: {
        sql: 'InsitutionName',
        type: 'string',
        title: 'Име на институция',
      }, 
      IssueSchoolYear: {
        sql: 'IssueSchoolYear',
        type: 'string',
        title: 'Учебна година',
      }, 
    },
  
    dataSource: 'default',
  });
  