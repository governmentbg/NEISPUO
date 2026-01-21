<template>
  <add-curriculum
    :person-id="pid"
    :class-id="classId"
    :school-year="schoolYear"
  />
</template>

<script>
import AddCurriculum from '@/components/students/class/AddCurriculum.vue';
import { mapGetters } from "vuex";
import { Permissions } from '@/enums/enums';


export default {
  name: "AddCurriculumView",
  components: {
    AddCurriculum
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
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
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentCurriculumRead)
      && !this.hasStudentPermission(Permissions.PermissionNameForStudentCurriculumManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
