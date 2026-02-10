<template>
  <v-text-field
    type="text"
    :value="calcValue"
    outlined
    disabled
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'SelectList',
  props: {    
    value: {
      type: [String, Object],
      default() {
        return null;
      }
    }
  },
  computed: {
    calcValue() {
      if(!this.value) return '';

      if(Object.prototype.hasOwnProperty.call(this.value, 'text')) {
        return this.value.text ?? '';
      } else {
        return this.value;
      }
    }
  }
};
</script>