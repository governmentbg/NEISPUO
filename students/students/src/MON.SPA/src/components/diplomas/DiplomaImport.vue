<template>
  <v-card>
    <v-card-title>
      {{ $t("diplomas.importTitle") }}
    </v-card-title>
    <v-card-subtitle>
      <v-alert
        border="bottom"
        colored-border
        type="info"
        elevation="2"
      >
        {{ $t('diplomas.externalImport', { fileSize: maxFileSize}) }}
      </v-alert>
    </v-card-subtitle>
    <v-card-text>
      <v-form
        :ref="'diplomaImportForm_' + _uid"
      >
        <v-row dense>
          <v-col>
            <v-file-input
              ref="file"
              v-model="file"
              label="Файл"
              accept="zip,application/octet-stream,application/zip,application/x-zip,application/x-zip-compressed"
              show-size
              truncate-length="50"
              :rules="[$validator.maxFileSize(maxFileSize)]"
            />
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-btn
        v-if="validationError"
        color="red"
        outlined
        @click="dialog = true"
      >
        <v-icon
          large
          class="mr-2"
        >
          mdi-alert-circle-outline
        </v-icon>
        Виж детайли на последната грешка
      </v-btn>
      <v-spacer />
      <button-tip
        ref="submit"
        icon-name="fa-file-import"
        tooltip="absence.fileImportText"
        text="buttons.import"
        raised
        color="primary"
        type="submit"
        :disabled="!file"
        @click="submitFile()"
      />
    </v-card-actions>
    <v-overlay :value="saving">
      <v-container class="text-center">
        <v-progress-circular
          indeterminate
          size="64"
        />
        <div class="mt-4 white--text text-h5 font-weight-bold pulsate">
          Моля, изчакайте!
        </div>
        <div class="mt-2 white--text text-subtitle-1">
          Импортът може да отнеме няколко минути!
        </div>
      </v-container>
    </v-overlay>
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        color="red"
        outlined
      >
        <v-btn
          icon
          dark
          @click="dialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-spacer />
        <v-toolbar-items>
          <button-tip
            icon
            icon-name="fa-copy"
            tooltip="buttons.copy"
            bottom
            iclass=""
            small
            @click="copyError()"
          />
        </v-toolbar-items>
      </v-toolbar>

      <api-error-details :value="validationError" />
    </v-dialog>
  </v-card>
</template>

<script>
import ApiErrorDetails from "@/components/admin/ApiErrorDetails.vue";
import { mapGetters } from "vuex";

export default {
  name: "DiplomaImport",
  components: {
    ApiErrorDetails,
  },
  data() {
    return {
      file: null,
      saving: false,
      validationError: null,
      dialog: false,
      maxFileSize: 50
    };
  },
  computed: {
    ...mapGetters(["userDetails"]),
  },
  methods: {
    submitFile() {
      this.validationError = "";

      const form = this.$refs[`diplomaImportForm_${this._uid}`];
      const isValid = form ? form.validate() : false;

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      let formData = new FormData();
      formData.append("file", this.file);

      this.saving = true;
      this.validationError = "";

      this.$api.diploma
        .import(formData)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"));
          this.$emit("submitFile");
        })
        .catch((error) => {
          const { message, errors } = this.$helper.parseError(error.response);
          this.validationError = { date: new Date(), ...error.response.data };
          this.$notifier.modalError(message, errors);
          this.$helper.logError(
            { action: "DiplomasImport", message: message },
            errors,
            this.userDetails
          );
        })
        .then(() => {
          this.$notifier.modalSuccess(this.$t("diplomas.importSuccess"), this.$t("diplomas.validateAfterImport"));
          this.saving = false;
        });
    },
    copyError() {
      let vm = this;
      navigator.clipboard.writeText(JSON.stringify(this.validationError)).then(
        function () {
          vm.$notifier.success("", vm.$t("common.clipboardSuccess"));
        },
        function () {
          vm.$notifier.error("", vm.$t("common.clipboardError"));
        }
      );
    },
  },
};
</script>

<style scoped>
.pulsate {
  animation: pulsate 1.5s ease-out infinite;
  opacity: 0.8;
}

@keyframes pulsate {
  0% {
    opacity: 0.8;
  }
  50% {
    opacity: 1;
    transform: scale(1.05);
  }
  100% {
    opacity: 0.8;
  }
}
</style>
