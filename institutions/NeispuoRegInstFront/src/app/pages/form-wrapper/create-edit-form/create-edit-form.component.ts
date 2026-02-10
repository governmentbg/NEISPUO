import {
  AfterContentChecked,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from "@angular/core";
import { FormGroup, FormBuilder, FormArray } from "@angular/forms";
import { FormDataService } from "../../../services/form-data.service";
import { Form } from "../../../models/form.interface";
import { Subsection } from "../../../models/subsection.interface";
import { Section } from "../../../models/section.interface";
import { Mode } from "../../../enums/mode.enum";
import { Influence } from "../../../models/influence.interface";
import { Menu } from "../../../enums/menu.enum";
import { InfluenceType } from "../../../enums/influenceType.enum";
import { CustomFormControl } from "../../../models/custom-form-control.interface";
import { ActivatedRoute, Router } from "@angular/router";
import { FormTypeInt } from "../../../enums/formType.enum";
import { environment } from "../../../../environments/environment";
import { AuthService } from "src/app/auth/auth.service";
import { FieldType } from "../../../enums/fieldType.enum";

@Component({
  exportAs: "createEditForm",
  selector: "app-create-edit-form",
  templateUrl: "./create-edit-form.component.html",
  styleUrls: ["./create-edit-form.component.scss"]
})
export class CreateEditFormComponent implements OnInit, OnChanges, AfterContentChecked {
  @Input() form: Form;
  @Input() validate: boolean;
  @Input() isNew: boolean = false;
  @Input() isHistoryActive: boolean = false;
  @Input() mode: Mode;
  @Input() historyPosition: number;
  @Input() instid: number | string;
  @Input() instKind: number | string;

  @Output() submit: EventEmitter<any> = new EventEmitter<any>();

  formGroup: FormGroup;
  statusFormControl;
  addRecordActive: boolean = false;

  private newSubsectionsCounter = 1;

  get value() {
    if (!this.formGroup) return {};
    return this.formGroup.value;
  }

  get modes() {
    return Mode;
  }

  get menu() {
    return Menu;
  }

  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private formDataService: FormDataService,
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit() {
    if (!this.mode) {
      this.mode = Mode.View;
    }
  }

  ngAfterContentChecked() {
    this.cdr.detectChanges();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.form) {
      this.formGroup = this.createFormGroup();
    }

    if (changes.validate && changes.validate.currentValue) {
      this.formDataService.validateAllFormFields(this.formGroup);
    }
  }

  private createFormGroup() {
    const group = this.fb.group({});
    if (this.form) {
      this.form.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          this.formDataService.createSubsectionGroup(subsection, group, section.hidden);
        });
      });
    }

    this.statusFormControl = group.get(this.form.statusFieldName);
    return group;
  }

  getFormGroup(subsection: Subsection) {
    return this.formDataService.getFormGroup(subsection, this.formGroup);
  }

  statusFinal() {
    return (
      this.statusFormControl.value &&
      ((this.statusFormControl.value.code >= 4 && this.statusFormControl.value.code <= 6) ||
        (this.statusFormControl.value >= 4 && this.statusFormControl.value <= 6))
    );
  }

  addSubsection(subsection: Subsection, section: Section) {
    this.adjustCounters(section, subsection.name, null);

    const position = subsection.subsectionRecordsCount - 1;
    let newSubsection;
    this.formDataService.duplicateSubsection(subsection, "new" + this.newSubsectionsCounter, {}, position + 1, this.instid).then(res => {
      newSubsection = res;
      newSubsection.position = position;

      this.formDataService.createSubsectionGroup(newSubsection, this.formGroup, section.hidden);
      this.newSubsectionsCounter++;

      section.subsections.push(newSubsection);
      section.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
    });
  }

  removeSubsection(subsection: Subsection, section: Section) {
    const arr = this.formGroup.get(subsection.name);
    const index = (<FormArray>arr).controls.findIndex((control: FormGroup) => Object.keys(control.controls)[0] == subsection.id);
    (<FormArray>arr).removeAt(index);

    section.subsections = section.subsections.filter(ssection => ssection.id !== subsection.id || ssection.name !== subsection.name);

    this.adjustCounters(section, subsection.name, subsection.position);

    if (subsection.subsectionRecordsCount === 1) {
      this.addSubsection(subsection, section);
    }
  }

  onSectionChanged(event: { subsection: Subsection; action: string }, section: Section) {
    if (event.action === "removed") {
      this.removeSubsection(event.subsection, section);
    } else if (event.action === "added") {
      this.addSubsection(event.subsection, section);
    }
  }

  onInfluenceChanged(event: { value: string | number; influence: Influence; filterValue: string | number }) {
    if (event.influence.sectionName) {
      const section = this.form.sections.find(section => section.name === event.influence.sectionName);
      const includes = event.influence.condition.includes(event.value);

      if (section && (event.influence.type === InfluenceType.Hide || event.influence.type === InfluenceType.HideClear)) {
        section.hidden = this.formDataService.isHidden(
          section.hiddenBy,
          event.value,
          event.influence.condition,
          true,
          this.formGroup,
          null,
          null
        );
        const clear = section.hidden && event.influence.type === InfluenceType.HideClear;
        section.subsections.forEach(sub => this.setFormControlsHidden(sub, includes, this.formGroup, section.hidden, clear));
      } else if (section && event.influence.type === InfluenceType.Render) {
        section.hidden = this.formDataService.isHidden(
          section.hiddenBy,
          event.value,
          event.influence.condition,
          false,
          this.formGroup,
          null,
          null
        );
        section.subsections.forEach(sub => this.setFormControlsHidden(sub, !includes, this.formGroup, section.hidden, false));
      } else if (section && event.influence.type === InfluenceType.Disable) {
        section.subsections && section.subsections.forEach(sub => this.disableSubsectionFields(sub, includes, this.formGroup));
      }
    } else if (event.influence.subsectionName) {
      let flag = false;
      this.form.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          const includes = event.influence.condition.includes(event.value);

          if (event.influence.subsectionName === subsection.name) {
            this.manageSubsectionInfluence(subsection, event, includes, section.hidden);
            flag = true;
          }

          if (!flag && subsection.subsections) {
            const sub = subsection.subsections.find(s => s.name === event.influence.subsectionName);
            const group = this.getFormGroup(subsection);
            sub && this.manageSubsectionInfluence(sub, event, includes, section.hidden, group);
            sub && (flag = true);
          }
        });
      });
    } else if (event.influence.fieldName) {
      let [influencedField, ssection] = this.getInfluencedField(event.influence.fieldName);

      if (influencedField && ssection) {
        const formGroup = this.getFormGroup(ssection);
        const formControl = formGroup.get(event.influence.fieldName);
        const initialValue = formControl.value;
        this.formDataService.manageInfluencedField(
          event,
          this.instid,
          influencedField,
          formControl,
          this.instKind,
          formGroup,
          null,
          this.formGroup
        );

        if (event.influence.type === InfluenceType.SetValueOppositeCondition) {
          const value = !event.influence.condition.includes(event.value) ? event.influence.value : null;
          for (let influence of influencedField.influence) {
            this.onInfluenceChanged({ value, influence, filterValue: value });
          }
        }

        if (event.influence.type === InfluenceType.SetValue && event.influence.condition.includes(event.value)) {
          const value = event.influence.value;
          for (let influence of influencedField.influence) {
            this.onInfluenceChanged({ value, influence, filterValue: value });
          }
        }

        const currentValue = formControl.value;
        if (event.influence.type === InfluenceType.Options && initialValue !== currentValue) {
          this.loopInfluences(influencedField);
        }
      }
    }
  }

  navigate() {
    const parent = this.route.parent;
    const url = parent.snapshot.url.length > 0 ? parent.snapshot.url : parent.parent.snapshot.url;
    let queryParams = this.route.snapshot.queryParams;
    environment.production && (queryParams = this.formDataService.decodeParams(queryParams["q"] || ""));

    const type = queryParams.instType;
    const instid = queryParams.instid;
    const procID = queryParams.procID;
    const path = url[0].path;
    const form = url[1].path;
    const instKind = path == Menu.CreateProcedure && procID ? queryParams.instKind : null;

    let navigateParams: any = {
      instid,
      instKind,
      procID,
      sysuserid: this.authService.getSysUserId(),
      region: this.authService.getRegion()
    };
    environment.production && (navigateParams = this.formDataService.encodeParams(navigateParams));
    (path != Menu.CreateProcedure || !procID) &&
      (navigateParams = { sysuserid: this.authService.getSysUserId(), region: this.authService.getRegion() });

    const newPath = path == Menu.CreateProcedure && procID ? `/${Menu.Active}/${FormTypeInt[type - 1]}/${form}` : `/${this.menu.Home}`;
    this.router.navigate([newPath], { queryParams: navigateParams });
  }

  onSelectionChange(event: { fieldName: string; value: any }) {
    if (event.fieldName === this.form.statusFieldName) {
      const [field] = this.getInfluencedField(event.fieldName);

      this.form.isDraft = field?.options.find(option => option.code === event.value)?.isDraft;
    }
  }

  private manageSubsectionInfluence(
    sub: Subsection,
    event: { value: string | number; influence: Influence; filterValue: string | number },
    includes: boolean,
    sectionHidden,
    group: FormGroup = this.formGroup
  ) {
    if (sub && (event.influence.type === InfluenceType.Hide || event.influence.type === InfluenceType.HideClear)) {
      sub.hidden = this.formDataService.isHidden(sub.hiddenBy, event.value, event.influence.condition, true, group, this.formGroup, null);
      const clear = sub.hidden && event.influence.type === InfluenceType.HideClear;
      this.setFormControlsHidden(sub, includes, this.formGroup, sectionHidden, clear);
    } else if (sub && event.influence.type === InfluenceType.Render) {
      sub.hidden = this.formDataService.isHidden(sub.hiddenBy, event.value, event.influence.condition, false, group, this.formGroup, null);
      this.setFormControlsHidden(sub, !includes, this.formGroup, sectionHidden, false);
    } else if (sub && event.influence.type === InfluenceType.Disable) {
      this.disableSubsectionFields(sub, includes, this.formGroup);
    }
  }

  private disableSubsectionFields(subsection: Subsection, includes: boolean, fg: FormGroup) {
    const formGroup: FormGroup = this.formDataService.getFormGroup(subsection, fg);
    subsection.fields &&
      subsection.fields.forEach(fld => {
        const formControl = formGroup ? formGroup.get(fld.name) : null;
        formControl && (includes ? formControl.disable() : formControl.enable());
      });

    subsection.subsections && subsection.subsections.forEach(sub => this.disableSubsectionFields(sub, includes, formGroup));
  }

  private setFormControlsHidden(subsection: Subsection, includes: boolean, fg: FormGroup, sectionHidden: boolean, clear: boolean) {
    const formGroup: FormGroup = this.formDataService.getFormGroup(subsection, fg);
    subsection.fields &&
      subsection.fields.forEach(fld => {
        const formControl = formGroup ? formGroup.get(fld.name) : null;
        const initialValue = formControl?.value;
        formControl && ((formControl as CustomFormControl).hidden = includes || sectionHidden || subsection.hidden);
        const val = fld.defaultValue ? fld.defaultValue : null;
        (formControl as CustomFormControl)?.hidden && clear && formControl.setValue(val);
        formControl?.updateValueAndValidity();
        const currentValue = formControl?.value;
        initialValue !== currentValue && this.loopInfluences(fld);
      });

    subsection.subsections &&
      subsection.subsections.forEach(sub => this.setFormControlsHidden(sub, includes, formGroup, sectionHidden, clear));
  }

  private getInfluencedField(fieldName: string) {
    let influencedField;
    let ssection;
    this.form.sections.forEach(section => {
      !influencedField &&
        section.subsections.forEach(subsection => {
          !influencedField && (influencedField = subsection.fields.find(fld => fld.name === fieldName)) && (ssection = subsection);

          if (!influencedField && subsection.subsections && subsection.subsections.length > 0) {
            subsection.subsections.forEach(sub => {
              !influencedField && (influencedField = sub.fields.find(fld => fld.name === fieldName)) && (ssection = sub);
            });
          }
        });
    });

    return [influencedField, ssection];
  }

  private loopInfluences(influencedField) {
    if (influencedField.influence) {
      for (const influenceRecord of influencedField.influence) {
        influenceRecord.type === InfluenceType.Options &&
          this.onInfluenceChanged({ value: null, influence: { ...influenceRecord }, filterValue: null });
      }
    }
  }

  onSubmit(event: Event, isFinal: boolean) {
    event.preventDefault();
    event.stopPropagation();

    if (this.formGroup.valid) {
      //getRawValue includes disabled fields also
      let formValues = this.customGetRawValue();

      //transform array elements
      formValues = this.formDataService.transformFormValue(formValues, this.form);

      this.submit.emit({
        value: formValues,
        isFinal,
        sysuserid: this.authService.getSysUserId(),
        region: this.authService.getRegion()
      });
    } else {
      this.formDataService.validateAllFormFields(this.formGroup);
    }
  }

  hasNonHiddenField(sub: Subsection) {
    return this.formDataService.hasNonHiddenField(sub);
  }

  private customGetRawValue() {
    let formValues = {};

    for (let key in this.formGroup.controls) {
      if (this.formGroup.controls[key] instanceof FormArray) {
        formValues[key] = [];
        const arr = <FormArray>this.formGroup.controls[key];
        for (const control of arr.controls) {
          const rowKey = Object.keys((<FormGroup>control).controls)[0];
          const innerGroup = <FormGroup>(<FormGroup>control).controls[rowKey];

          const rowValues = {};
          for (let key in innerGroup.controls) {
            const rowControl = <CustomFormControl>innerGroup.controls[key];
            rowValues[key] = rowControl.code || rowControl.value;

            if (rowControl.type === FieldType.Checkbox && rowValues[key] === "да") {
              rowValues[key] = true;
            } else if (rowControl.type === FieldType.Checkbox && rowValues[key] === "не") {
              rowValues[key] = false;
            }
          }

          formValues[key].push({ [rowKey]: rowValues });
        }
      } else {
        formValues[key] = this.formGroup.controls[key].value;
      }
    }

    return formValues;
  }

  private transformFormValue(formValues) {
    for (let key in formValues) {
      if (formValues[key] && formValues[key] instanceof Date) {
        formValues[key] = this.convertDate(formValues[key]);
      } else if (formValues[key] && typeof formValues[key] === "object" && formValues[key]["_d"]) {
        formValues[key] = this.convertDate(formValues[key]["_d"]);
      }

      if (formValues[key] && typeof formValues[key] === "object" && formValues[key].length > 0) {
        formValues[key] = formValues[key]
          .map(elem => {
            const keys = Object.keys(elem);
            const nullObj = this.checkIfNullObj(elem[keys[0]]);

            !nullObj && this.transformDocSubsection(elem, key, keys[0]);

            if (!nullObj && keys[0].startsWith("new")) {
              elem = this.transformElement(elem, keys, "");
            } else if (!keys[0].startsWith("new")) {
              elem = this.transformElement(elem, keys, keys[0]);
            } else {
              elem = null;
            }

            return elem;
          })
          .filter(elem => elem);
      } else if (formValues[key] && typeof formValues[key] === "object" && Object.keys(formValues[key]).length > 0) {
        formValues[key] = formValues[key] && formValues[key].code !== undefined ? formValues[key].code : formValues[key];
      }
    }
    return formValues;
  }

  private convertDate(date: Date) {
    let month = "" + (date.getMonth() + 1);
    let day = "" + date.getDate();
    let year = date.getFullYear();

    if (month.length < 2) month = "0" + month;
    if (day.length < 2) day = "0" + day;

    return [year, month, day].join("-");
  }

  private transformElement(obj, keys, id) {
    const props = obj[keys[0]];
    typeof id === "number" && (id = +id);
    obj = { id };
    for (const innerKey in props) {
      const val = props[innerKey] && props[innerKey].code !== undefined ? props[innerKey].code : props[innerKey];
      obj[innerKey] = this.transformFormValue({ [innerKey]: val })[innerKey];
    }
    return obj;
  }

  private transformDocSubsection(elem, subsectionName, id) {
    let docSubsection: Subsection;
    for (const section of this.form.sections) {
      for (const subsection of section.subsections) {
        if (subsection.id == id && subsection.name == subsectionName) {
          docSubsection = subsection;
          break;
        } else {
          const sub = subsection.subsections.find(s => s.name == subsectionName && s.id == id);
          if (sub) {
            docSubsection = sub;
            break;
          }
        }
      }

      if (docSubsection) {
        break;
      }
    }

    if (docSubsection && docSubsection.canHaveFiles) {
      elem[id].fileId = docSubsection.fileId;
      elem[id].fileLabel = docSubsection.fileLabel;
    }
  }

  private checkIfNullObj(obj) {
    let isNullObj = true;
    for (const key in obj) {
      if (obj[key] && typeof obj[key] === "object" && obj[key].length > 0) {
        for (const elem of obj[key]) {
          const innerKey = Object.keys(elem).length > 0 ? Object.keys(elem)[0] : null;
          innerKey && (isNullObj = isNullObj && this.checkIfNullObj(elem[innerKey]));
        }
      } else if (obj[key] && typeof obj[key] === "object" && Object.keys(obj[key]).length > 0) {
        for (const innerKey in obj[key]) {
          isNullObj = isNullObj && !obj[key][innerKey];
        }
      } else {
        isNullObj = isNullObj && !obj[key];
      }
    }
    return isNullObj;
  }

  private adjustCounters(section: Section, subsectionName: string, position: number) {
    section.subsections.forEach(subsection => {
      if (subsection.name === subsectionName) {
        const currentCount = subsection.subsectionRecordsCount;
        subsection.subsectionRecordsCount = position === null ? currentCount + 1 : currentCount - 1;
        position !== null && subsection.position > position && subsection.position--;
      }
    });
  }
}
