import ValidatorInstance from './rulesValidator';

export default {
  // called by Vue.use(RulesValidatorPlugin)
  install(Vue, options) {
    Vue.prototype.$validator = ValidatorInstance;
    console.log('Rules validator installed with option:', options);
  }
};