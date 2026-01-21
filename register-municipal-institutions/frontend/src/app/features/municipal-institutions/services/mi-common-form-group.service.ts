import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { bulstatValidator } from '@shared/validators/bulstat-validator';
import { ICommonFormBuildConfig } from '@shared/services/common-form-service/common-form-build-config.interface';
import { IMIFormComponentState } from './mi-form-component-state.interface';
@Injectable({
  providedIn: 'root',
})
export class MICommonFormGroupService {
  constructor(private fb: FormBuilder) { }

  public buildForm(configuration: ICommonFormBuildConfig<MunicipalInstitution, IMIFormComponentState>) {
    const { response, config } = configuration;
    return this.fb.group({
      RIInstitutionID: [response?.RIInstitutionID || null],
      InstitutionID: [
        {
          value: response?.InstitutionID || null,
          disabled: config?.InstitutionIDDisabled,
        },
        { validators: [Validators.required] },
      ],
      Name: [
        {
          value: response?.Name || null,
          disabled: config?.NameDisabled,
        },
        { validators: [Validators.required] },
      ],
      Abbreviation: [
        {
          value: response?.Abbreviation || null,
          disabled: config?.AbbreviationDisabled,
        },
        { validators: [Validators.required] },
      ],
      Bulstat: [
        {
          value: response?.Bulstat || null,
          disabled: config?.BulstatDisabled,
        },
        { validators: [Validators.required, Validators.minLength(9), bulstatValidator()] },
      ],
      Region: [
        {
          value: response?.Town?.Municipality?.Region || null,
          disabled: config?.RegionDisabled,
        },
        { validators: [Validators.required] },
      ],
      Municipality: [
        {
          value: response?.Town?.Municipality || null,
          disabled: config?.MunicipalityDisabled,
        },
        { validators: [Validators.required] },
      ],
      Town: [
        {
          value: response?.Town || null,
          disabled: config?.TownDisabled,
        },
        { validators: [Validators.required] },
      ],
      LocalArea: [
        {
          value: response?.LocalArea || null,
          disabled: config?.LocalAreaDisabled,
        },
      ],
      FinancialSchoolType: [
        {
          value: response?.FinancialSchoolType || null,
          disabled: config?.FinancialSchoolType,
        },
        { validators: [Validators.required] },
      ],
      BaseSchoolType: [
        {
          value: response?.BaseSchoolType || null,
          disabled: config?.BaseSchoolTypeDisabled,
        },
        { validators: [Validators.required] },
      ],
      BudgetingInstitution: [
        {
          value: response?.BudgetingInstitution || null,
          disabled: config?.BudgetingInstitutionDisabled,
        },
        { validators: [Validators.required] },
      ],
      DetailedSchoolType: [
        {
          value: response?.DetailedSchoolType || null,
          disabled: config?.DetailedSchoolTypeDisabled,
        },
        { validators: [Validators.required] },
      ],
      CPLRAreaType: [
        {
          value: response?.RIProcedure?.RICPLRArea?.CPLRAreaType || response?.CPLRAreaType || null,
          disabled: config?.CPLRAreaTypeDisabled,
        },
      ],
      RIFlexFieldValues: this.fb.array([]),
      TRPostCode: [
        {
          value: response?.TRPostCode || null,
          disabled: config?.TRPostCodeDisabled,
        },
        { validators: [Validators.required] },
      ],

      TRAddress: [
        {
          value: response?.TRAddress || null,
          disabled: config?.TRAddressDisabled,
        },
        { validators: [Validators.required] },
      ],

      HeadFirstName: [
        {
          value: response?.HeadFirstName || null,
          disabled: config?.HeadFirstNameDisabled,
        },
      ],
      HeadMiddleName: [
        {
          value: response?.HeadMiddleName || null,
          disabled: config?.HeadMiddleNameDisabled,
        },
      ],
      HeadLastName: [
        {
          value: response?.HeadLastName || null,
          disabled: config?.HeadLastNameDisabled,
        },
      ],

      /**
       * NOTE: IF RIProcedure form changes, YOU LIKELY MUST CHANGE THE FORM SHAPE BELLOW AS WELL (under addNewRIProcedureForm)
       */
      RIProcedure: this.fb.group({
        TransformDetails: [
          {
            value: response?.RIProcedure?.TransformDetails || null,
            disabled: config?.TransformDetailsDisabled,
          },
        ],
        ProcedureDate: [
          {
            value: response?.RIProcedure?.ProcedureDate ? new Date(response?.RIProcedure?.ProcedureDate) : null,
            disabled: config?.ProcedureDateDisabled,
          },
          { validators: [Validators.required] },
        ],
        YearDue: [
          {
            value: response?.RIProcedure?.YearDue || null,
            disabled: config?.YearDueDisabled,
          },
          { validators: [Validators.required] },
        ],
        RIDocument: this.fb.group({
          RIDocumentID: [{ value: response?.RIProcedure?.RIDocument?.RIDocumentID || null, disabled: true }],
          DocumentNo: [
            { value: response?.RIProcedure?.RIDocument?.DocumentNo || null, disabled: config?.RIDocumentNoDisabled },
            { validators: [Validators.required] },
          ],
          StateNewspaperData: [
            { value: response?.RIProcedure?.RIDocument?.StateNewspaperData || null, disabled: config?.RIDocumentNoDisabled },
            { validators: [Validators.required] },
          ],
          DocumentNotes: [
            { value: response?.RIProcedure?.RIDocument?.DocumentNotes || null, disabled: config?.RIDocumentNoDisabled },
          ],
          DocumentDate: [
            {
              value: response?.RIProcedure?.RIDocument?.DocumentDate
                ? new Date(response?.RIProcedure?.RIDocument?.DocumentDate)
                : null,
              disabled: config?.RIDocumentDateDisabled,
            },
            { validators: [Validators.required] },
          ],
          DocumentFile: [
            { value: response?.RIProcedure?.RIDocument?.DocumentFile || null, disabled: config?.RIDocumentFileDisabled },
          ],
        }),
        ProcedureType: [
          {
            value: response?.RIProcedure?.ProcedureType || null,
            disabled: config?.ProcedureTypeDisabled,
          },
        ],
        TransformType: [
          {
            value: response?.RIProcedure?.TransformType || null,
            disabled: config?.TransformTypeDisabled,
          },
        ],
        RIPremInstitution: this.fb.group({
          PremStudents: [
            {
              value: response?.RIProcedure?.RIPremInstitution?.PremStudents || null,
              disabled: config?.PremStudentsDisabled,
            },
          ],
          PremDocs: [
            {
              value: response?.RIProcedure?.RIPremInstitution?.PremDocs || null,
              disabled: config?.PremDocsDisabled,
            },
          ],

          PremInventory: [
            {
              value: response?.RIProcedure?.RIPremInstitution?.PremInventory || null,
              disabled: config?.PremInventoryDisabled,
            },
          ],
        }),
      }),
    });
  }

