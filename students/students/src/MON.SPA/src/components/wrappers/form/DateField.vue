<template>
  <date-picker
    ref="datePicker"
    v-model="model"
    :show-buttons="false"
    :scrollable="false"
    :no-title="true"
    :show-debug-data="false"
    :type="datePicketType"
    :format="format"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>


<script>
import Constants from '@/common/constants';

export default {
  name: 'DateField',
  props: {
    value: {
      type: [Date, String],
      default() {
        return null;
      }
    },
    datePicketType: {
      type: String,
      default() {
        return 'date'; // Дънамичните полета идва type (тип на полето), което обърква type свойството на date-picker-a (Determines the type of the picker - 'date' for date picker, 'month' for month picker)
      }
    },
    format: {
      type: String,
      default() {
        return null;
      }
    },
    persistentPlaceholder: {
      type: Boolean,
      default() {
        return Constants.FieldPersistentPlaceholder;
      }
    },
    outlined: {
      type: Boolean,
      default() {
        return Constants.FieldOutlined;
      }
    }
  },
  data() {
    return {
      model: this.formatDate(this.value)
    };
  },
  watch: {
    value() {
      this.model = this.formatDate(this.value);
    }
  },
  methods: {
    formatDate (date) {
      const formatString = this.format
         ? Constants.SqlToMomentFormatsMapper[this.format] ?? this.format
         : Constants.DATEPICKER_FORMAT;

      return this.$helper.formatDate(date, formatString);
    },
  }
};
</script>

<style lang="scss" scoped>
  ::v-deep {
    .v-input {
      padding-top: 0;
      margin-top: 0;
    }
  }
</style>
