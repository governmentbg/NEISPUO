export class NotificationModalOptions {
  constructor(obj = {}) {
    this.disabled = obj.disabled || false;
    this.fullscreen = obj.fullscreen || false;
    this.light = obj.light || false;
    this.persistent = obj.persistent  || false;
    this.scrollable = obj.scrollable  || false;
    this.width = obj.width  || 'auto';
    this.maxWidth = obj.maxWidth  || 800;
    this.showTextInPreTag = obj.showTextInPreTag || '';
    this.showSubtitle = obj.showSubtitle || '';
  }
}
