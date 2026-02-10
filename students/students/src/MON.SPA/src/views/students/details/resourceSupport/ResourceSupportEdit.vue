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
        <h3>{{ $t("resourceSupport.editTitle") }}</h3>
      </template>
      <template #default>
        <resource-support-form
          v-if="form !== null"
          :ref="'form' + _uid"
          :value="form"
          :disabled="disabled"
        />
      </template>
    </form-layout>
    <v-overlay
      :value="saving"
    >
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import ResourceSupportForm from "@/components/tabs/resourceSupport/ResourceSupportForm";
import { StudentResourceSupportModel } from "@/models/studentResourceSupportModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "ResourceSupportEdit",
  components: {
    ResourceSupportForm,
  },
  props: {
    personId: {
      type: Number,
      required: false,
      default: 0,
    },
    reportId: {
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
    ...mapGetters(['hasStudentPermission']),
    disabled() {
      return this.saving;
    },
  },
  mounted() {
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentResourceSupportManage
      )
    ) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.resourceSupport
        .getById(this.reportId)
        .then((response) => {
          if (response.data) {
            this.form = new StudentResourceSupportModel(
              response.data,
              this.$moment
            );
          }
        })
        .catch((error) => {
          console.log(error.response);
          this.$notifier.error('', this.$t('common.loadError'));
        })
        .then(() => {
          this.loading = false;
        });
    },
    onSave() {
      const form = this.$refs["form" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.reportDate = this.$helper.parseDateToIso(
        this.form.reportDate,
        ""
      );

      this.saving = true;
      this.$api.resourceSupport
        .update(this.form)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.$router.go(-1);
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'));
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
