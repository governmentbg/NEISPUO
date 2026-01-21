import Vue from 'vue';
import errorService from '@/services/error.service.js';
import { Severity, AuditModule } from "@/enums/enums";

Vue.config.errorHandler = (err, vm, info) => {
  console.error(`Error: ${err.toString()}\nInfo: ${info}`);
  let additionalInformation = `window.location: ${window.location}\r\n`;
  let stackTrace = `${err.stack.substring(0, 200)}...${err.stack.substring(err.stack.length - 200)}`;
  errorService.add({severity: Severity.Error, moduleId: AuditModule.Helpdesk, message: err.toString(), trace: `${info}\r\nStack trace: ${stackTrace}` , additionalInformation: additionalInformation }).catch(() => {});
};

Vue.config.warnHandler = function(msg, vm, trace) {
  console.warn(`Warn: ${msg}\nTrace: ${trace}`);
  let additionalInformation = `window.location: ${window.location}\r\n`;
  let err = new Error();
  let stackTrace = `${err.stack.substring(0, 200)}...${err.stack.substring(err.stack.length - 200)}`;
  errorService.add({severity: Severity.Warning, moduleId: AuditModule.Helpdesk, message: msg, trace: `${trace}\r\nStack trace: ${stackTrace}`, additionalInformation: additionalInformation }).catch(() => {});
};

window.onerror = function(message, source, lineno, colno, error) {
  console.error(`${message}, ${source}, ${lineno}, ${colno}, ${error}`);
  errorService.add({severity: Severity.Error, moduleId: AuditModule.Helpdesk, message: message, trace: `${source}: ${lineno}: ${colno}`, additionalInformation: error }).catch(() => {});
};
