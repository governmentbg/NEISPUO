import { ReasonModel } from './reasonModel.js';

export class PersonalDevelopmentSupportModel {

    constructor(obj = {}) {
        this.id = obj.id || null,
        this.personId = obj.personId || null,
        this.schoolYear = obj.schoolYear || null,
        this.additionalModulesNeededForNonBulgarianSpeakingInfo = obj.additionalModulesNeededForNonBulgarianSpeakingInfo || '';
        this.earlyEvaluationAndEducationalRiskInfo = obj.earlyEvaluationAndEducationalRiskInfo || '';
        this.evaluationConclusionInfo = obj.evaluationConclusionInfo || '';
        this.studentTypeId = obj.studentTypeId || null;
        this.supportPeriodTypeId = obj.supportPeriodTypeId || null;
        this.earlyEvaluationReasons = obj.earlyEvaluationReasons ? obj.earlyEvaluationReasons.map(el => new ReasonModel(el)) : [];
        this.commonSupportTypeReasons = obj.commonSupportTypeReasons ? obj.commonSupportTypeReasons.map(el => new ReasonModel(el)) : [];
        this.additionalSupportTypeReasons = obj.additionalSupportTypeReasons ? obj.additionalSupportTypeReasons.map(el => new ReasonModel(el)) : [];
        this.earlyEvaluationDocuments = obj.earlyEvaluationDocuments || [];
        this.additionalSupportDocuments = obj.additionalSupportDocuments || [];
        this.commonSupportDocuments = obj.commonSupportDocuments || [];
    }
}