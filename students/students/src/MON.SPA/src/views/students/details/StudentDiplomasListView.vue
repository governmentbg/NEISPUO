<template>
  <div>
    <diploma-list
      :institution-id="userInstitutionId"
      :person-id="id"
      :title="isValidation ? $t('validationDocument.list') : $t('diplomas.diplomaList')"
      :is-validation="isValidation"
      :is-equalization="isRuo"
    />
  </div>
</template>

<script>
import DiplomaList from '@/components/diplomas/DiplomaList.vue';
import { Permissions, UserRole } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentDiplomasListView',
  components: {
    DiplomaList
  },
  props: {
    id: {
      type: Number,
      required: true
    },
    isValidation: {
      type: Boolean,
      default() {
        return false;
      }
    },
  },
  computed: {
    ...mapGetters(['userInstitutionId', 'hasStudentPermission', 'isInRole']),
    isRuo() {
      return this.isInRole(UserRole.Ruo) || this.isInRole(UserRole.RuoExpert);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaRead)
      && !this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaByCreateRequestRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
