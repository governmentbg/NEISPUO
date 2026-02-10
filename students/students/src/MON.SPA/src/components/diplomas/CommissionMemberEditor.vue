<template>
  <v-row
    v-if="value"
    dense
  >
    <v-col
      cols="12"
      md="2"
    >
      <number-selector
        v-model="value.position"
        :min="0"
        :max="100"
        :label="$t('diplomas.template.position')"
        :rules="[$validator.required(true)]"
        class="required"
      />
    </v-col>
    <v-col
      cols="12"
      md="5"
    >
      <v-text-field
        v-model="value.fullName"
        :label="$t('diplomas.commission.name')"
        :rules="[$validator.required(), $validator.maxLength(1000)]"
        class="required"
        clearable
      />
    </v-col>
    <v-col
      cols="12"
      md="5"
    >
      <v-text-field
        v-model="value.fullNameLatin"
        :label="$t('diplomas.commission.nameLatin')"
        :rules="[$validator.maxLength(1000)]"
        clearable
      />
    </v-col>
  </v-row>
</template>

<script>

export default {
  name: 'CommissonMemberEditorComponent',
  props: {
    value: {
      type: Object,
      required: true,
    },
  },
};
</script>
