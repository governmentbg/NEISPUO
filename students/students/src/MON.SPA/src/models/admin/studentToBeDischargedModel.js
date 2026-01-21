export class StudentToBeDischargedModel{
    constructor(obj = {}){
        this.selected = obj.selected || false;
        this.fullName = obj.fullName || '';
        this.gender = obj.gender || '';
        this.pinType = obj.pinType || '';
        this.pin = obj.pin || '';
        this.admissionDocumentId = obj.admissionDocumentId || '';
        this.personId = obj.personId || '';
        this.newInstitutonId = obj.newInstitutonId || '';
        this.newInstituton = obj.newInstituton || '';
        this.newPosition = obj.newPosition || '';
        this.admissionDate = obj.admissionDate || '';
        this.dischargeDate = obj.dischargeDate || '';
        this.noteDate = obj.noteDate || '';
        this.oldStudentClassid = obj.oldStudentClassid || '';
        this.oldClassGroupId = obj.oldClassGroupId || '';
        this.oldClassName = obj.oldClassName || '';
        this.oldInstitutionId = obj.oldInstitutionId || '';
        this.oldInstitution = obj.oldInstitution || '';
        this.oldPosition = obj.oldPosition || '';
        this.relocationDocumentId = obj.relocationDocumentId || '';
    }
}
