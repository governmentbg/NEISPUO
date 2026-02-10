import Vue from 'vue';
import VueRouter from 'vue-router';
import { config } from '@/common/config';
import i18n from '@/plugins/language';


Vue.use(VueRouter);

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import("@/views/Home.vue"),
    meta: {
      title: i18n.t('appTitle')
    },
  },
  {
    path: "/details/:uid?",
    name: "Details",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ "@/views/Details.vue"),
    meta: {
      title: i18n.t('details.title')
    },
    props: (route) => ({ ...route.params})
  }
];

const router = new VueRouter({
  mode: 'history',
  base: config.spaBaseUrlRelative,
  routes
});

export default router;
