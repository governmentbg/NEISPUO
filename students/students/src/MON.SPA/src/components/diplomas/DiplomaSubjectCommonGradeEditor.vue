<template>
  <v-text-field
    v-model.number="value.grade"
    type="number"
    :label="$t('diplomas.grade')"
    :outlined="outlined"
    :persistent-placeholder="persistentPlaceholder"
    :dense="dense"
    v-bind="$attrs"
    style="min-width:90px;"
    v-on="$listeners"
    @wheel="$event.target.blur()"
  />
</template>

<script>
import Constants from "@/common/constants.js";

export default {
  name: 'DiplomaSubjectCommonGradeEditorComponent',
  props: {
    value: {
      type: Object,
      required: true,
    },
    persistentPlaceholder: {
      type: Boolean,
      default() {
        return false;
      }
    },
    outlined: {
      type: Boolean,
      default() {
        return false;
      }
    },
    dense: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      gradeTextOptions: Constants.gradeTextOptions
    };
  },
  computed: {
    gradeText() {
      if (!this.value.grade) return '';
      const gradeTextOption = this.gradeTextOptions.find(x => x.min <= this.value.grade && this.value.grade < x.max);
      return gradeTextOption ? gradeTextOption.value : '';
    }
  },
  beforeDestroy() {
    this.value.grade = null;
    this.value.gradeText = null;
  }
};
</script>

<style>
input[type='number'] {
    -moz-appearance:textfield;
}
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
    -webkit-appearance: none;
}
</style>
