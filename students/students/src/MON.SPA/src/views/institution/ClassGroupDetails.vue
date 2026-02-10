<template>
  <class-group-details
    :class-id="classId"
    :school-year="schoolYear"
  />
</template>

<script>
    import ClassGroupDetails from '@/components/institution/ClassGroupDetails.vue';
    import { Permissions } from '@/enums/enums';
    import { mapGetters } from 'vuex';

    export default {
        name: 'ClassGroupDetailsView',
        components: { ClassGroupDetails },
        props:{
            classId:{
                type: Number,
                required: true
            },
            schoolYear:{
                type: Number,
                required: true
            }
        },
        computed: {
          ...mapGetters(['hasClassPermission']),
        },
        mounted() {
            if(!this.hasClassPermission(Permissions.PermissionNameForClassStudentsRead)) {
                return this.$router.push('/errors/AccessDenied');
            }
        },
    };
</script>