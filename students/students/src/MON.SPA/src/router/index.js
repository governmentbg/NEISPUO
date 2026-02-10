import { Permissions, UserRole } from '@/enums/enums';

import AuthService from "@/services/auth.service";
import Constants from '@/common/constants';
import Helpers from '@/components/helper';
import Vue from 'vue';
import VueRouter from 'vue-router';
import absenceRoutes from './absence.routes';
import administrationRoutes from "./administration.routes";
import authRoutes from './auth.routes';
import classGroupRoutes from './classGroup.routes';
import { config } from '@/common/config';
import diplomaRoutes from './diploma.routes';
import healthInsuranceRoutes from './healthInsurance.routes';
import i18n from '@/language/language';
import institutionRoutes from './institution.routes';
import refugeeRoutes from './refugee.routes';
import leadTeacherRoutes from './leadTeacher.routes';
import studentRoutes from './student.routes';
import financeRoutes from './finance.routes';
import docManagementsRouter from './docManagements.router';

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
  ...administrationRoutes,
  ...authRoutes,
  ...studentRoutes,
  ...diplomaRoutes,
  ...institutionRoutes,
  ...classGroupRoutes,
  ...absenceRoutes,
  ...refugeeRoutes,
  ...healthInsuranceRoutes,
  ...leadTeacherRoutes,
  ...financeRoutes,
  ...docManagementsRouter,
  {
    path: '/',
    meta: {
        title: i18n.t('dashboards.dashboardsTitle'),
        scrollPos: {
          top: 0,
          left: 0,
        },
    },
    name: 'Home',
    component: () => import('@/views/administration/dashboards/Dashboards.vue'),
    beforeEnter: (to, from, next) => {
        Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForStudentCreate });
      },
  },
  {
    path: '/settings/nomenclatures/:name',
    meta: {
      title: 'Редакция номенклатура'
    },
    component: () => import('@/views/nomenclatures/TablesEdit.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForNomenclatureRead });
    },
  },
  {
    path: '/demonstration/theme',
    meta: {
      title: 'Theme'
    },
    name: 'Theme',
    component: () => import('@/views/demonstration/Theme.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDemonstrationSectionShow });
    },
  },
  {
    path: '/demonstration/helpers',
    meta: {
      title: i18n.t('menu.helpersDocumentation')
    },
    component: () => import('@/views/demonstration/HelpersDocumentation.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDemonstrationSectionShow });
    },
  },
  {
    path: '/demonstration/print/:reportName/:id',
    props: true,
    meta: {
      title: i18n.t('menu.print')
    },
    name: 'PrintDemonstration',
    component: () => import('@/views/print/Print.vue'),
    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDemonstrationSectionShow });
    },
  },
  {
    path: '/demonstration/reportDesigner/:id',
    props: (route) => ({ id: Number(route.params.id) }),
    meta: {
      title: i18n.t('menu.reportDesigner')
    },
    name: 'ReportDesignerDemonstration',
    component: () => import('@/views/print/ReportDesigner.vue'),

    beforeEnter: (to, from, next) => {
      Helpers.authorize(to, from, next, { permission: Permissions.PermissionNameForDemonstrationSectionShow });
    },
  },
  {
    path: '/software/localServer',
    meta: {
      title: i18n.t('menu.localServer')
    },
    name: 'LocalServer',
    component: () => import('@/views/software/LocalServer.vue')
  },
  {
    path: '/software/biss',
    meta: {
      title: i18n.t('menu.biss')
    },
    name: 'Biss',
    component: () => import('@/views/software/BISS.vue')
  },
  {
    path: '/errors/ServiceUnavailable',
    meta: {
      title: i18n.t('errors.serviceUnavailable')
    },
    name: 'ServiceUnavailable',
    component: () => import('@/views/errors/ServiceUnavailable.vue')
  },
  {
    path: '/errors/AccessDenied',
    meta: {
      title: i18n.t('errors.accessDenied')
    },
    name: 'AccessDenied',
    component: () => import('@/views/errors/AccessDenied.vue')
  },
  {
    path: '/errors/AuthenticationError',
    meta: {
      title: i18n.t('errors.authError')
    },
    name: 'AuthenticationError',
    component: () => import('@/views/errors/AuthenticationError.vue')
  },
  { path: '/docs'  }, // pass directly to documentation
  {
    path: '/test/fileManager',
    meta: {
      title: 'File manager'
    },
    component: () => import('@/views/test/FileManager.vue')
  },
  {
    path: '/test/regix',
    meta: {
      title: 'Regix connection'
    },
    component: () => import('@/views/test/RegixConnection.vue')
  },
  {

    path: '/notifications/messages',
    meta: {
      title: i18n.t('messages.title')
    },
    component: () => import ('@/views/notification/MessagesLayout')
  },
  {
    name: 'MessageDetails',
    path: '/message/:id/details',
    meta: {
      title: i18n.t('messages.title')
    },
    component: () => import ('@/views/notification/MessageDetails'),
    props: (route) => ({ id: Number(route.params.id)}),
  },
  {
    path: '/ores',
    component: () => import('@/views/ores/OresList.vue'),
    meta: {
      title: i18n.t('student.menu.ores')
    },
    props: (route) => ({ personId: route.query.personId ? Number(route.query.personId) : null,
      institutionId: route.query.institutionId ? Number(route.query.institutionId) : null,
      classId: route.query.classId ? Number(route.query.classId) : null }),
  },
  {
    path: '/ores/create',
    name: 'OresCreate',
    component: () => import('@/views/ores/OresCreate.vue'),
    meta: {
      title: i18n.t('ores.createTitle')
    },
    props: (route) => ({ personId: route.query.personId ? Number(route.query.personId) : null,
      institutionId: route.query.institutionId ? Number(route.query.institutionId) : null,
      classId: route.query.classId ? Number(route.query.classId) : null }),
  },
  {
    path: '/ores/:oresId/edit',
    name: 'OresEdit',
    component: () => import('@/views/ores/OresEdit.vue'),
    meta: {
      title: i18n.t('ores.editTitle')
    },
    props: (route) => ({ oresId: Number(route.params.oresId) })
  },
  {
    path: '/ores/:oresId/details',
    name: 'OresDetails',
    component: () => import('@/views/ores/OresDetails.vue'),
    meta: {
      title: i18n.t('ores.reviewTitle')
    },
    props: (route) => ({ oresId: Number(route.params.oresId) })
  },
  {
    name: 'LodAssessmentImport',
    path: '/lod/assessment/import',
    meta: {
      title: i18n.t('lod.assessments.importTitle')
    },
    component: () => import ('@/views/assessment/AsssessmentImport.vue'),
  },
  {
    name: 'LodFinalizationList',
    path: '/lodFinalizations',
    component: () => import('@/views/lod/LodFinalizationListView.vue'),
    meta: {
      title: i18n.t('student.menu.lodFinalizations')
    }
  },
  {
    name: 'LodAssessmentTemplateList',
    path: '/lod/assessment/template/list',
    meta: {
      title: i18n.t('menu.selfEduForTemplateList')
    },
    component: () => import ('@/views/assessment/TemplateList.vue'),
  },
  {
    name: 'LodAssessmentTemplateCreate',
    path: '/lod/assessment/template/create',
    meta: {
      title: i18n.t('menu.selfEduForTemplateList')
    },
    component: () => import ('@/views/assessment/TemplateCreate.vue'),
  },
  {
    name: 'LodAssessmentTemplateDetails',
    path: '/lod/assessment/template/:id/details',
    meta: {
      title: i18n.t('menu.selfEduForTemplateList')
    },
    component: () => import ('@/views/assessment/TemplateDetails.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    name: 'LodAssessmentTemplateEdit',
    path: '/lod/assessment/template/:id/edit',
    meta: {
      title: i18n.t('menu.selfEduForTemplateList')
    },
    component: () => import ('@/views/assessment/TemplateEdit.vue'),
    props: (route) => ({ id: Number(route.params.id) })
  },
  {
    // catch all 404 - define at the very end
    path: "*",
    component: () => import('@/views/errors/NotFound.vue')
  },
];

