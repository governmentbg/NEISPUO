<template>
  <div>
    <v-btn
      icon
      color="primary"
      large
      @click.stop="timelineDrawer = !timelineDrawer"
    >
      <v-icon>mdi-timeline-clock</v-icon>
    </v-btn>
    <v-navigation-drawer
      v-if="statuses && statuses.length"
      v-model="timelineDrawer"
      right
      width="600"
      disable-resize-watcher
      disable-route-watcher
      fixed
    >
      <v-app-bar
        flat
        color="transparent"
      >
        <slot name="title" />
        <v-spacer />
        <v-btn
          icon
          @click="timelineDrawer = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-app-bar>
      <v-timeline
        dense
      >
        <v-timeline-item
          v-for="(item, index) in statuses"
          :key="index"
          color="primary"
          icon-color="primary"
          small
        >
          <doc-management-application-status
            :value="item"
            @actionResponse="onActionResponseSaved"
          />
        </v-timeline-item>
      </v-timeline>

      <!-- Floating close button on left border -->
      <v-btn
        fab
        color="primary"
        class="timeline-close-btn"
        @click="timelineDrawer = false"
      >
        <v-icon>mdi-chevron-right</v-icon>
      </v-btn>
    </v-navigation-drawer>
  </div>
</template>

<style scoped>
.timeline-close-btn {
  position: absolute !important;
  top: 50%;
  left: 0px;
  transform: translateY(-50%);
  z-index: 1000;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3) !important;
}
</style>

<script>
import DocManagementApplicationStatus from '@/components/docManagement/DocManagementApplicationStatus.vue';
export default {
  name: 'DocManagementApplicationTimeLine',
  components: {
    DocManagementApplicationStatus,
  },
  props: {
    applicationId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      statuses: null,
      timelineDrawer: false,
    };
  },
  mounted() {
    this.get();
  },
  methods: {
     get() {
       this.$api.docManagementApplication.statuses(this.applicationId)
         .then((response) => {
           this.statuses = response.data;
         })
         .catch((error) => {
           this.$notifier.error('', this.$t("common.loadError"), 5000);
           console.log(error.response);
         });
     },
     onActionResponseSaved() {
        this.get();
        this.$emit('actionResponse');
    }
  },
};
</script>
