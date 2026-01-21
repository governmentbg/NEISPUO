import { NotificationModel } from "@/models/notification/notificationModel.model.js";
import { NotificationOptions } from '@/models/notification/notificationOptions.js';
import { NotificationSeverity } from '@/enums/enums';
import Vue from 'vue';
import { notifierEvents } from './notifierEvents.bus';

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

  modalError(title, text) {
    this.modal(title, text, NotificationSeverity.Error);
  }

  modalSuccess(title, text) {
    this.modal(title, text, NotificationSeverity.Success);
  }

  modalWarn(title, text) {
    this.modal(title, text, NotificationSeverity.Warn);
  }

  modalInfo(title, text) {
    this.modal(title, text, NotificationSeverity.Info);
  }

  error(title, text, timeout) {
    const model = new NotificationModel({ title, text, severity: NotificationSeverity.Error, timeout: timeout || 10000} );
    notifierEvents.$emit('show-snackbar', model);
  }

  success(title, text, timeout) {
    const model = new NotificationModel({ title, text, severity: NotificationSeverity.Success, timeout: timeout || 10000 });
    notifierEvents.$emit('show-snackbar', model);
  }

  warn(title, text, timeout) {
    const model = new NotificationModel({ title, text, severity: NotificationSeverity.Warn, timeout: timeout || 10000 });
    notifierEvents.$emit('show-snackbar', model);
  }

  info(title, text, timeout) {
    const model = new NotificationModel({ title, text, severity: NotificationSeverity.Info, timeout: timeout || 10000 });
    notifierEvents.$emit('show-snackbar', model);
  }

}

export default new Notifier();
