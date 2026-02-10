<template>
  <discharge-document-list
    :person-id="id"
    :education="education"
  />
</template>

<script>
import DischargeDocumentList from '@/components/tabs/studentMovement/DischargeDocumentList';
import { StudentEducationModel } from "@/models/studentMovement/studentEducationModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentDischargeDocumentsList',
  components: {
    DischargeDocumentList
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      education: new StudentEducationModel()
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
