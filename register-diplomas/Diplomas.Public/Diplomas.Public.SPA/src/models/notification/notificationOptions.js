import { NotificationSeverity, NotificationPosition } from '@/enums/enums';

// https://github.com/euvl/vue-notification/
export class NotificationOptions {
  constructor(obj = {}) {
    this.group = obj.group || 'common'; // Name of the notification holder, if specified
    this.type = obj.type || NotificationSeverity.Info; // Class that will be assigned to the notification (warn, error, success, etc)
    this.position = obj.position || NotificationPosition.Default; // Part of the screen where notifications will pop out.
    this.duration = obj.duration || 6000; // ime (ms) animation stays visible (if negative - forever or until clicked)
    this.speed = obj.speed || 300; // Speed of animation showing/hiding
    this.reverse = obj.reverse || false; // Show notifications in reverse order
    this.ignoreDuplicates = obj.ignoreDuplicates || false; // Ignore repeated instances of the same notification
    this.closeOnClick = obj.closeOnClick || true; // Close notification when clicked
  }
}
