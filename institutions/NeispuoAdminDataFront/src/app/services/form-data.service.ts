import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { Validators } from "@angular/forms";
import { CustomValidators } from "../shared/custom.validators/custom.validators";
import { Validator } from "../models/validator.interface";
import { FieldType } from "../enums/fieldType.enum";
import { FieldConfig } from "../models/field.interface";
import { Subsection } from "../models/subsection.interface";
import { Table } from "../models/table.interface";
import { Section } from "../models/section.interface";
import { InfluenceType } from "../enums/influenceType.enum";
import { Form } from "../models/form.interface";
import { ValidatorType } from "../enums/validatorType.enum";
import { Mode, ModeInt } from "../enums/mode.enum";
import { MenuItem } from "../models/menu-item.interface";
import { AuthService } from "../auth/auth.service";
import { environment } from "src/environments/environment";

@Injectable()
export class FormDataService {
  public mainMenuData: MenuItem[];
  public formValues: any = {};
  public ipAddress;

  private operationType;
  private lastOpenedTable: string = null;

  private real = true;
  private resultType = null;

  constructor(private http: HttpClient, private authService: AuthService) {}

  submitForm(body) {
    body.data.audit = this.getLogParams();
    return this.http.post("/data/save", body);
  }

  private getLogParams() {
    return {
      userAgent: window.navigator.userAgent,
      remoteIpAddress: this.ipAddress,
      loginSessionId: this.authService.getSessionId()
    };
  }

  getData(formName: string, body) {
    return this.http.post(`/data/get/${formName}`, body);
  }

  getIpAddress() {
    return this.http.get(environment.ipUrl).pipe(map((res: any) => res.ip || res.IPv4));
  }

  getMainMenuData() {
    return this.real
      ? this.http.get(`/uimeta/get/admin-menu`)
      : this.http.get("assets/json/menu-data/institution-menu.json");
  }

  getMessages() {
    return this.http.get("/uimeta/get/messages");
  }

  setSectionAccordionData(section: Section, body, mode: Mode) {
    const req = this.real
      ? this.http.post(`/data/get/${section.accordion.dataName}`, body)
      : this.http.get(`assets/json/fake-data/${section.accordion.dataName}.json`);

    return req.pipe(
      switchMap(async value => {
        this.formValues = { ...this.formValues, ...value };
        await this.setSectionValues(section, value, body, mode);
      })
    );
  }

  setSubsectionAccordionData(subsection: Subsection, body) {
    const req = this.real
      ? this.http.post(`/data/get/${subsection.accordion.dataName}`, body)
      : this.http.get(`assets/json/fake-data/${subsection.accordion.dataName}.json`);

    return req.pipe(
      switchMap(async value => {
        this.formValues = { ...this.formValues, ...value };
        subsection.rendered = this.substituteRenderedParams(subsection.rendered, body);

        if (subsection.rendered !== false) {
          const operationType = this.operationType; //for eval
          const instType = 1; // TODO remove

          subsection.rendered && eval(subsection.rendered + "");

          if (subsection.label && subsection.label.includes("if")) {
            eval(subsection.label);
          }

          !subsection.fields && (subsection.fields = []);
          !subsection.subsections && (subsection.subsections = []);

          for (let field of subsection.fields) {
            field = await this.transformField(field, value[field.name], body);
          }

          for (const subsubsection of subsection.subsections) {
            await this.transformSubsection(subsubsection, subsection, value, null, body);
          }
        }
      })
    );
  }

  getFormData(
    formName: string,
    body,
    operationType,
    mode: Mode = Mode.View,
    changeFormValues: boolean = true
  ): Observable<any> {
    const reqForm = this.real
      ? this.http.get(`/uimeta/get/${formName}`)
      : this.http.get(`assets/json/${formName}.json`);

    // for eval
    this.operationType = operationType;
    const instType = 1; // TODO remove

    return reqForm.pipe(
      switchMap(async (form: Form) => {
        this.resultType = null;

        if (form.dataName && form.dataName.includes("if")) {
          eval(form.dataName);
        }

        let dataName = operationType == 1 || form.dataName !== undefined ? form.dataName : formName;

        const reqData = this.real
          ? this.http.post(`/data/get/${dataName}`, body)
          : this.http.get(`assets/json/fake-data/${dataName}.json`);

        const res: any = dataName ? await reqData.toPromise() : null;
        let value = res && res.length > 0 ? res[0] : res || {};

        await this.transformForm(form, value, body, mode, changeFormValues);

        return form;
      })
    );
  }

