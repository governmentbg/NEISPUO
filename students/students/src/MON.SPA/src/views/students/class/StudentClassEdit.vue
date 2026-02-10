<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <v-card>
        <v-card-title>
          {{ $t("studentClassChange.currentStudentClass") }}
        </v-card-title>
        <v-card-text>
          <student-class-details :value="selectedStudentClass" />
        </v-card-text>
      </v-card>

      <v-divider class="mt-2" />

      <form-layout
        :disabled="disabled"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t("studentClass.edit") }}</h3>
        </template>

        <template #default>
          <v-form
            :ref="'studentClassEdit' + _uid"
            :disabled="disabled"
          >
            <v-card>
              <v-card-title>
                {{ $t("enroll.basicData") }}
              </v-card-title>
              <v-card-text>
                <v-row dense>
                  <v-col
                    cols="12"
                    sm="6"
                    md="4"
                    lg="2"
                  >
                    <v-text-field
                      :value="model.schoolYearName"
                      :label="$t('common.schoolYear')"
                      disabled
                    />
                  </v-col>
                  <v-col
                    cols="12"
                    sm="6"
                    md="4"
                    lg="2"
                  >
                    <date-picker
                      id="enrollmentDate"
                      ref="enrollmentDate"
                      v-model="model.enrollmentDate"
                      :show-buttons="false"
                      :scrollable="false"
                      :no-title="true"
                      :show-debug-data="false"
                      :label="$t('documents.admissionDateLabel')"
                      :rules="[$validator.required()]"
                      class="required"
                    />
                  </v-col>
                  <v-col
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <v-text-field
                      :value="studentClassName"
                      :label="$t('student.class')"
                      disabled
                    />
                  </v-col>
                  <v-col
                    v-if="model.classId"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <custom-autocomplete
                      id="Клас"
                      ref="Клас"
                      v-model="model.classGroup.basicClassId"
                      api="/api/lookups/GetBasicClassValidOptions"
                      :label="$t('enroll.basicClass')"
                      :placeholder="$t('buttons.search')"
                      hide-no-data
                      :defer-options-loading="false"
                      :disabled="model.classGroup.basicClassId !== null && !model.isNotPresentForm"
                      no-filter
                      loading
                      @change="onStudentClassChange"
                    />
                  </v-col>
                  <v-col
                    v-if="showClassTypeOptions"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <v-autocomplete
                      id="selectedClassType"
                      ref="selectedClassType"
                      v-model="model.selectedClassTypeId"
                      :label="$t('enroll.classType')"
                      name="selectedClassType"
                      item-text="text"
                      item-value="value"
                      :placeholder="$t('buttons.search')"
                      :items="classTypeOptions"
                      clearable
                      :rules="classTypeRequired ? [$validator.required()] : []"
                      :class="classTypeRequired ? 'required' : ''"
                    />
                  </v-col>
                  <v-col
                    v-if="model"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <v-select
                    
                      ref="classEduFormName"
                      v-model="model.studentEduFormId"
                      :label="$t('enroll.classEduFormName')"
                      :rules="[$validator.required(true)]"
                      :items="filteredEducationFormOptions"
                      class="required"
                    />
                  </v-col>
                  <v-col
                    v-if="!isKindergartenInstitution && model.classGroup"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <custom-autocomplete
                      id="Професия"
                      ref="Професия"
                      v-model="model.studentProfessionId"
                      api="/api/lookups/GetSPPOOProfession"
                      label="Професия"
                      :placeholder="$t('buttons.search')"
                      hide-no-data
                      :disabled="model.classGroup.classProfessionId === null && !model.isNotPresentForm"
                      :defer-options-loading="false"
                      no-filter
                      loading
                      clearable
                      @change="onStudentProfessionChange"
                    />
                  </v-col>
                  <v-col
                    v-if="!isKindergartenInstitution && model.classGroup"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <custom-autocomplete
                      id="Специалност"
                      ref="Специалност"
                      v-model="model.studentSpecialityId"
                      :filter="{
                        relatedObject: model.studentProfessionId,
                      }"
                      api="/api/lookups/GetSPPOOSpecialityExtendedText"
                      label="Специалност"
                      :placeholder="$t('buttons.search')"
                      hide-no-data
                      :defer-options-loading="false"
                      no-filter
                      :disabled="model.classGroup.classSpecialityId === null && !model.isNotPresentForm"
                      :rules="model.studentProfessionId ? [$validator.required()] : []"
                      :class="model.studentProfessionId ? 'required' : ''"
                      loading
                      clearable
                    />
                  </v-col>
                  <v-col
                    v-if="isKindergartenInstitution"
                    cols="12"
                    sm="6"
                    md="4"
                    lg="2"
                  >
                    <date-picker
                      id="entryDate"
                      ref="entryDate"
                      v-model="model.entryDate"
                      :show-buttons="false"
                      :scrollable="false"
                      :no-title="true"
                      :show-debug-data="false"
                      :label="$t('documents.entryDateLabel')"
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <student-class-dual-form-company-manager
              v-if="hasDualFormCompanyManagePermission"
              v-model="model"
              class="my-2"
            />

            <student-class-additional-data
              v-model="model"
              class="my-2"
            />

            <student-class-supportive-environment
              :key="supportiveEnvironmentComponentKey"
              v-model="model"
              class="my-2"
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
  </div>
