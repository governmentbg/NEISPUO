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
          <h3>{{ $t("personalDevelopment.editTitle") }}</h3>
        </template>

        <template #default>
          <personal-development-support-form
            v-if="model !== null"
            :ref="'personalDevelopmentSupportForm' + _uid"
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
import PersonalDevelopmentSupportForm from "@/components/students/PersonalDevelopmentSupportForm.vue";
import { PersonalDevelopmentSupportModel } from "@/models/personalDevelopmentSupportModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "StudentPersonalDevelopmentEdit",
  components: {
    PersonalDevelopmentSupportForm,
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
    pdId: {
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
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentPDS
        .getById(this.pdId)
        .then((response) => {
          if (response.data) {
            this.model = new PersonalDevelopmentSupportModel(response.data);
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
      const form = this.$refs["personalDevelopmentSupportForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.saving = true;
      this.$api.studentPDS
        .update(this.model)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          const { message } = this.$helper.parseError(error.response);
          this.$notifier.error(this.$t("common.save"), message);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    },
  },
};
</script>
