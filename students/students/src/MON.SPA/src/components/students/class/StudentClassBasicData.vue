<template>
  <v-card>
    <v-card-title>
      {{ $t("enroll.basicData") }}
    </v-card-title>
    <v-card-text>
      <!-- {{ model }} -->
      <v-row
        dense
      >
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <school-year-selector
            ref="schoolYear"
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
            :min-year="minSchoolYear"
            :max-year="maxSchoolYear"
            :rules="[$validator.required()]"
            class="required"
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
            :min="minEnrollmentDate"
            :label="$t('documents.admissionDateLabel')"
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="8"
        >
          <!-- item-value="relatedObjectId" - inst_year.ClassGroup.ParentClassID -->
          <v-autocomplete
            id="classId"
            ref="studentClass"
            v-model="model.classId"
            :label="$t('enroll.class')"
            name="classId"
            item-text="text"
            item-value="value"
            :rules="[$validator.required()]"
            :items="classGroupsOptions"
            class="required"
            clearable
          >
            <template v-slot:[`item`]="{ item }">
              <template>
                <v-list-item-content class="py-0">
                  <span v-if="!item.disabled">
                    {{ item.text }}
                  </span>
                  <span v-else>
                    <v-chip
                      class="mx-2"
                      color="orange"
                      outlined
                    >
                      {{ item.text }}
                    </v-chip>
                  </span>
                </v-list-item-content>
                <v-list-item-icon
                  v-if="item.isValid !== true"
                  class="py-0"
                >
                  <v-chip
                    class="mx-2"
                    color="error"
                    small
                    outlined
                  >
                    {{ $t('institution.details.invalidClass') }}
                  </v-chip>
                </v-list-item-icon>
                <v-list-item-icon
                  v-if="item.isClosed == true"
                  class="py-0"
                >
                  <v-chip
                    class="mx-2"
                    color="error"
                    small
                    outlined
                  >
                    {{ $t('institution.details.closedClass') }}
                  </v-chip>
                </v-list-item-icon>
              </template>
            </template>
          </v-autocomplete>
        </v-col>
        <v-col
          v-if="!cplrEnrollment && model.classGroup"
          cols="12"
          md="4"
        >
          <custom-autocomplete
            id="basicClass"
            ref="studentBasicClass"
            v-model="model.classGroup.basicClassId"
            :api="additionalEnrollment ? '/api/lookups/GetBasicClassesForLoggedUser' : '/api/lookups/GetBasicClassValidOptions'"
            :filter="{relatedObject: institutionId}"
            :label="$t('student.basicClass')"
            :placeholder="$t('buttons.search')"
            hide-no-data
            :disabled="model.classGroup.basicClassId !== null && disableComponentByNotPresentFormClassSelected"
            :rules="!disableComponentByNotPresentFormClassSelected || model.classGroup.basicClassId === null ? [$validator.required()] : []"
            :class="!disableComponentByNotPresentFormClassSelected || model.classGroup.basicClassId === null ? 'required' : ''"
            :defer-options-loading="false"
            no-filter
            loading
          />
        </v-col>
        <v-col
          v-if="!cplrEnrollment && showClassTypeOptions"
          cols="12"
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
          v-if="!cplrEnrollment && model.classGroup && (!additionalEnrollment || isNotPresentFormClassSelected) && (!isKindergartenInstitution || (isKindergartenInstitution && model.classGroup.classKindId !== ClassKind.Basic))"
          cols="12"
          md="4"
        >
          <v-select
            ref="classEduFormName"
            v-model="model.studentEduFormId"
            :label="$t('enroll.classEduFormName')"
            :items="filteredEducationFormOptions"
            :rules="[$validator.required(true)]"
            class="required"
          />
        </v-col>
        <v-col
          v-if="!isKindergartenInstitution && !cplrEnrollment && model.classGroup && (!additionalEnrollment || isNotPresentFormClassSelected)"
          cols="12"
          md="4"
        >
          <custom-autocomplete
            id="professionId"
            ref="professionId"
            v-model="model.studentProfessionId"
            api="/api/lookups/GetSPPOOProfession"
            :label="$t('student.profession')"
            :placeholder="$t('buttons.search')"
            hide-no-data
            :disabled="model.classGroup.classProfessionId === null && disableComponentByNotPresentFormClassSelected"
            :defer-options-loading="false"
            loading
            clearable
            @change="onStudentProfessionChange"
          />
        </v-col>
        <v-col
          v-if="!isKindergartenInstitution && !cplrEnrollment && model.classGroup && (!additionalEnrollment || isNotPresentFormClassSelected)"
          cols="12"
          md="4"
        >
          <custom-autocomplete
            id="specialityId"
            ref="specialityId"
            v-model="model.studentSpecialityId"
            :filter="{relatedObject: model.studentProfessionId}"
            api="/api/lookups/GetSPPOOSpecialityExtendedText"
            :label="$t('student.speciality')"
            :placeholder="$t('buttons.search')"
            hide-no-data
            :defer-options-loading="false"
            clearable
            :disabled="model.classGroup.classSpecialityId === null && disableComponentByNotPresentFormClassSelected"
            :rules="model.studentProfessionId ? [$validator.required()] : []"
            :class="model.studentProfessionId ? 'required' : ''"
            loading
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
            :min="model.enrollmentDate"
            :label="$t('documents.entryDateLabel')"
          />
        </v-col>
      </v-row>
      <v-alert
        v-if="isStudentClassCahange"
        border="left"
        colored-border
        type="info"
        elevation="2"
      >
        {{ $t('enroll.studentClassChangeEnrollmentDateInfo') }}
      </v-alert>
    </v-card-text>
  </v-card>
