<template>
  <div>
    <student-diploma-document
      :person-id="personId"
      :template-id="templateId"
      :basic-document-id="basicDocumentId"
      :basic-class-id="basicClassId"
      action-name="DiplomaCreate"
      form-name="StudentDiplomaCreateForm_"
    />
  </div>
</template>

<script>
import StudentDiplomaDocument from '@/components/diplomas/StudentDiplomaDocument.vue';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentDiplomaCreateView',
  components: {
    StudentDiplomaDocument
  },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    templateId: { // Създаване на диплома чрез шаблон.
      type: Number,
      default() {
        return null;
      }
    },
    basicDocumentId: { // Създаване на диплома чрез BasicDocument.
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
    },
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'hasPermission']),
    hasStudentDiplomaManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaManage);
    },
    hasStudentDiplomaByCreateRequestManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentDiplomaByCreateRequestManage);
    },
    hasAdminDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage);
    },
    hasMonHrDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage);
    },
    hasRuoHrDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage);
    },
    hasInstitutionDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage);
    }
  },
  async mounted() {
    if (!(this.hasStudentDiplomaManagePermission || this.hasStudentDiplomaByCreateRequestManagePermission
      || this.hasAdminDiplomaManagePermission || this.hasInstitutionDiplomaManagePermission
      || this.hasMonHrDiplomaManagePermission || this.hasRuoHrDiplomaManagePermission)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
