import { DynamicFieldModel } from '@/models/dynamic/dynamicFieldModel';

export class DynamicSectionModel {
  constructor(obj = {}) {
    this.uid = obj.uid || Math.floor((Math.random()) * 0x10000).toString(16);
    this.id = obj.id;
    this.title = obj.title;
    this.titleEn = obj.titleEn;
    this.order = obj.order;
    this.visible = obj.visible;
    this.items = obj.items
      ? obj.items.map(x => new DynamicFieldModel(x))
      : [];
  }
}
