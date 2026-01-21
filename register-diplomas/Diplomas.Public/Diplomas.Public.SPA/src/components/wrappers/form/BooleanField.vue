<template>
  <v-checkbox 
    :value="value"
    disabled
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'BooleanField',
  props: {
    value: {
      type: Boolean,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      model: this.value
    };
  }
};
</script>