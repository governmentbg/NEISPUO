import Helpers from '@/components/helper';
import { Permissions } from '@/enums/enums';
import i18n from '@/language/language';

export default [
  {
    path: '/administration/auditLogs',
    meta: {
      title: i18n.t('auditLogs.title')
    },
    component: () => import('@/views/administration/AuditLogs.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next,{ permission: Permissions.PermissionNameForAuditLogsShow });
    },
  },
  {
    path: '/administration/dashboard',
    meta: {
      title: i18n.t('dashboard.title')
    },
    component: () => import('@/views/administration/Dashboard.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentCreate });
    },
  },
  {
    path: '/administration/directorDashboard',
    meta: {
        title: i18n.t('dashboards.directorDashboardTitle')
    },
    component: () => import('@/views/administration/dashboards/DirectorDashboard.vue'),
    beforeEnter: (to, from, next) => {
        Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentCreate });
      },
  },
  {
    path: '/administration/contextualInformation',
    meta: {
      title: i18n.t('contextualInformation.title')
    },
    component: () => import('@/views/administration/ContextualInformation.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next,{ permission: Permissions.PermissionNameForContextualInformationManage });
    },
  },
  {
    path: '/administration/permissionsDocumentation',
    meta: {
      title: i18n.t('permissionsDocumentation.title')
    },
    component: () => import('@/views/administration/PermissionsDocumentation.vue'),
  },
  {
    path: '/administration/schoolTypeLodAccess',
    meta: {
      title: i18n.t('menu.schoolTypeLodAccess')
    },
    component: () => import('@/views/schoolTypeLodAccess/SchoolTypeLodAccess.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForLodAccessConfigurationShow });
    },
  },
  {
    path: '/administration/schoolTypeLodAccess/:id/edit',
    meta: {
      title: i18n.t('menu.schoolTypeLodAccess')
    },
    component: () => import('@/views/schoolTypeLodAccess/SchoolTypeLodAccessEdit.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForLodAccessConfigurationEdit });
    },
    props: (route) => ({ id: Number(route.params.id), type: route.query.type, returnUrl: route.query.returnUrl})
  },
  {
    path: '/administration/nomenclatures',
    meta: {
      title: i18n.t('menu.nomenclature')
    },
    component: () => import('@/views/administration/Nomenclatures.vue'),
    props: (route) => ({ type: route.query.type }),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next,{ permission: Permissions.PermissionNameForNomenclatureManage });
    },
  },
  {
    path: '/administration/entity/:id/details',
    meta: {
      title: i18n.t('menu.nomenclature')
    },
    component: () => import('@/views/dynamic/DynamicEntityDetails.vue'),
    props: (route) => ({ id: Number(route.params.id), type: route.query.type,  returnUrl: route.query.returnUrl})
  },
  {
    path: '/administration/entity/:id/edit',
    meta: {
      title: i18n.t('menu.nomenclature')
    },
    component: () => import('@/views/dynamic/DynamicEntityEdit.vue'),
    props: (route) => ({ id: Number(route.params.id), type: route.query.type, returnUrl: route.query.returnUrl})
  },
  {
    path: '/administration/entity/create',
    meta: {
      title: i18n.t('menu.nomenclature')
    },
    component: () => import('@/views/dynamic/DynamicEntityCreate.vue'),
    props: (route) => ({ type: route.query.type, returnUrl: route.query.returnUrl})
  },
  {
    name: 'CertificatesList',
    path: '/administration/certificates',
    meta: {
      title: i18n.t('menu.certifications')
    },
    component: () => import('@/views/administration/certificates/CertificatesListView.vue'),
  },
  {
    path: '/administration/certificates/create',
    meta: {
      title: i18n.t('menu.certifications')
    },
    component: () => import('@/views/administration/certificates/CertificateCreateView.vue'),
  },
  {
    path: '/administration/certificates/:id/edit',
    meta: {
      title: i18n.t('menu.certifications')
    },
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForCertificatesManage });
    },
    component: () => import('@/views/administration/certificates/CertificateEditView.vue'),
    props: (route) => ({ id: Number(route.params.id), type: route.query.type, returnUrl: route.query.returnUrl})
  },
  {
    path: '/administration/diplomaImportValidationExclusions',
    meta: {
      title: i18n.t('menu.diplomaImportValidationExclusions')
    },
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentDiplomaImportValidationExclusionsManage });
    },
    component: () => import('@/views/administration/DiplomaImportValidationExclusions.vue'),
  },
  {
    path: '/administration/cacheService',
    meta: {
      title: i18n.t('menu.cacheService')
    },
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForCacheServiceManage });
    },
    component: () => import('@/views/administration/CacheService.vue'),
  },
  {
    path: '/administration/lodFinalization',
    meta: {
      title: i18n.t('menu.cacheService')
    },
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForCacheServiceManage });
    },
    component: () => import('@/views/administration/LodFinalizationAdministration.vue'),
  },
];
