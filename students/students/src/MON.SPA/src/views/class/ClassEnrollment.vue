<template>
  <class-enrollment
    :class-id="classId"
    :school-year="schoolYear"
  />
</template>

<script>
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import ClassEnrollment from '@/components/class/ClassEnrollmentComp.vue';

export default {
  name: 'ClassEnrollmentView',
  components: { ClassEnrollment },
  props: {
    classId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    }
  },
  computed: {
    ...mapGetters(['hasClassPermission']),
  },
  mounted() {
    if (!this.hasClassPermission(Permissions.PermissionNameForStudentToClassEnrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
};
</script>
