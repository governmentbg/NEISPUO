<template>
  <div>
    <grid
      v-if="hasReadPermission && parentId"
      :ref="'docManagementAdditionalCampaignsGrid' + _uid"
      url="/api/docManagementAdditionalCampaign/list"
      file-export-name="Списък с допълнителни кампании за управление на документи с фабрична номерация"
      :headers="headers"
      title=""
      :item-class="itemRowBackground"
      :filter="{
        campaignId: parentId
      }"
      flat
      tile
    >
      <template v-slot:[`item.isManuallyActivated`]="props">
        <v-chip
          :color="props.item.isManuallyActivated === true ? 'success' : 'light'"
          small
          label
        >
          <yes-no
            :value="props.item.isManuallyActivated"
          />
        </v-chip>
      </template>

      <template v-slot:[`item.fromDate`]="props">
        <del v-if="props.item.isManuallyActivated">
          {{ props.item.fromDate ? $moment(props.item.fromDate).format(dateFormat) : "" }}
        </del>
        <span v-else>
          {{ props.item.fromDate ? $moment(props.item.fromDate).format(dateFormat) : "" }}
        </span>
      </template>

      <template v-slot:[`item.toDate`]="props">
        <del v-if="props.item.isManuallyActivated">
          {{ props.item.toDate ? $moment(props.item.toDate).format(dateFormat) : "" }}
        </del>
        <span v-else>
          {{ props.item.toDate ? $moment(props.item.toDate).format(dateFormat) : "" }}
        </span>
      </template>

      <template
        v-slot:[`item.labels`]="props"
      >
        <div v-if="props.item.labels && props.item.labels.length > 0">
          <v-chip
            v-for="(label, index) in props.item.labels"
            :key="index"
            :color="label.value"
            small
            class="mr-1"
            label
          >
            {{ label.key }}
          </v-chip>
        </div>
      </template>

      <template #actions="{ item: campaign }">
        <button-group>
          <button-tip
            v-if="hasApplicationManagePermission && campaign.isActive"
            icon
            icon-name="mdi-application-edit"
            icon-color="primary"
            tooltip="docManagement.application.newApplication"
            bottom
            iclass=""
            small
            :disabled="campaign.hasApplication"
            :to="`/docManagement/application/create?campaignId=${campaign.id}`"
          />
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            @click="onReviewAdditional(campaign)"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            @click="onEditAdditional(campaign)"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            :icon-name="campaign.isManuallyActivated ? 'mdi-lock' : 'mdi-lock-open-variant'"
            icon-color="primary"
            :tooltip="campaign.isManuallyActivated ? 'docManagement.campaign.manuallyActivationOff' : 'docManagement.campaign.manuallyActivationOn'"
            bottom
            iclass=""
            small
            @click="toggleManuallyActivation(campaign)"
          />
          <button-tip
            v-if="hasManagePermission && campaign.isActive === false"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteCampaign(campaign)"
          />
          <button-tip
            v-if="hasReportCreatePermission"
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="docManagement.application.generateReportFile"
            bottom
            small
            @click="generateReportFile(campaign)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            v-if="hasManagePermission"
            small
            color="primary"
            @click="openAdditionalDialog"
          >
            {{ $t('buttons.newRecord') }}
          </v-btn>
        </button-group>
      </template>
    </grid>
    <v-dialog
      v-model="additionalDialog"
      persistent
    >
      <v-card>
        <v-card-title>
          {{ additionalDialogTitle }}
        </v-card-title>
        <v-card-text>
          <doc-management-additional-campaign-form
            v-if="additionalModel"
            :ref="'docManagementAdditionalCampaignForm' + _uid"
            :value="additionalModel"
            :disabled="saving || dialogMode === 'review'"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            v-if="dialogMode !== 'review'"
            raised
            color="primary"
            @click.stop="saveAdditional"
          >
            <v-icon left>
              fas fa-save
            </v-icon>
            {{ $t('buttons.save') }}
          </v-btn>
          <v-btn
            raised
            color="error"
            @click.stop="additionalDialog = false"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { DocManagementCampaignModel } from '@/models/docManagement/docManagementCampaignModel.js';
import Grid from '@/components/wrappers/grid.vue';
import Constants from '@/common/constants.js';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import DocManagementAdditionalCampaignForm from '@/components/docManagement/DocManagementAdditionalCampaignForm.vue';
import clonedeep from 'lodash.clonedeep';
import Helper from "@/components/helper.js";


