import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { forkJoin, Observable, Subject, Subscribable } from "rxjs";
import { switchMap } from "rxjs/operators";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Form } from "../models/form.interface";
import { CustomValidators } from "../shared/custom.validators/custom.validators";
import { Validator } from "../models/validator.interface";
import { FieldType } from "../enums/fieldType.enum";
import { FieldConfig } from "../models/field.interface";
import { Subsection } from "../models/subsection.interface";
import { Table } from "../models/table.interface";
import { Role } from "../enums/role.enum";
import { Section } from "../models/section.interface";
import { InfluenceType } from "../enums/influenceType.enum";

@Injectable()
export class FormDataService {
  public optionsChanged: Subject<string> = new Subject(); // name of field whose options changed

  private instType;
  private operationType;

  constructor(private http: HttpClient, private fb: FormBuilder) {}

  getFilters() {
    // return this.http.get(`assets/json/filters.json`).pipe(
    return this.http.get(`/uimeta/get/filters`).pipe(
      switchMap(async (fields: FieldConfig[]) => {
        for (const field of fields) {
          if (field.optionsPath) {
            const options = await this.getDynamicNomenclature(field.optionsPath).toPromise();
            field.options = options;
            field.type !== FieldType.SearchMultiSelect && field.options.unshift({ label: "", code: null });
          }
        }
        return fields;
      })
    );
  }

  getInstitutionHistory(id: number) {
    return this.http.post("/data/get/institution-history", { instid: id });
    // return this.http.get(`assets/json/history-${id}.json`);
  }

  getFilteredData(isActive, filters, keyWord: string, table: Table): Observable<Form> {
    const formName = "public-register";
    let body: any = {};
    filters && Object.keys(filters).length > 0 ? (body = { ...filters }) : (body = body);
    body.isRIActive = isActive;
    keyWord && (body.keyWord = keyWord);

    if (!table) {
      let requests: Subscribable<any>[] = [];

      requests.push(this.http.get(`/uimeta/get/${formName}`));
      requests.push(this.http.post(`/data/get/${formName}`, body));

      // requests.push(this.http.get(`assets/json/home-public.json`));
      // requests.push(this.http.get(`assets/json/fake-data/home-public.json`));

      return this.mapInstitutionRes(requests, null);
    } else {
      // return this.http.get(`assets/json/fake-data/home-public-2.json`).pipe(
      return this.http.post(`/data/get/${formName}`, body).pipe(
        switchMap(async (res: any) => {
          let value;

          if (res && res.length > 0) {
            value = res[0];
          } else if (res && typeof res === "object") {
            value = res;
          }

          const values = value && value[table.name] ? value[table.name] : [{ id: "new0" }];
          await this.transformTable(table, values);

          return { sections: [{ table }] };
        })
      );
    }
  }

  getFormData(formName: string, body, instType, isHistory: boolean): Observable<any> {
    let requests: Subscribable<any>[] = [];
    this.instType = instType;
    this.operationType = isHistory ? 14 : 10; //for eval

    requests.push(this.http.get(`/uimeta/get/${formName}`));
    requests.push(this.http.post(`/data/get/${formName}`, body));

    // requests.push(this.http.get(`assets/json/${formName}.json`));

    // if (body.instid && body.procID)
    //   requests.push(this.http.get(`assets/json/fake-data/${formName}-${body.instid}-${body.procID}.json`));
    // else if (body.instid) requests.push(this.http.get(`assets/json/fake-data/${formName}-${body.instid}.json`));

    return this.mapInstitutionRes(requests, body.instid);
  }

