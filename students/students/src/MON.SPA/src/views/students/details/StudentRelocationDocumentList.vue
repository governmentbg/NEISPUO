<template>
  <relocation-document-list
    :person-id="id"
  />
</template>

<script>
import RelocationDocumentList from '@/components/tabs/studentMovement/RelocationDocumentList';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentRelocationDocumentList',
  components: {
    RelocationDocumentList
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>