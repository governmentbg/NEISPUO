/* eslint no-undef: "off" */

cube('RRegularFamilyReasonAbsences', {
  sql: 'SELECT * FROM R_Regular_Family_Reason_Absences',
  title: 'Справка за отсъствия по семейни причини - класове',
  description:
    'Отсъствия по семейни причини за класове, обобщени по ден, месец, срок и учебна година.',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RRegularFamilyReasonAbsences.RegionID,
  //       RRegularFamilyReasonAbsences.SchoolYear,
  //       RRegularFamilyReasonAbsences.InstitutionID,
  //       RRegularFamilyReasonAbsences.Term,
  //     ],
  //     measures: [
  //       RRegularFamilyReasonAbsences.countDay,
  //       RRegularFamilyReasonAbsences.sumAbsencesPerDay,
  //       RRegularFamilyReasonAbsences.sumAbsencesTerm1,
  //       RRegularFamilyReasonAbsences.sumAbsencesTerm2,
  //       RRegularFamilyReasonAbsences.sumAbsencesPerYear,
  //       RRegularFamilyReasonAbsences.sumAbsencesPerMonth,
  //     ],
  //     indexes: {
  //       r_regular_family_reason_absences_index: {
  //         columns: [
  //           RRegularFamilyReasonAbsences.RegionID,
  //           RRegularFamilyReasonAbsences.SchoolYear,
  //           RRegularFamilyReasonAbsences.InstitutionID,
  //           RRegularFamilyReasonAbsences.Term,
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
        Term,
        AbsencesPerDay,
        AbsencesTerm1,
        AbsencesTerm2,
        AbsencesPerYear,
        AbsencesPerMonth,
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
        Term,
        AbsencesPerDay,
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
    Term: {
      sql: 'Term',
      type: 'number',
      title: 'Срок',
    },
    AbsencesPerDay: {
      sql: 'AbsencesPerDay',
      type: 'number',
      title: 'Отсъствия за деня',
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
    AbsencesPerMonth: {
      sql: 'AbsencesPerMonth',
      type: 'number',
      title: 'Отсъствия за месеца',
    },
  },

  dataSource: 'default',
});


