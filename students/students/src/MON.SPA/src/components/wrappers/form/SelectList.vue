<template>
  <combo
    :value="value"
    :api="optionsUrl"
    :item-value="itemValue"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'SelectList',
  props: {
    value: {
      type: [Object, String],
      default() {
        return null;
      }
    },
    optionsUrl: {
      type: String,
      default() {
        return null;
      }
    },
    itemText: {
      type: [String, Array, Function],
      default() {
        return 'text';
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
    }
  },
  computed: {
    itemValue() {
      return !this.returnObject ? 'text': '';
    }
  }
};
</script>
