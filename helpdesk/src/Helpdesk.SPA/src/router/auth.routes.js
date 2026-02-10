import i18n from '@/language/language';

export default [
  {
    path: '/account/profile',
    meta: {
      title: i18n.t('menu.profile'),
      requiresAuth: true
    },
    name: 'Profile',
    component: () => import('@/views/account/Profile.vue')
  },
  {
    path: '/account/login',
    meta: {
      title: i18n.t('buttons.login'),
    },
    name: 'Login',
    props: (route) => ({ redirect: route.query.redirect }),
    component: () => import('@/views/account/Login.vue')
  },
];
