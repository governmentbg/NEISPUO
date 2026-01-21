<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'docManagementCampaignsGrid' + _uid"
      url="/api/docManagementCampaign/list"
      file-export-name="Списък с кампании за управление на документи с фабрична номерация"
      :headers="headers"
      :title="$tc('docManagement.campaign.title', 2)"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :item-class="itemRowBackground"
      :expanded.sync="expanded"
      show-expand
      :single-expand="true"
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

      <template v-slot:expanded-item="{ item }">
        <td :colspan="headers.length + 1">
          <v-card
            flat
            tile
          >
            <v-card-text>
              <doc-management-additional-campaigns-list :parent-id="item.id" />
            </v-card-text>
          </v-card>
        </td>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasApplicationManagePermission && item.item.isActive"
            icon
            icon-name="mdi-application-edit"
            icon-color="primary"
            tooltip="docManagement.application.newApplication"
            bottom
            iclass=""
            small
            :disabled="item.item.hasApplication"
            :to="`/docManagement/application/create?campaignId=${item.item.id}`"
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
            :to="`/docManagement/campaign/${item.item.id}/details`"
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
            :to="`/docManagement/campaign/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasReadPermission && !item.item.hasRuoAttachmentPermission"
            icon
            icon-name="mdi-file-document-plus"
            icon-color="primary"
            tooltip="buttons.addAttachment"
            bottom
            iclass=""
            small
            @click="attachFile(item.item)"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            :icon-name="item.item.isManuallyActivated ? 'mdi-lock' : 'mdi-lock-open-variant'"
            icon-color="primary"
            :tooltip="item.item.isManuallyActivated ? 'docManagement.campaign.manuallyActivationOff' : 'docManagement.campaign.manuallyActivationOn'"
            bottom
            iclass=""
            small
            @click="toggleManuallyActivation(item.item)"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isUpcoming"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteCampaign(item.item)"
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
            @click="generateReportFile(item.item)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            v-if="hasManagePermission"
            small
            color="primary"
            :to="`/docManagement/campaign/create`"
          >
            {{ $t("buttons.newRecord") }}
          </v-btn>
        </button-group>
      </template>
    </grid>

    <confirm-dlg ref="confirm" />
    <v-dialog
      v-model="attachDialog"
      persistent
      max-width="800px"
    >
      <v-card>
        <v-card-title>
          <span class="headline">{{ $t('common.attachedFiles') }}</span>
        </v-card-title>
        <v-card-text>
          <file-manager
            v-model="attachments"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="blue darken-1"
            text
            @click="attachDialog = false"
          >
            {{ $t('buttons.cancel') }}
          </v-btn>
          <v-btn
            color="blue darken-1"
            text
            @click="saveAttachments"
          >
            {{ $t('buttons.save') }}
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
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";
import DocManagementAdditionalCampaignsList from '@/components/docManagement/DocManagementAdditionalCampaignsList.vue';
import Helper from "@/components/helper.js";
import FileManager from '@/components/common/FileManager.vue';

export default {
  name: 'DocManagementCampaigns',
  components: {
    Grid,
    DocManagementAdditionalCampaignsList,
    FileManager,
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      expanded: [],
      suppressMainReload: false, // for main campaigns grid
      attachDialog: false,
      attachments: [],
      currentCampaignId: null,
      headers: [
        {
          text: this.$t('docManagement.campaign.headers.name'),
          value: "name",
        },
        {
          text: this.$t('docManagement.campaign.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('docManagement.campaign.headers.fromDate'),
          value: "fromDate",
        },
        {
          text: this.$t('docManagement.campaign.headers.toDate'),
          value: "toDate",
        },
        {
          text: this.$t('docManagement.campaign.headers.isManuallyActivated'),
          value: "isManuallyActivated",
        },
        {
          text: '',
          value: "labels",
          filterable: false,
          sortable: false,
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
    };
  },
  computed: {
    ...mapGetters(["hasPermission"]),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementCampaignRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementCampaignManage);
    },
    hasApplicationManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage);
    },
    hasReportCreatePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementReportCreate);
    },
  },
  mounted() {
    if (!this.hasReadPermission){
      return this.$router.push("/errors/AccessDenied");
    }

    this.$studentHub.$on('doc-management-campaign-modified', this.onDocManagementCampaignModified);
  },
  destroyed() {
    this.$studentHub.$off('doc-management-campaign-modified');
  },
  methods: {
    gridReload() {
      const grid = this.$refs['docManagementCampaignsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },

    toggleManuallyActivation(item) {
      this.saving = true;
      // prevent hub broadcast from triggering full reload we already expect
      this.suppressMainReload = true;
      this.$api.docManagementCampaign.toggleManuallyActivation(item)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          // do a targeted refresh (just this grid) since hub reload will be skipped
          this.gridReload();
        })
        .catch(() => {
          this.$notifier.error('', this.$t('common.saveError'), 5000);
        })
        .finally(() => {
          this.saving = false;
          // release after next tick so hub handler sees the flag once
          this.$nextTick(() => { this.suppressMainReload = false; });
        });
    },
    async deleteCampaign(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;

      this.$api.docManagementCampaign.delete(item.id)
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
    itemRowBackground(item) {
      return item.isActive ? 'custom-grid-row left border-success' : '';
    },
    onDocManagementCampaignModified() {
      if (this.suppressMainReload) {
        return; // skip redundant reload
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
    async attachFile(item) {
      this.currentCampaignId = item.id;
      this.saving = true;
      try {
        const response = await this.$api.docManagementCampaign.getAttachments(item.id);
        this.attachments = response.data;
        this.attachDialog = true;
      } catch (e) {
        this.$notifier.error('', this.$t('common.loadError'), 5000);
      } finally {
        this.saving = false;
      }
    },
    async saveAttachments() {
      this.saving = true;
      try {
        const model = {
          campaignId: this.currentCampaignId,
          attachments: this.attachments
        };
        await this.$api.docManagementCampaign.saveAttachments(model);
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.attachDialog = false;
      } catch (e) {
        this.$notifier.error('', this.$t('common.saveError'), 5000);
      } finally {
        this.saving = false;
      }
    },
  }
};
</script>
