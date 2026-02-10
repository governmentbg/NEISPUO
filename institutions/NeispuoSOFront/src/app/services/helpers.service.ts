import { Injectable } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Subject } from "rxjs";
import { AuthService } from "../auth/auth.service";
import { FieldType } from "../enums/fieldType.enum";
import { FormTypeInt } from "../enums/formType.enum";
import { InfluenceType } from "../enums/influenceType.enum";
import { Mode, ModeInt } from "../enums/mode.enum";
import { ValidatorType } from "../enums/validatorType.enum";
import { CustomFormControl } from "../models/custom-form-control.interface";
import { FieldConfig } from "../models/field.interface";
import { Form } from "../models/form.interface";
import { Influence } from "../models/influence.interface";
import { Option } from "../models/option.interface";
import { Subsection } from "../models/subsection.interface";
import { Validator } from "../models/validator.interface";
import { FormDataService } from "./form-data.service";

@Injectable()
export class HelperService {
  public optionsChanged: Subject<string> = new Subject(); // name of field whose options changed
  public regixValueSet: Subject<{ fldName: string; value: string | number }> = new Subject();
  public tableItemSelected: Subject<{ paramName: string; formDataId: string | number; additionalParams: Object }> = new Subject();
  public routeParamChanged: Subject<{ paramName: string; paramValue: string | number }> = new Subject();
  public oidcStarted: Subject<void> = new Subject();
  public subCleared: Subject<{ name: string; id: string | number }> = new Subject();
  public tableValuesChanged: Subject<{ fieldVal: string; multiselectField: string }> = new Subject();
  public addressDataGathered: Subject<Option[]> = new Subject();
  public historyDataGathered: Subject<{ periodData: Option[]; yearData: Option[] }> = new Subject();
  public errorOccured: boolean = false;
  public fieldHidden: Subject<string> = new Subject();

