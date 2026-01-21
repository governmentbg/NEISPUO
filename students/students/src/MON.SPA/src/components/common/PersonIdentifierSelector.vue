<template>
  <v-row>
    <v-col
      cols="12"
      sm="4"
      xs="12"
    >
      <v-select
        id="personalIdType"
        v-model="personalIdTypeModel"
        :label="$t('createStudent.pinType')"
        :items="pinTypeOptions"
        :disabled="disabled"
        single-line
        @input="$emit('update:personalIdType', $event)"
      />
    </v-col>
    <v-col
      cols="12"
      sm="8"
      xs="12"
    >
      <v-text-field
        v-model="personalIdModel"
        :disabled="disabled || personalIdTypeModel.value === -1"
        :label="$t('createStudent.pin')"
        clearable
        :rules="rules"
        @input="$emit('update:personalId', $event)"
      />
    </v-col>
  </v-row>
</template>

<script>
export default {
  name: 'PersonIdentifierSelector',
  props: {
    personalIdType: {
      type: Number,
      default() {
        return undefined;
      }
    },
    personalId: {
      type: Number,
      default() {
        return undefined;
      }
    },
    disabled:{
      type:Boolean,
      default() {
        return false;
      }
    },
    rules: {
      type: Array,
        required: false,
        default() {
          return [];
        }
    },
  },
  data() {
    return {
      personalIdTypeModel: this.personalIdType,
      personalIdModel: this.personalId,
      pinTypeOptions: [
        { value: -1, text: 'Без идентификатор'}
        , { value: 0, text: 'ЕГН'}
        , { value: 1, text: 'ЛНЧ'}
        , { value: 2, text: 'ИДН'}
      ]
    };
  }
};
</script>
