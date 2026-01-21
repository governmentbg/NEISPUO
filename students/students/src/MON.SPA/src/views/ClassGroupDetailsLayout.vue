<template>
  <div>
    <v-container fluid>
      <class-group-profile
        class="mb-2"
        :class-group-id="id"
      />
      <router-view
        v-if="permissionsLoaded"
      />
    </v-container>
  </div>
</template>

<script>
import ClassGroupProfile from '@/components/students/class/Profile.vue';
import { mapGetters, mapActions } from 'vuex';

export default {
  name: 'ClassGroupDetailsLayout',
  components: {
    ClassGroupProfile
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
    await this.loadPermissionsForClass(this.id);
    this.permissionsLoaded = true;
  },
  mounted() {
    if(!this.isInStudentsModuleRole) {
      return this.$router.push("/errors/AccessDenied");
    }
  },
  beforeDestroy() {
    this.clearPermissionsForClass();
  },
  methods: {
    ...mapActions(['clearPermissionsForClass', 'loadPermissionsForClass'])
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
