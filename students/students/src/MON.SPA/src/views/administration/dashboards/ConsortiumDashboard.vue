<template>
  <v-card :loading="loading">
    <v-card-title>
      {{ this.$t("dashboards.consortium.title") }}
    </v-card-title>
    <v-card-subtitle>
      {{ this.$t("dashboards.consortium.subtitle") }}
    </v-card-subtitle>
    <v-card-text>
      <v-row
        dense
        class="mt-2"
      >
        <v-expansion-panels
          v-model="expandablePanelModel"
          multiple
          popout
        >
          <v-expansion-panel v-if="hasRefugeeApplicationsReadPermission">
            <v-expansion-panel-header>
              <div>{{ this.$t('refugee.admissionDetails.listTitle') }}</div>
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
import RefugeeAdmissionList from '@/components/refugee/RefugeeAdmissionList';
import StudentEnvironmentCharacteristicList from '@/components/admin/StudentEnvironmentCharacteristicList';
import StudentUnsignedLodList from '@/components/admin/StudentUnsignedLodList';
import DualFormList from '@/components/students/DualFormList.vue';
import StudentExternalEvaluationList from '@/components/admin/StudentExternalEvaluationList.vue';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'ConsortiumDashboard',
  components: {
    RefugeeAdmissionList,
    StudentEnvironmentCharacteristicList,
    StudentUnsignedLodList,
    DualFormList,
    StudentExternalEvaluationList
},
  data() {
    return {
      loading: false,
      expandablePanelModel: []
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnRefugeeModule']),
    hasRefugeeApplicationsReadPermission() {
      return this.turnOnRefugeeModule && this.hasPermission(Permissions.PermissionNameForRefugeeApplicationsRead);
    },
    hasDualFormReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForDualFormRead
      );
    }
  }
};
</script>
