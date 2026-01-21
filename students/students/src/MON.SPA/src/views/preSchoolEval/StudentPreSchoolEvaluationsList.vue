<template>
  <pre-school-evaluations-list :person-id="personId" />
</template>

<script>
import PreSchoolEvaluationsList from '@/components/preSchoolEval/PreSchoolEvaluationsList';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: 'StudentPreSchoolEvaluationsList',
  components: {
    PreSchoolEvaluationsList
  },
  props: {
    personId: {
      type: Number,
      required: true,
    }
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
   mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentPreSchoolEvaluationRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
