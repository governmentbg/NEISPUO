import { DocumentModel } from './documentModel.js';
export class IssueModel {
  constructor(obj = {}) {
    this.id = obj.id || null;
    this.title = obj.title;
    this.phone = obj.phone;
    this.description = obj.description;
    this.htmlDescription = obj.htmlDescription;
    this.categoryId = obj.categoryId;
    this.category = obj.category || null;
    this.subcategoryId = obj.subcategoryId;
    this.subcategory = obj.subcategory || null;
    this.priorityId = obj.priorityId;
    this.priority = obj.priority || null;
    this.statusId = obj.statusId;
    this.status = obj.status || null;
    this.isEscalated = obj.isEscalated;
    this.isLevel3Support = obj.isLevel3Support;
    this.submitterUsername = obj.submitterUsername;
    this.createDate = obj.createDate;
    this.modifyDate = obj.modifyDate;
    this.comments = obj.comments || [];
    this.statusHistory = obj.statusHistory || [];
    this.institution = obj.institution;
    this.survey = obj.survey || null;
    this.requestForInformation = obj.requestForInformation || false;
    this.surveyObject = JSON.parse(this.survey);
    this.assignedToSysUserId = obj.assignedToSysUserId || null;
    this.assignedToSysUser = obj.assignedToSysUser;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.events = this.comments.map(el => {return {createDate: el.createDate, type:'comment', uid: el.uid, element: el};})
      .concat(this.statusHistory.map(el => {return {createDate: el.createDate, type: 'status', uid: el.uid, element: el};}))
      .sort(function(a, b) {
        return (new Date(a.createDate) - new Date(b.createDate));
    });
  }
}
