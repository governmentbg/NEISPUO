export class ReasonModel {
    constructor(obj = {}) {
        this.id = obj.id || null;
        this.reasonId = obj.reasonId || null;
        this.information = obj.information || '';
    }
}
