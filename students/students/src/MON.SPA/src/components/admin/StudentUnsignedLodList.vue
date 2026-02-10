<template>
  <div>
    <grid
      url="/api/administration/GetUnsignedStudentLodList"
      :headers="headers"
      title=""
      :file-export-name="$t('lodFinalization.unsignedStatList.title')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      item-key="uid"
      :ref-key="refKey"
      :filter="{
        schoolYear: gridFilters.schoolYear
      }"
    >
      <template #subtitle>
        <v-row dense>
          <v-col
            cols="12"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
            />
          </v-col>
        </v-row>
      </template>
    </grid>
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';

export default {
  name: 'StudentUnsignedLodListComponent',
  components: {
    Grid,
    SchoolYearSelector
  },
  data() {
    return {
      refKey: 'unsignedStudentLodList',
      defaultGridFilters: {
        schoolYear: null
      }
    };
  },
  computed: {
    headers() {
      return [
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.name'),
          value: 'fullName',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.identifier'),
          value: 'identifier',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.basicClass'),
          value: 'basicClass',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.schoolYear'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.institutionCode'),
          value: 'institutionId',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.institutionName'),
          value: 'institutionAbbreviation',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.region'),
          value: 'regionName',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.newInstitutionCode'),
          value: 'eduStateMainInstitutionId',
        },
        {
          text: this.$t('lodFinalization.unsignedStatList.headers.newInstitutionName'),
          value: 'eduStateMainInstitutionAbbreviation',
        },
        { text: '', value: 'controls', sortable: false, align: 'end' }
      ];
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
    }
  },
  beforeMount() {
    if(!this.gridFilters.schoolYear) {
      const currentSchoolYear = 2022;
      // const currentSchoolYear = await this.getCurrentSchoolYear();
      if(!isNaN(currentSchoolYear)) {
        this.gridFilters.schoolYear = currentSchoolYear;
      }
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
