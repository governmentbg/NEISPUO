/* eslint no-undef: "off" */

cube('Phones', {
  sql: 'SELECT * FROM Phones',
  title: 'Контактна информация за институции',
  description: 'Служи за извършване на справки свързани с данни на телефони, имейл, уебсайтове на институции.',

  // preAggregations: {
  //   phonesByInstitution: {
  //     type: 'rollup',
  //     dimensions: [
  //       Phones.InstitutionID,
  //       Phones.RegionID,
  //       Phones.MunicipalityID,
  //       Phones.TownID,
  //     ],
  //     measures: [Phones.countInstitutionID],
  //     indexes: {
  //       phones_index: {
  //         columns: [
  //           Phones.InstitutionID,
  //           Phones.RegionID,
  //           Phones.MunicipalityID,
  //           Phones.TownID,
  //         ],
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
        InstitutionAbbreviation,
        CountryName,
        RegionName,
        MunicipalityName,
        TownName,
        LocalAreaName,
        BaseSchoolTypeName,
        DetailedSchoolTypeName,
        FinancialSchoolTypeName,
        BudgetingInstitutionName,
        PhoneType,
        PhoneCode,
        PhoneNumber,
        ContactKind,
        IsMain,
        Email,
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
      title: 'Институция',
    },

    InstitutionAbbreviation: {
      sql: 'InstitutionAbbreviation',
      type: 'string',
      title: 'Кратко наименование',
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

    RegionID: {
      sql: 'RegionID',
      type: 'number',
      title: 'Код на област',
      shown: false,
    },

    MunicipalityID: {
      sql: 'MunicipalityID',
      type: 'number',
      title: 'Код на община',
      shown: false,
    },

    TownID: {
      sql: 'TownID',
      type: 'number',
      title: 'Код на населено място',
      shown: false,
    },

    LocalAreaID: {
      sql: 'LocalAreaID',
      type: 'number',
      title: 'Код на район',
      shown: false,
    },

    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код на финансираща институция',
      shown: false,
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

    PhoneType: {
      sql: 'PhoneType',
      type: 'string',
      title: 'Тип телефон',
    },

    PhoneCode: {
      sql: 'PhoneCode',
      type: 'string',
      title: 'Код за АТМ',
    },

    PhoneNumber: {
      sql: 'PhoneNumber',
      type: 'string',
      title: 'Телефон',
    },

    ContactKind: {
      sql: 'ContactKind',
      type: 'string',
      title: 'Вид телефон',
    },

    IsMain: {
      sql: 'IsMain',
      type: 'string',
      title: 'Основен',
    },
    Email: {
      sql: 'Email',
      type: 'string',
      title: 'Имейл'
    }
  },

  dataSource: 'default',
});