  mapInstitutionRes(requests: Subscribable<any>[], instid): Observable<Form> {
    return forkJoin(requests).pipe(
      switchMap(async (res: [Form, any]) => {
        let form: Form = res[0];
        let value = res.length > 1 && res[1].length > 0 ? res[1][0] : res[1] || {};
        const instKind = this.instType; //for eval
        const operationType = this.operationType; //for eval

        for (let section of form.sections) {
          !section.subsections && (section.subsections = []);
          section.rendered && eval(section.rendered + "");

          const initialLength = section.subsections.length;

          for (let i = 0; i < initialLength; i++) {
            const subsection = section.subsections[i];
            subsection.rendered && eval(subsection.rendered + "");

            await this.transformSubsection(subsection, section, value, instid, form);
          }

          section.subsections = section.subsections
            .filter(subsection => (subsection.fields.length > 0 || subsection.subsections.length > 0) && subsection.rendered !== false)
            .sort((subsection1, subsection2) => subsection1.order - subsection2.order);

          if (section.table) {
            const values = value && value[section.table.name] ? value[section.table.name] : [{ id: "new0" }];
            await this.transformTable(section.table, values);
          }
        }

        // set influences after all values have been set
        await this.setInfluencedFieldsValues(form, instid);

        form.sections = form.sections
          .filter(
            section => (section.subsections.length > 0 || (section.table && section.table.rendered !== false)) && section.rendered !== false
          )
          .sort((section1, section2) => section1.order - section2.order);

        return form;
      })
    );
  }

  private async setInfluencedFieldsValues(form: Form, instid) {
    if (form.sections) {
      for (const section of form.sections) {
        if (section.subsections && section.subsections.length) {
          for (const subsection of section.subsections) {
            await this.influenceHelper(subsection, section, form, instid);
          }
        }
      }
    }
  }

  private async influenceHelper(subsection: Subsection, parent, form: Form, instid: string) {
    if (subsection.fields && subsection.fields.length) {
      for (const field of subsection.fields) {
        await this.influence(
          subsection.canDuplicate === false || subsection.canDuplicate === true,
          field,
          subsection.fields,
          subsection,
          false,
          form
        );

        field.influence && (await this.manageSubsectionInfluence(field, subsection, parent, form));
      }
    }

    if (subsection.subsections && subsection.subsections.length) {
      for (const subsubsection of subsection.subsections) {
        await this.influenceHelper(subsubsection, subsection, form, instid);
      }
    }
  }

  private async transformTable(table: Table, values) {
    const instKind = this.instType; //for eval
    const operationType = this.operationType; //for eval
    table.values = [];

    table && table.rendered && eval(table.rendered + "");

    if (table.rendered !== false) {
      for (const tableField of table.fields) {
        if (tableField.optionsPath) {
          await this.setOptions(true, tableField.optionsPath, {}, tableField);
        }
      }

      for (const row of values) {
        const fields = [];

        for (const tableField of table.fields) {
          const field = { ...tableField };
          fields.push(field);
        }

        for (const field of fields) {
          await this.transformField(true, field, row[field.name], fields, row.procID, {});
        }

        table.values.push({
          id: row.id,
          fields,
          formName: row.formName,
          procID: row.procID,
          instKind: row.instKind,
          instid: row.instid
        });
      }
    }

    return table;
  }

  private async setOptions(multiple: boolean, optionsPath: string, body, field: FieldConfig, fields: FieldConfig[] = [], form = {}) {
    if (optionsPath) {
      const options = await this.getDynamicNomenclature(optionsPath, body).toPromise();
      field.options = options;
      const required = field.validations && field.validations.find(validation => validation.name === "required");
      if (!required && field.type !== FieldType.SearchMultiSelect) {
        field.options.unshift({ label: "", code: null });
      }
    } else {
      field.options = [];
      field.value = null;
    }

    await this.influence(multiple, field, fields, body.instid, true, form);
  }

  getDynamicNomenclature(url: string, body: any = {}): Observable<any> {
    if (url.startsWith("assets")) {
      return this.http.get(url);
    } else {
      return this.http.post(url, body);
    }
  }

  buildValidators(validations: Validator[]) {
    if (!validations) return [];
    return validations.map(v => {
      if (Validators[v.name])
        if (v.validation) v.validator = Validators[v.name](v.validation);
        else v.validator = Validators[v.name];
      else if (v.validation) v.validator = CustomValidators[v.name](v.validation);
      else v.validator = CustomValidators[v.name];
      return v;
    });
  }

