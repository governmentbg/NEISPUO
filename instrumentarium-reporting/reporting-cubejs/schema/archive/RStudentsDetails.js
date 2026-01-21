/* eslint no-undef: "off" */

cube('RStudentsDetails', {
  sql: 'SELECT * FROM airbyte_internal.r_students_details_view',
  title: 'Списък с деца/ученици по институции',
  description:
    'Извежда списък с деца/ученици, записани в учебни паралелки, по институции с паспортни данни и общи данни за обучението',

  joins: {},

  measures: {
    countPersonalID: {
      type: 'count',
      title: 'Брой деца/ученици',
      sql: 'PersonalID',
      drillMembers: [
        RegionName,
        MunicipalityName,
        TownName,
        LAreaName,
        InstitutionID,
        InstitutionName,
        InstitutionKind,
        BudgetingSchoolTypeID,
        FirstName,
        MiddleName,
        LastName,
        PublicEduNumber,
        IdType,
        BirthDate,
        Gender,
        PersonTown,
        PersonMunicipality,
        PersonRegion,
        Nationality,
        RomeName,
        ClassName,
        EduFormName,
        ProfName,
        ClassType,
        SpecName,
        IsIndividialCurriculum,
        IsTravel,
        IsRepeatClass,
        IsSOP,
        IsRP,
      ],
    },
  },

  dimensions: {
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
    LAreaName: {
      sql: 'LAreaName',
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
    InstitutionKind: {
      sql: 'InstitutionKind',
      type: 'string',
      title: 'Вид на институция',
    },
    BudgetingSchoolTypeID: {
      sql: 'BudgetingSchoolTypeID',
      type: 'number',
      title: 'Код на финансираща институция',
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
    PublicEduNumber: {
      sql: 'PublicEduNumber',
      type: 'string',
      title: 'ЛОН',
    },
    IdType: {
      sql: 'IdType',
      type: 'string',
      title: 'Вид идентификатор',
    },
    PersonalID: {
      sql: 'PersonalID',
      type: 'string',
      title: 'ЕГН',
    },
    BirthDate: {
      sql: 'BirthDate',
      type: 'time',
      title: 'Дата на раждане',
    },
    Gender: {
      sql: 'Gender',
      type: 'string',
      title: 'Пол',
    },
    PersonTown: {
      sql: 'PersonTown',
      type: 'string',
      title: 'Населено място (ученик)',
    },
    PersonMunicipality: {
      sql: 'PersonMunicipality',
      type: 'string',
      title: 'Община (ученик)',
    },
    PersonRegion: {
      sql: 'PersonRegion',
      type: 'string',
      title: 'Област (ученик)',
    },
    Nationality: {
      sql: 'Nationality',
      type: 'string',
      title: 'Националност',
    },
    RomeName: {
      sql: 'RomeName',
      type: 'string',
      title: 'Випуск',
    },
    ClassName: {
      sql: 'ClassName',
      type: 'string',
      title: 'Група/Паралелка',
    },
    EduFormName: {
      sql: 'EduFormName',
      type: 'string',
      title: 'Форма на обучение',
    },
    ProfName: {
      sql: 'ProfName',
      type: 'string',
      title: 'Професия',
    },
    ClassType: {
      sql: 'ClassType',
      type: 'string',
      title: 'Профил',
    },
    SpecName: {
      sql: 'SpecName',
      type: 'string',
      title: 'Специалност',
    },
    IsIndividialCurriculum: {
      sql: 'IsIndividualCurriculum',
      type: 'string',
      title: 'Инд. уч. план',
    },
    IsTravel: {
      sql: 'IsTravel',
      type: 'string',
      title: 'Пътуващ от друго населено място',
    },
    IsRepeatClass: {
      sql: 'IsRepeatClass',
      type: 'string',
      title: 'Второгодник',
    },
    IsSOP: {
      sql: 'IsSOP',
      type: 'string',
      title: 'СОП',
    },
    IsRP: {
      sql: 'IsRP',
      type: 'string',
      title: 'РП',
    },
  },

  dataSource: 'default',
});
