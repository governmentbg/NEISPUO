<template>
  <div>
    <assessment-import
      @submitFil="gridReload"
    />
    <grid
      :ref="'loadAssessmentImportList' + _uid"
      url="/api/lodAssessment/listImported"
      :headers="headers"
      :title="$t('lod.assessments.listImportedTitle')"
      :filter="{
        schoolYear: gridFilters.schoolYear
      }"
      multi-sort
      :ref-key="refKey"
      :debounce="500"
      item-key="uid"
      class="mt-3"
    >
      <template #subtitle>
        <v-row
          dense
        >
          <v-col
            cols="12"
            sm="6"
            lg="2"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
              clearable
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinTypeName}` }}
      </template>

      <template v-slot:[`item.gradeName`]="{ item }">
        {{ item.isProfSubject == true ? item.decimalGrade : item.gradeName }}
      </template>

      <template v-slot:[`item.isModule`]="{ item }">
        <v-chip
          :color="item.isModule ? 'info' : 'light'"
          small
        >
          {{ item.isModule | yesNo }}
        </v-chip>
      </template>
      <template v-slot:[`item.isProfSubject`]="{ item }">
        <v-chip
          :color="item.isProfSubject ? 'success' : 'light'"
          small
        >
          {{ item.isProfSubject | yesNo }}
        </v-chip>
      </template>
    </grid>
  </div>
</template>

<script>
import AssessmentImport from '@/components/lod/assessment/AssessmentImport.vue';
import Grid from '@/components/wrappers/grid';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'LodAssessmentImportView',
  components: { AssessmentImport, Grid, SchoolYearSelector },
  data() {
    return {
      refKey: 'importedLodAssessmentsList',
      defaultGridFilters: {},
      headers: [
        {
          text: this.$t('lod.assessments.listImportedHeaders.name'),
          value: 'fullName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.pin'),
          value: 'pin',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.pin'),
          value: 'pin',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.basicClass'),
          value: 'basicClassId',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.schoolYear'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.curriculumPart'),
          value: 'curriculumPartName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.subject'),
          value: 'subjectName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.subjectType'),
          value: 'subjectTypeName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.gradeCategory'),
          value: 'gradeCategoryName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.grade'),
          value: 'gradeName',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.profSubject'),
          value: 'isProfSubject',
        },
        {
          text: this.$t('lod.assessments.listImportedHeaders.module'),
          value: 'isModule',
        },
      ]
    };
  },
  computed:{
    ...mapGetters(['hasPermission']),
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
  mounted() {
    console.log('LodAssessmentImportView mounted.');
    if(!this.hasPermission(Permissions.PermissionNameForLodAssessmentImport)) {
      console.log('LodAssessmentImportView mounted: no permission');
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['loadAssessmentImportList' + this._uid];
      if(grid) {
        grid.get();
      }
    },
  }
};
</script>
