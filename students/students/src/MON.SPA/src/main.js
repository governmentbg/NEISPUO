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

import veeValidate from './plugins/veeValidate.js';
import vueSelect from './plugins/vueSelect.js';

import vuetify from './plugins/vuetify';
import moment from './plugins/moment';

import StudentHub from "./hubs/student-hub";

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

import UUID from "vue-uuid";
Vue.use(UUID);

Vue.use(StudentHub);

import Combo from '@/components/wrappers/CustomCombo.vue';
import DatePicker from '@/components/wrappers/CustomDatepicker.vue';
import ButtonTip from '@/components/wrappers/ButtonTip.vue';
import ButtonGroup from '@/components/wrappers/ButtonGroup.vue';
import ButtonMenu from '@/components/wrappers/ButtonMenu.vue';
import EditButton from '@/components/wrappers/EditButton.vue';
import ConfirmDlg from '@/components/wrappers/ConfirmDlg.vue';
import PromptDlg from '@/components/wrappers/PromptDlg.vue';
import YearPicker from '@/components/wrappers/YearPickerCombo.vue';
import YesNo from '@/components/common/YesNo.vue';
import FormLayout from '@/components/templates/FormLayout.vue';
import ContextualInfo from '@/components/wrappers/ContextualInfo.vue';
import CustomCheckbox from '@/components/wrappers/form/Checkbox.vue';
import DigitalSigningButton from '@/components/wrappers/DigitalSigningButton.vue';
import NumberSelector from '@/components/wrappers/NumberSelector.vue';
import EmailField from '@/components/wrappers/EmailField.vue';
import PhoneField from '@/components/wrappers/PhoneField.vue';
import CustomTextArea from '@/components/wrappers/form/TextAreaField.vue';
import CustomTextField from '@/components/wrappers/form/TextField.vue';
import CustomSelectList from '@/components/wrappers/CustomSelectList.vue';

Vue.component('Combo', Combo);
Vue.component('DatePicker', DatePicker);
Vue.component('ButtonTip', ButtonTip);
Vue.component('ButtonGroup', ButtonGroup);
Vue.component('ButtonMenu', ButtonMenu);
Vue.component('EditButton', EditButton);
Vue.component('ConfirmDlg', ConfirmDlg);
Vue.component('PromptDlg', PromptDlg);
Vue.component('YearPicker', YearPicker);
Vue.component('YesNo', YesNo);
Vue.component('FormLayout', FormLayout);
Vue.component('CInfo', ContextualInfo);
Vue.component('CCheckbox', CustomCheckbox);
Vue.component('SigningButton', DigitalSigningButton);
Vue.component('NumberSelector', NumberSelector);
Vue.component('EmailField', EmailField);
Vue.component('PhoneField', PhoneField);
Vue.component('CTextarea', CustomTextArea);
Vue.component('CTextField', CustomTextField);
Vue.component('CSelect', CustomSelectList);

import AuthService from '@/services/auth.service.js';
Vue.prototype.$auth = new AuthService();

import { StudentEventBus } from './buses/student-bus';
Vue.prototype.$studentEventBus = StudentEventBus;

import helper from '@/components/helper';
Vue.prototype.$helper = helper;

import { VueMaskDirective } from 'v-mask';
import './registerServiceWorker';
Vue.directive('mask', VueMaskDirective);

new Vue({
  router,
  store,
  i18n,
  veeValidate,
  vueSelect,
  vuetify,
  moment,
  render: h => h(App)
}).$mount('#app');
