import Helpers from '@/components/helper';
import { Permissions } from '@/enums/enums';
import i18n from '@/language/language';

export default [
  {
    path: '/institutions',
    meta: {
      title: i18n.t('menu.instututionList')
    },
    name: 'InstitutionList',
    component: () => import('@/views/institution/InstitutionListView.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForInstitutionRead });
    },
  },
  {
    path: '/institution/:id',
    name: 'InstitutionDetailsLayout',
    component: () => import('@/views/InstitutionDetailsLayout.vue'),
    meta: {},
    props: (route) => ({ id: Number(route.params.id) }),
    children: [
      {
        path: 'details',
        name: 'InstitutionDetails',
        component: () => import('@/views/institution/InstitutionDetailsView.vue'),
        meta: {
          title: i18n.t('menu.classes')
        },
        props: (route) => ({ institutionId: Number(route.params.id) })
      },
      {
        path: 'external',
        meta: {
          title: i18n.t('institution.externalSubtitle')
        },
        name: 'InstitutionExternalDetails',
        component: () => import('@/views/institution/ExternalDetails.vue'),
        props: (route) => ({ institutionId: Number(route.params.id) })
      },
      {
        path: 'students',
        meta: {
          title: i18n.t('institution.students')
        },
        name: 'InstitutionStudents',
        component: () => import('@/views/institution/InstitutionStudentsView.vue'),
        props: (route) => ({ institutionId: Number(route.params.id) }),
      },

    ]
  }

];
