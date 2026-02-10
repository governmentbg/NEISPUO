<template>
  <div>
    <v-card>
      <v-alert
        dense
        border="bottom"
        colored-border
        type="warning"
        elevation="1"
      >
        {{ $t('healthInsurance.studentsList.rangeChangeInfo') }}
      </v-alert>
      <v-card-title>
        <school-year-selector
          v-model="selectedYear"
          item-text="value"
          :label="$t('common.year')"
          :clearable="false"
        />
        <custom-month-picker
          v-model="selectedMonth"
          :clearable="false"
        />
        <v-spacer />
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          :label="$t('buttons.search')"
          clearable
          single-line
          hide-details
        />
      </v-card-title>
      <v-card-text>
        <v-data-table
          v-if="schoolYearAndMonthSelected"
          :items="pagedListOfItems.items"
          :headers="headers"
          :search="search"
          :loading="loading"
          :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true, showFirstLastPage: true }"
          multi-sort
          dense
        >
          <template v-slot:top>
            <v-toolbar flat>
              <GridExporter
                :items="pagedListOfItems.items"
                :file-extensions="['xlsx', 'csv', 'txt']"
                file-name="Данни за здравно осигуряване"
                :headers="headers"
              />
              <v-spacer />
            </v-toolbar>
          </template>
          <template v-slot:[`header.isExcludeFromList`]="{ header }">
            {{ header.text }}
            <v-tooltip bottom>
              <template v-slot:activator="{ on: tooltip }">
                <v-simple-checkbox
                  v-model="allExcludedCheck"
                  color="primary"
                  dense
                  v-on="{ ...tooltip }"
                />
              </template>
              <span>{{ allExcludedCheck ? $t('buttons.uncheckAll') : $t('buttons.checkAll') }}</span>
            </v-tooltip>
          </template>

          <template v-slot:[`item.pin`]="{ item }">
            {{ `${item.pin} - ${item.pinType}` }}
          </template>

          <template v-slot:[`item.birthDate`]="{ item }">
            {{ item.birthDate ? $moment(item.birthDate).format(dateFormat) : "" }}
          </template>

          <template v-slot:[`item.startDayNumber`]="{ item }">
            <v-edit-dialog
              :return-value.sync="item.startDayNumber"
              large
              :save-text="$t('buttons.save')"
              :cancel-text="$t('buttons.cancel')"
              @save="onItemEdit(item)"
            >
              <v-text-field
                :value="item.startDayNumber"
                prepend-icon="mdi-pencil"
                readonly
              />
              <template v-slot:input>
                <number-selector
                  v-model="item.startDayNumber"
                  :min="1"
                  :max="31"
                  :label="$t('healthInsurance.studentsList.headers.startDayNumber')"
                  single-line
                  hint="Enter за запис. Esc за изход"
                  persistent-hint
                />
              </template>
            </v-edit-dialog>
          </template>

          <template v-slot:[`item.endDayNumber`]="{ item }">
            <v-edit-dialog
              :return-value.sync="item.endDayNumber"
              large
              :save-text="$t('buttons.save')"
              :cancel-text="$t('buttons.cancel')"
              @save="onItemEdit(item)"
            >
              <v-text-field
                :value="item.endDayNumber"
                prepend-icon="mdi-pencil"
              />
              <template v-slot:input>
                <number-selector
                  v-model="item.endDayNumber"
                  :min="1"
                  :max="31"
                  :label="$t('healthInsurance.studentsList.headers.startDayNumber')"
                  single-line
                  hint="Enter за запис. Esc за изход"
                  persistent-hint
                />
              </template>
            </v-edit-dialog>
          </template>

          <template
            v-slot:[`item.isExcludeFromList`]="{ item }"
          >
            <v-checkbox
              v-model="item.isExcludeFromList"
              color="primary"
              dense
            />
          </template>

          <template v-slot:[`item.minimalInsuranceIncomeRate`]="{ item }">
            <span v-if="currency.showAltCurrency">{{ item.minimalInsuranceIncomeRateStr }}</span>
            <span v-else>{{ item.minimalInsuranceIncomeRate }}</span>
          </template>

          <template v-slot:[`item.insurancePayRate`]="{ item }">
            <span v-if="currency.showAltCurrency">{{ `${calcInsurancePayRate(item)} ${currency.currency.code} / ${calcAltInsurancePayRate(item)} ${currency.altCurrency.code}` }}</span>
            <span v-else>{{ calcInsurancePayRate(item) }}</span>
          </template>

          <template
            v-slot:[`item.correctionCode`]="{ item }"
          >
            <v-radio-group
              v-model="item.correctionCode"
              row
              dense
            >
              <v-radio
                :label="$t('healthInsurance.correctionCode.reqular')"
                :value="0"
              />
              <v-radio
                :label="$t('healthInsurance.correctionCode.corrective')"
                :value="1"
              />
              <v-radio
                :label="$t('healthInsurance.correctionCode.deleting')"
                :value="8"
              />
            </v-radio-group>
          </template>

          <template v-slot:[`footer.prepend`]>
            <button-group>
              <v-btn
                v-if="schoolYearAndMonthSelected"
                small
                color="secondary"
                outlined
                @click="load"
              >
                {{ $t('buttons.reload') }}
              </v-btn>
            </button-group>
          </template>
        </v-data-table>
        <v-alert
          v-else
          border="top"
          colored-border
          type="info"
          elevation="2"
        >
          Моля, изберете година и месец!
        </v-alert>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <button-tip
          icon-name="fas fa-save"
          tooltip="healthInsurance.studentsList.fileCreateBtnTooltip"
          text="healthInsurance.studentsList.fileCreateBtnText"
          bottom
          raised
          iclass="mx-2"
          @click="generateFile"
        />
      </v-card-actions>
      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
    </v-card>
    <exported-files-list
      :ref="'exportedFilesGrid' + _uid"
      class="mt-2"
    />
  </div>
