import {StudentInfoModel} from './studentInfoModel.js';

export class ClassGroupModel{
    constructor (obj = {}){
        this.classId = obj.classId || null;
        this.name = obj.name || '';
        this.basicClassName = obj.basicClassName || '';
        this.classTypeName = obj.classTypeName || '';
        this.eduFormName = obj.eduFormName || '';
        this.count = obj.count || 0;
        this.students = obj.students ? obj.students.map(el => new StudentInfoModel(el)) : [];
    }
}