//import Helpers from '@/components/helper';
//import { Permissions } from '@/enums/enums';

import i18n from '@/language/language';

export default [
  {
    path: '/refugee/application/create',
    meta: {
      title: i18n.t('refugee.createApplication')
    },
    name: 'RefugeeApplicationCreate',
    component: () => import('@/views/refugee/RefugeeApplicationCreate.vue'),
  },
  {
    path: '/refugee/applications',
    meta: {
      title: i18n.t('refugee.title')
    },
    name: 'RefugeeApplicationList',
    component: () => import('@/views/refugee/RefugeeApplicationList.vue'),
  },
  {
    path: '/refugee/application/:id/edit',
    meta: {
      title: i18n.t('refugee.editTitle')
    },
    name: 'RefugeeApplicationEdit',
    component: () => import('@/views/refugee/RefugeeApplicationEdit.vue'),
    props: (route) => ({ appId: Number(route.params.id) }),
  },
  {
    path: '/refugee/application/:id/details',
    meta: {
      title: i18n.t('refugee.reviewTitle')
    },
    name: 'RefugeeApplicationDetails',
    component: () => import('@/views/refugee/RefugeeApplicationDetails.vue'),
    props: (route) => ({ appId: Number(route.params.id) }),
  }];
