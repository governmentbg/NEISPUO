/* eslint no-undef: "off" */

cube('RefugeeWithdrawnRequests', {
  sql: 'SELECT * FROM RefugeeWithdrawnRequests',
  title: 'Оттеглени заявления от търсещи или получили закрила',
  description:
    'Служи за извършване на справки свързани с оттеглени заявления от търсещи или получили закрила.',

  // preAggregations: {
  //   // Pre-Aggregations definitions go here
  //   // Learn more here:
  //   // 1. https://cube.dev/docs/caching/pre-aggregations/getting-started
  //   // 2. https://cube.dev/docs/product/configuration/data-sources/clickhouse
  // },

  joins: {},

  measures: {
    countApplicationNumber: {
      type: 'count',
      title: 'Брой "Заявления"',
      sql: 'ApplicationNumber',
      drillMembers: [
        SchoolYear,
        ApplicationNumber,
        FirstName,
        MiddleName,
        LastName,
        TownName,
        MunicipalityName,
        RegionName,
      ],
    },
  },

  dimensions: {
    SchoolYear: {
      sql: 'SchoolYear',
      type: 'number',
      title: 'Учебна година',
    },

    ApplicationNumber: {
      sql: 'ApplicationNumber',
      type: 'number',
      title: 'Номер на заявление',
    },

    CancellationDate: {
      sql: 'CancellationDate',
      type: 'string', // It's explicity converted to varchar in the view so I'll leave it as string
      title: 'Дата на оттеляне',
    },

    CancellationReason: {
      sql: 'CancellationReason',
      type: 'string',
      title: 'Причина за оттегляне',
    },

    CreateDate: {
      sql: 'CreateDate',
      type: 'string', // It's explicity converted to varchar in the view so I'll leave it as string
      title: 'Дата на заявление',
    },

    PersonalIDTypeName: {
      sql: 'PersonalIDTypeName',
      type: 'string',
      title: 'Вид идентификатор',
    },
    FirstName: {
      sql: 'FirstName',
      type: 'string',
      title: 'Име',
    },
    MiddleName: {
      sql: 'MiddleName',
      type: 'string',
      title: 'Презиме',
    },
    LastName: {
      sql: 'LastName',
      type: 'string',
      title: 'Фамилия',
    },

    PersonalIDType: {
      sql: 'PersonalIDType',
      type: 'number',
      title: 'ЕГН/ЛНЧ',
    },

    ClassOrGroup: {
      sql: 'ClassOrGroup',
      type: 'string',
      title: 'Випуск',
    },
    TownID: {
      sql: 'TownID',
      type: 'number',
      title: 'Код населено място',
      shown: false,
    },

    TownName: {
      sql: 'TownName',
      type: 'string',
      title: 'Населено място',
    },

    MunicipalityID: {
      sql: 'MunicipalityID',
      type: 'number',
      title: 'Код община',
      shown: false,
    },

    MunicipalityName: {
      sql: 'MunicipalityName',
      type: 'string',
      title: 'Община',
    },

    RegionID: {
      sql: 'RegionID',
      type: 'number',
      title: 'Код област',
      shown: false,
    },

    RegionName: {
      sql: 'RegionName',
      type: 'string',
      title: 'Област',
    },
  },

  dataSource: 'default',
});
