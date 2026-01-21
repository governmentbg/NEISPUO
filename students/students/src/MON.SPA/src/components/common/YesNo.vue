<template>
  <span>
    {{ text }}
  </span>
</template>

<script>
export default {
  name: 'YesNo',
  props:{
    value: {
      type: Boolean,
      default() {
        return undefined;
      }
    },
    inverse: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      text: ''
    };
  },
  watch: {
    value() {
      this.init();
    }
  },
  mounted() {
    this.init();
  },
  methods: {
    init() {
      if(this.inverse) {
        this.text = this.value === true 
          ? this.$t('common.no')
          : this.value == false ? this.$t('common.yes') : '';
        return;
      }

    this.text = this.value === true 
      ? this.$t('common.yes')
      : this.value == false ? this.$t('common.no') : '';
    }
  }
};
</script>