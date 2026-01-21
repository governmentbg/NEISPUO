<template>
  <div>
    <grid
      v-if="mounted"
      url="/api/student/employerDetailsList"
      :headers="headers"
      title=" "
      :filter="{
        institutionId: gridFilters.institutionId,
        schoolYear: gridFilters.schoolYear,
      }"
      :ref-key="refKey"
      item-key="uid"
      multi-sort
      use-post-verb
      :file-export-name="$t('dualFormEdu.dashboardTitle')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    >
      <template #subtitle>
        <v-row dense>
          <v-col
            cols="12"
            sm="6"
            lg="2"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
          >
            <autocomplete
              v-model="gridFilters.institutionId"
              :defer-options-loading="false"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              clearable
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinTypeName}` }}
      </template>

      <template v-slot:[`item.startDate`]="{ item }">
        {{ item.startDate ? $moment(item.startDate).format(dateFormat) : '' }}
      </template>

      <template v-slot:[`item.endDate`]="{ item }">
        {{ item.endDate ? $moment(item.endDate).format(dateFormat) : '' }}
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.details"
            bottom
            iclass=""
            small
            :to="`/student/${item.item.personId}/generalTrainingData/${item.item.institutionId}/details?classId=${item.item.id}`"
          />
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="fas fa-graduation-cap"
            icon-color="primary"
            tooltip="buttons.chronology"
            bottom
            iclass=""
            small
            :to="`/student/${item.item.personId}/classes`"
          />
        </button-group>
      </template>
    </grid>
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import Constants from '@/common/constants.js';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'DualFormListComponent',
  components: { Grid, SchoolYearSelector, Autocomplete },
  props: {

  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      defaultGridFilters: {},
      refKey: 'dualFormList',
      mounted: false
    };
  },
  computed: {
    ...mapGetters(['turnOnOresModule', 'hasPermission', 'isInRole', 'mode', 'userRegionId', 'userInstitutionId']),
    hasReadPermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresRead);
    },
    hasManagePermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresManage);
    },
    headers() {
      const headers = [];

      if (!this.userInstitutionId) {
        // За роли, които не са институции или РУО нека колоните да бъдат:
        // "Област", "Община", "Населено място", "Код", "Институция"

        //За РУО:
        //"Община", "Населено място", "Код", "Институция"
          if (!this.userRegionId) {
            headers.push({
              text: this.$t("dualFormEdu.headers.region"),
              value: "institutionRegion",
            });
          }

          headers.push({
            text: this.$t("dualFormEdu.headers.municipality"),
            value: "institutionMunicipality",
            filterable: true,
            type: 'text'
          });

          headers.push({
            text: this.$t("dualFormEdu.headers.town"),
            value: "institutionTown",
            filterable: true,
            type: 'text'
          });

          headers.push({
            text: this.$t("dualFormEdu.headers.institutionCode"),
            value: "institutionCode",
            filterable: true,
            type: 'text'
          });

          headers.push({
            text: this.$t("dualFormEdu.headers.institution"),
            value: "institutionName",
            filterable: true,
            type: 'text'
          });
      }

      return [...headers, ...[
        {
          text: this.$t("dualFormEdu.headers.name"),
          value: "fullName",
          filterable: true,
          type: 'text',
        },
        {
          text: this.$t("dualFormEdu.headers.pin"),
          value: "pin",
          filterable: true,
          type: 'text',
        },
        {
          text: this.$t("dualFormEdu.headers.basicClass"),
          value: "basicClassName",
          filterable: true,
          type: 'text',
        },
        {
          text: this.$t("dualFormEdu.headers.class"),
          value: "className",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("dualFormEdu.headers.profession"),
          value: "profession",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("dualFormEdu.headers.schoolYear"),
          value: "schoolYearName",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("dualFormEdu.headers.startDate"),
          value: "startDate",
          type: 'date',
          dateFormat: this.dateFormat
        },
        {
          text: this.$t("dualFormEdu.headers.endDate"),
          value: "endDate",
          type: 'date',
          dateFormat: this.dateFormat
        },
        {
          text: this.$t("dualFormEdu.headers.companyName"),
          value: "companyName",
          filterable: true,
          type: 'text',
        },
        {
          text: this.$t("dualFormEdu.headers.companyUic"),
          value: "companyUic",
          filterable: true,
          type: 'text',
        },
        {
          text: "",
          value: "controls",
          groupable: false,
          filterable: false,
          sortable: false,
          align: "end",
        },
      ]];
    },
    gridFilters: {
      get () {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {

          };
        }
        else {
          return this.defaultGridFilters;
        }
      },
      set (value) {
        console.log('set');
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
    try {
      if(!this.gridFilters.schoolYear) {
        const currentSchoolYear = await this.getCurrentSchoolYear();
        if(!isNaN(currentSchoolYear)) {
          this.gridFilters.schoolYear = currentSchoolYear;
        }
      }
    } finally {
      this.mounted = true;
    }
  },
  methods: {
    async getCurrentSchoolYear() {
      const currentSchoolYear = Number((await this.$api.institution.getCurrentYear())?.data);
      return currentSchoolYear;
    }
  }
};
</script>

