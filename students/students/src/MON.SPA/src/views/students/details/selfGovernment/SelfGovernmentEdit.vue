<template>
  <div v-if="loading">
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div v-else>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("lod.selfGovernment.editTitle") }}</h3>
      </template>
      <template #default>
        <self-government-form
          v-if="form !== null"
          :ref="'selfGovernmentForm' + _uid"
          :value="form"
          :disabled="disabled"
        />
      </template>
    </form-layout>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import SelfGovernmentForm from "@/components/tabs/selfGovernment/SelfGovernmentForm.vue";
import { StudentSelfGovernmentModel } from "@/models/studentSelfGovernmentModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "SelfGovernmentEdit",
  components: {
    SelfGovernmentForm,
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
    selfGovernmentdId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      form: null,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission"]),
    disabled() {
      return this.saving;
    },
  },
  mounted() {
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentSelfGovernmentManage)) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.selfGovernment
        .getById(this.selfGovernmentdId)
        .then((response) => {
          if (response.data) {
            this.form = new StudentSelfGovernmentModel(response.data);
          }
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            this.$t("errors.studentInternationalMobilityLoad")
          );
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onSave() {
      const form = this.$refs["selfGovernmentForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"));
      }

      this.saving = true;
      this.$api.selfGovernment
        .update(this.form)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.$router.go(-1);
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("lod.selfGovernment.updateError"));
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