  private async transformForm(form: Form, value, body, mode: Mode, changeFormValues: boolean = true) {
    if (mode === Mode.View) {
      this.lastOpenedTable = this.authService.getTableName();
      this.authService.removeTableName();
    }

    changeFormValues && (this.formValues = value);
    await this.setInfluencedBy(form);

    const operationType = this.operationType; //for eval
    const instType = 1; // TODO remove

    if (form.procedureName && form.procedureName.includes("if")) {
      eval(form.procedureName);
    }

    for (let section of form.sections) {
      section.name &&
        value[section.name] &&
        typeof value[section.name] === "string" &&
        (section.label = value[section.name]);

      if (!section.subsections && !section.table) {
        section.labelOnly = true;
      }

      !section.subsections && (section.subsections = []);
      section.rendered = this.substituteRenderedParams(section.rendered, body);
      section.rendered && eval(section.rendered + "");

      if (section.label && section.label.includes("if")) {
        eval(section.label);
      }

      if (section.name === "InfoMessagesSectionNew" && this.resultType !== null && !this.resultType) {
        section.rendered = false;
      }

      if (section.rendered !== false) {
        if (section.accordion) {
          if (section.accordion.dataName && section.accordion.dataName.includes("if")) {
            let dataName = section.accordion.dataName;
            eval(dataName);
            section.accordion.dataName = dataName;
          }

          if (section.accordion && section.table && section.table.name === this.lastOpenedTable) {
            section.accordion.state = "opened";
          }

          if (section.accordion.dataName && section.accordion.state === "opened") {
            const reqData = this.real
              ? this.http.post(`/data/get/${section.accordion.dataName}`, body)
              : this.http.get(`assets/json/fake-data/${section.accordion.dataName}.json`);

            const res: any = await reqData.toPromise();
            value = res && res.length > 0 ? res[0] : res || {};
            section.dataLoaded = true;
            section.loading = false;
          } else if (section.accordion.state === "opened") {
            section.loading = false;
          }
        }

        const initialLength = section.subsections.length;

        for (let i = 0; i < initialLength; i++) {
          const subsection = section.subsections[i];
          subsection.rendered = this.substituteRenderedParams(subsection.rendered, body);
          subsection.rendered && eval(subsection.rendered + "");

          subsection.rendered !== false && (await this.transformSubsection(subsection, section, value, form, body));
        }

        section.subsections = section.subsections
          .filter(
            subsection => subsection.fields.length > 0 || subsection.subsections.length > 0 || subsection.labelOnly
          )
          .sort((subsection1, subsection2) => subsection1.order - subsection2.order);

        if (section.table) {
          const values = value && value[section.table.name] ? value[section.table.name] : [];
          await this.transformTable(section.table, values, body, mode);
        }

        let rendered = false;

        if (section.subsections && !rendered) {
          for (const sub of section.subsections) {
            if (sub.rendered !== false || sub.labelOnly) {
              rendered = true;
              break;
            }
          }
        }

        if (section.table && section.table.rendered !== false) {
          rendered = true;
        }

        section.rendered = rendered;
      }
    }

    // set influences after all values have been set
    await this.setInfluencedFieldsValues(form, body);

    form.sections = form.sections
      .filter(
        section =>
          section.subsections.length > 0 || (section.table && section.table.rendered !== false) || section.labelOnly
      )
      .sort((section1, section2) => section1.order - section2.order);

    this.lastOpenedTable = null;
    return form;
  }

  private async setInfluencedFieldsValues(form: Form, body) {
    if (form.sections) {
      for (const section of form.sections) {
        if (section.subsections && section.subsections.length) {
          for (const subsection of section.subsections) {
            await this.influenceHelper(subsection, section, form, body);
          }
        }
      }
    }
  }

  private async influenceHelper(subsection: Subsection, parent, form: Form, body) {
    if (subsection.fields && subsection.fields.length) {
      for (const field of subsection.fields) {
        await this.influence(field, subsection, null, body, form);
        field.influence && (await this.manageSubsectionInfluence(field, subsection, parent, form, body));
      }
    }

    if (subsection.subsections && subsection.subsections.length) {
      for (const subsubsection of subsection.subsections) {
        await this.influenceHelper(subsubsection, subsection, form, body);
      }
    }
  }

  async setSectionValues(section: Section, values, body, mode: Mode) {
    const operationType = this.operationType; //for eval
    const instType = 1; // TODO remove

    if (section.table) {
      const val = values && values[section.table.name] ? values[section.table.name] : [];
      await this.transformTable(section.table, val, body, mode);
    }

    const initialLength = section.subsections.length;

    for (let i = 0; i < initialLength; i++) {
      const subsection = section.subsections[i];
      subsection.rendered = this.substituteRenderedParams(subsection.rendered, body);
      subsection.rendered && eval(subsection.rendered + "");

      subsection.rendered !== false && (await this.transformSubsection(subsection, section, values, null, body));
    }
  }

