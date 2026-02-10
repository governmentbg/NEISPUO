<template>
  <v-card>
    <v-card-title>
      {{ $t('asp.enrolledStudentsExportTitle') }}
    </v-card-title>
    <v-card-text>
      <v-radio-group 
        v-model="fileType"
        row
      >
        <v-radio
          color="primary"
          :value="EnrolledStudentsExportFileTypes.EnrolledStudents"
          :label="$t('asp.enrolledStudentsRadioLabel')"
        />
        <v-radio
          color="primary"
          :label="$t('asp.enrolledStudentsCorrectionsRadioLabel')"
          :value="EnrolledStudentsExportFileTypes.EnrolledStudentsCorrections"
        />
      </v-radio-group>
      <v-row>
        <v-col>
          <school-year-selector
            v-model="selectedSchoolYear"
          /> 
        </v-col>
        <v-col>
          <custom-month-picker
            v-model="selectedMonth"
            :disabled="!enrolledStudentsCorrectionsSelected"
          />
        </v-col>
      </v-row>
      <v-card-actions class="justify-end">
        <button-tip
          icon-name="fa-file-export"
          :tooltip="exportBtnTooltip"
          text="asp.enrolledStudentsExportBtnText"
          bottom
          raised
          color="primary"
          type="submit"
          :disabled="exportBtnDisabled"
          @click="submitExport()"
        />       
      </v-card-actions>
    </v-card-text>
    <v-overlay 
      :value="saving"
    >
      <div class="text-center">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </div>
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import CustomMonthPicker from "@/components/wrappers/CustomMonthPicker";
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { EnrolledStudentsExportModel } from '@/models/asp/enrolledStudentsExportModel.js';
import { EnrolledStudentsExportFileTypes } from '@/enums/enums';

export default {
  name: "EnrolledStudentsExport",
  components: {
    CustomMonthPicker,
    SchoolYearSelector
  },
  data() {
    return {
      EnrolledStudentsExportFileTypes,
      saving: false,
      selectedSchoolYear: null,
      selectedMonth: null,
      fileType: EnrolledStudentsExportFileTypes.EnrolledStudents
    };
  },
  computed: {
    enrolledStudentsCorrectionsSelected() {
      return this.fileType === EnrolledStudentsExportFileTypes.EnrolledStudentsCorrections;
    },
    exportBtnDisabled() {
      switch(this.fileType){
        case EnrolledStudentsExportFileTypes.EnrolledStudents: 
          return !this.selectedSchoolYear;
        case EnrolledStudentsExportFileTypes.EnrolledStudentsCorrections: 
          return !(this.selectedSchoolYear && this.selectedMonth);
        default:
          return false;
      }
    },
    exportBtnTooltip() {
      switch(this.fileType){
        case EnrolledStudentsExportFileTypes.EnrolledStudents: 
          return 'asp.enrolledStudentsExportBtnTooltip';
        case EnrolledStudentsExportFileTypes.EnrolledStudentsCorrections: 
          return 'asp.enrolledStudentsCorrectionsExportBtnTooltip';
        default:
          return 'asp.enrolledStudentsExportBtnTooltip';
      }
    }
  },
  watch: {
    fileType() {
      if(this.fileType === EnrolledStudentsExportFileTypes.EnrolledStudents) {
        this.selectedMonth = null;
      }
    }
  },
  methods: {
    submitExport() {
      this.saving = true;

      const model = new EnrolledStudentsExportModel({
        fileType: this.fileType,
        schoolYear: this.selectedSchoolYear,
        month: this.selectedMonth
      });
      
      this.$api.asp.checkForExistingEnrolledStudentsExport(model)
        .then(async (result) => {
          const existingExport = result.data;
          if(!existingExport) {
            this.export(model);
          }
          else {
            this.saving = false;
            if (await this.$refs.confirm.open('', this.$t('asp.existingEnrolledStudentsExportConfirmationTitle'))) {
              this.saving = true;
              this.export(model);
            }
          }
        })
        .catch((error) => {
          this.saving = false;
          this.$notifier.error('', this.$t('asp.checkForExistingEnrolledStudentsExportError'));
          console.log(error.response);
        });
    },
    export(model) {
      this.$api.asp.exportEnrolledStudents(model)
        .then(() => {
          this.$emit('submitFile');
          this.$notifier.success('', this.$t('asp.enrolledStudentsExportSuccess'), 5000);
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('asp.enrolledStudentsExportError'));
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    }
  }
};
</script>