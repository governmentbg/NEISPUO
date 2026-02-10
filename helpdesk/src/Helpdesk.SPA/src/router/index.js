import Vue from 'vue';
import VueRouter from 'vue-router';

import i18n from '@/language/language';

import { config } from '@/common/config';
import AuthService from "@/services/auth.service";

import { UserRole } from '@/enums/enums';

import authRoutes from './auth.routes';

Vue.use(VueRouter);

const scrollBehavior = (to, from, savedPosition) => {
  const scrollpos = savedPosition || to.meta?.scrollPos || { left: 0, top: 0 };
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(scrollpos);
    }, 500);
  });
};

const routes = [
  ...authRoutes,
  {
    path: '/',
    meta: {
      title: i18n.t('common.home'),
      requiresAuth: true,
      scrollPos: {
        top: 0,
        left: 0,
      },
    },
    name: 'Home',
    component: () => import('@/views/Home.vue')
  },
  {
    path: '/issue/create',
    meta: {
      title: i18n.t('issue.createTitle'),
      requiresAuth: true
    },
    name: 'CreateIssue',
    component: () => import('@/views/issue/IssueCreate.vue'),
  },
  {
    path: '/issue/:id',
    meta: {
      title: i18n.t('issue.detailsTitle'),
      requiresAuth: true
    },
    name: 'ViewIssueDetails',
    component: () => import('@/views/issue/IssueDetails.vue'),
    props: (route) => ({ issueId: Number(route.params.id) }),
  },
  {
    path: '/stats/category',
    meta: {
      title: i18n.t('stats.category.home'),
      requiresAuth: true
    },
    name: 'ViewCategoryStatsList',
    component: () => import('@/views/stats/CategoryList.vue')
  },
  {
    path: '/questions',
    meta: {
      title: i18n.t('question.home'),
      requiresAuth: true
    },
    name: 'ViewQuestionsList',
    component: () => import('@/views/question/QuestionsList.vue')
  },
  {
    path: '/question/create',
    meta: {
      title: i18n.t('question.createTitle'),
      requiresAuth: true
    },
    name: 'CreateQuestion',
    component: () => import('@/views/question/QuestionCreate.vue'),
  },
  {
    path: '/question/:id',
    meta: {
      title: i18n.t('question.detailsTitle'),
      requiresAuth: true
    },
    name: 'ViewQuestionDetails',
    component: () => import('@/views/question/QuestionDetails.vue'),
    props: (route) => ({ questionId: Number(route.params.id) }),
  },
  {
    path: '/errors/AccessDenied',
    meta: {
      title: i18n.t('errors.accessDenied'),
      requiresAuth: true
    },
    name: 'AccessDenied',
    component: () => import('@/views/errors/AccessDenied.vue')
  },
  {
    // catch all 404 - define at the very end
    path: "*",
    component: () => import('@/views/errors/AccessDenied.vue')
   }
];

const router = new VueRouter({
  mode: 'history',
  base: config.spaBaseUrlRelative,
  routes,
  scrollBehavior
});

let authService = new AuthService();

router.beforeEach(async (to, from, next) => {
  const user = await authService.getUser();
  if (to.name != 'AccessDenied' && user && !user.expired)
  {
    const selectedRole = user.profile.selected_role.SysRoleID;
    if (![UserRole.School, UserRole.Mon, UserRole.Ruo, UserRole.MonExpert, UserRole.RuoExpert, UserRole.Teacher, UserRole.Cioo, UserRole.Consortium, UserRole.Municipality].includes(selectedRole))
    {
      next({
        path: '/errors/AccessDenied',
      });
    }
  }
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  if (requiresAuth) {
      if (user && !user.expired){
        if (to.meta.role){
          authService.getRole().then(
            sucess => {
              if (to.meta.role == sucess){
                next();
              }else {
                next({
                  path: '/account/login',
                  query: { redirect: to.fullPath }
                });
              }
            },
            err => {
              console.log(err);
            }
          );
        }
        else{
          next();
        }
      }
      else{
        next({
          path: '/account/login',
          query: { redirect: to.fullPath }
        });
      }
  } else {
    next();
  }
});


export default router;
