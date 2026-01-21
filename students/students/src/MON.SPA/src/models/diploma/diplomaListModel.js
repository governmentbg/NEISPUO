import Constants from '@/common/constants.js';

export class DiplomaListModel {
  constructor(obj = {}, component = {}) {
    this.id = obj.diplomaId || null;
    this.basicDocumentType = obj.basicDocumentType || '';
    this.series = obj.series || null;
    this.factoryNumber = obj.factoryNumber || null;
    this.protocolNumber = obj.protocolNumber || null;
    this.institutionName = obj.institutionName || null;
    this.schoolYear = obj.schoolYear || null;
    this.registrationDate = (obj.registrationDate && component.$moment) ? component.$moment(obj.registrationDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.reportFormPath = obj.reportFormPath;
    this.isFinalized = obj.isFinalized || false;
    this.isSigned = obj.isSigned || false;
    this.isCancelled = obj.isCancelled || false;
    this.canBeDeleted = obj.canBeDeleted;

    this.diplomaStatus = (()=>{
        if(obj.isFinalized){
            if(obj.isFinalized &&  obj.isCancelled){
                return component.$t("diplomas.isAnulledStatus");
            }
            return component.$t("diplomas.isFinalizedStatus");
        }

        if(obj.isPublic){
           return component.$t("diplomas.isPublicStatus");
        }else if(obj.isSigned){
           return  component.$t("diplomas.isSignedStatus");
        }
    })();
  }
}
