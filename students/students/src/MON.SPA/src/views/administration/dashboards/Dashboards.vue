<template>
  <div>
    <account-profile :extended="false" />
    <div class="mt-2">
      <div v-if="isInRole(UserRole.School)">
        <director-dashboard />
      </div>
      <div v-if="isInRole(UserRole.Teacher)">
        <lead-teacher-dashboard />
      </div>
      <div v-if="isInRole(UserRole.Mon) || isInRole(UserRole.MonExpert)">
        <MON-dashboard />
      </div>
      <div v-if="isInRole(UserRole.Cioo)">
        <CIOO-dashboard />
      </div>
      <div v-if="isInRole(UserRole.Ruo) || isInRole(UserRole.RuoExpert)">
        <RUO-dashboard />
      </div>
      <div v-if="isInRole(UserRole.Consortium)">
        <consortium-dashboard />
      </div>
    </div>
  </div>
</template>

<script>
import DirectorDashboard from './DirectorDashboard.vue';
import LeadTeacherDashboard from './LeadTeacherDashboard.vue';
import MONDashboard from './MONDashboard.vue';
import RUODashboard from './RUODashboard.vue';
import CIOODashboard from './CIOODashboard.vue';
import ConsortiumDashboard from './ConsortiumDashboard.vue';
import { mapGetters } from "vuex";
import { UserRole } from '@/enums/enums';
import AccountProfile from '@/components/account/Profile.vue';

export default {
  name: 'Dashboards',
  components: {
    DirectorDashboard,
    LeadTeacherDashboard,
    MONDashboard,
    RUODashboard,
    CIOODashboard,
    AccountProfile,
    ConsortiumDashboard
  },
  data() {
    return {
      UserRole: UserRole
    };
  },
  computed:{
    ...mapGetters(['isInRole'])
  },
};
</script>
