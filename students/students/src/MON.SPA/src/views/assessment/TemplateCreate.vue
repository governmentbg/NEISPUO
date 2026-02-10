<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('lodAssessmentTemplate.createTitle') }}</h3>
      </template>

      <template #default>
        <lod-assessment-template-form
          v-if="model !== null"
          :ref="'lodAssessmentTemplateCreateForm' + _uid"
          v-model="model"
          :disabled="saving"
          @curriculumPartRemove="onCurriculumPartRemove"
        >
          <template #curriculumsToolbar>
            <v-toolbar flat>
              <c-select
                :items="filteredCurriculumPartAddOptions"
                clearable
                :label="$t('lod.assessments.addCurriculumPart')"
                return-object
                dense
                hide-details
                :disabled="!model.basicClassId"
                @change="addCurriculumPart($event)"
              >
                <template #selection="">
                  {{ '' }}
                </template>
              </c-select>
            </v-toolbar>
          </template>
        </lod-assessment-template-form>
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import LodAssessmentTemplateForm from '@/components/lod/assessment/LodAssessmentTemplateForm.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'AssessmentTemplateCreateView',
  components: {
    LodAssessmentTemplateForm
  },
  data() {
    return {
      saving: false,
      model: null,
      curriculumPartOption: [],
    };
  },
  computed: {
    ...mapGetters(['hasPermission','userInstitutionId']),
    filteredCurriculumPartAddOptions() {
      const excludedCurriculumParts = !this.model || !this.model.curriculumParts
        ? []
        : this.model.curriculumParts.map(x => x.curriculumPartId);

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
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.loadCurriculumPartOptions();
    this.init();
  },
  methods: {
    init() {
      this.model = {
        name: null,
        description: null,
        curriculumParts: []
      };
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
    onSave() {
      const form = this.$refs['lodAssessmentTemplateCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }


      this.saving = true;
      this.$api.lodAssessmentTemplate.create(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error?.response?.data?.message ?? this.$t('common.error'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
    async addCurriculumPart(e) {
      if(this.model && !this.model.curriculumParts) this.model.curriculumParts = [];
      if(this.model && this.model.curriculumParts && this.model.curriculumParts.some(x => x.curriculumPartId === e.value)) return;


      this.model.curriculumParts.push({
        curriculumPartId: e.value,
        curriculumPart: e.originText,
        curriculumPartName: e.name,
        personId: 0,
        schoolYear: (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data,
        basicClassId: this.model.basicClassId,
        isSelfEduForm: true,
        subjectAssessments: []
      });
    },
    onCurriculumPartRemove(item) {
      if(!this.model || !this.model.curriculumParts) return; // Няма от какво да махаме

      const index = this.model.curriculumParts.findIndex(x => x.curriculumPartId === item.curriculumPartId);
      this.model.curriculumParts.splice(index, 1);
    },
  }
};
</script>
