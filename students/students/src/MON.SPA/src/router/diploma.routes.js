import Helpers from '@/components/helper';
import { Permissions } from '@/enums/enums';
import i18n from '@/language/language';

export default [
  {
    path: '/diplomaTemplate/create',
    meta: {
      title: i18n.t('menu.diplomas.createTemplate')
    },
    name: 'DiplomaTemplateCreate',
    component: () => import('@/views/diplomaTemplate/Create.vue'),
    props: (route) => ({
      basicDocumentId: route.query.basicDocumentId ? Number(route.query.basicDocumentId) : null,
      templateId: route.query.templateId ? Number(route.query.templateId) : null,
      basicClassId: route.query.basicClassId ? Number(route.query.basicClassId) : null
    }),
  },
  {
    path: '/diplomaTemplate/list',
    meta: {
      title: i18n.t('basicDocument.templates.list')
    },
    name: 'TemplatesList',
    component: () => import('@/views/diplomaTemplate/DiplomaTemplatesList.vue')
  },
  {
    path: '/diplomaTemplate/:id/edit',
    meta: {
      title: i18n.t('menu.diplomas.templateEdit')
    },
    name: 'DiplomaTemplateEdit',
    component: () => import('@/views/diplomaTemplate/Edit.vue'),
    props: (route) => ({ id: Number(route.params.id) }),
  },
  {
    path: '/basicDocument/:typeId/barcodes',
    props: (route) => ({ basicDocumentId: Number(route.params.typeId) }),
    meta: {
      title: i18n.t('menu.diplomas.barcodesList')
    },
    name: 'DiplomaBarcodeYearList',
    component: () => import('@/views/diploma/barcodes/BarcodeYearList.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDiplomaBarcodesShow });
    }
  },
  {
    path: '/basicDocument/:typeId/barcodes/add',
    name: 'BarcodeAdd',
    component: () => import('@/views/diploma/barcodes/BarcodeYearAdd.vue'),
    meta: {
      title: i18n.t('menu.diplomas.barcodesAdd')
    },
    props: (route) => ({ basicDocumentId: Number(route.params.typeId) }),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDipomasBarcodesAdd });
    },
  },
  {
    path: '/basicDocument/:typeId/barcodes/edit/:barcodeYearId',
    name: 'BarcodeEdit',
    component: () => import('@/views/diploma/barcodes/BarcodeYearEdit.vue'),
    meta: {
      title: i18n.t('menu.diplomas.barcodesEdit')
    },
    props: (route) => ({ basicDocumentId: Number(route.params.typeId), barcodeYearId: Number(route.params.barcodeYearId)  }),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDiplomasBarcodesEdit });
    },
  },
  {
    path: '/basicDocuments',
    meta: {
      title: i18n.t('basicDocument.title')
    },
    name: 'BasicDocumentsList',
    component: () => import('@/views/diploma/BasicDocuments.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForBasicDocumentsListShow });
    }
  },
  {
    path: '/basicDocuments/:id/edit',
    meta: {
      title: i18n.t('basicDocument.title')
    },
    name: 'BasicDocumentsEdit',
    component: () => import('@/views/diploma/BasicDocumentEdit.vue'),
    props: (route) => ({ id: Number(route.params.id) }),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForBasicDocumentEdit });
    }
  },
  {
    path: '/basicDocumentSequence/list',
    meta: {
      title: i18n.t('basicDocumentSequence.title')
    },
    name: 'BasicDocumentSequences',
    component: () => import('@/views/diploma/BasicDocumentSequences.vue')
  },
  {
    path: '/diploma/list',
    meta: {
      title: i18n.t('menu.diplomas.title')
    },
    name: 'DiplomaList',
    component: () => import('@/views/diploma/DiplomaListView.vue'),
    props: (route) => ({ selectedTab: route.query.tab })
  },
  {
    path: '/regBook/qualificationList',
    meta: {
      title: i18n.t('menu.regBooks.title')
    },
    name: 'RegBookQualificationList',
    component: () => import('@/views/diploma/RegBookQualificationListView.vue'),
    props: (route) => ({ selectedTab: route.query.tab })
  },
  {
    path: '/regBook/certificateList',
    meta: {
      title: i18n.t('menu.regBooks.title')
    },
    name: 'RegBookCertificateList',
    component: () => import('@/views/diploma/RegBookCertificateListView.vue'),
    props: (route) => ({ selectedTab: route.query.tab })
  },
  {
    path: '/regBook/qualificationDuplicatesList',
    meta: {
      title: i18n.t('menu.regBooks.title')
    },
    name: 'RegBookQualificationDuplicatesList',
    component: () => import('@/views/diploma/RegBookQualificationDuplicatesListView.vue'),
    props: (route) => ({ selectedTab: route.query.tab })
  },
  {
    path: '/regBook/certificationDuplicatesList',
    meta: {
      title: i18n.t('menu.regBooks.title')
    },
    name: 'RegBookCertificationDuplicatesList',
    component: () => import('@/views/diploma/RegBookCertificationDuplicatesListView.vue'),
    props: (route) => ({ selectedTab: route.query.tab })
  },
  {
    path: '/diploma/create',
    name: 'DiplomaCreate',
    component: () => import('@/views/diploma/StudentDiplomaCreateView.vue'),
    meta: {
      title: i18n.t('menu.diplomas.title')
    },
    props: (route) => ({
      personId: route.query.personId ? Number(route.query.personId) : null,
      templateId: route.query.templateId ? Number(route.query.templateId) : null,
      basicDocumentId: route.query.basicDocumentId ? Number(route.query.basicDocumentId) : null,
      basicClassId: route.query.basicClassId ? Number(route.query.basicClassId) : null
    })
  },
  {
    path: '/diploma/:diplomaId/edit',
    name: 'DiplomaEdit',
    component: () => import('@/views/diploma/StudentDiplomaEditView.vue'),
    meta: {
      title: i18n.t('menu.diplomas.title')
    },
    props: (route) => ({ diplomaId: Number(route.params.diplomaId) })
  },
  {
    path: '/diploma/:diplomaId/review',
    name: 'DiplomaReview',
    component: () => import('@/views/diploma/StudentDiplomaDetailsView.vue'),
    meta: {
      title: i18n.t('menu.diplomas.title')
    },
    props: (route) => ({ diplomaId: Number(route.params.diplomaId) })
  },
  {
    path: '/diploma/:id/images',
    name: 'DiplomaImages',
    component: () => import('@/views/diploma/DiplomaImagesView.vue'),
    meta: {
      title: i18n.t('menu.diplomas.title')
    },
    props: (route) => ({ id: Number(route.params.id), isValidationDocument: false, details: route.params.details })
  },
  {
    path: '/validation/:id/images',
    name: 'ValidationImages',
    component: () => import('@/views/diploma/DiplomaImagesView.vue'),
    meta: {
      title: i18n.t('menu.diplomas.title')
    },
    props: (route) => ({ id: Number(route.params.id), isValidationDocument: true, details: route.params.details })
  },
  {
    path: '/diploma/createRequests',
    name: 'DiplomaCreateRequestsList',
    component: () => import('@/views/diploma/DiplomaCreateRequestListView.vue'),
    meta: {
      title: i18n.t('diplomas.createRequest.title')
    },
  },
  {
    path: '/diploma/createRequest/create',
    name: 'DiplomaCreateRequestsListCreate',
    component: () => import('@/views/diploma/DiplomaCreateRequestCreateView.vue'),
    meta: {
      title: i18n.t('diplomas.createRequest.title')
    },
    props: (route) => ({ personId: Number(route.query.personId), requestingInstitutionId: Number(route.query.requestingInstitutionId), currentInstitutionId: Number(route.query.currentInstitutionId) }),
  },
  {
    path: '/diploma/createRequest/:id/edit',
    name: 'DiplomaCreateRequestsListEdit',
    component: () => import('@/views/diploma/DiplomaCreateRequestEditView.vue'),
    meta: {
      title: i18n.t('diplomas.createRequest.title')
    },
    props: (route) => ({ id: Number(route.params.id) }),
  },
  {
    path: '/printTemplate/list',
    name: 'PrintTemplatesList',
    meta: {
      title: i18n.t('institution.printTemplates')
    },
    component: () => import('@/views/printTemplate/PrintTemplateList.vue'),
  },
  {
    path: '/printTemplate/add',
    name: 'AddPrintTemplate',
    component: () => import('@/views/printTemplate/PrintTemplateAdd.vue'),
    meta: {
      title: i18n.t('institution.printTemplateAdd')
    },
  },
  {
    path: '/printTemplate/:id/edit',
    name: 'EditPrintTemplate',
    component: () => import('@/views/printTemplate/PrintTemplateEdit.vue'),
    meta: {
      title: i18n.t('institution.printTemplateEdit')
    },
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    path: '/printTemplate/:id/reportDesigner',
    meta: {
      title: i18n.t('menu.reportDesigner')
    },
    name: 'ReportDesigner',
    component: () => import('@/views/print/ReportDesigner.vue'),
    props: (route) => ({ id: Number(route.params.id) })

  },
];
