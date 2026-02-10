<template>
  <institution-details
    v-if="instId"
    :institution-id="instId"
  />
</template>

<script>
    import InstitutionDetails from '@/components/institution/InstitutionDetails';
    import { UserInfo } from '@/models/account/userInfo';
    import { Permissions } from '@/enums/enums';
    import { mapGetters } from 'vuex';

    export default {
        name: 'InstitutionDetailsView',
        components: {InstitutionDetails},
        props:{
            institutionId: {
                type: Number,
                required: false,
                default: null
            }
        },
        data(){
            return{
                instId: null
            };
        },
        computed: {
          ...mapGetters(['hasInstitutionPermission', 'userDetails']),
          userInfo() {
            return new UserInfo(this.userDetails);
          }
        },
        beforeMount(){
            this.init();
        },
        mounted() {
            if(!this.hasInstitutionPermission(Permissions.PermissionNameForInstitutionClassesRead)) {
                return this.$router.push('/errors/AccessDenied');
            }
        },
        methods:{
            async init() {
                if (!!this.institutionId == false){
                  this.instId = this.userInfo.institutionId;
                }
                else{
                    this.instId = this.institutionId;
                }
            }
        }

    };
</script>
