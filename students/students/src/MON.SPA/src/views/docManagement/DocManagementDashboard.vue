<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'docManagementApplicationsDashboard' + _uid"
      url="/api/docManagementApplication/dashboard"
      :headers="headers"
      :title="$tc('docManagement.application.title', 2)"
      file-export-name="Списък със заявления за документи с фабрична номерация"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        institutionId: gridFilters.institutionId,
        schoolYear: gridFilters.schoolYear,
        campaignId: gridFilters.campaignId,
        regionId: gridFilters.regionId,
        municipalityId: gridFilters.municipalityId,
        instType: gridFilters.instType,
        campaignType: gridFilters.campaignType,
        applicationStatusFilter: gridFilters.applicationStatusFilter,
        campaignStatusFilter: gridFilters.campaignStatusFilter,
        status: gridFilters.status
      }"
    >
      <template #subtitle>
        <v-row dense>
        <v-radio-group
          v-model="gridFilters.applicationStatusFilter"
          row
        >
          <v-radio
            :label="$t('docManagement.application.filter.unsubmitted')"
            :value="0"
          />
          <v-radio
            :label="$t('docManagement.application.filter.submitted')"
            :value="1"
          />
          <v-radio
            :label="$t('docManagement.application.filter.all')"
            :value="2"
          />
        </v-radio-group>
        <v-spacer />
        <v-radio-group
          v-model="gridFilters.campaignStatusFilter"
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
        <v-row
          dense
          class="mb-1"
        >
          <v-col
            cols="12"
            md="6"
            lg="3"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
              hide-details
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.campaignId"
              :items="campaignsOptions"
              :label="$t('docManagement.application.headers.campaign')"
              clearable
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
          >
            <autocomplete
              v-model="gridFilters.institutionId"
              :defer-options-loading="false"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              clearable
              :filter="{
                regionId: userRegionId
              }"
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId && !userRegionId"
            cols="12"
            md="6"
            lg="3"
          >
            <autocomplete
              v-model="gridFilters.regionId"
              :defer-options-loading="false"
              api="/api/lookups/getDistricts"
              :label="$t('institution.headers.region')"
              clearable
              hide-details
              @change="gridFilters.municipalityId = null"
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <autocomplete
              v-model="gridFilters.municipalityId"
              :defer-options-loading="false"
              api="/api/lookups/getMunicipalities"
              :label="$t('institution.headers.municipality')"
              clearable
              hide-details
              :filter="{
                regionId: userRegionId || gridFilters.regionId
              }"
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.campaignType"
              :items="options3"
              :label="$t('docManagement.application.filter.campaignType')"
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.instType"
              :items="options1"
              :label="$t('docManagement.application.filter.instType')"
              hide-details
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.status"
              :items="statusOptions"
              :label="$t('docManagement.application.headers.status')"
              clearable
              hide-details
            />
          </v-col>
        </v-row>
      </template>

      <template
        v-slot:[`item.docManagementApplicationStatus`]="{ item }"
      >
        <v-chip
          v-if="item.docManagementApplicationStatus"
          :color="['Approved', 'Submitted'].includes(item.docManagementApplicationStatus) ? 'success' : ['Rejected', 'ReturnedForCorrection'].includes(item.docManagementApplicationStatus) ? 'warning' : 'light'"
          small
          label
        >
          {{ item.statusName }}
        </v-chip>
      </template>

      <template
        v-slot:[`item.docManagementApplicationHasAttachment`]="{ item }"
      >
        <v-chip
          v-if="item.docManagementApplicationId"
          :color="item.docManagementApplicationHasAttachment ? 'success' : 'light'"
          small
          label
        >
          {{ item.docManagementApplicationHasAttachment | yesNo }}
        </v-chip>
      </template>

      <template
        v-slot:[`item.institutionId`]="{ item }"
      >
        {{ `${item.institutionId} - ${item.institutionName}` }}
      </template>

      <template
        v-slot:[`item.requestedInstitutionId`]="{ item }"
      >
        <span v-if="item.requestedInstitutionId">
          {{ `${item.requestedInstitutionId} - ${item.requestedInstitutionName}` }}
        </span>
        <span v-else />
      </template>


      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasReadPermission && item.item.docManagementApplicationId"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :to="`/docManagement/application/${item.item.docManagementApplicationId}/details`"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.docManagementApplicationId && item.item.isEditable && !item.item.isExchangeRequest"
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="docManagement.application.generateReportFile"
            bottom
            small
            @click="generateApplicationReportFile(item.item)"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.isExchangeRequest"
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="docManagement.application.generateProtocolFile"
            bottom
            small
            @click="generateProtocolFile(item.item)"
          />
        </button-group>
      </template>
    </grid>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";
