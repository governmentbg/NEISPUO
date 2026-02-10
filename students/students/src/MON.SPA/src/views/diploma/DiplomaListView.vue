<template>
  <v-tabs
    v-model="tab"
    centered
  >
    <v-tab
      key="diplomasList"
    >
      {{ $t('menu.diplomas.title') }}
    </v-tab>
    <v-tab
      key="validationsList"
    >
      {{ $t('validationDocument.menuTitle') }}
    </v-tab>
    <v-tab
      key="equalizationsList"
    >
      {{ $t('recognition.listTitle') }}
    </v-tab>
    <v-tab
      v-if="isInDevMode"
      key="unimportedDiplomasList"
    >
      {{ $t('menu.diplomas.unimportedTitle') }}
    </v-tab>
    <v-tab
      v-if="isInDevMode"
      key="diplomaCreateRequestList"
    >
      {{ $t('diplomas.createRequest.listTitle') }}
    </v-tab>
    <v-tabs-items
      v-model="tab"
    >
      <v-tab-item
        key="diplomasList"
      >
        <diploma-import
          v-if="hasManagePermission"
          class="mb-2"
          @submitFile="onSubmitFile"
        />
        <diploma-list
          :ref="'grid' + _uid"
          :institution-id="userInstitutionId"
          title=" "
        />
      </v-tab-item>
      <v-tab-item
        key="validationsList"
      >
        <diploma-list
          :ref="'validationsGrid' + _uid"
          :institution-id="userInstitutionId"
          is-validation
          title=" "
        />
      </v-tab-item>
      <v-tab-item
        key="equalizationsList"
      >
        <diploma-list
          :ref="'equalizationsGrid' + _uid"
          :institution-id="userInstitutionId"
          is-equalization
          title=" "
        />
      </v-tab-item>
      <v-tab-item
        v-if="isInDevMode"
        key="unimportedDiplomasList"
      >
        <unimported-diploma-list
          :ref="'unimportedDiplomasGrid' + _uid"
          :institution-id="userInstitutionId"
        />
      </v-tab-item>
      <v-tab-item
        v-if="isInDevMode"
        key="diplomaCreateRequestList"
      >
        <diploma-create-request-list
          hide-title
        />
      </v-tab-item>
    </v-tabs-items>
  </v-tabs>
</template>

<script>
import DiplomaList from '@/components/diplomas/DiplomaList.vue';
import UnimportedDiplomaList from '@/components/diplomas/UnimportedDiplomaList.vue';
import DiplomaImport from '@/components/diplomas/DiplomaImport.vue';
import DiplomaCreateRequestList from '@/components/diplomas/DiplomaCreateRequestList.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'DiplomaListView',
  components: {
    DiplomaList,
    DiplomaImport,
    UnimportedDiplomaList,
    DiplomaCreateRequestList
  },
  props: {
    selectedTab: {
      type: undefined,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      tab: this.selectedTab || this.$store.getters.diplomaListSelectedTab
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
  beforeDestroy() {
    this.$store.commit('setDiplomaListSelectedTab', this.tab);
  },
  methods: {
    onSubmitFile() {
       const grid = this.$refs['grid' + this._uid];
       if (grid) {
         grid.refresh();
       }

       const grid1 = this.$refs['unimportedDiplomasGrid' + this._uid];
       if (grid1) {
         grid1.refresh();
       }

       const grid2 = this.$refs['validationsGrid' + this._uid];
       if (grid2) {
        grid2.refresh();
       }

       const grid3 = this.$refs['validationsGrid' + this._uid];
       if (grid3) {
        grid3.refresh();
       }
    }
  },
};
</script>
