<template>
  <environment-characteristics
    :person-id="id"
  />
</template>

<script>
import EnvironmentCharacteristics from '@/components/tabs/environmentCharacteristics/EnvironmentCharacteristics.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentEnvironmentCharacteristics',
  components: {
    EnvironmentCharacteristics
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicRead)
      && !this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
};
</script>
