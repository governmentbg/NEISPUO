<template>
  <grid
    :ref="'аbsenceReportsGrid' + _uid"
    url="/api/absenceCampaign/AbsenceReportsList"
    file-export-name="Списък с отчети(импорти) с отсъствия"
    :headers="headers"
    :title="$t('absenceReport.title')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    :item-class="itemRowBackground"
    :filter="{
      reportTypeFilter: reportTypeFilter,
      campaignStatusFilter: campaignStatusFilter,
      schoolYear: absenceCampaign ? absenceCampaign.schoolYear : null ,
      month: absenceCampaign ? absenceCampaign.month : null }"
    multi-sort
  >
    <template #subtitle>
      <v-row dense>
        <v-radio-group
          v-model="reportTypeFilter"
          row
        >
          <v-radio
            :label="$t('absenceReport.filter.unsubmitted')"
            :value="0"
          />
          <v-radio
            :label="$t('absenceReport.filter.submitted')"
            :value="1"
          />
          <v-radio
            :label="$t('absenceReport.filter.unsigned')"
            :value="2"
          />
          <v-radio
            :label="$t('absenceReport.filter.signed')"
            :value="3"
          />
          <!-- <v-radio
            :label="$t('absenceReport.filter.unsubmittedOrUnsigned')"
            :value="4"
          /> -->
          <v-radio
            :label="$t('absenceReport.filter.all')"
            :value="5"
          />
        </v-radio-group>
        <v-spacer />
        <v-radio-group
          v-model="campaignStatusFilter"
          row
        >
          <v-radio
            :label="$t('absenceReport.filter.inactiveCampaign')"
            :value="0"
          />
          <v-radio
            :label="$t('absenceReport.filter.activeCampaign')"
            :value="1"
          />
          <v-radio
            :label="$t('absenceReport.filter.all')"
            :value="2"
          />
        </v-radio-group>
      </v-row>
      <v-row>
        <v-col cols="5">
          <autocomplete
            id="absence"
            v-model="absenceCampaign"
            :defer-options-loading="false"
            api="/api/lookups/GetAbsenceCampaigns"
            return-object
            :label="$t('absence.filterByCampaign')"
            :placeholder="$t('buttons.search')"
            hide-no-data
            hide-selected
            clearable
          />
        </v-col>
      </v-row>
    </template>

    <template v-slot:[`item.month`]="props">
      {{ $helper.getMonthName(props.item.month) }}
    </template>

    <template v-slot:[`item.absenceCampaignFromDate`]="props">
      {{ props.item.absenceCampaignFromDate ? $moment(props.item.absenceCampaignFromDate).format(dateFormat) : "" }}
    </template>

    <template v-slot:[`item.absenceCampaignToDate`]="props">
      {{ props.item.absenceCampaignToDate ? $moment(props.item.absenceCampaignToDate).format(dateFormat) : "" }}
    </template>

    <template v-slot:[`item.аbsenceImportSignedDate`]="props">
      {{ props.item.аbsenceImportSignedDate ? $moment(props.item.аbsenceImportSignedDate).format(dateAndTimeFormat) : "" }}
    </template>

    <template v-slot:[`item.absenceCampaignIsActive`]="props">
      <v-chip
        v-if="props.item.absenceCampaignIsActive"
        color="success"
        small
      >
        {{ $t('common.active_a') }}
      </v-chip>
    </template>

    <template v-slot:[`item.absenceImportSubmitted`]="props">
      <v-chip
        :color="props.item.absenceImportSubmitted ? 'success' : 'error'"
        small
      >
        <yes-no :value="props.item.absenceImportSubmitted" />
      </v-chip>
    </template>

    <template v-slot:[`item.absenceImportIsSigned`]="props">
      <v-chip
        :color="props.item.absenceImportIsSigned ? 'success' : 'error'"
        small
      >
        <yes-no :value="props.item.absenceImportIsSigned" />
      </v-chip>
    </template>

    <template v-slot:[`item.controls`]="props">
      <button-group>
        <button-tip
          v-if="hasReadPermission && props.item.absenceImportSubmitted && props.item.absenceImportId"
          icon
          icon-color="primary"
          tooltip="buttons.review"
          bottom
          iclass=""
          small
          :to="`/absence/import/${props.item.absenceImportId}/details`"
        />
        <doc-downloader
          v-if="props.item.blobId"
          :value="props.item"
          show-icon
          x-small
          :show-file-name="false"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>

import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import DocDownloader from '@/components/common/DocDownloader.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: 'AbsenceReportsList',
  components: {
    Grid,
    DocDownloader,
    Autocomplete
  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      absenceCampaign: null,
      reportTypeFilter: 0, // 0 - Неподадени отъствия, 1 - Подадени отсъствия, 2 - Неподписани отсъствия, 3 - Подписани отсъствия, 4 - Неподадени или неподписани, 5 - Всички
      campaignStatusFilter: 1, // 0 - Неактивна кампания, 1 - Активна кампания, , 2 - Всички
      headers: [
        {
          text: this.$t('absenceReport.headers.institutionCode'),
          value: "institutionId",
        },
        {
          text: this.$t('absenceReport.headers.institutionName'),
          value: "institutionAbbreviation",
        },
        {
          text: this.$t('absenceReport.headers.institutionTown'),
          value: "institutionTown",
        },
        {
          text: this.$t('absenceReport.headers.institutionRegion'),
          value: "institutionRegion",
        },
        {
          text: this.$t('absenceReport.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('absenceReport.headers.month'),
          value: "month",
        },
        {
          text: this.$t('absenceReport.headers.absenceImportSubmitted'),
          value: "absenceImportSubmitted",
        },
        {
          text: this.$t('absenceReport.headers.absenceImportIsSigned'),
          value: "absenceImportIsSigned",
        },
        {
          text: this.$t('absenceReport.headers.аbsenceImportSignedDate'),
          value: "аbsenceImportSignedDate",
        },
        {
          text: this.$t('absenceReport.headers.absenceCampaignFromDate'),
          value: "absenceCampaignFromDate",
        },
        {
          text: this.$t('absenceReport.headers.absenceCampaignToDate'),
          value: "absenceCampaignToDate",
        },
        {
          text: this.$t('absenceReport.headers.absenceCampaignStatus'),
          value: "absenceCampaignIsActive",
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userInstitutionId']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceRead);
    }
  },
  mounted() {
    if(this.userInstitutionId) {
      // Има userInstitutionId т.е. е в роля свързана с училище.
      this.reportTypeFilter = 5; // Всички импорти
      this.campaignStatusFilter = 2; // Всички кампании
    }

    this.$studentHub.$on('absence-campaign-modified', this.onAbsenceCampaignModified);
  },
  destroyed() {
    this.$studentHub.$off('absence-campaign-modified');
  },
  methods: {
    gridReload() {
      const grid = this.$refs['аbsenceReportsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    onAbsenceCampaignModified(id) {
      console.log(id);
      this.gridReload();
    },
    itemRowBackground(item) {
      return item.isActive ? 'custom-grid-row left border-success' : '';
    },
  }
};
</script>
