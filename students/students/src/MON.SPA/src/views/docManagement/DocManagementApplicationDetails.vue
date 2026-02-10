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
      v-if="model && model.campaign !== null && !model.campaign.isHidden"
      class="mb-3"
    >
      <v-card-text>
        <doc-management-campaign-form
          :value="model.campaign"
          disabled
        />
      </v-card-text>
    </v-card>
    <form-layout class="mb-3">
      <template #title>
        <v-row
          v-if="model"
          align="center"
          no-gutters
        >
          <v-chip
            v-if="model.statusName"
            :color="['Approved', 'Submitted'].includes(model.status) ? 'success' : ['Rejected', 'ReturnedForCorrection'].includes(model.status) ? 'warning' : 'light'"
            small
            label
            class="mr-3"
          >
            {{ model.statusName }}
          </v-chip>
          <v-col>
            <h4 v-if="model.isExchangeRequest">
              {{ $t('docManagement.exchangeRequest.reviewTitle') }}
            </h4>
            <h4 v-else>
              {{ $t('docManagement.application.reviewTitle') }}
            </h4>
          </v-col>
          <v-col
            v-if="!hideTimeline"
            class="text-right"
            cols="1"
          >
            <doc-management-application-time-line
              ref="docManagementApplicationTimeLine"
              :application-id="model.id"
              @actionResponse="onApproveRejectSaved"
            >
              <template #title>
                <v-btn
                  v-if="model.hasEvaluationPermission"
                  color="primary"
                  text
                  small
                  @click="onReturnForCorrection"
                >
                  <v-icon left>
                    mdi-undo
                  </v-icon>
                  {{ $t('buttons.returnedForCorrection') }}
                </v-btn>
                <v-btn
                  v-if="model.hasApprovePermission"
                  color="success"
                  text
                  small
                  @click="onApprove"
                >
                  <v-icon left>
                    mdi-check
                  </v-icon>
                  {{ $t('buttons.approve') }}
                </v-btn>
                <v-btn
                  v-if="model.hasApprovePermission"
                  color="error"
                  text
                  small
                  @click="onReject"
                >
                  <v-icon left>
                    mdi-file-cancel
                  </v-icon>
                  {{ $t('buttons.reject') }}
                </v-btn>
              </template>
            </doc-management-application-time-line>
          </v-col>
        </v-row>
      </template>
      <template #default>
        <doc-management-application-form
          v-if="model !== null"
          :value="model"
          disabled
        />
      </template>
      <template #actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>

    <doc-management-return-for-correction-dialog
      ref="returnForCorrectionDialog"
      @saved="onReturnForCorrectionSaved"
    />
    <doc-management-return-for-correction-dialog
      ref="approveDialog"
      @saved="onApproveRejectSaved"
    />
    <doc-management-return-for-correction-dialog
      ref="rejectDialog"
      @saved="onApproveRejectSaved"
    />
  </div>
</template>

<script>
import DocManagementCampaignForm from "@/components/docManagement/DocManagementCampaignForm";
import DocManagementApplicationForm from "@/components/docManagement/DocManagementApplicationForm.vue";
import DocManagementApplicationTimeLine from '@/components/docManagement/DocManagementApplicationTimeLine.vue';
import DocManagementReturnForCorrectionDialog from '@/components/docManagement/DocManagementReturnForCorrectionDialog.vue';
import { DocManagementApplicationModel } from "@/models/docManagement/docManagementApplicationModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'DocManagementApplicationCreate',
  components: {
    DocManagementCampaignForm,
    DocManagementApplicationForm,
    DocManagementApplicationTimeLine,
    DocManagementReturnForCorrectionDialog
  },
  props: {
    id: {
      type: Number,
      required: true
    },
    hideTimeline: {
      type: Boolean,
      default: false
    }
  },
  data()
  {
    return {
      loading: false,
      model: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
  },
   mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead)) {
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
    onReturnForCorrection() {
      this.$refs.returnForCorrectionDialog.open(this.model.id, null, this.model.isExchangeRequest, 'returnForCorrection', this.$t('docManagement.application.returnForCorrectionTitle'));
    },
    onReturnForCorrectionSaved() {
      const timeline = this.$refs.docManagementApplicationTimeLine;
      if(timeline && typeof timeline.get === 'function') {
        timeline.get(); // Refresh the timeline data
      }
    },
    onApprove() {
      this.$refs.approveDialog.open(this.model.id, null, this.model.isExchangeRequest, 'approve', this.$t('buttons.approve'));
    },
    onReject() {
      this.$refs.rejectDialog.open(this.model.id, null, this.model.isExchangeRequest, 'reject', this.$t('buttons.reject'));
    },
    onApproveRejectSaved() {
      // const timeline = this.$refs.docManagementApplicationTimeLine;
      // if(timeline && typeof timeline.get === 'function') {
      //   timeline.get(); // Refresh the timeline data
      // }

      this.load(); // Reload the application details
    },
  }
};
</script>
