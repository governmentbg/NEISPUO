<template>
  <other-document-list
    :person-id="id"
  />
</template>

<script>
import OtherDocumentList from '@/components/tabs/otherDocuments/OtherDocumentList.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentOtherDocumentList',
  components: {
    OtherDocumentList
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'hasPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherDocumentRead)
      && !this.hasPermission(Permissions.PermissionNameForStudentOtherDocumentManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
