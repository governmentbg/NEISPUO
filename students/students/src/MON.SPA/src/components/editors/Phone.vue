<template>
  <v-row 
    v-if="model"
  >
    <v-col
      cols="12"
      md="3"
      sm="6"
    >
      <v-text-field
        v-model="model.contactKind"
        :disabled="disabled"
        :readonly="readonly"
        :label="$t('phone.contactKind')"
        @input="v => $emit('input', model)"
      />
    </v-col>
    <v-col
      cols="12"
      md="3"
      sm="6"
    >
      <v-text-field
        v-model="model.type"
        :disabled="disabled"
        :readonly="readonly"
        :label="$t('phone.type')"
        @input="v => $emit('input', model)"
      />
    </v-col>
    <v-col
      cols="12"
      md="3"
      sm="6"
    >
      <v-text-field
        v-model="model.code"
        :disabled="disabled"
        :readonly="readonly"
        :label="$t('phone.code')"
        @input="v => $emit('input', model)"
      />
    </v-col>
    <v-col
      cols="12"
      md="3"
      sm="6"
    >
      <v-text-field
        v-model="model.number"
        :disabled="disabled"
        :readonly="readonly"
        :label="$t('phone.number')"
        @input="v => $emit('input', model)"
      />
    </v-col>
  </v-row>
</template>

<script>
export default {
  name: 'Phone',
  props: {
    value: {
      type: Object,
      default() {
        return {};
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    readonly: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.value,
    };
  },
};
</script>