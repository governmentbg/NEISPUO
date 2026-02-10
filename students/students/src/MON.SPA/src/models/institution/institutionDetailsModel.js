import {ClassGroupModel} from './classGroupModel.js';

export class InstitutionDetailsModel{
    constructor(obj = {}){
        this.name = obj.name || '';
        this.classes = obj.classes ? obj.classes.map(el => new ClassGroupModel(el)) : [];        
    }
}