/* eslint no-undef: "off" */

cube('RRZIStudents', {
  sql: 'SELECT * FROM reporting.R_RZI_Students',
  title: 'Деца/ученици по възраст за РЗИ',
  description:
    'Извежда списък с Институции и брой записани в учебни паралелки/групи деца/ученици, разпределени по възрастови групи (съгласно изисквания на РЗИ) и пол',

  joins: {},

  measures: {
    sumAllStds: {
      type: 'sum',
      title: 'Сумирай по "Брой деца/ученици"',
      sql: 'AllStds',
      drillMembers: [
        RegionName,
        MunicipalityName,
        TownName,
        RegionID,
        MunicipalityID,
        TownID,
        InstitutionID,
        InstitutionName,
        InstitutionKind,
        BudgetingSchoolTypeID,
        GroupsCount,
        A1_3male,
        A1_3female,
        A4_7malePG,
        A4_7femalePG,
        A7_13maleSch,
        A7_13femaleSch,
        A14_18male,
        A14_18female,
      ],
    },
  },

  dimensions: {
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
    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    InstitutionName: {
      sql: 'InstitutionName',
      type: 'string',
      title: 'Име на институция',
    },
    InstitutionKind: {
      sql: 'InstitutionKind',
      type: 'string',
      title: 'Вид на институция',
    },
    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код на финансираща институция',
      shown: false,
    },
    GroupsCount: {
      sql: 'GroupsCount',
      type: 'number',
      title: 'Общ брой групи/паралелки',
    },
    AllStds: {
      sql: 'AllStds',
      type: 'number',
      title: 'Общ брой деца/ученици',
    },
    A1_3male: {
      sql: 'A1_3male',
      type: 'number',
      title: 'Брой момчета 1-3 години',
    },
    A1_3female: {
      sql: 'A1_3female',
      type: 'number',
      title: 'Брой момичета 1-3 години',
    },
    A4_7malePG: {
      sql: 'A4_7malePG',
      type: 'number',
      title: 'Брой момчета 4-7 години в ПГ',
    },
    A4_7femalePG: {
      sql: 'A4_7femalePG',
      type: 'number',
      title: 'Брой момичета 4-7 години в ПГ',
    },
    A7_13maleSch: {
      sql: 'A7_13maleSch',
      type: 'number',
      title: 'Брой момчета 7-13 години в училище',
    },
    A7_13femaleSch: {
      sql: 'A7_13femaleSch',
      type: 'number',
      title: 'Брой момичета 7-13 години в училище',
    },
    A14_18male: {
      sql: 'A14_18male',
      type: 'number',
      title: 'Брой момчета 14-18 години',
    },
    A14_18female: {
      sql: 'A14_18female',
      type: 'number',
      title: 'Брой момичета 14-18 години',
    },
  },

  dataSource: 'default',
});
