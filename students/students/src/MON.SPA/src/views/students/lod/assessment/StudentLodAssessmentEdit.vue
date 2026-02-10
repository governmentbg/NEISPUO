<template>
  <div>
    <app-loader
      v-if="loading"
    />
    <div v-if="sortedCurriculumParts">
      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('lod.assessments.editTitle') }}</h3>
        </template>

        <template #subtitle>
          <v-toolbar
            flat
          >
            <c-text-field
              :label="$t('lod.assessments.headers.schoolYear')"
              :value="schoolYaerName"
              dense
              outlined
              hide-details
              persistent-placeholder
              disabled
            />
            <c-text-field
              :label="$t('lod.assessments.headers.basicClass')"
              :value="basicClassName"
              dense
              outlined
              hide-details
              persistent-placeholder
              class="ml-2"
              disabled
            />
            <v-spacer />
            <!-- <v-btn
              v-if="isSelfEduForm === true && hasManagePermission"
              color="primary"
              small
              class="mx-3"
              @click.stop="onTemplateLoad"
            >
              {{ $t('lodAssessmentTemplate.loadBtnText') }}
            </v-btn> -->
            <c-select
              :items="filteredCurriculumPartAddOptions"
              clearable
              :label="$t('lod.assessments.addCurriculumPart')"
              return-object
              dense
              hide-details
              @change="addCurriculumPart($event)"
            >
              <template #selection="">
                {{ '' }}
              </template>
            </c-select>
          </v-toolbar>
        </template>

        <template #default>
          <v-form
            :ref="'form' + _uid"
            :disabled="!hasManagePermission"
          >
            <lod-assessments-table
              v-for="curriculumPart in sortedCurriculumParts"
              :key="curriculumPart.curriculumPartId"
              :value="curriculumPart"
              :is-edit-mode="hasManagePermission"
              :grade-options="filteredGradeOptions"
              :subject-type-options="subjectTypeOptions.filter(x => x.partId === curriculumPart.curriculumPartId)"
              @curriculumPartRemove="onCurriculumPartRemove"
            />
          </v-form>
        </template>
      </form-layout>
    </div>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <prompt-dlg
      ref="templatePrompt"
      persistent
    >
      <template>
        <autocomplete
          v-model="selectedTemplateId"
          api="/api/lodAssessmentTemplate/GetDropdownOptions"
          :defer-options-loading="false"
          :filter="{
            basicClassId: basicClassId
          }"
        />
      </template>
    </prompt-dlg>
  </div>
</template>

