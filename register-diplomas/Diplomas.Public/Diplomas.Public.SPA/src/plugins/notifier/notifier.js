import Vue from 'vue';
import { NotificationOptions } from '@/models/notification/notificationOptions.js';
import { NotificationModel } from "@/models/notification/notificationModel.model.js";
import { NotificationSeverity } from '@/enums/enums';
import { notifierEvents } from './notifierEvents.bus.js';

export class Notifier {
  toast(title, text, severity, options) {
    options = new NotificationOptions(options);
    Vue.notify({
        title,
        text,
        group: options.position,
        type: severity || NotificationSeverity.Info,
        duration: options.duration,
        speed: options.speed,
    });
  }

  snackbar(title, text, severity) {
    const model = new NotificationModel({title, text, severity});
    notifierEvents.$emit('show-snackbar', model);
  }

  modal(title, text, severity, options) {
    const model = new NotificationModel({ title, text, severity });
    notifierEvents.$emit('show-modal', model, options);
  }

  error(title, text, timeout) {
    const model = new NotificationModel({ title, text, severity: 'error', timeout} );
    notifierEvents.$emit('show-snackbar', model);
  }

  success(title, text, timeout) {  
    const model = new NotificationModel({ title, text, severity: 'success', timeout });
    notifierEvents.$emit('show-snackbar', model);
  }
}

export default new Notifier();