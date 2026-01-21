<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        indeterminate
        color="primary"
      />
    </div>
    <v-card
      v-if="campaignModel !== null"
      class="mb-3"
    >
      <v-card-text>
        <doc-management-campaign-form
          :value="campaignModel"
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
        <h3>{{ $t('docManagement.application.addTitle') }}</h3>
      </template>
      <template #default>
        <doc-management-application-form
          v-if="model !== null"
          :ref="'docManagementApplicationCreateForm' + _uid"
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
import DocManagementApplicationForm from "@/components/docManagement/DocManagementApplicationForm.vue";
import { DocManagementCampaignModel } from "@/models/docManagement/docManagementCampaignModel.js";
import { DocManagementApplicationModel } from "@/models/docManagement/docManagementApplicationModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'DocManagementApplicationCreate',
  components: {
    DocManagementCampaignForm,
    DocManagementApplicationForm
  },
  props: {
    campaignId: {
      type: Number,
      required: true
    },
  },
  data()
  {
    return {
      loading: false,
      saving: false,
      campaignModel: null,
      model: null,
      currentSchoolYear: undefined
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userInstitutionId']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.loadCampaign();

    const basicDocuments = (await this.$api.docManagementApplication.getBasicDocuments()).data;

    this.model = new DocManagementApplicationModel({
      campaignId: this.campaignId,
      institutionId: this.userInstitutionId,
      basicDocuments: basicDocuments.map(x => {
        return {
          basicDocumentId: x.value,
          basicDocumentName: x.text,
          number: 0
        };
      })
    });
  },
  methods: {
    onSave() {
      const form = this.$refs['docManagementApplicationCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.saving = true;

      this.$api.docManagementApplication.create(this.model)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch(() => {
        this.$notifier.error('',this.$t("common.saveError"), 5000);
      })
      .finally(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
    loadCampaign() {
      this.loading = true;

      this.$api.docManagementCampaign.getById(this.campaignId)
        .then(response => {
          if (response.data) {
            this.campaignModel = new DocManagementCampaignModel(response.data, this.$moment);
          }
        })
        .finally(() => {
          this.loading = false;
        });
    },
  }
};
</script>
