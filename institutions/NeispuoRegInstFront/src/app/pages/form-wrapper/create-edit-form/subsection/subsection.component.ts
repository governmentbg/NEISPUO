import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormArray, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { InfluenceType } from "../../../../enums/influenceType.enum";
import { Mode } from "../../../../enums/mode.enum";
import { ValidatorType } from "../../../../enums/validatorType.enum";
import { CustomFormControl } from "../../../../models/custom-form-control.interface";
import { FieldConfig } from "../../../../models/field.interface";
import { Influence } from "../../../../models/influence.interface";
import { Subsection } from "../../../../models/subsection.interface";
import { FormDataService } from "../../../../services/form-data.service";
import { CustomValidators } from "../../../../shared/custom.validators/custom.validators";
import { ModalComponent } from "../../../../shared/modal/modal.component";
import { UploadComponent } from "../../../../shared/upload/upload.component";

@Component({
  selector: "app-subsection",
  templateUrl: "./subsection.component.html",
  styleUrls: ["./subsection.component.scss"]
})
export class SubsectionComponent implements OnInit {
  @Input() subsection: Subsection;
  @Input() formGroup: FormGroup;
  @Input() parentGroup: FormGroup;
  @Input() wholeFormGroup: FormGroup;
  @Input() mode: Mode;
  @Input() instid: string | number;
  @Input() instKind: string | number;

  @Output() sectionChanged: EventEmitter<{ subsection: Subsection; action: string }> = new EventEmitter<{
    subsection: Subsection;
    action: string;
  }>();

  @Output() influenceChanged: EventEmitter<{ value: string | number; influence: Influence }> = new EventEmitter<{
    value: string;
    influence: Influence;
  }>();

  @Output() selectionChange: EventEmitter<{ fieldName: string; value: any }> = new EventEmitter<{
    fieldName: string;
    value: any;
  }>();

  private newSubsectionsCounter = 1;

  constructor(private formDataService: FormDataService, private dialog: MatDialog) {}

  get modes() {
    return Mode;
  }

  ngOnInit() {
    if (!this.mode) {
      this.mode = Mode.View;
    }
  }

  onAddSubsection() {
    this.sectionChanged.emit({ subsection: this.subsection, action: "added" });
  }

  onRemoveSubsection() {
    this.sectionChanged.emit({ subsection: this.subsection, action: "removed" });
  }

  onSectionChanged(event: { subsection: Subsection; action: string }) {
    if (event.action === "removed") {
      this.subsection.subsections = this.subsection.subsections.filter(
        subsection => subsection.id !== event.subsection.id || event.subsection.name !== subsection.name
      );
      const arr = this.formGroup.get(event.subsection.name);
      const index = (<FormArray>arr).controls.findIndex((control: FormGroup) => Object.keys(control.controls)[0] == event.subsection.id);
      (<FormArray>arr).removeAt(index);

      this.adjustCounters(event.subsection.name, event.subsection.position);

      if (event.subsection.subsectionRecordsCount === 1) {
        this.addSubsection(event.subsection);
      }
    } else if (event.action === "added") {
      this.addSubsection(event.subsection);
    }
  }

  private addSubsection(subsection: Subsection) {
    this.adjustCounters(subsection.name, null);
    const position = subsection.subsectionRecordsCount - 1;
    let newSubsection;
    this.formDataService.duplicateSubsection(subsection, "new" + this.newSubsectionsCounter, {}, position + 1, this.instid).then(res => {
      newSubsection = res;
      newSubsection.position = position;

      this.formDataService.createSubsectionGroup(newSubsection, this.formGroup, false);

      this.newSubsectionsCounter++;

      this.subsection.subsections.push(newSubsection);
      this.subsection.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
    });
  }

  getFormGroup(subsection: Subsection) {
    return this.formDataService.getFormGroup(subsection, this.formGroup);
  }

  trackByName(index, field: FieldConfig) {
    return field.name ? field.name : index;
  }

