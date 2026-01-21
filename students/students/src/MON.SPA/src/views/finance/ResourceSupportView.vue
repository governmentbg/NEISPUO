<template>
  <resource-support-data />
</template>

<script>
import ResourceSupportData from "@/components/finance/ResourceSupportData.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "ResourceSupportView",
  components: {
    ResourceSupportData
  },
  computed: {
    ...mapGetters(['hasPermission'])
  },
  beforeMount() {
    if(!this.hasPermission(Permissions.PermissionNameForResourceSupportDataManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
