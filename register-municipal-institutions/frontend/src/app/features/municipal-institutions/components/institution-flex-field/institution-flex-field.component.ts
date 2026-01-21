import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import {
  ControlContainer, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators,
} from '@angular/forms';
import { AuthQuery } from '@core/authentication/auth-state-manager/auth.query';
import { EnvironmentService } from '@core/services/environment.service';
import { filter, switchMap, tap } from 'rxjs/operators';
import { FlexFieldDTO } from '../../../flex-fields/pages/flex-field-detail/flex-field-detail.page';

interface RIFlexFieldValue {
  RIFlexFieldValueID?: number;
  Value: any;
  /**
   * Relations
   */
  RIInstitution?: { RIInstitutionID: number };
  RIFlexField: FlexFieldDTO;
}

@Component({
  selector: 'app-institution-flex-field',
  templateUrl: './institution-flex-field.component.html',
  styleUrls: ['./institution-flex-field.component.scss'],
})
export class InstitutionFlexFieldComponent implements OnInit {
  @Input() RIInstitutionID: number;

  @Input() disableAddition: boolean;

  @Input() disableDeletion: boolean;

  @Input() isViewFlexField: boolean;

  parentForm: FormGroup;

  get formArray(): FormArray {
    return this.parentForm.get('RIFlexFieldValues') as FormArray;
  }

  selection: FlexFieldDTO = null;

  availableFlexFields = [];

  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(
    private controlContainer: ControlContainer,
    private httpClient: HttpClient,
    private authQuery: AuthQuery,
    private formBuilder: FormBuilder,
    private envService: EnvironmentService,
  ) { }

  ngOnInit(): void {
    this.parentForm = this.controlContainer.control as FormGroup;

    this.authQuery.myMunicipality$
      .pipe(
        filter((municipality) => !!municipality),
        switchMap((municipality) => this.loadMunicipalityFlexFields(municipality)),
      )
      .subscribe(); // TODO: Catch and display error messages
  }

  private loadMunicipalityFlexFields(municipality) {
    // this.scrollService.scrollTo('body');

    return this.httpClient.get<FlexFieldDTO[]>(`${this.baseEndpoint}/v1/ri-flex-field`).pipe(
      tap((ff) => {
        this.availableFlexFields = ff;
      }),
    );
  }

  private addFormGroupValidations(formGroup: FormGroup, selection: FlexFieldDTO) {
    const control = formGroup.get('Value');
    const validators: ValidatorFn[] = [];
    if (selection.Mandatory) {
      validators.push(Validators.required);
    }

    if (selection.Data.maxLength) {
      validators.push(Validators.maxLength(selection.Data.maxLength));
    }

    if (selection.Data.maxValue) {
      validators.push(Validators.max(selection.Data.maxValue));
    }

    control.setValidators(validators);
  }

  private createFormGroup(ffOrFfvalue: FlexFieldDTO | RIFlexFieldValue): FormGroup {
    const isFlexFieldValue = !!(ffOrFfvalue as RIFlexFieldValue)?.RIFlexField;
    const initialValues = {
      _name: null,
      _type: null,
      Value: null,
      RIFlexField: null,
      RIFlexFieldValueID: null,
      Data: null,
    };

    if (isFlexFieldValue) {
      const ffv: RIFlexFieldValue = ffOrFfvalue as RIFlexFieldValue;

      initialValues._name = ffv.RIFlexField.Data.label;
      initialValues._type = ffv.RIFlexField.Data.type;
      initialValues.Value = ffv.Value;
      initialValues.RIFlexField = { RIFlexFieldID: ffv.RIFlexField.RIFlexFieldID, Data: ffv.RIFlexField.Data };
      initialValues.RIFlexFieldValueID = ffv.RIFlexFieldValueID;
    } else {
      // isFlexFieldDTO
      const ff: FlexFieldDTO = ffOrFfvalue as FlexFieldDTO;
      initialValues._name = ff.Data.label;
      initialValues._type = ff.Data.type;
      initialValues.Value = null;
      initialValues.RIFlexField = { RIFlexFieldID: ff.RIFlexFieldID, Data: ff.Data };
      initialValues.RIFlexFieldValueID = null;
    }

    return this.formBuilder.group({
      _name: [initialValues._name],
      _type: [initialValues._type],
      Value: [initialValues.Value],
      RIFlexField: [initialValues.RIFlexField, Validators.required],
      RIFlexFieldValueID: [initialValues.RIFlexFieldValueID],
    });
  }

  /**
   * 1.This method is called by parent component when patching value
   */
  prePopulateFormArray(riFlexFieldValues: RIFlexFieldValue[] = []) {
    this.formArray.clear();

    for (const riFlexFieldValue of riFlexFieldValues) {
      const formGroup = this.createFormGroup(riFlexFieldValue);

      this.addFormGroupValidations(formGroup, riFlexFieldValue.RIFlexField);
      this.formArray.push(formGroup);
    }
  }

  addFlexField(selection: FlexFieldDTO) {
    const formGroup = this.createFormGroup(selection);

    this.addFormGroupValidations(formGroup, selection);

    this.formArray.push(formGroup);
    setTimeout(() => {
      this.selection = null; // ensures selection gets reset after every addition
    });
  }

  removeFlexField(index: number) {
    this.formArray.removeAt(index);
  }
}
