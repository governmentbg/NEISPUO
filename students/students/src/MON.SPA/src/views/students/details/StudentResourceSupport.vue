<template>
  <resource-support
    :person-id="id"
  />
</template>

<script>
import ResourceSupport from '@/components/tabs/resourceSupport/ResourceSupport.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentResourceSupport',
  components: {
    ResourceSupport
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportRead)
      && !this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>