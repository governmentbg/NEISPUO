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
  public physicalMenuData: MenuItem[];
  public historyMenuData: MenuItem[];
  public sampleListData: string[];
  public instIsLocked: boolean;
  public schoolYear: string | number = null;
  public detailedSchoolType: string | number = null;
  public formValues: any = {};
  public ipAddress;

  private operationType;
  private fakePreview: boolean = false;
  private instType;
  private lastOpenedTable: string = null;

  private real = true;
  private resultType = null;

  constructor(private http: HttpClient, private authService: AuthService) { }

  submitForm(body) {
    body.data.audit = this.getLogParams();
    return this.http.post("/data/save", this.transformBody(body));
  }

  performProcedure(body) {
    body.data.audit = this.getLogParams();
    return this.real ? this.http.post("/data/save", this.transformBody(body)) : this.http.get("assets/json/fake-data/classGroupData.json");
  }

  private getLogParams() {
    return {
      userAgent: window.navigator.userAgent,
      remoteIpAddress: this.ipAddress,
      loginSessionId: this.authService.getSessionId()
    };
  }

  getIpAddress() {
    return this.http.get(environment.ipUrl).pipe(map((res: any) => res.ip || res.IPv4));
  }

  getIsLocked(instid) {
    return this.real
      ? this.http.post("/data/get/instIsLocked", { instid }).pipe(
        switchMap(async (res: any) => {
          res && res.length && (res = res[0]);
          let changeYearRes: any = await this.http.post("/data/get/instIsLockedChangeYear", { instid }).toPromise();

          changeYearRes && changeYearRes.length && (changeYearRes = changeYearRes[0]);

          return { isLocked: !!res.isLocked || !!changeYearRes.isLocked };
        })
      )
      : this.http.get("assets/json/fake-data/isLocked.json");
  }

  getSchoolYear(instid) {
    return this.real
      ? this.http.post("/data/get/instCurrentYear", { instid })
      : this.http.get("assets/json/fake-data/instCurrentYear.json");
  }

  getDetailedSchoolType(instid) {
    return this.real
      ? this.http.post("/data/get/instDetailedSchoolType", { instid })
      : this.http.get("assets/json/fake-data/detailedSchoolType.json");
  }

  isOpenCampaign(instid) {
    return this.http.post("/data/get/instOpenCampaign", { instid });
  }

  getMainMenuData(type: string) {
    return this.real ? this.http.get(`/uimeta/get/${type}-menu`) : this.http.get("assets/json/menu-data/institution-menu.json");
  }

  getInstitutionReport(reportId: string) {
    return reportId === "reportHNY" ? this.http.get(`/uimeta/get/institutionHNYreport`) : this.http.get(`/uimeta/get/institutionSOBreport`);
  }

  getInstitutionReportData(reportId: string, userId: number, roleId: number) {
    return reportId === "reportHNY" ? this.http.post('/data/get/institutionHNYreport', { sysuserid: userId, sysroleid: roleId }) :
      this.http.post('/data/get/institutionSOBreport', { sysuserid: userId, sysroleid: roleId });
  }

  getPhysicalMenuData() {
    return this.real ? this.http.get(`/uimeta/get/physical-environment-menu`) : this.http.get("assets/json/menu-data/physical-menu.json");
  }

  getSampleListData() {
    return this.real ? this.http.get(`/uimeta/get/so-menu`) : this.http.get("assets/json/menu-data/so-menu.json");
  }

  getListViewData(body) {
    return this.real
      ? this.http.post(`/data/get/submissionValidityCheckResult`, this.transformBody(body))
      : this.http.get("assets/json/fake-data/listView.json");
  }

  submissionCheckPeriod(body) {
    return this.real
      ? this.http.post(`/data/get/submissionCheckPeriod`, this.transformBody(body))
      : this.http.get("assets/json/fake-data/submissionCheckPeriod.json");
  }

  getHistoryMenuData(type: string) {
    return this.real
      ? this.http.get(`/uimeta/get/${type}-menu`).pipe(
        map((res: MenuItem[]) => {
          res = this.filterChildren(res);
          return res;
        })
      )
      : this.http.get("assets/json/menu-data/institution-menu.json").pipe(
        map((res: MenuItem[]) => {
          res = this.filterChildren(res);
          return res;
        })
      );
  }

  private filterChildren(items: MenuItem[]) {
    items = items.filter(item => item.history !== false);
    items.forEach(item => item.children && (item.children = this.filterChildren(item.children)));
    return items;
  }

  getInstType(instid: number) {
    return this.real ? this.http.post("/data/get/instTypeByInstID", { instid }) : this.http.get("assets/json/instType.json");
  }

  getExtData(instid) {
    return this.real ? this.http.post("/data/get/instExtData", { instid }) : this.http.get(`assets/json/fake-data/2999991.json`);
  }

  personExistForInst(body) {
    return this.http.post("/data/get/personByInst", this.transformBody(body));
  }

  getTrData(body) {
    return this.http.post("/data/get/personData", this.transformBody(body));
  }

  getAddressData(body) {
    return this.real
      ? this.http.post("/data/get/institutionDepartments", this.transformBody(body))
      : this.http.get("assets/json/fake-data/addressData.json");
  }

  getPeriodData(body) {
    return this.real ? this.http.get("/uimeta/get/period") : this.http.get("assets/json/fake-data/periodData.json");
  }

  getYearData(body) {
    return this.real
      ? this.http.post("/data/get/schoolYear", this.transformBody(body))
      : this.http.get("assets/json/fake-data/yearData.json");
  }

  getMessages() {
    return this.http.get("/uimeta/get/messages");
  }

  fillData(operationType, instType, data, formName, isForTable: boolean = true) {
    const reqForm = this.http.get(`/uimeta/get/${formName}`);

    this.operationType = operationType;
    this.fakePreview = false;
    this.instType = instType;

    return reqForm.pipe(
      switchMap(async (form: Form) => {
        let tableName;
        for (let section of form.sections) {
          if (section.table) {
            tableName = section.table.name;
          }
        }

        const body = isForTable ? { [tableName]: data } : data;
        await this.transformForm(form, body, { instType }, false, Mode.View);

        return form;
      })
    );
  }

  getMultiAddTableValues(table: Table, body) {
    const reqData = this.real
      ? this.http.post(`/data/get/${table.multiAdd.addFrom}`, this.transformBody(body))
      : this.http.get(`assets/json/fake-data/${table.multiAdd.addFrom}.json`);

    return reqData.pipe(
      switchMap(async (res: any) => {
        let value = res;
        await this.transformTable(table, value, body, false, Mode.Edit);
        return table;
      })
    );
  }

  getMultiAddTableValuesOnly(addFrom: string, body) {
    return this.real
      ? this.http.post(`/data/get/${addFrom}`, this.transformBody(body))
      : this.http.get(`assets/json/fake-data/${addFrom}.json`);
  }

  // after decoding queryParams columns and additionalParams become strings and not string arrays
  getSwitchTableList(
    dataName: string,
    instid,
    tableParams: {
      tableName: string;
      columns: string;
      switchLabel: string;
      paramName: string;
      additionalParams: string;
    }
  ) {
    const reqData = this.real
      ? this.http.post(`/data/get/${dataName}`, { instid })
      : this.http.get(`assets/json/fake-data/${dataName}.json`);

    return reqData.pipe(
      map(formValues => {
        const res = [];

        if (tableParams) {
          const tableName = tableParams.tableName;
          const columns = tableParams.columns;

          if (columns && formValues && formValues[tableName] && formValues[tableName].length) {
            const rows = formValues[tableName];

            rows.forEach(row => {
              let rowVal = "";

              for (const key in row) {
                if (columns.includes(key)) {
                  rowVal += row[key] + " ";
                }
              }

              const additionalParams: any = {};

              tableParams.additionalParams &&
                tableParams.additionalParams.split(",").forEach(param => {
                  additionalParams[param] = row[param];
                });

              res.push({ code: row.id, label: rowVal.trim(), additionalParams });
            });
          }
        }

        return res;
      })
    );
  }

  setSectionAccordionData(section: Section, body, isHistory: boolean, mode: Mode) {
    const req = this.real
      ? isHistory
        ? this.http.post(`/data/getso/${section.accordion.dataName}`, this.transformBody(body))
        : this.http.post(`/data/get/${section.accordion.dataName}`, this.transformBody(body))
      : this.http.get(`assets/json/fake-data/${section.accordion.dataName}.json`);

    return req.pipe(
      switchMap(async value => {
        this.formValues = { ...this.formValues, ...value };
        await this.setSectionValues(section, value, body, isHistory, mode);
      })
    );
  }

  setSubsectionAccordionData(subsection: Subsection, body, isHistory: boolean) {
    const req = this.real
      ? isHistory
        ? this.http.post(`/data/getso/${subsection.accordion.dataName}`, this.transformBody(body))
        : this.http.post(`/data/get/${subsection.accordion.dataName}`, this.transformBody(body))
      : this.http.get(`assets/json/fake-data/${subsection.accordion.dataName}.json`);

    return req.pipe(
      switchMap(async value => {
        this.formValues = { ...this.formValues, ...value };
        subsection.rendered = this.substituteParams(subsection.rendered, body);

        if (subsection.rendered !== false) {
          const operationType = this.operationType; //for eval
          const instType = this.instType; //for eval

          subsection.rendered && eval(subsection.rendered + "");

          if (subsection.label && subsection.label.includes("if")) {
            eval(subsection.label);
          }

          !subsection.fields && (subsection.fields = []);
          !subsection.subsections && (subsection.subsections = []);

          for (let field of subsection.fields) {
            field = await this.transformField(field, value[field.name], body, isHistory);
          }

          for (const subsubsection of subsection.subsections) {
            await this.transformSubsection(subsubsection, subsection, value, null, body, isHistory);
          }
        }
      })
    );
  }

  getData(formName: string, body) {
    return this.http.post(`/data/get/${formName}`, this.transformBody(body));
  }

  getFormData(
    formName: string,
    body,
    operationType,
    instType,
    isHistory: boolean,
    mode: Mode = Mode.View,
    changeFormValues: boolean = true,
    fakePreview: boolean = false
  ): Observable<any> {
    const reqForm = this.real ? this.http.get(`/uimeta/get/${formName}`) : this.http.get(`assets/json/${formName}.json`);

    // for eval
    this.operationType = operationType;
    this.fakePreview = fakePreview;
    this.instType = instType;

    return reqForm.pipe(
      switchMap(async (form: Form) => {
        this.resultType = null;

        if (body.extdata === undefined) {
          const extDataRes: any = await this.getExtData(body.instid).toPromise();
          body.extdata = extDataRes && extDataRes.length ? extDataRes[0].extdata : null;
          body.extAlldata = extDataRes && extDataRes.length ? extDataRes[0].extAlldata : null;
        }

        if (formName === "changeYearDataList") {
          let yearIsAvailable: any = await this.performProcedure({
            operationType: 0,
            procedureName: "inst_basic.ChangeYearIsAvailable",
            data: { instid: body.instid, sysuserid: body.sysuserid }
          }).toPromise();

          try {
            yearIsAvailable && (yearIsAvailable = JSON.parse(yearIsAvailable));
            yearIsAvailable && yearIsAvailable.length && (yearIsAvailable = yearIsAvailable[0]);
          } catch (err) {}

          this.resultType = yearIsAvailable.resultType;
        }

        if (form.dataName && form.dataName.includes("if")) {
          form.dataName = this.substituteParams(form.dataName, body);
          eval(form.dataName);
        }

        instType && (body.instType = instType);

        let dataName = operationType == 1 || form.dataName !== undefined ? form.dataName : formName;

        const reqData = this.real
          ? isHistory
            ? this.http.post(`/data/getso/${dataName}`, this.transformBody(body))
            : this.http.post(`/data/get/${dataName}`, this.transformBody(body))
          : this.http.get(`assets/json/fake-data/${dataName}.json`);

        const res: any = dataName ? await reqData.toPromise() : null;
        const regixData = this.authService.getRegixData() ? JSON.parse(this.authService.getRegixData()) : null;
        let value = regixData ? await this.parseRegixData(regixData) : res && res.length > 0 ? res[0] : res || {};

        if (isHistory && (value.length === 0 || Object.keys(value).length === 0)) {
          form = {
            sections: [{ label: "Няма данни за посочения период", order: 1, subsections: [], labelOnly: true }]
          };
        } else {
          await this.transformForm(form, value, body, isHistory, mode, changeFormValues);
        }

        return form;
      })
    );
  }

  private async transformForm(form: Form, value, body, isHistory: boolean, mode: Mode, changeFormValues: boolean = true) {
    if (mode === Mode.View) {
      this.lastOpenedTable = this.authService.getTableName();
      this.authService.removeTableName();
    }

    changeFormValues && (this.formValues = value);
    await this.setInfluencedBy(form, body);

    const operationType = this.operationType; //for eval
    const instType = this.instType; //for eval

    if (form.procedureName && form.procedureName.includes("if")) {
      eval(form.procedureName);
    }

    for (let section of form.sections) {
      section.name && value[section.name] && typeof value[section.name] === "string" && (section.label = value[section.name]);

      if (!section.subsections && !section.table) {
        section.labelOnly = true;
      }

      !section.subsections && (section.subsections = []);
      section.rendered = this.substituteParams(section.rendered, body);
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
              ? isHistory
                ? this.http.post(`/data/getso/${section.accordion.dataName}`, this.transformBody(body))
                : this.http.post(`/data/get/${section.accordion.dataName}`, this.transformBody(body))
              : this.http.get(`assets/json/fake-data/${section.accordion.dataName}.json`);

            const res: any = await reqData.toPromise();
            value = res && res.length > 0 ? res[0] : res || {};
            this.formValues = { ...this.formValues, ...value };
            section.dataLoaded = true;
            section.loading = false;
          } else if (section.accordion.state === "opened") {
            section.loading = false;
          }
        }

        const initialLength = section.subsections.length;

        for (let i = 0; i < initialLength; i++) {
          const subsection = section.subsections[i];
          subsection.rendered = this.substituteParams(subsection.rendered, body);
          subsection.rendered && eval(subsection.rendered + "");

          subsection.rendered !== false && (await this.transformSubsection(subsection, section, value, form, body, isHistory));
        }

        section.subsections = section.subsections
          .filter(subsection => subsection.fields.length > 0 || subsection.subsections.length > 0 || subsection.labelOnly)
          .sort((subsection1, subsection2) => subsection1.order - subsection2.order);

        if (section.table) {
          const values = value && value[section.table.name] ? value[section.table.name] : [];
          await this.transformTable(section.table, values, body, isHistory, mode);
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
    await this.setInfluencedFieldsValues(form, body, isHistory);

    form.sections = form.sections
      .filter(section => section.subsections.length > 0 || (section.table && section.table.rendered !== false) || section.labelOnly)
      .sort((section1, section2) => section1.order - section2.order);

    this.lastOpenedTable = null;
    return form;
  }

  private async setInfluencedFieldsValues(form: Form, body, isHistory: boolean) {
    if (form.sections) {
      for (const section of form.sections) {
        if (section.subsections && section.subsections.length) {
          for (const subsection of section.subsections) {
            await this.influenceHelper(subsection, section, form, body, isHistory);
          }
        }
      }
    }
  }

  private async influenceHelper(subsection: Subsection, parent, form: Form, body, isHistory: boolean, hideClear: boolean = false) {
    if (subsection.fields && subsection.fields.length) {
      for (const field of subsection.fields) {
        await this.influence(field, subsection, null, body, form, isHistory, hideClear);
        field.influence && (await this.manageSubsectionInfluence(field, subsection, parent, form, body, isHistory));
      }
    }

    if (subsection.subsections && subsection.subsections.length) {
      for (const subsubsection of subsection.subsections) {
        await this.influenceHelper(subsubsection, subsection, form, body, isHistory, hideClear);
      }
    }
  }

  async setSectionValues(section: Section, values, body, isHistory: boolean, mode: Mode) {
    const operationType = this.operationType; //for eval
    const instType = this.instType; //for eval

    if (section.table) {
      const val = values && values[section.table.name] ? values[section.table.name] : [];
      await this.transformTable(section.table, val, body, isHistory, mode);
    }

    const initialLength = section.subsections.length;

    for (let i = 0; i < initialLength; i++) {
      const subsection = section.subsections[i];
      subsection.rendered = this.substituteParams(subsection.rendered, body);
      subsection.rendered && eval(subsection.rendered + "");

      subsection.rendered !== false && (await this.transformSubsection(subsection, section, values, null, body, isHistory));
    }
  }

  private async transformTable(table: Table, values, body, isHistory: boolean, mode: Mode) {
    const operationType = this.operationType; //for eval
    const instType = this.instType; //for eval

    table.values = [];

    table && table.rendered && (table.rendered = this.substituteParams(table.rendered, body));
    table && table.rendered && eval(table.rendered + "");

    if (table && table.multiAdd && (table.multiAdd + "").includes("if")) {
      table.multiAdd = this.substituteParams(table.multiAdd, body);
      eval(table.multiAdd + "");
    }

    if (table && table.createNew && table.createNew.includes("if")) {
      table.createNew = this.substituteParams(table.createNew, body);
      eval(table.createNew + "");
    }

    let selectFlds = [];

    if (this.resultType !== null && !this.resultType) {
      table.createNew = null;
    }

    if ((mode === Mode.Edit || table.disabledViewMode) && values && values.length && table.formName && table.rendered !== false) {
      const createFormReq = this.real
        ? this.http.get(`/uimeta/get/${table.formName}`)
        : this.http.get(`assets/json/${table.formName}.json`);
      const createForm = <Form>await createFormReq.toPromise();
      await this.transformForm(createForm, {}, body, isHistory, mode, false);

      createForm.sections.forEach(section => {
        section.subsections &&
          section.subsections.forEach(subsection => {
            subsection.fields &&
              subsection.fields.forEach(fld => {
                if (fld.type === FieldType.Searchselect || fld.type === FieldType.Select || fld.type === FieldType.Multiselect) {
                  selectFlds.push(fld);
                }
              });
          });
      });
    }

    if (table.rendered !== false) {
      for (const field of table.fields) {
        if (field.label && field.label.includes("if")) {
          field.label = this.substituteParams(field.label, body);
          eval(field.label);
        }

        if (field.rendered) {
          field.rendered = this.substituteParams(field.rendered, body);
          eval(field.rendered + "");
        }

        const selectFld = selectFlds.find(fld => fld.name === field.name);
        selectFld && (field.options = selectFld.options);
        field.options && field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
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

      const canDeleteRowCondition = table.canDeleteRow;
      const hasEditButtonCondition =
        (table.action !== "edit" && table.action !== undefined) || table.hasEditButton !== undefined ? table.hasEditButton : true;

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
          field = await this.transformField(field, value, body, isHistory);
          option && (field.code = option.code);

          if (field.disabled !== false || mode === Mode.View) {
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

        // NEISPUONEW-327
        let canDeleteRow = canDeleteRowCondition;
        if (table && canDeleteRowCondition && (canDeleteRowCondition + "").includes("if")) {
          canDeleteRow = this.substituteParams(canDeleteRowCondition, { ...body, ...row });
          eval(canDeleteRow + "");
          canDeleteRow = table.canDeleteRow;
        }

        let hasEditButton = hasEditButtonCondition;
        if (table && hasEditButtonCondition && (hasEditButtonCondition + "").includes("if")) {
          hasEditButton = this.substituteParams(hasEditButtonCondition, { ...body, ...row });
          eval(hasEditButton + "");
          hasEditButton = table.hasEditButton;
        }

        table.values.push({
          id: row.id,
          fields,
          formName: row.formName,
          formDataId: row.formDataId,
          additionalParams,
          canDeleteRow,
          hasEditButton: hasEditButton
        });
      }

      table.canDeleteRow = canDeleteRowCondition;
      table.hasEditButton = hasEditButtonCondition;
    }

    return table;
  }

  private async setOptions(optionsPath: string, body, field: FieldConfig, isHistory: boolean) {
    if (optionsPath) {
      const options = await this.getDynamicNomenclature(optionsPath, isHistory, body).toPromise();
      field.options = options;
      field.options && field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
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

  getDynamicNomenclature(url: string, isHistory: boolean, body: any = {}): Observable<any> {
    body.instType = this.instType;
    if (url.startsWith("assets")) {
      return this.http.get(url);
    } else {
      // isHistory && (url = url.replace("get", "getso"));
      return this.http.post(url, this.transformBody(body));
    }
  }

  getFieldValue(url: string, body: any): Observable<any> {
    return this.real ? this.http.post(url, this.transformBody(body)) : this.http.get(`assets/json/fake-data/permanentAddress.json`);
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

  private async transformField(field: FieldConfig, value, body, isHistory: boolean) {
    // eval strings use the following fields:
    const operationType = this.operationType;
    const instType = this.instType;

    value = typeof value === "string" ? value.trim() : value;
    if ((field.value + "").includes("if")) {
      eval(field.value + "");
    } else {
      field.value = value;
    }

    field.dbValue = field.value;

    if (!field.value && field.value !== 0 && field.value !== false && field.defaultValue !== undefined) {
      if ((field.defaultValue + "").includes("if")) {
        field.defaultValue = this.substituteParams(field.defaultValue, body);
        eval(field.defaultValue + "");
      }

      if (body) {
        for (const key in body) {
          if (field.defaultValue === key) {
            field.defaultValue = key !== "personalID" && !isNaN(body[key]) ? +body[key] : body[key];
          }
        }
      }

      field.value = typeof field.defaultValue === "string" ? field.defaultValue.trim() : field.defaultValue;
    }

    field.label = this.substituteParams(field.label, body);
    if (field.label && field.label.includes("if")) {
      eval(field.label);
    }

    field.rendered = this.substituteParams(field.rendered, body);
    field.rendered && eval(field.rendered + "");
    field.disabled = this.substituteParams(field.disabled, body);
    field.disabled && eval(field.disabled + "");
    field.flexible = this.substituteParams(field.flexible, body);
    field.flexible && eval(field.flexible + "");

    if (field.type === FieldType.Date && this.isValidDate(new Date(field.value)) && field.value) {
      field.value = new Date(field.value);
    }

    if (field.type === FieldType.Checkbox) {
      field.value = !!field.value;
    }

    if (
      (field.type === FieldType.Searchselect || field.type === FieldType.Multiselect || field.type === FieldType.Select) &&
      field.value &&
      !isNaN(+field.value)
    ) {
      field.value = +field.value;
    }

    if (field.optionsPath && field.optionsPath.includes("if") && field.optionsPath.includes("{")) {
      field.optionsPath = this.substituteParams(field.optionsPath, body);
      eval(field.optionsPath);
    }

    field.optionsPath && !field.options && (field.options = []);
    field.validations = this.buildValidators(field.validations);

    if (field.optionsPath && field.options.length === 0 && (operationType !== ModeInt.view || typeof field.value === "number")) {
      const options = await this.getDynamicNomenclature(field.optionsPath, isHistory, body).toPromise();

      field.options = options;
      field.options && field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));

      const required =
        field.validations &&
        field.validations.find(
          validation => validation.name === ValidatorType.Required || validation.name === ValidatorType.RequiredControl
        );
      !required && field.options.length && field.options.unshift({ label: "", code: null });
    }

    return field;
  }

  transformTab(item: MenuItem, body): MenuItem {
    //body is query params
    const tab = { showTab: true };
    let expression = item.showTab;

    if (!expression && expression !== false) {
      item.showTab = true;
      return item;
    }

    expression = this.substituteParams(expression, {
      ...body
    });

    if ((expression + "").includes("if")) {
      eval(expression + "");
      item.showTab = tab.showTab;
    } else {
      item.showTab = expression === true;
    }

    return item;
  }

  private async transformSubsection(subsection: Subsection, parent: Subsection | Section, value, form: Form, body, isHistory: boolean) {
    const operationType = this.operationType; //for eval
    const instType = this.instType; //for eval

    subsection.rendered = this.substituteParams(subsection.rendered, body);
    subsection.rendered && eval(subsection.rendered + "");

    if (subsection.disabled) {
      subsection.disabled = this.substituteParams(subsection.disabled, body);
      eval(subsection.disabled + "");
    }

    subsection.name && value[subsection.name] && typeof value[subsection.name] === "string" && (subsection.label = value[subsection.name]);

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
          ? isHistory
            ? this.http.post(`/data/getso/${subsection.accordion.dataName}`, this.transformBody(body))
            : this.http.post(`/data/get/${subsection.accordion.dataName}`, this.transformBody(body))
          : this.http.get(`assets/json/fake-data/${subsection.accordion.dataName}.json`);

        const res: any = await reqData.toPromise();
        value = res && res.length > 0 ? res[0] : res || {};
        this.formValues = { ...this.formValues, ...value };
        subsection.dataLoaded = true;
      }
    }

    if (subsection.canDuplicate === undefined || subsection.canDuplicate === null) {
      for (let field of subsection.fields) {
        field = await this.transformField(field, value[field.name], body, isHistory);
      }

      for (const subsubsection of subsection.subsections) {
        await this.transformSubsection(subsubsection, subsection, value, form, body, isHistory);
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
        field = await this.transformField(field, fieldVal, body, isHistory);
      }

      for (const subsubsection of subsection.subsections) {
        const vals = value[subsection.name] && value[subsection.name].length > 0 ? value[subsection.name][0] : {};
        await this.transformSubsection(subsubsection, subsection, vals, form, body, isHistory);
      }

      if (valuesLength > 0 && subsection.rendered !== false) {
        const additionalSubsections = await this.duplicate(subsection, value, valuesLength, body, form, isHistory);
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

  private async duplicate(subsection: Subsection, value: any, subsectionRecordsCount: number, body, form: Form, isHistory: boolean) {
    let result = [];
    for (let i = 1; i < subsectionRecordsCount; i++) {
      const valI = value[subsection.name][i];
      const newSubsection = await this.duplicateSubsection(subsection, valI.id, valI, subsectionRecordsCount, body, form, isHistory);

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
    isHistory: boolean,
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
        isHistory,
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
        field.options && field.options.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));

        field.value = field.value[0].code;
      }
    }

    for (const field of newSubsection.fields) {
      if (!onButton) {
        await this.influence(field, newSubsection, parent, body, form, isHistory);
      } else if (field.influencedBy && field.influencedBy.length) {
        const influencer = this.getField(field.influencedBy[0].fieldName, newSubsection, form);
        if (influencer) {
          this.instType && (body.instType = this.instType);

          const filterValues = this.collectFilterValues(field, influencer, newSubsection, parent, form);
          const url = this.getInfluenceUrl(field, filterValues);
          await this.setOptions(url, { ...body, ...filterValues }, field, isHistory);
        }
      }
    }

    newSubsection.subsectionRecordsCount = subsectionRecordsCount ? subsectionRecordsCount : 1;
    return newSubsection;
  }

  private async influence(
    field: FieldConfig,
    subsection: Subsection,
    parentSubsection: Subsection,
    body,
    form,
    isHistory: boolean,
    hideClear: boolean = false // ?
  ) {
    if (field.influence && (!field.flexible || field.value !== undefined)) {
      for (const influence of field.influence) {
        if (influence.condition && typeof influence.condition === "string") {
          influence.condition = this.real ? <any[]>await this.http
            .post(`/data/get/${influence.condition}`, { ...body })
            .pipe(map((codeList: any[]) => codeList.map(el => el.code)))
            .toPromise() : <any[]>await this.http.get(`assets/json/${influence.condition}.json`).toPromise();
        }

        if (influence.url && influence.url.includes("if") && influence.url.includes("{")) {
          influence.url = this.substituteParams(influence.url, body);
          eval(influence.url);
        }

        const allFields = this.getAllFields(influence.fieldName, subsection, parentSubsection, form);

        for (const [influencedField, ssection, parentSubsection] of allFields) {
          if (
            influencedField &&
            influence.type === InfluenceType.Options &&
            (!influencedField.options || hideClear) &&
            (this.operationType !== ModeInt.view || typeof field.value === "number" || this.fakePreview)
          ) {
            if (influencedField.influencedBy.length > 0 && field.name !== influencedField.influencedBy[0].fieldName) {
              continue;
            }

            this.instType && (body.instType = this.instType);
            let url = null;

            const filterValues = this.collectFilterValues(influencedField, field, ssection, parentSubsection, form);

            if (field.value) {
              url = this.getInfluenceUrl(influencedField, filterValues);
            } else if (influencedField.optionsPath) {
              url = influencedField.optionsPath;
            }

            await this.setOptions(url, { ...body, ...filterValues }, influencedField, isHistory);
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
                    validator => validator.name === ValidatorType.Required || validator.name === ValidatorType.RequiredControl
                  ) &&
                    influencedField.validations.push({
                      name: ValidatorType.RequiredControl,
                      message: influence.message,
                      validation: "",
                      validator: CustomValidators.requiredControl
                    });
                }

                influencedField.options &&
                  influencedField.options.length > 0 &&
                  (influencedField.options = influencedField.options.filter(option => option.code !== null));
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
              includesVal = isMultiSelect ? field.value.some(elem => elem || elem === 0) : field.value || field.value === 0;
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
            (field.type === FieldType.Select || field.type === FieldType.Searchselect || field.type === FieldType.Multiselect) &&
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
            (influence.type === InfluenceType.Render ||
              influence.type === InfluenceType.Hide ||
              influence.type === InfluenceType.HideClear) &&
            influencedField.rendered !== false
          ) {
            if (influencedField.hiddenBy.length > 0 && field.name !== influencedField.hiddenBy[0].fieldName) {
              continue;
            }

            const hide = influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear;
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

            influencedField.hidden && influence.type === InfluenceType.HideClear && (influencedField.value = null); // ?
          } else if (influence.type === InfluenceType.SetValue) {
            if (!influence.url) {
              influencedField.value = null;
            } else if (field.value) {
              this.getFieldValue(influence.url, { ...body, [field.name]: field.value }).subscribe((res: any) => {
                res && res.length && (res = res[0]);

                if (res[influencedField.name]) {
                  influencedField.value = res[influencedField.name];
                }
              });
            }
          } else if (
            influence.type === InfluenceType.SetValueOppositeCondition &&
            field.value &&
            !influence.condition.includes(field.value)
          ) {
            influencedField.value = influence.value;
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

      influencedField.influencedBy.forEach(influencer => {
        if (influencer.fieldName === field.name) {
          filterValues[influencer.fieldName] = field.value;
        } else {
          const val = this.findFldVal(influencer.fieldName, subsection, parentSubsection, form);
          val !== null && val !== undefined && (filterValues[influencer.fieldName] = val);
        }
      });

      return filterValues;
    } else if (field.value !== null && field.value !== undefined) {
      return { filterValue: field.value };
    } else {
      return {};
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

        if (influence.url && influence.url.includes("if") && influence.url.includes("{")) {
          influence.url = this.substituteParams(influence.url, body);
          eval(influence.url);
        }

        if (influence.type === InfluenceType.Options) {
          const influencedField = this.getField(influence.fieldName, subsection, form);
          !influencedField.influencedBy && (influencedField.influencedBy = []);
          influencedField.influencedBy.push({ fieldName: field.name, condition: influence.condition, url: influence.url });
        } else if (
          influence.fieldName &&
          [InfluenceType.Disable, InfluenceType.HideClear, InfluenceType.Hide, InfluenceType.Render].includes(influence.type)
        ) {
          const influencedField = this.getField(influence.fieldName, subsection, form);
          this.setInfluenceHelper(influence.type, influencedField, field.name, influence.condition, influence.notNull);
        } else if (influence.subsectionName) {
          const subsection = this.getSubsection(influence.subsectionName, form);
          this.setInfluenceHelper(influence.type, subsection, field.name, influence.condition, influence.notNull);
        } else if (influence.sectionName) {
          const section = this.getSection(influence.sectionName, form);
          this.setInfluenceHelper(influence.type, section, field.name, influence.condition, influence.notNull);
        } else if (influence.createNewTableName) {
          const table = this.getTable(influence.createNewTableName, form);
          const hide = influence.type === InfluenceType.Hide;
          !table.createNewHiddenBy && (table.createNewHiddenBy = []);
          table.createNewHiddenBy.push({
            fieldName: field.name,
            condition: influence.condition,
            hide,
            notNull: influence.notNull
          });
        }
      }
    }

    field.dependingTables = [];

    for (let section of form.sections) {
      if (section.table?.multiAdd?.dependsOn?.length) {
        if (section.table.multiAdd.dependsOn.includes(field.name)) {
          field.dependingTables.push(section.table.name);
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
    if (influenceType === InfluenceType.Hide || influenceType === InfluenceType.Render || influenceType === InfluenceType.HideClear) {
      const hide = influenceType === InfluenceType.Hide || influenceType === InfluenceType.HideClear;
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
    body,
    isHistory: boolean
  ) {
    for (const influence of field.influence) {
      if (influence.subsectionName) {
        const influencedSubsections = this.getAllInfluencedSubsections(influence.subsectionName, subsection, parent, form);

        for (const [sub, parent] of influencedSubsections) {
          if (sub && sub.rendered !== false) {
            if (
              influence.type === InfluenceType.Hide ||
              influence.type === InfluenceType.Render ||
              influence.type === InfluenceType.HideClear
            ) {
              const hide = influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear;
              sub.hidden = this.isHidden(sub.hiddenBy, field.value, influence.condition, influence.notNull, hide, sub, parent, form);

              sub.hidden && influence.type === InfluenceType.HideClear && (await this.clearSubsectionValues(sub, form, body, isHistory)); // ?
            } else if (influence.type === InfluenceType.Disable) {
              sub.disabled = this.isDisabled(sub.disabledBy, field.value, influence.condition, influence.notNull, sub, parent, form);
            }
          }
        }
      } else if (influence.sectionName) {
        let section = influence.sectionName === subParent.name ? subParent : null;
        !section && form && (section = form.sections.find(s => s.name === influence.sectionName));

        if (section && section.rendered !== false) {
          if (
            influence.type === InfluenceType.Hide ||
            influence.type === InfluenceType.Render ||
            influence.type === InfluenceType.HideClear
          ) {
            const hide = influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear;
            section.hidden = this.isHidden(section.hiddenBy, field.value, influence.condition, influence.notNull, hide, null, null, form);

            if (section.hidden && influence.type === InfluenceType.HideClear) {
              for (const sub of section.subsections) {
                await this.clearSubsectionValues(sub, form, body, isHistory); // ?
              }
            }
          } else if (influence.type === InfluenceType.Disable) {
            section.disabled = this.isDisabled(section.disabledBy, field.value, influence.condition, influence.notNull, null, null, form);
          }
        }
      } else if (influence.createNewTableName) {
        let table = this.getTable(influence.createNewTableName, form);

        if (table && table.rendered !== false) {
          const hide = influence.type === InfluenceType.Hide || influence.type === InfluenceType.HideClear;
          table.createNewHidden = this.isHidden(
            table.createNewHiddenBy,
            field.value,
            influence.condition,
            influence.notNull,
            hide,
            null,
            null,
            form
          );
        }
      }
    }
  }

  private getAllInfluencedSubsections(subsectionName: string, subsection: Subsection, parent: Subsection | Section, form: Form) {
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

  private async clearSubsectionValues(subsection: Subsection, form: Form, body, isHistory: boolean) {
    if (subsection.fields) {
      for (const fld of subsection.fields) {
        fld.value = fld.defaultValue !== undefined ? fld.defaultValue : null;
        await this.influenceHelper(subsection, subsection, form, body, isHistory, true);
      }
    }
    if (subsection.subsections) {
      for (const sub of subsection.subsections) {
        await this.clearSubsectionValues(sub, form, body, isHistory);
      }
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

  private findFldVal(fieldName: string, container, parent, form: Form) {
    let val = this.formValues[fieldName];
    const keys = Object.keys(this.formValues);

    if (val == undefined && !keys.includes(fieldName)) {
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
      if (object[key] && !object[key].length && Object.keys(object[key]).length > 0 && typeof object[key] === "object") {
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

  substituteParams(condition, body) {
    if (typeof condition === "string" && body) {
      for (const key in body) {
        if (condition.includes(key)) {
          const regexp = new RegExp(`${key}`, "g");
          const keyVal = !isNaN(body[key]) ? +body[key] : body[key];
          condition = condition.replace(regexp, keyVal);
        }
      }
    }
    return condition;
  }

  private async parseRegixData(regixData) {
    const map = [
      { regixField: "PersonNames.FirstName", neispuoField: "firstName" },
      { regixField: "PersonNames.SurName", neispuoField: "middleName" },
      { regixField: "PersonNames.FamilyName", neispuoField: "lastName" },
      { regixField: "BirthDate", neispuoField: "birthDate" },
      { regixField: "Gender.GenderName", neispuoField: "gender", mappingRoute: "assets/json/fake-data/gender-mapping.json" },
      { regixField: "Nationality.NationalityName", neispuoField: "nationalityID", mappingRoute: "/data/get/mapNationality" },
      { regixField: "TemporaryAddress.DistrictCode", neispuoField: "currentRegion" },
      { regixField: "TemporaryAddress.SettlementCode", neispuoField: "currentMunicipality", mappingRoute: "/data/get/mapMunicipality" },
      { regixField: "TemporaryAddress.SettlementCode", neispuoField: "currentTown" },
      { regixField: "TemporaryAddress.LocationName", neispuoField: "currentAddress" },
      { regixField: "PermanentAddress.DistrictCode", neispuoField: "permanentRegion" },
      { regixField: "PermanentAddress.SettlementCode", neispuoField: "permanentMunicipality", mappingRoute: "/data/get/mapMunicipality" },
      { regixField: "PermanentAddress.SettlementCode", neispuoField: "permanentTown" },
      { regixField: "PermanentAddress.LocationName", neispuoField: "permanentAddress" }
    ];

    const mappedRes: any = {};

    for (let fld of map) {
      const val = eval("regixData?." + fld.regixField.replace(/\./g, "?."));
      if (val) {
        let buildingNumber, entrance, floor, apartment, location;
        if (fld.regixField.includes("LocationName")) {
          const key = fld.regixField.split(".")[0];
          location = regixData[key]["LocationName"];
          buildingNumber = regixData[key]["BuildingNumber"] ? " № " + regixData[key]["BuildingNumber"] : "";
          entrance = regixData[key]["Entrance"] ? ", Вх. " + regixData[key]["Entrance"] : "";
          floor = regixData[key]["Floor"] ? ", Ет. " + regixData[key]["Floor"] : "";
          apartment = regixData[key]["Apartment"] ? ", Ап. " + regixData[key]["Apartment"] : "";
          mappedRes[fld.neispuoField] = location + buildingNumber + entrance + floor + apartment;
        } else {
          mappedRes[fld.neispuoField] = await this.getMappedValue(val, fld.mappingRoute);
        }
      }
    }

    return mappedRes;
  }

  private async getMappedValue(regixVal, mappingRoute) {
    if (mappingRoute) {
      let res = await this.getDynamicNomenclature(mappingRoute, false, { filterValue: regixVal }).toPromise();

      if (res && res.length) {
        return res.find(row => (row.regixValue + "").toUpperCase() === (regixVal + "").toUpperCase())?.neispuoValue || null;
      } else {
        return null;
      }
    } else {
      regixVal && !isNaN(+regixVal) && (regixVal = +regixVal);
      return regixVal;
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

  private transformBody(body) {
    const newBody = {};
    for (let key in body) {
      if (body[key] !== "null") {
        newBody[key] = body[key];
      }
    }

    return newBody;
  }
}
