<template>
  <div>
    <active-absence-campaigns class="mb-2" />

    <absences-export
      @exportComplete="onExportComplete"
    />

    <absence-exports-list
      ref="absenceExportsList"
      class="mt-10"
    />
  </div>
</template>

<script>
import ActiveAbsenceCampaigns from '@/components/absence/ActiveAbsenceCampaigns.vue';
import AbsenceExportsList from "@/components/absence/AbsenceExportsList.vue";
import AbsencesExport from "@/components/absence/AbsencesExport.vue";

export default {
name: "AbsencesExportView",
components: {
  ActiveAbsenceCampaigns,
  AbsenceExportsList,
  AbsencesExport
},
data() {
    return {};
},
methods:{
    onExportComplete() {
       this.$refs.absenceExportsList.gridReload();
    }
  }
};
</script>
