<template>
  <other-institution
    :person-id="id"
  />
</template>

<script>
import OtherInstitution from '@/components/tabs/studentOtherInstitutions/OtherInstitution.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentOtherInstitutions',
  components: {
    OtherInstitution
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionRead)
      && !this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>