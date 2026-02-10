<template>
  <div>
    <active-absence-campaigns
      class="mb-2"
      @campaignsLoaded="onActiveCampaignsLoaded"
    />
    <v-card>
      <v-card-title>
        <span
          v-if="classAbsenceModel"
        >
          {{ title }}
        </span>

        <v-spacer />
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          :label="$t('common.search')"
          single-line
          hide-details
        />
      </v-card-title>
      <v-card-subtitle>
        <v-row dense>
          <school-year-selector
            v-model="selectedSchoolYear"
            :label="$t('common.schoolYear')"
            disabled
          />
          <month-picker
            v-model="selectedMonth"
            :clearable="false"
            :disabled="!selectedSchoolYear"
            show-current-month-button
          />
        </v-row>

        <v-alert
          v-if="!hasActiveCampaignForSelectedSchoolYearAndMonth"
          border="top"
          colored-border
          type="warning"
          elevation="2"
        >
          {{ $t('absence.missingActiveCampaignForSelectedSchoolYearAndMont') }}
        </v-alert>
      </v-card-subtitle>
      <v-card-text>
        <v-data-table
          :headers="headers"
          :items="absenceItems"
          item-key="personId"
          class="elevation-1"
          :loading="loading"
          :search="search"
          :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
          hide-default-footer
          disable-pagination
        >
          <template v-slot:top>
            <v-alert
              v-if="!selectedMonth"
              border="top"
              colored-border
              type="info"
              elevation="2"
            >
              За да се заредят данни в списъка, изберете месец.
            </v-alert>
            <v-toolbar
              flat
            >
              <GridExporter
                v-if="classAbsenceModel"
                :items="absenceItems"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="classAbsenceModel.name + ' клас'"
                :headers="headers"
              />
            </v-toolbar>
          </template>

          <template v-slot:[`item.actions`]="{ item }">
            <button-group>
              <button-tip
                icon
                icon-name="mdi-eye"
                icon-color="primary"
                tooltip="buttons.details"
                bottom
                iclass=""
                small
                :to="`/student/${item.personId}/details`"
              />
              <!-- Редакцията/Добавянето на отсъствие е видимо
              ако потребителя е избрал опцията за ръчно въвеждане
              от профилното си меню -->
              <edit-absence
                v-if="importType === '3' && hasClassAbsenceManagePermission && hasActiveCampaignForSelectedSchoolYearAndMonth"
                :student-absence-model="item"
                :disabled="item.isLodFinalized"
                :tooltip-text="item.isLodFinalized ? $t('student.signedLodEditTooltip') : ''"
                @reset="load"
              />
            </button-group>
          </template>

          <template slot="body.append">
            <tr>
              <th
                class="title text-right"
                colspan="3"
              >
                <span
                  v-if="classAbsenceModel && classAbsenceModel.studentAbsences"
                >
                  {{ $t('absence.total') + ' ' + classAbsenceModel.studentAbsences.length + ' ' + $t('absence.students') }}
                </span>
              </th>
            </tr>
          </template>
        </v-data-table>

        <confirm-dlg ref="confirm" />
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import MonthPicker from "@/components/wrappers/CustomMonthPicker";
import EditAbsence from "@/components/absence/EditAbsence";
import ActiveAbsenceCampaigns from '@/components/absence/ActiveAbsenceCampaigns.vue';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { AppSettingKeys } from '@/enums/enums';

export default {
  name: 'AbsenceClass',
  components: { GridExporter, SchoolYearSelector, MonthPicker, EditAbsence, ActiveAbsenceCampaigns },
  props:{
    classId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    }
  },
  data(){
    return {
      classAbsenceModel: null,
      loading: false,
      search: '',
      headers: [
          {
              text: this.$t('absence.headers.classNumber'),
              value: 'classNumber'
          },
          {
              text:  this.$t('absence.headers.studentName'),
              value: 'name'
          },
          {
              text:  this.$t('absence.headers.excused'),
              value: 'excused'
          },
          {
              text: this.$t('absence.headers.unexcused'),
              value: 'unexcused'
          },
          { text: '', value: 'actions', sortable: false, align: 'end' },
      ],
      editedIndex: -1,
      editedItem: {
          excused: 0,
          unexcused: 0
      },
      selectedSchoolYear: this.schoolYear,
      selectedSchoolYearName: '',
      selectedMonth: null,
      importType: null,
      appSettingKey: AppSettingKeys.AbsenceImportType,
      activeCampaigns: null
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions', 'hasClassPermission']),
    absenceItems() {
      return this.classAbsenceModel
        ? this.classAbsenceModel.studentAbsences
        : [];
    },
    hasClassAbsenceManagePermission() {
      return this.hasClassPermission(Permissions.PermissionNameForClassAbsenceManage);
    },
    title() {
      return this.$t('absence.classAbsence', {
        class: this.classAbsenceModel?.name ?? '',
        schoolYearName: this.selectedSchoolYearName ?? '',
        month: this.selectedMonth ? this.$helper.getMonthName(this.selectedMonth) : ''
      });
    },
    hasActiveCampaignForSelectedSchoolYearAndMonth() {
      return this.selectedSchoolYear && this.selectedMonth && Array.isArray(this.activeCampaigns)
        && this.activeCampaigns.some(x => x.schoolYear === this.selectedSchoolYear && x.month === this.selectedMonth);
    },
  },
  watch:{
    selectedSchoolYear() {
      if(!this.selectedSchoolYear) {
        this.model = {};
        return;
      }

      this.load();
      this.loadSchoolYearName();
    },
    selectedMonth() {
      if(!this.selectedMonth) {
        this.model = {};
        return;
      }

      this.load();
    },
  },
  mounted() {
    this.load();
    this.loadImportTypeAppSetting();
    this.loadSchoolYearName();
  },
  methods: {
    async load(){
      if (!this.selectedMonth) {
        return;
      }

      this.loading = true;
      this.classAbsenceModel = (await this.$api.absence.getAbsencesForClass(this.classId, this.selectedSchoolYear, this.selectedMonth)).data;
      this.loading = false;
    },
    loadSchoolYearName() {
      this.$api.lookups.getSchoolYears({ })
      .then((result) => {
        this.selectedSchoolYearName = result.data?.find(x => x.value == this.selectedSchoolYear)?.text;
      });
    },
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
    onActiveCampaignsLoaded(activeCampaigns) {
      this.activeCampaigns = activeCampaigns;
      if(this.selectedMonth || !activeCampaigns || activeCampaigns.length === 0) return; // Има избран месец или няма активни кампании

      this.selectedMonth = activeCampaigns.map(x => x.month).sort()[0];
    }
  }
};
</script>