  private async transformTable(table: Table, values, body, mode: Mode) {
    const operationType = this.operationType; //for eval
    const instType = 1; // TODO remove

    table.values = [];

    table && table.rendered && (table.rendered = this.substituteRenderedParams(table.rendered, body));
    table && table.rendered && eval(table.rendered + "");

    if (table && table.createNew && table.createNew.includes("if")) {
      table.createNew = this.substituteRenderedParams(table.createNew, body);
      eval(table.createNew + "");
    }

    if (table && table.canDeleteRow && (table.canDeleteRow + "").includes("if")) {
      table.canDeleteRow = this.substituteRenderedParams(table.canDeleteRow, body);
      eval(table.canDeleteRow + "");
    }

    if (table && typeof table.hasEditButton === "string" && (table.hasEditButton + "").includes("if")) {
      table.hasEditButton = this.substituteRenderedParams(table.hasEditButton, body);
      eval(table.hasEditButton + "");
    }

    let selectFlds = [];

    if (this.resultType !== null && !this.resultType) {
      table.createNew = null;
    }

    if (
      (mode === Mode.Edit || table.disabledViewMode) &&
      values &&
      values.length &&
      table.formName &&
      table.rendered !== false
    ) {
      const createFormReq = this.real
        ? this.http.get(`/uimeta/get/${table.formName}`)
        : this.http.get(`assets/json/${table.formName}.json`);
      const createForm = <Form>await createFormReq.toPromise();
      await this.transformForm(createForm, {}, body, mode, false);

      createForm.sections.forEach(section => {
        section.subsections &&
          section.subsections.forEach(subsection => {
            subsection.fields &&
              subsection.fields.forEach(fld => {
                if (
                  fld.type === FieldType.Searchselect ||
                  fld.type === FieldType.Select ||
                  fld.type === FieldType.Multiselect
                ) {
                  selectFlds.push(fld);
                }
              });
          });
      });
    }

    if (table.rendered !== false) {
      for (const field of table.fields) {
        if (field.label && field.label.includes("if")) {
          eval(field.label);
        }

        if (field.rendered) {
          field.rendered = this.substituteRenderedParams(field.rendered, body);
          eval(field.rendered + "");
        }

        const selectFld = selectFlds.find(fld => fld.name === field.name);
        selectFld && (field.options = selectFld.options);
        field.options &&
          field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
      }

      // adjust column widths if not equal to 100 and no buffer column (no width in json)
      let widthSum = 0;

      for (const fld of table.fields) {
        if (fld.width && fld.rendered !== false) {
          widthSum += fld.width;
        } else if (!fld.width) {
          widthSum = null;
          break;
        }
      }

      if (widthSum && widthSum !== 100) {
        const coef = 100 / widthSum;
        for (const fld of table.fields) {
          fld.width *= coef;
        }
      }

      for (const row of values) {
        const fields = [];

        for (const tableField of table.fields) {
          const field = { ...tableField };
          fields.push(field);
        }

        for (let field of fields) {
          const option =
            field.options && field.options.length && row[field.name]
              ? field.options.find(opt => opt.code === row[field.name])
              : null;
          const value = option ? option.label : row[field.name];
          field = await this.transformField(field, value, body);
          option && (field.code = option.code);

          if (field.disabled !== false) {
            if (field.value === true) {
              field.value = "да";
            } else if (field.value === false) {
              field.value = "не";
            }
          }
        }

        const additionalParams: any = {};

        table.additionalParams &&
          table.additionalParams.forEach(param => {
            additionalParams[param] = row[param];
          });

        table.values.push({
          id: row.id,
          fields,
          formName: row.formName,
          formDataId: row.formDataId,
          additionalParams
        });
      }
    }

    return table;
  }

  private async setOptions(optionsPath: string, body, field: FieldConfig) {
    if (optionsPath) {
      const options = await this.getDynamicNomenclature(optionsPath, body).toPromise();
      field.options = options;
      field.options &&
        field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
      const required =
        field.validations &&
        field.validations.find(
          validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
        );
      !required && field.options.length && field.options.unshift({ label: "", code: null });
    } else {
      field.options = [];
      field.value = null;
    }
  }

  getDynamicNomenclature(url: string, body: any = {}): Observable<any> {
    if (url.startsWith("assets")) {
      return this.http.get(url);
    } else {
      return this.http.post(url, body);
    }
  }

  getFieldValue(url: string, body: any): Observable<any> {
    return this.real ? this.http.post(url, body) : this.http.get(`assets/json/fake-data/permanentAddress.json`);
  }

  private buildValidators(validations: Validator[]) {
    if (!validations) return [];
    return validations.map(v => {
      if (Validators[v.name])
        if (v.validation !== undefined) v.validator = Validators[v.name](v.validation);
        else {
          v.validator = v.name === ValidatorType.Required ? CustomValidators.requiredControl : Validators[v.name];
          v.name === ValidatorType.Required && (v.name = ValidatorType.RequiredControl);
        }
      else if (v.validation !== undefined) v.validator = CustomValidators[v.name](v.validation);
      else v.validator = CustomValidators[v.name];
      return v;
    });
  }

  isValidDate(date) {
    return date.getTime() === date.getTime();
  }

  private async transformField(field: FieldConfig, value, body) {
    // eval strings use the following fields:
    const operationType = this.operationType;
    const instType = 1; // TODO remove

    value = typeof value === "string" ? value.trim() : value;
    if ((field.value + "").includes("if")) {
      eval(field.value + "");
    } else {
      field.value = value;
    }

    field.dbValue = field.value;

    if (!field.value && field.value !== 0 && field.value !== false && field.defaultValue !== undefined) {
      (field.defaultValue + "").includes("if") && eval(field.defaultValue + "");
      if (body) {
        for (const key in body) {
          if (field.defaultValue === key) {
            field.defaultValue = key !== "personalID" && !isNaN(body[key]) ? +body[key] : body[key];
          }
        }
      }

      field.value = typeof field.defaultValue === "string" ? field.defaultValue.trim() : field.defaultValue;
    }

    if (field.label && field.label.includes("if")) {
      eval(field.label);
    }

    field.rendered = this.substituteRenderedParams(field.rendered, body);
    field.rendered && eval(field.rendered + "");
    field.disabled && eval(field.disabled + "");
    field.flexible && eval(field.flexible + "");

    if (field.type === FieldType.Date && this.isValidDate(new Date(field.value)) && field.value) {
      field.value = new Date(field.value);
    }

    if (field.type === FieldType.Checkbox) {
      field.value = !!field.value;
    }

    field.optionsPath && !field.options && (field.options = []);
    field.validations = this.buildValidators(field.validations);

    if (
      field.optionsPath &&
      field.options.length === 0 &&
      (operationType !== ModeInt.view || typeof field.value === "number")
    ) {
      const options = await this.getDynamicNomenclature(field.optionsPath, body).toPromise();

      field.options = options;
      field.options &&
        field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));

