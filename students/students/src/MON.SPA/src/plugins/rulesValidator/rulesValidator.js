import i18n from '@/language/language';

export class RulesValidaror {
  required(allowZero) {
    return (value) => Array.isArray(value)
      ? value.length > 0 || i18n.t('validation.requiredField')
      : (allowZero === true
        ? (!isNaN(value) && value !== null && value !== undefined) || i18n.t('validation.requiredField')
        : !!value || i18n.t('validation.requiredField'));
  }

  minLength(count) {
      return (value) => (value || '').toString().length >= count || i18n.t('validation.minLength', { count });
  }

  maxLength(count) {
      return (value) => (value || '').toString().length <= count || i18n.t('validation.maxLength', { count });
  }

  min(number) {
    return (value) => {
      const pattern = /^-?\d*\.?\d+$/;
      if (value && !pattern.test(value)) {
        return i18n.t('validation.onlyNumbersAllowed');
      }

      const val = Number(value), min = Number(number);
      return value === null || value === '' || isNaN(val) || isNaN(min) || val >= min || i18n.t('validation.minNumber', { number: min });
    };
  }

  max(number) {
    return (value) => {
      const pattern = /^-?\d*\.?\d+$/;
      if (value && !pattern.test(value)) {
        return i18n.t('validation.onlyNumbersAllowed');
      }

      const val = Number(value), max = Number(number);
      return value === null || value === '' || isNaN(val) || isNaN(max) || val <= max || i18n.t('validation.maxNumber', { number: max });
    };
  }

  email() {
      return (value) => {
        const pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return !value || pattern.test(value) || i18n.t('validation.emailNotValid');
     };
  }

  numbers() {
    return (value) => {
      const pattern = /^\d+$/;
      return !value || pattern.test(value) || i18n.t('validation.onlyNumbersAllowed');
    };
  }

  decimalNumber(params) {
    return (value) => {
      const pattern = new RegExp(`^\\d${params?.integerNumbersCount ? `{0,${params?.integerNumbersCount}}` : '*'}\\.?\\d${params?.realNumbersCount ? `{0,${params?.realNumbersCount}}` : '*'}$`);
      return !value || pattern.test(value) || i18n.t('validation.decimalNumberFormat', { numberOfDigitsBefore: params?.integerNumbersCount, numberOfDigitsAfter: params?.realNumbersCount });
    };
  }

  isInOptionsList(option) {
    return !option || !option.model || option.optionsList.filter(el => el.value === option.model.value).length > 0 || i18n.t('validation.optionNotExisting');
  }

  alphanumeric() {
    return (value) => {
      const pattern = /^[A-Za-z0-9]+$/;
      return !value || pattern.test(value) || i18n.t('validation.alphanumericNotValid');
    };
  }

  regex(pattern, errorText) {
    return (value) => {
      if (!value || value === null || value === undefined || value === '') return true;
      const regex = new RegExp(pattern);
      return regex.test(value) || errorText || i18n.t('validation.invalidFormat');
    };
  }

  validate(el) {
    if (!el) return undefined;

    let hasValidationErrors = false;

    for(const key in el.$refs) {
      const ref = el.$refs[key];
      if(!ref) {
        continue;
      }

      if (ref.isCustomDatepicker) {
        const datePicker = ref.$children && ref.$children[0].$children
          ? ref.$children[0].$children[0]
          : undefined;

          if(datePicker && !!datePicker.validate) {
            const hasError = !datePicker.validate(true); // Returns true if successful and false if not
            hasValidationErrors = hasValidationErrors || hasError;
          }
      } else if(ref.isCustomCombo) {
        const combo = ref.$children
          ? ref.$children[0]
          : undefined;

          if(combo && !!combo.validate) {
            const hasError = !combo.validate(true); // Returns true if successful and false if not
            hasValidationErrors = hasValidationErrors || hasError;
          }
      } else {
        if(ref.validate) {
          const hasError = !ref.validate(true); // Returns true if successful and false if not
          hasValidationErrors = hasValidationErrors || hasError;
          continue;
        }
      }
    }

    return hasValidationErrors;
  }

  maxFileSize(sizeInMb) {
    return (value) => {
      if (!value) return true;
      const fileSize = value.size;
      const isSmaller = fileSize < (sizeInMb * 1024 * 1024);

      return isSmaller || `Файлът трябва да е с големина по-малка от ${sizeInMb} MB!`;
    };
  }

  reset(el) {
    if (!el) return;

    for(const key in el.$refs) {
      const ref = el.$refs[key];

      if (ref.isCustomDatepicker) {
        const datePicker = ref.$children && ref.$children[0].$children
          ? ref.$children[0].$children[0]
          : undefined;

          if(datePicker && !!datePicker.reset) {
            datePicker.reset();
          }
      } else if(ref.isCustomCombo) {
        const combo = ref.$children
          ? ref.$children[0]
          : undefined;

          if(combo && !!combo.reset) {
            combo.reset();
          }
      } else {
        if (!ref.reset) continue;

        ref.reset();
      }
    }
  }
}

export default new RulesValidaror();
