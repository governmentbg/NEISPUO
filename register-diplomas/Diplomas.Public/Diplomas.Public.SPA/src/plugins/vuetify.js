import '@fortawesome/fontawesome-free/css/all.css'; // Ensure you are using css-loader
import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import bg from '@/assets/localization/vuetify.bg';
import en from '@/assets/localization/vuetify.en';

Vue.use(Vuetify, {
  iconfont: 'fa'
});

export default new Vuetify({
  lang: {
    locales: { bg, en },
    current: (localStorage.getItem('lang') || 'bg')
  },
  theme: {
    themes: {
      // НЕИСПУО theme
      light: {
        primary: '#2F73DA',
        secondary: '#424242',
        accent: '#7476f7',
        error: '#DF320C',
        info: '#2196F3',
        success: '#09A57F',
        warning: '#FFB400',
        stratos: '#002966'
      },
    },
  },
});
