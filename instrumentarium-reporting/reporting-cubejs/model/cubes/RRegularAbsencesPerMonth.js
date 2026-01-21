/* eslint no-undef: "off" */

cube('RRegularAbsencesPerMonth', {
  sql: 'SELECT * FROM R_Regular_Absences_Per_Month',
  title: 'Справка за неизвинените отсъствия по класове за месец, срок и учебна година',
  description:
    'Неизвинени отсъствия по класове, обобщени по месец, срок и учебна година.',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RRegularAbsencesPerMonth.RegionID,
  //       RRegularAbsencesPerMonth.SchoolYear,
  //       RRegularAbsencesPerMonth.InstitutionID,
  //       RRegularAbsencesPerMonth.Term,
  //     ],
  //     measures: [
  //       RRegularAbsencesPerMonth.countMonth,
  //       RRegularAbsencesPerMonth.sumAbsencesPerMonth,
  //       RRegularAbsencesPerMonth.sumAbsencesTerm1,
  //       RRegularAbsencesPerMonth.sumAbsencesTerm2,
  //       RRegularAbsencesPerMonth.sumAbsencesPerYear,
  //     ],
  //     indexes: {
  //       r_regular_absences_per_month_index: {
  //         columns: [
  //           RRegularAbsencesPerMonth.RegionID,
  //           RRegularAbsencesPerMonth.SchoolYear,
  //           RRegularAbsencesPerMonth.InstitutionID,
  //           RRegularAbsencesPerMonth.Term,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countMonth: {
      type: 'count',
      title: 'Брой месеци',
      sql: 'Month',
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
        Month,
        Term,
        AbsencesPerMonth,
        AbsencesTerm1,
        AbsencesTerm2,
        AbsencesPerYear,
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
        Term,
        AbsencesPerMonth,
      ],
    },
    sumAbsencesTerm1: {
      type: 'sum',
      title: 'Общо отсъствия през I срок',
      sql: 'AbsencesTerm1',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        SchoolYear,
        ClassOrGroupName,
        Term,
        AbsencesTerm1,
      ],
    },
    sumAbsencesTerm2: {
      type: 'sum',
      title: 'Общо отсъствия през II срок',
      sql: 'AbsencesTerm2',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        SchoolYear,
        ClassOrGroupName,
        Term,
        AbsencesTerm2,
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
    Month: {
      sql: 'Month',
      type: 'number',
      title: 'Месец',
    },
    Term: {
      sql: 'Term',
      type: 'number',
      title: 'Срок',
    },
    AbsencesPerMonth: {
      sql: 'AbsencesPerMonth',
      type: 'number',
      title: 'Отсъствия за месеца',
    },
    AbsencesTerm1: {
      sql: 'AbsencesTerm1',
      type: 'number',
      title: 'Отсъствия - I срок',
    },
    AbsencesTerm2: {
      sql: 'AbsencesTerm2',
      type: 'number',
      title: 'Отсъствия - II срок',
    },
    AbsencesPerYear: {
      sql: 'AbsencesPerYear',
      type: 'number',
      title: 'Отсъствия за учебната година',
    },
  },

  dataSource: 'default',
});

