<template>
  <div>
    <v-text-field
      type="text"
      :value="value"
      outlined
      disabled
      v-bind="$attrs"
      v-on="$listeners"
    />
  </div>
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
    }
  },
  computed: {
    formattedDate() {
      if(!this.value) {
        return this.value;
      }
      
      if(this.$moment(this.value, Constants.DATEPICKER_FORMAT).format(Constants.DATEPICKER_FORMAT) === this.value) {
        return this.value;
      }
      else {
        return this.$moment(this.value).format(Constants.DATEPICKER_FORMAT);
      }
    }
  }
};
</script>