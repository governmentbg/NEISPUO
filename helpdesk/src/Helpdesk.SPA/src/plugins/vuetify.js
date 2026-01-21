import Vue from 'vue';
import Vuetify,{ VBtnToggle} from 'vuetify/lib';
import '@fortawesome/fontawesome-free/css/all.css';
import bg from '@/assets/localization/vuetify.bg';
import en from '@/assets/localization/vuetify.en';
import store from '@/store/index';

Vue.use(Vuetify, {
  components:{
    // Добавя се, за да се копират стиловете
    VBtnToggle
  },
  iconfont: 'fa'
 });

export default new Vuetify({
   lang: {
    locales: { bg, en },
    current: store.getters.language,
  },
  theme: {
    themes: {
      // НЕИСПУО theme
      light: {
        primary: '#2F73DA', //#1976D2',
        secondary: '#424242',
        accent: '#7476f7',//'#82B1FF',
        error: '#DF320C', //'#FF5252',
        info: '#2196F3',
        success: '#09A57F', //'#4CAF50',
        warning: '#FFB400', //'#FFC107',
        stratos: '#002966'
      },

      // Default theme
      // light: {
      //   primary: #1976D2',
      //   secondary: '#424242',
      //   accent: '#82B1FF',
      //   error: '#FF5252',
      //   info: '#2196F3',
      //   success: '#4CAF50',
      //   warning: '#FFC107',
      // },
    },
  },
});
