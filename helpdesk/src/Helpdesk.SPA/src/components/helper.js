import i18n from '@/language/language';
import store from '@/store/index';
import moment from 'moment';
import CONSTANTS from '@/common/constants';
import { DocumentModel } from '@/models/documentModel.js';

export default {
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
  groupByArray(xs, key) { return xs.reduce(function (rv, x) { let v = key instanceof Function ? key(x) : x[key]; let el = rv.find((r) => r && r.key === v); if (el) { el.values.push(x); } else { rv.push({ key: v, values: [x] }); } return rv; }, []); },

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
  getFileContent(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      const fileContentToReturn = new DocumentModel({
        noteFileName: file.name,
        noteFileType: file.type,
      });

      reader.readAsArrayBuffer(file);

      reader.onload = function () {
        const array = new Uint8Array(this.result);
        const byteArray = [];

        Object.keys(array).forEach((key) => {
          const byteForKey = array[key];
          byteArray.push(byteForKey);
        });

        fileContentToReturn.noteContents = byteArray;

        resolve(fileContentToReturn);
      };

      reader.onerror = reject;
    });
  },
};
