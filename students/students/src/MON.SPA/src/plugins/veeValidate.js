import Vue from 'vue';
import { ValidationObserver, ValidationProvider, extend } from "vee-validate";
import { required } from 'vee-validate/dist/rules';

// Add a rule.
extend('required', {
  ...required,
  message: 'Полето е задължително.'
});

// Register it globally
Vue.component("ValidationObserver", ValidationObserver);
Vue.component("ValidationProvider", ValidationProvider);

export default ValidationProvider;

