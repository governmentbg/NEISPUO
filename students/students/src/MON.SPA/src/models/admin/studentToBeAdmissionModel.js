export class StudentToBeAdmissionModel{
    constructor(obj = {}){
        this.selected = obj.selected || false;
        this.personId = obj.personId || 0;
        this.fullName = obj.fullName || '';
        this.gender = obj.gender || '';
        this.institutionId = obj.institutionId || 0;
        this.pin = obj.pin || 0;
        this.pinType = obj.pinType || 0;
    }
}