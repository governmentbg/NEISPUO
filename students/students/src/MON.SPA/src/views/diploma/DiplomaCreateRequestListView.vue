<template>
  <diploma-create-request-list />
</template>

<script>
import DiplomaCreateRequestList from "@/components/diplomas/DiplomaCreateRequestList";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
    name: "StudentAdmissionDocumentPermissionRequestListView",
    components: {
        DiplomaCreateRequestList,
    },
    computed: {
      ...mapGetters(['hasPermission']),
    },
    mounted() {
      if(!this.hasPermission(Permissions.PermissionNameForDiplomaCreateRequestRead)) {
        return this.$router.push('/errors/AccessDenied');
      }
    }
};
</script>