const router = new VueRouter({
  mode: 'history',
  base: config.spaBaseUrlRelative,
  routes,
  scrollBehavior
});

let authService = new AuthService();

router.beforeEach(async (to, from, next) => {
  // console.log(`From: ${from.fullPath}`);
  // console.log(`To: ${to.fullPath}`);
  if (to.name === 'AccessDenied' || to.name === 'Login') {
    next();
  } else {
    const user = await authService.getUser();

    if(!user || user.expired) {
      next({
        path: '/account/login',
        query: { redirect: to.fullPath }
      });
    } else {
      const selectedRole = user.profile.selected_role;

      // Потребител с роля позволяваща достъп до модула.
      if (Constants.STUDENTS_MODULE_ALLOWED_ROLES.includes(selectedRole?.SysRoleID)) {

        // Потребител с роля учител има достъп до модула, ако е класен ръководител.
        if (UserRole.Teacher === selectedRole?.SysRoleID && selectedRole?.IsLeadTeacher === false) {
          next({
            path: '/errors/AccessDenied',
          });
        } else {
          next();
        }
      } else {
        // Всички автентикирани потребители, независимо от ролята, имат достъп до /software
        if(to.name === 'Home' || to.fullPath.indexOf('/software/' >= 0)) {
          next();
        } else {
          next({
            path: '/errors/AccessDenied',
          });
        }
      }
    }
  }
});

export default router;
