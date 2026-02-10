<template>
  <div>
    <v-card dense>
      <v-card-title>
        <v-icon
          v-if="prependIcon"
          :color="prependIconColor"
          class="mr-3"
        >
          {{ prependIcon }}
        </v-icon>
        <v-chip
          outlined
          :color="['Approved', 'Submitted'].includes(value.status) ? 'success' : ['Response'].includes(value.status) ? 'primary' : ['Rejected', 'ReturnedForCorrection'].includes(value.status) ? 'warning' : 'light'"
          label
        >
          {{ value.statusName }}
        </v-chip>
      </v-card-title>
      <v-card-subtitle class="text-left">
        {{ $t('docManagement.application.cardSubtitle', { lastRevisionDate: $moment.utc(value.createDate ?? value.modifyDate).format(dateAndTimeFormat), lastEditorUsername: value.creator ?? value.updater}) }}
      </v-card-subtitle>
      <v-card-text v-if="value.description">
        <v-textarea
          :value="value.description"
          :label="$t('docManagement.additionalCampaign.headers.description')"
          outlined
          readonly
          rows="3"
          auto-grow
          hide-details
        />
      </v-card-text>
      <v-card-actions
        v-if="!hideControls && hasManagePermission && value.hasResponsePermission"
      >
        <v-spacer />
        <v-btn
          color="primary"
          raised
          small
          @click="onActionResponse"
        >
          {{ $t('buttons.response') }}
        </v-btn>
      </v-card-actions>
    </v-card>
    <div
      v-if="!hideResponses && (value?.responses?.length ?? 0 > 0)"
      class="py-2 pl-5"
    >
      <div
        v-for="(response, index) in value?.responses"
        :key="index"
        class="mb-2"
      >
        <doc-management-application-status
          :value="response"
          hide-controls
          hide-responses
          prepend-icon="mdi-keyboard-return"
          prepend-icon-color="primary"
        />
      </div>
    </div>
    <doc-management-return-for-correction-dialog
      ref="actionResponseDialog"
      @saved="onActionResponseSaved"
    />
  </div>
</template>

<script>
import Constants from '@/common/constants.js';
import DocManagementApplicationStatus from '@/components/docManagement/DocManagementApplicationStatus.vue';
import DocManagementReturnForCorrectionDialog from '@/components/docManagement/DocManagementReturnForCorrectionDialog.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'DocManagementApplicationStatus',
  components: {
    DocManagementApplicationStatus,
    DocManagementReturnForCorrectionDialog
  },
  props: {
    value: {
      type: Object,
      required: true
    },
    hideControls: {
      type: Boolean,
      default: false,
    },
    hideResponses: {
      type: Boolean,
      default: false,
    },
    prependIcon: {
      type: String,
      default: '',
    },
    prependIconColor: {
      type: String,
      default: 'primary',
    },
  },
  data() {
    return {
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage);
    }
  },
  methods:{
    onActionResponse() {
      this.$refs.actionResponseDialog.open(this.value.applicationId, this.value.id, this.value.isExchangeRequest, 'actionResponse', this.$t('docManagement.application.actionResponseTitle') );
    },
    onActionResponseSaved() {
       this.$emit('actionResponse');
    }
  }
};
</script>
