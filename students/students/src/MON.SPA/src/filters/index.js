import Vue from 'vue';
import Moment from "moment";
import i18n from '@/language/language';

Vue.filter("shortDate", function (value) {
    return Moment(value).format("L");
});

Vue.filter("date", function (value, formatType = "long") {
    if (formatType === "short") {
        return Moment(value).format("L");
    } else if (formatType === "long") {
        return Moment(value).format("LL");
    } else {
        return "Грешен формат";
    }
});

Vue.filter("datetime", function (value, formatType= "DD/MM/YYYY HH:mm:ss") {
   return Moment(value).format(formatType);
});

Vue.filter("count", function (value) {
  return value.length;
});

Vue.filter("yesNo", function (value, inverse = false) {
  if (inverse) {
    return value === true
      ? i18n.t('common.no')
      : value == false ? i18n.t('common.yes') : '';
  } else {
    return value === true
      ? i18n.t('common.yes')
      : value == false ? i18n.t('common.no') : '';
  }
});
