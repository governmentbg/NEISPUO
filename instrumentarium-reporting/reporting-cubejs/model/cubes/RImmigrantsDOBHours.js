/* eslint no-undef: "off" */

cube('RImmigrantsDOBHours', {
  sql: 'SELECT * FROM RImmigrantsDOBHours',
  title: 'Часове ДОБ мигранти',
  description: 'Служи за извършване на справки свързани с часове ДОБ мигранти',

  // preAggregations: {
  //   // Pre-Aggregations definitions go here
  //   // Learn more here:
  //   // 1. https://cube.dev/docs/caching/pre-aggregations/getting-started
  //   // 2. https://cube.dev/docs/product/configuration/data-sources/clickhouse
  // },

  joins: {},

  measures: {
    sumByHoursWeekly: {
      type: 'sum',
      title: 'Сумиране по "Брой часове"',
      sql: 'HoursWeekly',
      drillMembers: [
        InstitutionID,
        InstitutionName,
        RegionName,
        MunicipalityName,
        TownName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        HoursWeekly,
      ],
    },
    count: {
      type: 'count',
      title: 'Брой "Институции"',
      sql: 'InstitutionID',
      drillMembers: [
        InstitutionID,
        InstitutionName,
        RegionName,
        MunicipalityName,
        TownName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        HoursWeekly,
      ],
    },
  },

  dimensions: {
    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    InstitutionName: {
      sql: 'InstitutionName',
      type: 'string',
      title: 'Пълно наименование',
    },

    InstitutionAbbreviation: {
      sql: 'InstitutionAbbreviation',
      type: 'string',
      title: 'Кратко наименование',
    },

    BaseSchoolTypeName: {
      sql: 'BaseSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.37',
    },

    BaseSchoolTypeID: {
      sql: 'BaseSchoolTypeID',
      type: 'number',
      title: 'Вид по чл.37 - код',
      shown: false,
    },

    DetailedSchoolTypeName: {
      sql: 'DetailedSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.38 (детайлен)',
    },

    DetailedSchoolTypeID: {
      sql: 'DetailedSchoolTypeID',
      type: 'number',
      title: 'Вид по чл.38 (детайлен) - код',
    },

    FinancialSchoolTypeName: {
      sql: 'FinancialSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.35-36 (според собствеността)',
    },

    FinancialSchoolTypeID: {
      sql: 'FinancialSchoolTypeID',
      type: 'number',
      title: 'Вид по чл.35-36 (според собствеността) - код',
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

    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код финансираща институция',
      shown: false,
    },

    Lecturer: {
      sql: 'Lecturer',
      type: 'string',
      title: 'Преподавател',
    },

    HoursWeekly: {
      sql: 'HoursWeekly',
      type: 'number',
      title: 'Брой часове',
    },
  },

  dataSource: 'default',
});
