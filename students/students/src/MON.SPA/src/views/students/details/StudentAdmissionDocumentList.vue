<template>
  <admission-document-list :person-id="id" />
</template>

<script>
import AdmissionDocumentList from "@/components/tabs/studentMovement/AdmissionDocumentList";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "StudentAdmissionDocumentList",
  components: {
    AdmissionDocumentList,
  },
  props: {
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {};
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
