export default {
  students: [],
  editMode: false,
  user: null,
  userDetails: null,
  language: localStorage.getItem('culture') || 'bg',
  gridItemsPerPageOptions: [5,10,15,50,100,-1],
  permissions: [],
  permissionsForStudent: [],
  permissionsForInstitution: [],
  permissionsForClass: [],
  appMenu: {
    showMainMenu: true
  },
  studentSearchModel: null,
  currentStudentSummary: null,
  demandingPermission: false,
  selectedStudentClass: null,
  contextualInfo: null,
  dynamicEntitiesSchema: null,
  termOptions: [
    { value: 1, text: 'първи срок'}
  ], // Използва се само в ЛОД/Признаване, където не е нужна опция за Втори срок
  manageContextualInformation: false,
  studentFinalizedLods: [],
  messagesCount: 0,
  apiErros: [],
  diplomaListSelectedTab: undefined,
  dualEduFormId: 9,
  dualClassTypeId: 20,
  gridOptions: {
    'institutionDiplomaList': {},
    'institutionsList': {},
    'institutionExternalList': {},
    'institutionStudentslList': {},
    'preSchoolEvaluationsList': {},
    'institutionClassesList': {},
    'oresList': {},
    'classOresList': {},
    'studentOresList': {},
    'unsignedStudentLodList': {},
    'lodFInalizationList': {},
    'dualFormList': {},
    'externalEvaluationList': {},
    'docManagementApplicationsList': {},
    'docManagementAdditionalCampaignsList': {},
    'docManagementUnusedDocsList': {},
    'docManagementReportList': {},
    'docManagementApplicationsDashboard': {}
  },
  gridFilters: {
    'institutionDiplomaList': {
      schoolYear: null,
      filterForSigning: false,
      basicDocuments: []
    },
    'institutionsList': {},
    'institutionClassesList': {},
    'institutionStudentslList': {},
    'oresList': {
      schoolYear: null,
      oresTypeId: null,
      inheritanceType: null,
      oresRange: null,
      institutionId: null
    },
    'classOresList': {
      schoolYear: null,
      oresTypeId: null,
      inheritanceType: null,
      oresRange: null,
      institutionId: null
    },
    'studentOresList': {
      schoolYear: null,
      oresTypeId: null,
      inheritanceType: null,
      oresRange: null,
      institutionId: null
    },
    'unsignedStudentLodList': {
      schoolYear: null
    },
    'lodFInalizationList': {
      schoolYear: null,
      institutionId: null,
    },
    'dualFormList': {
      schoolYear: null,
      institutionId: null,
    },
    'externalEvaluationList': {
      schoolYear: null,
      institutionId: null,
    },
    'docManagementApplicationsList': {
      schoolYear: null,
      institutionId: null,
      campaignId: null,
      regionId: null,
      municipalityId: null,
      instType: 1, // 1 - Всички, 2 - С делегиран бюджет, 3 - Без делегиран бюджет
      groupingType: 1, // 1 - Без групиране, 2 - Групиране по регион, 3 - Групиране по община
      campaignType: 1, // 1 - Всички, 2 - Основни кампании, 3 - Допълнителни кампании
    },
    'docManagementAdditionalCampaignsList': {
      institutionId: null,
      regionId: null,
    },
    'docManagementUnusedDocsList': {
      institutionId: null,
      regionId: null,
      municipalityId: null,
      townId: null,
      basicDocumentId: null
    },
    'docManagementReportList': {
      schoolYear: null,
      institutionId: null,
      regionId: null,
      municipalityId: null,
      townId: null,
      basicDocumentId: null
    },
    'docManagementApplicationsDashboard': {
      schoolYear: null,
      institutionId: null,
      campaignId: null,
      regionId: null,
      municipalityId: null,
      instType: 1, // 1 - Всички, 2 - С делегиран бюджет, 3 - Без делегиран бюджет
      campaignType: 1, // 1 - Всички, 2 - Основни кампании, 3 - Допълнителни кампании
      status: null,
       applicationStatusFilter: 0, // 0 - Неподадени отъствия, 1 - Подадени отсъствия, 2 - Всички
        campaignStatusFilter: 1 // 0 - Неактивна кампания, 1 - Активна кампания, , 2 - Всички
    },
  },
  currency: {},
  personalDevelopmentSuppert_v2: false,
  // currency: {
  //   showAltCurrency: false,
  //   currency: {
  //     code: 'BGN',
  //     name: 'Лев',
  //     description: 'лв.',
  //     exchangeRate: 1.95583,
  //   },
  //   altCurrency: {
  //     code: 'EUR',
  //     name: 'Евро',
  //     description: 'евро',
  //     exchangeRate: 0.511292,
  //   },
};
