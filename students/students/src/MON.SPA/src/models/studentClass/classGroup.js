export class ClassGroup{
    constructor(obj = {}){
        this.schoolYear = obj.schoolYear || 0;
        this.institutionId = obj.institutionId || 0;
        this.institutionName = obj.institutionName || '';
        this.className = obj.className || '';
        this.basicClassName = obj.basicClassName || '';
        this.classTypeName = obj.classTypeName || '';
        this.classSpecialityName = obj.classSpecialityName || '';
        this.classEduFormName = obj.classEduFormName || '';
    }
}
