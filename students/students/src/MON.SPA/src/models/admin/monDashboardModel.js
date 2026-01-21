import { StudentsDataModel } from './studentsDataModel.js';

export class MonDashboardModel{
    constructor(obj = {}){
      this.classTypesGroupCount = obj.classTypesGroupCount ? obj.classTypesGroupCount : '';
      this.studentsData = obj.studentsData ? new StudentsDataModel(obj.studentsData)  : new StudentsDataModel();
      this.studentClassesData = obj.studentClassesData ?? []; 
    }
}
