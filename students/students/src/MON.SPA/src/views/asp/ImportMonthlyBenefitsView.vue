<template>
  <div>
    <ImportMonthlyBenefits
      v-if="hasAspImportPermission"
      :file="file"
      @submitFile="onSubmitFile"
    />

    <ImportedMonthlyBenefitsList
      ref="importedBenefitsList"
      class="mt-10"
      :institution-id="institutionId"
    />
  </div>
</template>

<script>
import ImportMonthlyBenefits from '@/components/asp/ImportMonthlyBenefits.vue';
import ImportedMonthlyBenefitsList from '@/components/asp/ImportedMonthlyBenefitsList.vue';
import UserService from '@/services/user.service.js';
import { UserInfo } from '@/models/account/userInfo';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'MonthlyBenefitsView',
  components: {
      ImportedMonthlyBenefitsList,
      ImportMonthlyBenefits
    },
  data() {
    return {
      file: '',
      institutionId: null,
      userInfo: null,
    };
  },
  computed:{
    ...mapGetters(['hasPermission']),
    hasAspImportPermission() {
      return this.hasPermission(Permissions.PermissionNameForASPImport);
    },
    hasAspManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForASPManage);
    },
  },
  beforeMount(){
    if(!this.hasAspImportPermission && !this.hasAspManagePermission) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.init();
  },
  methods:{
      async init(){
          if (!!this.institutionId == false){
              UserService.getUserInfo().then((data) => {
                  this.userInfo = new UserInfo(data.data);
                  this.institutionId = this.userInfo.institutionId;
              });
          }
      },
      async onSubmitFile(){
        this.$refs.importedBenefitsList.gridReload();
      }
  }
};
</script>
