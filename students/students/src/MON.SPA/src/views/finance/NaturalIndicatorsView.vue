<template>
  <natural-indicators />
</template>

<script>
import NaturalIndicators from "@/components/finance/NaturalIndicators.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "NaturalIndicatorsView",
  components: {
    NaturalIndicators
  },
  computed: {
    ...mapGetters(['hasPermission'])
  },
  beforeMount() {
    if(!this.hasPermission(Permissions.PermissionNameForNaturalIndicatorsManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
