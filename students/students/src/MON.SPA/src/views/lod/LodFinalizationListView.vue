<template>
  <lod-finalization-list />
</template>

<script>
import LodFinalizationList from '@/components/lod/LodFinalizationList.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'LodFinalizationListView',
  components: {
      LodFinalizationList
  },
  computed: {
    ...mapGetters(['hasPermission']),
  },
  mounted() {
    if (!this.hasPermission(Permissions.PermissionNameForLodFinalizationRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
