<template>
  <div>
    <v-container fluid>
      <institution-profile
        class="mb-2"
        :institution-id="id"
      />
      <router-view
        v-if="permissionsLoaded"
      />
    </v-container>
  </div>
</template>

<script>
import InstitutionProfile from '@/components/institution/Profile.vue';
import { mapGetters, mapActions } from 'vuex';

export default {
  name: 'InstitutionDetailsLayout',
  components: {
    InstitutionProfile
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      permissionsLoaded: false
    };
  },
  computed: {
    ...mapGetters(['isInStudentsModuleRole'])
  },
  async created() {
    await this.loadPermissionsForInstitution(this.id);
    this.permissionsLoaded = true;
  },
  mounted() {
    if(!this.isInStudentsModuleRole) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  beforeDestroy() {
    this.clearPermissionsForInstitution();
  },
  methods: {
    ...mapActions(['clearPermissionsForInstitution', 'loadPermissionsForInstitution'])
  }
};
</script>

<style scoped>
  .homeLink {
    text-decoration: none;
    color: white;
  }
  .wrap-text {
    white-space: normal !important;
    font-size: 0.929rem !important;
  }
</style>
