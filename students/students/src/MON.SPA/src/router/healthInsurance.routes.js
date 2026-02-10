import i18n from '@/language/language';

export default [
  {
    path: '/healthInsurance/list',
    name: 'HealthInsuranceStudentsList',
    meta: {
      title: i18n.t('menu.healthInsurance.studentsList')
    },
    component: () => import('@/views/healthInsurance/HealthInsuranceStudentsListView.vue'),
  },
  {
    path: '/healthInsurance/exports',
    name: 'HealthInsuranceExportedFilesList',
    meta: {
      title: i18n.t('menu.healthInsurance.exportedFilesList')
    },
    component: () => import('@/views/healthInsurance/HealthInsuranceExportedFilesListView.vue'),
  }
];
