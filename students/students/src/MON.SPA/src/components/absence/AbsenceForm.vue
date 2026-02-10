<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-row>
      <v-col
        cols="12"
        md="6"
      >
        <v-text-field
          v-model="value.excused"
          :label="$t('absence.excused')"
          type="number"
          :rules="[$validator.min(0), $validator.max(999)]"
        />
      </v-col>
      <v-col
        cols="12"
        md="6"
      >
        <v-text-field
          v-model="value.unexcused"
          :label="$t('absence.unexcused')"
          type="number"
          :rules="[$validator.min(0), $validator.max(999)]"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
export default {
  name: "AbsenceForm",
  props: {
    value: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    },
  },
  methods: {
    validate() {
      // Използва се в EditAbsence.vue
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
  }
};
</script>
