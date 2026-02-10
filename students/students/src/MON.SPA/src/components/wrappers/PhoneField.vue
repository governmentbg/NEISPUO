<template>
  <v-text-field
    v-model="model"
    type="phone"
    :label="$t('common.phoneNumber')"
    :rules="required ? [$validator.required()] : []"
    :class="required ? 'required' : ''"
    :prepend-icon="prependIcon"
    autocomplete="phone"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'PhoneFieldWrapper',
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
        return 'mdi-phone';
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
