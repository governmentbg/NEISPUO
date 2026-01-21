export class NomenclatureModel {
    constructor(obj = {}) {
        this.name = obj.name || '';
        this.text = obj.text || '';
        this.isValid = obj.isValid || false;
        this.validFrom = obj.validFrom || '';
        this.validTo = obj.validTo || '';
    }
}