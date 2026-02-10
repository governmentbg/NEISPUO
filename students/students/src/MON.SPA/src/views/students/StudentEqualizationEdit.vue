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
          <h3>{{ $t("equalization.editTitle") }}</h3>
        </template>

        <template #default>
          <equalization-form
            v-if="document !== null"
            :ref="'equalizationForm' + _uid"
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

import EqualizationForm from "@/components/students/EqualizationForm.vue";
import { EqualizationModel } from "@/models/equalizationModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import Constants from "@/common/constants.js";

export default {
  name: "StudentEqualizationEdit",
  components: {
    EqualizationForm,
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
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentEqualizationManage)) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.equalization
        .getById(this.docId)
        .then((response) => {
          if (response.data) {
            this.document = new EqualizationModel(response.data);
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
        const form = this.$refs["equalizationForm" + this._uid];
        const isValid = form.validate();

        if (!isValid) {
          return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
        }

      this.saving = true;

      if (this.document.reasonId !== Constants.EqualizationEnableClassReasonId) {
        // InClassId се въвежда само при избрана причина "преместване на ученика от VIII до XII клас вкл. (чл. 32, ал. 1, т. 1 от Наредба №11"
        this.document.InClass = null;
      }

      this.$api.equalization
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
