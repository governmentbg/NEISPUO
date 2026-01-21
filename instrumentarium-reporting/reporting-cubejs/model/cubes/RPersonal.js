/* eslint no-undef: "off" */

cube('RPersonal', {
  sql: 'SELECT * FROM R_Personal_N',
  title: 'Персонал',
  description: 'Служи за извършване на справки свързани с данни по персонал.',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RPersonal.RegionID,
  //       RPersonal.InstitutionID,
  //       RPersonal.MunicipalityID,
  //       RPersonal.TownID,
  //       RPersonal.StaffTypeName,
  //       RPersonal.CategoryStaffTypeName,
  //     ],
  //     measures: [
  //       RPersonal.countPersonal,
  //       RPersonal.sumPositionCount,
  //       RPersonal.sumWorkExpSpecYears,
  //       RPersonal.sumWorkExpTeachYears,
  //       RPersonal.sumWorkExpTotalYears,
  //     ],
  //     indexes: {
  //       r_personal_index: {
  //         columns: [
  //           RPersonal.RegionID,
  //           RPersonal.InstitutionID,
  //           RPersonal.MunicipalityID,
  //           RPersonal.TownID,
  //           RPersonal.StaffTypeName,
  //           RPersonal.CategoryStaffTypeName,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    sumPositionCount: {
      sql: 'PositionCount',
      type: 'sum',
      title: 'Сума "Щат"',
      drillMembers: [
        FirstName,
        MiddleName,
        LastName,
        PermanentTownName,
        CurrentTownName,
        NationalityName,
        BirthPlaceTownName,
        BirthPlaceCountryName,
        GenderName,
        PersonDetailTitle,
        InstitutionName,
        InstitutionID,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        PositionKindName,
        StaffTypeName,
        CategoryStaffTypeName,
        ContractWithName,
        NKPDPositionName,
        SubjectGroupName,
        IsNotMeetReq,
        CurrentlyValid,
        ContractTypeName,
        ContractReasonName,
      ],
    },

    sumWorkExpTotalYears: {
      sql: 'WorkExpTotalYears',
      type: 'sum',
      title: 'Сума "Трудов стаж"',
      drillMembers: [
        FirstName,
        MiddleName,
        LastName,
        PermanentTownName,
        CurrentTownName,
        NationalityName,
        BirthPlaceTownName,
        BirthPlaceCountryName,
        GenderName,
        PersonDetailTitle,
        InstitutionName,
        InstitutionID,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        PositionKindName,
        StaffTypeName,
        CategoryStaffTypeName,
        ContractWithName,
        NKPDPositionName,
        SubjectGroupName,
        IsNotMeetReq,
        CurrentlyValid,
        ContractTypeName,
        ContractReasonName,
      ],
    },

    sumWorkExpSpecYears: {
      sql: 'WorkExpSpecYears',
      type: 'sum',
      title: 'Сума "Трудов стаж - по специалността"',
      drillMembers: [
        FirstName,
        MiddleName,
        LastName,
        PermanentTownName,
        CurrentTownName,
        NationalityName,
        BirthPlaceTownName,
        BirthPlaceCountryName,
        GenderName,
        PersonDetailTitle,
        InstitutionName,
        InstitutionID,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        PositionKindName,
        StaffTypeName,
        CategoryStaffTypeName,
        ContractWithName,
        NKPDPositionName,
        SubjectGroupName,
        IsNotMeetReq,
        CurrentlyValid,
        ContractTypeName,
        ContractReasonName,
      ],
    },

    sumWorkExpTeachYears: {
      sql: 'WorkExpTeachYears',
      type: 'sum',
      title: 'Сума "Трудов стаж - учителски"',
      drillMembers: [
        FirstName,
        MiddleName,
        LastName,
        PermanentTownName,
        CurrentTownName,
        NationalityName,
        BirthPlaceTownName,
        BirthPlaceCountryName,
        GenderName,
        PersonDetailTitle,
        InstitutionName,
        InstitutionID,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        PositionKindName,
        StaffTypeName,
        CategoryStaffTypeName,
        ContractWithName,
        NKPDPositionName,
        SubjectGroupName,
        IsNotMeetReq,
        CurrentlyValid,
        ContractTypeName,
        ContractReasonName,
      ],
    },

    countPersonal: {
      type: 'count',
      title: 'Брой "Персонал"',
      drillMembers: [
        FirstName,
        MiddleName,
        LastName,
        PermanentTownName,
        CurrentTownName,
        NationalityName,
        BirthPlaceTownName,
        BirthPlaceCountryName,
        GenderName,
        PersonDetailTitle,
        InstitutionName,
        InstitutionID,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        PositionKindName,
        StaffTypeName,
        CategoryStaffTypeName,
        ContractWithName,
        NKPDPositionName,
        SubjectGroupName,
        IsNotMeetReq,
        CurrentlyValid,
        ContractTypeName,
        ContractReasonName,
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

    PermanentAddress: {
      sql: 'PermanentAddress',
      type: 'string',
      title: 'Постоянен адрес',
    },

    PermanentTownName: {
      sql: 'PermanentTownName',
      type: 'string',
      title: 'Постоянен адрес - населено място',
    },

    CurrentAddress: {
      sql: 'CurrentAddress',
      type: 'string',
      title: 'Настоящ адрес',
    },

    CurrentTownName: {
      sql: 'CurrentTownName',
      type: 'string',
      title: 'Настоящ адрес - населено място',
    },

    PublicEduNumber: {
      sql: 'PublicEduNumber',
      type: 'string',
      title: 'Персонален образователен номер',
    },

    NationalityName: {
      sql: 'NationalityName',
      type: 'string',
      title: 'Гражданство',
    },

    BirthPlaceTownName: {
      sql: 'BirthPlaceTownName',
      type: 'string',
      title: 'Месторождение - населено място',
    },

    BirthPlaceCountryName: {
      sql: 'BirthPlaceCountryName',
      type: 'string',
      title: 'Месторождение - държава',
    },

    GenderName: {
      sql: 'GenderName',
      type: 'string',
      title: 'Пол',
    },

    PersonDetailTitle: {
      sql: 'PersonDetailTitle',
      type: 'string',
      title: 'Титла',
    },

    InstitutionName: {
      sql: 'InstitutionName',
      type: 'string',
      title: 'Институция - Пълно наименование',
    },
    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    InstitutionAbbreviation: {
      sql: 'InstitutionAbbreviation',
      type: 'string',
      title: 'Институция - Кратко наименование',
    },

    RegionName: {
      sql: 'RegionName',
      type: 'string',
      title: 'Институция - Област',
    },

    MunicipalityName: {
      sql: 'MunicipalityName',
      type: 'string',
      title: 'Институция - община',
    },

    TownName: {
      sql: 'TownName',
      type: 'string',
      title: 'Институция - населено място',
    },

    LocalAreaName: {
      sql: 'LocalAreaName',
      type: 'string',
      title: 'Институция - район',
    },

    BaseSchoolTypeName: {
      sql: 'BaseSchoolTypeName',
      type: 'string',
      title: 'Институция - Вид по чл.37',
    },

    DetailedSchoolTypeName: {
      sql: 'DetailedSchoolTypeName',
      type: 'string',
      title: 'Институция - Вид по чл.38 (детайлен)',
    },

    FinancialSchoolTypeName: {
      sql: 'FinancialSchoolTypeName',
      type: 'string',
      title: 'Институция - Вид по чл.35-36 (според собствеността)',
    },

    BudgetingInstitutionName: {
      sql: 'BudgetingInstitutionName',
      type: 'string',
      title: 'Институция - Източник на финансиране',
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

    BirthDate: {
      sql: 'BirthDate',
      type: 'time',
      title: 'Дата на раждане',
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

    LocalAreaID: {
      sql: 'LocalAreaID',
      type: 'number',
      title: 'Код район',
    },

    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код финансираща институция',
      shown: false
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

    WorkExpTeachYears: {
      sql: 'WorkExpTeachYears',
      type: 'number',
      title: 'Трудов стаж - учителски',
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

    AcquiredPK: {
      sql: 'AcquiredPK',
      type: 'string',
      title: 'Придобита Професионална Квалификация'
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

    QCourseDurationCredits: {
      sql: 'QCourseDurationCredits',
      type: 'number',
      title: 'Kредити от квалификационния курс'
    },

    PKSType:{
      sql: 'PKSType',
      type: 'string',
      title:'Професионална квалификационна степен'
    }
  },

  dataSource: 'default',
});