  constructor(private fb: FormBuilder, private formDataService: FormDataService, private authService: AuthService) {}

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  getFormGroup(subsection: Subsection, fg: FormGroup) {
    let id = "";
    let formGroup = fg;
    if (subsection && subsection.canDuplicate !== undefined && subsection.canDuplicate !== null) {
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

  createSubsectionGroup(subsection: Subsection, group: FormGroup, sectionHidden: boolean, sectionDisabled: boolean) {
    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      subsection.fields.forEach(field =>
        this.addControl(
          field,
          group,
          subsection.hidden || subsection.rendered === false,
          subsection.disabled,
          sectionHidden,
          sectionDisabled
        )
      );

      if (subsection.subsections) {
        for (const subsubsection of subsection.subsections) {
          this.createSubsectionGroup(subsubsection, group, sectionHidden, sectionDisabled);
        }
      }
    } else {
      const idGroup = this.fb.group({});
      const rowGroup = this.fb.group({});

      subsection.fields.forEach(field =>
        this.addControl(
          field,
          rowGroup,
          subsection.hidden || subsection.rendered === false,
          subsection.disabled,
          sectionHidden,
          sectionDisabled
        )
      );

      if (subsection.subsections) {
        for (const subsubsection of subsection.subsections) {
          this.createSubsectionGroup(subsubsection, rowGroup, sectionHidden, sectionDisabled);
        }
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

  addControl(
    field: FieldConfig,
    group: FormGroup,
    subsectionHidden: boolean,
    subsectionDisabled: boolean,
    sectionHidden: boolean,
    sectionDisabled: boolean
  ) {
    if (field.type === FieldType.Breakpoint || field.type === FieldType.Info || field.type === FieldType.Button) {
      return;
    }

    const control = this.fb.control(field.value, this.bindValidations(field.validations || []));
    (control as CustomFormControl).hidden = !!(field.hidden || subsectionHidden || sectionHidden || field.rendered === false);
    (control as CustomFormControl).code = field.code;
    (control as CustomFormControl).type = field.type;
    group.addControl(field.name, control);

    if (field.disabled || subsectionDisabled || sectionDisabled) {
      control.disable();
    }
  }

  private bindValidations(validations: Validator[]) {
    if (validations.length > 0) {
      const validList = [];

      validations.forEach(validation => {
        validList.push(validation.validator);
      });

      return Validators.compose(validList);
    }

    return null;
  }

  transformFormValue(formValues) {
    for (let key in formValues) {
      if (formValues[key] && formValues[key] instanceof Date) {
        formValues[key] = this.convertDate(formValues[key]);
      } else if (formValues[key] && typeof formValues[key] === "object" && formValues[key]["_d"]) {
        formValues[key] = this.convertDate(formValues[key]["_d"]);
      }

      if (formValues[key] && typeof formValues[key] === "object" && formValues[key].length > 0) {
        formValues[key] = formValues[key]
          .map(elem => {
            if (Object.keys(elem).length > 0) {
              const keys = Object.keys(elem);
              const nullObj = this.checkIfNullObj(elem[keys[0]]);

              // !nullObj && this.transformDocSubsection(elem, key, keys[0], sections);

              if (!nullObj && keys[0].startsWith("new")) {
                elem = this.transformElement(elem, keys, "");
              } else if (!keys[0].startsWith("new")) {
                elem = this.transformElement(elem, keys, keys[0]);
              } else {
                elem = null;
              }
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
        isNullObj = isNullObj && (obj[key] === null || obj[key] === undefined);
      }
    }
    return isNullObj;
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

  async manageInfluencedField(
    event: { value: any; influence: Influence; filterValue: string | number; label?: string; fieldName?: string },
    influencedField: FieldConfig,
    formControl: AbstractControl,
    queryParams: Object,
    formGroup: FormGroup,
    parentGroup: FormGroup,
    allFieldsGroup: FormGroup,
    isHistory: boolean
  ) {
    const type = event.influence.type;

    if (type === InfluenceType.Options) {
      this.manageInfluencedFieldOptions(
        event,
        influencedField,
        formControl,
        queryParams,
        formGroup,
        parentGroup,
        allFieldsGroup,
        isHistory
      );
    } else if (type === InfluenceType.Hide || type === InfluenceType.Render || type === InfluenceType.HideClear) {
      influencedField.hidden = this.isHidden(
        influencedField.hiddenBy,
        event.value,
        event.influence.condition,
        event.influence.notNull,
        type === InfluenceType.Hide || type === InfluenceType.HideClear,
        formGroup,
        parentGroup,
        allFieldsGroup
      );

      influencedField.hidden && type === InfluenceType.HideClear && (influencedField.value = null);
      formControl && type === InfluenceType.HideClear && formControl.setValue(null);

      this.fieldHidden.next(influencedField.name);
      formControl && ((formControl as CustomFormControl).hidden = influencedField.hidden);
      formControl.updateValueAndValidity();
    } else if (type === InfluenceType.Disable) {
      const includes = this.isDisabled(
        influencedField.disabledBy,
        event.value,
        event.influence.condition,
        event.influence.notNull,
        formGroup,
        parentGroup,
        allFieldsGroup
      );

      includes ? formControl.disable() : formControl.enable();
    } else if (type === InfluenceType.Fill && !formControl.value && formControl.value !== 0 && event.label) {
      if (
        influencedField.type === FieldType.Select ||
        influencedField.type === FieldType.Searchselect ||
        influencedField.type === FieldType.Multiselect
      ) {
        influencedField.value = event.value;
        formControl.setValue(event.value);
      } else {
        influencedField.value = event.label;
        formControl.setValue(event.label);
      }
    } else if (type === InfluenceType.DefaultValue) {
      const isMultiSelect = event.value && typeof event.value === "object" && event.value.length >= 0;
      let includesVal;

      if (event.influence.condition) {
        includesVal = isMultiSelect
          ? event.value.some(elem => event.influence.condition.includes(elem))
          : event.influence.condition.includes(event.value);
      } else if (event.influence.notNull) {
        includesVal = isMultiSelect ? event.value.some(elem => elem || elem === 0) : event.value || event.value === 0;
      }

      if (includesVal && influencedField.dbValue === undefined) {
        influencedField.value = event.influence.defaultValue;
        influencedField.defaultValue = event.influence.defaultValue;
        formControl.setValue(event.influence.defaultValue);
      }
    } else if (type === InfluenceType.SetValue) {
      await this.manageInfluencedFieldSetValue(event, influencedField, formControl, queryParams);
    } else if (type === InfluenceType.SetValueOppositeCondition && event.value && !event.influence.condition.includes(event.value)) {
      influencedField.value = event.influence.value;
      formControl.setValue(influencedField.value);
    }
  }

  private manageInfluencedFieldOptions(
    event: { value: string | number; influence: Influence; filterValue: string | number; fieldName?: string },
    influencedField: FieldConfig,
    formControl: AbstractControl,
    queryParams: Object,
    formGroup: FormGroup,
    parentGroup: FormGroup,
    allFieldsGroup: FormGroup,
    isHistory: boolean
  ) {
    const filterValues = this.collectFilterValues(influencedField.influencedBy, event.filterValue, formGroup, parentGroup, allFieldsGroup);

    event.filterValue = Object.keys(filterValues).length ? event.filterValue : null;

    const url = this.getInfluenceUrl(influencedField, filterValues);

    if (!url || event.filterValue === null || event.filterValue === undefined) {
      influencedField.options = [];
      formControl.setValue(null);
      influencedField.value = null;
      this.optionsChanged.next(influencedField.name);
    } else {
      if (event.filterValue) {
        this.formDataService.getDynamicNomenclature(url, isHistory, { ...queryParams, ...filterValues }).subscribe(res => {
          influencedField.options = res;
          influencedField.options &&
            influencedField.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
          this.optionsChanged.next(influencedField.name);

          if (!influencedField.options.find(option => option.code == formControl.value)) {
            formControl.setValue(null);
            influencedField.value = null;
          }

          const required = influencedField.validations.find(
            validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
          );
          !required && influencedField.options.length && influencedField.options.unshift({ label: "", code: null });
        });
      }
    }
  }

  private async manageInfluencedFieldSetValue(
    event: { value: string | number; influence: Influence; filterValue: string | number; fieldName?: string },
    influencedField: FieldConfig,
    formControl: AbstractControl,
    queryParams: Object
  ) {
    if (!event.influence.url || event.filterValue === null || event.filterValue === undefined) {
      influencedField.value = null;
      formControl.setValue(null);
    } else {
      let res = await this.formDataService
        .getFieldValue(event.influence.url, { ...queryParams, [event.fieldName]: event.filterValue })
        .toPromise();
      res && res.length && (res = res[0]);

      if (res[influencedField.name] !== undefined) {
        influencedField.value = res[influencedField.name];
        formControl.setValue(res[influencedField.name]);
      }
    }
  }

  encodeParams(queryParams) {
    let queryString = "";

    for (const key in queryParams) {
      if (queryParams[key] !== undefined && queryParams[key] !== null) {
        queryString = queryString ? queryString + "&" + key + "=" + queryParams[key] : key + "=" + queryParams[key];
      }
    }

    const regex = new RegExp("=" + "+$");
    const queryParam = btoa(encodeURIComponent(queryString)).replace(regex, "");

    return (queryParams = { q: queryParam });
  }

  decodeParams(encodedParamString: string) {
    if (encodedParamString) {
      const decoded = decodeURIComponent(window.atob(encodedParamString));
      const arr = decoded.split("&");
      const queryParams: any = {};

      arr.forEach(pair => {
        const splitPair = pair.split("=");
        queryParams[splitPair[0]] = splitPair[1];
      });

      return queryParams;
    } else {
      return {};
    }
  }

  customGetRawValue(formGroup: FormGroup) {
    let formValues = {};

    for (let key in formGroup.controls) {
      if (formGroup.controls[key] instanceof FormArray) {
        formValues[key] = [];
        const arr = <FormArray>formGroup.controls[key];
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
        formValues[key] = formGroup.controls[key].value;
      }
    }

    return formValues;
  }

  private collectFilterValues(
    influencedBy: { fieldName: string; condition: (string | number)[]; url: string }[],
    filterValue,
    formGroup: FormGroup,
    parentGroup: FormGroup,
    allFieldsGroup: FormGroup
  ) {
    if (influencedBy && influencedBy.length > 1) {
      const filter: any = {};

      for (const influencer of influencedBy) {
        let val = formGroup && formGroup.getRawValue()[influencer.fieldName];
        val === undefined && parentGroup && (val = parentGroup.getRawValue()[influencer.fieldName]);
        val === undefined && allFieldsGroup && (val = allFieldsGroup.getRawValue()[influencer.fieldName]);
        val !== null && val !== undefined && (filter[influencer.fieldName] = val);
      }

      return filter;
    } else {
      return { filterValue };
    }
  }

  isHidden(
    influencers: { fieldName: string; condition: (string | number)[]; hide: boolean; notNull: boolean }[],
    value,
    condition: (string | number)[],
    notNull: boolean,
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

        const isMultiSelect = val && typeof val === "object" && val.length >= 0;

        let includesVal;
        if (influencer.condition) {
          includesVal = isMultiSelect ? val.some(elem => influencer.condition.includes(elem)) : influencer.condition.includes(val);
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

  isDisabled(
    influencers: { fieldName: string; condition: (string | number)[]; notNull: boolean }[],
    value,
    condition: (string | number)[],
    notNull: boolean,
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

        const isMultiSelect = val && typeof val === "object" && val.length >= 0;

        let includesVal;
        if (influencer.condition) {
          includesVal = isMultiSelect ? val.some(elem => influencer.condition.includes(elem)) : influencer.condition.includes(val);
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

  setFormControlsDisabled(subsection: Subsection, sectionDisabled: boolean, fg: FormGroup, parentGroup) {
    const formGroup: FormGroup = this.getFormGroup(subsection, fg);
    const parent = formGroup === fg ? parentGroup : formGroup;

    subsection.fields &&
      subsection.fields.forEach(fld => {
        const formControl = formGroup ? formGroup.get(fld.name) : parent.get(fld.name);
        formControl && (sectionDisabled || subsection.disabled || fld.disabled ? formControl.disable() : formControl.enable());
      });

    subsection.subsections &&
      subsection.subsections.forEach(sub => this.setFormControlsDisabled(sub, sectionDisabled || subsection.disabled, formGroup, parent));
  }

  setFormControlsHidden(subsection: Subsection, sectionHidden: boolean, fg: FormGroup, parentGroup, clear: boolean) {
    const formGroup: FormGroup = this.getFormGroup(subsection, fg);
    const parent = formGroup === fg ? parentGroup : formGroup;

    subsection.fields &&
      subsection.fields.forEach(fld => {
        const formControl = formGroup ? formGroup.get(fld.name) : parent.get(fld.name);
        formControl && ((formControl as CustomFormControl).hidden = sectionHidden || subsection.hidden || fld.hidden);
        const val = fld.defaultValue ? fld.defaultValue : null;
        formControl?.hidden && clear && formControl.setValue(val);
        formControl?.updateValueAndValidity();
      });

    subsection.subsections &&
      subsection.subsections.forEach(sub => this.setFormControlsHidden(sub, sectionHidden || subsection.hidden, formGroup, parent, clear));
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

  getRequestBody(production: boolean, isCreateMode: boolean, mode: Mode, queryParams, params) {
    queryParams = production ? this.decodeParams(queryParams["q"]) : queryParams;
    const body: any = { ...queryParams };

    for (const key in body) {
      if (body[key] === "null") {
        delete body[key];
      }
    }

    let operationType = ModeInt.view;

    const instType = params.type ? FormTypeInt[params.type] + 1 : FormTypeInt[this.authService.getType()] + 1;

    if (mode === Mode.Edit) {
      isCreateMode ? (operationType = ModeInt.update) : (operationType = ModeInt.create);
      queryParams["operationType"] && (operationType = queryParams["operationType"]);
    }

    instType && (body.instType = instType);
    return [body, operationType];
  }

  transformReorderValues(formValues, form: Form) {
    for (let key in formValues) {
      if (
        formValues[key] &&
        typeof formValues[key] === "object" &&
        formValues[key].length &&
        formValues[key][0] &&
        typeof formValues[key][0] === "object"
      ) {
        let isReorderTable = false;

        for (const section of form.sections) {
          if (section.table && section.table.name === key && section.table.reorder) {
            isReorderTable = true;
            break;
          }
        }

        if (isReorderTable) {
          for (let i = 0; i < formValues[key].length; i++) {
            formValues[key][i].ordernum = i + 1;
          }
        }
      }
    }
  }

  setCurrentFormValues(form: Form, formValues) {
    for (let section of form.sections) {
      for (let subsection of section.subsections) {
        for (let field of subsection.fields) {
          if (formValues[field.name] !== undefined) {
            field.value = formValues[field.name];
          }

          for (let sub of subsection.subsections) {
            for (let field of sub.fields) {
              if (formValues[field.name] !== undefined) {
                field.value = formValues[field.name];
              }
            }
          }
        }
      }
    }
  }

  private getInfluenceUrl(influencedField: FieldConfig, fieldValues) {
    let url = null;

    for (let influencer of influencedField.influencedBy) {
      if (!influencer.condition || influencer.condition.includes(fieldValues[influencer.fieldName])) {
        url = influencer.url;
        break;
      }
    }

    return url;
  }
}
