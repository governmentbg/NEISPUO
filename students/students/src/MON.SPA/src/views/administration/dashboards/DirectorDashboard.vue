<template>
  <v-card>
    <v-card-title>
      {{ this.$t("dashboards.directorDashboardTitle") }}
    </v-card-title>
    <v-card-subtitle>
      {{ this.$t("dashboards.directorDashboardSubtitle") }}
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
          <v-expansion-panel v-if="hasAdmissionPermissionRequestReadPermission">
            <v-expansion-panel-header>
              <div>
                {{ this.$t('studentAdmissionDocumentPermissionRequest.listTitle') }} -
                <v-avatar
                  v-if="admissionDocumentPermissionRequestPendingCount"
                  color="green"
                  :size="admissionDocumentPermissionRequestPendingCount < 100 ? '25' : 35"
                >
                  {{ admissionDocumentPermissionRequestPendingCount }}
                </v-avatar>
              </div>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <student-admission-document-permission-request-list
                :hide-title="true"
              />
            </v-expansion-panel-content>
          </v-expansion-panel>

          <v-expansion-panel v-if="hasSopEnrollmentDetailsReadPermission">
            <v-expansion-panel-header>
              <div>
                {{ this.$t('sop.enrollmentDetails.title') }} -
                <v-avatar
                  v-if="sopEnrollmentDetailsTotalCount"
                  :size="sopEnrollmentDetailsTotalCount < 100 ? '25' : 35"
                  color="green"
                >
                  {{ sopEnrollmentDetailsTotalCount }}
                </v-avatar>
              </div>
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

          <!-- https://github.com/Neispuo/students/issues/1200 -->
          <!-- Списък с ученици за отписване да се скрие от всички dashboards #1200 -->
          <!-- <v-expansion-panel>
            <v-expansion-panel-header>
              {{ this.$t('dashboards.studentsForDischargeList') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <students-for-discharge />
            </v-expansion-panel-content>
          </v-expansion-panel> -->

          <v-expansion-panel v-if="hasRefugeeApplicationsReadPermission">
            <v-expansion-panel-header>
              <div>
                {{ this.$t('refugee.admissionDetails.listTitle') }}
                <v-tooltip
                  v-if="refugeeApplicationsPendingAdmissionCount"
                  bottom
                >
                  <template v-slot:activator="{ on: tooltip }">
                    <v-avatar
                      color="green"
                      :size="refugeeApplicationsPendingAdmissionCount < 100 ? '25' : 35"
                      class="px-2"
                      v-on="{ ...tooltip }"
                    >
                      <span>{{ refugeeApplicationsPendingAdmissionCount }}</span>
                    </v-avatar>
                  </template>
                  <span> {{ $t('refugee.admissionDetails.pending') }}</span>
                </v-tooltip>
              </div>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <refugee-admission-list />
            </v-expansion-panel-content>
          </v-expansion-panel>

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
import { DirectorDashboardModel } from '@/models/admin/directorDashboardModel.js';
import StudentStats from '@/components/admin/stats/StudentStats.vue';
import ClassGroupStats from '@/components/admin/stats/ClassGroupStats.vue';
import DiplomaStats from '@/components/admin/stats/DiplomaStats.vue';
import SopEnrollmentDetails from '@/components/admin/SopEnrollmentDetails.vue';
import StudentAdmissionDocumentPermissionRequestList from '@/components/tabs/studentMovement/StudentAdmissionDocumentPermissionRequestList.vue';
// import StudentsForDischarge from '@/components/admin/directorsDashboard/StudentsForDischarge';
import StudentsForAdmission from '@/components/admin/directorsDashboard/StudentsForAdmission';
import ActiveCampaigns from '@/components/admin/directorsDashboard/ActiveCampaigns';
import RefugeeAdmissionList from '@/components/refugee/RefugeeAdmissionList';
import StudentEnvironmentCharacteristicList from '@/components/admin/StudentEnvironmentCharacteristicList';
import StudentUnsignedLodList from '@/components/admin/StudentUnsignedLodList';
import DualFormList from '@/components/students/DualFormList.vue';
import StudentExternalEvaluationList from '@/components/admin/StudentExternalEvaluationList.vue';
import { Permissions, UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'DirectorDashboard',
  components: {
    StudentStats,
    ClassGroupStats,
    DiplomaStats,
    SopEnrollmentDetails,
    StudentAdmissionDocumentPermissionRequestList,
    // StudentsForDischarge,
    StudentsForAdmission,
    RefugeeAdmissionList,
    ActiveCampaigns,
    StudentEnvironmentCharacteristicList,
    StudentUnsignedLodList,
    DualFormList,
    StudentExternalEvaluationList,
  },
  data() {
    return {
      dashboardData: new DirectorDashboardModel(),
      expandablePanelModel: [],
      sopEnrollmentDetailsTotalCount: undefined,
      admissionDocumentPermissionRequestPendingCount: undefined,
      refugeeApplicationsPendingAdmissionCount: undefined
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnRefugeeModule', 'isInRole']),
    hasSopEnrollmentDetailsReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForSopEnrollmentDetailsRead
      );
    },
    hasAdmissionPermissionRequestReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForAdmissionPermissionRequestRead
      );
    },
    hasRefugeeApplicationsReadPermission() {
      return this.turnOnRefugeeModule && this.hasPermission(Permissions.PermissionNameForRefugeeApplicationsRead);
    },
    hasRefugeeApplicationsPendingAdmissionCount() {
      return this.isInRole(UserRole.School) || this.isInRole(UserRole.Consortium);
    },
    hasDualFormReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForDualFormRead
      );
    }
  },
  beforeMount() {
    this.init();
    this.getCounters();
  },
  methods: {
    async init() {
      this.$api.admin.getDirectorDashboard().then((result) => {
        this.dashboardData = new DirectorDashboardModel(result.data);
      });
    },
    getCounters() {
      if (this.hasAdmissionPermissionRequestReadPermission) {
        this.$api.admin
          .getInstitutionSopEnrollmentsCount()
          .then((response) => {
            const count = Number(response.data);
            if (!isNaN(count)) {
              this.sopEnrollmentDetailsTotalCount = count;
            }
          })
          .catch((error) => {
            console.log(error.response);
          });
      }

      if (this.hasAdmissionPermissionRequestReadPermission) {
        this.$api.studentAdmissionDocumentPermissionRequest
          .countPending()
          .then((response) => {
            const count = Number(response.data);
            if (!isNaN(count)) {
              this.admissionDocumentPermissionRequestPendingCount = count;
            }
          })
          .catch((error) => {
            console.log(error.response);
          });
      }

      if(this.hasRefugeeApplicationsPendingAdmissionCount) {
        this.$api.refugee
          .countPendingAdmissions()
          .then((response) => {
            const count = Number(response.data);
            if (!isNaN(count)) {
              this.refugeeApplicationsPendingAdmissionCount = count;
            }
          })
          .catch((error) => {
            console.log(error.response);
          });
      }
    },
  },
};
</script>
