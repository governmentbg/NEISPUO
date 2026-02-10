<template>
  <div>
    <div>
      <form-layout
        :disabled="saving || !model || model.length === 0"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('lod.assessments.createTitle') }}</h3>
        </template>
        <template #subtitle>
          <v-toolbar
            flat
          >
            <c-select
              v-model="selectedSchoolYear"
              :items="schoolYearOptions"
              :label="$t('lod.evaluations.schoolYearSelect')"
              dense
              outlined
              hide-details
              persistent-placeholder
              @change="selectedStudentClass = null"
            >
              <template v-slot:[`item`]="{ item }">
                <span>
                  {{ item.text }}
                </span>
                <v-spacer />
                <span
                  v-if="item.disabled"
                >
                  <v-chip
                    class="ma-2"
                    color="error"
                    outlined
                    small
                  >
                    {{ $t('errors.lodIsFInalizedSelectionError', {schoolYear: item.text}) }}
                  </v-chip>
                </span>
              </template>
            </c-select>
            <c-select
              v-if="selectedSchoolYear"
              v-model="selectedStudentClass"
              :items="studentClassOptions"
              :label="$t('lod.evaluations.studentClassSelect')"
              return-object
              dense
              outlined
              hide-details
              persistent-placeholder
              class="ml-2"
              item-value="uid"
            />
            <v-spacer />
            <v-btn
              v-if="selectedStudentClass && selectedStudentClass.isNotPresentForm !== true && hasManagePermission"
              color="primary"
              small
              class="mx-3"
              @click.stop="onCurriculumLoad"
            >
              {{ $t('curriculum.addButton') }}
            </v-btn>
            <v-btn
              v-if="selectedStudentClass && selectedStudentClass.isNotPresentForm === true && hasManagePermission"
              color="primary"
              small
              class="mx-3"
              @click.stop="onTemplateLoad"
            >
              {{ $t('lodAssessmentTemplate.loadBtnText') }}
            </v-btn>
            <c-select
              v-if="selectedStudentClass"
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

        <template
          v-if="sortedCurriculumParts"
          #default
        >
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
    <confirm-dlg ref="curriculumLoadConfirm" />
    <prompt-dlg
      ref="templatePrompt"
      persistent
    >
      <template>
        <autocomplete
          v-if="selectedStudentClass"
          v-model="selectedTemplateId"
          api="/api/lodAssessmentTemplate/GetDropdownOptions"
          :defer-options-loading="false"
          :filter="{
            basicClassId: selectedStudentClass.basicClassId
          }"
        />
      </template>
    </prompt-dlg>
  </div>
</template>

<script>
import LodAssessmentsTable from '@/components/lod/assessment/LodAssessmentsTable.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentLodAssessmentCreateView',
  components: {
    LodAssessmentsTable,
    Autocomplete
  },
  props: {
    personId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      curriculumPartOption: [],
      gradeOptions: [],
      subjectTypeOptions: [],
      saving: false,
      selectedSchoolYear: null,
      selectedStudentClass: null,
      selectedTemplateId: null,
      studentClasses: [],
      model: []
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

      let options = this.curriculumPartOption;

      // if(this.selectedStudentClass?.isNotPresentForm === true) {
      //   options = options.filter(x => x.value !== 3);
      // }

      options = options
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
      if(!this.selectedStudentClass) {
        return this.gradeOptions;
      }

      if(this.selectedStudentClass.basicClassId < 4) {
        return this.gradeOptions.filter(x => x.gradeTypeId !== 1);
      } else {
        return this.gradeOptions.filter(x => x.gradeTypeId !== 4);
      }
    },
    schoolYearOptions() {
      if(!this.studentClasses) return [];

      return this.studentClasses.map(x => {
        return {
          value: x.schoolYear,
          text: x.schoolYearName,
          disabled: x.isLodFinalized
        };
      });
    },
    studentClassOptions() {
      if(!this.studentClasses) return [];


      const filteredStudentClasses =  this.studentClasses
        .filter(x => x.schoolYear === this.selectedSchoolYear)
        .map(x => {
          return {
            value: x.value,
            text: x.text,
            basicClassId: x.basicClassId,
            isNotPresentForm: x.isNotPresentForm,
            uid: x.uid
          };
        });

        return filteredStudentClasses;
    },
  },
  mounted() {
    this.loadMainStudentClasses();
    this.loadCurriculumPartOptions();
    this.loadGradeOptions();
    this.loadSubjectTypeOptions();
  },
  methods: {
    loadMainStudentClasses() {
      this.$api.lodAssessment.getMainStudentClasses(this.personId)
        .then((response) => {
          if(response.data) {
            this.studentClasses = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
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
          this.subjectTypeOptions = this.$helper.groupSubjectTypeOptions(response.data);
        })
        .catch((error) => {
          console.log(error);
        });
    },
    addCurriculumPart(e) {
      if(this.model && this.model.some(x => x.curriculumPartId === e.value)) return;

      this.model.push({
        curriculumPartId: e.value,
        curriculumPart: e.originText,
        curriculumPartName: e.name,
        personId: this.personId,
        schoolYear: this.selectedSchoolYear,
        basicClassId: this.selectedStudentClass.basicClassId,
        isSelfEduForm: this.selectedStudentClass.isNotPresentForm ?? false,
        subjectAssessments: []
      });
    },
    onCurriculumPartRemove(item) {
      if(!this.model) return; // Няма от какво да махаме

      const index = this.model.findIndex(x => x.curriculumPartId === item.curriculumPartId);
      this.model.splice(index, 1);
    },
    async onCurriculumLoad() {
      if(await this.$refs.curriculumLoadConfirm.open(this.$t('buttons.confirm'), this.$t('lod.assessments.loadCurriculumConfirmPrompt'))) {
        this.saving = true;
        this.$api.lodAssessment.getStudentClassCurriculum(this.selectedStudentClass.value)
          .then((response) => {
            if(Array.isArray(response.data)) {
              if(response.data.length) {
                this.model = response.data;
                this.$helper.fixupLodAssessmentCreateOrUpdateModel(this.model);
              } else {
                // За избраната група/паралелка липсва учебен план
                this.$notifier.warn('', this.$t('lod.assessments.loadCurriculumNoDataForClass'));
              }
            }
          })
          .catch((error) => {
            console.log(error.response);
          })
          .finally(() => {
            this.saving = false;
          });

      }

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
              if(this.selectedSchoolYear) {
                x.schoolYear = this.selectedSchoolYear;
              }

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

