/* eslint no-undef: "off" */

cube('RCurriculumSectionBProfessional', {
  sql: 'SELECT * FROM R_Curriculum_Section_B_Professional',
  title: 'Учебен план - Раздел Б Професионална',
  description:
    'Служи за извършване на справки свързани с данни по учебен план - раздел Б професионална',

  // preAggregations: {
  //   orderByRegion: {
  //     type: 'rollup',
  //     dimensions: [
  //       RCurriculumSectionBProfessional.RegionName,
  //       RCurriculumSectionBProfessional.InstitutionID,
  //       RCurriculumSectionBProfessional.MunicipalityName,
  //       RCurriculumSectionBProfessional.TownName,
  //       RCurriculumSectionBProfessional.EducationStage,
  //       RCurriculumSectionBProfessional.SubjectName,
  //     ],
  //     measures: [
  //       RCurriculumSectionBProfessional.countInstitutionID,
  //     ],
  //     indexes: {
  //       r_curriculum_section_b_professional_index: {
  //         columns: [
  //           RCurriculumSectionBProfessional.RegionName,
  //           RCurriculumSectionBProfessional.InstitutionID,
  //           RCurriculumSectionBProfessional.MunicipalityName,
  //           RCurriculumSectionBProfessional.TownName,
  //           RCurriculumSectionBProfessional.EducationStage,
  //           RCurriculumSectionBProfessional.SubjectName,
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
        SchoolYear,
        EducationStage,
        ClassName,
        SubjectName,
        StudyMethod,
        FirstTermWeeks,
        FirstTermHours,
        SecondTermWeeks,
        SecondTermHours,
        TotalHours,
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
    TeacherName: {
      sql: '`Преподавател`',
      type: 'string',
      title: 'Преподавател',
    },
  },

  dataSource: 'default',
});