  private isValidDate(date) {
    return date.getTime() === date.getTime();
  }

  private async transformField(multiple: boolean, field: FieldConfig, value, fields: FieldConfig[], instid, form) {
    const instKind = this.instType; // for eval
    const operationType = this.operationType; // for eval

    value = typeof value === "string" ? value.trim() : value;
    if ((field.value + "").includes("if")) {
      eval(field.value + "");
    } else {
      field.value = value;
    }

    if (!field.value && field.value !== 0 && field.defaultValue) {
      (field.defaultValue + "").includes("if") && eval(field.defaultValue + "");
      field.value = typeof field.defaultValue === "string" ? field.defaultValue.trim() : field.defaultValue;
    }

    field.disabled && eval(field.disabled + "");
    field.rendered && eval(field.rendered + "");
    field.flexible && eval(field.flexible + "");

    if (field.type === FieldType.Date && this.isValidDate(new Date(field.value)) && field.value) {
      field.value = new Date(field.value);
    }

    field.optionsPath && !field.options && (field.options = []);
    field.validations = this.buildValidators(field.validations);

    if (field.optionsPath && field.options.length === 0) {
      const options = await this.getDynamicNomenclature(field.optionsPath, {
        instid,
        instKind: this.instType
      }).toPromise();
      field.options = options;
      const required = field.validations && field.validations.find(validation => validation.name === "required");
      if (!required && field.type !== FieldType.SearchMultiSelect) {
        field.options.unshift({ label: "", code: null });
      }
    }

    // await this.influence(multiple, field, fields, instid, false, form);

    return field;
  }

  private async transformSubsection(subsection: Subsection, parent: Subsection | Section, value, instid, form: Form) {
    !subsection.fields && (subsection.fields = []);
    !subsection.subsections && (subsection.subsections = []);

    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      for (let field of subsection.fields) {
        field = await this.transformField(false, field, value[field.name], subsection.fields, instid, form);
      }

      for (const subsubsection of subsection.subsections) {
        await this.transformSubsection(subsubsection, subsection, value, instid, form);
      }
    } else {
      let valuesLength;
      value[subsection.name] && (valuesLength = value[subsection.name].length);
      const valArr = valuesLength > 0 ? value[subsection.name] : null;

      subsection.id = valuesLength > 0 ? valArr[0].id : "new0";
      subsection.position = 0;
      subsection.subsectionRecordsCount = valuesLength > 0 ? valuesLength : 1;

      if (subsection.canHaveFiles && valuesLength > 0) {
        subsection.fileId = valArr[0].fileId;
        subsection.fileLabel = valArr[0].fileLabel;
      }

      for (let field of subsection.fields) {
        const fieldVal = valuesLength > 0 ? valArr[0][field.name] : null;
        field = await this.transformField(true, field, fieldVal, subsection.fields, instid, form);
      }

      if (subsection.canHaveCertificate && valuesLength > 0) {
        subsection.certificateId = valArr[0].certificateId;
        subsection.certificateLabel = valArr[0].certificateLabel;
        subsection.copyCertificateId = valArr[0].copyCertificateId;
        subsection.copyCertificateLabel = valArr[0].copyCertificateLabel;
      }

      for (const subsubsection of subsection.subsections) {
        const vals = value[subsection.name] && value[subsection.name].length > 0 ? value[subsection.name][0] : {};
        await this.transformSubsection(subsubsection, subsection, vals, instid, form);
      }

      if (valuesLength > 0 && subsection.rendered !== false) {
        const additionalSubsections = await this.duplicate(subsection, value, valuesLength, instid);
        parent.subsections = parent.subsections.concat(additionalSubsections);
      }
    }

    subsection.fields = subsection.fields
      .filter(field => !(!field.value && field.value !== 0 && field.flexible) && field.rendered !== false)
      .sort((field1, field2) => field1.order - field2.order);

