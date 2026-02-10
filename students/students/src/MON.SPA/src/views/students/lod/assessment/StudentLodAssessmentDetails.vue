<template>
  <v-card>
    <v-card-title
      v-if="userInstitutionId || isInRole(roleConsortium)"
    >
      <v-checkbox
        v-model="filterForCurrentInstitution"
        :label="$t('lod.assessments.showFromMyClassBook')"
      />
      <v-checkbox
        v-model="filterForCurrentSchoolBook"
        :label="$t('lod.assessments.showFromCurrentClassBook')"
        class="ml-3"
      />
    </v-card-title>
    <v-card-text v-if="assessments">
      <lod-assessments-table
        v-for="curriculumPart in assessments"
        :key="curriculumPart.curriculumPartId"
        :value="curriculumPart"
      />
    </v-card-text>
    <v-card-actions
      v-if="!hideBackButton"
      class="pt-0"
    >
      <v-spacer />
      <v-btn
        raised
        color="primary"
        @click.stop="backClick"
      >
        <v-icon left>
          fas fa-chevron-left
        </v-icon>
        {{ $t('buttons.back') }}
      </v-btn>
    </v-card-actions>
    <v-expansion-panels
      v-if="showDebugDetails"
      multiple
      class="my-3"
    >
      <v-expansion-panel>
        <v-expansion-panel-header>
          Оценки
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <vue-json-pretty
            path="res"
            :data="assessments"
            show-length
          />
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import LodAssessmentsTable from '@/components/lod/assessment/LodAssessmentsTable.vue';
import { mapGetters } from 'vuex';
import { Permissions, UserRole } from '@/enums/enums';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

export default {
  name: 'StudentLodAssessmentDetailsView',
  components: {
    LodAssessmentsTable,
    VueJsonPretty
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    basicClassId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    },
    isSelfEduForm: {
      type: Boolean,
      required: true
    },
    assessmentDetails: {
      type: Object,
      required: false,
      default() {
        return null;
      }
    },
    hideBackButton: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      assessments: null,
      loading: false,
      filterForCurrentInstitution: false,
      filterForCurrentSchoolBook: false,
      roleConsortium: UserRole.Consortium
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInRole', 'mode', 'userInstitutionId']),
    showDebugDetails() {
      return this.mode !== 'prod' || this.isInRole(UserRole.Consortium);
    },
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentEvaluationRead);
    },
  },
  mounted() {
    if(!this.hasReadPermission) {
      return this.$router.push('/errors/AccessDenied');
    }


    this.filterForCurrentInstitution = !!this.userInstitutionId;
    this.loadAssessments();

    this.$watch('filterForCurrentInstitution', () => this.loadAssessments());
    this.$watch('filterForCurrentSchoolBook', () => this.loadAssessments());
  },
  methods: {
    loadAssessments() {
      this.loading = true;

      this.$api.lodAssessment.getPersonAssessments(this.personId, this.basicClassId, this.schoolYear, this.isSelfEduForm, this.filterForCurrentInstitution, this.filterForCurrentSchoolBook)
      .then((response) => {
        this.assessments = response?.data;
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('common.loadError'));
      })
      .finally(() => {
        this.loading = false;
      });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
