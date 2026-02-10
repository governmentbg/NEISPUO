export class StudentInfoModel{
    constructor(obj = {}){
        this.personId = obj.personId || null;
        this.fullName = obj.fullName || '';
        this.classNumber = obj.classNumber || null;
        this.birthPlace = obj.birthPlace || '';
        this.phone = obj.phone || '';
        this.hasSpecialNeeds = obj.hasSpecialNeeds || false;
        this.eduFormName = obj.eduFormName || '';
        this.username = obj.username,
        this.initialPassword = obj.initialPassword;
    }
}