    subsection.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
  }

  private async duplicate(subsection: Subsection, value: any, subsectionRecordsCount: number, instid: string | number) {
    let result = [];
    for (let i = 1; i < subsectionRecordsCount; i++) {
      const valI = value[subsection.name][i];
      const newSubsection = await this.duplicateSubsection(subsection, valI.id, valI, subsectionRecordsCount, instid);
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
    instid: string | number,
    copy: boolean = true
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

      subsubsection = await this.duplicateSubsection(subsubsection, id, values, count, instid, false);
    }

    for (let j = 0; j < newSubsection.fields.length; j++) {
      const field = newSubsection.fields[j];
      field.value = valuesObj[field.name] || valuesObj[field.name] === 0 ? valuesObj[field.name] : field.defaultValue;
      if (field.value && field.type === FieldType.Date && this.isValidDate(new Date(field.value))) {
        field.value = new Date(field.value);
      }

      await this.influence(true, field, newSubsection.fields, instid);
    }

    newSubsection.subsectionRecordsCount = subsectionRecordsCount ? subsectionRecordsCount : 1;
    return newSubsection;
  }

  private async influence(multiple: boolean, field: FieldConfig, fields: FieldConfig[], instid, setOptions = false, form = null) {
    if (field.influence && (!field.flexible || field.value !== undefined)) {
      for (const influence of field.influence) {
        const influencedField = influence.fieldName ? this.getInfluencedField(multiple, influence.fieldName, fields, form) : null;

        if (influencedField && influence.type === InfluenceType.Options) {
          const url = field.value ? influence.url : null;
          const body: any = instid ? { filterValue: field.value, instid } : { filterValue: field.value };

          this.instType && (body.instKind = this.instType);
          await this.setOptions(multiple, url, body, influencedField, fields, form);
        } else if (influencedField && influence.type === InfluenceType.Disable && !setOptions) {
          influencedField.disabled = influence.condition.includes(field.value);
        } else if (influencedField && influence.type === InfluenceType.SetValue) {
          influencedField.value = influence.condition.includes(field.value) ? influence.value : influencedField.value;
        } else if (influencedField && influence.type === InfluenceType.DefaultValue) {
          let includesVal;

          if (influence.condition) {
            includesVal = influence.condition.includes(field.value);
          }

          if (includesVal && (influencedField.value === undefined || influencedField.value === null)) {
            influencedField.value = influence.defaultValue;
            influencedField.defaultValue = influence.defaultValue;
          }
        } else if (influencedField && influence.fieldName && !setOptions) {
          const includes = influence.condition.includes(field.value);
          influencedField.hidden = influence.type === InfluenceType.Hide ? includes : !includes;
          field.rendered === false && (influencedField.rendered = !influencedField.hidden);
        }
      }
    }
  }

  private getInfluencedField(multiple: boolean, fieldName: string, fields: FieldConfig[], form: Form | null): FieldConfig {
    let influencedField = fields.find(fld => fld.name == fieldName);

    if (!influencedField && !multiple && form && form.sections) {
      form.sections.forEach(section => {
        !influencedField &&
          section.subsections.forEach(subsection => {
            !influencedField && (influencedField = subsection.fields.find(fld => fld.name === fieldName));

            if (!influencedField && subsection.subsections && subsection.subsections.length > 0) {
              subsection.subsections.forEach(sub => {
                !influencedField && (influencedField = sub.fields.find(fld => fld.name === fieldName));
              });
            }
          });
      });
    }

    return influencedField;
  }

  private manageSubsectionInfluence(field: FieldConfig, subsection: Subsection, parent: Subsection | Section, form: Form) {
    for (const influence of field.influence) {
      if (influence.subsectionName) {
        let sub = influence.subsectionName === subsection.name ? subsection : null;
        if (!sub && form && form.sections) {
          form.sections.forEach(section => {
            section.subsections &&
              section.subsections.forEach(ssection => {
                if (!sub && ssection.name === influence.subsectionName) {
                  sub = ssection;
                } else if (!sub && ssection.subsections) {
                  sub = ssection.subsections.find(s => s.name === influence.subsectionName);
                }
              });
          });
        }

        const includes = influence.condition && influence.condition.includes(field.value);
        sub && (sub.hidden = influence.type === InfluenceType.Hide ? includes : !includes);
        field.rendered === false && sub && (sub.rendered = !sub.hidden);
      } else if (influence.sectionName) {
        let section = influence.sectionName === parent.name ? parent : null;
        !section && form && (section = form.sections.find(s => s.name === influence.sectionName));

        const includes = influence.condition && influence.condition.includes(field.value);
        section && (section.hidden = influence.type === InfluenceType.Hide ? includes : !includes);
        field.rendered === false && section && (section.rendered = !section.hidden);
      }
    }
  }

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  addControl(field: FieldConfig, group: FormGroup) {
    if (field.type === FieldType.Button) {
      return;
    }

    const control = this.fb.control(field.value, this.bindValidations(field.validations || []));
    group.addControl(field.name, control);

    if (field.disabled) {
      control.disable();
    }
  }

  bindValidations(validations: Validator[]) {
    if (validations.length > 0) {
      const validList = [];

      validations.forEach(validation => {
        validList.push(validation.validator);
      });

      return Validators.compose(validList);
    }

    return null;
  }

  deepCopyObj(object) {
    let copy = { ...object };
    for (let key in object) {
      if (object[key] && !object[key].length && Object.keys(object[key]).length > 0 && typeof object[key] === "object") {
        copy[key] = this.deepCopyObj(object[key]);
      } else if (object[key] && object[key].length >= 0 && typeof object[key] === "object") {
        copy[key] = object[key].length > 0 ? this.deepCopyArr(object[key]) : [];
      }
    }
    return copy;
  }

  deepCopyArr(array) {
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

  createSubsectionGroup(subsection: Subsection, group: FormGroup) {
    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      subsection.fields.forEach(field => this.addControl(field, group));
      for (const subsubsection of subsection.subsections) {
        this.createSubsectionGroup(subsubsection, group);
      }
    } else {
      const idGroup = this.fb.group({});
      const rowGroup = this.fb.group({});

      subsection.fields.forEach(field => this.addControl(field, rowGroup));

      for (const subsubsection of subsection.subsections) {
        this.createSubsectionGroup(subsubsection, rowGroup);
      }

      idGroup.addControl(subsection.id, rowGroup);

      if (group.get(subsection.name)) {
        const arr = group.get(subsection.name);
        (<FormArray>arr).push(idGroup);
      } else {
        const arr = this.fb.array([]);
        arr.push(idGroup);
        group.addControl(subsection.name, arr);
      }
    }
  }

  hasNonHiddenField(subsection: Subsection) {
    let res = false;

    for (const field of subsection.fields) {
      if (!field.hidden && field.rendered !== false && field.type !== FieldType.Breakpoint) {
        res = true;
        break;
      }
    }

    if (!res && subsection.subsections) {
      for (const sub of subsection.subsections) {
        for (const fld of sub.fields) {
          if (!fld.hidden && fld.rendered !== false && fld.type !== FieldType.Breakpoint) {
            res = true;
            break;
          }
        }
      }
    }

    return res;
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

    const newSubsections: Subsection[] = subsection.subsections.filter(sub => sub.canDuplicate === undefined || sub.canDuplicate == null);

    for (const key in subsectionsCounters) {
      const toCopy = subsection.subsections.find(sub => sub.name === key);
      for (let i = 0; i < subsectionsCounters[key]; i++) {
        const sub = this.deepCopyObj(toCopy);
        sub.position = i;
        newSubsections.push(sub);
      }
    }

    newSubsections.sort((sub1, sub2) => sub1.order - sub2.order);
    subsection.subsections = newSubsections;
  }
}
