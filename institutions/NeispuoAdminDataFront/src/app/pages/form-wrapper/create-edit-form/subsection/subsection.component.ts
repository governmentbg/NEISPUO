import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { FormArray, FormGroup } from "@angular/forms";
import { InfluenceType } from "../../../../enums/influenceType.enum";
import { Mode } from "../../../../enums/mode.enum";
import { ValidatorType } from "../../../../enums/validatorType.enum";
import { FieldConfig } from "../../../../models/field.interface";
import { Influence } from "../../../../models/influence.interface";
import { Subsection } from "../../../../models/subsection.interface";
import { FormDataService } from "../../../../services/form-data.service";
import { CustomValidators } from "../../../../shared/custom.validators/custom.validators";
import { environment } from "../../../../../environments/environment";
import { ActivatedRoute } from "@angular/router";
import { HelperService } from "src/app/services/helpers.service";
import { Section } from "src/app/models/section.interface";
import { SnackbarService } from "src/app/services/snackbar.service";
import { FieldType } from "src/app/enums/fieldType.enum";

@Component({
  selector: "app-subsection",
  templateUrl: "./subsection.component.html",
  styleUrls: ["./subsection.component.scss"]
})
export class SubsectionComponent implements OnInit, OnChanges {
  @Input() subsection: Subsection;
  @Input() section: Section;
  @Input() formGroup: FormGroup;
  @Input() mode: Mode;
  @Input() instid: string | number;
  @Input() parentGroup: FormGroup;
  @Input() wholeFormGroup: FormGroup;
  @Input() subsectionAdded: { name?: string; id?: string | number } = {};
  @Input() allClosed: boolean;

  @Output() sectionChanged: EventEmitter<{ subsection: Subsection; action: string }> = new EventEmitter<{
    subsection: Subsection;
    action: string;
  }>();

  @Output() influenceChanged: EventEmitter<{ value: string | number; influence: Influence }> = new EventEmitter<{
    value: string | number;
    influence: Influence;
  }>();

  @Output() procedurePerformed: EventEmitter<{
    procParams: Object;
    procName: string;
    canSign: boolean;
    searchByEik: boolean;
    requiredFields: string[];
    groupValues: Object;
    generateCertificate: boolean;
    button: FieldConfig;
    subsection: { name: string; id: number | string };
  }> = new EventEmitter<{
    procParams: Object;
    procName: string;
    canSign: boolean;
    searchByEik: boolean;
    requiredFields: string[];
    groupValues: Object;
    generateCertificate: boolean;
    button: FieldConfig;
    subsection: { name: string; id: number | string };
  }>();

  isLoading: boolean = false;
  checkActive: boolean = false;
  subAdded: { name: string; id: string | number };
  private newSubsectionsCounter = 1;

  constructor(
    private formDataService: FormDataService,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private snackbarService: SnackbarService
  ) {}

  get modes() {
    return Mode;
  }