import cSelect from "@/components/wrappers/CustomSelectList.vue";
import Helper from "@/components/helper.js";

export default {
  name: 'DocManagementDashboard',
  components: {
    Grid,
    SchoolYearSelector,
    Autocomplete,
    cSelect,
  },
  data() {
    return {
      saving: false,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      refKey: 'docManagementApplicationsDashboard',
      defaultGridFilters: {
        instType: 1,
        campaignType: 1,
        status: null,
        applicationStatusFilter: 0, // 0 - Неподадени отъствия, 1 - Подадени отсъствия, 2 - Всички
        campaignStatusFilter: 1, // 0 - Неактивна кампания, 1 - Активна кампания, , 2 - Всички
        schoolYear: null,
      },
      campaignsOptions: null,
      headers: [
        // {
        //   text: this.$t('docManagement.application.headers.id'),
        //   value: "docManagementApplicationId",
        // },
        {
          text: this.$t('docManagement.application.headers.campaign'),
          value: "docManagementCampaignName",
        },
        {
          text: this.$t('docManagement.application.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('docManagement.application.headers.institution'),
          value: "institutionId",
        },
        {
          text: this.$t('docManagement.application.headers.institutionTown'),
          value: "institutionTown",
        },
        {
            text: this.$t('docManagement.application.headers.institutionMunicipality'),
            value: "institutionMunicipality",
        },
        {
            text: this.$t('docManagement.application.headers.institutionRegion'),
            value: "institutionRegion",
        },
        // {
        //   text: this.$t('docManagement.application.headers.requestedInstitution'),
        //   value: "requestedInstitutionId",
        // },
        {
          text: this.$t('docManagement.application.headers.status'),
          value: "docManagementApplicationStatus",
        },
        {
          text: this.$t('docManagement.application.headers.hasAttachments'),
          value: "docManagementApplicationHasAttachment",
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
      options1: [
        { text: 'Всички', value: 1 },
        { text: 'С делегиран бюджет', value: 2 },
        { text: 'Без делегиран бюджет', value: 3 },
      ],
      options3: [
        { text: 'Всички', value: 1 },
        { text: 'Основни кампании', value: 2 },
        { text: 'Допълнителни кампании', value: 3 },
      ],
      statusOptions: [
        { text: 'Подаден', value: 'Submitted' },
        { text: 'Върнат за корекция', value: 'ReturnedForCorrection' },
        { text: 'Одобрен', value: 'Approved' },
        { text: 'Отхвърлен', value: 'Rejected' },
      ],
    };
  },
  computed: {
    ...mapGetters(["hasPermission", 'userInstitutionId', 'userRegionId']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead);
    },
    hasReportCreatePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementReportCreate);
    },
    gridFilters: {
      get () {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {};
        }
        else {
          return this.defaultGridFilters;
        }
      },
      set (value) {
        if (this.refKey in this.$store.state.gridFilters) {
          this.$store.commit('updateGridFilter', { options: value, refKey: this.refKey });
        }
        else {
          return this.defaultGridFilters = value;
        }
      }
    },
  },
  async mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.getCampaignsOptions();
    if (!this.gridFilters.schoolYear) {
      this.gridFilters.schoolYear = (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data;
    }
  },
  methods: {
    getCampaignsOptions() {
      this.$api.docManagementCampaign.getDropdownOptions()
      .then(response => {
        this.campaignsOptions = response?.data;
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    async generateApplicationReportFile(item) {
      this.saving = true;
      await this.$api.docManagementApplication.generateApplicationReport({ applicationId: item.docManagementApplicationId })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Заявка за ЗУД_${item.institutionId}.docx`;

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => { this.saving = false; });
    },
    async generateProtocolFile(item) {
      this.saving = true;
      await this.$api.docManagementExchange.generateProtocol({ applicationId: item.docManagementApplicationId })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Заявка за ЗУД_${item.institutionId}.docx`;

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => { this.saving = false; });
    },
  }
};
</script>
