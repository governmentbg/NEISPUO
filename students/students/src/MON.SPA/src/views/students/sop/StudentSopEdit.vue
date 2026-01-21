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
          <h3>{{ $t("sop.editTitle") }}</h3>
        </template>

        <template #default>
          <sop-form
            v-if="model !== null"
            :ref="'sopForm' + _uid"
            v-model="model"
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
import SopForm from "@/components/students/SopForm.vue";
import { StudentSopModel } from "@/models/studentSopModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "StudentSopEdit",
  components: {
    SopForm,
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
    sopId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      model: null,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission"]),
  },
  mounted() {
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentSopManage)) {
      return this.$router.push("/errors/AccessDenied");
    }
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentSOP
        .getById(this.sopId)
        .then((response) => {
          if (response.data) {
            this.model = new StudentSopModel(response.data);
          }
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            this.$t("documents.studentSopLoadErrorMessage", 5000)
          );
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onSave() {
      const form = this.$refs["sopForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.saving = true;
      this.model.id = this.sopId;

      this.$api.studentSOP
        .update(this.model)
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
