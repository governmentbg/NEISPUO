import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
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
import { CustomFormControl } from "../models/custom-form-control.interface";
import { AuthService } from "../auth/auth.service";
import { environment } from "../../environments/environment";

@Injectable()
export class FormDataService {
  public routeParamChanged: Subject<{ paramName: string; paramValue: string | number }> = new Subject();
  public optionsChanged: Subject<string> = new Subject(); // name of field whose options changed

  private operationType;
  private instKind;
  private procType;
  private instType;

  constructor(private http: HttpClient, private fb: FormBuilder, private authService: AuthService) {}

  private real: boolean = true;
  private formValues: any;

  getRegixData(data: { operation: string; xmlns: string; requestName: string; params }): Observable<any> {
    return this.http.get(`${environment.regixUrl}`, {
      params: data,
      headers: new HttpHeaders({
        Authorization: `Bearer ${this.authService.getToken()}`
      })
    });
  }

  submitForm(body) {
    body.data.sysuserid = this.authService.getSysUserId();
    return this.http.post("/data/save", body);
  }

  getInstitutionHistory(id: number | string) {
    return this.real
      ? this.http.post("/data/get/institution-history", {
          instid: id,
          sysuserid: this.authService.getSysUserId(),
          region: this.authService.getRegion()
        })
      : this.http.get(`assets/json/history-${id}.json`);
  }

  getTrData(body) {
    return this.real ? this.http.post("/data/getTRData", body) : this.http.get("assets/json/institution-combo.json");
  }

  getData(formName: string, body) {
    return this.http.post(`/data/get/${formName}`, body);
  }

  getInstitutionCombo(role: number) {
    return this.real
      ? this.http.get("/uimeta/get/institution-combo").pipe(
          map((res: { code: string; label: string; formName: string; roleID: number; instType: number }[]) => {
            res = res.filter(elem => elem.roleID == role);
            res.forEach(elem => delete elem.roleID);
            return res;
          })
        )
      : this.http.get("assets/json/institution-combo.json");
  }

  getEditPermissions(isMon: boolean) {
    return this.real
      ? this.http.get("/uimeta/get/edit-permissions").pipe(
          map(res => {
            const prop = isMon ? "mon" : "ruo";
            res = res[prop];
            return res;
          })
        )
      : this.http.get("assets/json/permissions.json").pipe(
          map(res => {
            const prop = isMon ? "mon" : "ruo";
            res = res[prop];
            return res;
          })
        );
  }

  getFormData(formName: string, body, operationType, instKind, instType): Observable<any> {
    const instid = body.instid ? body.instid : null;

    const reqForm = this.real ? this.http.get(`/uimeta/get/${formName}`) : this.http.get(`assets/json/${formName}.json`);

    this.operationType = operationType;
    this.instKind = instKind;
    this.procType = body.procType;
    this.instType = instType;

    return reqForm.pipe(
      switchMap(async (form: Form) => {
        // for eval
        const procType = body.procType;
        if (form.dataName && form.dataName.includes("if")) {
          eval(form.dataName);
        }

        const dataName = operationType == 1 || form.dataName !== undefined ? form.dataName : formName;
        const reqData = this.real ? this.http.post(`/data/get/${dataName}`, body) : this.http.get(`assets/json/fake-data/${formName}.json`);

        const res: any = dataName ? await reqData.toPromise() : null;
        let value = res && res.length > 0 ? res[0] : res || {};

        await this.transformForm(form, value, instid, body, dataName);
        return form;
      })
    );
  }

