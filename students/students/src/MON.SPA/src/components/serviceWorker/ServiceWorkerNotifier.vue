<template>
  <v-snackbar
    :value="updateExists"
    :vertical="true"
    color="primary"
    :timeout="-1"
  >
    {{ $t('versionChangeMsg') }}

    <template v-slot:action="{ attrs }">
      <v-btn
        text
        v-bind="attrs"
        @click="refreshApp"
      >
        {{ $t('buttons.reload') }}
      </v-btn>
    </template>
  </v-snackbar>
</template>
<script>
export default {
  name: 'ServiceWorkerNotifier',
  data() {
    return {
      refreshing: false,
      registration: null,
      updateExists: false,
    };
  },
  created() {
    this.swUpdated();
  },
  methods: {
    swUpdated() {
      // Listen for our custom event from the SW registration
      document.addEventListener('swUpdated', this.updateAvailable, { once: true });

      // Prevent multiple refreshes
      if(navigator && navigator.serviceWorker) {
        navigator.serviceWorker.addEventListener('controllerchange', () => {
          if (this.refreshing) return;
          this.refreshing = true;
          // Here the actual reload of the page occurs
          window.location.reload();
        });
      }
    },
    updateAvailable(event) {
      this.registration = event.detail;
      this.updateExists = true;
    },
    refreshApp() {
      this.updateExists = false;
      // Make sure we only send a 'skip waiting' message if the SW is waiting
      if (!this.registration || !this.registration.waiting) return;
      // Send message to SW to skip the waiting and activate the new SW
      this.registration.waiting.postMessage({ type: 'SKIP_WAITING' });
    }
  }
};
</script>
