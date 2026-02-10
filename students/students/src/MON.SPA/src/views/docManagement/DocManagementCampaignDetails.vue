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
    <form-layout class="mb-3">
      <template #title>
        <h3>{{ $t('docManagement.campaign.reviewTitle') }}</h3>
      </template>
      <template #default>
        <doc-management-campaign-form
          v-if="model !== null"
          :value="model"
          disabled
        />
      </template>
      <template #actions>
        <v-btn
          v-if="model && model.hasRuoAttachmentPermision"
          raised
          color="primary"
          class="mr-2"
          @click="downloadAll"
          :loading="downloading"
          :disabled="downloading"
          small
        >
          <v-icon left>
            fas fa-download
          </v-icon>
          {{ $t('docManagement.campaign.downloadAllLinkAttachments') }}
        </v-btn>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
          small
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>
  </div>
</template>

<script>
import DocManagementCampaignForm from "@/components/docManagement/DocManagementCampaignForm";
import { DocManagementCampaignModel } from "@/models/docManagement/docManagementCampaignModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";


export default {
  name: 'AbsenceCampaignDetails',
  components: {
    DocManagementCampaignForm
  },
  props: {
    id: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: false,
      downloading: false,
      model: null,
      aspAskingSession: undefined
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
  },
  mounted() {
    if (!this.hasPermission(Permissions.PermissionNameForDocManagementCampaignRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

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
        .finally(() => {
          this.loading = false;
        });
    },
    backClick() {
      this.$router.go(-1);
    },
    downloadAll() {
      this.downloading = true;
      this.$api.docManagementCampaign.downloadAllAttachments(this.id)
        .then(response => {
          const url = window.URL.createObjectURL(new Blob([response.data]));
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', `Campaign_${this.id}_Attachments.zip`);
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        })
        .catch(error => {
          this.$notifier.error('', this.$t('common.downloadError') || 'Error downloading files');
          console.log(error);
        })
        .finally(() => {
          this.downloading = false;
        });
    }
  }
};
</script>
