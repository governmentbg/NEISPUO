export class DynamicFieldModel {
  constructor(obj = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id || '';
    this.columnName = obj.columnName || '';
    this.type = obj.type || '';
    this.label = obj.label  || '';
    this.labelEn = obj.labelEn  || '';
    this.order = obj.order || 1;

    this.readonly = obj.readonly || false;
    this.required = obj.required || false;
    this.visible = obj.visible || false;
    this.filterable = obj.filterable || false;
    this.editable = obj.editable || false;
    this.clearable = obj.clearable || false;

    this.min = obj.min || '';
    this.max = obj.max || '';
    this.format = obj.format || undefined;

    this.returnObject = obj.returnObject || false;
    this.optionsUrl = obj.optionsUrl || '';
    this.deferOptionsLoading = obj.deferOptionsLoading || false;
    this.showDeferredLoadingHint = obj.showDeferredLoadingHint || false;

    this.itemText = obj.itemText || undefined;
    this.itemValue = obj.itemValue || undefined;

    this.cols = obj.cols || '';
    this.xl = obj.xl || '';
    this.lg = obj.lg || '';
    this.md = obj.md || '';
    this.sm = obj.sm || '';

    this.hint = obj.hint || '';
    this.persistentHint = obj.persistentHint || false;
    this.contextInfo = obj.contextInfo || '';

    this.showGRAOSearch = obj.showGRAOSearch || false;

    this.defaultValue = obj.defaultValue;
  }
}