      const required =
        field.validations &&
        field.validations.find(
          validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
        );
      !required && field.options.length && field.options.unshift({ label: "", code: null });
    }

    return field;
  }

  private async transformSubsection(subsection: Subsection, parent: Subsection | Section, value, form: Form, body) {
    const operationType = this.operationType; //for eval
    const instType = 1; // TODO remove

    subsection.rendered = this.substituteRenderedParams(subsection.rendered, body);
    subsection.rendered && eval(subsection.rendered + "");

    if (subsection.disabled) {
      subsection.disabled = this.substituteRenderedParams(subsection.disabled, body);
      eval(subsection.disabled + "");
    }

    subsection.name &&
      value[subsection.name] &&
      typeof value[subsection.name] === "string" &&
      (subsection.label = value[subsection.name]);

    if (subsection.label && subsection.label.includes("if")) {
      eval(subsection.label);
    }

    !subsection.fields && !subsection.subsections && (subsection.labelOnly = true);
    !subsection.fields && (subsection.fields = []);
    !subsection.subsections && (subsection.subsections = []);

    if (subsection.accordion) {
      if (subsection.accordion.dataName && subsection.accordion.dataName.includes("if")) {
        let dataName = subsection.accordion.dataName;
        eval(dataName);
        subsection.accordion.dataName = dataName;
      }

      if (subsection.accordion.dataName && subsection.accordion.state === "opened" && !subsection.dataLoaded) {
        const reqData = this.real
          ? this.http.post(`/data/get/${subsection.accordion.dataName}`, body)
          : this.http.get(`assets/json/fake-data/${subsection.accordion.dataName}.json`);

        const res: any = await reqData.toPromise();
        value = res && res.length > 0 ? res[0] : res || {};
        subsection.dataLoaded = true;
      }
    }

    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      for (let field of subsection.fields) {
        field = await this.transformField(field, value[field.name], body);
      }

      for (const subsubsection of subsection.subsections) {
        await this.transformSubsection(subsubsection, subsection, value, form, body);
      }
    } else {
      let valuesLength;
      value[subsection.name] && (valuesLength = value[subsection.name].length);
      const valArr = valuesLength > 0 ? value[subsection.name] : null;

      subsection.id = valuesLength > 0 ? valArr[0].id : "new0";
      subsection.position = 0;
      subsection.subsectionRecordsCount = valuesLength > 0 ? valuesLength : 1;

      for (let field of subsection.fields) {
        const fieldVal = valuesLength > 0 ? valArr[0][field.name] : null;
        field = await this.transformField(field, fieldVal, body);
      }

      for (const subsubsection of subsection.subsections) {
        const vals = value[subsection.name] && value[subsection.name].length > 0 ? value[subsection.name][0] : {};
        await this.transformSubsection(subsubsection, subsection, vals, form, body);
      }

      if (valuesLength > 0 && subsection.rendered !== false) {
        const additionalSubsections = await this.duplicate(subsection, value, valuesLength, body, form);
        parent.subsections = parent.subsections.concat(additionalSubsections);
      }
    }

    subsection.fields = subsection.fields
      .filter(field => !(!field.value && field.value !== 0 && field.flexible))
      .sort((field1, field2) => field1.order - field2.order);

    let rendered = subsection.rendered;

    if (subsection.rendered !== false) {
      rendered = false;

      for (const field of subsection.fields) {
        if (field.rendered !== false) {
          rendered = true;
          break;
        }
      }

      if (subsection.subsections && !rendered) {
        for (const sub of subsection.subsections) {
          if (sub.rendered !== false) {
            rendered = true;
            break;
          }
        }
      }
    }

    subsection.rendered = rendered;

    subsection.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
  }

  private async duplicate(subsection: Subsection, value: any, subsectionRecordsCount: number, body, form: Form) {
    let result = [];
    for (let i = 1; i < subsectionRecordsCount; i++) {
      const valI = value[subsection.name][i];
      const newSubsection = await this.duplicateSubsection(
        subsection,
        valI.id,
        valI,
        subsectionRecordsCount,
        body,
        form
      );

      newSubsection.position = i;
      result.push(newSubsection);
    }

    return result;
  }

  async duplicateSubsection(
    subsection: Subsection,
    id: string,
    valuesObj: any,
    subsectionRecordsCount: number,
    body,
    form: Form,
    copy: boolean = true,
    parent = null,
    onButton = false
  ) {
    const newSubsection: Subsection = copy ? this.deepCopyObj(subsection) : subsection;
    id && (newSubsection.id = id);

    this.adjustMultipleSubsections(newSubsection, valuesObj);

    for (let i = 0; i < newSubsection.subsections.length; i++) {
      let subsubsection = newSubsection.subsections[i];
      let values = {};
      let count, id;

      if (subsubsection.canDuplicate !== undefined && subsubsection.canDuplicate !== null) {
        valuesObj[subsubsection.name] && (count = valuesObj[subsubsection.name].length);
        values = count > subsubsection.position ? valuesObj[subsubsection.name][subsubsection.position] : {};
        id = values["id"] ? values["id"] : "new" + subsubsection.position;
      } else {
        values = valuesObj;
      }

      subsubsection = await this.duplicateSubsection(
        subsubsection,
        id,
        values,
        count,
        body,
        form,
        false,
        newSubsection,
        onButton
      );
    }

    //first set all values and then do influences
    for (const field of newSubsection.fields) {
      for (const key in body) {
        if (field.defaultValue === key) {
          field.defaultValue = !isNaN(body[key]) ? +body[key] : body[key];
        }
      }
      field.value = valuesObj[field.name] || valuesObj[field.name] === 0 ? valuesObj[field.name] : field.defaultValue;
      if (field.value && field.type === FieldType.Date && this.isValidDate(new Date(field.value))) {
        field.value = new Date(field.value);
      }

      if (field.influencedBy && field.influencedBy.length) {
        field.options = null;
      }

      if (
        onButton === false &&
        this.operationType === ModeInt.view &&
        (field.type === FieldType.Searchselect || field.type === FieldType.Select) &&
        field.value &&
        typeof field.value === "object" &&
        field.value.length > 0
      ) {
        field.options = field.value;
        field.options &&
          field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));

        field.value = field.value[0].code;
      }
    }

    if (!onButton) {
      for (const field of newSubsection.fields) {
        await this.influence(field, newSubsection, parent, body, form);
      }
    }

    newSubsection.subsectionRecordsCount = subsectionRecordsCount ? subsectionRecordsCount : 1;
    return newSubsection;
  }

  private async influence(field: FieldConfig, subsection: Subsection, parentSubsection: Subsection, body, form) {
    if (field.influence && (!field.flexible || field.value !== undefined)) {
      for (const influence of field.influence) {
        if (influence.condition && typeof influence.condition === "string") {
          const path = this.real ? `/uimeta/get/${influence.condition}` : `assets/json/${influence.condition}.json`;
          influence.condition = <any[]>await this.http.get(path).toPromise();
        }

        const allFields = this.getAllFields(influence.fieldName, subsection, parentSubsection, form);

        for (const [influencedField, ssection, parentSubsection] of allFields) {
          if (
            influencedField &&
            influence.type === InfluenceType.Options &&
            !influencedField.options &&
            (this.operationType !== ModeInt.view || typeof field.value === "number")
          ) {
            if (influencedField.influencedBy.length > 0 && field.name !== influencedField.influencedBy[0]) {
              continue;
            }

            const url = field.value ? influence.url : null;
            const filterValues = this.collectFilterValues(influencedField, field, ssection, parentSubsection, form);

            await this.setOptions(url, { ...body, ...filterValues }, influencedField);
          } else if (influence.type === InfluenceType.Require) {
            for (const fieldName of influence.fields) {
              const influencedField = this.getField(fieldName, subsection, form);
              if (influencedField) {
                !influencedField.requiredBy && (influencedField.requiredBy = []);
                influencedField.requiredBy.push({
                  fieldName: field.name,
                  condition: influence.condition,
                  notNull: influence.notNull
                });
                !influencedField.validations && (influencedField.validations = []);

                if (
                  (influence.condition && influence.condition.includes(field.value)) ||
                  (influence.notNull && (field.value || field.value === 0))
                ) {
                  !influencedField.validations.find(
                    validator =>
                      validator.name === ValidatorType.Required || validator.name === ValidatorType.RequiredControl
                  ) &&
                    influencedField.validations.push({
                      name: ValidatorType.RequiredControl,
                      message: influence.message,
                      validation: "",
                      validator: CustomValidators.requiredControl
                    });

                  influencedField.options &&
                    influencedField.options.length > 0 &&
                    (influencedField.options = influencedField.options.filter(option => option.code !== null));
                }
              }
            }
          } else if (influencedField && influence.type === InfluenceType.Disable) {
            if (influencedField.disabledBy.length > 0 && field.name !== influencedField.disabledBy[0].fieldName) {
              continue;
            }

            influencedField.disabled = this.isDisabled(
              influencedField.disabledBy,
              field.value,
              influence.condition,
              influence.notNull,
              ssection,
              parentSubsection,
              form
            );
          } else if (influencedField && influence.type === InfluenceType.DefaultValue) {
            const isMultiSelect = field.value && typeof field.value === "object" && field.value.length >= 0;
            let includesVal;

            if (influence.condition) {
              includesVal = isMultiSelect
                ? field.value.some(elem => influence.condition.includes(elem))
                : influence.condition.includes(field.value);
            } else if (influence.notNull) {
              includesVal = isMultiSelect
                ? field.value.some(elem => elem || elem === 0)
                : field.value || field.value === 0;
            }

            if (includesVal && influencedField.value === undefined) {
              influencedField.value = influence.defaultValue;
              influencedField.defaultValue = influence.defaultValue;
            }
          } else if (
            influencedField &&
            !influencedField.value &&
            influencedField.value !== 0 &&
            influence.type === InfluenceType.Fill &&
            (field.type === FieldType.Select ||
              field.type === FieldType.Searchselect ||
              field.type === FieldType.Multiselect) &&
            field.options &&
            field.value
          ) {
            if (
              influencedField.type === FieldType.Select ||
              influencedField.type === FieldType.Multiselect ||
              influencedField.type === FieldType.Searchselect
            ) {
              influencedField.value = field.value;
            } else {
              if (field.type !== FieldType.Multiselect) {
                const option = field.options.find(option => option.code === field.value);
                option && (influencedField.value = option.label);
              } else {
                let label = "";
                let option;

                for (const val of field.value) {
                  option = field.options.find(option => option.code === val);
                  option && (val || val === 0) && (label += option.label + "; ");
                }

                influencedField.value = label;
              }
            }
          } else if (
            influencedField &&
            influence.fieldName &&
            (influence.type === InfluenceType.Render || influence.type === InfluenceType.Hide) &&
            influencedField.rendered !== false
          ) {
            if (influencedField.hiddenBy.length > 0 && field.name !== influencedField.hiddenBy[0].fieldName) {
              continue;
            }

            const hide = influence.type === InfluenceType.Hide;
            influencedField.hidden = this.isHidden(
              influencedField.hiddenBy,
              field.value,
              influence.condition,
              influence.notNull,
              hide,
              ssection,
              parentSubsection,
              form
            );
          } else if (influence.type === InfluenceType.SetValue) {
            if (!influence.url) {
              influencedField.value = null;
            } else {
              this.getFieldValue(influence.url, { ...body, [field.name]: field.value }).subscribe((res: any) => {
                res && res.length && (res = res[0]);

                if (res[influencedField.name]) {
                  influencedField.value = res[influencedField.name];
                }
              });
            }
          }
        }
      }
    }
  }

  private collectFilterValues(
    influencedField: FieldConfig,
    field: FieldConfig,
    subsection: Subsection,
    parentSubsection: Subsection,
    form: Form
  ) {
    if (influencedField.influencedBy.length > 1) {
      const filterValues = {};

      influencedField.influencedBy.forEach(fieldName => {
        if (fieldName === field.name) {
          filterValues[fieldName] = field.value;
        } else {
          const val = this.findFldVal(fieldName, subsection, parentSubsection, form);
          val !== null && val !== undefined && (filterValues[fieldName] = val);
        }
      });

      return filterValues;
    } else {
      return { filterValue: field.value };
    }
  }

  getField(fieldName: string, ssection: Subsection, form: Form): FieldConfig {
    let field = ssection.fields.find(fld => fld.name == fieldName);

    if (!field && form && form.sections) {
      form.sections.forEach(section => {
        !field &&
          section.subsections &&
          section.subsections.forEach(subsection => {
            !field && subsection.fields && (field = subsection.fields.find(fld => fld.name === fieldName));

            if (!field && subsection.subsections && subsection.subsections.length > 0) {
              subsection.subsections.forEach(sub => {
                !field && (field = sub.fields.find(fld => fld.name === fieldName));
              });
            }
          });

        !field && section.table && (field = section.table.fields.find(fld => fld.name === fieldName));
      });
    }

    return field;
  }

  // get all fields with that name with their subsection and parent subsection
  getAllFields(
    fieldName: string,
    subsection: Subsection,
    parentSubsection: Subsection,
    form: Form
  ): [FieldConfig, Subsection, Subsection][] {
    if (!fieldName) {
      return [[null, null, null]];
    }

    let res: [FieldConfig, Subsection, Subsection][] = [];
    let field;
    const subsectionDuplicate = subsection
      ? subsection.canDuplicate === true || subsection.canDuplicate === false
      : false;
    const parentDuplicate = parentSubsection
      ? parentSubsection.canDuplicate === true || parentSubsection.canDuplicate === false
      : false;

    if (subsection && subsection.fields && subsectionDuplicate) {
      field = subsection.fields.find(fld => fld.name === fieldName);

      field && res.push([field, subsection, parentSubsection]);
    }

    if (!res.length && !subsectionDuplicate && parentSubsection) {
      field = parentSubsection.fields.find(fld => fld.name === fieldName);
      field && res.push([field, parentSubsection, null]);

      if (!field && parentSubsection.subsections) {
        parentSubsection.subsections.forEach(sub => {
          field = null;
          if (sub.fields) {
            field = sub.fields.find(fld => fld.name === fieldName);
            field && res.push([field, sub, parentSubsection]);
          }
        });
      }
    }

    if (!res.length && !subsectionDuplicate && !parentDuplicate && form && form.sections) {
      for (const section of form.sections) {
        if (section.subsections && section.subsections.length) {
          for (const ssection of section.subsections) {
            field = null;
            ssection.fields && (field = ssection.fields.find(fld => fld.name === fieldName));
            field && res.push([field, ssection, null]);

            if (ssection.subsections) {
              ssection.subsections.forEach(sub => {
                field = null;
                sub.fields && (field = sub.fields.find(fld => fld.name === fieldName));
                field && res.push([field, sub, ssection]);
              });
            }
          }
        }

        if (section.table && section.table.fields) {
          field = section.table.fields.find(fld => fld.name === fieldName);

          field && res.push([field, null, null]);
        }
      }
    }

    return res;
  }

  private getSubsection(subsectionName: string, form: Form) {
    for (const section of form.sections) {
      if (section.subsections) {
        for (const subsection of section.subsections) {
          if (subsection.name === subsectionName) {
            return subsection;
          } else if (subsection.subsections) {
            for (const subsubsection of subsection.subsections) {
              if (subsubsection.name === subsectionName) {
                return subsubsection;
              }
            }
          }
        }
      }
    }
  }

  private getSection(sectionName: string, form: Form) {
    for (const section of form.sections) {
      if (section.name === sectionName) {
        return section;
      }
    }
  }

  private getTable(tableName: string, form: Form) {
    for (const section of form.sections) {
      if (section.table && section.table.name === tableName) {
        return section.table;
      }
    }
  }

  private async setInfluencedBy(form: Form) {
    if (form.sections) {
      for (let section of form.sections) {
        if (section.subsections) {
          for (let subsection of section.subsections) {
            if (subsection.fields) {
              for (let fld of subsection.fields) {
                await this.setInfluence(fld, subsection, form);
              }
            }

            if (subsection.subsections) {
              for (let subsubsection of subsection.subsections) {
                if (subsubsection.fields) {
                  for (let fld of subsubsection.fields) {
                    await this.setInfluence(fld, subsubsection, form);
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  private async setInfluence(field: FieldConfig, subsection: Subsection, form: Form) {
    if (field.influence) {
      for (let influence of field.influence) {
        if (influence.condition && typeof influence.condition === "string") {
          const path = this.real ? `/uimeta/get/${influence.condition}` : `assets/json/${influence.condition}.json`;
          influence.condition = <any[]>await this.http.get(path).toPromise();
        }

        if (influence.type === InfluenceType.Options) {
          const influencedField = this.getField(influence.fieldName, subsection, form);
          !influencedField.influencedBy && (influencedField.influencedBy = []);
          influencedField.influencedBy.push(field.name);
        } else if (
          influence.fieldName &&
          [InfluenceType.Disable, InfluenceType.Hide, InfluenceType.Render].includes(influence.type)
        ) {
          const influencedField = this.getField(influence.fieldName, subsection, form);
          this.setInfluenceHelper(influence.type, influencedField, field.name, influence.condition, influence.notNull);
        } else if (influence.subsectionName) {
          const subsection = this.getSubsection(influence.subsectionName, form);
          this.setInfluenceHelper(influence.type, subsection, field.name, influence.condition, influence.notNull);
        } else if (influence.sectionName) {
          const section = this.getSection(influence.sectionName, form);
          this.setInfluenceHelper(influence.type, section, field.name, influence.condition, influence.notNull);
        }
      }
    }
  }

  private setInfluenceHelper(
    influenceType: InfluenceType,
    object: FieldConfig | Subsection | Section,
    fieldName: string,
    condition: (string | number)[],
    notNull: boolean
  ) {
    if (influenceType === InfluenceType.Hide || influenceType === InfluenceType.Render) {
      const hide = influenceType === InfluenceType.Hide;
      !object.hiddenBy && (object.hiddenBy = []);
      object.hiddenBy.push({ fieldName, condition, hide, notNull });
    } else if (influenceType === InfluenceType.Disable) {
      !object.disabledBy && (object.disabledBy = []);
      object.disabledBy.push({ fieldName, condition, notNull });
    }
  }

  private async manageSubsectionInfluence(
    field: FieldConfig,
    subsection: Subsection,
    subParent: Subsection | Section,
    form: Form,
    body
  ) {
    for (const influence of field.influence) {
      if (influence.subsectionName) {
        const influencedSubsections = this.getAllInfluencedSubsections(
          influence.subsectionName,
          subsection,
          parent,
          form
        );

        for (const [sub, parent] of influencedSubsections) {
          if (sub && sub.rendered !== false) {
            if (influence.type === InfluenceType.Hide || influence.type === InfluenceType.Render) {
              const hide = influence.type === InfluenceType.Hide;
              sub.hidden = this.isHidden(
                sub.hiddenBy,
                field.value,
                influence.condition,
                influence.notNull,
                hide,
                sub,
                parent,
                form
              );
            } else if (influence.type === InfluenceType.Disable) {
              sub.disabled = this.isDisabled(
                sub.disabledBy,
                field.value,
                influence.condition,
                influence.notNull,
                sub,
                parent,
                form
              );
            }
          }
        }
      } else if (influence.sectionName) {
        let section = influence.sectionName === subParent.name ? subParent : null;
        !section && form && (section = form.sections.find(s => s.name === influence.sectionName));

        if (section && section.rendered !== false) {
          if (influence.type === InfluenceType.Hide || influence.type === InfluenceType.Render) {
            const hide = influence.type === InfluenceType.Hide;
            section.hidden = this.isHidden(
              section.hiddenBy,
              field.value,
              influence.condition,
              influence.notNull,
              hide,
              null,
              null,
              form
            );
          } else if (influence.type === InfluenceType.Disable) {
            section.disabled = this.isDisabled(
              section.disabledBy,
              field.value,
              influence.condition,
              influence.notNull,
              null,
              null,
              form
            );
          }
        }
      }
    }
  }

  private getAllInfluencedSubsections(
    subsectionName: string,
    subsection: Subsection,
    parent: Subsection | Section,
    form: Form
  ) {
    if (subsectionName === subsection.name) {
      return [subsection, parent];
    }

    const res = [];
    if (
      (subsection.canDuplicate === true || subsection.canDuplicate === false) &&
      subsection.subsections &&
      subsection.subsections.length
    ) {
      for (const sub of subsection.subsections) {
        if (sub.name === subsectionName) {
          res.push([sub, subsection]);
        }
      }
      return res;
    } else {
      for (const section of form.sections) {
        for (const sub of section.subsections) {
          if (sub.name === subsectionName) {
            res.push([sub, null]);
          }

          if (sub.subsections) {
            for (const subsub of sub.subsections) {
              if (subsub.name === subsectionName) {
                res.push([subsub, sub]);
              }
            }
          }
        }
      }

      return res;
    }
  }

  private isHidden(
    influencers: { fieldName: string; condition: (string | number)[]; hide: boolean; notNull: boolean }[],
    value,
    condition: (string | number)[],
    notNull: boolean,
    hide: boolean,
    subsection: Subsection,
    parentSubsection: Subsection,
    form: Form
  ) {
    if (influencers && influencers.length > 1) {
      let res = false;

      for (const influencer of influencers) {
        let val = this.findFldVal(influencer.fieldName, subsection, parentSubsection, form);
        val === undefined && (val = null);

        const isMultiSelect = val && typeof val === "object" && val.length >= 0;
        let includesVal;

        if (influencer.condition) {
          includesVal = isMultiSelect
            ? val.some(elem => influencer.condition.includes(elem))
            : influencer.condition.includes(val);
        } else if (influencer.notNull) {
          includesVal = isMultiSelect ? val.some(elem => elem || elem === 0) : val || val === 0;
        }

        res = influencer.hide ? res || includesVal : res || !includesVal;
      }

      return res;
    } else {
      const isMultiSelect = value && typeof value === "object" && value.length >= 0;
      let includes = false;

      if (condition) {
        includes = isMultiSelect ? value.some(elem => condition.includes(elem)) : condition.includes(value);
      } else if (notNull) {
        includes = isMultiSelect ? value.some(elem => elem || elem === 0) : value || value === 0;
      }

      return hide ? includes : !includes;
    }
  }

  private isDisabled(
    influencers: { fieldName: string; condition: (string | number)[]; notNull: boolean }[],
    value,
    condition: (string | number)[],
    notNull: boolean,
    subsection: Subsection,
    parentSubsection: Subsection,
    form: Form
  ) {
    if (influencers && influencers.length > 1) {
      let res = false;

      for (const influencer of influencers) {
        let val = this.findFldVal(influencer.fieldName, subsection, parentSubsection, form);
        val === undefined && (val = null);

        const isMultiSelect = val && typeof val === "object" && val.length >= 0;
        let includesVal;
        if (influencer.condition) {
          includesVal = isMultiSelect
            ? val.some(elem => influencer.condition.includes(elem))
            : influencer.condition.includes(val);
        } else if (influencer.notNull) {
          includesVal = isMultiSelect ? val.some(elem => elem || elem === 0) : val || val === 0;
        }

        res = res || includesVal;
      }

      return res;
    } else {
      const isMultiSelect = value && typeof value === "object" && value.length >= 0;

      if (condition) {
        return isMultiSelect ? value.some(elem => condition.includes(elem)) : condition.includes(value);
      } else if (notNull) {
        return isMultiSelect ? value.some(elem => elem || elem === 0) : value || value === 0;
      }

      return false;
    }
  }

  private findFldVal(fieldName: string, container, parent, form: Form) {
    let val = this.formValues[fieldName];
    const keys = Object.keys(this.formValues);

    if (val == undefined && !keys.includes(fieldName) && container) {
      let fld;

      if (container) {
        fld = this.findFldInContainer(container, fieldName);
      }

      if (!fld && parent) {
        fld = this.findFldInContainer(parent, fieldName);
      }

      if (!fld && form) {
        form.sections.forEach(section => !fld && (fld = this.findFldInContainer(section, fieldName)));
      }

      fld && (val = fld.value);
    }

    return val;
  }

  private findFldInContainer(container, fieldName: string) {
    let fld = container.fields ? container.fields.find(field => field.name === fieldName) : null;

    !fld &&
      container.subsections &&
      container.subsections.forEach(sub => {
        !fld && sub.fields && (fld = sub.fields.find(field => field.name === fieldName));

        !fld &&
          sub.subsections &&
          sub.subsections.find(subsubsection => {
            !fld && subsubsection.fields && (fld = subsubsection.fields.find(field => field.name === fieldName));
          });
      });

    return fld;
  }

  private deepCopyObj(object) {
    let copy = { ...object };
    for (let key in object) {
      if (
        object[key] &&
        !object[key].length &&
        Object.keys(object[key]).length > 0 &&
        typeof object[key] === "object"
      ) {
        copy[key] = this.deepCopyObj(object[key]);
      } else if (object[key] && object[key].length >= 0 && typeof object[key] === "object") {
        copy[key] = object[key].length > 0 ? this.deepCopyArr(object[key]) : [];
      }
    }
    return copy;
  }

  private deepCopyArr(array) {
    let copy = [...array];
    for (let i = 0; i < array.length; i++) {
      if (array[i] && !array[i].length && Object.keys(array[i]).length > 0 && typeof array[i] === "object") {
        copy[i] = this.deepCopyObj(array[i]);
      } else if (array[i] && array[i].length >= 0 && typeof array[i] == "object") {
        copy[i] = array[i].length > 0 ? this.deepCopyArr(array[i]) : [];
      }
    }
    return copy;
  }

  private adjustMultipleSubsections(subsection: Subsection, values) {
    const subsectionsCounters = {};
    subsection.subsections.forEach(subsubsection => {
      if (
        subsubsection.canDuplicate !== undefined &&
        subsubsection.canDuplicate !== null &&
        subsectionsCounters[subsubsection.name] === undefined
      ) {
        subsectionsCounters[subsubsection.name] = values[subsubsection.name] ? values[subsubsection.name].length : 1;
      }
    });

    const newSubsections: Subsection[] = subsection.subsections.filter(
      sub => sub.canDuplicate === undefined || sub.canDuplicate === null
    );

    for (const key in subsectionsCounters) {
      const toCopy = subsection.subsections.find(sub => sub.name == key);
      for (let i = 0; i < subsectionsCounters[key]; i++) {
        const sub = this.deepCopyObj(toCopy);
        sub.position = i;
        newSubsections.push(sub);
      }
    }

    newSubsections.sort((sub1, sub2) => sub1.order - sub2.order);
    subsection.subsections = newSubsections;
  }

  private substituteRenderedParams(renderedCondition, body) {
    if (typeof renderedCondition === "string" && body) {
      for (const key in body) {
        if (renderedCondition.includes(key)) {
          const regexp = new RegExp(`${key}`, "g");
          const keyVal = !isNaN(body[key]) ? +body[key] : body[key];
          renderedCondition = renderedCondition.replace(regexp, keyVal);
        }
      }
    }
    return renderedCondition;
  }
}
