<template>
  <v-form
    v-if="value"
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <!-- {{ value }} -->
    <v-row dense>
      <v-col
        cols="12"
        md="4"
      >
        <v-text-field
          :value="value.basicClass"
          :label="$t('preSchool.headers.basicClass')"
          disabled
        />
      </v-col>
      <v-col
        cols="12"
        md="4"
      >
        <v-text-field
          :value="value.subject"
          :label="$t('preSchool.headers.subject')"
          disabled
        />
      </v-col>
      <v-col
        cols="12"
        md="4"
      >
        <v-text-field
          :value="value.schoolYearName"
          :label="$t('preSchool.headers.schoolYearName')"
          disabled
        />
      </v-col>
      <v-col
        cols="12"
        md="6"
      >
        <v-textarea
          v-model="value.startOfYearEvaluation"
          counter
          outlined
          clearable
          persistent-placeholder
          :label="$t('preSchool.startOfYearEvaluation')"
        />
      </v-col>
      <v-col
        cols="12"
        md="6"
      >
        <v-textarea
          v-model="value.endOfYearEvaluation"
          counter
          outlined
          clearable
          persistent-placeholder
          :label="$t('preSchool.endOfYearEvaluation')"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
export default {
  name: 'PreSchoolEvaluationForm',
  props: {
    value:{
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