</template>

<script>
import StudentClassDetails from "@/components/students/class/StudentClassDetails.vue";
import StudentClassSupportiveEnvironment from "@/components/students/class/StudentClassSupportiveEnvironment.vue";
import StudentClassAdditionalData from "@/components/students/class/StudentClassAdditionalData.vue";
import { StudentClassEnrollModel } from "@/models/studentClass/studentClassEnrollModel";
import { Permissions, InstType } from "@/enums/enums";
import { mapGetters } from 'vuex';
import CustomAutocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import StudentClassDualFormCompanyManager from '@/components/students/class/StudentClassDualFormCompanyManager.vue';
export default {
  name: "StudentClassEdit",
  components: {
    StudentClassDetails,
    StudentClassSupportiveEnvironment,
    StudentClassAdditionalData,
    CustomAutocomplete,
    StudentClassDualFormCompanyManager
  },
  props: {
    pid: {
      type: Number,
      required: true,
    },
    classId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      model: new StudentClassEnrollModel(),
      supportiveEnvironmentComponentKey: 0,
      educationFormOptions: [],
      selectedStudentClass: {},
      filteredEducationFormOptions: [],
      classTypeOptions: [],
      classTypeRequired: true
    };
  },
  computed: {
    ...mapGetters(['hasInstitutionPermission', 'isInstType', 'dualEduFormId', 'dualClassTypeId']),
    isKindergartenInstitution() {
      return this.isInstType(InstType.Kindergarten);
    },
    disabled() {
      return this.saving;
    },
    institutionId() {
      return this.selectedStudentClass && this.selectedStudentClass.classGroup
        ? this.selectedStudentClass.classGroup.institutionId
        : undefined;
    },
    studentClassName() {
      return `${this.model.classGroup.className} - ${this.model.classGroup.classEduFormName ? this.model.classGroup.classEduFormName : ''} - ${this.model.classGroup.classTypeName ? this.model.classGroup.classTypeName : ''} - /${this.model.classGroup.studentsInClass} деца/`;
    },
    showClassTypeOptions() {
      if(this.model.isNotPresentForm) {
        this.loadClassTypeOptions();
      }

      return this.model.isNotPresentForm;
    },
    hasDualFormCompanyManagePermission() {
      return [11,12].includes(this.model?.classGroup?.basicClassId)
        && this.model?.classGroup?.classTypeId == this.dualClassTypeId
        && this.model?.studentEduFormId === this.dualEduFormId;

    }
  },
  async mounted() {
    if (!this.hasInstitutionPermission(Permissions.PermissionNameForStudentClassUpdate)) {
      return this.$router.push("/errors/AccessDenied");
    }
    await this.load();
    this.loadEducationValidOptions();
    this.supportiveEnvironmentComponentKey++;
  },
  methods: {
    loadClassTypeOptions() {
      this.$api.lookups
        .getClassTypeOptions(this.model.classGroup.basicClassId)
        .then((response) => {
          this.classTypeOptions = response.data;
          this.classTypeRequired = this.classTypeOptions.length > 0 ? true : false;
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.classTypesOptionsLoad"));
          console.log(error);
        });
    },
    async load() {
      await this.$api.studentClass.getById(this.classId)
        .then((response) => {
          if (response.data) {
            this.model = new StudentClassEnrollModel(response.data, this.$moment);
            this.selectedStudentClass = { ...response.data };
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    loadEducationValidOptions() {
      this.$api.lookups
        .getEduFormsForLoggedUser(this.model.isNotPresentForm)
        .then((response) => {
          const educationFormOptions = response.data;
          if (this.educationFormOptions) {
            this.educationFormOptions = educationFormOptions;
            this.filterEduForms();
          }
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.educationTypeOptionsLoad"));
          console.log(error);
        });
    },
    onSave() {
      const form = this.$refs["studentClassEdit" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.model.enrollmentDate = this.$helper.parseDateToIso(this.model.enrollmentDate ?? '');
      this.model.entryDate = this.$helper.parseDateToIso(this.model.entryDate ?? '');
      this.model.dischargeDate = this.$helper.parseDateToIso(this.model.dischargeDate ?? '');

      if(this.model.dualFormCompanies?.length > 0) {
        this.model.dualFormCompanies.forEach(x => {
          x.startDate = this.$helper.parseDateToIso(x.startDate ?? '');
          x.endDate = this.$helper.parseDateToIso(x.endDate ?? '');
        });
      }

      this.saving = true;

      this.$api.studentClass
        .update(this.model)
        .then(() => {
          this.$studentEventBus.$emit(
            "studentMovementUpdate",
            this.model.personId
          );
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.classSave"), 5000);
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    },
    filterEduForms() {
      if(this.model && this.model.classGroup) {
        this.filteredEducationFormOptions = this.educationFormOptions.filter(el => {
          return !!el.isNotPresentForm === !!this.model.classGroup.isNotPresentForm;
        });
      }
    },
    onStudentProfessionChange() {
      if(this.model) {
        this.model.studentSpecialityId = null;
      }
    },
    onStudentClassChange() {
      if(this.model.isNotPresentForm) {
        this.model.selectedClassTypeId = null;
      }
    }
  },
};
</script>
