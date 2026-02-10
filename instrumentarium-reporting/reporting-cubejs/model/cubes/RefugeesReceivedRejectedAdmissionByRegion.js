/* eslint no-undef: "off" */

cube('RefugeesReceivedRejectedAdmissionByRegion', {
  sql: 'SELECT * FROM RefugeesReceivedRejectedAdmissionByRegion',
  title: 'Заявления от търсещи или получили закрила по области',
  description: 'Служи за извършване на справки свързани с данни за заявления от търсещи или получили закрила по области',

  // preAggregations: {
  //   refugeesByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RefugeesReceivedRejectedAdmissionByRegion.RegionID,
  //       RefugeesReceivedRejectedAdmissionByRegion.SchoolYear,
  //       RefugeesReceivedRejectedAdmissionByRegion.ClassOrGroup,
  //     ],
  //     measures: [
  //       RefugeesReceivedRejectedAdmissionByRegion.sumChildNumber,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumRejected,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumKindergartenLastInstitution,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumSchoolLastInstitution,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumStudentsWithEGN,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumStudentsWithLNCH,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumHasDocumentForCompletedClass,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumHasImmunizationStatusDocumentSum,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumKGEnrolledWithEGN,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumKGEnrolledWithLNCH,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumSEnrolledWithEGN,
  //       RefugeesReceivedRejectedAdmissionByRegion.sumSEnrolledWithLNCH,
  //     ],
  //     indexes: {
  //       refugees_by_region_index: {
  //         columns: [
  //           RefugeesReceivedRejectedAdmissionByRegion.RegionID,
  //           RefugeesReceivedRejectedAdmissionByRegion.SchoolYear,
  //           RefugeesReceivedRejectedAdmissionByRegion.ClassOrGroup,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    sumChildNumber: {
      type: 'sum',
      title: 'Сумирай по "Брой ученици"',
      sql: 'ChildNumber',
      drillMembers: [
        Rejected,
        RegionName,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        SchoolLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumRejected: {
      type: 'sum',
      title: 'Сумирай по "От тях оттеглени"',
      sql: 'Rejected',
      drillMembers: [
        ChildNumber,
        RegionName,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        SchoolLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumKindergartenLastInstitution: {
      type: 'sum',
      title: 'Сумирай по "За ДГ"',
      sql: 'KindergartenLastInstitution',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        StudentsWithEGN,
        StudentsWithLNCH,
        SchoolLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumSchoolLastInstitution: {
      type: 'sum',
      title: 'Сумирай по "За училище"',
      sql: 'SchoolLastInstitution',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumStudentsWithEGN: {
      type: 'sum',
      title: 'Сумирай по "Деца с ЕГН"',
      sql: 'StudentsWithEGN',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumStudentsWithLNCH: {
      type: 'sum',
      title: 'Сумирай по "Деца с ЛНЧ"',
      sql: 'StudentsWithLNCH',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumHasDocumentForCompletedClass: {
      type: 'sum',
      title: 'Сумирай по "Документи за обучение"',
      sql: 'HasDocumentForCompletedClass',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumHasImmunizationStatusDocumentSum: {
      type: 'sum',
      title: 'Сумирай по "Документи за имунизация"',
      sql: 'HasImmunizationStatusDocumentSum',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumKGEnrolledWithEGN: {
      type: 'sum',
      title: 'Сумирай по "Записани с ЕГН в ДГ"',
      sql: 'KGEnrolledWithEGN',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumKGEnrolledWithLNCH: {
      type: 'sum',
      title: 'Сумирай по "Записани с ЛНЧ в ДГ"',
      sql: 'KGEnrolledWithLNCH',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        SEnrolledWithEGN,
        SEnrolledWithLNCH
      ]
    },
    sumSEnrolledWithEGN: {
      type: 'sum',
      title: 'Сумирай по "Записани с ЕГН в училище"',
      sql: 'SEnrolledWithEGN',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithLNCH
      ]
    },
    sumSEnrolledWithLNCH: {
      type: 'sum',
      title: 'Сумирай по "Записани с ЛНЧ в училище"',
      sql: 'SEnrolledWithLNCH',
      drillMembers: [
        ChildNumber,
        Rejected,
        RegionName,
        SchoolLastInstitution,
        StudentsWithEGN,
        StudentsWithLNCH,
        KindergartenLastInstitution,
        HasDocumentForCompletedClass,
        HasImmunizationStatusDocumentSum,
        KGEnrolledWithEGN,
        KGEnrolledWithLNCH,
        SEnrolledWithEGN
      ]
    }
  },

  dimensions: {
    SchoolYear: {
      sql: 'SchoolYear',
      type: 'number',
      title: 'Учебна година',
    },

    ClassOrGroup: {
      sql: 'ClassOrGroup',
      type: 'string',
      title: 'Випуск',
    },

    ChildNumber: {
      sql: 'ChildNumber',
      type: 'number',
      title: 'Общо деца',
    },
    Rejected: {
      sql: 'Rejected',
      type: 'number',
      title: 'От тях оттеглени',
    },

    KindergartenLastInstitution: {
      sql: 'KindergartenLastInstitution',
      type: 'number',
      title: 'За ДГ',
    },

    SchoolLastInstitution: {
      sql: 'SchoolLastInstitution',
      type: 'number',
      title: 'За училище',
    },

    StudentsWithEGN: {
      sql: 'StudentsWithEGN',
      type: 'number',
      title: 'Деца с ЕГН',
    },

    StudentsWithLNCH: {
      sql: 'StudentsWithLNCH',
      type: 'number',
      title: 'Деца с ЛНЧ',
    },

    HasDocumentForCompletedClass: {
      sql: 'HasDocumentForCompletedClass',
      type: 'number',
      title: 'Документи за обучение',
    },
    HasImmunizationStatusDocumentSum: {
      sql: 'HasImmunizationStatusDocumentSum',
      type: 'number',
      title: 'Документ за имунизации',
    },

    KGEnrolledWithEGN: {
      sql: 'KGEnrolledWithEGN',
      type: 'number',
      title: 'Записани с ЕГН в ДГ',
    },

    KGEnrolledWithLNCH: {
      sql: 'KGEnrolledWithLNCH',
      type: 'number',
      title: 'Записани с ЛНЧ в ДГ',
    },

    SEnrolledWithEGN: {
      sql: 'SEnrolledWithEGN',
      type: 'number',
      title: 'Записани с ЕГН в училище',
    },

    SEnrolledWithLNCH: {
      sql: 'SEnrolledWithLNCH',
      type: 'number',
      title: 'Записани с ЛНЧ в училище',
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
  },

  dataSource: 'default',
});

