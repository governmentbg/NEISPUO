<template>
  <exported-files-list />
</template>

<script>
import ExportedFilesList from "@/components/healthInsurance/HealthInsuranceExportedFilesList.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "HealthInsuranceExportedFilesListView",
  components: {
    ExportedFilesList
  },
  computed: {
    ...mapGetters(['hasPermission'])
  },
  beforeMount() {
    if(!this.hasPermission(Permissions.PermissionNameForHealthInsuranceManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