</template>

<script>
import CustomMonthPicker from "@/components/wrappers/CustomMonthPicker";
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import ExportedFilesList from "@/components/healthInsurance/HealthInsuranceExportedFilesList.vue";
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';
import { Permissions } from "@/enums/enums";
import GridExporter from "@/components/wrappers/gridExporter";

export default {
  name: "HealthInsuranceStudentsListComponent",
  components: {
    CustomMonthPicker,
    SchoolYearSelector,
    ExportedFilesList,
    GridExporter
  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      selectedYear: null,
      selectedMonth: null,
      search: '',
      loading: false,
      saving: false,
      allExcludedCheck: false,
      pagedListOfItems: {
        totalCount: 0,
        items: [],
      },
      headers: [
        { text: this.$t("healthInsurance.studentsList.headers.pin"), value: "pin", groupable: false },
        { text: this.$t("healthInsurance.studentsList.headers.name"), value: "fullName", groupable: false  },
        { text: this.$t("healthInsurance.studentsList.headers.birthDate"), value: "birthDate", groupable: false  },
        { text: this.$t("healthInsurance.studentsList.headers.className"), value: "className", groupable: true },
        { text: this.$t("healthInsurance.studentsList.headers.startDayNumber"), value: "startDayNumber", filterable: false, sortable: false, groupable: false },
        { text: this.$t("healthInsurance.studentsList.headers.endDayNumber"), value: "endDayNumber", filterable: false, sortable: false, groupable: false },
        { text: this.$t("healthInsurance.studentsList.headers.minimalInsuranceIncomeRate"), value: "minimalInsuranceIncomeRate", filterable: false, sortable: false, groupable: false },
        { text: this.$t("healthInsurance.studentsList.headers.insurancePayRate"), value: "insurancePayRate", filterable: false, sortable: false, groupable: false },
        { text: this.$t("healthInsurance.studentsList.headers.excludeFromListCheck"), value: "isExcludeFromList", filterable: false, sortable: false, groupable: false },
        { text: this.$t("healthInsurance.studentsList.headers.correctionCode"), value: "correctionCode", filterable: false, sortable: false, groupable: false },
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'gridItemsPerPageOptions', 'currency']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForHealthInsuranceManage);
    },
    schoolYearAndMonthSelected() {
      return this.selectedYear && this.selectedMonth;
    },
  },
  watch: {
    selectedYear() {
      this.allExcludedCheck = false;
      if(!this.selectedYear) {
        return;
      }

      this.load();
    },
    selectedMonth() {
      this.allExcludedCheck = false;
      if(!this.selectedMonth) {
        return;
      }

      this.load();
    },
    allExcludedCheck(val) {
      if (!this.pagedListOfItems.items || !Array.isArray(this.pagedListOfItems.items)) return;

      this.pagedListOfItems.items.forEach(x => {
        x.isExcludeFromList = val;
      });
    }
  },
  async beforeMount() {
    if (!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.selectedMonth = this.$helper.getMonth();
    await this.getCurrentSchoolYear();
  },

  methods: {
    async getCurrentSchoolYear() {
      try {
        const currentSchoolYear = Number((await this.$api.institution.getCurrentYear())?.data);
        if(!isNaN(currentSchoolYear)) {
          this.selectedYear = currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    },
    load() {
      if(!this.schoolYearAndMonthSelected) {
        return;
      }

      this.loading = true;
      this.$api.healthInsurance.list(this.selectedYear, this.selectedMonth)
        .then((response) => {
          this.pagedListOfItems = response.data;
        })
        .catch(error => {
          console.log(error.response);
        })
        .then(() => (this.loading = false));
    },
    generateFile() {
      this.saving = true;

      this.$api.healthInsurance.generateFile({
        year: this.selectedYear,
        month: this.selectedMonth,
        healthInsuranceStudentsModel: this.pagedListOfItems.items
      })
      .then(() => {
        this.$notifier.success(this.$t('healthInsurance.studentsList.fileCreateBtnText'), this.$t('common.saveSuccess'));

        this.exportsGridReload();
      })
      .catch((error) => {
        this.$notifier.error(this.$t('healthInsurance.studentsList.fileCreateBtnText'), this.$t('common.saveError'));
        console.log(error.response);
      })
      .finally(() => {
        this.saving = false;
      });
    },
    exportsGridReload() {
      const grid = this.$refs['exportedFilesGrid' + this._uid];
      if(grid) {
        grid.gridReload();
      }
    },
    onItemEdit(item){
      this.calcInsurancePayRate(item);
      this.calcAltInsurancePayRate(item);

      const editedIndex = this.pagedListOfItems.items.indexOf(item);
      this.pagedListOfItems.items[editedIndex].insurancePayRate = this.calcInsurancePayRate(item);
      this.pagedListOfItems.items[editedIndex].altInsurancePayRate = this.calcAltInsurancePayRate(item);
    },
    calcInsurancePayRate(item) {
      if(!item) return;

      const insurancePayRate = ((item.minimalInsuranceIncomeRate / item.monthDays) * (item.endDayNumber - item.startDayNumber + 1)).toFixed(2);
      return insurancePayRate;
    },
    calcAltInsurancePayRate(item) {
      if(!item) return;

      const insurancePayRate = ((item.altMinimalInsuranceIncomeRate / item.monthDays) * (item.endDayNumber - item.startDayNumber + 1)).toFixed(2);
      return insurancePayRate;
    }
  }
};
</script>
