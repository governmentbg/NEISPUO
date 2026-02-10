/* eslint no-undef: "off" */

cube('RInstitutions', {
  sql: 'SELECT * FROM R_Institutions',
  title: 'Институции',
  description:
    'Служи за извършване на справки свързани с данни на ниво Институция',

  // preAggregations: {
  //   institutionCountByRegion: {
  //     type: 'rollup',
  //     measures: [RInstitutions.countInstitutionID],
  //     dimensions: [RInstitutions.RegionID],
  //     indexes: {
  //       r_institutions_index: {
  //         columns: [RInstitutions.RegionID],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countInstitutionID: {
      type: 'count',
      sql: 'InstitutionID',
      title: 'Брой "Институции"',
      drillMembers: [
        InstitutionID,
        InstitutionName,
        ShortInstitutionName,
        Bulstat,
        CountryName,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        IsDelegateBudget,
        Address,
        PhoneNumber,
        Email,
        Website,
        EstablishedYear,
        ConstitActFirst,
        ConstitActLast,
        SchoolShiftType,
        IsCentral,
        IsProtected,
        IsInnovative,
        IsNational,
        IsProfSchool,
        IsNonIndDormitory,
        HasMunDecisionFor4,
        IsAppInnovSystem,
        IsODZ,
        IsProvideEduServ,
        Director,
        InstitutionPublicCouncil,
        StaffCountAll,
        PedagogStaffCount,
        NonpedagogStaffCount,
        TownID,
        MunicipalityID,
        RegionID,
        LocalAreaID,
        BudgetingSchoolTypeID,
        YearlyBudget,
      ],
    },
  },

  dimensions: {
    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    InstitutionName: {
      sql: 'InstitutionName',
      type: 'string',
      title: 'Пълно наименование',
    },

    ShortInstitutionName: {
      sql: 'ShortInstitutionName',
      type: 'string',
      title: 'Кратко наименование',
    },

    Bulstat: {
      sql: 'Bulstat',
      type: 'string',
      title: 'Булстат/ЕИК',
    },

    CountryName: {
      sql: 'CountryName',
      type: 'string',
      title: 'Държава',
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

    LocalAreaName: {
      sql: 'LocalAreaName',
      type: 'string',
      title: 'Район',
    },

    BaseSchoolTypeName: {
      sql: 'BaseSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.37',
    },

    DetailedSchoolTypeName: {
      sql: 'DetailedSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.38 (детайлен)',
    },

    FinancialSchoolTypeName: {
      sql: 'FinancialSchoolTypeName',
      type: 'string',
      title: 'Вид по чл.35-36 (според собствеността)',
    },

    BudgetingInstitutionName: {
      sql: 'BudgetingInstitutionName',
      type: 'string',
      title: 'Източник на финансиране',
    },

    IsDelegateBudget: {
      sql: 'IsDelegateBudget',
      type: 'string',
      title: 'На делегиран бюджет',
    },

    Address: {
      sql: 'Address',
      type: 'string',
      title: 'Основен адрес',
    },

    PhoneNumber: {
      sql: 'PhoneNumber',
      type: 'string',
      title: 'Основен телефон',
    },

    Email: {
      sql: 'Email',
      type: 'string',
      title: 'Имейл',
    },

    Website: {
      sql: 'Website',
      type: 'string',
      title: 'Интернет страница',
    },

    EstablishedYear: {
      sql: 'EstablishedYear',
      type: 'string',
      title: 'Година на основаване',
    },

    ConstitActFirst: {
      sql: 'ConstitActFirst',
      type: 'string',
      title: 'Акт за създаване',
    },

    ConstitActLast: {
      sql: 'ConstitActLast',
      type: 'string',
      title: 'Последен акт за преобразуване',
    },

    SchoolShiftType: {
      sql: 'SchoolShiftType',
      type: 'string',
      title: 'Сменност на обучение',
    },

    IsCentral: {
      sql: 'IsCentral',
      type: 'string',
      title: 'Средищно/а',
    },

    IsProtected: {
      sql: 'IsProtected',
      type: 'string',
      title: 'Защитено/а',
    },

    IsInnovative: {
      sql: 'IsInnovative',
      type: 'string',
      title: 'Иновативно',
    },

    IsNational: {
      sql: 'IsNational',
      type: 'string',
      title: 'С национално значение',
    },

    IsProfSchool: {
      sql: 'IsProfSchool',
      type: 'string',
      title: 'Училището осигурява професионална подготовка',
    },

    IsNonIndDormitory: {
      sql: 'IsNonIndDormitory',
      type: 'string',
      title: 'Към училището има несамостоятелно общежитие',
    },

    HasMunDecisionFor4: {
      sql: 'HasMunDecisionFor4',
      type: 'string',
      title: 'Решение на ОС за задълж.предуч.обуч. на 4 год.',
    },

    IsAppInnovSystem: {
      sql: 'IsAppInnovSystem',
      type: 'string',
      title: 'ДГ прилага иновативна програмна система',
    },

    IsODZ: {
      sql: 'IsODZ',
      type: 'string',
      title: 'Към ДГ има яслена група',
    },

    IsProvideEduServ: {
      sql: 'IsProvideEduServ',
      type: 'string',
      title: 'ДГ организира доп.услуга по отглеждане на децата',
    },

    Director: {
      sql: 'Director',
      type: 'string',
      title: 'Директор',
    },

    InstitutionPublicCouncil: {
      sql: 'InstitutionPublicCouncil',
      type: 'string',
      title: 'Към училището/ДГ има обществен съвет',
    },

    StaffCountAll: {
      sql: 'StaffCountAll',
      type: 'number',
      title: 'Определена численост - персонал общо',
    },

    PedagogStaffCount: {
      sql: 'PedagogStaffCount',
      type: 'number',
      title: 'Определена численост - педаг.персонал',
    },

    NonpedagogStaffCount: {
      sql: 'NonpedagogStaffCount',
      type: 'number',
      title: 'Определена численост - непедаг.персонал',
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
      shown: false,
    },

    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код финансираща институция',
      shown: false,
    },

    YearlyBudget: {
      sql: 'YearlyBudget',
      type: 'number',
      title: 'Утвърден бюджет за календарна година',
    },
  },

  dataSource: 'default',
});
