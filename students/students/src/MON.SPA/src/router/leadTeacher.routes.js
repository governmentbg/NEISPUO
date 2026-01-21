import i18n from '@/language/language';

export default [
  {
    path: '/leadTeacher/list',
    meta: {
      title: i18n.t('leadTeacher.title')
    },
    name: 'LeadTeacherList',
    component: () => import('@/views/leadTeacher/LeadTeacherList.vue'),
  },
  {
    path: '/leadTeacher/:classId/edit',
    meta: {
      title: i18n.t('leadTeacher.editTitle')
    },
    name: 'LeadTeacherEdit',
    component: () => import('@/views/leadTeacher/LeadTeacherEdit.vue'),
    props: (route) => ({ classId: Number(route.params.classId) }),
  },
  {
    path: '/leadTeacher/:classId/details',
    meta: {
      title: i18n.t('leadTeacher.detailsTitle')
    },
    name: 'LeadTeacherDetails',
    component: () => import('@/views/leadTeacher/LeadTeacherDetails.vue'),
    props: (route) => ({ classId: Number(route.params.classId) }),
  }
];
