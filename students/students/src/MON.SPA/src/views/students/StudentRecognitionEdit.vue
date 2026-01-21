<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t("recognition.editTitle") }}</h3>
        </template>

        <template #default>
          <recognition-form
            v-if="document !== null"
            :ref="'recognitionForm' + _uid"
            :document="document"
            :disabled="saving"
          />
        </template>
      </form-layout>
    </div>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import RecognitionForm from "@/components/students/RecognitionForm.vue";
import { RecognitionModel } from "@/models/recognitionModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "StudentRecognitionEdit",
  components: {
    RecognitionForm,
  },
  props: {
    pid: {
      type: Number,
      required: true,
    },
    docId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission"]),
  },
  mounted() {
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentRecognitionManage
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.recognition.getById(this.docId)
        .then((response) => {
          if (response.data) {
            this.document = new RecognitionModel(response.data, this.$moment);
            console.log(this.document);
          }
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            this.$t("documents.documentLoadErrorMessage", 5000)
          );
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onSave() {
      const form = this.$refs["recognitionForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.document.ruoDocumentDate = this.$helper.parseDateToIso(
        this.document.ruoDocumentDate,
        ""
      );
      this.document.diplomaDate = this.$helper.parseDateToIso(
        this.document.diplomaDate,
        ""
      );

      this.saving = true;
      this.$api.recognition
        .update(this.document)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(
            this.$t("common.save"),
            error?.response?.data?.message ?? this.$t("common.error"),
            7000
          );
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    },
  },
};
</script>
