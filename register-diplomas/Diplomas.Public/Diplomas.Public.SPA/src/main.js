import Vue from 'vue';
import App from './App.vue';
import router from './router';
import vuetify from './plugins/vuetify';
import i18n from './plugins/language';
import http from './services/http.service';

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
Vue.component('FontAwesomeIcon', FontAwesomeIcon);
import 'material-design-icons-iconfont/dist/material-design-icons.css';

import NotifierPlugin from '@/plugins/notifier/notifier-plugin';
Vue.use(NotifierPlugin);

import '@/plugins/errorHandler/errorHandlerPlugin';

import { VueReCaptcha } from 'vue-recaptcha-v3';
Vue.use(VueReCaptcha, {
  siteKey: '6LeJv2waAAAAAATa8a2SqM_xjx7k_aGwVm-25NPP',
  loaderOptions: {
    useRecaptchaNet: true
  }
});

Vue.config.productionTip = false;
Vue.prototype.$http = http;
//vuetify.lang.current = i18n.locale;

new Vue({
  router,
  vuetify,
  i18n,
  render: h => h(App)
}).$mount('#app');
