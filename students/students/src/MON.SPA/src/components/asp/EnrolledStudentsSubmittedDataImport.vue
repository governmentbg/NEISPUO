<template>
  <v-card>
    <v-card-title>
      {{ $t('asp.importSubmittedData') }}
    </v-card-title>
    <v-card-text>
      <v-row dense>
        <v-col>
          <v-file-input
            ref="file"
            v-model="file"
            label="Файл"
            show-size
            truncate-length="50"
          />
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <button-tip
        ref="submit"
        icon-name="fa-file-import"
        tooltip="absence.fileImportText"
        text="buttons.import"
        raised
        color="primary"
        type="submit"
        left
        :disabled="!file"
        @click="submitFile()"
      />
    </v-card-actions>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>


<script>
import { mapGetters } from "vuex";

export default {
  name: "EnrolledStudentsSubmittedDataImport",
  data() {
    return {
      file: null,
      saving: false,
    };
  },
  computed: {
    ...mapGetters(['userDetails'])
  },
  methods: {
    submitFile() {
      let formData = new FormData();
      formData.append("file", this.file);

      this.saving = true;
      this.$api.asp
        .uploadSubmittedData(formData)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.$emit('submitFile');
        })
        .catch((error) => {
          const {message, errors} = this.$helper.parseError(error.response);
          this.$notifier.modalError(message, errors);
          this.$helper.logError({ action: 'SubmittedDataFileUpload', message: message}, errors, this.userDetails);
        })
        .then(() => {
          this.saving = false;
        });

    }
  }
};
</script>
