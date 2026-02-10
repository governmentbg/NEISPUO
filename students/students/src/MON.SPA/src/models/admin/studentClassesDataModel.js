export class StudentClassesDataModel{
    constructor(obj = {}){
        this.totalClassesCount = obj.totalClassesCount || 0;
        this.classTypesGroupCount = obj.classTypesGroupCount ? obj.classTypesGroupCount : '';
    }
}

  