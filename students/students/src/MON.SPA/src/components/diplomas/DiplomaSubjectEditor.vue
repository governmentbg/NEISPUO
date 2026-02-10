<template>
  <diploma-template-subject-editor
    :ref="`TemplateSubjectEditor_${_uid}`"
    :value="value"
    :disabled="disabled"
    class="mb-1"
    :hover="hover"
    :can-add-modules="canAddModules"
    is-diploma-subject
    v-bind="$attrs"
    v-on="$listeners"
    @subjectChange="onSubjectChange"
  >
    <template #extension>
      <v-col>
        <diploma-subject-grade-editor
          :value="value"
          :has-external-evaluation-limit="hasExternalEvaluationLimit"
        />
      </v-col>

      <v-col
        v-if="hasExternalEvaluationLimit"
      >
        <diploma-subject-nvo-editor
          :value="value"
          clearable
        />
      </v-col>
      <v-col
        v-if="showEctsGrade"
      >
        <v-select
          v-model="value.ects"
          :items="ectsGradeOptions"
          :label="$t('diplomas.ectsGrade')"
          clearable
          class="custom-small-text"
        />
      </v-col>
      <v-col
        v-if="!hasExternalEvaluationLimit"
        :style="{ 'max-width': '70px' }"
      >
        <v-checkbox
          v-model="value.showFlSubject"
          color="primary"
          class="custom-small-text custom-center"
          dense
          :hint="$t('diplomas.showFlSubject')"
          persistent-hint
        />
      </v-col>
      <v-col
        v-if="value.showFlSubject"
        cols="12"
        class="pt-0"
      >
        <span class="d-flex justify-end">
          <v-col
            cols="6"
            sm="3"
            class="py-0"
          >
            <autocomplete
              :ref="`FlSubject_${_uid}`"
              v-model="value.flSubjectId"
              api="/api/lookups/GetFlOptions"
              :label="$t('diplomas.flsubject')"
              :placeholder="$t('buttons.search')"
              :page-size="20"
              :rules="[$validator.required()]"
              class="required custom-small-text"
              :defer-options-loading="false"
              @change="onFlSubjectChange"
            />
          </v-col>
          <v-col
            cols="6"
            md="1"
            class="py-0"
          >
            <v-text-field
              v-model.number="value.flHorarium"
              type="number"
              :label="$t('diplomas.template.horarium')"
              :rules="[$validator.min(1)]"
              class="custom-small-text"
              @wheel="$event.target.blur()"
            />
          </v-col>
        </span>
      </v-col>
    </template>
  </diploma-template-subject-editor>
</template>

<script>

import Constants from '@/common/constants.js';
import DiplomaTemplateSubjectEditor from '@/components/diplomas/DiplomaTemplateSubjectEditor';
import DiplomaSubjectGradeEditor from '@/components/diplomas/DiplomaSubjectGradeEditor';
import DiplomaSubjectNvoEditor from '@/components/diplomas/DiplomaSubjectNvoEditor';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'DiplomaSubjectEditorComponent',
  components: {
    DiplomaTemplateSubjectEditor,
    DiplomaSubjectGradeEditor,
    DiplomaSubjectNvoEditor,
    Autocomplete
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
    showEctsGrade: {
      type: Boolean,
      default() {
        return false;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    canAddModules: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hover: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      gradeCategoryOptions: Constants.gradeCategoryOptions,
      ectsGradeOptions: Constants.ectsGradeOptions
    };
  },
  mounted() {
    if (!this.value.gradeCategory) {
      this.value.gradeCategory = 1; // по подразбиране
    }
  },
  methods: {
    onSubjectChange(subjectId) {
      const selector = this.$refs[`TemplateSubjectEditor_${this._uid}`];
       if (selector) {
        const selectedItem = selector.getSubjectOptions().find(x => x.value === subjectId);
        this.value.subjectName = selectedItem?.text;
      }
    },
    onFlSubjectChange(subjectId) {
      const selector = this.$refs[`FlSubject_${this._uid}`];
      if (selector) {
        const selectedItem = selector.optionsList.find(x => x.value === subjectId);
        this.value.flSubjectName = selectedItem?.text;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
  ::v-deep {
    .custom-center .v-input__slot {
      align-items: center;
      justify-content: center;
    }

    .custom-center .v-messages__message {
      text-align: center !important;
    }


 }
</style>

