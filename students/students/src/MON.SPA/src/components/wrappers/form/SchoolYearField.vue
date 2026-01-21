<template>
  <school-year-selector
    v-model="model"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Constants from '@/common/constants';

export default {
  name: 'SchoolYearField',
  components: {
    SchoolYearSelector
  },
  props: {
    value: {
      type: Number,
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
    }
  },
  data() {
    return {
      model: this.value
    };
  }
};
</script>
