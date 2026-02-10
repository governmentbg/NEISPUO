<template>
  <grid
    :ref="refKey"
    :headers="headers"
    url="/api/institution/GetClassGroups"
    :ref-key="refKey"
    :title="$t('institution.detailsSubtitle')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    :filter="{
      institutionId: institutionId,
      schoolYear: customFilter.schoolYear,
      basicClass: customFilter.basicClass
    }"
    group-by="parentClassId"
  >
    <template #topAppend>
      <v-row
        dense
        class="mt-1"
      >
        <v-select
          v-model="customFilter.basicClass"
          :items="basicClassOptions"
          :label="$t('institution.details.headers.basicClass')"
          clearable
          dense
        />
        <v-spacer />
        <school-year-selector
          v-model="customFilter.schoolYear"
          :label="$t('common.schoolYear')"
          dense
          :clearable="false"
          show-current-school-year-button
        />
      </v-row>
    </template>
    <template v-slot:[`item.name`]="{ item }">
      {{ item.name }}
      <v-chip
        v-if="item.isValid !== true"
        color="error"
        outlined
        small
        class="ml-2"
      >
        {{ $t('institution.details.invalidClass') }}
      </v-chip>
      <v-chip
        v-if="item.isClosed == true"
        color="error"
        outlined
        small
        class="ml-2"
      >
        {{ $t('institution.details.closedClass') }}
      </v-chip>
    </template>
    <template v-slot:[`group.header`]="{ group, items, toggle, isOpen }">
      <td :colspan="headers.length">
        <v-btn
          :ref="group"
          x-small
          icon
          @click="toggle"
        >
          <v-icon v-if="isOpen">
            mdi-chevron-up
          </v-icon>
          <v-icon v-else>
            mdi-chevron-down
          </v-icon>
        </v-btn>
        <!-- <span class="mx-5 font-weight-bold">{{ items[0].parentBasicClassName }}</span> -->
        <span class="mx-5 font-weight-bold">{{ items[0].parenClassName }}</span>
      </td>
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
          :to="`/class/${item.classId}/details?schoolYear=${customFilter.schoolYear}`"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
  import Grid from "@/components/wrappers/grid.vue";
  import SchoolYearSelector from '@/components/common/SchoolYearSelector';

export default{
  name: 'InstitutionDetailsComponent',
  components: { Grid, SchoolYearSelector },
  props:{
    institutionId: {
      type:Number,
      required: true
    },
  },
  data(){
    return{
      refKey: 'institutionClassesList',
      basicClassOptions: [],
      customFilter: {
        schoolYear: null,
        basicClass: null
      },
      headers: [
          {
              text: this.$t('institution.details.headers.name'),
              value: 'name'
          },
          {
              text: this.$t('institution.details.headers.basicClass'),
              value: 'basicClassName'
          },
          {
              text: this.$t('institution.details.headers.classType'),
              value: 'classTypeName'
          },
          {
              text: this.$t('institution.details.headers.profession'),
              value: 'profession'
          },
          {
              text: this.$t('institution.details.headers.speciality'),
              value: 'speciality'
          },
          {
              text: this.$t('institution.details.headers.eduForm'),
              value: 'eduFormName'
          },
          {
              text: this.$t('institution.details.headers.count'),
              value: 'count'
          },
          {
              text: this.$t('institution.details.headers.studentCountPlaces'),
              value: 'studentCountPlaces'
          },
          { text: '', value: 'actions', sortable: false, align: 'end' },
      ]
    };
  },
  async mounted() {
    this.customFilter = this.$refs[this.refKey].gridFilters;
    if (!this.customFilter.schoolYear) {
      const schoolYear = Number((await this.$api.institution.getCurrentYear(this.institutionId))?.data);
      this.$set(this.customFilter, 'schoolYear', schoolYear);  // this was needed on vue 2
    }
    this.loadBasicClassOptions();
  },
  methods: {
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassesLimitForInstitution(this.institutionId)
        .then((response) => {
          this.basicClassOptions = response.data;
        })
        .catch((error) => {
          console.log(error);
        });

    }
  }
};
</script>
