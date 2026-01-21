<template>
  <v-text-field
    v-model="model"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    v-bind="$attrs"
    v-on="$listeners"
  >
    <template
      v-for="(_, slot) of $scopedSlots"
      v-slot:[slot]="scope"
    >
      <slot
        :name="slot"
        v-bind="scope"
      />
    </template>
    <template #append-outer>
      <slot name="append-outer" />
    </template>
    <template #append>
      <slot name="append" />
    </template>
    <template #prepend>
      <slot name="prepend" />
    </template>
    <template #prepend-inner>
      <slot name="prepend-inner" />
    </template>
  </v-text-field>
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'TextField',
  props: {
    value: {
      type: [String, Number],
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
  },
  watch: {
    value(val) {
      this.model = val;
    },
  }
};
</script>
