<template>
  <div>
    <v-dialog
      v-model="dialog"
      width="600px"
      persistent
    >
      <form-layout
        @on-save="save"
        @on-cancel="close"
      >
        <template
          v-if="title"
          #title
        >
          {{ title }}
        </template>
        <template #default>
          <v-row>
            <v-col cols="12">
              <v-textarea
                v-model="model.description"
                :label="$t('docManagement.additionalCampaign.headers.description')"
                persistent-placeholder
                outlined
                rows="4"
                :disabled="saving"
              />
            </v-col>
          </v-row>
        </template>
      </form-layout>
    </v-dialog>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import { DocManagementApplicationReturnForCorrectionModel } from '@/models/docManagement/docManagementApplicationModel.js';

export default {
  name: 'DocManagementReturnForCorrectionDialog',
  data() {
    return {
      dialog: false,
      saving: false,
      model: null,
      title: null,
      dialogModel: 'returnForCorrection',
    };
  },
  methods: {
    open(applicationId, parentId, isExchangeRequest = false, dialogModel = 'returnForCorrection', title = null) {
      this.model = new DocManagementApplicationReturnForCorrectionModel({
        applicationId: applicationId,
        parentId: parentId,
        isExchangeRequest: isExchangeRequest
      });
      this.dialogModel = dialogModel;
      this.title = title;
      this.dialog = true;
    },
    close() {
      this.dialog = false;
      this.model = null;
    },
    save() {
      this.saving = true;

      let apiPromise = null;
      switch(this.dialogModel) {
        case 'returnForCorrection':
          apiPromise = this.$api.docManagementApplication.returnForCorrection(this.model);
          break;
        case 'actionResponse':
          apiPromise = this.$api.docManagementApplication.actionResponse(this.model);
          break;
        case 'approve':
          apiPromise = this.model.isExchangeRequest ?
            this.$api.docManagementExchange.approveExchangeRequest(this.model) :
            this.$api.docManagementApplication.approve(this.model);
          break;
        case 'reject':
          apiPromise = this.model.isExchangeRequest ?
            this.$api.docManagementExchange.rejectExchangeRequest(this.model) :
            this.$api.docManagementApplication.reject(this.model);
          break;
        default:
          console.error('Unknown mode:', this.dialogModel);
          this.saving = false;
          return;
      }

      if(!apiPromise) {
        this.$notifier.error('', this.$t('common.saveError'), 5000);
        this.saving = false;
        return;
      }

      apiPromise
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.$emit('saved');
          this.close();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'), 5000);
          console.error('API Error:', error);
        })
        .finally(() => {
          this.saving = false;
        });
    }
  }
};
</script>
