<template>
  <v-text-field
    v-model="model"
    type="email"
    :label="$t('common.email')"
    :rules="required ? [$validator.email(), $validator.required()] : [$validator.email()]"
    :class="required ? 'required' : ''"
    :prepend-icon="prependIcon"
    autocomplete="email"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'EmailFieldWrapper',
  props: {
    value: {
      type: String,
      default() {
        return null;
      }
    },
    required: {
      type: Boolean,
      default() {
        return false;
      }
    },
    prependIcon: {
      type: String,
      default() {
        return 'mdi-at';
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
