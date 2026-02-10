<template>
  <div>
    <reg-book-list
      :ref="'grid' + _uid"
      :institution-id="userInstitutionId"
      :reg-book-type="RegBookType.RegBookQualificationDuplicate"
      :title="$t('menu.regBooks.qualificationDuplicates')"
    />
  </div>
</template>

<script>
import RegBookList from '@/components/diplomas/RegBookList.vue';
import { mapGetters } from 'vuex';
import { Permissions, RegBookType } from '@/enums/enums';

export default {
  name: 'RegBookQualificationDuplicatesListView',
  components: {
    RegBookList,
  },
  data() {
    return {
      RegBookType: RegBookType
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId', 'hasPermission', 'mode']),
    isInDevMode() {
      return this.mode !== 'prod';
    },
    hasManagePermission() {
      return (this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage));
    },
    hasReadPermission() {
      return (this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaRead)
        || this.hasPermission(Permissions.PermissionNameForAdminDiplomaRead)
        || this.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead));
    },
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaRead)
      && !this.hasManagePermission
      && !this.hasReadPermission)
      return this.$router.push('/errors/AccessDenied');
  },
  methods: {
    onSubmitFile() {
       const grid = this.$refs['grid' + this._uid];
       if (grid) {
         grid.refresh();
       }
    }
  },
};
</script>
