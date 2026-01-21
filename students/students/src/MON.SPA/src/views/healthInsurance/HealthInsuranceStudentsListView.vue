<template>
  <students-list />
</template>

<script>
import StudentsList from "@/components/healthInsurance/HealthInsuranceStudentsList.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "HealthInsuranceStudentsListView",
  components: {
    StudentsList
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
