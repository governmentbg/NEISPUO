<template>
  <div>
    <v-card>
      <div class="d-flex flex-no-wrap justify-space-between">
        <v-avatar
          class="ma-3"
          tile
        >
          <v-icon
            x-large
            :color="getColor()"
          >
            {{ getIcon() }}
          </v-icon>
        </v-avatar>

        <div class="overflow-auto">
          <v-card-title
            v-if="notification.title"
            class="headline"
            style="word-break: normal;"
            v-text="notification.title"
          />

          <v-card-subtitle v-if="options.showSubtitle">
            {{ notification.time ? moment(notification.time).format("DD.MM.YYYY HH:mm") : '' }}
          </v-card-subtitle>

          <v-card-text>
            <pre v-if="options.showTextInPreTag">
              {{ notification.text }}
            </pre>
            <ul v-else-if="Array.isArray(notification.text)">
              <li
                v-for="(val, index) in notification.text"
                :key="index"
              >
                {{ val }}
              </li>
            </ul>
            <p
              v-else
              class="overflow-y-scroll expandable-text"
            >
              {{ notification.text }}
            </p>
          </v-card-text>

          <v-divider />

          <v-card-actions>
            <slot name="actions">
              <div>Default actions slot</div>
            </slot>
          </v-card-actions>
        </div>
      </div>
    </v-card>
  </div>
</template>

<script>
import { NotificationModel } from "@/models/notification/notificationModel.model.js";
import { NotificationModalOptions } from '@/models/notification/notificationModalOptions.js';
import { NotificationSeverity } from '@/enums/enums';
import moment from "moment";

export default {
  name: 'NotificationCard',
  props: {
    value: {
      type: Object,
      required: true,
      default() {
        return new NotificationModel();
      }
    },
    options: {
      type: Object,
      default() {
        return new NotificationModalOptions();
      }
    }
  },
  data() {
    return {
      moment: moment,
      notification: new NotificationModel(this.value),
    };
  },
  methods: {
    getIcon() {
      switch (this.notification.severity) {
        case NotificationSeverity.Success: {
          return "mdi-check-circle-outline";
        }
        case NotificationSeverity.Warn: {
          return "mdi-alert-outline";
        }
        case NotificationSeverity.Error: {
          return "mdi-alert-circle-outline";
        }
        case NotificationSeverity.Info: {
          return "mdi-information-outline";
        }
        default: {
          return "";
        }
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
  }
};
</script>

<style lang="scss" scoped>
.overflow-auto {
    overflow: auto;
    min-width: 85%;
}
.overflow-y-scroll {
    max-height: 300px;
    overflow-y: auto;
}
.expandable-text {
  width: 100%;
  display: inline-block;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}
.expandable-text:hover {
  white-space: normal;
  /* or:
  width: auto;
  */
}
</style>
