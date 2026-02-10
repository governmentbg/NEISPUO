import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { FieldType } from "src/app/enums/fieldType.enum";
import { FormTypeInt } from "src/app/enums/formType.enum";
import { Mode, ModeInt } from "src/app/enums/mode.enum";
import { Tabs } from "src/app/enums/tabs.enum";
import { FieldConfig } from "src/app/models/field.interface";
import { Form } from "src/app/models/form.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { Row } from "src/app/models/table.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../environments/environment";

@Component({
  selector: "app-new-table-record",
  templateUrl: "./new-table-record.component.html",
  styleUrls: ["./new-table-record.component.scss"]
})
export class NewTableRecordComponent implements OnInit, OnChanges {
  @Input() formName: string;
  @Input() createParams: Object = {};
  @Input() row: Row;
  @Input() paramName: string;
  @Input() parentMode: Mode;
  @Input() canSave: boolean;

  @Output() save: EventEmitter<{ fields: FieldConfig[]; procedureName: string; formValues: any }> = new EventEmitter<{
    fields: FieldConfig[];
    procedureName: string;
    formValues: any;
  }>();

  form: Form;
  formGroup: FormGroup;
  mode: Mode = Mode.Edit;
  isLoading = false;
  fields: FieldConfig[] = [];
  instid: string;

  get modes() {
    return Mode;
  }

  constructor(
    private fb: FormBuilder,
    private formDataService: FormDataService,
    private route: ActivatedRoute,
    private helperService: HelperService
  ) {}

  ngOnInit() {
    !this.canSave && (this.mode = Mode.View);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.formName && changes.row) {
      this.init();
    }
  }

  private init() {
    this.isLoading = true;
    this.fields = [];
    const type = this.route.snapshot.params["type"] || null;
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;
    this.instid = queryParams["instid"] || null;
    let body: any = { instid: this.instid, ...queryParams };
    const operationType = this.row === null || this.parentMode === Mode.Edit ? ModeInt.create : ModeInt.update;

    if (operationType === ModeInt.create) {
      body = { ...body, ...this.createParams };
    } else {
      body[this.paramName] = this.row.id;
      body = { ...body, ...this.row.additionalParams };
    }

    const isHistory = this.route.snapshot.params.tab === Tabs.history || queryParams.year !== undefined;

    this.formDataService
      .getFormData(this.formName, body, operationType, FormTypeInt[type] + 1, isHistory, this.parentMode, false)
      .subscribe((form: Form) => {
        this.parentMode === Mode.Edit && this.row && this.updateFormValues(form);
        this.initForm(form);
      });
  }

  private updateFormValues(form: Form) {
    form.sections &&
      form.sections.forEach(section => {
        section.subsections &&
          section.subsections.forEach(subsection => {
            subsection.fields &&
              subsection.fields.forEach(field => {
                const updateField = this.row.fields.find(fld => fld.name === field.name);
                if (
                  updateField &&
                  field.options &&
                  (field.type === FieldType.Select ||
                    field.type === FieldType.Searchselect ||
                    field.type === FieldType.Multiselect)
                ) {
                  const option = field.options.find(option => option.label === updateField.value);
                  field.value = option ? option.code : null;
                } else if (updateField && updateField.type == FieldType.Checkbox) {
                  field.value = updateField.value === "да" ? true : false;
                } else if (updateField) {
                  field.value = updateField.value;
                }
              });
          });
      });
  }

  private initForm(form: Form) {
    this.form = form;
    this.formGroup = this.fb.group({});

    this.form.sections &&
      this.form.sections.forEach(section => {
        section.subsections &&
          section.subsections.forEach(subsection => {
            subsection.fields &&
              subsection.fields.forEach(field => {
                this.fields.push(field);
              });
          });
      });

    this.fields.forEach(field => this.helperService.addControl(field, this.formGroup, false, false, false, false));
    this.isLoading = false;
  }

  onSave(save: boolean) {
    if (!save) {
      this.save.emit({ fields: [], procedureName: this.form.procedureName, formValues: null });
    } else {
      const fields = [];
      this.fields.forEach(field => {
        const newField = { ...field };
        newField.value = this.formGroup.get(newField.name).value;
        fields.push(newField);
      });

      this.save.emit({
        fields,
        procedureName: this.form.procedureName,
        formValues: this.helperService.transformFormValue(this.formGroup.value)
      });
    }
  }

  hasNonHiddenField(sub: Subsection) {
    return this.helperService.hasNonHiddenField(sub);
  }
}
