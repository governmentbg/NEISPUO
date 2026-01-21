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
  name: 'TextField',
  props: {
    value: {
      type: String,
      default() {
        return null;
      }
    }
  }
};
</script>