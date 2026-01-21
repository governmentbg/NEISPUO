<template>
  <v-card>
    <v-card-title>
      {{ this.$t('dashboards.leadTeacher.title') }}
    </v-card-title>
    <v-card-subtitle>
      {{ this.$t('dashboards.leadTeacher.subtitle') }}
    </v-card-subtitle>
    <v-card-text>
      <v-row
        dense
        class="mt-2"
      >
        <v-expansion-panels

          multiple
          popout
        >
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ this.$t('environmentCharacteristics.statList.title') }}
            </v-expansion-panel-header>

            <v-expansion-panel-content>
              <student-environment-characteristic-list />
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
import StudentEnvironmentCharacteristicList from '@/components/admin/StudentEnvironmentCharacteristicList';
import DualFormList from '@/components/students/DualFormList.vue';
import StudentExternalEvaluationList from '@/components/admin/StudentExternalEvaluationList.vue';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default{
  name: 'LeadTeacherDashboard',
  components: {
    StudentEnvironmentCharacteristicList,
    DualFormList,
    StudentExternalEvaluationList
  },
  data() {
    return {
      expandablePanelModel: []
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasDualFormReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForDualFormRead
      );
    }
  }
};
</script>
