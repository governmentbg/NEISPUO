<template>
  <v-card>
    <v-card-title>
      {{ $tc('institution.title',2) }}
    </v-card-title>
    <v-card-subtitle>{{ $t('institution.detailsSubtitle') }}</v-card-subtitle>
    <v-card-text>
      <grid
        :ref="refKey"
        url="/api/institution/list"
        :headers="headers"
        title=" "
        :ref-key="refKey"
        file-export-name="Институции"
        :file-exporter-extensions="['xlsx', 'csv', 'txt']"
        :filter="{
          regionId: customFilter.regionId,
          instTypeId: customFilter.instTypeId
        }"
      >
        <template #subtitle>
          <v-row
            v-if="!isInstitution"
            dense
          >
            <v-col
              v-if="!isRuo"
            >
              <autocomplete
                v-model="customFilter.regionId"
                api="/api/lookups/GetDistricts"
                :label="$t('institution.headers.region')"
                :defer-options-loading="false"
                clearable
              />
            </v-col>
            <v-col>
              <autocomplete
                v-model="customFilter.instTypeId"
                api="/api/lookups/GetInstitutionTypeOptions"
                :label="$t('institution.headers.instType')"
                :defer-options-loading="false"
                clearable
              />
            </v-col>
          </v-row>
        </template>
        <template #actions="item">
          <button-group>
            <button-tip
              icon
              icon-name="mdi-eye"
              icon-color="primary"
              tooltip="buttons.details"
              bottom
              iclass=""
              small
              :to="`/institution/${item.item.id}/details`"
            />
            <button-tip
              icon
              icon-name="fa-network-wired"
              icon-color="primary"
              tooltip="institution.externalSubtitle"
              bottom
              iclass=""
              small
              :to="`/institution/${item.item.id}/external`"
            />
            <button-tip
              icon
              icon-name="fa-users"
              icon-color="primary"
              tooltip="institution.students"
              bottom
              iclass=""
              small
              :to="`/institution/${item.item.id}/students`"
            />
          </button-group>
        </template>
      </grid>
    </v-card-text>
  </v-card>
</template>

<script>
  import Grid from "@/components/wrappers/grid.vue";
  import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";
  import { mapGetters } from 'vuex';

  export default{
    name: 'InstitutionList',
    components: { Grid, Autocomplete },
    data(){
      return{
        refKey: 'institutionsList',
        customFilter: {
          regionId:  null,
          instTypeId: null
        },
        headers: [
          {
              text: this.$t('institution.headers.id'),
              value: 'id'
          },
          {
              text: this.$t('institution.headers.name'),
              value: 'abbreviation'
          },
          {
              text: this.$t('institution.headers.region'),
              value: 'region'
          },
          {
              text: this.$t('institution.headers.municipality'),
              value: 'municipality'
          },
          {
              text: this.$t('institution.headers.town'),
              value: 'town'
          },
          {
              text: this.$t('institution.headers.instType'),
              value: 'instTypeAbbreviation'
          },
          {
              text: this.$t('institution.headers.baseSchoolType'),
              value: 'baseSchoolTypeName'
          },
          {
              text: this.$t('institution.headers.detailedSchoolType'),
              value: 'detailedSchoolTypeName'
          },
          {
              text: this.$t('institution.headers.financialSchoolType'),
              value: 'financialSchoolTypeName'
          },
           {
              text: this.$t('institution.headers.budgetingSchoolType'),
              value: 'budgetingSchoolTypeName'
          },
          { text: '', value: 'controls', sortable: false, filterable: false, align: 'end' },
        ]
      };
    },
    computed: {
      ...mapGetters(['userInstitutionId', 'userRegionId']),
      isInstitution() {
        return !!this.userInstitutionId;
      },
      isRuo() {
        return !!this.userRegionId && !this.isInstitution;
      }
    },
    mounted() {
      this.customFilter = this.$refs[this.refKey].gridFilters;
    },
    methods: { }
  };
</script>
