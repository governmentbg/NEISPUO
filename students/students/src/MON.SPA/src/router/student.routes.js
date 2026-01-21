import Helpers from '@/components/helper';
import { Permissions } from '@/enums/enums';
import i18n from '@/language/language';

export default [
  {
    path: '/students',
    meta: {
      title: 'Деца и ученици'
    },
    name: 'Students',
    component: () => import('@/views/students/Students.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentSearch });
    },
  },
  {
    path: '/student/create',
    meta: {
      title: 'Добавяне на ученик'
    },
    name: 'CreateStudent',
    component: () => import('@/views/students/CreateStudent.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentCreate });
    },
  },
  {
    path: '/asp/monthlyBenefitsImport',
    meta: {
      title: i18n.t('menu.absencesImport')
    },
    name: 'AspImportMonthlyBenefits',
    component: () => import('@/views/asp/ImportMonthlyBenefitsView.vue')
  },
  {
    path: '/asp/monthlyBenefitsImport/:year/:month/benefitDetails/:importedFileId',
    meta: {
      title: i18n.t('menu.importedFileDetails')
    },
    name: 'ImportedBenefitFileDetails',
    component: () => import('@/views/asp/ImportedMonthlyBenefitsDetailsView.vue'),
    props: (route) => ({
      importedFileId: Number(route.params.importedFileId),
      year: Number(route.params.year),
      month: Number(route.params.month),
      gridStatusFilter: Number(route.query.gridStatusFilter)
    }),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForASPImportDetailsShow });
    },
  },
  {
    path: '/asp/monthlyBenefitsImport/:importedFileId/edit',
    meta: {
      title: i18n.t('menu.importedFileDetails')
    },
    name: 'ImportedMonthlyBenefitFileMetaDataEdit',
    component: () => import('@/views/asp/ImportedMonthlyBenefitFileMetaDataEditView.vue'),
    props: (route) => ({ importedFileId: Number(route.params.importedFileId)  })
  },
  {
    path: '/asp/enrolledStudentsExport',
    meta: {
      title: i18n.t('menu.asp.enrolledStudentsExport')
    },
    name: 'EnrolledStudentsExport',
    component: () => import('@/views/asp/EnrolledStudentsExportView.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForASPEnrolledStudentsExport });
    },
  },
  {
    path: '/asp/enrolledStudentSubmittedDataHistory',
    meta: {
      title: i18n.t('menu.asp.enrolledStudentSubmittedDataHistory')
    },
    name: 'EnrolledStudentSubmittedDataHistory',
    component: () => import('@/views/asp/EnrolledStudentSubmittedDataHistory.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForASPEnrolledStudentsExport });
    },
  },
  {
    path: '/studentAdmissionDocumentPermissionRequests',
    name: 'StudentAdmissionDocumentPermissionRequestList',
    component: () => import('@/views/students/details/StudentAdmissionDocumentPermissionRequestList.vue'),
    meta: {
      title: i18n.t('admissionDocument.permissionRequest')
    },
  },
  {
    path: '/studentAdmissionDocumentPermissionRequest/create',
    name: 'StudentAdmissionDocumentPermissionRequestCreate',
    component: () => import('@/views/students/details/StudentAdmissionDocumentPermissionRequestCreate.vue'),
    meta: {
      title: i18n.t('admissionDocument.permissionRequest')
    },
    props: (route) => ({ personId: Number(route.query.personId), requestingInstitutionId: Number(route.query.requestingInstitutionId), authorizingInstitutionId: Number(route.query.authorizingInstitutionId) }),
  },
  {
    path: '/studentAdmissionDocumentPermissionRequest/:id/edit',
    name: 'StudentAdmissionDocumentPermissionRequestEdit',
    component: () => import('@/views/students/details/StudentAdmissionDocumentPermissionRequestEdit.vue'),
    meta: {
      title: i18n.t('admissionDocument.permissionRequest')
    },
    props: (route) => ({ id: Number(route.params.id)  }),
  },
  // Test student nested router-view.
  // Не трябва да защитаваме /student/:id раута, който е layout-а за ученика.
  // Освен това не трябва да защитаваме раута по подразбиране (/student/:id/details).
  // При beforeEnter все още не са се заредили правата за дадения ученик.
  // Второто не важи, ако раута по подразбиране е /student/:id. Тогава ще се заредят правата и всички от
  // children може да се проверява.
  {
    path: '/student/:id',
    name: 'StudentDetailsLayout',
    component: () => import('@/views/StudentDetailsLayout.vue'),
    meta: {
      title: i18n.tc('student.title', 1)
    },
    props: (route) => ({ id: Number(route.params.id) }),
    children: [
      {
        path: 'details',
        name: 'StudentDetails',
        component: () => import('@/views/students/details/StudentBasicDetails.vue'),
        meta: {
          title: i18n.t('student.menu.details')
        },
        props: (route) => ({ id: Number(route.params.id) })
      },

      {
        path: 'otherInstitutions/create',
        name: 'AddOtherInstitution',
        component: () => import('@/views/students/details/otherInstitution/OtherInstitutionCreate.vue'),
        meta: {
          title: i18n.t('studentOtherInstitutions.title')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'otherInstitutions',
        component: () => import('@/views/students/details/otherInstitution/OtherInstitutionList.vue'),
        meta: {
          title: i18n.t('studentOtherInstitutions.title')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'otherInstitutions/:institutionId/details',
        name: 'OtherInstitutionDetails',
        component: () => import('@/views/students/details/otherInstitution/OtherInstitutionDetails.vue'),
        meta: {
          title: i18n.t('studentOtherInstitutions.title')
        },
        props: (route) => ({ personId: Number(route.params.id), institutionId: Number(route.params.institutionId) }),
      },
      {
        path: 'otherInstitutions/:institutionId/edit',
        name: 'EditOtherInstitution',
        component: () => import('@/views/students/details/otherInstitution/OtherInstitutionEdit.vue'),
        meta: {
          title: i18n.t('studentOtherInstitutions.title')
        },
        props: (route) => ({ personId: Number(route.params.id), institutionId: Number(route.params.institutionId) }),
      },
      {
        path: 'lodFinalizations',
        component: () => import('@/views/students/lod/StudentLodFinalizationList.vue'),
        meta: {
          title: i18n.t('student.menu.lodFinalizations')
        },
        props: (route) => ({ id: Number(route.params.id) }),
      },
      {
        path: 'notes',
        component: () => import('@/views/students/details/notes/StudentNotesList.vue'),
        meta: {
          title: i18n.t('student.menu.notes')
        },
        props: (route) => ({ id: Number(route.params.id) }),
      },
      {
        path: 'note/create',
        component: () => import('@/views/students/details/notes/StudentNoteCreate.vue'),
        meta: {
          title: i18n.t('student.menu.notes')
        },
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'note/:noteId/edit',
        component: () => import('@/views/students/details/notes/StudentNoteEdit.vue'),
        meta: {
          title: i18n.t('student.menu.notes')
        },
        props: (route) => ({ pid: Number(route.params.id), noteId: Number(route.params.noteId) })
      },
      {
        path: 'note/:noteId/details',
        component: () => import('@/views/students/details/notes/StudentNoteDetails.vue'),
        meta: {
          title: i18n.t('student.menu.notes')
        },
        props: (route) => ({ pid: Number(route.params.id), noteId: Number(route.params.noteId) })
      },
      {
        path: 'otherDocuments',
        component: () => import('@/views/students/details/StudentOtherDocumentList.vue'),
        meta: {
          title: i18n.t('student.menu.otherDocuments')
        },
        props: (route) => ({ id: Number(route.params.id) }),
      },
      {
        path: 'otherDocument/create',
        component: () => import('@/views/students/details/StudentOtherDocumentCreate.vue'),
        meta: {
          title: i18n.t('student.menu.otherDocuments')
        },
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'otherDocument/:docId/edit',
        component: () => import('@/views/students/details/StudentOtherDocumentEdit.vue'),
        meta: {
          title: i18n.t('student.menu.otherDocuments')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'otherDocument/:docId/details',
        component: () => import('@/views/students/details/StudentOtherDocumentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.otherDocuments')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'personalDevelopment/',
        name: 'StudentPersonalDevelopmentList',
        component: () => import('@/views/students/personalDevelopment/StudentPersonalDevelopmentList.vue'),
        meta: {
          title: `${i18n.t('personalDevelopment.title')} / ${i18n.t('sop.subtitle')}`
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'personalDevelopment/create',
        name: 'StudentPersonalDevelopmentCreate',
        component: () => import('@/views/students/personalDevelopment/StudentPersonalDevelopmentCreate.vue'),
        meta: {
          title: i18n.t('personalDevelopment.createTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), schoolYear: Number(route.query.schoolYear) }),
      },
      {
        path: 'personalDevelopment/:pdId/edit',
        name: 'StudentPersonalDevelopmentEdit',
        component: () => import('@/views/students/personalDevelopment/StudentPersonalDevelopmentEdit.vue'),
        meta: {
          title: i18n.t('personalDevelopment.editTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), pdId: Number(route.params.pdId) })
      },
      {
        path: 'personalDevelopment/:pdId/details',
        name: 'StudentPersonalDevelopmentDetails',
        component: () => import('@/views/students/personalDevelopment/StudentPersonalDevelopmentDetails.vue'),
        meta: {
          title: i18n.t('personalDevelopment.reviewTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), pdId: Number(route.params.pdId) })
      },
      {
        path: 'earlyAssessment/',
        name: 'StudentEarlyAssessment',
        component: () => import('@/views/students/personalDevelopment/StudentEarlyAssessment.vue'),
        meta: {
          title: i18n.t('student.menu.earlyAssessment')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/documents',
        component: () => import('@/views/students/details/StudentLodDocuments.vue'),
        meta: {
          title: i18n.t('student.menu.lodDocuments')
        },
        props: (route) => ({ id: Number(route.params.id) }),
        beforeEnter: (to, from, next) => {
          Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentLodDocumentsShow, studentId: Number(to.params.id) });
        },
      },
      {
          path: 'lod/awards',
          component: () => import('@/views/students/details/awards/AwardsList.vue'),
          meta: {
            title: i18n.t('student.menu.studentAwards')
          },
          props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/award/create',
        name: 'AddAward',
        component: () => import('@/views/students/details/awards/AwardCreate.vue'),
        meta: {
          title: i18n.t('menu.addAwardTitle')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/award/:awardId/edit',
        name: 'EditAward',
        component: () => import('@/views/students/details/awards/AwardEdit.vue'),
        meta: {
          title: i18n.t('menu.addAwardTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), awardId: Number(route.params.awardId) }),
      },
      {
        path: 'lod/award/:awardId/details',
        name: 'AwardDetails',
        component: () => import('@/views/students/details/awards/AwardDetails.vue'),
        meta: {
          title: i18n.t('menu.addAwardTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), awardId: Number(route.params.awardId) }),
      },
      {
        path: 'lod/sanctions',
        component: () => import('@/views/students/details/sanctions/SanctionsList.vue'),
        meta: {
          title: i18n.t('student.menu.studentSanctions')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/sanction/create',
        component: () => import('@/views/students/details/sanctions/SanctionCreate.vue'),
        meta: {
          title: i18n.t('student.menu.studentSanctions')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/sanction/:sanctionId/edit',
        component: () => import('@/views/students/details/sanctions/SanctionEdit.vue'),
        meta: {
          title: i18n.t('student.menu.studentSanctions')
        },
        props: (route) => ({ personId: Number(route.params.id), sanctionId: Number(route.params.sanctionId) } ),
      },
      {
        path: 'lod/sanction/:sanctionId/details',
        component: () => import('@/views/students/details/sanctions/SanctionDetails.vue'),
        meta: {
          title: i18n.t('student.menu.studentSanctions')
        },
        props: (route) => ({ personId: Number(route.params.id), sanctionId: Number(route.params.sanctionId) } ),
      },
      {
        path: 'lod/selfGovernments',
        component: () => import('@/views/students/details/selfGovernment/SelfGovernmentList.vue'),
        meta: {
          title: i18n.t('student.menu.selfGovernment')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/selfGovernment/create',
        component: () => import('@/views/students/details/selfGovernment/SelfGovernmentCreate.vue'),
        meta: {
          title: i18n.t('student.menu.selfGovernment')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/selfGovernment/:selfGovernmentdId/edit',
        component: () => import('@/views/students/details/selfGovernment/SelfGovernmentEdit.vue'),
        meta: {
          title: i18n.t('student.menu.selfGovernment')
        },
        props: (route) => ({ personId: Number(route.params.id), selfGovernmentdId: Number(route.params.selfGovernmentdId) }),
      },
      {
        path: 'lod/selfGovernment/:selfGovernmentdId/details',
        component: () => import('@/views/students/details/selfGovernment/SelfGovernmentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.selfGovernment')
        },
        props: (route) => ({ personId: Number(route.params.id), selfGovernmentdId: Number(route.params.selfGovernmentdId) }),
      },
      {
        path: 'lod/internationalMobilities',
        component: () => import('@/views/students/details/internationalMobility/InternationalMobilityList.vue'),
        meta: {
          title: i18n.t('student.menu.internationalMobility')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/internationalMobility/create',
        component: () => import('@/views/students/details/internationalMobility/InternationalMobilityCreate.vue'),
        meta: {
          title: i18n.t('student.menu.internationalMobility')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/internationalMobility/:internationalMobilityId/details',
        component: () => import('@/views/students/details/internationalMobility/InternationalMobilityDetails.vue'),
        meta: {
          title: i18n.t('student.menu.internationalMobility')
        },
        props: (route) => ({ personId: Number(route.params.id),  internationalMobilityId: Number(route.params.internationalMobilityId) }),
      },
      {
        path: 'lod/internationalMobility/:internationalMobilityId/edit',
        component: () => import('@/views/students/details/internationalMobility/InternationalMobilityEdit.vue'),
        meta: {
          title: i18n.t('student.menu.internationalMobility')
        },
        props: (route) => ({ personId: Number(route.params.id),  internationalMobilityId: Number(route.params.internationalMobilityId) }),
      },
      {
        path: 'scholarships',
        component: () => import('@/views/students/details/scholarships/ScholarshipsList.vue'),
        meta: {
          title: i18n.t('student.menu.scholarships')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'scholarship/add',
        component: () => import('@/views/students/details/scholarships/ScholarshipCreate.vue'),
        meta: {
          title: i18n.t('student.menu.scholarships')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'scholarship/:scholarshipId/edit',
        component: () => import('@/views/students/details/scholarships/ScholarshipEdit.vue'),
        meta: {
          title: i18n.t('student.menu.scholarships')
        },
        props: (route) => ({ personId: Number(route.params.id), scholarshipId: Number(route.params.scholarshipId) }),
      },
      {
        path: 'scholarship/:scholarshipId/details',
        component: () => import('@/views/students/details/scholarships/ScholarshipDetails.vue'),
        meta: {
          title: i18n.t('student.menu.scholarships')
        },
        props: (route) => ({ personId: Number(route.params.id), scholarshipId: Number(route.params.scholarshipId) }),
      },
      {
        path: 'sop',
        component: () => import('@/views/students/sop/StudentSopList.vue'),
        meta: {
          title: `${i18n.t('sop.sopTitle')} / ${i18n.t('sop.subtitle')}`
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'sop/create',
        name: 'StudentSopCreate',
        component: () => import('@/views/students/sop/StudentSopCreate.vue'),
        meta: {
          title: i18n.t('sop.createTitle')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'sop/:sopId/edit',
        name: 'StudentSopEdit',
        component: () => import('@/views/students/sop/StudentSopEdit.vue'),
        meta: {
          title: i18n.t('sop.editTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), sopId: Number(route.params.sopId) })
      },
      {
        path: 'sop/:sopId/details',
        name: 'StudentSopDetails',
        component: () => import('@/views/students/sop/StudentSopDetails.vue'),
        meta: {
          title: i18n.t('sop.reviewTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), sopId: Number(route.params.sopId) })
      },
      {
        path: 'resourceSupports',
        meta: {
          title: i18n.t('student.menu.resourceSupport')
        },
        component: () => import('@/views/students/details/resourceSupport/ResourceSupportList.vue'),
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'resourceSupport/create',
        name: 'ResourceSupportCreate',
        meta: {
          title: i18n.t('student.menu.resourceSupport')
        },
        component: () => import('@/views/students/details/resourceSupport/ResourceSupportCreate.vue'),
        props: (route) => ({ personId: Number(route.params.id), schoolYear: Number(route.query.schoolYear) }),
      },
      {
        path: 'resourceSupport/:reportId/edit',
        component: () => import('@/views/students/details/resourceSupport/ResourceSupportEdit.vue'),
        meta: {
          title: i18n.t('student.menu.resourceSupport')
        },
        props: (route) => ({ personId: Number(route.params.id), reportId: Number(route.params.reportId) })
      },
      {
        path: 'resourceSupport/:reportId/details',
        component: () => import('@/views/students/details/resourceSupport/ResourceSupportDetails.vue'),
        meta: {
          title: i18n.t('student.menu.resourceSupport')
        },
        props: (route) => ({ personId: Number(route.params.id), reportId: Number(route.params.reportId) })
      },
      {
        path: 'environmentCharacteristics',
        meta: {
          title: i18n.t('student.menu.environmentCharacteristics')
        },
        component: () => import('@/views/students/details/environmentCharacteristics/EnvironmentCharacteristicsDetails.vue'),
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'environmentCharacteristics/relative/add',
        meta: {
          title: i18n.t('student.menu.environmentCharacteristics')
        },
        component: () => import('@/views/students/details/environmentCharacteristics/RelativeAdd.vue'),
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'environmentCharacteristics/relative/:relativeId/edit',
        meta: {
          title: i18n.t('student.menu.environmentCharacteristics')
        },
        component: () => import('@/views/students/details/environmentCharacteristics/RelativeEdit.vue'),
        props: (route) => ({ personId: Number(route.params.id), relativeId: Number(route.params.relativeId)  }),
      },
      {
        path: 'generalTrainingData',
        meta: {
          title: i18n.t('student.menu.generalTrainingData')
        },
        component: () => import('@/views/students/details/StudentGeneralTrainingData.vue'),
        props: (route) => ({ id: Number(route.params.id) }),
      },
      {
        path: 'preSchoolEvaluations',
        meta: {
          title: i18n.t('student.menu.preSchoolEvaluation')
        },
        component: () => import('@/views/preSchoolEval/StudentPreSchoolEvaluationsList.vue'),
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'preSchoolEvaluation/:evalId/details',
        meta: {
          title: i18n.t('student.menu.preSchoolEvaluation')
        },
        component: () => import('@/views/preSchoolEval/StudentPreSchoolEvaluationDetails.vue'),
        props: (route) => ({ personId: Number(route.params.id), evalId: Number(route.params.evalId) }),
      },
      {
        path: 'preSchoolEvaluation/:evalId/edit',
        meta: {
          title: i18n.t('student.menu.preSchoolEvaluation')
        },
        component: () => import('@/views/preSchoolEval/StudentPreSchoolEvaluationEdit.vue'),
        props: (route) => ({ personId: Number(route.params.id), evalId: Number(route.params.evalId) }),
      },
      {
        path: 'generalTrainingData/:institution/details',
        meta: {
          title: i18n.t('generalTrainingData.details.title')
        },
        component: () => import('@/views/students/details/StudentGeneralTrainingDataDetails.vue'),
        props: (route) => ( { studentId: Number(route.params.id), institutionId: Number(route.params.institution), classId: Number(route.query.classId) }),
      },
      {
        path: 'institutionDetails',
        meta: {
          title: i18n.t('student.menu.institutionDetails')
        },
        component: () => import('@/views/students/details/StudentCurrentInstitutionDetails.vue'),
        props: (route) => ({ id: Number(route.params.id) }),
      },
      {
        path: 'externalEval',
        component: () => import('@/views/students/details/ExternalEvaluation.vue'),
        meta: {
          title: i18n.t('student.menu.externalEval')
        },
        props: (route) => ( { studentId: Number(route.params.id) }),
      },
      {
        path: 'classes',
        name: 'StudentClasses',
        component: () => import('@/views/students/class/Classes.vue'),
        meta: {
          title: i18n.t('student.menu.classes')
        },
        props: (route) => ({ pid: Number(route.params.id), showProfile: false })
      },
      {
        path: 'class/:class/history',
        name: 'StudentClass',
        meta: {
          title: i18n.t('studentClass.history')
        },
        component: () => import('@/views/students/class/History.vue'),
        props: (route) => ({ pid: Number(route.params.id), id: Number(route.params.class) }),
      },
      {
        path: 'class/initialEnrollment',
        name: 'StudentClassInitialEnrollment',
        meta: {
          title: i18n.t('student.enroll')
        },
        component: () => import('@/views/students/class/InitialEnrollment.vue'),
        props: (route) => ({ pid: Number(route.params.id), admissionDocumentId: Number(route.query.admissionDocumentId), initialEnrollmentPosition: Number(route.query.initialEnrollmentPosition) })
      },
      {
        path: 'class/initialCplrEnrollment',
        name: 'StudentClassInitialCprlEnrollment',
        meta: {
          title: i18n.t('student.enroll')
        },
        component: () => import('@/views/students/class/InitialCplrEnrollment.vue'),
        props: (route) => ({ pid: Number(route.params.id), admissionDocumentId: Number(route.query.admissionDocumentId) })
      },
      {
        path: 'class/enroll', // Запис е допълнителна група/паралелка (не през документ за записване). Бутон в ученическите профилни данни.
        meta: {
          title: i18n.t('student.enroll')
        },
        component: () => import('@/views/students/class/Enrollment.vue'),
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'class/cplrEnroll', // Запис в допълнителна група/паралелка (не през документ за записване). Бутон в ученическите профилни данни.
        meta: {
          title: i18n.t('student.enroll')
        },
        component: () => import('@/views/students/class/Enrollment.vue'),
        props: (route) => ({ pid: Number(route.params.id), isCplrEnrollment: true })
      },
      {
        path: 'class/enroll/edit', // Редакция на група/паралелка
        meta: {
          title: i18n.t('student.enrollEdit')
        },
        component: () => import('@/views/students/class/EnrollmentEdit.vue'),
        props: (route) => ({ pid: Number(route.params.id), studentClassId: Number(route.query.studentClassId), classId: Number(route.query.classId) })
      },
      {
        path: 'class/enroll/change', // Местене от една в друга група паралелка в рамките на инстируцията
        meta: {
          title: i18n.t('student.enrollChange')
        },
        component: () => import('@/views/students/class/EnrollmentChange.vue'),
        props: (route) => ({ pid: Number(route.params.id), studentClassId: Number(route.query.studentClassId), classId: Number(route.query.classId) })
      },
      // Да се скрие меню "Отсъствия" в ЛОД #1201
        // https://github.com/Neispuo/students/issues/1201
      // {
      //   path: 'absences',
      //   component: () => import('@/views/absence/StudentAbsence.vue'),
      //   meta: {
      //     title: i18n.t('student.menu.absences')
      //   },
      //   props: (route) => ({ id: Number(route.params.id) })
      // },
      {
        path: 'class/change',
        component: () => import('@/views/students/class/StudentClassChange.vue'),
        meta: {
          title: i18n.t('student.movement.move')
        },
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'class/:classId/edit',
        component: () => import('@/views/students/class/StudentClassEdit.vue'),
        meta: {
          title: i18n.t('studentClass.edit')
        },
        props: (route) => ({ pid: Number(route.params.id), classId: Number(route.params.classId) })
      },
      {
        path: 'admissionDocuments',
        component: () => import('@/views/students/details/StudentAdmissionDocumentList.vue'),
        meta: {
          title: i18n.t('student.menu.admissionDocuments')
        },
        props: (route) => ({ id: Number(route.params.id) })
      },
      {
        path: 'admissionDocument/create',
        component: () => import('@/views/students/details/StudentAdmissionDocumentCreate.vue'),
        meta: {
          title: i18n.t('student.menu.admission')
        },
        props: (route) => ({ personId: Number(route.params.id), studentClassId: Number(route.query.studentClassId) })
      },
      {
        path: 'admissionDocument/:docId/edit',
        component: () => import('@/views/students/details/StudentAdmissionDocumentEdit.vue'),
        meta: {
          title: i18n.t('student.menu.admission')
        },
        props: (route) => ({ personId: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'admissionDocument/:docId/details',
        component: () => import('@/views/students/details/StudentAdmissionDocumentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.admission')
        },
        props: (route) => ({ personId: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'relocationDocuments',
        component: () => import('@/views/students/details/StudentRelocationDocumentList.vue'),
        meta: {
          title: i18n.t('student.menu.relocationDocuments')
        },
        props: (route) => ({ id: Number(route.params.id) })
      },
      {
        path: 'relocationDocument/create',
        component: () => import('@/views/students/details/StudentRelocationDocumentCreate.vue'),
        meta: {
          title: i18n.t('student.menu.relocation')
        },
        props: (route) => ({
          personId: Number(route.params.id),
          studentClassId: Number(route.query.studentClassId),
          hostingInstitution: (route.query.hostingInstitution ? Number(route.query.hostingInstitution) : null)
        })
      },
      {
        path: 'relocationDocument/:docId/edit',
        component: () => import('@/views/students/details/StudentRelocationDocumentEdit.vue'),
        meta: {
          title: i18n.t('student.menu.relocation')
        },
        props: (route) => ({ personId: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'relocationDocument/:docId/details',
        component: () => import('@/views/students/details/StudentRelocationDocumentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.relocation')
        },
        props: (route) => ({ personId: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'dischargeDocuments',
        component: () => import('@/views/students/details/StudentDischargeDocumentList.vue'),
        meta: {
          title: i18n.t('student.menu.dischargeDocuments')
        },
        props: (route) => ({ id: Number(route.params.id) })
      },
      {
        path: 'dischargeDocument/create',
        component: () => import('@/views/students/details/StudentDischargeDocumentCreate.vue'),
        meta: {
          title: i18n.t('student.menu.discharge')
        },
        props: (route) => ({ personId: Number(route.params.id), studentClassId: Number(route.query.studentClassId) })
      },
      {
        path: 'dischargeDocument/:docId/edit',
        component: () => import('@/views/students/details/StudentDischargeDocumentEdit.vue'),
        meta: {
          title: i18n.t('student.menu.discharge')
        },
        props: (route) => ({ personId: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'dischargeDocument/:docId/details',
        component: () => import('@/views/students/details/StudentDischargeDocumentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.discharge')
        },
        props: (route) => ({ personId: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'recognitions',
        component: () => import('@/views/students/StudentRecognitionsList.vue'),
        meta: {
          title: i18n.t('student.menu.recognitions')
        },
        props: (route) => ({ pid: Number(route.params.id) }),
      },
      {
        path: 'recognition/create',
        component: () => import('@/views/students/StudentRecognitionCreate.vue'),
        meta: {
          title: i18n.t('student.menu.recognitions')
        },
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'recognition/:docId/edit',
        component: () => import('@/views/students/StudentRecognitionEdit.vue'),
        meta: {
          title: i18n.t('student.menu.recognitions')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'recognition/:docId/details',
        component: () => import('@/views/students/StudentRecognitionDetails.vue'),
        meta: {
          title: i18n.t('student.menu.recognitions')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'equalizations',
        component: () => import('@/views/students/StudentEqualizationsList.vue'),
        meta: {
          title: i18n.t('student.menu.equalizations')
        },
        props: (route) => ({ pid: Number(route.params.id) }),
      },
      {
        path: 'equalization/create',
        component: () => import('@/views/students/StudentEqualizationCreate.vue'),
        meta: {
          title: i18n.t('student.menu.equalizations')
        },
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'equalization/:docId/edit',
        component: () => import('@/views/students/StudentEqualizationEdit.vue'),
        meta: {
          title: i18n.t('student.menu.equalizations')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'equalization/:docId/details',
        component: () => import('@/views/students/StudentEqualizationDetails.vue'),
        meta: {
          title: i18n.t('student.menu.equalizations')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'validation/:diplomaId/documents',
        name: "ValidationDocumentDocuments",
        component: () => import('@/views/students/details/StudentDiplomaDocuments.vue'),
        meta: {
          title: i18n.t('validationDocument.menuTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), diplomaId: Number(route.params.diplomaId), isValidationDocument: true })
      },
      {
        path: 'diplomas',
        name: 'StudentDiplomasList',
        component: () => import('@/views/students/details/StudentDiplomasListView.vue'),
        meta: {
          title: i18n.t('menu.diplomas.title')
        },
        props: (route) => ({ id: Number(route.params.id), isValidation: false }),
      },
      {
        path: 'diploma/create',
        name: 'StudentDiplomaCreate',
        component: () => import('@/views/diploma/StudentDiplomaCreateView.vue'),
        meta: {
          title: i18n.t('menu.diplomas.title')
        },
        props: (route) => ({
          personId: Number(route.params.id),
          templateId: route.query.templateId ? Number(route.query.templateId) : null,
          basicDocumentId: route.query.basicDocumentId ? Number(route.query.basicDocumentId) : null,
          basicClassId: route.query.basicClassId ? Number(route.query.basicClassId) : null
        }),
      },
      {
        path: 'diploma/:diplomaId/edit',
        name: 'StudentDiplomaEdit',
        component: () => import('@/views/diploma/StudentDiplomaEditView.vue'),
        meta: {
          title: i18n.t('menu.diplomas.title')
        },
        props: (route) => ({ diplomaId: Number(route.params.diplomaId) }),
      },
      {
        path: 'diploma/:diplomaId/review',
        name: 'StudentDiplomaReview',
        component: () => import('@/views/diploma/StudentDiplomaDetailsView.vue'),
        meta: {
          title: i18n.t('menu.diplomas.title')
        },
        props: (route) => ({ diplomaId: Number(route.params.diplomaId) }),
      },
      {
        path: 'validations',
        name: 'StudentValidationsList',
        component: () => import('@/views/students/details/StudentDiplomasListView.vue'),
        meta: {
          title: i18n.t('validationDocument.menuTitle')
        },
        props: (route) => ({ id: Number(route.params.id), isValidation: true }),
      },
      {
        path: 'diploma/:diplomaId/documents',
        name: "DiplomaDocuments",
        component: () => import('@/views/students/details/StudentDiplomaDocuments.vue'),
        meta: {
          title: i18n.t('menu.diplomas.title')
        },
        props: (route) => ({ personId: Number(route.params.id), diplomaId: Number(route.params.diplomaId) })
      },
      {
        path: 'class/:classId/addCurriculum/:schoolYear',
        component: () => import('@/views/students/class/AddCurriculum.vue'),
        meta: {
          title: i18n.t('student.menu.addCurriculum')
        },
        props: (route) => ({ pid: Number(route.params.id), classId: Number(route.params.classId), schoolYear: Number(route.params.schoolYear) })
      },
      {
        path: 'lod/assessments',
        name: 'StudentLodAssessmentsList',
        component: () => import('@/views/students/lod/assessment/StudentLodAssessmentsList.vue'),
        meta: {
          title: i18n.t('student.menu.evaluations')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'lod/assessment',
        name: 'StudentLodAssessment',
        component: () => import('@/views/students/lod/assessment/StudentLodAssessmentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.evaluations')
        },
        props: (route) => ({
          personId: Number(route.params.id),
          basicClassId: Number(route.query.basicClassId),
          schoolYear: Number(route.query.schoolYear),
          isSelfEduForm: Helpers.boolTryParse(route.query.isSelfEduForm),
          assessmentDetails: route.params.assessmentDetails
        }),
      },
      {
        path: 'lod/assessment/create',
        component: () => import('@/views/students/lod/assessment/StudentLodAssessmentCreate.vue'),
        meta: {
          title: i18n.t('lod.evaluations.evaluationsCreateTitle')
        },
        props: (route) => ({ personId: Number(route.params.id) })
      },
      {
        path: 'lod/assessment/edit',
        component: () => import('@/views/students/lod/assessment/StudentLodAssessmentEdit.vue'),
        meta: {
          title: i18n.t('lod.evaluations.evaluationsCreateTitle')
        },
        props: (route) => ({
          personId: Number(route.params.id),
          schoolYear: Number(route.query.schoolYear),
          basicClassId: Number(route.query.basicClassId),
          isSelfEduForm: Helpers.boolTryParse(route.query.isSelfEduForm),
        })
      },
      {
        path: 'ores',
        name: 'StudentOresList',
        component: () => import('@/views/ores/OresList.vue'),
        meta: {
          title: i18n.t('student.menu.ores')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'ores/create',
        name: 'StudentOresCreate',
        component: () => import('@/views/ores/OresCreate.vue'),
        meta: {
          title: i18n.t('ores.createTitle')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'ores/:oresId/edit',
        name: 'StudentOresEdit',
        component: () => import('@/views/ores/OresEdit.vue'),
        meta: {
          title: i18n.t('ores.editTitle')
        },
        props: (route) => ({ oresId: Number(route.params.oresId) })
      },
      {
        path: 'ores/:oresId/details',
        name: 'StudentOresDetails',
        component: () => import('@/views/ores/OresDetails.vue'),
        meta: {
          title: i18n.t('ores.reviewTitle')
        },
        props: (route) => ({ oresId: Number(route.params.oresId) })
      },
      {
        path: 'commonPersonalDevelopment/list',
        name: 'StudentCommonPersonalDevelopmentList',
        component: () => import('@/views/students/personalDevelopment/StudentCommonPersonalDevelopmentList.vue'),
        meta: {
          title: i18n.t('student.menu.commonPersonalDevelopment')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'commonPersonalDevelopment/create',
        name: 'StudentCommonPersonalDevelopmentCreate',
        component: () => import('@/views/students/personalDevelopment/StudentCommonPersonalDevelopmentCreate.vue'),
        meta: {
          title: i18n.t('commonPersonalDevelopment.createTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), id: Number(route.params.cpdId) }),
      },
      {
        path: 'commonPersonalDevelopment/:cpdId/details',
        name: 'StudentCommonPersonalDevelopmentDetails',
        component: () => import('@/views/students/personalDevelopment/StudentCommonPersonalDevelopmentDetails.vue'),
        meta: {
          title: i18n.t('commonPersonalDevelopment.reviewTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), id: Number(route.params.cpdId) }),
      },
      {
        path: 'commonPersonalDevelopment/:cpdId/edit',
        name: 'StudentCommonPersonalDevelopmentEdit',
        component: () => import('@/views/students/personalDevelopment/StudentCommonPersonalDevelopmentEdit.vue'),
        meta: {
          title: i18n.t('commonPersonalDevelopment.editTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), id: Number(route.params.cpdId) }),
      },
      {
        path: 'additionalPersonalDevelopment/list',
        name: 'StudentAdditionalPersonalDevelopmentList',
        component: () => import('@/views/students/personalDevelopment/StudentAdditionalPersonalDevelopmentList.vue'),
        meta: {
          title: i18n.t('student.menu.additionalPersonalDevelopment')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'additionalPersonalDevelopment/create',
        name: 'StudentAdditionalPersonalDevelopmentCreate',
        component: () => import('@/views/students/personalDevelopment/StudentAdditionalPersonalDevelopmentCreate.vue'),
        meta: {
          title: i18n.t('additionalPersonalDevelopment.createTitle')
        },
        props: (route) => ({ personId: Number(route.params.id) }),
      },
      {
        path: 'additionalPersonalDevelopment/:apdId/details',
        name: 'StudentAdditionalPersonalDevelopmentDetails',
        component: () => import('@/views/students/personalDevelopment/StudentAdditionalPersonalDevelopmentDetails.vue'),
        meta: {
          title: i18n.t('additionalPersonalDevelopment.reviewTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), id: Number(route.params.apdId) }),
      },
      {
        path: 'additionalPersonalDevelopment/:apdId/edit',
        name: 'StudentAdditionalPersonalDevelopmentEdit',
        component: () => import('@/views/students/personalDevelopment/StudentAdditionalPersonalDevelopmentEdit.vue'),
        meta: {
          title: i18n.t('additionalPersonalDevelopment.editTitle')
        },
        props: (route) => ({ personId: Number(route.params.id), id: Number(route.params.apdId) }),
      },
      {
        path: 'reassessments',
        component: () => import('@/views/students/StudentReassessmentsList.vue'),
        meta: {
          title: i18n.t('student.menu.reassessment')
        },
        props: (route) => ({ pid: Number(route.params.id) }),
      },
      {
        path: 'reassessments/create',
        component: () => import('@/views/students/StudentReassessmentCreate.vue'),
        meta: {
          title: i18n.t('student.menu.reassessment')
        },
        props: (route) => ({ pid: Number(route.params.id) })
      },
      {
        path: 'reassessments/:docId/details',
        component: () => import('@/views/students/StudentReassessmentDetails.vue'),
        meta: {
          title: i18n.t('student.menu.reassessment')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
      {
        path: 'reassessments/:docId/edit',
        component: () => import('@/views/students/StudentReassessmentEdit.vue'),
        meta: {
          title: i18n.t('student.menu.reassessment')
        },
        props: (route) => ({ pid: Number(route.params.id), docId: Number(route.params.docId) })
      },
    ]
  }
];