  private async transformForm(form: Form, value, instid, body, dataName: string) {
    //for eval
    const operationType = this.operationType;
    const instKind = this.instKind;
    const procType = body.procType;
    const instType = this.instType;

    this.formValues = value;
    await this.setInfluencedBy(form, body);

    for (let section of form.sections) {
      !section.subsections && (section.subsections = []);
      section.rendered && eval(section.rendered + "");

      if (section.rendered !== false) {
        const initialLength = section.subsections.length;

        for (let i = 0; i < initialLength; i++) {
          const subsection = section.subsections[i];
          subsection.rendered && eval(subsection.rendered + "");

          subsection.rendered !== false && (await this.transformSubsection(subsection, section, value, instid, form, body, dataName));
        }

        section.subsections = section.subsections
          .filter(subsection => (subsection.fields.length > 0 || subsection.subsections.length > 0) && subsection.rendered !== false)
          .sort((subsection1, subsection2) => subsection1.order - subsection2.order);

        if (section.table) {
          const values = value && value[section.table.name] ? value[section.table.name] : [];
          await this.transformTable(section.table, values, body, instid);
        }
      }
    }

    // set influences after all values have been set
    await this.setInfluencedFieldsValues(form, instid, body);

    form.sections = form.sections
      .filter(
        section => (section.subsections.length > 0 || (section.table && section.table.rendered !== false)) && section.rendered !== false
      )
      .sort((section1, section2) => section1.order - section2.order);
  }

  private async setInfluencedFieldsValues(form: Form, instid, body) {
    if (form.sections) {
      for (const section of form.sections) {
        if (section.subsections && section.subsections.length) {
          for (const subsection of section.subsections) {
            await this.influenceHelper(subsection, section, form, instid, body);
          }
        }
      }
    }
  }

  private async transformTable(table: Table, values, body, instid) {
    const instKind = this.instKind; //for eval
    const operationType = this.operationType; //for eval
    table.values = [];

    table && table.rendered && eval(table.rendered + "");

    let selectFlds = [];
    if ((operationType === 1 || table.disabledViewMode) && values && values.length && table.formName && table.rendered !== false) {
      const createFormReq = this.real
        ? this.http.get(`/uimeta/get/${table.formName}`)
        : this.http.get(`assets/json/${table.formName}.json`);
      const createForm = <Form>await createFormReq.toPromise();

      await this.transformForm(createForm, {}, instid, body, table.formName);

      createForm.sections.forEach(section => {
        section.subsections &&
          section.subsections.forEach(subsection => {
            subsection.fields &&
              subsection.fields.forEach(fld => {
                if (fld.type === FieldType.SearchSelect || fld.type === FieldType.Select) {
                  selectFlds.push(fld);
                }
              });
          });
      });
    }

    if (table.rendered !== false) {
      for (const tableField of table.fields) {
        if (tableField.optionsPath) {
          await this.setOptions(true, tableField.optionsPath, {}, tableField);
        } else if (selectFlds.find(fld => fld.name === tableField.name)) {
          const selectFld = selectFlds.find(fld => fld.name === tableField.name);
          selectFld && (tableField.options = selectFld.options);
          tableField.options && tableField.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
        }
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
            field.options && field.options.length && row[field.name] ? field.options.find(opt => opt.code === row[field.name]) : null;
          const value = option ? option.label : row[field.name];
          await this.transformField(true, field, value, fields, row.procID, { sections: [] }, body, table.formName);
          option && (field.code = option.code);

          if (field.disabled !== false) {
            if (field.value === true) {
              field.value = "да";
            } else if (field.value === false) {
              field.value = "не";
            }
          }
        }

        table.values.push({
          id: row.id,
          fields,
          formName: row.formName,
          procID: row.procID,
          instKind: row.instKind,
          instid: row.instid,
          navigateActiveProc: row.navigateActiveProc,
          instType: row.instType,
          isActive: row.isActive
        });
      }
    }

