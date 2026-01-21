export default [
  {
    path: '/account/profile',
    meta: {
      title: 'Профил'
    },
    name: 'Profile',
    component: () => import('@/views/account/Profile.vue')
  },
  {
    path: '/account/login',
    meta: {
      title: 'Вход',
    },
    name: 'Login',
    props: (route) => ({ redirect: route.query.redirect }),
    component: () => import('@/views/account/Login.vue')
  },
];
