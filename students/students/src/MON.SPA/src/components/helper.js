import { AuditModule, Severity, Months } from "@/enums/enums";

import CONSTANTS from '@/common/constants';
import { DocumentModel } from '@/models/documentModel.js';
import errorService from '@/services/error.service.js';
import i18n from '@/language/language';
import moment from 'moment';
import notifier from '@/plugins/notifier/notifier';
import store from '@/store/index';
import groupBy from 'lodash.groupby';


export default {
  onFileSelected(id, fileName, fileType, component) {
    component.$api.document
      .getById(id)
      .then((response) => {
        const blob = new Blob([response.data], { type: fileType });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
      })
      .catch((error) => {
        notifier.error('',  i18n.t('errors.fileRead'));
        console.log(error);
      });
  },
  async checkForLocalServer(component) {
    let hasLocalServer = false;
    await component.$api.localServer
      .version()
      .then(() => {
        hasLocalServer = true;
      })
      .catch(() => {
        hasLocalServer = false;
      });
    return hasLocalServer;
  },
  getFileContentFromApi(id, fileName, component) {
    component.$api.document
      .getById(id)
      .then((response) => {
        if(response && response.data) {
          const file = new File([response.data], fileName, {type: response.data.type});
          component.form.files.push(file);
          component.form.previousFiles.push(file);
        }
      })
      .catch((error) => {
        notifier.error('',  i18n.t('errors.fileRead'));
        console.log(error);
      });
  },
  getFileContent(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      const fileContentToReturn = new DocumentModel({
        noteFileName: file.name,
        noteFileType: file.type,
        size: file.size
      });

      reader.readAsArrayBuffer(file);

      reader.onload = function() {
        const array = new Uint8Array(this.result);
        const byteArray = [];

        Object.keys(array).forEach((key) =>{
            const byteForKey = array[key];
            byteArray.push(byteForKey);
        });

        fileContentToReturn.noteContents = byteArray;

        resolve(fileContentToReturn);
      };

      reader.onerror = reject;
    });
  },
  blobToBase64(blob) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => {
        var dataUrl = reader.result;
        var base64 = dataUrl.split(',')[1];
        resolve(base64);
      };
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });
  },
  // formatDate(date) {
  //   if(!date) {
  //     return null;
  //   }

  //   const [year, month, day] = date.split('-');
  //   return `${day}/${month}/${year}`;
  // },
  // parseDate(date) {
  //   if(!date) {
  //     return null;
  //   }

  //   const [month, day, year] = date.split('/');
  //   return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
  // },
  async validate(component, dropdownModels) {
    component.$v.$touch();

    let anyError = this.validateDropdowns(dropdownModels);

    if (!component.$v.$invalid && !anyError) {
      var result = await component.$refs.confirm.open(component.$t('common.save'), component.$t('common.confirm'));
      return result;
    }
  },
  getRequiredValidationMessage(field, isDropdown) {
    if (!field.$dirty) {
      return '';
    }

    const dropdwonModelInvalid = isDropdown === true && (!field.$model || !field.$model.text);

    if(!field.required || dropdwonModelInvalid) {
      return  i18n.t('validation.requiredField');
    }
  },
  institutionChange(component, value) {
    if(typeof value === 'object' && value !== null) {
      component.$refs.institutionDropdown.optionsList = [];
    }
    else if(component.institutionBlur) {
      component.institutionBlur();
    }
  },
  getYear() {
    const today = new Date();

    return today.getMonth() >= 9 && today.getDay() >= 1
      ? today.getFullYear()
      : today.getFullYear() - 1;
  },
  getMonth() {
    const today = new Date();
    return today.getMonth() + 1;
  },
  getFirstLetters(str) {
    if(!str) return undefined;
    const strArr = str.split(/[\s,]+/);
    if(!strArr) return undefined;

    if(strArr.length < 2) {
      return strArr[0].substring(0,2);
    }

    const result = strArr.reduce((acc, curr) => {
      return acc + (curr ? curr.charAt(0) : '');
    }, '');

    return result;
  },
  getAvatarText(str){
    const result = this.getFirstLetters(str);
    return result ? result.toUpperCase() : '';
  },
  validateDropdowns(dropdownObjects) {
    let errors = false;

    if(dropdownObjects) {
      for (let index = 0; index < dropdownObjects.length; index++) {
        const dropdownModel = dropdownObjects[index];
        if(!dropdownModel || !dropdownModel.text) {
          errors = true;
          break;
        }
      }
    }

    return errors;
  },
  authorize(to, from, next, model) {
    store.dispatch('authorize', model)
    .then((hasPermission) => {
      if(hasPermission) {
        next();
      } else {
        next('/errors/AccessDenied');
      }
    })
    .catch(() => {
      next('/errors/AccessDenied');
    });
  },
  authorizeForStudent(to, from, next, model) {
    const permissionsForStudent = store.getters.permissionsForStudent;
    let hasPermission = false;

    if(model && model.permissions) {
      hasPermission == Array.isArray(model.permissions)
        && permissionsForStudent
        && model.permissions.some(x => permissionsForStudent.includes(x));
    }

    if (model && model.permission && typeof model.permission === 'string') {
      hasPermission = permissionsForStudent
        && permissionsForStudent.includes(model.permission);
    }

    if(hasPermission) {
      next();
    } else {
      next('/errors/AccessDenied');
    }
  },
  isValidCustomFormattedDate(date, dateFormat, timeFormat) {
    if(!date) {
      return false;
    }

    const m = date.length === 10
      ? moment(date, `${dateFormat}`, true)
      : moment(date, `${dateFormat}T${timeFormat}`, true);
    return m.isValid();
  },
  isValidIsoFormattedDate(date) {
    if(!date) {
      return false;
    }

    const m = date.length === 10
      ? moment(date, "YYYY-MM-DD", true)
      : (date.length === 22
        ? moment(date, "YYYY-MM-DDTHH:mm:ss.SS", true)
        : ( date.length === 27
          ? moment(date, "YYYY-MM-DDTHH:mm:ss.SSSSSSS", true)
          : moment(date, "YYYY-MM-DDTHH:mm:ss", true)
          )
        );
    return m.isValid();
  },
  getDateSeparator(format) {
    let dateCharSeparator = '/';

    if(format && format === CONSTANTS.DATEPICKER_FORMAT_DOTS) {
      dateCharSeparator = '.';
    }

    return dateCharSeparator;
  },
  formatDate (date, format) {
    if(!date) {
      return null;
    }

    if(!this.isValidIsoFormattedDate(date)) {
      return date;
    }

    const dateSeparator = this.getDateSeparator(format);
    const [year, month, day] = date.split('-');

    return `${ day.substring(0,2).padStart(2, '0')}${dateSeparator}${month.substring(0,2).padStart(2, '0')}${dateSeparator}${year}`;
  },
  formatFromIsoDate(date, format) {
    if(!date) return date;

    const m = date.length === 10
      ? moment(date, "YYYY-MM-DD", true)
      : (date.length === 22
        ? moment(date, "YYYY-MM-DDTHH:mm:ss.SS", true)
        : ( date.length === 27
          ? moment(date, "YYYY-MM-DDTHH:mm:ss.SSSSSSS", true)
          : moment(date, "YYYY-MM-DDTHH:mm:ss", true)
          )
      );

    if(!m.isValid) return date;
    const dateSeparator = this.getDateSeparator(format);
    return m.format(`DD${dateSeparator}MM${dateSeparator}YYYY`);
  },
  parseDateToIso(date, format) {
    if(!date) {
      return null;
    }

    if(this.isValidIsoFormattedDate(date)) {
      return date;
    }

    const dateSeparator = this.getDateSeparator(format);

    const dateFormat = format ? format : CONSTANTS.DATEPICKER_FORMAT;
    const timeFormat = CONSTANTS.DISPLAY_TIME_FORMAT;

    if(this.isValidCustomFormattedDate(date, dateFormat, timeFormat)) {
      const [day, month, year] = date.split(dateSeparator);
      return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
    }

    const [month, day, year] = date.split(dateSeparator);
    return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
  },
  groupBy(xs, key) {
    return xs.reduce(function(rv, x) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  },
  groupByProperties( array , f )
  {
    var groups = {};
    array.forEach( function( o )
    {
      var group = JSON.stringify( f(o) );
      groups[group] = groups[group] || [];
      groups[group].push( o );
    });
    return Object.keys(groups).map( function( group )
    {
      return groups[group];
    });
  },
  groupByArray(xs, key) {
    return xs.reduce(function (rv, x) { let v = key instanceof Function ? key(x) : x[key]; let el = rv.find((r) => r && r.key === v); if (el) { el.values.push(x); } else { rv.push({ key: v, values: [x] }); } return rv; }, []);
  },

  getColsProp(field) {
    if(!field || !field.cols) return false;

    return field.cols;
  },
  getXlProp(field) {
    if(!field || !field.xl) return false;

    return field.xl;
  },
  getLgProp(field) {
    if(!field || !field.ld) return false;

    return field.ld;
  },
  getMdProp(field) {
    if(!field || !field.md) return false;

    return field.md;
  },
  getSmProp(field) {
    if(!field || !field.sm) return false;

    return field.sm;
  },
  setDropdownOption(value, dropdownName, model, refs) {
    if(typeof value === 'string') {
      const option = refs[dropdownName].optionsList.filter(el => el.text === value)[0];
      if(option) {
        model[dropdownName] = option;
      }
    }
  },
  base64ToByteArray(base64) {
    var binary_string = window.atob(base64);
    var len = binary_string.length;
    var bytes = new Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes;
  },
  byteArrayToBase64(data) {
    var base64String = window.btoa(data);
    return base64String;
  },
  utf8ToBase64( str ) {
    return window.btoa(unescape(encodeURIComponent( str )));
  },
  base64ToUtf8( str ) {
      return decodeURIComponent(escape(window.atob( str )));
  },
  parseError(error) {
    if(!error) return { messsage: '', errors: ''};

    // eslint-disable-next-line no-prototype-builtins
    if (error.hasOwnProperty('data') && error.data.hasOwnProperty('isError')) {
      const errors = error.data.errors
        ? error.data.errors.map((error) => {
          return error.message;
        })
        : '';

      return { message: error.data.message, errors, clientNotificationLevel: error.data.clientNotificationLevel || 'error' };
    }

    // eslint-disable-next-line no-prototype-builtins
    if (error.hasOwnProperty('data') && error.data.hasOwnProperty('errorStr')) {
      return { message: error.data.message, errors: error.data.errorStr, clientNotificationLevel: error.data.clientNotificationLevel || 'error' };
    }

    // eslint-disable-next-line no-prototype-builtins
    if (error.hasOwnProperty('response') && error.response.hasOwnProperty('data') && error.response.data.message) {
      return { message: error.response.data.message, errors: '', clientNotificationLevel: error.response.data.clientNotificationLevel || 'error' };
    }

    return { message: error.message, errors: '', clientNotificationLevel: 'error' };
  },
  isJsonParsable(string) {
    try {
      JSON.parse(string);
    } catch (e) {
      return false;
    }
    return true;
  },
  logError(message, trace, additionalInformation) {
    if (!this.isJsonParsable(message)) {
      message = JSON.stringify(message);
    }

    if (!this.isJsonParsable(trace)) {
      trace = JSON.stringify(trace);
    }

    if (!this.isJsonParsable(additionalInformation)) {
      additionalInformation = JSON.stringify(additionalInformation);
    }

    errorService.add({ severity: Severity.Error, moduleId: AuditModule.Students, message: message, trace: trace, additionalInformation: additionalInformation }).catch(() => { });
  },
  clearArray(items) {
    if(Array.isArray(items) && items.length > 0) {
      items.splice(0);
    }
  },
  getValidationErrorsDetails(form) {
    const errors = [];
    if (!form) return errors;

    if (!!form.$data?.inputs === true) {
      form.$data.inputs.filter(x => x.valid === false).forEach(x => {
        errors.push({
          label: x.$options?.propsData?.label,
          errors: x.errorBucket
        });
      });
    }

    return errors;
  },
  getMonthName(month){
    return Months.find(x => x.value === month).monthName;
  },

  fixupLodAssessmentCreateOrUpdateModel(model) {
    if(!model) return;

    if(model && model.length > 0) {
      model.forEach(part => {
          if(part.subjectAssessments && part.subjectAssessments.length > 0) {
            part.subjectAssessments.forEach(s => {
              if(s.firstTermAssessments.length === 0) s.firstTermAssessments.push({ gradeId: null });
              if(s.secondTermAssessments.length === 0) s.secondTermAssessments.push({ gradeId: null });
              if(s.annualTermAssessments.length === 0) s.annualTermAssessments.push({ gradeId: null });
              if(s.firstRemedialSession.length === 0) s.firstRemedialSession.push({ gradeId: null });
              if(s.secondRemedialSession.length === 0) s.secondRemedialSession.push({ gradeId: null });
              if(s.additionalRemedialSession.length === 0) s.additionalRemedialSession.push({ gradeId: null });

              if(s.modules && s.modules.length > 0) {
                s.modules.forEach(m => {
                  if(m.firstTermAssessments.length === 0) m.firstTermAssessments.push({ gradeId: null });
                  if(m.secondTermAssessments.length === 0) m.secondTermAssessments.push({ gradeId: null });
                  if(m.annualTermAssessments.length === 0) m.annualTermAssessments.push({ gradeId: null });
                  if(m.firstRemedialSession.length === 0) m.firstRemedialSession.push({ gradeId: null });
                  if(m.secondRemedialSession.length === 0) m.secondRemedialSession.push({ gradeId: null });
                  if(m.additionalRemedialSession.length === 0) m.additionalRemedialSession.push({ gradeId: null });
                });
              }
            });
          }
      });
    }
  },
  groupGradeOptions(grades) {
    if(!grades) return [];

    let options = [];
    const groups = groupBy(grades, 'gradeTypeId');

    for (const gradeTypeId in groups) {
      if(Number(gradeTypeId) !== 1) {
        options.push({
          header: groups[gradeTypeId][0].gradeTypeName,
          gradeTypeId: Number(gradeTypeId)
        });
      }

      options = [...options, ...groups[gradeTypeId].map(x => {
        return { value: x.value, text: x.text, gradeTypeId: x.gradeTypeId, gradeTypeName: x.gradeTypeName };
      })];
      options.push({ divider: true });
    }

    return options;
  },
  groupSubjectTypeOptions(subjects) {
    if(!subjects) return [];

    let options = [];
    const groups = groupBy(subjects, 'partId');

    for (const partId in groups) {
      options.push({
        header:  groups[partId][0].partName,
        partId: Number(partId)
      });
      options = [...options, ...groups[partId].map(x => {
        return { value: x.value, text: x.text, partId: x.partId, partName: x.partName, isValid: x.isValid };
      })];
      options.push({ divider: true });
    }

    return options;
  },
  parseJwt (token) {
      if (token){
      var base64Url = token.split('.')[1];
      var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));

      return JSON.parse(jsonPayload);
    }
    else{
      return null;
    }
  },
  getVersion(version){
    if (version){
      let versionParts = version.split('.');
      if (versionParts.length == 3)
      {
        return Number(versionParts[0]) * 10000 + Number(versionParts[1]) * 100 + Number(versionParts[2]);
      }
      else{
        return null;
      }
    }
    else{
      return null;
    }
  },
  boolTryParse(val) {
    try {
      return val.toString().toLowerCase() === 'true';
    } catch (error) {
      return false;
    }
  },
  extractFileNameFromDisposition(disposition) {
    if (!disposition) return null;
    let filename = null;
    // RFC 5987 / 6266 filename* takes precedence
    let match = /filename\*=UTF-8''([^;]+)/i.exec(disposition);
    if (match && match[1]) {
      try { filename = decodeURIComponent(match[1]); } catch (_) { filename = match[1]; }
    } else {
      // Fallback standard filename="..."
      match = /filename="?([^";]+)"?/i.exec(disposition);
      if (match && match[1]) {
        filename = match[1];
      }
    }

    if (filename) {
      return filename.replace(new RegExp('[\\\\/]', 'g'), '_');
    }
    return null;
  }
};
