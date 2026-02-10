import { Permissions } from '@/enums/enums';
import { config } from '@/common/config';
import i18n from '@/language/language';
import store from '@/store/index';

export const menuItems = () => {
  return [
    { title: i18n.t('menu.home'), iconColor: "primary", icon: 'home', link: '/', visible: true },
    {
      'title': i18n.t('menu.students'),
      'perimission': '',
      'expanded': false,
      iconColor: "primary",
      'icon': 'fa-child',
      'icon-alt': 'mdi-chevron-down',
      visible: store.getters.hasPermission(Permissions.PermissionNameForStudentSearch)
        || store.getters.hasPermission(Permissions.PermissionNameForStudentCreate),
      'children': [
        {
          icon: 'mdi-magnify',
          title: i18n.t('menu.studentSearch'),
          link: '/students',
          visible: store.getters.hasPermission(Permissions.PermissionNameForStudentSearch)
        },
        {
          icon: 'mdi-plus',
          title: i18n.t('menu.studentNew'),
          link: '/student/create',
          visible: store.getters.hasPermission(Permissions.PermissionNameForStudentCreate)
        }
      ],
    },
    {
      title: i18n.t('menu.classes'),
      iconColor: "primary",
      icon: 'fa-university',
      link: '/institutions',
      visible: !store.getters.userInstitutionId
        && store.getters.hasPermission(Permissions.PermissionNameForInstitutionRead)
    },
    {
      title: i18n.t('menu.classes'),
      iconColor: "primary",
      icon: 'fa-university',
      expanded: false,
      visible: store.getters.userInstitutionId
        && store.getters.hasPermission(Permissions.PermissionNameForInstitutionRead),
      children: [
        {
          title: i18n.t('menu.lists.classes'),
          icon: 'mdi-google-classroom',
          link: `/institution/${store.getters.userInstitutionId}/details`,
          visible: store.getters.hasPermission(Permissions.PermissionNameForInstitutionRead)
        },
        {
          title: i18n.t('menu.lists.students'),
          icon: 'mdi-account-group',
          link: `/institution/${store.getters.userInstitutionId}/students`,
          visible: store.getters.hasPermission(Permissions.PermissionNameForInstitutionRead)
        },
        {
          title: i18n.t('menu.lists.externalStudents'),
          icon: 'mdi-lan',
          link: `/institution/${store.getters.userInstitutionId}/external`,
          visible: store.getters.hasPermission(Permissions.PermissionNameForInstitutionRead)
        }
      ]
    },
    {
      title: i18n.t('menu.refugee'),
      iconColor: "primary",
      icon: 'fas fa-umbrella',
      expanded: false,
      visible: store.getters.hasPermission(Permissions.PermissionNameForRefugeeApplicationsManage),
      children: [
        {
          title: i18n.t('menu.refugeeApplications'),
          icon: 'fas fa-umbrella',
          link: '/refugee/applications',
          visible: store.getters.hasPermission(Permissions.PermissionNameForRefugeeApplicationsManage)
        }
      ]
    },
    {
      title: i18n.t('menu.absences'),
      iconColor: "primary",
      icon: 'fas fa-file-import',
      expanded: false,
      visible: store.getters.hasPermission(Permissions.PermissionNameForStudentAbsenceManage)
        || store.getters.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignRead)
        || store.getters.hasPermission(Permissions.PermissionNameForStudentAbsenceRead)
        || store.getters.hasPermission(Permissions.PermissionNameForAbsencesExportRead),
      children: [
        {
          title: i18n.t('menu.absencesImport'),
          icon: 'fas fa-file-import',
          link: '/absence/import',
          visible: store.getters.hasPermission(Permissions.PermissionNameForStudentAbsenceManage)
        },
        {
          title: i18n.t('menu.absencesCampaigns'),
          icon: 'fas fa-tasks',
          link: '/absence/campaigns',
          visible: store.getters.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignRead)
        },
        {
          title: i18n.t('menu.absencesReports'),
          icon: 'mdi-view-dashboard',
          link: '/absence/reports',
          visible: store.getters.hasPermission(Permissions.PermissionNameForStudentAbsenceRead)
        },
        {
          title: i18n.t('menu.absencesExport'),
          icon: 'fas fa-file-export', link: '/absence/absencesExport',
          visible: store.getters.hasPermission(Permissions.PermissionNameForAbsencesExportRead)
        }
      ]
    },
    {
      title: i18n.t('menu.asp.title'),
      iconColor: "primary",
      icon: 'fas fa-book-reader',
      link: '/asp',
      visible: store.getters.hasPermission(Permissions.PermissionNameForASPManage)
        || store.getters.hasPermission(Permissions.PermissionNameForASPImport)
        || store.getters.hasPermission(Permissions.PermissionNameForASPBenefitUpdate)
        || store.getters.hasPermission(Permissions.PermissionNameForASPEnrolledStudentsExport),
      children: [
        {
          title: i18n.t('menu.asp.monthlyBenefitsImport'),
          icon: 'fas fa-file-import',
          link: '/asp/monthlyBenefitsImport',
          visible: store.getters.hasPermission(Permissions.PermissionNameForASPManage)
            || store.getters.hasPermission(Permissions.PermissionNameForASPImport)
            || store.getters.hasPermission(Permissions.PermissionNameForASPBenefitUpdate)
        },
        {
          title: i18n.t('menu.asp.enrolledStudentsExport'),
          icon: 'fas fa-file-export',
          link: '/asp/enrolledStudentsExport',
          visible: store.getters.hasPermission(Permissions.PermissionNameForASPEnrolledStudentsExport)
        },
        {
          title: i18n.t('menu.asp.enrolledStudentSubmittedDataHistory'),
          icon: 'fas fa-history',
          link: '/asp/enrolledStudentSubmittedDataHistory',
          visible: store.getters.hasPermission(Permissions.PermissionNameForASPEnrolledStudentsExport)
        },
      ]
    },
    // {
    //   title: i18n.t('menu.finance.title'),
    //   iconColor: "primary",
    //   icon: 'fas fa-euro-sign',
    //   link: '/finance',
    //   visible: store.getters.hasPermission(Permissions.PermissionNameForNaturalIndicatorsManage) || store.getters.hasPermission(Permissions.PermissionNameForResourceSupportDataManage),
    //   children: [
    //     {
    //       title: i18n.t('menu.finance.naturalIndicators'),
    //       icon: 'fas fa-chart-bar',
    //       link: '/finance/naturalIndicators',
    //       visible: store.getters.hasPermission(Permissions.PermissionNameForNaturalIndicatorsManage)
    //     },
    //     {
    //       title: i18n.t('menu.finance.resourceSupport'),
    //       icon: 'fas fa-chart-bar',
    //       link: '/finance/resourceSupport',
    //       visible: store.getters.hasPermission(Permissions.PermissionNameForResourceSupportDataManage)
    //     }
    //   ]
    // },
    {
      title: i18n.t('menu.healthInsurance.title'),
      iconColor: "primary",
      icon: 'fa-book-medical',
      link: '/healthInsurance',
      visible: store.getters.hasPermission(Permissions.PermissionNameForHealthInsuranceManage),
      children: [
        {
          title: i18n.t('menu.healthInsurance.studentsList'),
          icon: 'fas fa-th-list',
          link: '/healthInsurance/list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForHealthInsuranceManage)
        },
        // {
        //   title: i18n.t('menu.healthInsurance.exportedFilesList'),
        //   icon: 'fas fa-file-export',
        //   link: '/healthInsurance/exports',
        //   visible: store.getters.hasPermission(Permissions.PermissionNameForHealthInsuranceManage)
        // }
      ]
    },
    {
      'title': i18n.t('menu.settings'),
      'expanded': false,
      iconColor: "primary",
      'icon': 'fa fa-tools',
      'icon-alt': 'mdi-chevron-down',
      visible: store.getters.hasPermission(Permissions.PermissionNameForSettingsSectionShow),
      'children': [
        {
          title: i18n.t('menu.nomenclature'),
          link: '/settings',
          icon: 'settings',
          visible: store.getters.hasPermission(Permissions.PermissionNameForNomenclatureRead),
          'children': [
            { title: 'Вид студент', link: '/settings/nomenclatures/StudentType', icon: 'fas fa-user-graduate', visible: true},
            { title: 'Тип ресурсен специалист', link: '/settings/nomenclatures/ResourceSupportSpecialistType', icon: 'settings', visible: true },
            { title: 'Вид пътуване', link: '/settings/nomenclatures/CommuterType', icon: 'fas fa-bus', visible: true },
            { title: 'Основание за отпускане на стипендия', link: '/settings/nomenclatures/ScholarshipType', icon: 'fas fa-coins', visible: true },
            { title: 'Специални потребности', link: '/settings/nomenclatures/SpecialNeedsType', icon: 'fas fa-coins', visible: true },
            { title: 'Подтип Специални потребности', link: '/settings/nomenclatures/SpecialNeedsSubType', icon: 'fas fa-coins', visible: true },
            { title: 'Вид ресурсно подпомагане', link: '/settings/nomenclatures/ResourceSupportType', icon: 'fas fa-coins', visible: true},
            { title: 'Вид специалист ресурсно подпомагане', link: '/settings/nomenclatures/ResourceSupportSpecialistType', icon: 'fas fa-coins', visible: true },
            { title: 'Вид обща подкрепа', link: '/settings/nomenclatures/CommonSupportType', icon: 'fas fa-coins', visible: true },
            { title: 'Вид допълнителна подкрепа', link: '/settings/nomenclatures/AdditionalSupportType', icon: 'fas fa-coins', visible: true },
            { title: 'Основание за записване', link: '/settings/nomenclatures/AdmissionReasonType', icon: 'fas fa-coins', visible: true },
            { title: 'Основание за отписване', link: '/settings/nomenclatures/DischargeReasonType', icon: 'fas fa-coins', visible: true },
            { title: 'Езици', link: '/settings/nomenclatures/Language', icon: 'fas fa-language', visible: true }
          ]
        },
        {
          title: i18n.t('menu.contextualInformation'),
          link: '/administration/contextualInformation',
          icon: 'fas fa-th-list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForContextualInformationManage)
        },
        {
          title: i18n.t('menu.schoolTypeLodAccess'),
          link: '/administration/schoolTypeLodAccess',
          icon: 'fas fa-users-cog',
          visible: store.getters.hasPermission(Permissions.PermissionNameForLodAccessConfigurationShow)
        },
      ]
    },
    {
      title: i18n.t('basicDocument.templates.menu'),
      iconColor: "primary",
      icon: 'mdi-image',
      expanded: false,
      visible: store.getters.hasPermission(Permissions.PermissionNameForTemplatesSectionShow),
      children: [
        {
          title: i18n.t('basicDocument.list'),
          icon: 'fas fa-th-list',
          link: '/basicDocuments',
          visible: store.getters.hasPermission(Permissions.PermissionNameForBasicDocumentsListShow)
        },
        {
          title: i18n.t('basicDocument.templates.list'),
          icon: 'fas fa-list',
          link: '/diplomaTemplate/list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForDiplomaTemplatesRead)
            || store.getters.hasPermission(Permissions.PermissionNameForDiplomaTemplatesManage)
        },
        {
          title: i18n.t('printTemplate.title'),
          icon: 'fas fa-pencil-ruler',
          link: '/printTemplate/list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForPrintTemplatesShow)
        },
        {
          title: i18n.t('basicDocumentSequence.title'),
          icon: 'fas fa-sort-numeric-down',
          link: '/basicDocumentSequence/list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForBasicDocumentSequenceRead)
        },
        {
          title: i18n.t('menu.selfEduForTemplateList'),
          icon: 'mdi-clipboard-list-outline',
          link: '/lod/assessment/template/list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateRead)
            || store.getters.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateManage)
        },
      ]
    },
    {
      title: i18n.t('menu.leadTeacher.title'),
      icon: "fas fa-chalkboard-teacher",
      iconColor: "primary",
      link: `/leadTeacher/list`,
      visible: false && store.getters.hasPermission(Permissions.PermissionNameForLeadTeacherManage)
    },
    {
      title: i18n.t('menu.diplomas.title'),
      icon: "fas fa-certificate",
      iconColor: "primary",
      link: `/diploma/list`,
      visible:
        (store.getters.hasPermission(Permissions.PermissionNameForInstitutionDiplomaRead)
          || store.getters.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
          || store.getters.hasPermission(Permissions.PermissionNameForAdminDiplomaRead)
          || store.getters.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
          || store.getters.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead)
          || store.getters.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage)),
    },
    {
      title: i18n.t('menu.regBooks.title'),
      icon: "fas fa-book",
      iconColor: "primary",
      visible:
        (store.getters.hasPermission(Permissions.PermissionNameForInstitutionDiplomaRead)
          || store.getters.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
          || store.getters.hasPermission(Permissions.PermissionNameForAdminDiplomaRead)
          || store.getters.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
          || store.getters.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead)
          || store.getters.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage)
          ),
          'children': [
            {
              icon: 'mdi-book-open-page-variant-outline',
              title: i18n.t('menu.regBooks.qualifications'),
              link: '/regBook/qualificationList',
              visible: true
            },
            {
              icon: 'mdi-book-open-page-variant-outline',
              title: i18n.t('menu.regBooks.qualificationDuplicates'),
              link: '/regBook/qualificationDuplicatesList',
              visible: true
            },
            {
              icon: 'mdi-book-open-page-variant-outline',
              title: i18n.t('menu.regBooks.certifications'),
              link: '/regBook/certificateList',
              visible: true
            },
            {
              icon: 'mdi-book-open-page-variant-outline',
              title: i18n.t('menu.regBooks.certificationDuplicates'),
              link: '/regBook/certificationDuplicatesList',
              visible: true
            },
          ]
    },
    {
      title: i18n.t('student.menu.ores'),
      icon: 'fas fa-viruses',
      iconColor: 'primary',
      link: `/ores`,
      visible: store.getters.turnOnOresModule && store.getters.hasPermission(Permissions.PermissionNameForOresRead),
    },
    {
      title: i18n.t('student.menu.lodFinalizations'),
      icon: 'fa-file-signature',
      iconColor: 'primary',
      link: `/lodFinalizations`,
      visible: store.getters.turnOnOresModule && store.getters.hasPermission(Permissions.PermissionNameForLodFinalizationRead),
    },
    {
      title: i18n.t('menu.docsWithFactoryNumber'),
      iconColor: "primary",
      icon: 'fas fa-barcode',
      expanded: false,
      visible: store.getters.hasPermission(Permissions.PermissionNameForDocManagementCampaignRead)
        || store.getters.hasPermission(Permissions.PermissionNameForDocManagementCampaignManage)
        || store.getters.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead)
        || store.getters.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage),
      children: [
        {
          title: i18n.tc('docManagement.campaign.title', 2),
          icon: 'mdi-timeline',
          link: '/docManagement/campaigns',
          visible: store.getters.hasPermission(Permissions.PermissionNameForDocManagementCampaignRead)
        },
        {
          title: i18n.tc('docManagement.application.title', 2),
          icon: 'mdi-list-box',
          link: '/docManagement/applications',
          visible: store.getters.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead)
        },
        {
          title: i18n.tc('docManagement.unusedDocs.title', 2),
          icon: 'mdi-list-box',
          link: '/docManagement/unusedDocs',
          visible: store.getters.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead)
        },
        {
          title: i18n.tc('docManagement.report.title', 2),
          icon: 'mdi-poll',
          link: '/docManagement/report',
          visible: store.getters.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead)
        },
        {
          title: i18n.tc('docManagement.menuTitle'),
          icon: 'mdi-view-dashboard',
          link: '/docManagement/dashboard',
          visible: store.getters.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead)
        },
      ]
    },
    {
      title: i18n.t('menu.help'),
      iconColor: "primary",
      icon: 'mdi-help-box',
      link: config.spaBaseUrlRelative + 'docs/index.html',
      isExternal: true,
      visible: store.getters.hasPermission(Permissions.PermissionNameForDocumentationShow)
    },
    {
      'title': i18n.t('menu.administration'),
      'expanded': false,
      iconColor: "primary",
      'icon': 'fa-cog',
      'icon-alt': 'mdi-chevron-down',
      visible: store.getters.hasPermission(Permissions.PermissionNameForAdministrationSectionShow),
      'children': [
        {
          title: i18n.t('menu.auditLogs'),
          icon: 'mdi-table-large',
          link: '/administration/auditLogs',
          visible: store.getters.hasPermission(Permissions.PermissionNameForAuditLogsShow)
        },
        {
          title: i18n.t('menu.dashboard'),
          icon: 'mdi-table-large',
          link: '/administration/dashboard',
          visible: store.getters.hasPermission(Permissions.PermissionNameForAdministrationSectionShow)
        },
        {
          title: i18n.t('menu.nomenclature'),
          link: '/administration/nomenclatures',
          icon: 'fas fa-coins',
          visible: store.getters.hasPermission(Permissions.PermissionNameForNomenclatureManage)
        },
        {
          title: i18n.t('menu.permissionsDocumentation'),
          link: '/administration/permissionsDocumentation',
          icon: 'fas fa-th-list',
          visible: true
        },
        {
          title: i18n.t('menu.certifications'),
          link: '/administration/certificates',
          icon: 'fa-solid fa-certificate',
          visible: store.getters.hasPermission(Permissions.PermissionNameForCertificatesRead)
        },
        {
          title: i18n.t('menu.diplomaImportValidationExclusions'),
          link: '/administration/diplomaImportValidationExclusions',
          icon: 'fa-solid fa-list',
          visible: store.getters.hasPermission(Permissions.PermissionNameForStudentDiplomaImportValidationExclusionsManage)
        },
        {
          title: i18n.t('menu.cacheService'),
          link: '/administration/cacheService',
          icon: 'mdi-cached',
          visible: store.getters.hasPermission(Permissions.PermissionNameForCacheServiceManage)
        },
        {
          title: i18n.t('student.menu.lodFinalizations'),
          link: '/administration/lodFinalization',
          icon: 'fa-file-signature',
          visible: store.getters.hasPermission(Permissions.PermissionNameForLodFinalizationAdministration)
        },
      ]
    },
    {
      title: i18n.t('menu.lodAssessmentImport'),
      iconColor: "primary",
      icon: 'mdi-file-import',
      link: '/lod/assessment/import',
      visible: store.getters.hasPermission(Permissions.PermissionNameForLodAssessmentImport)
    },
    { divider: true },
    {
      'title': i18n.t('menu.demonstration'),
      'expanded': false,
      iconColor: "primary",
      'icon': 'fa-icons',
      'icon-alt': 'mdi-chevron-down',
      visible: store.getters.hasPermission(Permissions.PermissionNameForDemonstrationSectionShow),
      'children': [
        { title: i18n.t('menu.helpersDocumentation'), icon: 'mdi-test-tube', link: '/demonstration/helpers', visible: true },
        { title: i18n.t('menu.theme'), icon: 'category', link: '/demonstration/theme', visible: true },
        { title: i18n.t('menu.print'), icon: 'print', link: '/demonstration/print/Student%5CThirdAndFifthClass%5CThirdAndFifthClassRelocationDocument/55', visible: true },
        { title: i18n.t('menu.reportDesigner'), icon: 'print', link: '/demonstration/reportDesigner/1', visible: true },
        { title: i18n.t('menu.testDiplomaPrint'), icon: 'print', link: '/demonstration/print/Diploma%5CDiploma/24', visible: true },
        { title: i18n.t('dashboards.directorDashboardTitle'), icon: 'mdi-table-large', link: '/administration/directorDashboard', visible: store.getters.hasPermission(Permissions.PermissionNameForDashboardShow) },
        { title: 'File manager', icon: 'mdi-table-large', link: '/test/fileManager', visible: true },
        { title: 'Regix', icon: 'mdi-connection', link: '/test/regix', visible: true },
      ]
    }
  ];
};
