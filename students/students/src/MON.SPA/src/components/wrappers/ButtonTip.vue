<template>
  <v-tooltip
    :bottom="bottom"
    :top="top"
    :left="left"
    :right="right"
  >
    <template #activator="{ on }">
      <v-btn
        :color="color"
        :disabled="disabled"
        :class="bclass"
        v-bind="$attrs"
        v-on="on"
        @click="raiseClickEvent()"
      >
        <v-icon
          :color="iconColor"
          :class="iclass"
          :size="iconSize"
        >
          {{ iconName }}
        </v-icon>
        {{ buttonText }}
      </v-btn>
      <!-- <span
        v-on="on"
      >
        <slot>
          <v-btn
            :color="color"
            :disabled="disabled"
            :class="bclass"
            v-bind="$attrs"
            @click="raiseClickEvent()"
          >
            <v-icon
              :color="iconColor"
              :class="iclass"
            >
              {{ iconName }}
            </v-icon>
            {{ buttonText }}
          </v-btn>
        </slot>
      </span> -->
    </template>
    <span> {{ tooltipText }} </span>
  </v-tooltip>
</template>

<script>
export default {
    name: "ButtonTip",
    props: {
        text: {
            type: [String, Object],
            required: false,
            default() {
                return '';
            },
        },
        iconName: {
            type: String,
            required: false,
            default() {
                return '';
            },
        },
        color: {
            type: String,
            required: false,
            default() {
                return 'primary';
            },
        },
        iconColor: {
            type: String,
            required: false,
            default() {
                return 'white';
            },
        },
        iclass: {
            type: String,
            required: false,
            default() {
                return 'mr-2';
            }
        },
        tooltip: {
            type: [String, Object],
            required: false,
            default() {
                return '';
            },
        },
        eventname: {
            type: String,
            required: false,
            default() {
                return 'click';
            },
        },
        bottom: {
            type: Boolean,
            required: false,
            default() {
                return false;
            },
        },
        top: {
            type: Boolean,
            required: false,
            default() {
                return false;
            },
        },
        left: {
            type: Boolean,
            required: false,
            default() {
                return false;
            },
        },
        right: {
            type: Boolean,
            required: false,
            default() {
                return false;
            },
        },
        bclass: {
            type: String,
            required: false,
            default() {
                return '';
            }
        },
        disabled: {
            type: Boolean,
            required: false,
            default() {
                return false;
            }
        },
        lodFinalized: {
          type: Boolean,
            default() {
                return false;
            }
        },
        iconSize: {
            type: String,
            required: false,
            default() {
                return '';
            }
        },
    },
    computed: {
        tooltipText() {
          if(this.disabled && this.lodFinalized) {
            return this.$t('student.signedLodEditTooltip');
          } else {
            return typeof(this.tooltip) === 'object' ?
                this.$t(this.tooltip.key, [this.tooltip.value]) : this.$t(this.tooltip);
          }
        },
        buttonText() {
            return typeof(this.text) === 'object' ?
                this.$t(this.text.key, [this.text.value]) : this.$t(this.text);
        }
    },
    methods: {
        raiseClickEvent() {
            this.$emit(this.eventname, this.$t(this.text));
        },
    },
};
</script>

<style scoped>
.v-btn-toggle {
  border-radius: 0;
}
</style>

