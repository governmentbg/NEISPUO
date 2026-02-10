<template>
  <diploma-template-create
    :basic-document-id="basicDocumentId"
    :template-id="templateId"
    :basic-class-id="basicClassId"
  />
</template>

<script>
import DiplomaTemplateCreate from '@/components/diplomas/DiplomaTemplateCreate.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'DiplomaTemplateCreateView',
  components: {
    DiplomaTemplateCreate
  },
  props: {
    basicDocumentId: { // Създаване на шаблон по даден BasicDocument.
      type: Number,
      default() {
        return null;
      }
    },
    templateId: { // Създаване на шаблон чрез копиране/клониране на друг шаблон.
      type: Number,
      default() {
        return null;
      }
    },
    basicClassId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  computed: {
    ...mapGetters(['hasPermission'])
  },
  mounted() {
    if (!this.hasPermission(Permissions.PermissionNameForDiplomaTemplatesManage)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
