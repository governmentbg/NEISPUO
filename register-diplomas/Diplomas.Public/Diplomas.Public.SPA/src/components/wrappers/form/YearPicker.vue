<template>
  <v-text-field
    type="text"
    :value="value"
    disabled
    outlined
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'YearPicker',
  props: {
    value: {
      type: [String, Number],
      default() {
        return null;
      }
    }
  }
};
</script>