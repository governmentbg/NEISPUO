<template>
  <div>
    <v-dialog
      v-if="notification"
      v-model="dialog"
      :disabled="options.disabled"
      :fullscreen="options.fullscreen"
      :light="options.light"
      :persistent="options.persistent"
      :scrollable="options.scrollable"
      :width="options.width"
      :max-width="options.maxWidth"
    >
      <NotificationCard
        v-if="dialog"
        ref="notificationCard"
        :value="notification"
        :options="options"
      >
        <template #actions>
          <v-spacer />
          <v-btn
            :color="getColor()"
            text
            @click="dialog = false"
          >
            {{ $t("buttons.ok") }}
          </v-btn>
        </template>
      </NotificationCard>
    </v-dialog>
  </div>
</template>

<script>
import { notifierEvents } from "./notifierEvents.bus";
import NotificationCard from "./NotificationCard.vue";
import { NotificationModalOptions } from '@/models/notification/notificationModalOptions.js';
import { NotificationSeverity } from '@/enums/enums';

export default {
    name: "NotificationModal",
    components: { NotificationCard },
    data() {
      return {
        dialog: false,
        options: new NotificationModalOptions(),
        notification: null
      };
    },
    mounted() {
      notifierEvents.$on('show-modal', this.showNotificationModal);
    },
    destroyed() {
      notifierEvents.$off('show-modal');
    },
    methods: {
      showNotificationModal(notification, options) {
        this.options = new NotificationModalOptions(options);
        if (notification) {
          this.notification = notification;
          this.dialog = true;
        } else {
          this.notification = null;
          this.dialog = false;
        }
      },
      getColor() {
        switch (this.notification.severity) {
          case NotificationSeverity.Success: {
            return "success";
          }
          case NotificationSeverity.Warn: {
            return "warning";
          }
          case NotificationSeverity.Error: {
            return "error";
          }
          case NotificationSeverity.Info: {
            return "info";
          }
          default: {
            return "primary";
          }
        }
      }
      // getColor() {
      //   const card = this.$refs.notificationCard;
      //   return card ? card.getColor(this.notification.severity) : '';
      // }
    }
};
</script>