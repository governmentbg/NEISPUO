<template>
  <class-absences
    :class-id="classId"
    :school-year="schoolYear"
  />
</template>

<script>
    import ClassAbsences from '@/components/absence/ClassAbsences.vue';
    import { Permissions } from '@/enums/enums';
    import { mapGetters } from 'vuex';

    export default {
        name: 'ClassAbsenceView',
        components: {ClassAbsences},
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
            if(!this.hasClassPermission(Permissions.PermissionNameForClassAbsenceRead)) {
                return this.$router.push('/errors/AccessDenied');
            }
        },
    };
</script>