  ngOnInit() {
    if (!this.mode) {
      this.mode = Mode.View;
    }

    if (this.subsection.firstRecordDisabled && this.subsection.position === 0) {
      this.subsection.fields && this.subsection.fields.forEach(fld => this.formGroup.get(fld.name).disable());

      this.subsection.subsections &&
        this.subsection.subsections.forEach(sub => {
          const fg = this.getFormGroup(sub);

          sub.fields && sub.fields.forEach(fld => fg.get(fld.name).disable());
        });
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (
      changes.subsectionAdded &&
      this.subsectionAdded &&
      this.subsection &&
      this.subsectionAdded.name === this.subsection.name &&
      this.subsectionAdded.id == this.subsection.id
    ) {
      this.subsection.fields &&
        this.subsection.fields.forEach(fld => {
          if (fld.influence) {
            this.influence(fld);
          }
        });
    }

    if (changes.allClosed && this.allClosed !== undefined) {
      this.changeAccordionsState();
    }
  }

  private changeAccordionsState() {
    if (
      this.subsection.accordion &&
      ((this.subsection.accordion.state === "closed" && !this.allClosed) ||
        (this.subsection.accordion.state === "opened" && this.allClosed))
    ) {
      this.changeSubsectionState();
    }

    if (this.subsection.subsections) {
      for (const sub of this.subsection.subsections) {
        if (
          sub.accordion &&
          ((sub.accordion.state === "closed" && !this.allClosed) ||
            (sub.accordion.state === "opened" && this.allClosed))
        ) {
          this.changeSubsectionState(sub);
        }
      }
    }
  }

  onAddSubsection() {
    this.sectionChanged.emit({ subsection: this.subsection, action: "added" });
  }

  onRemoveSubsection() {
    if (this.subsection.checkDelete) {
      this.isLoading = true;
      this.checkActive = true;
      const operationType = 13;

      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let formValues: any = { ...queryParams };
      formValues.id = this.subsection.id;
      formValues.forceOperation = 0;

      this.formDataService
        .submitForm({
          data: formValues,
          procedureName: this.subsection.checkDelete.procedure,
          operationType
        })
        .subscribe((res: any) => {
          res && (res = JSON.parse(res));
          if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.isLoading = false;
              this.checkActive = false;
              this.snackbarService.openErrorSnackbar(messages[index]);
            });
          } else {
            this.isLoading = false;
            this.checkActive = false;
            let fieldVal;
            const fld = this.subsection.fields.find(fld => fld.name === this.subsection.checkDelete.subsectionField);
            const val = this.formGroup.controls[fld.name].value;

            if (fld.type === FieldType.Select || FieldType.Searchselect) {
              fieldVal = fld.options.find(option => option.code == val).label;
            } else {
              fieldVal = val;
            }

            this.helperService.tableValuesChanged.next({
              fieldVal,
              multiselectField: this.subsection.checkDelete.multiselectField
            });

            this.sectionChanged.emit({ subsection: this.subsection, action: "removed" });
          }
        });
    } else {
      this.sectionChanged.emit({ subsection: this.subsection, action: "removed" });
    }
  }

  onSectionChanged(event: { subsection: Subsection; action: string }) {
    if (event.action === "removed") {
      this.removeInnerSubsection(event);
    } else if (event.action === "added") {
      this.addSubsection(event.subsection);
    }
  }

  private removeInnerSubsection(event: { subsection: Subsection; action: string }) {
    this.subsection.subsections = this.subsection.subsections.filter(
      subsection => subsection.id !== event.subsection.id || event.subsection.name !== subsection.name
    );
    const arr = this.formGroup.get(event.subsection.name);
    const index = (<FormArray>arr).controls.findIndex(
      (control: FormGroup) => Object.keys(control.controls)[0] == event.subsection.id
    );
    (<FormArray>arr).removeAt(index);

    this.adjustCounters(event.subsection.name, event.subsection.position);

    if (event.subsection.subsectionRecordsCount === 1) {
      this.addSubsection(event.subsection);
    }
  }

  private addSubsection(subsection: Subsection) {
    this.adjustCounters(subsection.name, null);
    const position = subsection.subsectionRecordsCount - 1;
    let newSubsection: Subsection;

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    const form = this.helperService.getFormValues(this.wholeFormGroup);

    this.formDataService
      .duplicateSubsection(
        subsection,
        "new" + this.newSubsectionsCounter,
        {},
        position + 1,
        { ...queryParams },
        form,
        true,
        null,
        true
      )
      .then((res: Subsection) => {
        newSubsection = res;
        newSubsection.position = position;

        this.helperService.createSubsectionGroup(newSubsection, this.formGroup, false, false);
        this.newSubsectionsCounter++;

        this.subAdded = { name: newSubsection.name, id: newSubsection.id };

        this.subsection.subsections.push(newSubsection);
        this.subsection.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
      });
  }

  private influence(field: FieldConfig) {
    let influence: Influence = null;

    for (const influenceRecord of field.influence) {
      if (influenceRecord.type === InfluenceType.Options || influenceRecord.type === InfluenceType.SetValue) {
        let url: string = field.value ? influenceRecord.url : null;
        influence = { fieldName: influenceRecord.fieldName, url, type: influenceRecord.type };
        this.onValueChange({ value: field.value, influence, filterValue: field.value });
      } else {
        influence = { ...influenceRecord };
        const option = field.options ? field.options.find(option => option.code == field.value) : null;
        const label = option ? option.label : "";
        this.onValueChange({ value: field.value, influence, filterValue: null, label });
      }
    }
  }

  getFormGroup(subsection: Subsection) {
    return this.helperService.getFormGroup(subsection, this.formGroup);
  }

  trackByName(index, field: FieldConfig) {
    return field.name ? field.name : index;
  }

  onValueChange(event: { value: string | number; influence: Influence; filterValue: string | number; label?: string }) {
    if (event.influence) {
      if (event.influence.subsectionName) {
        let flag = false;

        this.subsection.subsections &&
          this.subsection.subsections.forEach(subsection => {
            if (event.influence.subsectionName === subsection.name) {
              const type = event.influence.type;
              if (type === InfluenceType.Hide || type === InfluenceType.Render) {
                subsection.hidden = this.helperService.isHidden(
                  subsection.hiddenBy,
                  event.value,
                  event.influence.condition,
                  event.influence.notNull,
                  type === InfluenceType.Hide,
                  this.formGroup,
                  this.parentGroup,
                  this.wholeFormGroup
                );

                this.helperService.setFormControlsHidden(
                  subsection,
                  this.section.hidden,
                  this.formGroup,
                  this.parentGroup
                );
              } else if (type === InfluenceType.Disable) {
                subsection.disabled = this.helperService.isDisabled(
                  subsection.disabledBy,
                  event.value,
                  event.influence.condition,
                  event.influence.notNull,
                  this.formGroup,
                  this.parentGroup,
                  this.wholeFormGroup
                );

                this.helperService.setFormControlsDisabled(
                  subsection,
                  this.section.disabled,
                  this.formGroup,
                  this.parentGroup
                );
              }

              flag = true;
            }
          });

        const multiple = this.subsection.canDuplicate === false || this.subsection.canDuplicate === true;
        !flag && !multiple && this.influenceChanged.emit(event);
      } else if (event.influence.fieldName) {
        let res = this.getAllInfluenced(event.influence.fieldName, this.subsection);

        if (!res.length) {
          (this.subsection.canDuplicate === undefined || this.subsection.canDuplicate === null) &&
            this.influenceChanged.emit(event);
          return;
        }

        const queryParams = environment.production
          ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
          : this.route.snapshot.queryParams;

        for (const [influencedField, ssection] of res) {
          const formGroup = ssection === this.subsection ? this.formGroup : this.getFormGroup(ssection);
          const parent = formGroup === this.formGroup ? this.parentGroup : this.formGroup;
          const formControl = formGroup.get(event.influence.fieldName);

          if (!influencedField || !formControl) {
            (this.subsection.canDuplicate === undefined || this.subsection.canDuplicate === null) &&
              this.influenceChanged.emit(event);
            return;
          }

          this.helperService.manageInfluencedField(
            event,
            influencedField,
            formControl,
            queryParams,
            formGroup,
            parent,
            this.wholeFormGroup
          );

          this.loopInfluences(influencedField);
        }
      } else if (event.influence.fields) {
        this.manageRequireInfluence(event);
      }
    }
  }

  private onValueChangeHelper(subsection: Subsection) {
    if (this.subsection !== subsection) {
      return;
    }

    subsection.fields.forEach(fld => {
      if (fld.influence) {
        fld.influence.forEach(influence => {
          const filterValue = influence.type === InfluenceType.Options ? fld.defaultValue : null;
          this.onValueChange({
            value: fld.defaultValue !== undefined ? fld.defaultValue : null,
            filterValue: filterValue,
            influence,
            label: ""
          });
        });
      }
    });

    subsection.subsections && subsection.subsections.forEach(sub => this.onValueChangeHelper(sub));
  }

  private getAllInfluenced(fieldName: string, subsection: Subsection) {
    const res = [];

    let influencedField = subsection.fields.find(field => field.name === fieldName);

    if (influencedField) {
      res.push([influencedField, subsection]);
    } else if (subsection.subsections) {
      subsection.subsections.forEach(sub => {
        sub.fields && (influencedField = sub.fields.find(fld => fld.name === fieldName));
        influencedField && res.push([influencedField, sub]);
      });
    }

    return res;
  }

  private loopInfluences(influencedField: FieldConfig) {
    if (influencedField.influence) {
      for (const influenceRecord of influencedField.influence) {
        influenceRecord.type === InfluenceType.Options &&
          this.onValueChange({
            value: null,
            influence: { ...influenceRecord },
            filterValue: null,
            label: ""
          });
      }
    }
  }

  private manageRequireInfluence(event: {
    value: string | number;
    influence: Influence;
    filterValue: string | number;
  }) {
    for (let fieldName of event.influence.fields) {
      let influencedField = this.subsection.fields.find(field => field.name === fieldName);
      let ssection = this.subsection;

      if (!influencedField) {
        this.subsection.subsections &&
          this.subsection.subsections.forEach(subsection => {
            !influencedField &&
              (influencedField = subsection.fields.find(field => field.name === fieldName)) &&
              (ssection = subsection);
          });
      }

      if (
        influencedField &&
        ssection &&
        !influencedField.validations.find(
          validator => validator.name === ValidatorType.Required || validator.name === ValidatorType.RequiredControl
        ) &&
        ((event.influence.condition && event.influence.condition.includes(event.value)) ||
          (event.influence.notNull && (event.value || event.value === 0)))
      ) {
        influencedField.validations.push({
          name: ValidatorType.RequiredControl,
          message: event.influence.message,
          validation: "",
          validator: CustomValidators.requiredControl
        });

        const formControl = this.formGroup.get(fieldName);
        formControl.validator
          ? formControl.setValidators([CustomValidators.requiredControl, formControl.validator])
          : formControl.setValidators([CustomValidators.requiredControl]);
        formControl.updateValueAndValidity();

        //check if value! check on '-'
      } else if (
        influencedField &&
        ssection &&
        influencedField.validations.find(
          validator => validator.name === ValidatorType.Required || validator.name === ValidatorType.RequiredControl
        ) &&
        ((event.influence.condition && !event.influence.condition.includes(event.value)) ||
          (event.influence.notNull && !(event.value || event.value === 0)))
      ) {
        let flag = false;
        let noneIncludesValue = true;

        influencedField.requiredBy &&
          influencedField.requiredBy.forEach(influencer => {
            let inSubsection = this.subsection.fields.find(fld => fld.name === influencer.fieldName);
            if (!inSubsection) {
              this.subsection.subsections &&
                this.subsection.subsections.forEach(subsection => {
                  !inSubsection &&
                    (inSubsection = subsection.fields.find(field => field.name === influencer.fieldName));
                });
            }
            !inSubsection && (flag = true);
          });

        if (!flag) {
          influencedField.requiredBy &&
            influencedField.requiredBy.forEach(influencer => {
              const formControl = this.formGroup.get(influencer.fieldName);
              const includes = influencer.condition
                ? influencer.condition.includes(formControl.value)
                : influencer.notNull && (formControl.value || formControl.value === 0);

              formControl && (noneIncludesValue = noneIncludesValue && !includes);
            });

          if (noneIncludesValue) {
            influencedField.validations = influencedField.validations.filter(
              vld => vld.name !== ValidatorType.Required && vld.name !== ValidatorType.RequiredControl
            );

            const validators = [];
            influencedField.validations.forEach(validation => validators.push(validation.validator));

            const formControl = this.formGroup.get(influencedField.name);
            formControl.setValidators(validators);
            formControl.updateValueAndValidity();
          }
        } else {
          this.influenceChanged.emit(event);
        }
      } else if (
        !influencedField &&
        (this.subsection.canDuplicate === undefined || this.subsection.canDuplicate === null)
      ) {
        this.influenceChanged.emit(event);
        break;
      }
    }
  }

  isNull() {
    return this.checkIfFormNull(this.formGroup);
  }

  private checkIfFormNull(form: FormGroup) {
    let res = true;
    for (let key in form.controls) {
      const arr = <FormArray>form.controls[key];
      if (arr && arr.controls && arr.controls.length) {
        arr.controls.forEach(control => {
          const key = Object.keys((<FormGroup>control).controls)[0];
          const childRes = this.checkIfFormNull(<FormGroup>(<FormGroup>control).controls[key]);
          res = res && childRes;
        });
      } else if (form.controls[key].value && Object.keys(form.controls[key].value).length > 0) {
        for (const innerKey in form.controls[key].value) {
          res = res && !form.controls[key].value[innerKey];
        }
      } else {
        res = res && !form.controls[key].value;
      }
    }

    return res;
  }

  private adjustCounters(subsectionName: string, position: number) {
    this.subsection.subsections.forEach(subsection => {
      if (subsection.name === subsectionName) {
        const currentCount = subsection.subsectionRecordsCount;
        subsection.subsectionRecordsCount = position === null ? currentCount + 1 : currentCount - 1;
        position !== null && subsection.position > position && subsection.position--;
      }
    });
  }

  changeSubsectionState(subsection: Subsection = null) {
    const sub = subsection || this.subsection;

    if (sub.accordion && sub.rendered !== false && !sub.hidden) {
      sub.accordion.state = sub.accordion.state === "opened" ? "closed" : "opened";

      if (!sub.dataLoaded && sub.accordion.dataName && sub.accordion.state === "opened") {
        this.isLoading = true;
        sub.dataLoaded = true;

        const [body, operationType] = this.helperService.getRequestBody(
          environment.production,
          false,
          this.mode,
          this.route.snapshot.queryParams,
          this.route.snapshot.params || {}
        );

        this.formDataService.setSubsectionAccordionData(sub, body).subscribe(
          res => {
            this.setSubsectionControlsValues(sub, operationType);
            this.isLoading = false;
          },
          err => (this.isLoading = false)
        );
      }
    }
  }

  hasNonHiddenField(sub: Subsection) {
    return this.helperService.hasNonHiddenField(sub);
  }

  private setSubsectionControlsValues(subsection: Subsection, operationType: number) {
    const fg = this.getFormGroup(subsection);

    for (const field of subsection.fields) {
      field.name && fg.controls[field.name].setValue(field.value);
    }

    for (const sub of subsection.subsections) {
      this.setSubsectionControlsValues(sub, operationType);
    }
  }

  hasDeleteButton() {
    return (
      this.mode !== this.modes.View &&
      this.subsection.canDuplicate &&
      !(this.subsection.atLeastOneRequired && this.subsection.subsectionRecordsCount === 1) &&
      (!this.isNull() || this.subsection.position !== 0 || this.subsection.subsectionRecordsCount !== 1) &&
      !(this.subsection.firstRecordDisabled && this.subsection.position === 0)
    );
  }

  onPerformProcedure(event: {
    procName: string;
    procParams: Object;
    canSign?: boolean;
    searchByEik?: boolean;
    requiredFields?: string[];
    groupValues: Object;
    generateCertificate?: boolean;
    button?: FieldConfig;
  }) {
    const procParams = event.procParams ? event.procParams : {};
    this.procedurePerformed.emit({
      procParams,
      procName: event.procName,
      canSign: event.canSign,
      searchByEik: event.searchByEik,
      requiredFields: event.requiredFields,
      groupValues: event.groupValues,
      generateCertificate: event.generateCertificate,
      button: event.button,
      subsection: { name: this.subsection.name, id: this.subsection.id }
    });
  }
}
