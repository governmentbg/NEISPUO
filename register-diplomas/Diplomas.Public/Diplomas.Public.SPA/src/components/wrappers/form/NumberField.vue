<template>
  <v-text-field
    type="number"
    :value="value"
    outlined
    disabled
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'NumberField',
  props: {
    value: {
      type: Number,
      default() {
        return null;
      }
    }
  }
};
</script>