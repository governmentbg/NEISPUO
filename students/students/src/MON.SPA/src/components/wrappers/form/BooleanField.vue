<template>
  <v-checkbox 
    v-model="model"
    v-bind="$attrs"
    v-on="$listeners"
    @change="$emit('input', $event)"
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