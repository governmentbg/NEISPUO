import { NotificationSeverity } from '@/enums/enums';

export class NotificationModel {
  constructor(obj = {}) {
    this.title = obj.title || '';
    this.text = obj.text || '';
    this.severity = obj.severity || NotificationSeverity.Info;
    this.time = obj.time || new Date();
    this.timeout = obj.timeout || -1;
  }
}
