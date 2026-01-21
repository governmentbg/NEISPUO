/* eslint no-undef: "off" */

cube('RPersonalStaff', {
  sql: 'SELECT * FROM R_Personal_Staff',
  title: 'Персонал - Служители',
  description:
    'Служи за извършване на справки свързани с данни по персонал - служители',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RPersonalStaff.RegionName,
  //       RPersonalStaff.InstitutionID,
  //       RPersonalStaff.MunicipalityName,
  //       RPersonalStaff.TownName,
  //       RPersonalStaff.Gender,
  //       RPersonalStaff.PersonnelCategory,
  //     ],
  //     measures: [
  //       RPersonalStaff.countInstitutionID,
  //     ],
  //     indexes: {
  //       r_personal_staff_index: {
  //         columns: [
  //           RPersonalStaff.RegionName,
  //           RPersonalStaff.InstitutionID,
  //           RPersonalStaff.MunicipalityName,
  //           RPersonalStaff.TownName,
  //           RPersonalStaff.Gender,
  //           RPersonalStaff.PersonnelCategory,
  //         ],
  //       },
  //     },
  //   },
  // },

  joins: {},

  measures: {
    countInstitutionID: {
      type: 'count',
      sql: '`Код по НЕИСПУО`',
      title: 'Брой "Институции"',
      drillMembers: [
        InstitutionID,
        InstitutionName,
        TownName,
        MunicipalityName,
        RegionName,
        RegionID,
        FinancialSchoolTypeName,
        DetailedSchoolTypeName,
        BudgetingInstitutionName,
        Email,
        FirstName,
        MiddleName,
        LastName,
        PublicEduNumber,
        Gender,
        BirthDate,
        BirthPlaceCountry,
        BirthPlaceCity,
        Citizenship,
        PermanentAddressMunicipality,
        PermanentAddressRegion,
        PermanentAddressTown,
        PermanentAddress,
        CurrentAddressMunicipality,
        CurrentAddressRegion,
        CurrentAddressTown,
        CurrentAddress,
        PhoneNumber,
        PersonalEmail,
        IsWorkingPensioner,
        IsContinuingEducation,
        ActivePositions,
        PersonnelCategory,
      ],
    },
  },

  dimensions: {
    InstitutionID: {
      sql: '`Код по НЕИСПУО`',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    InstitutionName: {
      sql: '`Наименование`',
      type: 'string',
      title: 'Наименование',
    },
    TownName: {
      sql: '`Населено място`',
      type: 'string',
      title: 'Населено място',
    },
    MunicipalityName: {
      sql: '`Община`',
      type: 'string',
      title: 'Община',
    },
    RegionName: {
      sql: '`Област`',
      type: 'string',
      title: 'Област',
    },
    RegionID: {
      sql: '`RegionID`',
      type: 'number',
      title: 'Код на област',
      shown: false,
    },
    FinancialSchoolTypeName: {
      sql: '`Вид по чл. 35-36 (според собствеността)`',
      type: 'string',
      title: 'Вид по чл. 35-36 (според собствеността)',
    },
    DetailedSchoolTypeName: {
      sql: '`Вид по чл. 38 (детайлен)`',
      type: 'string',
      title: 'Вид по чл. 38 (детайлен)',
    },
    BudgetingInstitutionName: {
      sql: '`Финансиращ орган`',
      type: 'string',
      title: 'Финансиращ орган',
    },
    Email: {
      sql: '`Email`',
      type: 'string',
      title: 'Email',
    },
    FirstName: {
      sql: '`Име`',
      type: 'string',
      title: 'Име',
    },
    MiddleName: {
      sql: '`Презиме`',
      type: 'string',
      title: 'Презиме',
    },
    LastName: {
      sql: '`Фамилия`',
      type: 'string',
      title: 'Фамилия',
    },
    PublicEduNumber: {
      sql: '`ЛОН`',
      type: 'string',
      title: 'ЛОН',
    },
    Gender: {
      sql: '`Пол`',
      type: 'string',
      title: 'Пол',
    },
    BirthDate: {
      sql: '`Дата на раждане`',
      type: 'time',
      title: 'Дата на раждане',
    },
    BirthPlaceCountry: {
      sql: '`Месторождение(държава)`',
      type: 'string',
      title: 'Месторождение(държава)',
    },
    BirthPlaceCity: {
      sql: '`Месторождение(град)`',
      type: 'string',
      title: 'Месторождение(град)',
    },
    Citizenship: {
      sql: '`Гражданство`',
      type: 'string',
      title: 'Гражданство',
    },
    PermanentAddressMunicipality: {
      sql: '`Постоянен адрес – Община`',
      type: 'string',
      title: 'Постоянен адрес - Община',
    },
    PermanentAddressRegion: {
      sql: '`Постоянен адрес – Област`',
      type: 'string',
      title: 'Постоянен адрес - Област',
    },
    PermanentAddressTown: {
      sql: '`Постоянен адрес – Населено място`',
      type: 'string',
      title: 'Постоянен адрес - Населено място',
    },
    PermanentAddress: {
      sql: '`Постоянен адрес`',
      type: 'string',
      title: 'Постоянен адрес',
    },
    CurrentAddressMunicipality: {
      sql: '`Настоящ адрес  – Община`',
      type: 'string',
      title: 'Настоящ адрес - Община',
    },
    CurrentAddressRegion: {
      sql: '`Настоящ адрес – Област`',
      type: 'string',
      title: 'Настоящ адрес - Област',
    },
    CurrentAddressTown: {
      sql: '`Настоящ адрес – Населено място`',
      type: 'string',
      title: 'Настоящ адрес - Населено място',
    },
    CurrentAddress: {
      sql: '`Настоящ адрес`',
      type: 'string',
      title: 'Настоящ адрес',
    },
    PhoneNumber: {
      sql: '`Телефон`',
      type: 'string',
      title: 'Телефон',
    },
    PersonalEmail: {
      sql: '`Електронна поща`',
      type: 'string',
      title: 'Електронна поща',
    },
    IsWorkingPensioner: {
      sql: '`Работещ пенсионер`',
      type: 'string',
      title: 'Работещ пенсионер',
    },
    IsContinuingEducation: {
      sql: '`Продължава образованието си`',
      type: 'string',
      title: 'Продължава образованието си',
    },
    ActivePositions: {
      sql: '`Активни длъжности`',
      type: 'string',
      title: 'Активни длъжности',
    },
    PersonnelCategory: {
      sql: '`Категория персонал`',
      type: 'string',
      title: 'Категория персонал',
    },
  },

  dataSource: 'default',
});
