<template>
  <div>
    <v-snackbar
      v-model="snackbar"
      center
      :timeout="timeout"
      :multi-line="multiLine"
      :color="snackbarColor"
    >
      <div v-if="notification">
        <h2>{{ notification.title }}</h2>
        <h4>{{ notification.text }}</h4>  
      </div>

      <template v-slot:action="{ attrs }">
        <v-btn
          v-bind="attrs"
          depressed
          :color="snackbarColor + ' lighten-1'"
          @click="snackbar = false"
        >
          {{ $t('buttons.close') }}
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script>
import { notifierEvents } from "./notifierEvents.bus";
import { NotificationSeverity, NotificationSeverityColor } from '@/enums/enums';

export default {
  name: 'NotificationSnackbar',
  data: () => ({
    snackbar: false,
    multiLine: true,
    timeout: -1,
    notification: null
  }),
  computed: {
    snackbarColor() {
      if(!this.notification) return NotificationSeverityColor.Error;
      switch(this.notification.severity) {
        case NotificationSeverity.Info:
          return NotificationSeverityColor.Info;
        case NotificationSeverity.Success:
          return NotificationSeverityColor.Success;
        case NotificationSeverity.Warn:
          return NotificationSeverityColor.Warn;
        case NotificationSeverity.Error:
        default:
          return NotificationSeverityColor.Error;
      }
    }
  },
  mounted() {
    notifierEvents.$on('show-snackbar', this.showSnackbar);
  },
  destroyed() {
    notifierEvents.$off('show-snackbar');
  },
  methods: {
    showSnackbar(notification) {
      if (notification) {
        this.notification = notification;
        this.snackbar = true;
        this.timeout = notification.timeout;
      } else {
        this.notification = null;
        this.snackbar = false;
        this.timeout = -1;
      }
    }
  }
};
</script>