  /**
   * Only required by specific forms.
   * IF THIS FORM SHAPE CHANGES, YOU LIKELY MUST CHANGE THE FORM SHAPE ABOVE AS WELL (under buildForm)
   */
  public addNewRIProcedureForm(
    riInstitutionForm: FormGroup,
    configuration: ICommonFormBuildConfig<MunicipalInstitution, IMIFormComponentState>,
  ) {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const { response, config } = configuration;

    riInstitutionForm.addControl(
      '_newRIProcedure',
      this.fb.group({
        TransformDetails: [
          {
            value: response?.RIProcedure?.TransformDetails || null,
            disabled: false,
          },
        ],
        ProcedureDate: [
          {
            value: response?.RIProcedure?.ProcedureDate ? new Date(response?.RIProcedure?.ProcedureDate) : null,
            disabled: false,
          },
        ],
        YearDue: [
          {
            value: response?.RIProcedure?.YearDue || null,
            disabled: false,
          },
        ],
        RIDocument: this.fb.group({
          RIDocumentID: [{ value: response?.RIProcedure?.RIDocument?.RIDocumentID || null, disabled: false }],
          DocumentNo: [
            { value: response?.RIProcedure?.RIDocument?.DocumentNo || null, disabled: false },
            { validators: [Validators.required] },
          ],
          StateNewspaperData: [
            { value: response?.RIProcedure?.RIDocument?.StateNewspaperData || null, disabled: false },
            { validators: [Validators.required] },
          ],
          DocumentNotes: [
            { value: response?.RIProcedure?.RIDocument?.DocumentNotes || null, disabled: false },
            { validators: [Validators.required] },
          ],
          DocumentDate: [
            {
              value: response?.RIProcedure?.RIDocument?.DocumentDate
                ? new Date(response?.RIProcedure?.RIDocument?.DocumentDate)
                : null,
              disabled: false,
            },
            { validators: [Validators.required] },
          ],
          DocumentFile: [
            { value: response?.RIProcedure?.RIDocument?.DocumentFile || null, disabled: false },
            { validators: [Validators.required] },
          ],
        }),
      }),
    );
  }
}
