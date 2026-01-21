<template>
  <grid
    ref="SopEnrollmentDetailsGrid"
    url="/api/admin/GetSopEnrollmentDetails"
    :headers="headers"
    item-key="uid"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    file-export-name="Ученици със СОП"
    title=""
    @gridDataLoaded="$emit('gridDataLoaded', $event)"
  >
    <template #actions="item">
      <button-group>
        <button-tip
          :to="`/student/${item.item.personId}/institutionDetails`"
          icon
          icon-color="primary"
          iclass=""
          icon-name="mdi-eye"
          small
          tooltip="student.menu.lod"
          bottom
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
export default{
  name: 'SopEnrollmentDetails',
  components: {
    Grid
  },
  data() {
    return {
      headers: [
        {text: this.$t('sop.enrollmentDetails.headers.name'), value: "fullName", sortable: true},
        {text: this.$t('sop.enrollmentDetails.headers.age'), value: "age", sortable: true},
        {text: this.$t('sop.enrollmentDetails.headers.basicInstitutionName'), value: "mainInstName", sortable: true},
        {text: this.$t('sop.enrollmentDetails.headers.basicInstitutionRegion'), value: "mainInstRegionName", sortable: true},
        {text: this.$t('sop.enrollmentDetails.headers.sopInstitutionName'), value: "csopInstName", sortable: true},
        {text: this.$t('sop.enrollmentDetails.headers.sopInstitutionRegion'), value: "csopRegionName", sortable: true},
        {text: '', value: 'controls', inFavourOfIdentifier: false, sortable: false, align: 'end'},
      ]
    };
  }
};
</script>
