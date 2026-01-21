/* eslint no-undef: "off" */

cube('RClasses', {
  sql: 'SELECT * FROM inst_basic.R_Classes',
  title: 'Класове',
  description: 'Служи за извършване на справки свързани с данни по класове.',

  preAggregations: {
    // Pre-Aggregations definitions go here
    // Learn more here: https://cube.dev/docs/caching/pre-aggregations/getting-started
  },

  joins: {},

  measures: {
    countClassName: {
      title: 'Брой "Паралелка/група - наименование"',
      type: 'count',
      sql: "ClassName",
      drillMembers: [
        [ClassName, BasicClass, ClassTypeName, EduFormName, ClassEduDurationName, ClassShiftName, FLStudyTypeName, FLName, Address, BudgetingInstitutionName, EntranceLevel, SPPOOProfAreaName, SPPOOProfessionName, SPPOOSpeciality, IsProtected, IsPriority, IsProfModule, Notes, IsCombined, IsSpecNeed, ClassWeigth, SchoolYear, InstitutionID, TownID, MunicipalityID, RegionID, BudgetingSchoolTypeID]
      ],
    },
  },

  dimensions: {
    ClassName: {
      sql: 'ClassName',
      type: 'string',
      title: 'Паралелка/група - наименование',
    },

    BasicClass: {
      sql: 'BasicClass',
      type: 'string',
      title: 'Випуск/Възрастова група',
    },

    ClassTypeName: {
      sql: 'ClassTypeName',
      type: 'string',
      title: 'Вид',
    },

    EduFormName: {
      sql: 'EduFormName',
      type: 'string',
      title: 'Форма на обучение',
    },

    ClassEduDurationName: {
      sql: 'ClassEduDurationName',
      type: 'string',
      title: 'Срок на обучение',
    },

    ClassShiftName: {
      sql: 'ClassShiftName',
      type: 'string',
      title: 'Организация на учебния процес',
    },

    FLStudyTypeName: {
      sql: 'FLStudyTypeName',
      type: 'string',
      title: 'Начин на изучаване на ЧЕ',
    },

    FLName: {
      sql: 'FLName',
      type: 'string',
      title: 'Чужд език',
    },

    Address: {
      sql: 'Address',
      type: 'string',
      title: 'Адрес на обучение',
    },

    BudgetingInstitutionName: {
      sql: 'BudgetingInstitutionName',
      type: 'string',
      title: 'Финансира се от',
    },

    EntranceLevel: {
      sql: 'EntranceLevel',
      type: 'string',
      title: 'Прием след:',
    },

    SPPOOProfAreaName: {
      sql: 'SPPOOProfAreaName',
      type: 'string',
      title: 'Професионално направление',
    },

    SPPOOProfessionName: {
      sql: 'SPPOOProfessionName',
      type: 'string',
      title: 'Професия',
    },

    SPPOOSpeciality: {
      sql: 'SPPOOSpeciality',
      type: 'string',
      title: 'Специалност',
    },

    IsProtected: {
      sql: 'IsProtected',
      type: 'string',
      title: 'Защитена специалност',
    },

    IsPriority: {
      sql: 'IsPriority',
      type: 'string',
      title: 'Приоритетна специалност',
    },

    IsProfModule: {
      sql: 'IsProfModule',
      type: 'string',
      title: 'Обучение по модули',
    },

    Notes: {
      sql: 'Notes',
      type: 'string',
      title: 'Бележки',
    },

    IsCombined: {
      sql: 'IsCombined',
      type: 'string',
      title: 'Слята',
    },

    IsSpecNeed: {
      sql: 'IsSpecNeed',
      type: 'string',
      title: 'Специална/за деца със СОП',
    },

    ClassWeigth: {
      sql: 'ClassWeigth',
      type: 'string',
      title: 'Тежест на паралелката/групата',
    },

    SchoolYear: {
      sql: 'SchoolYear',
      type: 'number',
      title: 'Година',
    },

    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },

    TownID: {
      sql: 'TownID',
      type: 'number',
      title: 'Код населено място',
      shown: false,
    },

    MunicipalityID: {
      sql: 'MunicipalityID',
      type: 'number',
      title: 'Код община',
      shown: false,
    },

    RegionID: {
      sql: 'RegionID',
      type: 'number',
      title: 'Код област',
      shown: false,
    },

    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код финансираща институция',
      shown: false,
    },
  },

  dataSource: 'default',
});
