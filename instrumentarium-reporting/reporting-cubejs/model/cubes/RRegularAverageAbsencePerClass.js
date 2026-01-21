/* eslint no-undef: "off" */

cube('RRegularAverageAbsencePerClass', {
  sql: 'SELECT * FROM R_Regular_Average_Absence_Per_Class',
  title: 'Справка за отсъствия по класове - I-XII клас',
  description:
    'Средни отсъствия по класове (неизвинени и извинени по вид) за I–XII клас.',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RRegularAverageAbsencePerClass.RegionID,
  //       RRegularAverageAbsencePerClass.SchoolYear,
  //       RRegularAverageAbsencePerClass.InstitutionID,
  //     ],
  //     measures: [
  //       RRegularAverageAbsencePerClass.countClassBookId,
  //       RRegularAverageAbsencePerClass.sumTotalUnexcusedAbsences,
  //       RRegularAverageAbsencePerClass.sumExcusedByMedicalNoticeAbsence,
  //       RRegularAverageAbsencePerClass.sumExcusedByHealthReasonAbsence,
  //       RRegularAverageAbsencePerClass.sumExcusedByFamilyReasonAbsence,
  //       RRegularAverageAbsencePerClass.sumExcusedByOtherReasonAbsence,
  //       RRegularAverageAbsencePerClass.sumTotalExcusedAbsences,
  //     ],
  //     indexes: {
  //       r_regular_average_absence_per_class_index: {
  //         columns: [
  //           RRegularAverageAbsencePerClass.RegionID,
  //           RRegularAverageAbsencePerClass.SchoolYear,
  //           RRegularAverageAbsencePerClass.InstitutionID,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countClassBookId: {
      type: 'count',
      title: 'Брой класове',
      sql: 'ClassBookId',
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
        ClassBookId,
        ClassOrGroupName,
        TotalUnexcusedAbsences,
        ExcusedByMedicalNoticeAbsence,
        ExcusedByHealthReasonAbsence,
        ExcusedByFamilyReasonAbsence,
        ExcusedByOtherReasonAbsence,
        TotalExcusedAbsences,
      ],
    },
    sumTotalUnexcusedAbsences: {
      type: 'sum',
      title: 'Общо неизвинени отсъствия',
      sql: 'TotalUnexcusedAbsences',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        TotalUnexcusedAbsences,
      ],
    },
    sumExcusedByMedicalNoticeAbsence: {
      type: 'sum',
      title: 'Извинени отсъствия - медицинско удостоверение',
      sql: 'ExcusedByMedicalNoticeAbsence',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        ExcusedByMedicalNoticeAbsence,
      ],
    },
    sumExcusedByHealthReasonAbsence: {
      type: 'sum',
      title: 'Извинени отсъствия - здравни причини',
      sql: 'ExcusedByHealthReasonAbsence',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        ExcusedByHealthReasonAbsence,
      ],
    },
    sumExcusedByFamilyReasonAbsence: {
      type: 'sum',
      title: 'Извинени отсъствия - семейни причини',
      sql: 'ExcusedByFamilyReasonAbsence',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        ExcusedByFamilyReasonAbsence,
      ],
    },
    sumExcusedByOtherReasonAbsence: {
      type: 'sum',
      title: 'Извинени отсъствия - други причини',
      sql: 'ExcusedByOtherReasonAbsence',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        ExcusedByOtherReasonAbsence,
      ],
    },
    sumTotalExcusedAbsences: {
      type: 'sum',
      title: 'Общо извинени отсъствия',
      sql: 'TotalExcusedAbsences',
      drillMembers: [
        RegionID,
        RegionName,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        TotalExcusedAbsences,
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
    TotalUnexcusedAbsences: {
      sql: 'TotalUnexcusedAbsences',
      type: 'number',
      title: 'Общо неизвинени отсъствия',
    },
    ExcusedByMedicalNoticeAbsence: {
      sql: 'ExcusedByMedicalNoticeAbsence',
      type: 'number',
      title: 'Извинени отсъствия - мед. удостоверение',
    },
    ExcusedByHealthReasonAbsence: {
      sql: 'ExcusedByHealthReasonAbsence',
      type: 'number',
      title: 'Извинени отсъствия - здравни причини',
    },
    ExcusedByFamilyReasonAbsence: {
      sql: 'ExcusedByFamilyReasonAbsence',
      type: 'number',
      title: 'Извинени отсъствия - семейни причини',
    },
    ExcusedByOtherReasonAbsence: {
      sql: 'ExcusedByOtherReasonAbsence',
      type: 'number',
      title: 'Извинени отсъствия - други причини',
    },
    TotalExcusedAbsences: {
      sql: 'TotalExcusedAbsences',
      type: 'number',
      title: 'Общо извинени отсъствия',
    },
  },

  dataSource: 'default',
});