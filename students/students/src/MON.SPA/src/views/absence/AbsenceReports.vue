<template>
  <div>
    <active-absence-campaigns class="mb-2" />
    <absence-reports-list />
  </div>
</template>

<script>
import AbsenceReportsList from '@/components/absence/AbsenceReportsList.vue';
import ActiveAbsenceCampaigns from '@/components/absence/ActiveAbsenceCampaigns.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: 'AbsenceReportsView',
  components: { AbsenceReportsList, ActiveAbsenceCampaigns },
  computed: {
    ...mapGetters(["hasPermission"]),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceRead);
    }
  },
  mounted() {
    if (!this.hasReadPermission){
      return this.$router.push("/errors/AccessDenied");
    }
  }
};
</script>