    return table;
  }

  private async setOptions(multiple: boolean, optionsPath: string, body, field: FieldConfig, fields: FieldConfig[] = [], form = {}) {
    if (optionsPath) {
      const options = await this.getDynamicNomenclature(optionsPath, body).toPromise();
      field.options = options;
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
    body.sysuserid = this.authService.getSysUserId();
    const regionId = this.authService.getRegion();
    regionId && regionId !== "null" && (body.region = regionId);
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
        else {
          v.validator = v.name === ValidatorType.Required ? CustomValidators.requiredControl : Validators[v.name];
          v.name === ValidatorType.Required && (v.name = ValidatorType.RequiredControl);
        }
      else if (v.validation) v.validator = CustomValidators[v.name](v.validation);
      else v.validator = CustomValidators[v.name];
      return v;
    });
  }

  private isValidDate(date) {
    return date.getTime() === date.getTime();
  }

  private async transformField(
    multiple: boolean,
    field: FieldConfig,
    value,
    fields: FieldConfig[],
    instid,
    form: Form,
    body,
    dataName: string
  ) {
    // eval strings use the following fields:
    const operationType = this.operationType;
    const instKind = this.instKind;
    const procType = this.procType;
    const region = body.region;

    value = typeof value === "string" ? value.trim() : value;
    if ((field.value + "").includes("if")) {
      eval(field.value + "");
    } else {
      field.value = value;
    }

    this.defaultVal(field, body);

    field.disabled && eval(field.disabled + "");
    field.rendered && eval(field.rendered + "");
    field.flexible && eval(field.flexible + "");

    if (field.type === FieldType.Date && this.isValidDate(new Date(field.value)) && field.value) {
      field.value = new Date(field.value);
    }

    if (field.type === FieldType.Checkbox) {
      field.value = !!field.value;
    }

    field.optionsPath && !field.options && (field.options = []);
    field.validations = this.buildValidators(field.validations);

    if (field.optionsPath && field.options.length === 0) {
      const body = this.instType ? { instid, instKind, instType: this.instType } : { instid, instKind };
      const options = await this.getDynamicNomenclature(field.optionsPath, body).toPromise();

      field.options = options;
      const required =
        field.validations &&
        field.validations.find(
          validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
        );
      !required && field.options.length && field.options.unshift({ label: "", code: null });
    }

    if ((field.type === FieldType.Select || field.type == FieldType.SearchSelect) && field.name === form.statusFieldName) {
      form.isDraft = field.options.find(option => option.code === field.value)?.isDraft;
    }

    if ((field.value || field.value === 0) && dataName === "institution-create") {
      field.disabled = true;
    }

    return field;
  }

  private defaultVal(field: FieldConfig, body) {
    // for eval
    const operationType = this.operationType;
    const instKind = this.instKind;
    const procType = this.procType;
    const region = body.region;
    const instid = body.instid;

    if (!field.value && field.value !== 0 && field.value !== false && field.defaultValue !== undefined) {
      (field.defaultValue + "").includes("if") && eval(field.defaultValue + "");
      if (body) {
        for (const key in body) {
          if (field.defaultValue === key) {
            field.defaultValue = key !== "personalID" && key != "instid" && !isNaN(body[key]) ? +body[key] : body[key];
          }
        }
      }

      field.defaultValue === "null" && (field.defaultValue = null);
      field.value = typeof field.defaultValue === "string" ? field.defaultValue.trim() : field.defaultValue;
    }
  }

  private async transformSubsection(
    subsection: Subsection,
    parent: Subsection | Section,
    value,
    instid,
    form: Form,
    body,
    dataName: string
  ) {
    !subsection.fields && (subsection.fields = []);
    !subsection.subsections && (subsection.subsections = []);

    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      for (let field of subsection.fields) {
        field = await this.transformField(false, field, value[field.name], subsection.fields, instid, form, body, dataName);
      }

      for (const subsubsection of subsection.subsections) {
        await this.transformSubsection(subsubsection, subsection, value, instid, form, body, dataName);
      }
    } else {
      let valuesLength;
      value[subsection.name] && (valuesLength = value[subsection.name].length);
      const valArr = valuesLength > 0 ? value[subsection.name] : null;

      subsection.id = valuesLength > 0 ? valArr[0].id : "new0";
      subsection.position = 0;
      subsection.subsectionRecordsCount = valuesLength > 0 ? valuesLength : 1;

      if (valuesLength > 0) {
        subsection.fileId = valArr[0].fileId;
        subsection.fileLabel = valArr[0].fileLabel;
      }

      for (let field of subsection.fields) {
        const fieldVal = valuesLength > 0 ? valArr[0][field.name] : null;
        field = await this.transformField(true, field, fieldVal, subsection.fields, instid, form, body, dataName);
      }

      if (subsection.canHaveCertificate && valuesLength > 0) {
        subsection.certificateId = valArr[0].certificateId;
        subsection.certificateLabel = valArr[0].certificateLabel;
        subsection.copyCertificateId = valArr[0].copyCertificateId;
        subsection.copyCertificateLabel = valArr[0].copyCertificateLabel;
      }

      for (const subsubsection of subsection.subsections) {
        const vals = value[subsection.name] && value[subsection.name].length > 0 ? value[subsection.name][0] : {};
        await this.transformSubsection(subsubsection, subsection, vals, instid, form, body, dataName);
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

      await this.influence(true, field, newSubsection.fields, newSubsection, parent, instid);
    }

    newSubsection.subsectionRecordsCount = subsectionRecordsCount ? subsectionRecordsCount : 1;
    newSubsection.fileId = valuesObj.fileId;
    newSubsection.fileLabel = valuesObj.fileLabel;
    return newSubsection;
  }

  private async influence(
    multiple: boolean,
    field: FieldConfig,
    fields: FieldConfig[],
    subsection: Subsection,
    parentSubsection: Subsection,
    instid,
    setOptions = false,
    form = null,
    clear: boolean = false
  ) {
    if (field.influence && (!field.flexible || field.value !== undefined)) {
      for (const influence of field.influence) {
        const allFields = this.getAllFields(influence.fieldName, subsection, parentSubsection, form);

        for (const [influencedField, ssection, parentSubsection] of allFields) {
          if (influencedField && influence.type === InfluenceType.Options && (!influencedField.options || clear)) {
            const filterValue = field.value === "null" ? null : field.value;
            const url = filterValue ? influence.url : null;
            const body: any = instid ? { filterValue, instid } : { filterValue };

            this.instKind && (body.instKind = this.instKind);
            this.instType && (body.instType = this.instType);
            await this.setOptions(multiple, url, body, influencedField, fields, form);
          } else if (influence.type === InfluenceType.Require && !setOptions) {
            for (const fieldName of influence.fields) {
              const influencedField = this.getInfluencedField(multiple, fieldName, fields, form);
              if (influencedField) {
                !influencedField.requiredBy && (influencedField.requiredBy = []);
                influencedField.requiredBy.push(field.name);
                !influencedField.validations && (influencedField.validations = []);

                if (field.value) {
                  !influencedField.validations.find(
                    validator => validator.name === ValidatorType.Required || validator.name === ValidatorType.RequiredControl
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
          } else if (influencedField && influence.type === InfluenceType.Disable && !setOptions) {
            influencedField.disabled = influence.condition.includes(field.value);
          } else if (influencedField && influence.type === InfluenceType.SetValueOppositeCondition) {
            influencedField.value = field.value && !influence.condition.includes(field.value) ? influence.value : influencedField.value;
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
          } else if (influencedField && influence.fieldName && !setOptions && influence.type !== InfluenceType.Options) {
            if (influencedField.hiddenBy.length > 0 && field.name !== influencedField.hiddenBy[0].fieldName) {
              continue;
            }

            influencedField.hidden = this.isHiddenInitially(
              influencedField.hiddenBy,
              field.value,
              influence.condition,
              influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear,
              ssection,
              parentSubsection,
              form
            );
            field.rendered === false && (influencedField.rendered = !influencedField.hidden);
            influencedField.hidden && influence.type === InfluenceType.HideClear && (influencedField.value = null);
          }
        }
      }
    }
  }

  private getInfluencedField(multiple: boolean, fieldName: string, fields: FieldConfig[], form: Form | null): FieldConfig {
    let influencedField = fields.find(fld => fld.name == fieldName);

    if (!influencedField && !multiple && form && form.sections) {
      form.sections.forEach(section => {
        !influencedField &&
          section.subsections &&
          section.subsections.forEach(subsection => {
            !influencedField && subsection.fields && (influencedField = subsection.fields.find(fld => fld.name === fieldName));

            if (!influencedField && subsection.subsections && subsection.subsections.length > 0) {
              subsection.subsections.forEach(sub => {
                !influencedField && sub.fields && (influencedField = sub.fields.find(fld => fld.name === fieldName));
              });
            }
          });
      });
    }

    return influencedField;
  }

  private async manageSubsectionInfluence(
    field: FieldConfig,
    subsection: Subsection,
    parent: Subsection | Section,
    form: Form,
    instid,
    body
  ) {
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

        if (sub && (influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear)) {
          sub.hidden = this.isHiddenInitially(sub.hiddenBy, field.value, influence.condition, true, sub, parent, form);
          sub.hidden && influence.type === InfluenceType.HideClear && (await this.clearSubsectionValues(sub, form, instid, body)); // ?
        } else if (sub && influence.type === InfluenceType.Render) {
          sub.hidden = this.isHiddenInitially(sub.hiddenBy, field.value, influence.condition, false, sub, parent, form);
        } else if (sub && influence.type === InfluenceType.Disable) {
          this.disableSubsectionFields(sub);
        }

        field.rendered === false && sub && (sub.rendered = !sub.hidden);
      } else if (influence.sectionName) {
        let section = influence.sectionName === parent.name ? parent : null;
        !section && form && (section = form.sections.find(s => s.name === influence.sectionName));

        if (section && (influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear)) {
          section.hidden = this.isHiddenInitially(section.hiddenBy, field.value, influence.condition, true, null, null, form);
          if (section.hidden && influence.type === InfluenceType.HideClear) {
            for (const sub of section.subsections) {
              await this.clearSubsectionValues(sub, form, instid, body); // ?
            }
          }
        } else if (section && influence.type === InfluenceType.Render) {
          section.hidden = this.isHiddenInitially(section.hiddenBy, field.value, influence.condition, false, null, null, form);
        } else if (section && influence.type === InfluenceType.Disable) {
          section.subsections && section.subsections.forEach(sub => this.disableSubsectionFields(sub));
        }

        field.rendered === false && section && (section.rendered = !section.hidden);
      }
    }
  }

  private async clearSubsectionValues(subsection: Subsection, form: Form, instid, body) {
    if (subsection.fields) {
      for (const fld of subsection.fields) {
        fld.value = null;
        this.defaultVal(fld, body);
        await this.influenceHelper(subsection, subsection, form, instid, body, true);
      }
    }
    if (subsection.subsections) {
      for (const sub of subsection.subsections) {
        await this.clearSubsectionValues(sub, form, instid, body);
      }
    }
  }

  private async influenceHelper(subsection: Subsection, parent, form: Form, instid: string, body, hideClear: boolean = false) {
    if (subsection.fields && subsection.fields.length) {
      for (const field of subsection.fields) {
        await this.influence(
          subsection.canDuplicate === false || subsection.canDuplicate === true,
          field,
          subsection.fields,
          subsection,
          null,
          instid,
          false,
          form,
          hideClear
        );

        field.influence && (await this.manageSubsectionInfluence(field, subsection, parent, form, instid, body));

        if (field.name === "procedureTransformTypeID") {
          form.canBeAnnulled = field.options.find(option => option.code === field.value)?.isAnullment;
          field.value === 4 && (form.canBeRenewed = true);
          field.value === 26 ? (form.hideCertificateBtn = true) : (form.hideCertificateBtn = false);
        }
      }
    }

    if (subsection.subsections && subsection.subsections.length) {
      for (const subsubsection of subsection.subsections) {
        await this.influenceHelper(subsubsection, subsection, form, instid, body, hideClear);
      }
    }
  }

  private disableSubsectionFields(subsection: Subsection) {
    subsection.fields && subsection.fields.forEach(fld => (fld.disabled = true));

    subsection.subsections && subsection.subsections.forEach(sub => this.disableSubsectionFields(sub));
  }

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  addControl(field: FieldConfig, group: FormGroup, subsectionHidden: boolean, sectionHidden: boolean) {
    if (!field.value && field.value !== 0 && field.requiredBy && field.requiredBy.length > 0) {
      field.validations = field.validations.filter(
        validation => validation.name !== ValidatorType.Required && validation.name !== ValidatorType.RequiredControl
      );
    }

    const control = this.fb.control(field.value, this.bindValidations(field.validations || []));
    (control as CustomFormControl).hidden = !!(field.hidden || subsectionHidden || sectionHidden);
    (control as CustomFormControl).code = field.code;
    (control as CustomFormControl).type = field.type;
    group.addControl(field.name, control);

    if (field.disabled) {
      control.disable();
    }
  }

  getFormGroup(subsection: Subsection, fg) {
    let id = "";
    let formGroup = fg;
    if (subsection.canDuplicate !== undefined && subsection.canDuplicate !== null) {
      const formArr = <FormArray>fg.get(subsection.name);
      const parentGroup = formArr.controls.find((control: FormGroup) => {
        const key = Object.keys(control.controls)[0];
        const matches = key == subsection.id;
        matches && (id = key);
        return matches;
      });

      if (parentGroup) {
        formGroup = <FormGroup>parentGroup.get(id);
      }
    }
    return formGroup;
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

  createSubsectionGroup(subsection: Subsection, group: FormGroup, sectionHidden: boolean) {
    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      subsection.fields.forEach(field => this.addControl(field, group, subsection.hidden, sectionHidden));
      for (const subsubsection of subsection.subsections) {
        this.createSubsectionGroup(subsubsection, group, sectionHidden);
      }
    } else {
      const idGroup = this.fb.group({});
      const rowGroup = this.fb.group({});

      subsection.fields.forEach(field => this.addControl(field, rowGroup, subsection.hidden, sectionHidden));

      for (const subsubsection of subsection.subsections) {
        this.createSubsectionGroup(subsubsection, rowGroup, sectionHidden);
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

    const newSubsections: Subsection[] = subsection.subsections.filter(sub => sub.canDuplicate === undefined || sub.canDuplicate === null);

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

  manageInfluencedField(
    event,
    instid,
    influencedField,
    formControl,
    instKind,
    formGroup: FormGroup,
    parentGroup: FormGroup,
    allFieldsGroup: FormGroup
  ) {
    if (event.influence.type === InfluenceType.Options) {
      formControl.setValue(null);
      influencedField.value = null;

      if (!event.influence.url || !event.filterValue || event.filterValue === "null") {
        influencedField.options = [];
        this.optionsChanged.next(influencedField.name);
      } else {
        const body: any = { filterValue: event.filterValue };
        instid && (body.instid = instid);
        instKind && (body.instKind = instKind);

        if (event.filterValue) {
          this.getDynamicNomenclature(event.influence.url, body).subscribe(res => {
            influencedField.options = res;
            this.optionsChanged.next(influencedField.name);

            const required = influencedField.validations.find(
              validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
            );
            !required && influencedField.options.length && influencedField.options.unshift({ label: "", code: null });
          });
        }
      }
    } else if (event.influence.type === InfluenceType.Hide || event.influence.type === InfluenceType.HideClear) {
      influencedField.hidden = this.isHidden(
        influencedField.hiddenBy,
        event.value,
        event.influence.condition,
        true,
        formGroup,
        parentGroup,
        allFieldsGroup
      );
      (formControl as CustomFormControl).hidden = influencedField.hidden;
      influencedField.hidden && event.influence.type === InfluenceType.HideClear && formControl.setValue(null);
    } else if (event.influence.type === InfluenceType.Render) {
      influencedField.hidden = this.isHidden(
        influencedField.hiddenBy,
        event.value,
        event.influence.condition,
        false,
        formGroup,
        parentGroup,
        allFieldsGroup
      );
      (formControl as CustomFormControl).hidden = influencedField.hidden;
    } else if (event.influence.type === InfluenceType.Disable) {
      event.influence.condition.includes(event.value) && formControl.disable();
      !event.influence.condition.includes(event.value) && formControl.enable();
    } else if (event.influence.type === InfluenceType.SetValueOppositeCondition) {
      influencedField.value = event.value && !event.influence.condition.includes(event.value) ? event.influence.value : null;
      formControl.setValue(influencedField.value);
    } else if (event.influence.type === InfluenceType.SetValue) {
      influencedField.value = event.influence.condition.includes(event.value) ? event.influence.value : formControl.value;
      formControl.setValue(influencedField.value);
    } else if (event.influence.type === InfluenceType.DefaultValue) {
      let includesVal;

      if (event.influence.condition) {
        includesVal = event.influence.condition.includes(event.value);
      }

      if (includesVal && (formControl.value === undefined || formControl.value === null)) {
        influencedField.value = event.influence.defaultValue;
        influencedField.defaultValue = event.influence.defaultValue;
        formControl.setValue(event.influence.defaultValue);
      }
    }
  }

  encodeParams(queryParams) {
    let queryString = "";

    for (const key in queryParams) {
      if (queryParams[key] || queryParams[key] === 0) {
        queryString = queryString ? queryString + "&" + key + "=" + queryParams[key] : key + "=" + queryParams[key];
      }
    }

    const regex = new RegExp("=" + "+$");
    const queryParam = btoa(queryString).replace(regex, "");

    return (queryParams = { q: queryParam });
  }

  decodeParams(encodedParamString: string) {
    const decoded = atob(encodedParamString);
    const arr = decoded.split("&");
    const queryParams: any = {};

    arr.forEach(pair => {
      const splitPair = pair.split("=");
      queryParams[splitPair[0]] = splitPair[1];
    });

    return queryParams;
  }

  transformFormValue(formValues, form) {
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

            !nullObj && this.transformDocSubsection(elem, key, keys[0], form);

            if (!nullObj && keys[0].startsWith("new")) {
              elem = this.transformElement(elem, keys, "", form);
            } else if (!keys[0].startsWith("new")) {
              elem = this.transformElement(elem, keys, keys[0], form);
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

  private transformElement(obj, keys, id, form) {
    const props = obj[keys[0]];
    typeof id === "number" && (id = +id);
    obj = { id };
    for (const innerKey in props) {
      const val = props[innerKey] && props[innerKey].code !== undefined ? props[innerKey].code : props[innerKey];
      obj[innerKey] = this.transformFormValue({ [innerKey]: val }, form)[innerKey];
    }
    return obj;
  }

  private transformDocSubsection(elem, subsectionName, id, form) {
    let docSubsection: Subsection;
    for (const section of form.sections) {
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

  isHidden(
    influencers: { fieldName: string; condition: (string | number)[]; hide: boolean }[],
    value,
    condition: (string | number)[],
    hide: boolean,
    formGroup: FormGroup,
    parentGroup: FormGroup,
    allFieldsGroup: FormGroup
  ) {
    if (influencers && influencers.length > 1) {
      let res = false;

      for (const influencer of influencers) {
        const fieldName = influencer.fieldName;
        let val = formGroup && formGroup.getRawValue()[fieldName];
        val === undefined && parentGroup && (val = parentGroup.getRawValue()[fieldName]);
        val === undefined && allFieldsGroup && (val = allFieldsGroup.getRawValue()[fieldName]);
        val === undefined && (val = null);

        let includesVal;
        if (influencer.condition) {
          includesVal = influencer.condition.includes(val);
        }

        res = influencer.hide ? res || includesVal : res || !includesVal;
      }

      return res;
    } else {
      let includes = false;

      if (condition) {
        includes = condition.includes(value);
      }

      return hide ? includes : !includes;
    }
  }

  isDisabled(
    influencers: { fieldName: string; condition: (string | number)[] }[],
    value,
    condition: (string | number)[],
    formGroup: FormGroup,
    parentGroup: FormGroup,
    allFieldsGroup: FormGroup
  ) {
    if (influencers && influencers.length > 1) {
      let res = false;

      for (const influencer of influencers) {
        const fieldName = influencer.fieldName;
        let val = formGroup && formGroup.getRawValue()[fieldName];
        val === undefined && parentGroup && (val = parentGroup.getRawValue()[fieldName]);
        val === undefined && allFieldsGroup && (val = allFieldsGroup.getRawValue()[fieldName]);
        val === undefined && (val = null);

        let includesVal;
        if (influencer.condition) {
          includesVal = influencer.condition.includes(val);
        }

        res = res || includesVal;
      }

      return res;
    } else {
      if (condition) {
        return condition.includes(value);
      }

      return false;
    }
  }

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
    const subsectionDuplicate = subsection ? subsection.canDuplicate === true || subsection.canDuplicate === false : false;
    const parentDuplicate = parentSubsection ? parentSubsection.canDuplicate === true || parentSubsection.canDuplicate === false : false;

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

  private isHiddenInitially(
    influencers: { fieldName: string; condition: (string | number)[]; hide: boolean }[],
    value,
    condition: (string | number)[],
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

        let includesVal;

        if (influencer.condition) {
          includesVal = influencer.condition.includes(val);
        }

        res = influencer.hide ? res || includesVal : res || !includesVal;
      }

      return res;
    } else {
      let includes = false;

      if (condition) {
        includes = condition.includes(value);
      }

      return hide ? includes : !includes;
    }
  }

  private isDisabledInitially(
    influencers: { fieldName: string; condition: (string | number)[] }[],
    value,
    condition: (string | number)[],
    subsection: Subsection,
    parentSubsection: Subsection,
    form: Form
  ) {
    if (influencers && influencers.length > 1) {
      let res = false;

      for (const influencer of influencers) {
        let val = this.findFldVal(influencer.fieldName, subsection, parentSubsection, form);
        val === undefined && (val = null);

        let includesVal;
        if (influencer.condition) {
          includesVal = influencer.condition.includes(val);
        }

        res = res || includesVal;
      }

      return res;
    } else {
      if (condition) {
        return condition.includes(value);
      }

      return false;
    }
  }

  private findFldVal(fieldName: string, container, parent, form: Form) {
    let val = this.formValues[fieldName];
    const keys = Object.keys(this.formValues);

    if ((val == undefined && !keys.includes(fieldName)) || val == null) {
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

  private async setInfluencedBy(form: Form, body) {
    if (form.sections) {
      for (let section of form.sections) {
        if (section.subsections) {
          for (let subsection of section.subsections) {
            if (subsection.fields) {
              for (let fld of subsection.fields) {
                await this.setInfluence(fld, subsection, form, body);
              }
            }

            if (subsection.subsections) {
              for (let subsubsection of subsection.subsections) {
                if (subsubsection.fields) {
                  for (let fld of subsubsection.fields) {
                    await this.setInfluence(fld, subsubsection, form, body);
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  private async setInfluence(field: FieldConfig, subsection: Subsection, form: Form, body) {
    if (field.influence) {
      for (let influence of field.influence) {
        if (influence.condition && typeof influence.condition === "string") {
          influence.condition = this.real ? <any[]>await this.http
                .post(`/data/get/${influence.condition}`, { ...body })
                .pipe(map((codeList: any[]) => codeList.map(el => el.code)))
                .toPromise() : <any[]>await this.http.get(`assets/json/${influence.condition}.json`).toPromise();
        }

        if (
          influence.fieldName &&
          [InfluenceType.Disable, InfluenceType.HideClear, InfluenceType.Hide, InfluenceType.Render].includes(influence.type)
        ) {
          const influencedField = this.getField(influence.fieldName, subsection, form);
          this.setInfluenceHelper(influence.type, influencedField, field.name, influence.condition);
        } else if (influence.subsectionName) {
          const subsection = this.getSubsection(influence.subsectionName, form);
          this.setInfluenceHelper(influence.type, subsection, field.name, influence.condition);
        } else if (influence.sectionName) {
          const section = this.getSection(influence.sectionName, form);
          this.setInfluenceHelper(influence.type, section, field.name, influence.condition);
        }
      }
    }
  }

  private setInfluenceHelper(
    influenceType: InfluenceType,
    object: FieldConfig | Subsection | Section,
    fieldName: string,
    condition: (string | number)[]
  ) {
    if (influenceType === InfluenceType.Hide || influenceType === InfluenceType.Render || influenceType === InfluenceType.HideClear) {
      const hide = influenceType === InfluenceType.Hide || influenceType === InfluenceType.HideClear;
      !object.hiddenBy && (object.hiddenBy = []);
      object.hiddenBy.push({ fieldName, condition, hide });
    } else if (influenceType === InfluenceType.Disable) {
      !object.disabledBy && (object.disabledBy = []);
      object.disabledBy.push({ fieldName, condition });
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
}
