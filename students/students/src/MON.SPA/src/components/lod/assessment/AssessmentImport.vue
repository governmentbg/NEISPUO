<template>
  <v-card>
    <v-card-title>
      {{ $t('lod.assessments.importTitle') }}
    </v-card-title>
    <v-card-subtitle>
      <v-alert
        border="bottom"
        colored-border
        type="info"
        elevation="2"
      >
        {{ $t('lod.assessments.importInfo') }}
      </v-alert>
    </v-card-subtitle>
    <v-card-text>
      <v-form
        :ref="'assessmentImportForm_' + _uid"
      >
        <v-row dense>
          <v-col>
            <v-file-input
              ref="file"
              v-model="file"
              label="Файл"
              accept=".csv,text/plain"
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
      <v-btn
        v-if="importValidationModel"
        :color="importValidationModel.hasErrors === true ? 'error' : 'success'"
        outlined
        @click="validationDialog = true"
      >
        <v-icon
          v-if="importValidationModel.hasErrors === true "
          large
          class="mr-2"
        >
          mdi-alert-circle
        </v-icon>
        <v-icon
          v-else
          large
          class="mr-2"
        >
          mdi-check-circle
        </v-icon>
        Виж детайли от валидацията
      </v-btn>
      <v-spacer />
      <button-group>
        <button-tip
          icon-name="mdi-file-check-outline"
          tooltip="buttons.validate"
          text="buttons.validate"
          color="primary"
          small
          :disabled="!file"
          @click="validateFile()"
        />
        <button-tip
          icon-name="mdi-file-import"
          tooltip="buttons.import"
          text="buttons.import"
          color="primary"
          small
          :disabled="!file"
          @click="submitFile()"
        />
      </button-group>
    </v-card-actions>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
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
    <v-dialog
      v-model="validationDialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        :color="importValidationModel && importValidationModel.hasErrors === true ? 'red' : 'success'"
        outlined
      >
        <v-btn
          icon
          dark
          @click="validationDialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-toolbar-title class="white--text">
          Детайли от валидацията
        </v-toolbar-title>
        <v-spacer />
      </v-toolbar>
      <lod-assessment-import-validation-details
        v-if="importValidationModel"
        :value="importValidationModel"
      />
    </v-dialog>
  </v-card>
</template>

<script>
import ApiErrorDetails from '@/components/admin/ApiErrorDetails.vue';
import LodAssessmentImportValidationDetails from '@/components/lod/assessment/LodAssessmentImportValidationDetails.vue';
import { mapGetters } from 'vuex';


export default {
  name: 'AssessmentImport',
  components: {
    ApiErrorDetails,
    LodAssessmentImportValidationDetails
  },
  data() {
    return {
      file: null,
      saving: false,
      validationError: null,
      importValidationModel: null,
      dialog: false,
      validationDialog: false,
      maxFileSize: 15
    };
  },
  computed: {
    ...mapGetters(["userDetails"]),
  },
  methods: {
    submitFile() {
      this.validationError = '';

      let formData = new FormData();
      formData.append("file", this.file);

      this.saving = true;

      this.$api.lodAssessment.import(formData)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"));
          this.$emit("submitFile");
        })
        .catch((error) => {
          const { message, errors } = this.$helper.parseError(error.response);
          this.validationError = { date: new Date(), ...error.response.data };
          this.$helper.logError(
            { action: "DiplomasImport", message: message },
            errors,
            this.userDetails
          );
        })
        .finally(() => {
          this.saving = false;
        });
    },
    validateFile() {
      this.importValidationModel = null;
      let formData = new FormData();
      formData.append("file", this.file);

      this.saving = true;

      this.$api.lodAssessment.validateImport(formData)
        .then((response) => {
          this.importValidationModel = response.data;
        })
        .catch((error) => {
          console.log(error.response);
        })
        .finally(() => {
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
