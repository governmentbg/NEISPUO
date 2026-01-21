import Vue from 'vue';

import App from './App.vue';
import router from './router';
import store from './store';

import i18n from './language/language';

import api from "./services/api";

import { library } from '@fortawesome/fontawesome-svg-core';
import { faInfoCircle, faEdit, faExchangeAlt, faArchive, faChild, faSearch, faSave, faUser } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import 'material-design-icons-iconfont/dist/material-design-icons.css';

import vuetify from './plugins/vuetify';
import moment from './plugins/moment';

library.add(faInfoCircle, faEdit, faExchangeAlt, faArchive, faChild, faSearch, faSave, faUser);
Vue.component('font-awesome-icon', FontAwesomeIcon);

import Vue2Filters from 'vue2-filters';
Vue.use(Vue2Filters);
import "./filters/index";

Vue.config.productionTip = false;

Vue.prototype.$api = api;

import NotifierPlugin from '@/plugins/notifier/notifier-plugin';
Vue.use(NotifierPlugin);

import RulesValidatorPlugin from '@/plugins/rulesValidator/rules-validator-plugin';
Vue.use(RulesValidatorPlugin);

import '@/plugins/errorHandler/errorHandlerPlugin';

import ButtonTip from '@/components/wrappers/ButtonTip.vue';
import ButtonBase from '@/components/wrappers/ButtonBase.vue';
import TextField from '@/components/wrappers/TextField.vue';
import ButtonGroup from '@/components/wrappers/ButtonGroup.vue';
import ConfirmDlg from '@/components/wrappers/ConfirmDlg.vue';
import FormLayout from '@/components/templates/FormLayout.vue';

Vue.component('ButtonTip', ButtonTip);
Vue.component('ButtonBase', ButtonBase);
Vue.component('TextField', TextField);
Vue.component('ButtonGroup', ButtonGroup);
Vue.component('ConfirmDlg', ConfirmDlg);
Vue.component('FormLayout', FormLayout);

import AuthService from '@/services/auth.service.js';
Vue.prototype.$auth = new AuthService();

import helper from '@/components/helper';
Vue.prototype.$helper = helper;

import UUID from "vue-uuid";
Vue.use(UUID);

new Vue({
  router,
  store,
  i18n,
  vuetify,
  moment,
  render: h => h(App)
}).$mount('#app');
