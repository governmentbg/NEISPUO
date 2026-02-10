import Vue from "vue";
import Moment from "moment";
import Constants from "@/common/constants";

Vue.filter("shortDate", function (value) {
    return Moment.utc(value).local().format("L");
});

Vue.filter("longDate", function (value) {
  return Moment.utc(value).local().format(Constants.DISPLAY_DATETIME_FORMAT);
});

Vue.filter("date", function (value, formatType = "long") {
    if (formatType === "short") {
        return Moment.utc(value).local().format("L");
    } else if (formatType === "long") {
        return Moment.utc(value).local().format("LL");
    } else {
        return "Грешен формат";
    }
});

Vue.filter("count", function (value) {
  return value.length;
});