</template>

<script>
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import CustomAutocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import { InstType, ClassKind } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "StudentClassBasicData",
  components: {
    SchoolYearSelector,
    CustomAutocomplete,
  },
  props: {
    value: {
      type: Object,
      required: true,
    },
    minSchoolYear: {
      // Минимална стойност за селектора за учебната година
      type: Number,
      default() {
        return 2001;
      },
    },
    maxSchoolYear: {
      // Максимална стойност за селектора за учебната година
      type: Number,
      default() {
        return 3001;
      },
    },
    institutionId: {
      // Групи/класове от точно определена институция.
      type: Number,
      default() {
        return undefined;
      },
    },
    basicClass: {
      // Групи от точно определен клас (1,2,3,4 и т.н.)
      type: Number,
      default() {
        return undefined;
      },
    },
    minBasicClass: {
      // Групи от минимален клас (1,2,3,4 и т.н.)
      type: Number,
      default() {
        return undefined;
      },
    },
    isInitialEnrollment: {
      type: Boolean,
      default() {
        return undefined;
      },
    },
    additionalEnrollment: {
      type: Boolean,
      default() {
        return false;
      }
    },
    cplrEnrollment: {
      type: Boolean,
      default() {
        return false;
      }
    },
    minEnrollmentDate: {
      type: String,
      default() {
        return undefined;
      }
    },
    isStudentClassCahange: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.value,
      classGroupsOptions: [],
      educationFormOptions: [],
      specialityFormOptions: [],
      professionOptions: [],
      filteredEducationFormOptions: [],
      classTypeOptions: [],
      classTypeRequired: true,
      ClassKind: ClassKind
    };
  },
  computed: {
    ...mapGetters(['isInstType']),
    isKindergartenInstitution() {
      return this.isInstType(InstType.Kindergarten);
    },
    isNotPresentFormClassSelected() {
      if(!this.model || !this.model.classId || !this.classGroupsOptions) {
        return false;
      }

      const selecteClass = this.classGroupsOptions.find(x => x.value == this.model.classId);

      return !selecteClass
        ? false
        : selecteClass.isNotPresentForm;
    },
    disableComponentByNotPresentFormClassSelected() {
      return (this.model.classId && !this.isNotPresentFormClassSelected);
    },
    showClassTypeOptions() {
      if(this.isNotPresentFormClassSelected && this.model.classGroup && this.model.classGroup.basicClassId) {
        this.loadClassTypeOptions();
      }

      return this.isNotPresentFormClassSelected && this.model.classGroup && this.model.classGroup.basicClassId;
    },
  },
  watch: {
    'model.schoolYear': function (newValue, oldVal) {
      if (oldVal !== "") {
        this.model.classId = "";
      }
      this.loadClassGroupsOptions(newValue);
    },
    //когато се сетне personId зареждаме данните за classGroup-а му.
    // 'model.personId': function(){
    //   this.loadClassGroupDataForPerson(this.model.personId);
    // },

    //При избор на друга паралелка трябва да заредим данните от classGroup - id, specialityId, professionId, eduForm
    'model.classId': function(newVal, oldVal) {
      //model.classId се зарежда от подадените данни от studentClassChange и първоначално няма нужда да зареждаме данните, защото вече ги имаме.
      if(oldVal !== 0) {
        if(this.model.classGroup) {
          this.model.classGroup.basicClassId = null;
        }
        this.model.selectedClassTypeId = null;
        this.loadClassGroupBasicClass(this.model.classId);
      }
    },
    //когато classGroup няма basicClass даваме възможност да се избере клас и да се запише в studentClass.
    //v-model е model.classGroup.basicClassId, но трябва да се запише в model.basicClassId - който е класът на студента
    'model.classGroup.basicClassId': function(newVal, oldVal) {
      if (oldVal !== undefined) {
        this.model.basicClassId = this.model.classGroup
          ? this.model.classGroup.basicClassId
          : null;
      }

      if(this.isNotPresentFormClassSelected) {
        this.model.selectedClassTypeId = null;
      }
    }
  },
  mounted() {
    this.loadEducationValidOptions(this.model.classId);
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
    loadClassGroupsOptions(schoolYear) {
      if (!schoolYear) {
        this.classGroupsOptions = [];
        return;
      }

      if(this.additionalEnrollment) {
        this.loadClassGroupOptionsForAdditionalEnrollment(schoolYear);
      } else {
        this.loadClassGroupOptionsForInitialEnrollment(schoolYear);
      }
    },
    loadClassGroupOptionsForInitialEnrollment(schoolYear) {
      this.$api.lookups
        .getClassGroupsOptions(
          this.institutionId,
          schoolYear,
          this.basicClass,
          this.minBasicClass,
          this.model.personId,
          this.isInitialEnrollment
        )
        .then((response) => {
          this.classGroupsOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.classGroupsOptionsLoad"));
          console.log(error);
        });
    },
    loadClassGroupOptionsForAdditionalEnrollment(schoolYear) {
      this.$api.lookups.getClassGroupsForLoggedUser(schoolYear, this.model?.personId)
        .then((response) => {
          this.classGroupsOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.classGroupsOptionsLoad"));
          console.log(error);
        });
    },
    //Зареждаме валидните форми на обучение за студент
    loadEducationValidOptions() {
      this.$api.lookups
        .getValidEducationFormsOptionsForPerson()
        .then((response) => {
          const educationFormOptions = response.data;
          if (this.educationFormOptions) {
            this.educationFormOptions = educationFormOptions;
            this.filterEduForms();
          }
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("errors.educationTypeOptionsLoad"));
          console.log(error.response);
        });
    },
    loadClassGroupBasicClass(id){
      if(!id) {
        // Това се случва когато изберем учебна година,
        // за която няма въведени паралелки за дадената институция.
        // Примерно следващата учебна година.

        if(this.model) {
          if(this.model.classGroup) this.model.classGroup = null;
          if(this.model.studentProfessionId) this.model.studentProfessionId = null;
          if(this.model.studentSpecialityId) this.model.studentSpecialityId = null;
          if(this.model.studentEduFormId) this.model.studentEduFormId = null;
          if(this.model.basicClassId) this.model.basicClassId = null;
        }

        return;
      }

      this.$api.classGroup.getById(id).then((response) => {
        this.model.classGroup = response.data;
        const classGroup = response.data;
        this.model.classGroup.classProfessionId = response.data.classProfessionId;
        this.model.studentProfessionId = response.data.classProfessionId;
        this.model.studentSpecialityId = response.data.classSpecialityId;
        this.model.classGroup.classSpecialityId = response.data.classSpecialityId;
        this.model.studentEduFormId = response.data.classEduFormId;
        this.model.basicClassId = classGroup?.basicClassId;
        this.model.classGroup.basicClassId = classGroup?.basicClassId;
        this.filterEduForms();
     });
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
    }
  },
};
</script>
