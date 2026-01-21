<template>
  <v-skeleton-loader
    :type="type"
  />
</template>

<script>
export default {
  name: 'LoaderWrapper',
  props: {
    type: {
      type: String,
      default() {
        return 'list-item-two-line';
      }
    }
  }
};
</script>