<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'docManagementReportListGrid' + _uid"
      url="/api/docManagementApplication/ReportList"
      :headers="headers"
      :title="$tc('docManagement.report.title', 2)"
      file-export-name="Отчет на ЗУД"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        institutionId: gridFilters.institutionId,
        schoolYear: gridFilters.schoolYear,
        regionId: gridFilters.regionId,
        municipalityId: gridFilters.municipalityId,
        townId: gridFilters.townId,
        basicDocumentId: gridFilters.basicDocumentId,
        isDiplomaSigned: customFilter.isDiplomaSigned,
        hasDiplomaDocument: customFilter.hasDiplomaDocument,
        seriesFilter: customFilter.seriesFilter
          ? customFilter.seriesFilter.filter
          : null,
        seriesFilterOp: customFilter.seriesFilter
          ? customFilter.seriesFilter.op
          : null,
        factoryNumberFilter: customFilter.factoryNumberFilter
          ? customFilter.factoryNumberFilter.filter
          : null,
        factoryNumberFilterOp: customFilter.factoryNumberFilter
          ? customFilter.factoryNumberFilter.op
          : null,
        basicDocumentFilter: customFilter.basicDocumentFilter
          ? customFilter.basicDocumentFilter.filter
          : null,
        basicDocumentFilterOp: customFilter.basicDocumentFilter
          ? customFilter.basicDocumentFilter.op
          : null,
        schoolYearFilter: customFilter.schoolYearFilter
          ? customFilter.schoolYearFilter.filter
          : null,
        schoolYearFilterOp: customFilter.schoolYearFilter
          ? customFilter.schoolYearFilter.op
          : null,
        personFullNameFilter: customFilter.personFullNameFilter
          ? customFilter.personFullNameFilter.filter
          : null,
        personFullNameFilterOp: customFilter.personFullNameFilter
          ? customFilter.personFullNameFilter.op
          : null,
        personIdentifierFilter: customFilter.personIdentifierFilter
          ? customFilter.personIdentifierFilter.filter
          : null,
        personIdentifierFilterOp: customFilter.personIdentifierFilter
          ? customFilter.personIdentifierFilter.op
          : null,
      }"
    >
      <template #subtitle>
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
            md="9"
          >
            <autocomplete
              v-model="gridFilters.basicDocumentId"
              :defer-options-loading="false"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('docManagement.report.headers.basicDocument')"
              clearable
              hide-details
              :filter="{ filterByDetailedSchoolType: !!userInstitutionId }"
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
        </v-row>
      </template>

      <template v-slot:[`item.institutionId`]="{ item }">
        {{ `${item.institutionId} - ${item.institutionName}` }}
      </template>

      <template v-slot:[`item.series`]="{ item }">
        {{ `${item.series ? item.series : ''}${item.edition ? `/${item.edition}` : ''}` }}
      </template>

      <template v-slot:[`item.diplomaRegistrationNumberYear`]="{ item }">
        <span v-if="item.diplomaRegistrationDate">{{ `${item.diplomaRegistrationNumberTotal} - ${item.diplomaRegistrationNumberYear} / ${$moment(item.diplomaRegistrationDate).format(dateFormat)}` }}</span>
        <span v-else />
      </template>

      <template v-slot:[`item.institutionTownName`]="{ item }">
        {{ `${item.institutionTownName}, ${item.institutionMunicipalityName}, ${item.institutionRegionName}` }}
      </template>

      <template v-slot:[`item.personIdentifier`]="{ item }">
        <span v-if="item.personIdentifier">
          {{ `${item.personIdentifier} - ${item.personIdentifierType}` }}
        </span>
      </template>

      <template v-slot:[`item.isDiplomaSigned`]="{ item }">
        <v-chip
          v-if="item.personIdentifier"
          :color="item.isDiplomaSigned === true ? 'success' : 'error'"
          outlined
          small
        >
          <yes-no :value="item.isDiplomaSigned" />
        </v-chip>
      </template>

      <template v-slot:[`item.hasDiplomaDocument`]="{ item }">
        <v-chip
          v-if="item.personIdentifier"
          :color="item.hasDiplomaDocument === true ? 'success' : 'error'"
          outlined
          small
        >
          <yes-no :value="item.hasDiplomaDocument" />
        </v-chip>
      </template>

      <template v-slot:[`header.isDiplomaSigned`]="{ header }">
        {{ header.text }}
        <bool-header-filter v-model="customFilter.isDiplomaSigned" />
      </template>

      <template v-slot:[`header.hasDiplomaDocument`]="{ header }">
        {{ header.text }}
        <bool-header-filter v-model="customFilter.hasDiplomaDocument" />
      </template>

      <template v-slot:[`header.series`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.seriesFilter" />
      </template>

      <template v-slot:[`header.docNumber`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.factoryNumberFilter" />
      </template>

      <template v-slot:[`header.schoolYearName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.schoolYearFilter" />
      </template>

      <template v-slot:[`header.basicDocumentName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.basicDocumentFilter" />
      </template>

      <template v-slot:[`header.personFullName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.basicDocumentFilter" />
      </template>

      <template v-slot:[`header.personIdentifier`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.personIdentifierFilter" />
      </template>

      <template
        v-if="userInstitutionId"
        #topAppend
      >
        <v-row no-gutters>
          <v-spacer />
          <button-tip
            v-if="hasManagePermission && gridFilters.schoolYear"
            icon-color="white"
            icon-name="fas fa-file-word"
            iclass="mr-3"
            tooltip="docManagement.report.generateReportFile"
            :text="$t('docManagement.report.generateReportFile')"
            bottom
            small
            icon-size="16"
            @click="generateReportFile()"
          />
          <button-tip
            v-if="hasManagePermission && gridFilters.schoolYear"
            color="error"
            icon-color="white"
            icon-name="fas fa-file-word"
            iclass="mr-3"
            tooltip="docManagement.report.generateDestructionProtocol"
            :text="$t('docManagement.report.generateDestructionProtocol')"
            bottom
            small
            bclass="ml-2"
            icon-size="16"
            @click="generateDestructionProtocol()"
          />
        </v-row>
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
// import cSelect from "@/components/wrappers/CustomSelectList.vue";
import Helper from "@/components/helper.js";
import BoolHeaderFilter from "@/components/wrappers/grid/BoolHeaderFilter.vue";
import TextHeaderFilter from "@/components/wrappers/grid/TextHeaderFilter.vue";


export default {
  name: 'DocManagementApplications',
  components: {
    Grid,
    SchoolYearSelector,
    Autocomplete,
    BoolHeaderFilter,
    TextHeaderFilter
    // cSelect,
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      refKey: 'docManagementReportList',
      customFilter: {
        isDiplomaSigned: null,
        hasDiplomaDocument: null,
        seriesFilter: null,
        factoryNumberFilter: null,
        schoolYearFilter: null,
        basicDocumentFilter: null,
        personFullNameFilter: null,
        personIdentifierFilter: null,
      }
    };
  },
  computed: {
    ...mapGetters(["hasPermission", 'userInstitutionId', 'userRegionId']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage);
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
    headers() {
      const headers = [
        {
          text: this.$t('docManagement.report.headers.institution'),
          value: "institutionId",
          hidden: false,
        },
        {
          text: this.$t('docManagement.report.headers.institutionTown'),
          value: "institutionTownName",
          visible: !this.userInstitutionId
        },
        {
          text: this.$t('docManagement.report.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('docManagement.report.headers.basicDocument'),
          value: "basicDocumentName",
        },
        {
          text: this.$t('docManagement.report.headers.series'),
          value: "series",
        },
        {
          text: this.$t('docManagement.report.headers.docNumber'),
          value: "docNumber",
        },
        {
          text: this.$t('docManagement.report.headers.regNumber'),
          value: "diplomaRegistrationNumberYear",
        },
        {
          text: this.$t('docManagement.report.headers.personFullName'),
          value: "personFullName",
        },
        {
          text: this.$t('docManagement.report.headers.personIdentifier'),
          value: "personIdentifier",
        },
        {
          text: this.$t('docManagement.report.headers.isDiplomaSigned'),
          value: "isDiplomaSigned",
        },
        {
          text: this.$t('docManagement.report.headers.hasDiplomaDocument'),
          value: "hasDiplomaDocument",
        },
      ];

      return headers.filter(h => h.visible !== false);
    }
  },
  async created() {
    if(!this.gridFilters.schoolYear) {
      const currentYear = Number(
        (await this.$api.institution.getCurrentYear(this.institutionId))?.data
      );
      if (!isNaN(currentYear)) {
        this.gridFilters.schoolYear = currentYear;
        this.refresh();
      } else {
        this.$helper.getYear();
      }
    }
  },
  mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['docManagementReportListGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async generateReportFile() {
      // Приложение № 6 към чл. 52, ал. 1
      this.saving = true;

      await this.$api.docManagementApplication.generateReport({ schoolYear: this.gridFilters.schoolYear })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Отчет на ЗУД_${this.gridFilters.schoolYear}.docx`;

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    async generateDestructionProtocol() {
      // Протоколът за унищожаване е формуляр
      this.saving = true;

      await this.$api.docManagementApplication.generateDestructionProtocol({ schoolYear: this.gridFilters.schoolYear })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Протоколът за унищожаване на ЗУД_${this.gridFilters.schoolYear}.docx`;

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => {
          this.saving = false;
        });
    },
  }
};
</script>
