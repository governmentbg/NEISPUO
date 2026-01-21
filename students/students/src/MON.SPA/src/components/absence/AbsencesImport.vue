<template>
  <v-card>
    <v-card-title>{{ $t('absence.title') }}</v-card-title>
    <v-card-subtitle>
      {{ $t('absence.importSubtitle') }}
    </v-card-subtitle>
    <v-alert
      border="top"
      colored-border
      type="info"
      elevation="2"
    >
      ВАЖНО! Във файла с отсъствията не е задължително да бъдат включени всички записани в институцията деца! Системата ще обработи успешно файл с данни само за децата, които за периода имат отсъствия по уважителни или неуважителни причини."
    </v-alert>
    <confirm-dlg ref="confirm" />
    <v-tabs
      v-model="tab"
      color="deep-purple accent-4"
      centered
    >
      <v-tab
        v-if="importType === '1'"
        key="importFromSchoolBooks"
      >
        {{ $t('absence.importDataFromSchoolBooks') }}
      </v-tab>
      <v-tab
        v-if="importType === '2'"
        key="importFromFile"
      >
        {{ $t('absence.importDataFromFile') }}
      </v-tab>
      <v-tab
        v-if="importType === '3'"
        key="importDataFromManualEntry"
      >
        {{ $t('absence.importDataFromManualEntry') }}
      </v-tab>

      <v-tabs-items
        v-model="tab"
      >
        <v-tab-item
          v-if="importType === '1'"
          key="importFromSchoolBooks"
        >
          <v-card-text>
            <v-row dense>
              <v-col>
                <school-year-selector
                  v-model="schoolYear"
                  class="required"
                  :institution-id="institutionId"
                  :rules="[$validator.required()]"
                />
              </v-col>
              <v-col>
                <custom-month-picker
                  v-model="month"
                  :rules="[$validator.required()]"
                  class="required"
                  :disabled="!schoolYear"
                />
              </v-col>
            </v-row>
            <v-card-actions class="justify-end">
              <button-group>
                <button-tip
                  icon-name="fa-file-import"
                  tooltip="absence.importFromSchoolBooksTooltip"
                  text="buttons.import"
                  bottom
                  raised
                  iclass="mx-2"
                  :disabled="!month || !schoolYear"
                  @click="importFromSchoolBooks()"
                />
              </button-group>
            </v-card-actions>
          </v-card-text>
        </v-tab-item>
        <v-tab-item
          v-if="importType === '2'"
          key="importFromFile"
        >
          <v-card-text>
            <v-row dense>
              <v-col>
                <div class="large-12 medium-12 small-12 cell">
                  <v-file-input
                    ref="file"
                    v-model="file"
                    label="Файл"
                    show-size
                    truncate-length="50"
                  />
                </div>
              </v-col>
            </v-row>
            <v-row>
              <v-col
                sm="6"
                md="2"
              >
                <school-year-selector
                  v-model="schoolYearInFile"
                  :institution-id="institutionId"
                  disabled
                />
              </v-col>
              <v-col
                sm="6"
                md="2"
              >
                <custom-month-picker
                  v-model="monthInFile"
                  disabled
                />
              </v-col>
              <v-col
                sm="12"
                md="8"
              >
                <v-text-field
                  :value="institutionInFile"
                  :label="$t('absence.institution')"
                  disabled
                />
              </v-col>
            </v-row>
            <v-card-actions class="justify-end">
              <button-group>
                <button-tip
                  ref="submit"
                  icon-name="fa-file-import"
                  tooltip="absence.fileImportText"
                  text="buttons.import"
                  raised
                  color="primary"
                  type="submit"
                  :disabled="!file || !monthInFile"
                  @click="submitFile()"
                />
              </button-group>
            </v-card-actions>
          </v-card-text>
        </v-tab-item>
        <v-tab-item
          v-if="importType === '3'"
          key="importDataFromManualEntry"
        >
          <v-card-text>
            <v-row dense>
              <v-col>
                <school-year-selector
                  v-model="manualImportSchoolYear"
                  class="required"
                  :institution-id="institutionId"
                  :rules="[$validator.required()]"
                />
              </v-col>
              <v-col>
                <custom-month-picker
                  v-model="manualImportMonth"
                  :rules="[$validator.required()]"
                  class="required"
                  :disabled="!manualImportSchoolYear"
                />
              </v-col>
            </v-row>

            <v-data-table
              v-if="manualImportSampleData && manualImportSampleData.length > 0"
              ref="absenceImportedFileDataGrid"
              :headers="headers"
              :items="manualImportSampleData"
              :search="search"
              :loading="loading"
              :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true }"
              multi-sort
              class="elevation-1"
            >
              <template v-slot:top>
                <v-toolbar flat>
                  <v-spacer />
                  <v-text-field
                    v-model="search"
                    append-icon="mdi-magnify"
                    :label="$t('common.search')"
                    single-line
                    hide-details
                  />
                </v-toolbar>
              </template>
            </v-data-table>

            <v-card-actions class="justify-end">
              <button-group>
                <button-tip
                  icon-name="fa-file-import"
                  tooltip="absence.importDataFromManualEntry"
                  text="buttons.import"
                  bottom
                  raised
                  iclass="mx-2"
                  :disabled="!manualImportMonth || !manualImportSchoolYear"
                  @click="importFromManualEntry"
                />
              </button-group>
            </v-card-actions>
          </v-card-text>
        </v-tab-item>
      </v-tabs-items>
    </v-tabs>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import CustomMonthPicker from '@/components/wrappers/CustomMonthPicker';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Constants from "@/common/constants.js";