<script>
import AppLoader from '@/components/wrappers/loader.vue';
import LodAssessmentsTable from '@/components/lod/assessment/LodAssessmentsTable.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentLodAssessmentEditView',
  components: {
    LodAssessmentsTable,
    AppLoader,
    Autocomplete
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    },
    basicClassId: {
      type: Number,
      required: true
    },
    isSelfEduForm: {
      type: Boolean,
      required: true
    },
  },
  data() {
    return {
      curriculumPartOption: [],
      gradeOptions: [],
      subjectTypeOptions: [],
      saving: false,
      loading: false,
      schoolYaerName: null,
      basicClassName: null,
      filterForCurrentInstitution: false,
      filterForCurrentSchoolBook: false,
      model: [],
      selectedTemplateId: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentEvaluationManage);
    },
    sortedCurriculumParts() {
      if(!this.model) return this.model;

      const parts = this.model;
      return parts.sort((a,b) => { return a.curriculumPartId - b.curriculumPartId; });
    },
    filteredCurriculumPartAddOptions() {
      const excludedCurriculumParts = !this.model
        ? []
        : this.model.map(x => x.curriculumPartId);

      const options = this.curriculumPartOption
        .filter(x => !excludedCurriculumParts.includes(x.value))
        .map(x => {
          return {
            value: x.value,
            text: `${this.$t('buttons.add')} "${x.text}"`,
            name: x.name,
            originText: x.text
          };
        });

       return options;
    },
    filteredGradeOptions() {
      if(!this.basicClassId) {
        return this.gradeOptions;
      }

      if(this.basicClassId < 4) {
        return this.gradeOptions.filter(x => x.gradeTypeId !== 1);
      } else {
        return this.gradeOptions.filter(x => x.gradeTypeId !== 4);
      }
    },
  },
  async mounted() {
    await this.load();
    this.loadCurriculumPartOptions();
    this.loadGradeOptions();
    this.loadSubjectTypeOptions();
    this.getSchoolYearName();
    this.getBasicClassName();
  },
  methods: {
    async load() {
      this.loading = true;
      try {
        this.model =  (await this.$api.lodAssessment.getPersonAssessments(this.personId, this.basicClassId, this.schoolYear, this.isSelfEduForm, this.filterForCurrentInstitution, this.filterForCurrentSchoolBook))?.data;
        this.$helper.fixupLodAssessmentCreateOrUpdateModel(this.model);
      } catch (error) {
        console.log(error);
      } finally {
        this.loading = false;
      }
    },
    loadCurriculumPartOptions() {
      this.$api.lookups.getCurriculumPartOptions()
        .then((response) => {
          if(response.data) {
            this.curriculumPartOption = response.data.filter(x => x.value <= 4);
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
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
      this.$api.lookups.getSubjectTypeOptions({showAll: true})
        .then((response) => {
          if (response.data) {
            this.subjectTypeOptions = this.$helper.groupSubjectTypeOptions(response.data);
          }
        })
        .catch((error) => {
          console.log(error);
        });
    },
    getSchoolYearName() {
      this.$api.lookups.getSchoolYears()
      .then((response) => {
          if(Array.isArray(response.data) && response.data.length > 0) {
            this.schoolYaerName = response.data.find(x => x.value === this.schoolYear)?.text;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    getBasicClassName() {
      this.$api.lookups.getBasicClassOptions()
      .then((response) => {
          if(Array.isArray(response.data) && response.data.length > 0) {
            const basicClass = response.data.find(x => x.value === this.basicClassId);
            this.basicClassName = `${basicClass?.code} - ${basicClass?.text}`;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    addCurriculumPart(e) {
      if(this.model && this.model.some(x => x.curriculumPartId === e.value)) return;
      this.model.push({
        curriculumPartId: e.value,
        curriculumPart: e.originText,
        curriculumPartName: e.name,
        personId: this.personId,
        schoolYear: this.schoolYear,
        basicClassId: this.basicClassId,
        isSelfEduForm: this.isSelfEduForm,
        subjectAssessments: []
      });
    },
    onCurriculumPartRemove(item) {
      if(!this.model) return; // Няма от какво да махаме

      const index = this.model.findIndex(x => x.curriculumPartId === item.curriculumPartId);
      this.model.splice(index, 1);
    },
    onSave() {
      const form = this.$refs["form" + this._uid];
      const isValid = form ? form.validate() : false;

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.saving = true;

      this.$api.lodAssessment.createOrUpdate(this.model)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'));
        this.onCancel();
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('common.saveError'));
        console.log(error.response);
      })
      .finally(() => {
        this.saving = false;
      });
    },
    onCancel() {
      this.$router.go(-1);
    },
    async onTemplateLoad() {
      this.selectedTemplateId = null;
      if(await this.$refs.templatePrompt.open(this.$t('lodAssessmentTemplate.loadBtnText'))) {
        if(!this.selectedTemplateId) {
          return this.$notifier.error('',`${this.$t("errors.invalidSelection")}`);
        }

        this.saving = true;
        this.$api.lodAssessmentTemplate.getById(this.selectedTemplateId)
        .then((response) => {
          if (response.data && Array.isArray(response.data.curriculumParts) && response.data.curriculumParts.length > 0) {
            this.$helper.clearArray(this.model);
            response.data.curriculumParts.forEach(x => {
              this.model.push({ ...x, personId: this.personId});
            });
          }
        })
        .catch((error) => {
          this.$notifier.success('', this.$t('common.loadError'), 5000);
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
      }
    }
  }
};
</script>

