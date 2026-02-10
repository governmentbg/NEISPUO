import i18n from '@/language/language';

export default [
  {
    name: 'DocManagementCampaigns',
    path: '/docManagement/campaigns',
    meta: {
      title: i18n.tc('docManagement.campaign.title', 2)
    },
    component: () => import('@/views/docManagement/DocManagementCampaigns.vue')
  },
  {
    name: 'DocManagementCampaignCreate',
    path: '/docManagement/campaign/create',
    meta: {
      title: i18n.t('docManagement.campaign.addTitle')
    },
    component: () => import('@/views/docManagement/DocManagementCampaignCreate.vue')
  },
  {
    name: 'DocManagementCampaignDetails',
    path: '/docManagement/campaign/:id/details',
    meta: {
      title: i18n.t('docManagement.campaign.reviewTitle')
    },
    component: () => import('@/views/docManagement/DocManagementCampaignDetails.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    name: 'DocManagementCampaignEdit',
    path: '/docManagement/campaign/:id/edit',
    meta: {
     title: i18n.t('docManagement.campaign.editTitle')
    },
    component: () => import('@/views/docManagement/DocManagementCampaignEdit.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    name: 'DocManagementApplications',
    path: '/docManagement/applications',
    meta: {
      title: i18n.tc('docManagement.application.title', 2)
    },
    component: () => import('@/views/docManagement/DocManagementApplications.vue')
  },
  {
    name: 'DocManagementApplicationCreate',
    path: '/docManagement/application/create',
    meta: {
      title: i18n.t('docManagement.application.addTitle')
    },
    component: () => import('@/views/docManagement/DocManagementApplicationCreate.vue'),
     props: (route) => ({ campaignId: Number(route.query.campaignId) })
  },
  {
    name: 'DocManagementApplicationDetails',
    path: '/docManagement/application/:id/details',
    meta: {
      title: i18n.t('docManagement.application.reviewTitle')
    },
    component: () => import('@/views/docManagement/DocManagementApplicationDetails.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    name: 'DocManagementApplicationEdit',
    path: '/docManagement/application/:id/edit',
    meta: {
     title: i18n.t('docManagement.application.editTitle')
    },
    component: () => import('@/views/docManagement/DocManagementApplicationEdit.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    name: 'DocManagementApplicationDelivery',
    path: '/docManagement/application/:id/delivery',
    meta: {
     title: i18n.t('docManagement.application.deliveryReportTitle')
    },
    component: () => import('@/views/docManagement/DocManagementApplicationDelivery.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    name: 'DocManagementUnusedDocs',
    path: '/docManagement/unusedDocs',
    meta: {
      title: i18n.tc('docManagement.unusedDocs.title', 2)
    },
    component: () => import('@/views/docManagement/DocManagementUnusedDocs.vue')
  },
  {
    name: 'DocManagementReport',
    path: '/docManagement/report',
    meta: {
      title: i18n.tc('docManagement.report.title', 2)
    },
    component: () => import('@/views/docManagement/DocManagementReportList.vue')
  },
  {
    path: '/docManagement/dashboard',
    meta: {
      title: i18n.t('menu.absencesReports')
    },
    name: 'DocManagementDashboard',
    component: () => import('@/views/docManagement/DocManagementDashboard.vue')
  },
];
