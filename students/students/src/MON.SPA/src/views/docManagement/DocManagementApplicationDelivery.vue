<template>
  <div
    v-if="loading"
  >
    <v-progress-linear
      indeterminate
      color="primary"
    />
  </div>
  <div v-else>
    <v-card
      v-if="model?.campaign !== null"
      class="mb-3"
    >
      <v-card-text>
        <doc-management-campaign-form
          :value="model.campaign"
          disabled
        />
      </v-card-text>
    </v-card>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('docManagement.application.deliveryReportTitle') }}</h3>
      </template>
      <template #default>
        <doc-management-delivery-report-form
          v-if="model !== null"
          :ref="'docManagementDeliveryReportForm' + _uid"
          :value="model"
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
import DocManagementCampaignForm from "@/components/docManagement/DocManagementCampaignForm";
import DocManagementDeliveryReportForm from "@/components/docManagement/DocManagementDeliveryReportForm.vue";
import { DocManagementApplicationModel } from "@/models/docManagement/docManagementApplicationModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'DocManagementApplicationDelivery',
  components: {
    DocManagementCampaignForm,
    DocManagementDeliveryReportForm
  },
  props: {
    id: {
      type: Number,
      required: true
    },
  },
  data()
  {
    return {
      loading: true,
      saving: false,
      model: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    disabled() {
      return this.saving;
    }
  },
   mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.docManagementApplication.getById(this.id)
        .then(response => {
          if (response.data) {
            this.model = new DocManagementApplicationModel(response.data, this.$moment);
          }
        })
        .catch(() => {
          this.$notifier.error('', this.$t('common.loadError'));
        })
        .finally(() => {
          this.loading = false;
        });
    },
    onSave() {
        const form = this.$refs['docManagementDeliveryReportForm' + this._uid];
        const isValid = form.validate();

        if(!isValid) {
            return this.$notifier.error('', this.$t('validation.hasErrors'));
        }

        this.saving = true;

        this.$api.docManagementApplication.reportDelivery(this.model)
        .then(() => {
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
            this.$router.go(-1);
        })
        .catch((error) => {
           const {message, errors} = this.$helper.parseError(error.response);
           this.$notifier.modalError(message, errors);
        })
        .finally(() => { this.saving = false; });
    },
    onCancel() {
        this.$router.go(-1);
    },
  }
};
</script>
