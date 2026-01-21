<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <form-layout
      v-else
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t("refugee.editTitle") }} № {{ appId }}</h3>
        <v-spacer />
        <h3
          v-if="userDetails && userDetails.region"
        >
          РУО {{ userDetails.region }}
        </h3>
      </template>

      <template #default>
        <refugee-application-form
          v-if="document"
          :ref="'refugeeApplicationForm' + _uid"
          :document="document"
          :disabled="saving"
          @onCompleteApplicationChild="completeApplicationChild"
          @onCancelApplicationChild="cancelApplicationChild"
          @onUnlockApplicationChild="unlockApplicationChild"
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
import RefugeeApplicationForm from "@/components/refugee/RefugeeApplicationForm.vue";
import { RefugeeApplicationModel } from "@/models/refugee/refugeeApplicationModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'RefugeeApplicationEditView',
  components: {
    RefugeeApplicationForm,
  },
  props: {
    appId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userDetails']),
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForRefugeeApplicationsManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.refugee.getById(this.appId)
      .then(response => {
        if(response.data) {
         if(response.data) {
          this.document = new RefugeeApplicationModel(response.data, this.$moment);
        }
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    onSave() {
      const form = this.$refs["refugeeApplicationForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
      }

      this.document.applicationDate = this.$helper.parseDateToIso(this.document.applicationDate, '');
      this.document.children.forEach(element => element.birthDate = this.$helper.parseDateToIso(element.birthDate, ''));
      this.document.children.forEach(element => { if (element.ruoDocDate) element.ruoDocDate = this.$helper.parseDateToIso(element.ruoDocDate,'');});

      this.saving = true;
      this.$api.refugee
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
    async completeApplicationChild(id) {
      this.saving = true;
      this.$api.refugee.completeApplicationChild(id)
        .then(() => {
          this.$notifier.success('', this.$t("common.saveSuccess"));
          this.$router.go();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t("common.saveError"));
          console.error(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    async unlockApplicationChild(id) {
      this.saving = true;
      this.$api.refugee.unlockApplicationChild(id)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"));
          this.$router.go();
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("common.saveError"));
          console.error(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    async cancelApplicationChild(model) {
      this.saving = true;
      this.$api.refugee.cancelApplicationChild(model)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.$router.go();
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("common.saveError"), 5000);
          console.error(error.response);
        })
        .finally(() => {
          this.cancellationReason = null;
          this.saving = false;
        });
    },
  }
};
</script>
