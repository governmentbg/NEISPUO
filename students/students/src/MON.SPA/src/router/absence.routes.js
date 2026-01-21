import Helpers from '@/components/helper';
import { Permissions } from '@/enums/enums';
import i18n from '@/language/language';

export default [
  {
    path: '/absence/campaigns',
    meta: {
      title: i18n.t('menu.absencesCampaigns')
    },
    component: () => import('@/views/absence/AbsenceCampaigns.vue')
  },
  {
    path: '/absence/campaign/create',
    meta: {
      title: i18n.t('student.menu.studentSanctions')
    },
    component: () => import('@/views/absence/AbsenceCampaignCreate.vue')
  },
  {
    path: '/absence/campaign/:id/details',
    meta: {
      title: i18n.t('menu.absencesCampaigns')
    },
    component: () => import('@/views/absence/AbsenceCampaignDetails.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    path: '/absence/campaign/:id/edit',
    meta: {
      title: i18n.t('menu.absencesCampaigns')
    },
    component: () => import('@/views/absence/AbsenceCampaignEdit.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    path: '/absence/import',
    meta: {
      title: i18n.t('menu.absencesImport')
    },
    name: 'AbsenceImport',
    component: () => import('@/views/absence/AbsencesImport.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentAbsenceManage });
    },
  },
  {
    path: '/absence/reports',
    meta: {
      title: i18n.t('menu.absencesReports')
    },
    name: 'AbsenceReports',
    component: () => import('@/views/absence/AbsenceReports.vue')
  },
  {
    path: '/absence/absencesExport',
    meta: {
      title: i18n.t('menu.absencesExport')
    },
    name: 'AbsencesExport',
    component: () => import('@/views/absence/AbsencesExport.vue')
  },
  {
    path: '/absence/import/:id/details',
    meta: {
      title: i18n.t('menu.importedFileDetails')
    },
    name: 'ImportedFileDetails',
    component: () => import('@/views/absence/AbsenceImportDetails.vue'),
    props: (route) => ({ absenceImportId: Number(route.params.id) }),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentAbsenceRead });
    },
  },
];
