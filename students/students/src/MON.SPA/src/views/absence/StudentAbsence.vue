<template>
  <student-absence :student-id="id" />
</template>

<script>
import StudentAbsence from "@/components/absence/StudentAbsence.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";


export default {
  name: "StudentAbsenceView",
  components: { StudentAbsence },
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAbsenceRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>