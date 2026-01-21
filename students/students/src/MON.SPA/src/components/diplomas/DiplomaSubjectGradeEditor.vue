<template>
  <span class="d-flex justify-start">
    <v-col class="pt-0 px-1 pb-1">
      <v-select
        v-model="value.gradeCategory"
        :items="gradeCategoryOptions"
        :label="$t('diplomas.gradeCategory')"
        class="custom-small-text"
        :outlined="outlined"
        :persistent-placeholder="persistentPlaceholder"
        :dense="dense"
        :append-icon="appendIcon"
      />
    </v-col>
    <v-col
      v-if="value.gradeCategory === 1"
      class="pt-0 px-1 pb-1"
    >
      <diploma-subject-common-grade-editor
        :value="value"
        clearable
        :rules="!hasExternalEvaluationLimit ? [$validator.required()] : []"
        :class="!hasExternalEvaluationLimit ? 'required custom-small-text' : 'custom-small-text'"
        :outlined="outlined"
        :persistent-placeholder="persistentPlaceholder"
        :dense="dense"
        :append-icon="appendIcon"
      />
    </v-col>
    <v-col
      v-if="value.gradeCategory === 2"
      class="pt-0 px-1 pb-1"
    >
      <diploma-subject-special-needs-grade-editor
        :value="value"
        clearable
        :rules="!hasExternalEvaluationLimit ? [$validator.required()] : []"
        :class="!hasExternalEvaluationLimit ? 'required custom-small-text' : 'custom-small-text'"
        :outlined="outlined"
        :persistent-placeholder="persistentPlaceholder"
        :dense="dense"
        :append-icon="appendIcon"
      />
    </v-col>
    <v-col
      v-if="value.gradeCategory === 3"
      class="pt-0 px-1 pb-1"
    >
      <diploma-subject-other-grade-editor
        :value="value"
        clearable
        :rules="!hasExternalEvaluationLimit ? [$validator.required()] : []"
        :class="!hasExternalEvaluationLimit ? 'required custom-small-text' : 'custom-small-text'"
        :outlined="outlined"
        :persistent-placeholder="persistentPlaceholder"
        :dense="dense"
        :append-icon="appendIcon"
      />
    </v-col>
    <v-col
      v-if="value.gradeCategory === 4"
      class="pt-0 px-1 pb-1"
    >
      <diploma-subject-qualitative-grade-editor
        :value="value"
        clearable
        :rules="!hasExternalEvaluationLimit ? [$validator.required()] : []"
        :class="!hasExternalEvaluationLimit ? 'required custom-small-text' : 'custom-small-text'"
        :outlined="outlined"
        :persistent-placeholder="persistentPlaceholder"
        :dense="dense"
        :append-icon="appendIcon"
      />
    </v-col>
  </span>
</template>

<script>
import DiplomaSubjectCommonGradeEditor from '@/components/diplomas/DiplomaSubjectCommonGradeEditor';
import DiplomaSubjectSpecialNeedsGradeEditor from '@/components/diplomas/DiplomaSubjectSpecialNeedsGradeEditor';
import DiplomaSubjectOtherGradeEditor from '@/components/diplomas/DiplomaSubjectOtherGradeEditor';
import DiplomaSubjectQualitativeGradeEditor from '@/components/diplomas/DiplomaSubjectQualitativeGradeEditor';
import Constants from "@/common/constants.js";

export default {
  name: 'DiplomaSubjectGradeEditorComponent',
  components: {
    DiplomaSubjectCommonGradeEditor,
    DiplomaSubjectSpecialNeedsGradeEditor,
    DiplomaSubjectOtherGradeEditor,
    DiplomaSubjectQualitativeGradeEditor
  },
  props: {
    value: {
      type: Object,
      required: true,
    },
    hasExternalEvaluationLimit: {
      type: Boolean,
      default() {
        return false;
      }
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
    },
    appendIcon: {
      type: String,
      default() {
        return '$dropdown';
      }
    }
  },
  data() {
    return {
      gradeCategoryOptions: Constants.gradeCategoryOptions
    };
  }
};
</script>

<style lang="scss" scoped>
  ::v-deep {
    .v-select__selection--disabled {
      color: rgba(0, 0, 0, 0.87) !important;
    }
  }
</style>
