/* eslint no-undef: "off" */

cube('RPersonalPOKS', {
  sql: 'SELECT * FROM inst_basic.R_Personal_POKS',
  title: 'ПКС и ОКС на персонал',
  description: 'Служи за извършване на справки свързани с професионална квалификация и образователна квалификационна степен на персонала.',


  joins: {},

  measures: {
    countPersonal: {
      type: 'count',
      title: 'Брой "Персонал"',
      drillMembers: [
        FirstName,
        MiddleName,
        LastName,
        InstitutionID,
        StaffTypeName,
        AcquiredPK,
        EducationGradeType,
        SpecialityOKS,
        PKSType,
        QCourseDurationCredits
      ],
    },
  },

  dimensions: {
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
     InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    StaffTypeName: {
      sql: 'StaffTypeName',
      type: 'string',
      title: 'Вид персонал',
    },
    AcquiredPK: {
      sql: 'AcquiredPK',
      type: 'string',
      title: 'Професионална квалификация',
    },
    EducationGradeType: {
      sql: 'EducationGradeType',
      type: 'string',
      title: 'Образователна степен'
    },
    SpecialityOKS: {
      sql: 'SpecialityOKS',
      type: 'string',
      title: 'Специалност'
    },
    PKSType:{
      sql: 'PKSType',
      type: 'string',
      title:'Професионална квалификационна степен'
    },
    QCourseDurationCredits: {
      sql: 'QCourseDurationCredits',
      type: 'number',
      title: 'Kредити от квалификационния курс'
    },
  },

  dataSource: 'default',
});
