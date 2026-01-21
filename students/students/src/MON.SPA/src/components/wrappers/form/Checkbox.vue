<template>
  <v-checkbox
    v-model="model"
    v-bind="$attrs"
    :ripple="ripple"
    v-on="$listeners"
    @change="$emit('input', $event)"
  >
    <template v-slot:label>
      <button
        style="width: 100%; text-align: left"
        @click.stop="onCustomLabelClick"
      >
        <span>{{ label }}</span>
      </button>
    </template>
  </v-checkbox>
</template>

<script>
export default {
  name: 'CheckboxWrapper',
  props: {
    value: undefined,
    label: {
      type: String,
      default() {
        return undefined;
      }
    },
    ripple: {
      type: Boolean,
      default() {
        return true;
      }
    }
  },
  data() {
    return {
      model: this.value,
    };
  },
  methods: {
    onCustomLabelClick(e) {
      // Click важи само върху Input-a, но не и върху Label-а.
      e.preventDefault();
    }
  }
};
</script>
