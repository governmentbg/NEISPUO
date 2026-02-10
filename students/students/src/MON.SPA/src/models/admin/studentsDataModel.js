export class StudentsDataModel{
    constructor(obj = {}){
        this.totalStudentsInInstitution = obj.totalStudentsInInstitution || 0;
        this.studentsByGroupCount = obj.studentsByGroupCount || null;
    }
}