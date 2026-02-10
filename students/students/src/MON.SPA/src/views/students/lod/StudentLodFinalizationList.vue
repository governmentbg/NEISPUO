<template>
  <lod-finalization-list
    :person-id="id"
  />
</template>

<script>
import LodFinalizationList from '@/components/lod/LodFinalizationList.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentLodFinalizationList',
  components: {
      LodFinalizationList
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
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentLodFinalizationRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