import { AppSettingKeys } from '@/enums/enums';
import { mapGetters } from "vuex";

export default {
  name: "AbsencesImportComponent",
  components: { CustomMonthPicker, SchoolYearSelector },
  props: {
    institutionId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      search: '',
      currentSchoolYear: null,
      file: null,
      month: null,
      monthInFile: null,
      schoolYear: null,
      schoolYearInFile: null,
      institutionInFile: null,
      tab: null,
      saving: false,
      importType: null,
      appSettingKey: AppSettingKeys.AbsenceImportType,
      manualImportSchoolYear: null,
      manualImportMonth: null,
      manualImportSampleData: [],
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      headers: [
        {
          text: this.$t('absence.importFile.headers.institutionCode'),
          value: 'institutionCode',
        },
        {
          text: this.$t('absence.importFile.headers.schoolYear'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('absence.importFile.headers.month'),
          value: 'month',
        },
        {
          text: this.$t('absence.importFile.headers.studentIdentificationType'),
          value: 'studentIdentificationType',
        },
        {
          text: this.$t('absence.importFile.headers.identification'),
          value: 'identification',
        },
        {
          text: this.$t('absence.importFile.headers.firstName'),
          value: 'firstName',
        },
        {
          text: this.$t('absence.importFile.headers.middleName'),
          value: 'middleName',
        },
        {
          text: this.$t('absence.importFile.headers.lastName'),
          value: 'lastName',
        },
        {
          text: this.$t('absence.importFile.headers.class'),
          value: 'class',
        },
        {
          text: this.$t('absence.importFile.headers.classCode'),
          value: 'classCode',
        },
        {
          text: this.$t('absence.importFile.headers.groupCode'),
          value: 'groupCode',
        },
        {
          text: this.$t('absence.importFile.headers.className'),
          value: 'className',
        },
        {
          text: this.$t('absence.importFile.headers.monthlyAbsencesForValidReason'),
          value: 'monthlyAbsencesForValidReason',
        },
        {
          text: this.$t('absence.importFile.headers.monthlyAbsencesForUnvalidReason'),
          value: 'monthlyAbsencesForUnvalidReason',
        },
      ],
    };
  },
  computed:{
    ...mapGetters(['gridItemsPerPageOptions', 'userInstitutionId', 'userDetails']),
  },
  watch: {
    async file (newValue) {
      await this.parseFile(newValue);
    },
    manualImportSchoolYear() {
      this.loadManualImportSampleData();
    },
    manualImportMonth() {
      this.loadManualImportSampleData();
    },
  },
  async mounted() {
    this.loadImportTypeAppSetting();

    await this.getCurrentSchoolYear();
    this.schoolYear = this.currentSchoolYear;
    this.manualImportSchoolYear = this.currentSchoolYear;
  },
  methods: {
    loadImportTypeAppSetting() {
      this.loading = true;
      this.$api.administration.getTenantAppSetting(this.appSettingKey)
      .then((response) => {
        if(response.data) {
          this.importType = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('errors.load'));
      })
      .then(() => {
        this.loading = false;
      });
    },
    async parseFile(file) {
      if(!file) {
        this.schoolYearInFile = null;
        this.monthInFile = null;
        this.institutionInFile = null;
        return;
      }

      let formData = new FormData();
      formData.append("file", this.file);

      this.saving = true;
      this.$api.absence.readDateAndMonthFromFile(formData)
      .then((response) => {
        this.$notifier.success('', this.$t('absence.validationSuccess'));
        this.schoolYearInFile = response.data.schoolYear;
        this.monthInFile = response.data.month;
        this.institutionInFile = response.data.institutionName;
      })
      .catch((error) => {
        const { message, errors } = this.$helper.parseError(error.response);
        this.$notifier.modalError(message, errors);
        this.$helper.logError({ action: 'AbsenceImportParseFile', message: message}, errors, this.userDetails);
      })
      .then(() => {
        this.saving = false;
      });
    },
    submitFile() {
      if(!this.schoolYearInFile) {
        return this.$notifier.modalError(this.$t('absence.importDataFromFile'), this.$t('absence.missingSelectedSchoolYear'));
      }

      if(!this.monthInFile) {
        return this.$notifier.modalError(this.$t('absence.importDataFromFile'), this.$t('absence.missingSelectedMonth'));
      }

      let formData = new FormData();
      formData.append("file", this.file);

      this.saving = true;
      this.$api.absence
        .upload(formData)
        .then(() => {
          this.$notifier.success('', this.$t('absence.importSuccess'));
          this.$emit('submitFile');
        })
        .catch((error) => {
          const {message, errors} = this.$helper.parseError(error.response);
          this.$notifier.modalError(message, errors);
          this.$helper.logError({ action: 'AbsenceImportSubmitFile', message: message}, errors, this.userDetails);
        })
        .then(() => {
          this.saving = false;
        });
    },
    async importFromSchoolBooks(){
      this.saving = true;
      this.$api.absence.importAbsencesFromSchoolBooks(this.schoolYear, this.month)
        .then(() => {
          this.$notifier.success('', this.$t('absence.importSuccess'));
          this.$emit('submitFile');
        })
        .catch((error) => {
          const {message, errors} = this.$helper.parseError(error.response);
          this.$notifier.modalError(message, errors);
          this.$helper.logError({ action: 'AbsenceImportFromSchoolBooks', message: message}, errors, this.userDetails);
        })
        .then(() => {
          this.saving = false;
        });
    },
    importFromManualEntry() {
      this.saving = true;
      this.$api.absence.importAbsencesFromManualEntry(this.manualImportSchoolYear, this.manualImportMonth)
        .then(() => {
          this.$notifier.success('', this.$t('absence.importSuccess'));
          this.$emit('submitFile');
        })
        .catch((error) => {
          const {message, errors} = this.$helper.parseError(error.response);
          this.$notifier.modalError(message, errors);
          this.$helper.logError({ action: 'AbsenceImportFromManualEntry', message: message}, errors, this.userDetails);
        })
        .then(() => {
          this.saving = false;
        });
    },
    loadManualImportSampleData() {
      this.manualImportSampleData = [];
      if(!this.manualImportSchoolYear || !this.manualImportMonth) {
        return;
      }

      this.saving = true;
      this.$api.absence.getManualImportSampleData(this.manualImportSchoolYear, this.manualImportMonth)
      .then((response) => {
        if(response.data) {
          this.manualImportSampleData = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('errors.load'));
      })
      .then(() => {
        this.saving = false;
      });
    },
    async getCurrentSchoolYear() {
      try {
        const currentSchoolYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
        if(!isNaN(currentSchoolYear)) {
          this.currentSchoolYear = currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    }
  }
};
</script>
