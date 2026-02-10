<template>
  <v-card>
    <v-card-title>
      {{ this.$t("dashboards.mon.title") }}
    </v-card-title>
    <v-card-subtitle>
      {{ this.$t("dashboards.mon.subtitle") }}
    </v-card-subtitle>
    <v-card-text>
      <v-row
        dense
        class="mt-5"
      >
        <v-col
          cols="12"
          sm="12"
          md="4"
          xl="3"
        >
          <student-stats />
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="4"
          xl="3"
        >
          <class-group-stats />
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="4"
          xl="3"
        >
          <diploma-stats />
        </v-col>
      </v-row>

      <ActiveCampaigns show-extended-details />

      <v-row
        dense
        class="mt-2"
      >
        <v-expansion-panels
          v-model="expandablePanelModel"
          multiple
          popout
        >
          <v-expansion-panel v-if="hasSopEnrollmentDetailsReadPermission">
            <v-expansion-panel-header>
              <div>{{ this.$t('sop.enrollmentDetails.title') }}</div>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <sop-enrollment-details />
            </v-expansion-panel-content>
          </v-expansion-panel>

          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ this.$t('dashboards.studentsForAdmissionList') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <students-for-admission />
            </v-expansion-panel-content>
          </v-expansion-panel>

          <v-expansion-panel v-if="hasRefugeeApplicationsReadPermission">
            <v-expansion-panel-header>
              <div>
                {{ this.$t('refugee.admissionDetails.listTitle') }}
              </div>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <refugee-admission-list />
            </v-expansion-panel-content>
          </v-expansion-panel>

          <!-- https://github.com/Neispuo/students/issues/1200 -->
          <!-- Списък с ученици за отписване да се скрие от всички dashboards #1200 -->
          <!-- <v-expansion-panel>
            <v-expansion-panel-header>
              <div>{{ this.$t('dashboards.studentsForDischargeList') }}</div>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <students-for-discharge />
            </v-expansion-panel-content>
          </v-expansion-panel> -->

          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ this.$t('environmentCharacteristics.statList.title') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <student-environment-characteristic-list />
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ this.$t('lodFinalization.unsignedStatList.title') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <student-unsigned-lod-list />
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-expansion-panel v-if="hasDualFormReadPermission">
            <v-expansion-panel-header>
              {{ this.$t('dualFormEdu.dashboardTitle') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <dual-form-list />
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ this.$t('externalEval.dashboardTitle') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <student-external-evaluation-list />
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import StudentStats from '@/components/admin/stats/StudentStats.vue';
import ClassGroupStats from '@/components/admin/stats/ClassGroupStats.vue';
import DiplomaStats from '@/components/admin/stats/DiplomaStats.vue';
import RefugeeAdmissionList from "@/components/refugee/RefugeeAdmissionList";
// import StudentsForDischarge from "@/components/admin/directorsDashboard/StudentsForDischarge";
import SopEnrollmentDetails from "@/components/admin/SopEnrollmentDetails.vue";
import ActiveCampaigns from "@/components/admin/directorsDashboard/ActiveCampaigns";
import StudentsForAdmission from '@/components/admin/directorsDashboard/StudentsForAdmission';
import StudentEnvironmentCharacteristicList from '@/components/admin/StudentEnvironmentCharacteristicList';
import StudentUnsignedLodList from '@/components/admin/StudentUnsignedLodList';
import DualFormList from '@/components/students/DualFormList.vue';
import StudentExternalEvaluationList from '@/components/admin/StudentExternalEvaluationList.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "MONDashboard",
  components: {
    StudentStats,
    ClassGroupStats,
    DiplomaStats,
    RefugeeAdmissionList,
    // StudentsForDischarge,
    SopEnrollmentDetails,
    ActiveCampaigns,
    StudentsForAdmission,
    StudentEnvironmentCharacteristicList,
    StudentUnsignedLodList,
    DualFormList,
    StudentExternalEvaluationList
  },
  data() {
    return {
      expandablePanelModel: []
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnRefugeeModule']),
    hasRefugeeApplicationsReadPermission() {
      return this.turnOnRefugeeModule && this.hasPermission(Permissions.PermissionNameForRefugeeApplicationsRead);
    },
    hasSopEnrollmentDetailsReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForSopEnrollmentDetailsRead
      );
    },
    hasDualFormReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForDualFormRead
      );
    }
  }
};
</script>
