<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row dense>
        <v-col
          cols="12"
          sm="6"
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
            show-current-school-year-button
            :clearable="false"
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="2"
        >
          <date-picker
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
          lg="8"
        >
          <v-autocomplete
            v-if="model.positionId !== positions.ProfessionalEducation"
            id="classId"
            ref="studentClass"
            v-model="model.classId"
            :label="$t('enroll.class')"
            name="classId"
            item-text="text"
            item-value="value"
            :rules="[$validator.required()]"
            :items="classGroupsOptions"
            clearable
            class="required"
          >
            <template v-slot:[`item`]="{ item }">
              <span
                v-if="!item.disabled"
              >
                {{ item.text }}
              </span>
              <span
                v-else
              >
                <v-chip
                  class="ma-2"
                  color="orange"
                  outlined
                  small
                >
                  {{ item.text }}
                </v-chip>
              </span>

              <span
                v-if="item.hasExternalSoProviderLimitation"
              >
                <v-chip
                  class="ma-2"
                  color="error"
                  outlined
                  small
                >
                  {{ $t('common.externalSoProviderClassLimitation') }}
                </v-chip>
              </span>
            </template>
          </v-autocomplete>
          <c-text-field
            v-else
            disabled
            :value="`${model.classGroup?.className} - ${model.classGroup?.classEduFormName} - ${model.classGroup?.classTypeName}`"
            :label="$t('enroll.class')"
            outlined
            persistent-placeholder
            dense
          />
        </v-col>
        <!-- <v-col
          v-if="model.positionId === positions.ProfessionalEducation"
          cols="12"
          lg="6"
        >
          <c-text-field
            disabled
            :value="model.profession"
            :label="$t('student.profession')"
            outlined
            persistent-placeholder
            dense
          />
        </v-col> -->
        <v-col
          v-if="model.positionId === positions.ProfessionalEducation"
          cols="12"
          lg="6"
        >
          <c-text-field
            disabled
            :value="model.eduForm"
            :label="$t('enroll.classEduFormName')"
            outlined
            persistent-placeholder
            dense
          />
        </v-col>
      </v-row>
    </v-form>
  </div>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { StudentClassBaseModel } from '@/models/studentClass/studentClassBaseModel';
import { mapGetters } from "vuex";
import { Positions } from '@/enums/enums';

export default {
  name: "EnrollmentFormComponent",
  components: {
    SchoolYearSelector
  },
  props: {
    document: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    }
  },
  data() {
    return {
      model: this.document ?? new StudentClassBaseModel(),
      classGroupsOptions: [],
      saving: false,
      minSchoolYear: undefined,
      maxSchoolYear: undefined,
      positions: Positions
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId']),
  },
  watch: {
    'model.schoolYear': function(newValue, oldVal) {
      if (oldVal) {
        this.model.classId = "";
      }
      this.loadClassGroupsOptions(newValue);
    }
  },
  mounted() {
    this.$api.institution.getCurrentYear(this.userInstitutionId)
    .then(response => {
      const currentYear = Number(response.data);
      if(!isNaN(currentYear)) {
        this.model.schoolYear = currentYear;
        this.minSchoolYear = currentYear;
        this.maxSchoolYear = currentYear + 1;
      }
    });
  },
  methods: {
    loadClassGroupsOptions(schoolYear) {
      if(!schoolYear || !this.model || !this.model.personId) {
        this.classGroupsOptions = [];
        console.log("SchoolYear,model or personId is undefined.");
        return;
      }

      const params = { personId: this.model?.personId, schoolYear: schoolYear, isAdditionalEnrollment: true };

      this.$api.institution.getClassGroupsForAdditionalEnrollment(params)
        .then((response) => {
          this.classGroupsOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.classGroupsOptionsLoad'));
          console.log(error.response);
        });
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
