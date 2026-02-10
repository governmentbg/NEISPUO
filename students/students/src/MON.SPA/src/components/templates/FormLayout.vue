<template>
  <v-card>
    <v-card-title
      v-if="!hideTitle"
    >
      <slot name="title">
        Default title
      </slot>
    </v-card-title>

    <v-card-subtitle>
      <slot name="subtitle" />
    </v-card-subtitle>

    <v-card-text>
      <slot name="default" />
    </v-card-text>

    <v-card-actions
      v-if="!disabled"
    >
      <slot name="actions">
        <slot name="left-actions" />
        <v-spacer />
        <v-btn
          v-if="!hideSaveBtn"
          raised
          color="primary"
          @click.stop="$emit('on-save');"
        >
          <v-icon left>
            fas fa-save
          </v-icon>
          {{ $t('buttons.save') }}
        </v-btn>

        <v-btn
          raised
          color="error"
          @click.stop="cancelClick"
        >
          <v-icon left>
            fas fa-times
          </v-icon>
          {{ $t('buttons.cancel') }}
        </v-btn>
      </slot>
    </v-card-actions>

    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
export default {
  name: 'FormLayout',
  props: {
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    skipCancelPrompt: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideTitle: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideSaveBtn: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  methods: {
    async cancelClick() {
      if(this.skipCancelPrompt) return this.$emit('on-cancel');

      if(await this.$refs.confirm.open('', this.$t('common.confirm'))) {
        this.$emit('on-cancel');
      }
    }
  }
};
</script>
