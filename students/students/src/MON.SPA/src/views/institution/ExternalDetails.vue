<template>
  <institution-external-list
    v-if="instId"
    :institution-id="instId"
  />
</template>

<script>
    import InstitutionExternalList from '@/components/institution/InstitutionExternalList';
    import { UserInfo } from '@/models/account/userInfo';
    import { mapGetters } from 'vuex';

    export default {
        name: 'List',
        components: { InstitutionExternalList },
        props:{
            institutionId:{
                type: Number,
                required: false,
                default: null
            }
        } ,
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
        methods:{
            async init(){
                if (!!this.institutionId == false) {
                  this.instId = this.userInfo.institutionId;
                }
                else{
                    this.instId = this.institutionId;
                }
            }
        }
    };
</script>
