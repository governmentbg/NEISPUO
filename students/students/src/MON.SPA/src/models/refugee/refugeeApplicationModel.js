import Constants from '@/common/constants.js';
import { DocumentModel } from '../documentModel.js';
import { RefugeeApplicationChildModel } from '@/models/refugee/refugeeApplicationChildModel';

export class RefugeeApplicationModel {
  constructor(obj = {}, moment = {}) {
    this.id = obj.id,
    this.applicantFullName = obj.applicantFullName;
    this.personalId = obj.personalId;
    this.personalIdType = obj.personalIdType || 0;
    this.nationalityId = obj.nationalityId ;
    this.regionId = obj.regionId;
    this.region = obj.region;
    this.phone = obj.phone;
    this.email = obj.email;
    this.address = obj.address;
    this.townId = obj.townId;
    this.guardianType = obj.guardianType;
    this.applicationDate = (obj.applicationDate && moment) ? moment(obj.applicationDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.status = obj.status;
    this.childrenCount = obj.childrenCount;
    this.childrenWithOrderCount = obj.childrenWithOrderCount;
    this.documents = obj.documents ? obj.documents.map(el => new DocumentModel(el)) : [];
    this.children = obj.children ? obj.children.map(el => new RefugeeApplicationChildModel(el, moment)) : [];

    this.region = obj.region;
    this.nationality = obj.nationality;
    this.guardianTypeName = obj.guardianTypeName;
    this.town = obj.town;
    this.personalIdTypeName = obj.personalIdTypeName;

  }
}
