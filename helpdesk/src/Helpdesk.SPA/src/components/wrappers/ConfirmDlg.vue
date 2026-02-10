<template>
  <v-dialog
    v-model="dialog"
    v-bind="$attrs"
    :max-width="options.width"
    @keydown.esc="cancel"
  >
    <v-card>
      <v-toolbar
        v-show="!!title"
        class="blue-grey lighten-5"
      >
        <v-toolbar-title class="text-h5">
          {{ title }}
        </v-toolbar-title>
      </v-toolbar>
      <v-card-subtitle
        v-show="!!message"
        class="pa-4 text-h6"
      >
        {{ message }}
      </v-card-subtitle>
      <v-card-text v-if="showConfirmText">
        <text-field
          v-model="confirmText"
          :label="confirmTextLabel"
          clearable
        />
      </v-card-text>
      <v-card-actions class="pt-3">
        <v-spacer />
        <v-btn
          v-if="!options.noconfirm"
          color="red darken-1"
          text
          class="body-2 font-weight-bold"
          @click.native="cancel"
        >
          {{ $t('buttons.cancel') }}
        </v-btn>
        <v-btn
          color="primary"
          class="body-2 font-weight-bold"
          outlined
          @click.native="agree"
        >
          {{ $t('buttons.ok') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
  export default {
    name: "ConfirmDlg",
    props: {
      showConfirmText: {
        type: Boolean,
        default: false
      },
      confirmTextLabel: {
        type: String,
        default: ''
      }
    },
    data() {
      return {
        dialog: false,
        resolve: null,
        reject: null,
        message: null,
        title: null,
        confirmText: null,
        options: {
          width: 500,
          noconfirm: false,
        },
      };
    },

    methods: {
      open(title, message, options) {
        this.dialog = true;
        this.title = title;
        this.message = message;
        this.options = Object.assign(this.options, options);
        this.confirmText = '';
        return new Promise((resolve, reject) => {
          this.resolve = resolve;
          this.reject = reject;
        });
      },
      agree() {
        this.resolve(true);
        this.dialog = false;
      },
      cancel() {
        this.resolve(false);
        this.dialog = false;
      },
    },
  };
</script>
