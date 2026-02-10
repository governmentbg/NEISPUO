<template>
  <v-item-group class="v-btn-toggle custom-btn-group">
    <slot />
  </v-item-group>
</template>

<script>
export default {
  name: 'ButtonGroup'
};
</script>

<!-- <style lang="scss">
    .custom-btn-group > * {
      opacity: 0.8 !important;
      border-style: solid !important;
      border-width: thin !important;
      border-color: rgba(0, 0, 0, 0.1) !important;
      border-radius: 0px !important;
      box-shadow: none !important;
    }
</style> -->
