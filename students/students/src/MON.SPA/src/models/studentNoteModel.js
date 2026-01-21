import Constants from '@/common/constants.js';

export class StudentNoteModel {
    constructor(obj = {} , moment = {}) {
        this.noteId = obj.noteId || null;
        this.title = obj.title || null;
        this.schoolYear = obj.schoolYear || null;
        this.personId = obj.personId || null;
        this.content = obj.content || null;
        this.issueDate = (obj.issueDate && moment) 
        ? moment(obj.issueDate).format(Constants.DATEPICKER_FORMAT) 
        : '';
    }
}