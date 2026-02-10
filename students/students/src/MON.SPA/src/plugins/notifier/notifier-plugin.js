import NotificationModal from  './NotificationModal.vue';
import NotificationSnackbar from './NotificationSnackbar.vue';
import NotifierInstance from './notifier';
import Notifications from 'vue-notification';

export default {
  // called by Vue.use(NotifierPlugin)
  install(Vue, options) {
    Vue.use(Notifications);
    Vue.prototype.$notifier = NotifierInstance;
    Vue.component("NotificationModal", NotificationModal);
    Vue.component("NotificationSnackbar", NotificationSnackbar);
    console.log('NotifierPlugin installed with options:', options);
  }
};