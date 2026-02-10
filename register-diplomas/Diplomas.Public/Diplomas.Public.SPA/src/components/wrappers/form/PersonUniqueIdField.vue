<template>
  <v-text-field
    type="text"
    :value="value"
    :label="personUniqueIdType"
    outlined
    disabled
  />
</template>

<script>
export default {
  name: 'PersonUniqueIdField',
  props: {
    value: {
      type: String,
      default() {
        return null;
      }
    },
    personUniqueIdType: {
      type: String,
      default() {
        return null;
      }
    }
  }
};
</script>