  onValueChange(event: { value: string | number; influence: Influence; filterValue: string | number }) {
    if (event.influence) {
      if (event.influence.subsectionName) {
        let flag = false;

        this.subsection.subsections &&
          this.subsection.subsections.forEach(subsection => {
            if (event.influence.subsectionName === subsection.name) {
              const includes = event.influence.condition.includes(event.value);

              if (event.influence.type === InfluenceType.Hide || event.influence.type === InfluenceType.HideClear) {
                subsection.hidden = this.formDataService.isHidden(
                  subsection.hiddenBy,
                  event.value,
                  event.influence.condition,
                  true,
                  this.formGroup,
                  this.parentGroup,
                  this.wholeFormGroup
                );
                const clear = subsection.hidden && event.influence.type === InfluenceType.HideClear;

                this.setFormControlsHidden(subsection, includes, this.formGroup, clear);
              } else if (event.influence.type === InfluenceType.Render) {
                subsection.hidden = this.formDataService.isHidden(
                  subsection.hiddenBy,
                  event.value,
                  event.influence.condition,
                  false,
                  this.formGroup,
                  this.parentGroup,
                  this.wholeFormGroup
                );
                this.setFormControlsHidden(subsection, !includes, this.formGroup, false);
              } else if (event.influence.type === InfluenceType.Disable) {
                subsection.fields &&
                  subsection.fields.forEach(fld => {
                    const formControl = this.formGroup.get(fld.name);
                    formControl && (includes ? formControl.disable() : formControl.enable());
                  });
              }

              flag = true;
            }
          });

        const multiple = this.subsection.canDuplicate === false || this.subsection.canDuplicate === true;
        !flag && !multiple && this.influenceChanged.emit(event);
      } else if (event.influence.sectionName) {
        const multiple = this.subsection.canDuplicate === false || this.subsection.canDuplicate === true;
        !multiple && this.influenceChanged.emit(event);
      } else if (event.influence.fieldName) {
        let res = this.getAllInfluenced(event.influence.fieldName, this.subsection);

        if (!res.length) {
          (this.subsection.canDuplicate === undefined || this.subsection.canDuplicate === null) && this.influenceChanged.emit(event);
          return;
        }

        for (const [influencedField, ssection] of res) {
          const formGroup = ssection === this.subsection ? this.formGroup : this.getFormGroup(ssection);
          const parent = formGroup === this.formGroup ? this.parentGroup : this.formGroup;
          const formControl = formGroup.get(event.influence.fieldName);
          this.formDataService.manageInfluencedField(
            event,
            this.instid,
            influencedField,
            formControl,
            this.instKind,
            formGroup,
            parent,
            this.wholeFormGroup
          );

          if (event.influence.type === InfluenceType.SetValueOppositeCondition) {
            const value = !event.influence.condition.includes(event.value) ? event.influence.value : null;
            for (let influence of influencedField.influence) {
              this.onValueChange({ value, influence, filterValue: value });
            }
          }

          if (event.influence.type === InfluenceType.SetValue && event.influence.condition.includes(event.value)) {
            const value = event.influence.value;
            for (let influence of influencedField.influence) {
              this.onValueChange({ value, influence, filterValue: value });
            }
          }

          if (event.influence.type === InfluenceType.Options) {
            this.loopInfluences(influencedField);
          }
        }
      } else if (event.influence.fields) {
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
            event.value
          ) {
            influencedField.validations.push({
              name: ValidatorType.RequiredControl,
              message: event.influence.message,
              validation: "",
              validator: Validators[ValidatorType.Required]
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
            !event.value
          ) {
            let flag = false;
            let allNull = true;

            influencedField.requiredBy &&
              influencedField.requiredBy.forEach(fldName => {
                let inSubsection = this.subsection.fields.find(fld => fld.name === fldName);
                if (!inSubsection) {
                  this.subsection.subsections &&
                    this.subsection.subsections.forEach(subsection => {
                      !inSubsection && (inSubsection = subsection.fields.find(field => field.name === fieldName));
                    });
                }
                !inSubsection && (flag = true);
              });

            if (!flag) {
              influencedField.requiredBy &&
                influencedField.requiredBy.forEach(fldName => {
                  const formControl = this.formGroup.get(fldName);
                  allNull = formControl ? allNull && (!formControl.value || formControl.value.code === null) : false;
                });

              if (allNull) {
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
          } else if (!influencedField && (this.subsection.canDuplicate === undefined || this.subsection.canDuplicate === null)) {
            this.influenceChanged.emit(event);
          }
        }
      }
    }
  }

  onSelectionChange(event: { value: any; fieldName: string }) {
    this.selectionChange.emit(event);
  }

  private setFormControlsHidden(subsection: Subsection, includes: boolean, fg: FormGroup, clear: boolean) {
    const formGroup: FormGroup = this.formDataService.getFormGroup(subsection, fg);
    subsection.fields &&
      subsection.fields.forEach(fld => {
        const formControl = formGroup ? formGroup.get(fld.name) : null;
        formControl && ((formControl as CustomFormControl).hidden = includes || subsection.hidden);
        const val = fld.defaultValue ? fld.defaultValue : null;
        (formControl as CustomFormControl)?.hidden && clear && formControl.setValue(val);
        formControl?.updateValueAndValidity();
      });

    subsection.subsections && subsection.subsections.forEach(sub => this.setFormControlsHidden(sub, includes, formGroup, clear));
  }

  private loopInfluences(influencedField: FieldConfig) {
    if (influencedField.influence) {
      for (const influenceRecord of influencedField.influence) {
        influenceRecord.type === InfluenceType.Options &&
          this.onValueChange({ value: null, influence: { ...influenceRecord }, filterValue: null });
      }
    }
  }

  hasNonHiddenField(sub: Subsection) {
    return this.formDataService.hasNonHiddenField(sub);
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

  openUploadModal() {
    if (this.subsection.fileId) {
      const dRef = this.dialog.open(ModalComponent, {
        width: "45%",
        data: {
          message: "Качване на нов документ, ще замести предходния. Искате ли да продължите?",
          confirmBtnLbl: "Продължи",
          cancelBtnLbl: "Откажи"
        }
      });

      dRef.afterClosed().subscribe((res: boolean) => {
        if (res) {
          this.openHelperModal();
        }
      });
    } else {
      this.openHelperModal();
    }
  }

  private openHelperModal() {
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "29%",
      panelClass: "l-modal-custom"
    });

    dialogRef.afterClosed().subscribe((fileId: number) => {
      if (fileId) {
        this.subsection.fileId = fileId;
        this.subsection.fileLabel = `document_${this.instid}_${fileId}`;
      }
    });
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
}
