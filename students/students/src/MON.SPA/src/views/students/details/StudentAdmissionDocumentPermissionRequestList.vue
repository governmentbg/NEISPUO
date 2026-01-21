<template>
  <student-admission-document-permission-request-list />
</template>

<script>
import StudentAdmissionDocumentPermissionRequestList from "@/components/tabs/studentMovement/StudentAdmissionDocumentPermissionRequestList.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
    name: "StudentAdmissionDocumentPermissionRequestListView",
    components: {
        StudentAdmissionDocumentPermissionRequestList,
    },
    computed: {
      ...mapGetters(['hasPermission']),
    },
    mounted() {
      if(!this.hasPermission(Permissions.PermissionNameForAdmissionPermissionRequestRead)) {
        return this.$router.push('/errors/AccessDenied');
      }
    }
};
</script>
