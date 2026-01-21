/* eslint no-undef: "off" */

cube('RefugeesSearchingOrReceivedAdmission', {
  sql: 'SELECT * FROM RefugeesSearchingOrReceivedAdmission',
  title: 'Записани в институция търсещи или получили закрила',
  description:
    'Служи за извършване на справки свързани с данни на записани в институция търсещи или получили закрила.',

  // preAggregations: {
  //   // Pre-Aggregations definitions go here
  //   // Learn more here:
  //   // 1. https://cube.dev/docs/caching/pre-aggregations/getting-started
  //   // 2. https://cube.dev/docs/product/configuration/data-sources/clickhouse
  // },

  joins: {},

  measures: {
    sumStudentCount: {
      type: 'sum',
      title: 'Сумирай по "Брой деца/ученици"',
      sql: 'StudentCount',
      drillMembers: [
        SchoolYear,
        StudentCount,
        InstitutionID,
        InstitutionAbbreviation,
        TownName,
        MunicipalityName,
        RegionName,
      ],
    },
    countInstitutionID: {
      type: 'count',
      title: 'Брой "Институции"',
      sql: 'InstitutionID',
      drillMembers: [
        StudentCount,
        SchoolYear,
        StudentCount,
        InstitutionID,
        InstitutionAbbreviation,
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

    StudentCount: {
      sql: 'StudentCount',
      type: 'number',
      title: 'Брой деца/ученици',
    },

    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },

    InstitutionAbbreviation: {
      sql: 'InstitutionAbbreviation',
      type: 'string',
      title: 'Кратко наименование',
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
    RegionID: {
      sql: 'RegionID',
      type: 'number',
      title: 'Код на област',
      shown: false,
    },

    MunicipalityID: {
      sql: 'MunicipalityID',
      type: 'number',
      title: 'Код на община',
      shown: false,
    },

    TownID: {
      sql: 'TownID',
      type: 'number',
      title: 'Код на населено място',
      shown: false,
    },

    FinancialSchoolTypeName: {
      sql: 'FinancialSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.35-36 (според собствеността)',
    },
  },

  dataSource: 'default',
});
