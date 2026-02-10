import { TasksDataModel } from './tasksDataModel.js';
import { StudentsDataModel } from './studentsDataModel.js';
import {DiplomasDataModel} from './diplomasDataModel.js';

export class DirectorDashboardModel{
    constructor(obj = {}){ 
      this.institutionCity = obj.institutionCity ? obj.institutionCity : '';
      this.institutionName = obj.institutionName ? obj.institutionName : '';
      //this.institutionType =  obj.institutionType ? obj.institutionType : '';
      this.classTypesGroupCount = obj.classTypesGroupCount ? obj.classTypesGroupCount : '';
      this.tasksData = obj.tasksData ? new TasksDataModel(obj.tasksData) : new TasksDataModel();
      this.studentsData = obj.studentsData ? new StudentsDataModel(obj.studentsData)  : new StudentsDataModel();
      this.studentClassesData = obj.studentClassesData ?? [];
      this.diplomasData = obj.diplomasData ? new DiplomasDataModel(obj.diplomasData) : new DiplomasDataModel();
    }
}