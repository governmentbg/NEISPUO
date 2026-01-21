<template>
  <div
    v-if="loading"
  >
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div
    v-else
  >
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('docManagement.campaign.editTitle') }}</h3>
      </template>
      <template #default>
        <doc-management-campaign-form
          v-if="model !== null"
          :ref="'docManagementCampaignEditForm' + _uid"
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
    name: 'DocManagementCampaignEdit',
    components: {
       DocManagementCampaignForm
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
      this.load();
    },
    methods: {
        load() {
            this.loading = true;

            this.$api.docManagementCampaign.getById(this.id)
            .then(response => {
                if (response.data) {
                    this.model = new DocManagementCampaignModel(response.data, this.$moment);
                }
            })
            .catch(error => {
                this.$notifier.error('', this.$t('common.loadError'));
                console.log(error.response);
            })
           .finally(() => { this.loading = false; });
        },
        onSave() {
            const form = this.$refs['docManagementCampaignEditForm' + this._uid];
            const isValid = form.validate();

            if(!isValid) {
                return this.$notifier.error('', this.$t('validation.hasErrors'));
            }

            this.model.fromDate = this.$helper.parseDateToIso(this.model.fromDate, '');
            this.model.toDate = this.$helper.parseDateToIso(this.model.toDate, '');

            this.saving = true;

            this.$api.docManagementCampaign.update(this.model)
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
