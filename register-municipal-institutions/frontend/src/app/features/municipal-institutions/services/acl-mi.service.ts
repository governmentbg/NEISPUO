import { Injectable } from '@angular/core';
import { FormMode } from '@municipal-institutions/models/form-mode';
import { IMIFormComponentState } from './mi-form-component-state.interface';

@Injectable({
  providedIn: 'root',
})

export class ACLMunicipalityInstitutionService {
  constructor() {

  }

  getConfiguration(formMode: FormMode): IMIFormComponentState {
    /**
     * Properties that will always be read-only
     */
    const config = {
      InstitutionIDDisabled: true,
      MunicipalityDisabled: true,
      RegionDisabled: true,
      FinancialSchoolType: true,
      BudgetingInstitutionDisabled: true,
      ProcedureTypeDisabled: true,
      TransformTypeDisabled: true,
    } as IMIFormComponentState;
    if (formMode === 'CREATE') {
      return config;
    } if (formMode === 'EDIT') {
      return config;
    } if (formMode === 'READ') {
      return {
        ...config,
        BulstatDisabled: true,
        NameDisabled: true,
        AbbreviationDisabled: true,
        TownDisabled: true,
        LocalAreaDisabled: true,
        TRAddressDisabled: true,
        TRPostCodeDisabled: true,
        HeadFirstNameDisabled: true,
        HeadLastNameDisabled: true,
        HeadMiddleNameDisabled: true,
        BaseSchoolTypeDisabled: true,
        CPLRAreaTypeDisabled: true,
        DetailedSchoolTypeDisabled: true,
        ProcedureDateDisabled: true,
        TransformDetailsDisabled: true,
        YearDueDisabled: true,
        RIDocumentDateDisabled: true,
        RIDocumentNoDisabled: true,
        RIDocumentFileDisabled: true,
        RIFlexFieldValuesDisabled: true,
        PremDocsDisabled: true,
        PremInventoryDisabled: true,
        PremStudentsDisabled: true,
      };
    } if (formMode === 'DELETE') {
      return {
        ...config,
        BulstatDisabled: true,
        NameDisabled: true,
        AbbreviationDisabled: true,
        TownDisabled: true,
        LocalAreaDisabled: true,
        TRAddressDisabled: true,
        TRPostCodeDisabled: true,
        HeadFirstNameDisabled: true,
        HeadLastNameDisabled: true,
        HeadMiddleNameDisabled: true,
        BaseSchoolTypeDisabled: true,
        CPLRAreaTypeDisabled: true,
        DetailedSchoolTypeDisabled: true,
        RIFlexFieldValuesDisabled: true,
      };
    }
    if (formMode = 'TRANSFORM') {
      return {
        ...config,
        BulstatDisabled: true,
        NameDisabled: true,
        AbbreviationDisabled: true,
        TownDisabled: true,
        LocalAreaDisabled: true,
        TRAddressDisabled: true,
        TRPostCodeDisabled: true,
        HeadFirstNameDisabled: true,
        HeadLastNameDisabled: true,
        HeadMiddleNameDisabled: true,
        BaseSchoolTypeDisabled: true,
        CPLRAreaTypeDisabled: true,
        DetailedSchoolTypeDisabled: true,
        RIFlexFieldValuesDisabled: true,
      };
    }
    return config;
  }
}
