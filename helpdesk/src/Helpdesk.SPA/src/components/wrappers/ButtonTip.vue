<template>
  <v-tooltip
    :bottom="bottom"
    :top="top"
    :left="left"
    :right="right"
  >
    <template v-slot:activator="{ on }">
      <slot>
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
          >
            {{ iconName }}
          </v-icon>
          {{ $t(text) }}
        </v-btn>
      </slot>
    </template>
    <span> {{ $t(tooltip) }} </span>
  </v-tooltip>
</template>

<script>
export default {
    name: "ButtonTip",
    props: {
        text: {
            type: String,
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
            type: String,
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
        }
    },
    methods: {
        raiseClickEvent() {
            this.$emit(this.eventname, this.$t(this.text));
        },
    },
};
</script>
