import { LodFinalizationModel } from "./lodFinalizationModel";

export class LodFinalizationUndoModel extends LodFinalizationModel {
  constructor(obj = {}) {
    super(obj);
    this.reason = obj.reason;
  }
}