export default {
  name: "DocManagementAdditionalCampaignsListComp",
  components: {
      Grid,
      DocManagementAdditionalCampaignForm,
    },
  props: {
    parentId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      // Additional Campaign dialog state
      additionalDialog: false,
      additionalModel: null,
      dialogMode: 'create',
      // Suppression flags to avoid redundant grid reloads when local changes already applied
      suppressAdditionalReload: {}, // keyed by parentId for additional campaigns
      headers: [
        { text: this.$t('docManagement.additionalCampaign.headers.institution'), value: 'institutionName' },
        { text: this.$t('docManagement.additionalCampaign.headers.name'), value: 'name' },
        { text: this.$t('docManagement.additionalCampaign.headers.fromDate'), value: 'fromDate' },
        { text: this.$t('docManagement.additionalCampaign.headers.toDate'), value: 'toDate' },
        { text: this.$t('docManagement.additionalCampaign.headers.isManuallyActivated'), value: 'isManuallyActivated' },
        { text: '', value: 'labels', filterable: false, sortable: false },
        { text: '', value: 'controls', filterable: false, sortable: false, align: 'end' },
      ],
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementCampaignRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementAdditionalCampaignManage);
    },
    hasApplicationManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage);
    },
    hasReportCreatePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementReportCreate);
    },
    additionalDialogTitle() {
      switch (this.dialogMode) {
        case 'review':
          return this.$t('docManagement.additionalCampaign.reviewTitle');
        case 'edit':
          return this.$t('docManagement.additionalCampaign.editTitle');
        default:
          return this.$t('docManagement.additionalCampaign.addTitle');
      }
    },
  },
  watch: {
    additionalDialog (val) {
      val || this.onCloseDialog();
    },
  },
  mounted() {
    if (!this.hasReadPermission){
      return this.$router.push("/errors/AccessDenied");
    }

    this.$studentHub.$on('doc-management-additional-campaign-modified', this.onDocManagementAdditionalCampaignModified);
  },
  destroyed() {
    this.$studentHub.$off('doc-management-additional-campaign-modified');
  },
  methods: {
    gridReload() {
      const grid = this.$refs['docManagementAdditionalCampaignsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    itemRowBackground(item) {
      return item.isActive ? 'custom-grid-row left border-success' : '';
    },
    onDocManagementAdditionalCampaignModified(id, parentId) {
      if (this.suppressAdditionalReload[parentId]) {
        return; // skip because we already refreshed locally
      }
      this.gridReload();
    },
    async generateReportFile(item) {
      await this.$api.docManagementCampaign.generateReport({ campaignId: item.id })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Заявка за ЗУД_${item.schoolYearName}.docx`;

          const url = window.URL.createObjectURL(new Blob([response.data]));
          const link = document.createElement("a");
          link.href = url;
          link.setAttribute("download", fileName);
          document.body.appendChild(link);
          link.click();
        });
    },
    openAdditionalDialog() {
      this.dialogMode = 'create';
      this.additionalModel = new DocManagementCampaignModel({ parentId: this.parentId });
      this.additionalDialog = true;
    },
    onReviewAdditional(item) {
      this.dialogMode = 'review';
      this.additionalModel = new DocManagementCampaignModel(clonedeep(item), this.$moment);
      this.additionalDialog = true;
    },
    onEditAdditional(item) {
      this.dialogMode = 'edit';
      this.additionalModel = new DocManagementCampaignModel(clonedeep(item), this.$moment);
      this.additionalDialog = true;
    },
    onCloseDialog() {
      this.$nextTick(() => {
        this.additionalModel = null;
      });
    },
    async saveAdditional() {
      const formRef = this.$refs['docManagementAdditionalCampaignForm' + this._uid];
      const isValid = formRef && typeof formRef.validate === 'function' ? formRef.validate() : true;
      if (!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      const payload = { ...this.additionalModel };
      payload.fromDate = this.$helper.parseDateToIso(payload.fromDate, '');
      payload.toDate = this.$helper.parseDateToIso(payload.toDate, '');

      this.saving = true;
      try {
        if (this.dialogMode === 'edit' && payload.id) {
          await this.$api.docManagementAdditionalCampaign.update(payload);
        } else {
          await this.$api.docManagementAdditionalCampaign.create(payload);
        }
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.additionalDialog = false;

        this.gridReload();
      } catch (error) {
        this.$notifier.error('', this.$t('common.saveError'), 5000);
        // eslint-disable-next-line no-console
      } finally {
        this.saving = false;
      }
    },
    toggleManuallyActivation(item) {
      this.saving = true;

      this.$api.docManagementAdditionalCampaign.toggleManuallyActivation(item)
      .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.gridReload();
      })
      .catch(() => {
          this.$notifier.error('',this.$t("common.saveError"), 5000);
      })
      .then(() => { this.saving = false; });
    },
    async deleteCampaign(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;

      this.$api.docManagementAdditionalCampaign.delete(item.id)
        .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.gridReload();
        })
        .catch(() => {
            this.$notifier.error('',this.$t("common.deleteError"), 5000);
        })
        .finally(() => { this.saving = false; });
      }
    },
  }
};
</script>
