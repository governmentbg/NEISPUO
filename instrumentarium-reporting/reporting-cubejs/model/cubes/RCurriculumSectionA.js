/* eslint no-undef: "off" */

cube('RCurriculumSectionA', {
  sql: 'SELECT * FROM R_Curriculum_Section_A',
  title: 'Учебен план - Раздел А',
  description:
    'Служи за извършване на справки свързани с данни по учебен план - раздел А',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RCurriculumSectionA.RegionName,
  //       RCurriculumSectionA.InstitutionID,
  //       RCurriculumSectionA.MunicipalityName,
  //       RCurriculumSectionA.TownName,
  //       RCurriculumSectionA.EducationStage,
  //       RCurriculumSectionA.SubjectName,
  //     ],
  //     measures: [
  //       RCurriculumSectionA.countInstitutionID,
  //     ],
  //     indexes: {
  //       r_curriculum_section_a_index: {
  //         columns: [
  //           RCurriculumSectionA.RegionName,
  //           RCurriculumSectionA.InstitutionID,
  //           RCurriculumSectionA.MunicipalityName,
  //           RCurriculumSectionA.TownName,
  //           RCurriculumSectionA.EducationStage,
  //           RCurriculumSectionA.SubjectName,
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
        LocalAreaName,
        FinancialSchoolTypeName,
        DetailedSchoolTypeName,
        BudgetingInstitutionName,
        BaseSchoolTypeName,
        Email,
        SchoolYear,
        EducationStage,
        ClassName,
        SubjectName,
        StudyMethod,
        ForeignLanguage,
        FirstTermWeeks,
        FirstTermHours,
        SecondTermWeeks,
        SecondTermHours,
        TotalHours,
        StudentsCount,
        TeacherName,
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
    LocalAreaName: {
      sql: '`Район`',
      type: 'string',
      title: 'Район',
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
    BaseSchoolTypeName: {
      sql: '`Вид по чл. 37 (общ, според вида на подготовката)`',
      type: 'string',
      title: 'Вид по чл. 37 (общ, според вида на подготовката)',
    },
    Email: {
      sql: '`Email`',
      type: 'string',
      title: 'Email',
    },
    SchoolYear: {
      sql: '`Учебна година`',
      type: 'number',
      title: 'Учебна година',
    },
    EducationStage: {
      sql: '`Етап на обучение`',
      type: 'string',
      title: 'Етап на обучение',
    },
    ClassName: {
      sql: '`Наименование на випуск/клас`',
      type: 'string',
      title: 'Наименование на випуск/клас',
    },
    SubjectName: {
      sql: '`Учебен предмет`',
      type: 'string',
      title: 'Учебен предмет',
    },
    StudyMethod: {
      sql: '`Начин на изучаване`',
      type: 'string',
      title: 'Начин на изучаване',
    },
    ForeignLanguage: {
      sql: '`Чужд език`',
      type: 'string',
      title: 'Чужд език',
    },
    FirstTermWeeks: {
      sql: '`I срок УС`',
      type: 'number',
      title: 'I срок УС',
    },
    FirstTermHours: {
      sql: '`I срок ЧС`',
      type: 'number',
      title: 'I срок ЧС',
    },
    SecondTermWeeks: {
      sql: '`II срок УС`',
      type: 'number',
      title: 'II срок УС',
    },
    SecondTermHours: {
      sql: '`II срок ЧС`',
      type: 'number',
      title: 'II срок ЧС',
    },
    TotalHours: {
      sql: '`Общ бр.ч.`',
      type: 'number',
      title: 'Общ бр.ч.',
    },
    StudentsCount: {
      sql: '`Общ брой ученици`',
      type: 'number',
      title: 'Общ брой ученици',
    },
    TeacherName: {
      sql: '`Преподавател`',
      type: 'string',
      title: 'Преподавател',
    },
  },

  dataSource: 'default',
});
