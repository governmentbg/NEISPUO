<template>
  <diploma-documents
    :diploma-id="diplomaId"
    :is-validation-document="isValidationDocument"
  />
</template>

<script>
import DiplomaDocuments from "@/components/tabs/diplomas/DiplomaDocuments.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentDiplomaDocuments',
  components: {
    DiplomaDocuments
  },
  props: {
    diplomaId: {
      type: Number,
      required: true
    },
    isValidationDocument: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  computed: {
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaRead)) {
        return this.$router.push('/errors/AccessDenied');
      }
  },
};
</script>
