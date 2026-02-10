/* eslint no-undef: "off" */

cube('RPersonalP', {
  sql: 'SELECT * FROM inst_basic.R_Personal_P',
  title: 'Длъжности на персонала',
  description: 'Служи за извършване на справки свързани с длъжности на персонала.',

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
       WorkStartYear,
       WorkExpTotalYears,
       WorkExpSpecYears,
       WorkExpTeachYears,
       StaffOrd,
       StaffPositionNo,
       PositionKindName,
       StaffTypeName,
       CategoryStaffTypeName,
       PositionCount,
       IsNotMeetReq,
       ContractWithName,
       NKPDPositionName,
       SubjectGroupName,
       CurrentlyValid,
       PositionNotes,
       ContractTypeName,
       ContractReasonName,
       ContractNo,
       ContractYear,
      ContractNotes,
      IsAccountablePerson,
      IsTravel,
      IsExtendStudent,
      IsPensioneer,
      IsMentor,
      IsTrainee,
      IsHospital,
      Norma,
      NormaT,
      ReductionHours,
      LectYear,
      PhoneNumber,
      Email
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

    SubjectGroupName: {
      sql: 'SubjectGroupName',
      type: 'string',
      title: 'Назначен на щатно място по',
    },
    IsNotMeetReq: {
      sql: 'IsNotMeetReq',
      type: 'string',
      title: 'Не отговаря на изискванията (щатна бройка)',
    },
    CurrentlyValid: {
      sql: 'CurrentlyValid',
      type: 'string',
      title: 'Активна за текущата учебна година',
    },

    PositionNotes: {
      sql: 'PositionNotes',
      type: 'string',
      title: 'Бележки по длъжността',
    },

    ContractTypeName: {
      sql: 'ContractTypeName',
      type: 'string',
      title: 'Вид на договора',
    },

    ContractReasonName: {
      sql: 'ContractReasonName',
      type: 'string',
      title: 'Основание по КТ',
    },

    ContractNo: {
      sql: 'ContractNo',
      type: 'string',
      title: '№ на договора',
    },

    ContractNotes: {
      sql: 'ContractNotes',
      type: 'string',
      title: 'Бележки',
    },

    IsAccountablePerson: {
      sql: 'IsAccountablePerson',
      type: 'string',
      title: 'МОЛ',
    },

    IsTravel: {
      sql: 'IsTravel',
      type: 'string',
      title: 'Пътуващ от друго населено място',
    },

    IsExtendStudent: {
      sql: 'IsExtendStudent',
      type: 'string',
      title: 'Продължава образованието си',
    },

    IsPensioneer: {
      sql: 'IsPensioneer',
      type: 'string',
      title: 'Работещ пенсионер',
    },

    IsMentor: {
      sql: 'IsMentor',
      type: 'string',
      title: 'Учител - наставник',
    },

    IsTrainee: {
      sql: 'IsTrainee',
      type: 'string',
      title: 'Учител - стажант',
    },

    IsHospital: {
      sql: 'IsHospital',
      type: 'string',
      title: 'Болничен учител',
    },

    Norma: {
      sql: 'Norma',
      type: 'string',
      title: 'Минимална ЗНПР',
    },

    NormaT: {
      sql: 'NormaT',
      type: 'string',
      title: 'Индивидуална ЗНПР',
    },

    ReductionHours: {
      sql: 'ReductionHours',
      type: 'number',
      title: 'Всичко възложени часове (с редукция)',
    },

    LectYear: {
      sql: 'LectYear',
      type: 'number',
      title: 'Лекторски / недостиг - инд.ЗНПР (год.)',
    },

    WorkStartYear: {
      sql: 'WorkStartYear',
      type: 'number',
      title: 'Година на постъпване',
    },

    WorkExpTotalYears: {
      sql: 'WorkExpTotalYears',
      type: 'number',
      title: 'Трудов стаж - общ',
    },

    WorkExpSpecYears: {
      sql: 'WorkExpSpecYears',
      type: 'number',
      title: 'Трудов стаж - по специалността',
    },

    ContractWithName: {
      sql: 'ContractWithName',
      type: 'string',
      title: 'Назначен към',
    },

    NKPDPositionName: {
      sql: 'NKPDPositionName',
      type: 'string',
      title: 'Длъжност',
    },

    ContractYear: {
      sql: 'ContractYear',
      type: 'string',
      title: 'Година на договор'
    },

    WorkExpTeachYears: {
      sql: 'WorkExpTeachYears',
      type: 'number',
      title: 'Трудов стаж - учителски',
    },

    StaffOrd: {
      sql: "StaffOrd",
      type: "number",
      title: "Пореден №"
    },

    StaffPositionNo: {
      sql: "StaffOrd",
      type: "number",
      title: "Длъжност №"
    },

    PositionKindName: {
      sql: 'PositionKindName',
      type: 'string',
      title: 'Титуляр/заместник',
    },

    StaffTypeName: {
      sql: 'StaffTypeName',
      type: 'string',
      title: 'Вид персонал',
    },

    CategoryStaffTypeName: {
      sql: 'CategoryStaffTypeName',
      type: 'string',
      title: 'Категория персонал',
    },

    PositionCount: {
      sql: 'PositionCount',
      type: 'number',
      title: 'Щат',
    },

    PhoneNumber: {
      sql: 'PhoneNumber',
      type: 'string',
      title: 'Телефонен номер',
    },

    Email: {
      sql: 'Email',
      type: 'string',
      title: 'Имейл'
    },

  },

  dataSource: 'default',
});
