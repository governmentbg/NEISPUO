<template>
  <grid
    v-if="mounted"
    url="/api/admin/GetStudentExernalEvaluationsList"
    :headers="headers"
    title=""
    :filter="{
      institutionId: gridFilters.institutionId,
      schoolYear: gridFilters.schoolYear,
    }"
    item-key="uid"
    multi-sort
    :ref-key="refKey"
    :file-export-name="$t('externalEval.dashboardTitle')"
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
      {{ `${item.pin} - ${item.pinType}` }}
    </template>

    <template v-slot:[`item.originalPoints`]="{ item }">
      {{ item.originalPoints.toFixed(2) }}
    </template>

    <template v-slot:[`item.grade`]="{ item }">
      {{ item.grade.toFixed(2) }}
    </template>


    <template v-slot:[`item.points`]="{ item }">
      {{ item.points.toFixed(2) }}
    </template>

    <template v-slot:[`item.hasParentConsent`]="{ item }">
      <v-chip
        color="light"
        small
      >
        {{ item.hasParentConsent | yesNo }}
      </v-chip>
    </template>
    <template #actions="item">
      <button-group>
        <button-tip
          :to="`/student/${item.item.personId}/details`"
          icon
          icon-color="primary"
          iclass=""
          icon-name="mdi-eye"
          small
          tooltip="student.menu.details"
          bottom
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentEnvironmentCharacteristicListComponent',
  components: {
    Grid,
    SchoolYearSelector,
    Autocomplete
  },
  data() {
    return {
      defaultGridFilters: {},
      refKey: 'externalEvaluationList',
      mounted: false
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId', 'userRegionId']),
    headers() {
      const headers = [];

      if (!this.userInstitutionId) {
        // За роли, които не са институции или РУО нека колоните да бъдат:
        // "Област", "Код", "Институция"

        //За РУО:
        //"Код", "Институция"
          if (!this.userRegionId) {
            headers.push({
              text: this.$t("dualFormEdu.headers.region"),
              value: "regionName",
            });
          }

          headers.push({
            text: this.$t("dualFormEdu.headers.institutionCode"),
            value: "institutionId",
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
          text: this.$t("dualFormEdu.headers.schoolYear"),
          value: "schoolYearName",
          filterable: true,
          type: 'text'
        },
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
          text: this.$t("dualFormEdu.headers.class"),
          value: "className",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("externalEval.headers.type"),
          value: "type",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("externalEval.subject"),
          value: "subjectName",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("externalEval.headers.subjectType"),
          value: "subjectTypeName",
          filterable: true,
          type: 'text'
        },
        {
          text: this.$t("externalEval.points"),
          value: "points",
          filterable: true,
          type: 'number'
        },
        {
          text: this.$t("externalEval.originalPoints"),
          value: "originalPoints",
          filterable: true,
          type: 'number'
        },
        {
          text: this.$t("externalEval.grade"),
          value: "grade",
          filterable: true,
          type: 'number'
        },
        {
          text: this.$t("externalEval.flLevel"),
          value: "flLevel",
          filterable: true,
          type: 'text'
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
