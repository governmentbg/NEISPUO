<template>
  <div>
    <v-form
      ref="form"
      class="text-left pt-6"
    >
      <v-card>
        <v-card-title
          v-if="userDetails"
        >
          {{ $t('enroll.title', { institution: userDetails.institution }) }}
        </v-card-title>
        <v-card-text>
          <v-card>
            <v-card-title>
              {{ $t("enroll.basicData") }}
            </v-card-title>
            <v-card-text>
              <v-row
                dense
              >
                <v-col
                  cols="12"
                  sm="6"
                  md="2"
                >
                  <school-year-selector
                    ref="schoolYear"
                    v-model="form.schoolYear"
                    :label="$t('common.schoolYear')"
                    :min-year="currentYear"
                    :max-year="currentYear + 1"
                    :rules="[$validator.required()]"
                    class="required"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="6"
                  md="2"
                >
                  <v-checkbox
                    v-model="form.notPresentFormClassFilter"
                    color="primary"
                    :label="$t('enroll.serviceClass')"
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="8"
                >
                  <v-autocomplete
                    id="classId"
                    ref="studentClass"
                    v-model="form.classGroups"
                    :label="$t('enroll.class')"
                    name="classId"
                    item-text="text"
                    item-value="value"
                    :rules="[$validator.required()]"
                    :items="filteredClassGroups"
                    class="required"
                    multiple
                    chips
                    clearable
                  >
                    <template v-slot:[`item`]="{ item }">
                      <span v-if="!item.disabled">
                        {{ item.text }}
                      </span>
                      <span v-else>
                        <v-chip
                          class="ma-2"
                          color="orange"
                          outlined
                        >
                          {{ item.text }}
                        </v-chip>
                      </span>
                    </template>
                  </v-autocomplete>
                </v-col>
                <v-col
                  v-show="form.notPresentFormClassFilter"
                  cols="12"
                  md="4"
                >
                  <custom-autocomplete
                    id="basicClass"
                    ref="studentBasicClass"
                    v-model="form.basicClassId"
                    api="/api/lookups/GetBasicClassesForLoggedUser"
                    :label="$t('student.basicClass')"
                    :placeholder="$t('buttons.search')"
                    hide-no-data
                    :disabled="!form.notPresentFormClassFilter"
                    :rules="form.notPresentFormClassFilter ? [$validator.required()] : []"
                    :class="form.notPresentFormClassFilter ? 'required' : ''"
                    :defer-options-loading="false"
                    no-filter
                    loading
                    clearable
                  />
                </v-col>
                <v-col
                  v-show="form.notPresentFormClassFilter"
                  cols="12"
                  md="4"
                >
                  <custom-autocomplete
                    id="selectedClassType"
                    ref="selectedClassType"
                    v-model="form.selectedClassTypeId"
                    api="/api/lookups/getClassTypesForLoggedUser"
                    :label="$t('enroll.classType')"
                    :placeholder="$t('buttons.search')"
                    clearable
                    hide-no-data
                    hide-selected
                    :disabled="!form.notPresentFormClassFilter"
                    :rules="form.notPresentFormClassFilter ? [$validator.required()] : []"
                    :class="form.notPresentFormClassFilter ? 'required' : ''"
                    :defer-options-loading="false"
                    no-filter
                    loading
                  />
                </v-col>
                <v-col
                  v-show="form.notPresentFormClassFilter"
                  cols="12"
                  md="4"
                >
                  <v-select
                    ref="classEduFormName"
                    v-model="form.studentEduFormId"
                    :label="$t('enroll.classEduFormName')"
                    :items="isNotPresentEduFormsOptions"
                    :rules="form.notPresentFormClassFilter ? [$validator.required(true)] : []"
                    :class="form.notPresentFormClassFilter ? 'required' : ''"
                    clearable
                  />
                </v-col>
                <v-col
                  v-show="form.notPresentFormClassFilter"
                  cols="12"
                  md="4"
                >
                  <custom-autocomplete
                    id="professionId"
                    ref="professionId"
                    v-model="form.studentProfessionId"
                    api="/api/lookups/GetSPPOOProfession"
                    :label="$t('student.profession')"
                    :placeholder="$t('buttons.search')"
                    hide-no-data
                    :disabled="!form.notPresentFormClassFilter"
                    :defer-options-loading="false"
                    loading
                    clearable
                  />
                </v-col>
                <v-col
                  v-show="form.notPresentFormClassFilter"
                  cols="12"
                  md="4"
                >
                  <custom-autocomplete
                    id="specialityId"
                    ref="specialityId"
                    v-model="form.studentSpecialityId"
                    :filter="{relatedObject: form.studentProfessionId}"
                    api="/api/lookups/GetSPPOOSpecialityExtendedText"
                    :label="$t('student.speciality')"
                    :placeholder="$t('buttons.search')"
                    hide-no-data
                    :defer-options-loading="false"
                    clearable
                    :disabled="!form.notPresentFormClassFilter"
                    :rules="form.notPresentFormClassFilter ? [$validator.required()] : []"
                    :class="form.notPresentFormClassFilter ? 'required' : ''"
                    loading
                  />
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <student-class-supportive-environment
            v-model="form"
            class="my-2"
          />
        </v-card-text>

        <v-card-actions class="justify-end">
          <v-btn
            ref="submit"
            raised
            color="primary"
            :disabled="sending"
            @click="onConfirm"
          >
            {{ $t('buttons.save') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click="onReset"
          >
            {{ $t('buttons.clear') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
    <v-overlay :value="sending">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import StudentClassSupportiveEnvironment from '@/components/students/class/StudentClassSupportiveEnvironment.vue';
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import CustomAutocomplete from "@/components/wrappers/CustomAutocomplete.vue";

import { StudentClass } from '@/models/studentClass/studentClass';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "InitialCplrEnrollmentInStudentClass",
  components:{
    StudentClassSupportiveEnvironment,
    SchoolYearSelector,
    CustomAutocomplete
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
    admissionDocumentId:{
      type: Number,
      required: true
    }
  },
  data() {
      return {
          form: new StudentClass(),
          classGroupsOptions: [],
          isNotPresentEduFormsOptions: [],
          sending: false,
          currentYear: undefined // Използва се за определяне на границите на SchoolYear селектора
      };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userDetails', 'userInstitutionId']),
    filteredClassGroups() {
      if(!this.form || !this.classGroupsOptions || this.classGroupsOptions.length === 0) {
        return this.classGroupsOptions;
      }

      return this.classGroupsOptions.filter(x => x.isNotPresentForm === !!this.form.notPresentFormClassFilter);
    }
  },
  watch: {
    'form.schoolYear': function (newValue, oldVal) {
      if (oldVal !== "" && this.form) {
        this.$helper.clearArray(this.form.classGroups);
      }
      this.loadClassGroupsOptions(newValue);
    },
    'form.notPresentFormClassFilter': function() {
      if (this.form) {
        this.$helper.clearArray(this.form.classGroups);
        this.form.basicClassId = null;
        this.form.selectedClassTypeId = null;
        this.form.studentEduFormId = null;
        this.form.studentProfessionId = null;
        this.form.studentSpecialityId = null;
      }
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.loadIsNotPresentEduForm();

    this.form.personId = this.pid;
    this.form.admissionDocumentId = this.admissionDocumentId;

    this.currentYear = (await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data;
    this.form.schoolYear = this.currentYear;
  },
  methods: {
    loadIsNotPresentEduForm() {
      this.$api.lookups
        .getEduFormsForLoggedUser(true)
        .then((response) => {
          if(response.data) {
            this.isNotPresentEduFormsOptions = response.data;
          }
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.educationTypeOptionsLoad"));
          console.log(error.response);
        });
    },
    loadClassGroupsOptions(schoolYear) {
      if (!schoolYear) {
        this.classGroupsOptions = [];
        return;
      }

      this.$api.lookups
        .getClassGroupsForLoggedUser(schoolYear,this.pid)
        .then((response) => {
          this.classGroupsOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.classGroupsOptionsLoad"));
          console.log(error);
        });
    },
    onConfirm() {
      const isValid = this.$refs.form.validate();

      if(!this.form.admissionDocumentId) {
        return this.$notifier.error('', 'admissionDocumentId is required!', 5000);
      }

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      if (this.form.basicClassId == null) {
        this.form.basicClassId = 0; // Стойност по подразбиране инак не се байндва модела в контролера
      }

      this.sending = true;
      this.$api.studentClass
        .enrollInCplrClass(this.form)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.form.personId);
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.$router.push({ name: 'StudentDetails', params: { id: this.pid } });
          this.resetForm();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('errors.classSave'), error.response.data.message);
          console.log(error.response.data.message);
        })
        .then(() => { this.sending = false; });
    },
    resetForm() {
      this.form = new StudentClass();
    },
    onReset() {
      this.$refs.form.reset();
    }
  }
};
</script>
