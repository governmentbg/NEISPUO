/* eslint no-undef: "off" */

cube('RPGFamilyReasonAbsences', {
  sql: 'SELECT * FROM R_PG_Family_Reason_Absences',
  title: 'Справка за отсъствия по семейни причини – предучилищни групи (ПГ)',
  description:
    'Отсъствия по семейни причини в ПГ по ден, месец и учебна година.',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RPGFamilyReasonAbsences.RegionID,
  //       RPGFamilyReasonAbsences.SchoolYear,
  //       RPGFamilyReasonAbsences.InstitutionID,
  //     ],
  //     measures: [
  //       RPGFamilyReasonAbsences.countDay,
  //       RPGFamilyReasonAbsences.sumAbsencesPerDay,
  //       RPGFamilyReasonAbsences.sumAbsencesPerMonth,
  //       RPGFamilyReasonAbsences.sumAbsencesPerYear,
  //     ],
  //     indexes: {
  //       r_pg_family_reason_absences_index: {
  //         columns: [
  //           RPGFamilyReasonAbsences.RegionID,
  //           RPGFamilyReasonAbsences.SchoolYear,
  //           RPGFamilyReasonAbsences.InstitutionID,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countDay: {
      type: 'count',
      title: 'Брой дни',
      sql: 'Day',
      drillMembers: [
        RegionID,
        RegionName,
        MunicipalityName,
        TownName,
        InstitutionID,
        InstitutionName,
        DetailedSchoolTypeID,
        BudgetingSchoolTypeID,
        FinancialSchoolTypeID,
        SchoolYear,
        ClassOrGroupName,
        Day,
        Month,
        AbsencesPerDay,
        AbsencesPerMonth,
        AbsencesPerYear,
      ],
    },
    sumAbsencesPerDay: {
      type: 'sum',
      title: 'Общо отсъствия за деня',
      sql: 'AbsencesPerDay',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        SchoolYear,
        ClassOrGroupName,
        Day,
        Month,
        AbsencesPerDay,
      ],
    },
    sumAbsencesPerMonth: {
      type: 'sum',
      title: 'Общо отсъствия за месеца',
      sql: 'AbsencesPerMonth',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        SchoolYear,
        ClassOrGroupName,
        Month,
        AbsencesPerMonth,
      ],
    },
    sumAbsencesPerYear: {
      type: 'sum',
      title: 'Общо отсъствия за учебната година',
      sql: 'AbsencesPerYear',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        SchoolYear,
        ClassOrGroupName,
        AbsencesPerYear,
      ],
    },
  },

  dimensions: {
    RegionID: {
      sql: 'RegionID',
      type: 'number',
      title: 'Код на област',
      shown: false,
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
    DetailedSchoolTypeID: {
      sql: 'DetailedSchoolTypeID',
      type: 'number',
      title: 'Вид по чл. 38 (детайлен)',
      shown: false,
    },
    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Вид по чл. 35-36 (според собствеността)',
      shown: false,
    },
    FinancialSchoolTypeID: {
      sql: 'FinancialSchoolTypeID',
      type: 'number',
      title: 'Финансиращ орган',
      shown: false,
    },
    SchoolYear: {
      sql: 'SchoolYear',
      type: 'number',
      title: 'Учебна година',
    },
    ClassOrGroupName: {
      sql: 'ClassOrGroupName',
      type: 'string',
      title: 'Клас/група',
    },
    Day: {
      sql: 'Day',
      type: 'time',
      title: 'Ден',
    },
    Month: {
      sql: 'Month',
      type: 'number',
      title: 'Месец',
    },
    AbsencesPerDay: {
      sql: 'AbsencesPerDay',
      type: 'number',
      title: 'Отсъствия за деня',
    },
    AbsencesPerMonth: {
      sql: 'AbsencesPerMonth',
      type: 'number',
      title: 'Отсъствия за месеца',
    },
    AbsencesPerYear: {
      sql: 'AbsencesPerYear',
      type: 'number',
      title: 'Отсъствия за учебната година',
    },
  },

  dataSource: 'default',
});


