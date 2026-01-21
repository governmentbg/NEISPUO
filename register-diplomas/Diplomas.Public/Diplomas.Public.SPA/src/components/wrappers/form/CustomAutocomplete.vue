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
  name: 'CustomAutocomplete',
  props: {    
    value: {
      type: Object,
      default() {
        return null;
      }
    }
  },
  computed: {
    calcValue() {
      if(!this.value) return '';

      return this.value.text ? this.value.text : this.value;
    }
  }
};
</script>