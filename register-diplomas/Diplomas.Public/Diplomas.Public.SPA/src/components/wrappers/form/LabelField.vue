<template>
  <p
    :class="textClass"
  >
    {{ staticText }}
  </p>
</template>

<script>
export default {
  name: 'LabelField',
  props: {
    textType: {
      type: String,
      default() {
        return undefined;
      }
    },
    staticText: {
      type: String,
      default() {
        return undefined;
      }
    }
  },
  computed: {
    textClass() {
      return `text-${this.textType}`;
    }
  }
};
</script>