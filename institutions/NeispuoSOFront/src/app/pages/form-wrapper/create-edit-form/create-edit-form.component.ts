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
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { environment } from "../../../../environments/environment";
import { HelperService } from "src/app/services/helpers.service";

import * as html2pdf from "html2pdf.js";
import { Tabs } from "src/app/enums/tabs.enum";
import { ValidatorType } from "src/app/enums/validatorType.enum";
import { CustomValidators } from "src/app/shared/custom.validators/custom.validators";
import { SnackbarService } from "src/app/services/snackbar.service";
import { MessagesService } from "src/app/services/messages.service";
import { FieldConfig, RegixData } from "src/app/models/field.interface";

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
  @Input() mode: Mode;
  @Input() instid: number | string;
  @Input() path: string;
  @Input() submissionPeriod: { isOpenPeriod: boolean; period: number; schoolYear: number };
  @Input() isLocked: boolean;

  @Output() refresh: EventEmitter<void> = new EventEmitter<void>();
  @Output() submit: EventEmitter<any> = new EventEmitter<any>();
  @Output() procedurePerformed: EventEmitter<{
    data: Object;
    procName: string;
    canSign: boolean;
    searchByEik: boolean;
    requiredFields: string[];
    groupValues: Object;
    generateCertificate: boolean;
    button: FieldConfig;
    subsection: { name: string; id: number | string };
  }> = new EventEmitter<{
    data: Object;
    procName: string;
    canSign: boolean;
    searchByEik: boolean;
    requiredFields: string[];
    groupValues: Object;
    generateCertificate: boolean;
    button: FieldConfig;
    subsection: { name: string; id: number | string };
  }>();

  @Output() performRegixProc: EventEmitter<{ regixData: RegixData; groupValues: any }> = new EventEmitter<{
    regixData: RegixData;
    groupValues: any;
  }>();

  subAdded: { name: string; id: string | number };
  formGroup: FormGroup;
  hasEditButton: boolean = false;
  isMonRuo: boolean = false;
  isHistory: boolean = false;
  isPreview: boolean = false;
  allClosed: boolean;
  addRecordActive: boolean = false;
  tableFilters: boolean[] = [];

  private newSubsectionsCounter = 1;
  private isCreateMode;

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
    private authService: AuthService,
    private helperService: HelperService,
    private snackbarService: SnackbarService,
    private msgService: MessagesService
  ) {}

  ngOnInit() {
    this.isMonRuo = this.authService.isMon() || this.authService.isRuo();
    this.tableFilters = Array(this.form.sections.length).fill(false);

    if (!this.mode) {
      this.mode = Mode.View;
    }

    if (this.form && this.form.changeData === false) {
      this.hasEditButton = false;
    } else if (this.form && this.form.sections) {
      for (const section of this.form.sections) {
        if (section.subsections && section.subsections.length > 0) {
          this.hasEditButton = true;
          break;
        }
      }
    }

    const route = this.route.snapshot;
    this.isPreview = this.router.url.includes("preview/");

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };
    this.isHistory = route.params.tab === Tabs.history || queryParams.year !== undefined;
    this.isCreateMode = !!(route.parent && route.parent.url && route.parent.url.length && route.parent.url[0].path === Menu.Create);
  }

  ngAfterContentChecked() {
    this.cdr.detectChanges();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.form) {
      this.formGroup = this.createFormGroup();
    }

    if (changes.validate && changes.validate.currentValue) {
      this.helperService.validateAllFormFields(this.formGroup);
    }
  }

  private createFormGroup() {
    const group = this.fb.group({});
    if (this.form) {
      this.form.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          this.helperService.createSubsectionGroup(subsection, group, section.hidden || section.rendered === false, section.disabled);
        });
      });
    }

    return group;
  }

  getFormGroup(subsection: Subsection, formGroup: FormGroup = this.formGroup) {
    return this.helperService.getFormGroup(subsection, formGroup);
  }

  addSubsection(subsection: Subsection, section: Section) {
    this.adjustCounters(section, subsection.name, null);

    const position = subsection.subsectionRecordsCount - 1;
    let newSubsection: Subsection;

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    const formValues = this.helperService.customGetRawValue(this.formGroup);
    this.helperService.setCurrentFormValues(this.form, formValues);

    this.formDataService
      .duplicateSubsection(
        subsection,
        "new" + this.newSubsectionsCounter,
        {},
        position + 1,
        { ...queryParams },
        this.form,
        this.isHistory,
        true,
        null,
        true
      )
      .then(res => {
        newSubsection = res;
        newSubsection.position = position;

        this.helperService.createSubsectionGroup(
          newSubsection,
          this.formGroup,
          section.hidden || section.rendered === false,
          section.disabled
        );
        this.newSubsectionsCounter++;

        this.subAdded = { name: newSubsection.name, id: newSubsection.id };

        section.subsections.push(newSubsection);
        section.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
      });
  }

  removeSubsection(subsection: Subsection, section: Section) {
    const arr = this.formGroup.get(subsection.name);
    const index = (<FormArray>arr).controls.findIndex((control: FormGroup) => Object.keys(control.controls)[0] == subsection.id);
    (<FormArray>arr).removeAt(index);

    const tableNames = [];
    for (let field of subsection.fields) {
      if (field.dependingTables) {
        tableNames.push(...field.dependingTables);
      }
    }

    this.onFilterTables(tableNames);

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

  async onInfluenceChanged(
    event: {
      value: string | number;
      influence: Influence;
      filterValue: string | number;
      label?: string;
      fieldName?: string;
    },
    subsection: Subsection = null,
    initialInfluenceOptions: boolean = null
  ) {
    if (event.influence.sectionName) {
      const section = this.form.sections.find(section => section.name === event.influence.sectionName);

      const type = event.influence.type;
      if (section && (type === InfluenceType.Hide || type === InfluenceType.Render || type === InfluenceType.HideClear)) {
        section.hidden = this.helperService.isHidden(
          section.hiddenBy,
          event.value,
          event.influence.condition,
          event.influence.notNull,
          type === InfluenceType.Hide || type === InfluenceType.HideClear,
          this.formGroup,
          null,
          null
        );

        const clear = section.hidden && type === InfluenceType.HideClear;
        section.hidden &&
          section.subsections.forEach(sub => {
            this.helperService.setFormControlsHidden(sub, section.hidden, this.formGroup, { value: {} }, clear);
            clear && this.helperService.subCleared.next({ name: sub.name, id: sub.id });
          });
      } else if (section && type === InfluenceType.Disable) {
        section.disabled = this.helperService.isDisabled(
          section.disabledBy,
          event.value,
          event.influence.condition,
          event.influence.notNull,
          this.formGroup,
          null,
          null
        );

        section.subsections &&
          section.subsections.forEach(sub =>
            this.helperService.setFormControlsDisabled(sub, section.disabled, this.formGroup, { value: {} })
          );
      }
    } else if (event.influence.subsectionName) {
      let flag = false;
      this.form.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          if (event.influence.subsectionName === subsection.name) {
            this.manageSubsectionInfluence(subsection, event, section.hidden, section.disabled);
            flag = true;
          }

          if (!flag && subsection.subsections) {
            const sub = subsection.subsections.find(s => s.name === event.influence.subsectionName);
            const group = this.getFormGroup(subsection);
            sub && this.manageSubsectionInfluence(sub, event, section.hidden, section.disabled, group);
            sub && (flag = true);
          }
        });
      });
    } else if (event.influence.createNewTableName) {
      const table = this.form.sections.find(section => section.table && section.table.name === event.influence.createNewTableName)?.table;

      table.createNewHidden = this.helperService.isHidden(
        table.createNewHiddenBy,
        event.value,
        event.influence.condition,
        event.influence.notNull,
        event.influence.type === InfluenceType.Hide || event.influence.type === InfluenceType.HideClear,
        this.formGroup,
        null,
        null
      );
    } else if (event.influence.fieldName) {
      let res = this.getAllInfluenced(event.influence.fieldName, subsection);

      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      for (const [influencedField, ssection, parentSubsection] of res) {
        let formGroup;

        if (parentSubsection && (parentSubsection.canDuplicate === true || parentSubsection.canDuplicate === false)) {
          const parentGroup = this.getFormGroup(parentSubsection);
          const isMultiple = ssection.canDuplicate === true || ssection.canDuplicate === false;
          formGroup = isMultiple ? this.getFormGroup(ssection, parentGroup) : parentGroup;
        } else {
          formGroup = this.getFormGroup(ssection);
        }

        const formControl = formGroup.get(event.influence.fieldName);

        await this.helperService.manageInfluencedField(
          event,
          influencedField,
          formControl,
          queryParams,
          formGroup,
          null,
          this.formGroup,
          this.isHistory
        );

        if (
          event.influence.type === InfluenceType.Options ||
          event.influence.type === InfluenceType.SetValue ||
          event.influence.type === InfluenceType.SetValueOppositeCondition
        ) {
          this.loopInfluences(
            influencedField,
            ssection,
            formControl.value,
            event.influence.fieldName,
            initialInfluenceOptions === null ? event.influence.type === InfluenceType.Options : initialInfluenceOptions
          );
        }
      }
    } else if (event.influence.fields) {
      let formGroup;

      for (const fieldName of event.influence.fields) {
        let res = this.getAllInfluenced(fieldName, subsection);
        for (const [influencedField, ssection, parentSubsection] of res) {
          if (parentSubsection && (parentSubsection.canDuplicate === true || parentSubsection.canDuplicate === false)) {
            const parentGroup = this.getFormGroup(parentSubsection);
            const isMultiple = ssection.canDuplicate === true || ssection.canDuplicate === false;
            formGroup = isMultiple ? this.getFormGroup(ssection, parentGroup) : parentGroup;
          } else {
            formGroup = this.getFormGroup(ssection);
          }

          const formControl = formGroup.get(fieldName);

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
            let noneIncludesValue = true;

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

              formControl.setValidators(validators);
              formControl.updateValueAndValidity();
            }
          }
        }
      }
    }
  }

  goBack() {
    let path = "/";
    let queryParams = {};

    if (this.authService.getPrevUrlData()) {
      [path, queryParams] = this.authService.getPrevUrlData();
    }

    this.router.navigate([path], { queryParams }).then(res => {
      if (res) {
        this.authService.removeUrl();
      }
    });
  }

  edit() {
    this.authService.setPrevUrlData(this.router.url.split("?")[0], this.route.snapshot.queryParams);

    const formName = this.route.snapshot.params["formName"];

    this.router.navigate(["/", Menu.Edit, formName], { queryParamsHandling: "preserve" });
  }

  onFilterTables(tableNames: string[]) {
    for (let name of tableNames) {
      const index = this.form.sections.findIndex(section => section.table?.name === name);
      this.tableFilters[index] = true;
    }
  }

  resetFilterFlag(index) {
    this.tableFilters[index] = false;
  }

  private manageSubsectionInfluence(
    sub: Subsection,
    event: { value: string | number; influence: Influence; filterValue: string | number },
    sectionHidden,
    sectionDisabled,
    group: FormGroup = this.formGroup
  ) {
    const type = event.influence.type;

    if (sub && (type === InfluenceType.Hide || type === InfluenceType.Render || type === InfluenceType.HideClear)) {
      sub.hidden = this.helperService.isHidden(
        sub.hiddenBy,
        event.value,
        event.influence.condition,
        event.influence.notNull,
        type === InfluenceType.Hide || type === InfluenceType.HideClear,
        group,
        this.formGroup,
        null
      );

      const clear = sub.hidden && type === InfluenceType.HideClear;
      this.helperService.setFormControlsHidden(sub, sectionHidden, group, this.formGroup, clear);
      clear && this.helperService.subCleared.next({ name: sub.name, id: sub.id });
    } else if (sub && type === InfluenceType.Disable) {
      sub.disabled = this.helperService.isDisabled(
        sub.disabledBy,
        event.value,
        event.influence.condition,
        event.influence.notNull,
        group,
        this.formGroup,
        null
      );

      this.helperService.setFormControlsDisabled(sub, sectionDisabled, group, this.formGroup);
    }
  }

  private loopInfluences(influencedField: FieldConfig, subsection: Subsection, value, fieldName: string, initialInfluenceOptions: boolean) {
    if (influencedField.influence) {
      for (const influenceRecord of influencedField.influence) {
        if (influenceRecord.type === InfluenceType.Options) {
          this.onInfluenceChanged(
            {
              value: null,
              influence: { ...influenceRecord },
              filterValue: initialInfluenceOptions ? null : value,
              label: ""
            },
            subsection,
            initialInfluenceOptions
          );
        } else if (influenceRecord.type === InfluenceType.SetValue || influenceRecord.type === InfluenceType.SetValueOppositeCondition) {
          this.onInfluenceChanged(
            {
              value,
              influence: { ...influenceRecord },
              filterValue: value,
              label: "",
              fieldName
            },
            subsection,
            initialInfluenceOptions
          );
        }
      }
    }
  }

  // if influences field in multiple section
  private getAllInfluenced(fieldName: string, subsection: Subsection) {
    const res = [];
    let influencedField;
    const subsectionDuplicate = subsection && (subsection.canDuplicate === true || subsection.canDuplicate === false);

    if (subsectionDuplicate) {
      influencedField = subsection.fields.find(fld => fld.name === fieldName);
    }

    if (influencedField) {
      res.push([influencedField, subsection, null]);
    } else if (!subsectionDuplicate) {
      this.form.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          influencedField = subsection.fields.find(fld => fld.name === fieldName);

          if (influencedField) {
            res.push([influencedField, subsection, null]);
          } else if (!influencedField && subsection.subsections && subsection.subsections.length > 0) {
            subsection.subsections.forEach(sub => {
              influencedField = sub.fields.find(fld => fld.name === fieldName);

              if (influencedField) {
                res.push([influencedField, sub, subsection]);
              }
            });
          }
        });

        if (section.table) {
          influencedField = section.table.fields.find(fld => fld.name === fieldName);

          if (influencedField) {
            res.push([influencedField, null, null]);
          }
        }
      });
    }

    return res;
  }

  onSubmit(event: Event, isFinal: boolean) {
    event.preventDefault();
    event.stopPropagation();

    if (!this.formGroup.invalid) {
      let formValues = this.helperService.customGetRawValue(this.formGroup);

      //transform array elements
      this.helperService.transformFormValue(formValues);
      this.helperService.transformReorderValues(formValues, this.form);

      this.submit.emit({ value: formValues, isFinal });
    } else {
      this.helperService.validateAllFormFields(this.formGroup);
      this.allClosed = true;
      this.changeAllAccordionsState();
      this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.invalidInput);
    }
  }

  changeSectionState(section: Section) {
    if (section.accordion && section.rendered !== false && !section.hidden) {
      section.accordion.state = section.accordion.state === "opened" ? "closed" : "opened";

      if (!section.dataLoaded && section.accordion.dataName && section.accordion.state === "opened") {
        section.dataLoaded = true;
        section.loading = true;

        const [body, operationType] = this.helperService.getRequestBody(
          environment.production,
          this.isCreateMode,
          this.mode,
          this.route.snapshot.queryParams,
          this.route.snapshot.params || {}
        );

        this.formDataService.setSectionAccordionData(section, body, this.isHistory, this.mode).subscribe(
          res => {
            for (const subsection of section.subsections) {
              this.setSubsectionControlsValues(section, subsection, this.formGroup, operationType);
            }

            section.loading = false;
          },
          err => (section.loading = false)
        );
      } else {
        section.loading = false;
      }
    }
  }

  hasAccordions() {
    for (const section of this.form.sections) {
      if (section.accordion && section.hidden !== true && section.rendered !== false) {
        return true;
      }

      for (const subsection of section.subsections) {
        if (subsection.accordion && subsection.hidden !== true && subsection.rendered !== false) {
          return true;
        }

        if (subsection.subsections) {
          for (let sub of subsection.subsections) {
            if (sub.accordion && sub.hidden !== true && sub.rendered !== false) {
              return true;
            }
          }
        }
      }
    }

    return false;
  }

  changeAllAccordionsState() {
    if (this.allClosed === undefined) {
      this.allClosed = false;
    } else {
      this.allClosed = !this.allClosed;
    }

    this.form.sections.forEach(section => {
      if (
        section.accordion &&
        ((section.accordion.state === "closed" && !this.allClosed) || (section.accordion.state === "opened" && this.allClosed))
      ) {
        this.changeSectionState(section);
      }
    });
  }

  private setSubsectionControlsValues(section: Section, subsection: Subsection, formGroup: FormGroup, operationType: number) {
    let fg = this.getFormGroup(subsection, formGroup);
    const multiple = subsection.canDuplicate === false || subsection.canDuplicate === true;

    if (multiple && fg === this.formGroup) {
      const arr = this.formGroup.get(subsection.name);
      const index = (<FormArray>arr).controls.findIndex((control: FormGroup) => Object.keys(control.controls)[0] == "new0");
      index >= 0 && (<FormArray>arr).removeAt(index);

      this.helperService.createSubsectionGroup(subsection, this.formGroup, section.hidden, section.disabled);
      fg = this.getFormGroup(subsection, formGroup);
    }

    for (const field of subsection.fields) {
      field.name && fg.controls[field.name].setValue(field.value);
    }

    for (const sub of subsection.subsections) {
      this.setSubsectionControlsValues(section, sub, fg, operationType);
    }
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

  hasNonHiddenField(sub: Subsection) {
    return this.helperService.hasNonHiddenField(sub);
  }

  generatePdf() {
    const params = this.route.snapshot.params || {};
    const element = document.getElementById("wrapper-card");

    const opt = {
      filename: `${params.formName}.pdf`,
      image: { type: "jpeg", quality: 1 },
      html2canvas: { scale: 2 },
      jsPDF: { unit: "in", format: "letter", orientation: "portrait" }
    };

    html2pdf().from(element).set(opt).save();
  }

  onProcedurePerformed(event: {
    procParams: Object;
    procName: string;
    canSign: boolean;
    searchByEik: boolean;
    requiredFields: string[];
    groupValues: Object;
    generateCertificate: boolean;
    button: FieldConfig;
    subsection: { name: string; id: number | string };
  }) {
    if (!this.formGroup.invalid) {
      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let formValues = this.helperService.customGetRawValue(this.formGroup);

      //transform array elements
      this.helperService.transformFormValue(formValues);

      this.procedurePerformed.emit({
        data: { ...queryParams, ...formValues, ...event.procParams },
        procName: event.procName,
        canSign: event.canSign,
        searchByEik: event.searchByEik,
        requiredFields: event.requiredFields,
        groupValues: event.groupValues,
        generateCertificate: event.generateCertificate,
        button: event.button,
        subsection: event.subsection
      });
    } else {
      this.helperService.validateAllFormFields(this.formGroup);
      this.allClosed = true;
      this.changeAllAccordionsState();
      this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.invalidInput);
    }
  }

  onPerformRegixProc(event) {
    this.performRegixProc.emit(event);
  }
}
