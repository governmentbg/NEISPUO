import Vue from 'vue';
import VueI18n from 'vue-i18n';
import store from '@/store/index';

Vue.use(VueI18n);

const locales = {
    bg: require('@/assets/localization/bg.json'),
    en: require('@/assets/localization/en.json')
};

const i18n = new VueI18n({
    locale: store.getters.language,
    fallbackLocale: 'bg',
    messages: locales
});

export default i18n;
