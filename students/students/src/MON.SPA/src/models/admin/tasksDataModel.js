
import {StudentToBeDischargedModel} from './studentToBeDischargedModel.js';
import {StudentToBeAdmissionModel} from './studentToBeAdmissionModel.js';

export class TasksDataModel{
    constructor(obj = {}){
        this.studentsForRelocationCount = obj.studentsForRelocationCount || 0;
        this.studentsForDischarge = obj.studentsForDischarge ? obj.studentsForDischarge.map(el => new StudentToBeDischargedModel(el)) : [];
        this.studentsForAdmission = obj.studentsForAdmission ? obj.studentsForAdmission.map(el => new StudentToBeAdmissionModel(el)) : [];
    }
}

  