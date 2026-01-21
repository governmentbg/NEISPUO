import i18n from '@/language/language';

export default [
  {
    path: '/class/:id',
    name: 'ClassGroupDetailsLayout',
    component: () => import('@/views/ClassGroupDetailsLayout.vue'),
    meta: {
    },
    props: (route) => ({ id: Number(route.params.id) }),
    children: [
      {
        path: 'details',
        name: 'ClassGroupDetails',
        component: () => import('@/views/institution/ClassGroupDetails.vue'),
        meta: {
          title: i18n.t('student.menu.institutionClass')
        },
        props: (route) => ({ classId: Number(route.params.id), schoolYear: Number(route.query.schoolYear) })
      },
      {
        path: 'absence',
        meta: {
          title: i18n.t('menu.absences')
        },
        name: 'AbsenceClass',
        component: () => import('@/views/absence/ClassAbsence.vue'),
        props: (route) => ({ classId: Number(route.params.id), schoolYear: Number(route.query.schoolYear) }),
      },
      {
        path: 'enroll',
        meta: {
          title: i18n.t('common.enroll')
        },
        name: 'EnrollmentToClass',
        component: () => import('@/views/class/ClassEnrollment.vue'),
        props: (route) => ({ classId: Number(route.params.id), schoolYear: Number(route.query.schoolYear) }),
      },
    ]
  }

];
