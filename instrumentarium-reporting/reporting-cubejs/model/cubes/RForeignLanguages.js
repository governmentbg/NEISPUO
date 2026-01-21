/* eslint no-undef: "off" */

cube('RForeignLanguages', {
  sql: 'SELECT * FROM R_foreign_languages',
  title: 'Чужди езици',
  description:
    'Служи за извършване на справки свързани с данни по чужди езици.',

  // preAggregations: {
  //   // Pre-Aggregations definitions go here
  //   // Learn more here:
  //   // 1. https://cube.dev/docs/caching/pre-aggregations/getting-started
  //   // 2. https://cube.dev/docs/product/configuration/data-sources/clickhouse
  // },

  joins: {},

  measures: {
    count: {
      sql: 'InstitutionID',
      type: 'count',
      title: 'Брой "Институции"',
      drillMembers: [
        InstitutionName,
        ClassName,
        CurriculumPartName,
        SubjectName,
        SubjectTypeName,
        FLStudyTypeName,
        FLName,
        RegionName,
        TownName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
      ],
    },
  },

  dimensions: {
    InstitutionName: {
      sql: 'InstitutionName',
      type: 'string',
      title: 'Институция',
    },

    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },

    ClassName: {
      sql: 'ClassName',
      type: 'string',
      title: 'Клас - име',
    },

    ClassType: {
      sql: 'ClassType',
      type: 'string',
      title: 'Профил на паралелката',
    },

    Teacher: {
      sql: 'Teacher',
      type: 'string',
      title: 'Преподавател',
    },

    CurriculumPartName: {
      sql: 'CurriculumPartName',
      type: 'string',
      title: 'Раздел от учебния план - име',
    },

    SubjectName: {
      sql: 'SubjectName',
      type: 'string',
      title: 'Учебен предмет - име',
    },

    SubjectTypeName: {
      sql: 'SubjectTypeName',
      type: 'string',
      title: 'Начин на изучаване - име',
    },

    FLStudyTypeName: {
      sql: 'FLStudyTypeName',
      type: 'string',
      title: 'Начин на изучаване на ЧЕ - име',
    },

    FLName: {
      sql: 'FLName',
      type: 'string',
      title: 'ЧЕ - име',
    },

    RegionName: {
      sql: 'RegionName',
      type: 'string',
      title: 'Област',
    },

    MunicipalityName: {
      sql: 'MunicipalityName',
      type: 'string',
      title: 'Община',
    },

    TownName: {
      sql: 'TownName',
      type: 'string',
      title: 'Населено място',
    },

    LocalAreaName: {
      sql: 'LocalAreaName',
      type: 'string',
      title: 'Район',
    },

    BaseSchoolTypeName: {
      sql: 'BaseSchoolTypeName',
      type: 'string',
      title: 'по чл. 37',
    },

    DetailedSchoolTypeName: {
      sql: 'DetailedSchoolTypeName',
      type: 'string',
      title: 'по чл. 38',
    },

    FinancialSchoolTypeName: {
      sql: 'FinancialSchoolTypeName',
      type: 'string',
      title: 'Финансиране',
    },

    SubjectID: {
      sql: 'SubjectID',
      type: 'number',
      title: 'Учебен предмет - код',
    },

    RegionID: {
      sql: 'RegionID',
      type: 'number',
      title: 'Област - код',
      shown: false,
    },

    MunicipalityID: {
      sql: 'MunicipalityID',
      type: 'number',
      title: 'Община - код',
      shown: false,
    },

    TownID: {
      sql: 'TownID',
      type: 'number',
      title: 'Населено място - код',
      shown: false,
    },
  },

  dataSource: 'default',
});
