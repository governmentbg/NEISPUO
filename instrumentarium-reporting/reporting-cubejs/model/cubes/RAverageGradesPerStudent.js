/* eslint no-undef: "off" */

cube('RAverageGradesPerStudent', {
  sql: 'SELECT * FROM R_Average_Grades_Per_Student',
  title: 'Справка за срочен и годишен успех по ученици',
  description:
    'Срочни и годишни средни оценки по ученици (I/II срок и годишно).',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RAverageGradesPerStudent.RegionID,
  //       RAverageGradesPerStudent.SchoolYear,
  //       RAverageGradesPerStudent.InstitutionID,
  //       RAverageGradesPerStudent.FirstName,
  //       RAverageGradesPerStudent.LastName,
  //       RAverageGradesPerStudent.PersonID,
  //     ],
  //     measures: [
  //       RAverageGradesPerStudent.countPersonID,
  //       RAverageGradesPerStudent.avgAverageTerm1,
  //       RAverageGradesPerStudent.avgAverageTerm2,
  //       RAverageGradesPerStudent.avgAverageYear,
  //     ],
  //     indexes: {
  //       r_average_grades_per_student_index: {
  //         columns: [
  //           RAverageGradesPerStudent.RegionID,
  //           RAverageGradesPerStudent.SchoolYear,
  //           RAverageGradesPerStudent.InstitutionID,
  //           RAverageGradesPerStudent.FirstName,
  //           RAverageGradesPerStudent.LastName,
  //           RAverageGradesPerStudent.PersonID,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countPersonID: {
      type: 'count',
      title: 'Брой ученици',
      sql: 'PersonID',
      drillMembers: [
        RegionID,
        RegionName,
        MunicipalityName,
        TownName,
        SchoolYear,
        InstitutionID,
        InstitutionName,
        DetailedSchoolTypeID,
        BudgetingSchoolTypeID,
        FinancialSchoolTypeID,
        PersonID,
        FirstName,
        MiddleName,
        LastName,
        ClassBookId,
        ClassOrGroupName,
        AverageTerm1,
        AverageTerm2,
        AverageYear,
      ],
    },
    avgAverageTerm1: {
      type: 'avg',
      title: 'Средна оценка - I срок',
      sql: 'AverageTerm1',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        PersonID,
        FirstName,
        LastName,
        SchoolYear,
        ClassOrGroupName,
        AverageTerm1,
      ],
    },
    avgAverageTerm2: {
      type: 'avg',
      title: 'Средна оценка - II срок',
      sql: 'AverageTerm2',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        PersonID,
        FirstName,
        LastName,
        SchoolYear,
        ClassOrGroupName,
        AverageTerm2,
      ],
    },
    avgAverageYear: {
      type: 'avg',
      title: 'Средна оценка за учебната година',
      sql: 'AverageYear',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        PersonID,
        FirstName,
        LastName,
        SchoolYear,
        ClassOrGroupName,
        AverageYear,
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
    SchoolYear: {
      sql: 'SchoolYear',
      type: 'number',
      title: 'Учебна година',
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
    PersonID: {
      sql: 'PersonID',
      type: 'number',
      title: 'Идентификатор на ученика',
      shown: false,
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
    ClassBookId: {
      sql: 'ClassBookId',
      type: 'number',
      title: 'Код на клас',
      shown: false,
    },
    ClassOrGroupName: {
      sql: 'ClassOrGroupName',
      type: 'string',
      title: 'Клас/група',
    },
    AverageTerm1: {
      sql: 'AverageTerm1',
      type: 'number',
      title: 'Средна оценка през I срок',
    },
    AverageTerm2: {
      sql: 'AverageTerm2',
      type: 'number',
      title: 'Средна оценка през II срок',
    },
    AverageYear: {
      sql: 'AverageYear',
      type: 'number',
      title: 'Средна оценка за учебната година',
    },
  },

  dataSource: 'default',
});


