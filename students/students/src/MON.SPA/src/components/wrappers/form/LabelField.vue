<template>
  <p
    v-bind="$attrs"
    v-on="$listeners"
  >
    {{ staticText }}
  </p>
</template>

<script>
export default {
  name: 'LabelField',
  props: {
    staticText: {
      type: String,
      default() {
        return undefined;
      }
    }
  }
};
</script>
