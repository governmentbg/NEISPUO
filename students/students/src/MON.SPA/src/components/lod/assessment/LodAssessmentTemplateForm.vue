<template>
  <v-form
    :ref="'lodAssessmentTemplateForm' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        cols="12"
        md="6"
        lg="10"
        dense
      >
        <c-text-field
          v-model="model.name"
          :label="$t('lodAssessmentTemplate.headers.name')"
          outlined
          persistent-placeholder
          dense
          :rules="[$validator.required()]"
          class="required"
        />
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
        dense
      >
        <c-text-field
          v-if="disabled"
          v-model="model.basicClassName"
          :label="$t('lodAssessmentTemplate.headers.basicClass')"
          outlined
          persistent-placeholder
          dense
        />
        <autocomplete
          v-else
          v-model="model.basicClassId"
          api="/api/lookups/GetBasicClassesForLoggedUser"
          :label="$t('lodAssessmentTemplate.headers.basicClass')"
          :placeholder="$t('buttons.search')"
          :defer-options-loading="false"
          hide-no-data
          hide-selected
          :rules="[$validator.required()]"
          class="required"
          dense
          outlined
          persistent-placeholder
        />
      </v-col>
      <v-col
        cols="12"
        dense
      >
        <c-textarea
          v-model="model.description"
          :label="$t('lodAssessmentTemplate.headers.description')"
          outlined
          dense
          persistent-placeholder
        />
      </v-col>
    </v-row>
    <slot
      name="curriculumsToolbar"
    />
    <v-row>
      <v-col cols="12">
        <lod-assessments-table
          v-for="curriculumPart in sortedCurriculumParts"
          :key="curriculumPart.curriculumPartId"
          :value="curriculumPart"
          :is-edit-mode="!disabled"
          :grade-options="filteredGradeOptions"
          :subject-type-options="subjectTypeOptions.filter(x => x.partId === curriculumPart.curriculumPartId)"
          @curriculumPartRemove="v => $emit('curriculumPartRemove', v)"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import LodAssessmentsTable from '@/components/lod/assessment/LodAssessmentsTable.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'LodAssessmentTemplateFormComponent',
  components: {
    LodAssessmentsTable,
    Autocomplete
  },
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
  data() {
    return {
      model: this.value,
      gradeOptions: [],
      subjectTypeOptions: [],
    };
  },
  computed: {
    sortedCurriculumParts() {
      if(!this.model || !this.model.curriculumParts) return this.model;

      const parts = this.model.curriculumParts;
      return parts.sort((a,b) => { return a.curriculumPartId - b.curriculumPartId; });
    },
    filteredGradeOptions() {
      return this.gradeOptions;
    },
  },
  watch: {
    value() {
      this.model = this.value;
    }
  },
  mounted() {
    this.loadGradeOptions();
    this.loadSubjectTypeOptions();
  },
  methods: {
    validate() {
      const form = this.$refs['lodAssessmentTemplateForm' + this._uid];
      return form ? form.validate() : false;
    },
    loadGradeOptions() {
      this.$api.lookups.getGradeOptions()
        .then((response) => {
          this.gradeOptions = this.$helper.groupGradeOptions(response.data);
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    loadSubjectTypeOptions() {
      this.$api.lookups.getSubjectTypeOptions()
        .then((response) => {
          this.subjectTypeOptions = this.$helper.groupSubjectTypeOptions(response.data);
        })
        .catch((error) => {
          console.log(error);
        });
    },
  }
};
</script>
