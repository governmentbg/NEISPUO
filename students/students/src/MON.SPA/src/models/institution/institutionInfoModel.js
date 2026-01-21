export class InstitutionInfoModel{
    constructor(obj = {}){
        this.id = obj.id || null;
        this.name = obj.name || '';
        this.region = obj.region || '';
        this.regionId = obj.regionId || null;
        this.municipality = obj.municipality || '';
        this.municipalityId = obj.municipalityId || null;
        this.town = obj.town || '';
        this.townId = obj.townId || null;
    }
}