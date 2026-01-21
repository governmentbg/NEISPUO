<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('docManagement.campaign.addTitle') }}</h3>
      </template>
      <template #default>
        <doc-management-campaign-form
          v-if="model !== null"
          :ref="'docManagementCampaignCreateForm' + _uid"
          :value="model"
          :disabled="disabled"
          :min-year="currentSchoolYear"
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
import { DocManagementCampaignModel } from "@/models/docManagement/docManagementCampaignModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'DocManagementCampaignCreate',
  components: {
    DocManagementCampaignForm,
  },
  data()
  {
    return {
      saving: false,
      model: new DocManagementCampaignModel(),
      currentSchoolYear: undefined
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForDocManagementCampaignManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.currentSchoolYear = (await this.$api.institution.getCurrentYear()).data;
  },
  methods: {
    onSave() {
      const form = this.$refs['docManagementCampaignCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.model.fromDate = this.$helper.parseDateToIso(this.model.fromDate, '');
      this.model.toDate = this.$helper.parseDateToIso(this.model.toDate, '');

      this.saving = true;

      this.$api.docManagementCampaign.create(this.model)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("common.saveError"), 5000);
        console.log(error.response);
      })
      .finally(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
