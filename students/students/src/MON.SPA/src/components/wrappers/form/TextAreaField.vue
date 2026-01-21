<template>
  <v-textarea
    v-model="model"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    :prepend-icon="prependIcon"
    :rows="rows"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'TextAreaField',
  props: {
    value: {
      type: String,
      default() {
        return null;
      }
    },
    persistentPlaceholder: {
      type: Boolean,
      default() {
        return Constants.FieldPersistentPlaceholder;
      }
    },
    outlined: {
      type: Boolean,
      default() {
        return Constants.FieldOutlined;
      }
    },
    rows: {
      type: [Number, String],
      default() {
        return 3;
      }
    },
    prependIcon: {
      type: String,
      default() {
        return 'mdi-comment';
      }
    }
  },
  data() {
    return {
      model: this.value
    };
  },
  watch: {
    value(val) {
      this.model = val;
    },
  }
};
</script>
