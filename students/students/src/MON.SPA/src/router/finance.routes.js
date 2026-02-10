import i18n from '@/language/language';

export default [
  {
    path: '/finance/naturalIndicators',
    name: 'NaturalIndicatorsView',
    meta: {
      title: i18n.t('menu.finance.naturalIndicators')
    },
    component: () => import('@/views/finance/NaturalIndicatorsView.vue'),
  },
{
    path: '/finance/resourceSupport',
    name: 'ResourceSupportView',
    meta: {
      title: i18n.t('menu.finance.resourceSupport')
    },
    component: () => import('@/views/finance/ResourceSupportView.vue'),
  }  
];
