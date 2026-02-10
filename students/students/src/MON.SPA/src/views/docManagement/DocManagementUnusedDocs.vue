<template>
  <div>
    <grid
      :ref="'docManagementUnusedDocsGrid' + _uid"
      url="/api/docManagementExchange/listFree"
      :headers="headers"
      :title="$tc('docManagement.unusedDocs.title', 2)"
      file-export-name="Списък с незиползвани документи с фабрична номерация"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        institutionId: gridFilters.institutionId,
        regionId: gridFilters.regionId,
        municipalityId: gridFilters.municipalityId,
        townId: gridFilters.townId,
        basicDocumentId: gridFilters.basicDocumentId,
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
            cols="12"
            md="6"
          >
            <autocomplete
              v-model="gridFilters.basicDocumentId"
              :defer-options-loading="false"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('docManagement.unusedDocs.headers.basicDocument')"
              clearable
              hide-details
              :filter="{ filterByDetailedSchoolType: true }"
            />
          </v-col>
          <v-col
            v-if="!userRegionId"
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
              @change="{ gridFilters.municipalityId = null, gridFilters.townId = null }"
            />
          </v-col>
          <v-col
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
              @change="gridFilters.townId = null"
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
            lg="3"
          >
            <autocomplete
              v-model="gridFilters.townId"
              :defer-options-loading="true"
              api="/api/lookups/getCities"
              :label="$t('institution.headers.town')"
              clearable
              hide-details
              :filter="{
                municipalityId: gridFilters.municipalityId
              }"
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.institutionId`]="props">
        {{ `${props.item.institutionId} - ${props.item.institutionName}` }}
      </template>


      <!-- <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :to="`/docManagement/application/${item.item.id}/details`"
          />
        </button-group>
      </template> -->

      <!-- <template
        v-if="!userInstitutionId"
        #topAppend
      >
        <v-row no-gutters>
          <v-spacer />
          <button-tip
            v-if="hasReportCreatePermission && gridFilters.campaignId"
            icon-color="white"
            icon-name="fas fa-file-word"
            iclass="mr-3"
            tooltip="docManagement.application.generateSummaryReportFile"
            :text="$t('docManagement.application.generateSummaryReportFile')"
            bottom
            small
            @click="generateSummaryReportFile()"
          />
        </v-row>
      </template> -->
    </grid>

    <confirm-dlg ref="confirm" />
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
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: 'DocManagementUnusedDocs',
  components: {
    Grid,
    Autocomplete,
  },
  data() {
    return {
      saving: false,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      refKey: 'docManagementUnusedDocsList',
      defaultGridFilters: {

      },
      headers: [
        {
          text: this.$t('docManagement.unusedDocs.headers.basicDocument'),
          value: "basicDocumentName",
        },
        {
          text: this.$t('docManagement.unusedDocs.headers.institution'),
          value: "institutionId",
        },
        {
          text: this.$t('docManagement.unusedDocs.headers.freeDocCount'),
          value: "freeDocCount",
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
    };
  },
  computed: {
    ...mapGetters(["hasPermission", 'userInstitutionId', 'userRegionId']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead);
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
  mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['docManagementUnusedDocsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
  }
};
</script>
