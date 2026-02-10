import { DropDownModel } from '@/models/dropdownModel';

export class EnvironmentCharacteristicsModel {
    constructor(obj = {}) {
        
        this.gpPhone = obj.gpPhone || null;
        this.gpName = obj.gpName || null;
        this.hasParentConsent = obj.hasParentConsent || false;
        this.livesWithFosterFamily = obj.livesWithFosterFamily || false;
        this.representedByTheMajor = obj.representedByTheMajor || false;
        this.nativeLanguage = obj.nativeLanguage || new DropDownModel();
        this.familyEducationWeight = obj.familyEducationWeight || 0;
        this.familyWorkStatusWeight = obj.familyWorkStatusWeight || 0;
    }
  }